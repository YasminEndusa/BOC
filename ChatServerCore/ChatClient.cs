using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatAPI;
using Newtonsoft.Json;

namespace ChatServerCore
{
	public class ChatClient
	{
		public event EventHandler<ClientDisconnectedEventArgs> Disconnected;
		public event EventHandler<ChatMessageReceivedEventArgs> ChatMessageReceived;

		private readonly RsaEncryption ServerEncryption;

		public string Username { get; private set; }
		public Socket Socket { get; }
		public bool IsAuthenticated { get; set; }
		public string PublicKey { get; private set; }
		public string RijndaelKey { get; private set; }
		public string RijndaelIV { get; private set; }

		public IPAddress Address { get { return ((IPEndPoint)this.Socket.RemoteEndPoint).Address; } }

		public ChatClient(string _username, Socket socket, RsaEncryption serverEncryption)
		{
			this.Username = _username;
			this.Socket = socket;
			this.ServerEncryption = serverEncryption;
		}

		internal void SetUsername(string newName)
		{
			this.Username = newName;
		}

		internal void SetPublicKey(string key)
		{
			this.PublicKey = key;
		}

		internal void SetRijndaelValues(string key, string iv)
		{
			this.RijndaelKey = key;
			this.RijndaelIV = iv;
		}

		public void ListenFrom()
		{
			try
			{
				while (ServerManager.Running && this.Socket.Connected)
				{
					SocketError socketError;
					byte[] buffer = new byte[4];
					int received = this.Socket.Receive(buffer, 0, buffer.Length, SocketFlags.None, out socketError);

					if (socketError != SocketError.Success)
					{
						this.HandleSocketError(socketError);
						continue;
					}

					int length = BitConverter.ToInt32(buffer);

					if (length > Constants.MAX_MESSSAGE_SIZE)
					{
						_ = Task.Run(() =>
						{
							Message m = new Message(MessageType.Error, "Message too long, please send shorter messages/smaller images/files.");
							this.SendMessage(m);
						});

						continue;
					}

					buffer = new byte[length];
					received = this.Socket.Receive(buffer, 0, buffer.Length, SocketFlags.None, out socketError);

					if (socketError != SocketError.Success)
					{
						this.HandleSocketError(socketError);
						continue;
					}

					_ = Task.Run(() =>
					{
						try
						{
							if (received > 0)
							{
								Message message = this.ReadMessage(buffer, received);
								this.HandleMessage(message);
							}
						}
						catch (Exception)
						{
							Message m = new Message(MessageType.Error, "Error receiving your message. Please try again!");
							this.SendMessage(m);
						}
					});
				}
			}
			catch (Exception ex)
			{				
				this.OnDisconnected(ex.Message);
			}
		}

		private void HandleSocketError(SocketError socketError)
		{
			throw new Exception(string.Format("SocketError: {0}", socketError.ToString()));
		}

		private void OnDisconnected(string message)
		{
			this.Disconnected?.Invoke(this, new ClientDisconnectedEventArgs(this, message));
		}

		public void SendMessage(Message message, bool rsa = false, bool rijndael = true)
		{
			string text = JsonConvert.SerializeObject(message);

			if (rsa)
			{
				using (RsaEncryption enc = new RsaEncryption(this.PublicKey))
				{
					text = RsaEncryption.PREFIX_RSA + enc.Encrypt(text);
				}
			}
			else if (rijndael)
			{
				using (RijndaelEncryption enc = new RijndaelEncryption(this.RijndaelKey, this.RijndaelIV))
				{
					text = RijndaelEncryption.PREFIX_RIJNDAEL + enc.Encrypt(text);
				}
			}
			else
			{
				text = RsaEncryption.PREFIX_UNENCRYPTED + text;
			}

			byte[] buffer = Encoding.UTF8.GetBytes(text);
			byte[] length = BitConverter.GetBytes(buffer.Length);
			byte[] final = length.Concat(buffer).ToArray();
			_ = this.Socket.Send(final);
		}

		private void HandleMessage(Message message)
		{
			if (message._messageType == MessageType.Connect)
			{
				string clientPublicKey = message._payload.ToString();
				this.SetPublicKey(clientPublicKey);

				using (RijndaelEncryption encryption = new RijndaelEncryption())
				{
					this.SetRijndaelValues(encryption.GetKeyString(), encryption.GetVectorString());
				}

				Message answer = new Message(MessageType.Connect, new string[] { this.RijndaelKey, this.RijndaelIV });
				this.SendMessage(answer, true, false);
			}
			else if (message._messageType == MessageType.Authenticate)
			{
				KeyValuePair<string, string> payload = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(message._payload.ToString());
				string username = payload.Key;
				string password = payload.Value;

				Message answer = new Message(MessageType.Authenticate, false);

				if (AuthenticateUser(username, password))
				{
					this.IsAuthenticated = true;
					this.SetUsername(username);
					answer._payload = true;
				}

				this.SendMessage(answer);
			}
			else if (message._messageType == MessageType.TextMessage)
			{
				if (!this.IsAuthenticated)
				{
					Message errorMessage = new Message(MessageType.Error, "Client is not authenticated!");
					this.SendMessage(errorMessage);
				}
				else
				{
					string rawMessage = message._payload.ToString();
					string output = string.Format("{0}", rawMessage);

					if (output != null)
					{
						if (output.StartsWith('/'))
						{
							ClientCommands.ClientCommandManager.HandleCommandInput(this, output);
						}
						else
						{
							string sender = this.Username;
							MessageType messageType = MessageType.TextMessage;
							Message messageOut = new Message(sender, messageType, output);

							this.OnChatMessageReceived(messageOut);
						}
					}
				}
			}
			else if (message._messageType == MessageType.Image || message._messageType == MessageType.Blob)
			{
				message._sender = this.Username;
				this.OnChatMessageReceived(message);
			}
		}

		private void OnChatMessageReceived(Message message)
		{
			this.ChatMessageReceived?.Invoke(this, new ChatMessageReceivedEventArgs(message));
		}

		private Message ReadMessage(byte[] buffer, int received)
		{
			string text = Encoding.UTF8.GetString(buffer, 0, received);

			if (text.StartsWith(RsaEncryption.PREFIX_UNENCRYPTED))
			{
				text = text.Remove(0, RsaEncryption.PREFIX_UNENCRYPTED.Length);
			}
			else if (text.StartsWith(RsaEncryption.PREFIX_RSA))
			{
				text = text.Remove(0, RsaEncryption.PREFIX_RSA.Length);
				text = this.ServerEncryption.Decrypt(text);
			}
			else if (text.StartsWith(RijndaelEncryption.PREFIX_RIJNDAEL))
			{
				text = text.Remove(0, RijndaelEncryption.PREFIX_RIJNDAEL.Length);

				using (RijndaelEncryption enc = new RijndaelEncryption(this.RijndaelKey, this.RijndaelIV))
				{
					text = enc.Decrypt(text);
				}
			}

			Message message = JsonConvert.DeserializeObject<Message>(text);
			return message;
		}

		private static bool AuthenticateUser(string username, string password)
		{
			password = Helpers.ComputeHash(password);

			return DataManager.Users.ContainsKey(username) && DataManager.Users[username] == password;
		}
	}
}