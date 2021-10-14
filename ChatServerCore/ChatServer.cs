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
	public class ChatServer
	{
		public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

		readonly static RsaEncryption _encryption = new RsaEncryption();
		readonly List<ChatClient> _clients = new List<ChatClient>();
		readonly Socket _socket = null;
		readonly int _port;
		readonly bool _running = false;

		public ChatServer(int port)
		{
			Console.WriteLine("Server starting up, please wait...");
			this._port = port;

			IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, this._port);
			this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this._socket.Bind(endPoint);

			this._socket.Listen(1);
			this._running = true;

			Console.WriteLine("Server started up successfully.");
			_ = Task.Run(() =>
			{
				while (this._running)
				{
					Socket clientSocket = this._socket.Accept();
					string username = string.Format("[{0}]", (clientSocket.RemoteEndPoint as IPEndPoint).Address.ToString());
					ChatClient client = new ChatClient(username, clientSocket);
					this._clients.Add(client);

					_ = Task.Run(() =>
					{
						this.ListenFromClient(client);
					});
				}
			});
		}

		private void ListenFromClient(ChatClient client)
		{
			try
			{
				while (this._running)
				{
					byte[] buffer = new byte[4];
					int received = client.Socket.Receive(buffer);
					int length = BitConverter.ToInt32(buffer);

					if (length > Constants.MAX_MESSSAGE_SIZE)
					{
						_ = Task.Run(() =>
						{
							Message m = new Message(MessageType.Error, "Message too long, please send shorter messages/smaller images/files.");
							SendToClient(client, m);
						});

						continue;
					}

					buffer = new byte[length];
					received = client.Socket.Receive(buffer);

					_ = Task.Run(() =>
					{
						if (received > 0)
						{
							Message message = ReadMessage(client, buffer, received);
							this.HandleMessage(client, message);
						}
					});
				}
			}
			catch (SocketException se)
			{
				_ = this._clients.Remove(client);
				this.OnClientDisconnected(client.Username, client.Address, se.Message);
			}
		}

		private void OnClientDisconnected(string username, IPAddress address, string message)
		{
			this.ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(username, address, message));
		}

		private void HandleMessage(ChatClient client, Message message)
		{
			if (message._messageType == MessageType.Connect)
			{
				string clientPublicKey = message._payload.ToString();
				client.SetPublicKey(clientPublicKey);

				using (RijndaelEncryption encryption = new RijndaelEncryption())
				{
					client.SetRijndaelValues(encryption.GetKeyString(), encryption.GetVectorString());
				}

				Message answer = new Message(MessageType.Connect, new string[] { client.RijndaelKey, client.RijndaelIV });
				SendToClient(client, answer, true, false);
			}
			else if (message._messageType == MessageType.Authenticate)
			{
				KeyValuePair<string, string> payload = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(message._payload.ToString());
				string username = payload.Key;
				string password = payload.Value;

				Message answer = new Message(MessageType.Authenticate, false);

				if (AuthenticateUser(username, password))
				{
					client.IsAuthenticated = true;
					client.SetUsername(username);
					answer._payload = true;
				}

				SendToClient(client, answer);
			}
			else if (message._messageType == MessageType.TextMessage)
			{
				if (!client.IsAuthenticated)
				{
					Message errorMessage = new Message(MessageType.Error, "Client is not authenticated!");
					SendToClient(client, errorMessage);
				}
				else
				{
					string rawMessage = message._payload.ToString();
					string output = string.Format("{0}", rawMessage);

					if (output != null)
					{
						if (output.StartsWith('/'))
						{
							//TODO command handling
							//FIXME temporary solution to change password
							if (output.StartsWith("/passwd"))
							{
								output = output.Remove(0, "/passwd".Length + 1);
								output = Helpers.ComputeHash(output);
								DataManager.Users[client.Username] = output;
								DataManager.SaveUsers();
							}
						}
						else
						{
							string sender = client.Username;
							MessageType messageType = MessageType.TextMessage;

							this.SendToAllClients(messageType, sender, output);
						}
					}
				}
			}
			else if (message._messageType == MessageType.Image || message._messageType == MessageType.Blob)
			{
				message._sender = client.Username;
				this.SendToAllClients(message);
			}
		}

		private static Message ReadMessage(ChatClient client, byte[] buffer, int received)
		{
			string text = Encoding.UTF8.GetString(buffer, 0, received);

			if (text.StartsWith(RsaEncryption.PREFIX_UNENCRYPTED))
			{
				text = text.Remove(0, RsaEncryption.PREFIX_UNENCRYPTED.Length);
			}
			else if (text.StartsWith(RsaEncryption.PREFIX_RSA))
			{
				text = text.Remove(0, RsaEncryption.PREFIX_RSA.Length);
				text = _encryption.Decrypt(text);
			}
			else if (text.StartsWith(RijndaelEncryption.PREFIX_RIJNDAEL))
			{
				text = text.Remove(0, RijndaelEncryption.PREFIX_RIJNDAEL.Length);

				using (RijndaelEncryption enc = new RijndaelEncryption(client.RijndaelKey, client.RijndaelIV))
				{
					text = enc.Decrypt(text);
				}
			}

			Message message = JsonConvert.DeserializeObject<Message>(text);
			return message;
		}

		private static void SendToClient(ChatClient client, Message message, bool rsa = false, bool rijndael = true)
		{
			string text = JsonConvert.SerializeObject(message);

			if (rsa)
			{
				using (RsaEncryption enc = new RsaEncryption(client.PublicKey))
				{
					text = RsaEncryption.PREFIX_RSA + enc.Encrypt(text);
				}
			}
			else if (rijndael)
			{
				using (RijndaelEncryption enc = new RijndaelEncryption(client.RijndaelKey, client.RijndaelIV))
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
			_ = client.Socket.Send(final);
		}

		private static bool AuthenticateUser(string username, string password)
		{
			password = Helpers.ComputeHash(password);

			return DataManager.Users.ContainsKey(username) && DataManager.Users[username] == password;
		}

		public void SendToAllClients(MessageType messageType, string sender, object payload)
		{
			Message message = new Message(sender, messageType, payload);
			this.SendToAllClients(message);
		}

		private void SendToAllClients(Message message)
		{
			_ = Task.Run(() =>
			{
				_ = Parallel.ForEach(this._clients, (client) =>
				{
					SendToClient(client, message);
				});
			});
		}

		public void CloseAllClientSockets()
		{
			_ = Task.Run(() =>
			{
				foreach (ChatClient chatClient in this._clients)
				{
					chatClient.Socket.Close();
				}
			});
		}

		public void CloseServer()
		{
			if (this._socket != null && this._socket.IsBound)
			{
				this._socket.Close();
			}

			_encryption.Dispose();
		}
	}
}