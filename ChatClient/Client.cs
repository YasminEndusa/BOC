using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatAPI;
using Newtonsoft.Json;

namespace ChatClient
{
	internal class Client
	{
		public event EventHandler<NewMessageEventArgs> NewMessage;

		private static readonly RsaEncryption _clientEncryption = new RsaEncryption();
		private static readonly RijndaelEncryption _encryption = new RijndaelEncryption();

		private readonly Socket _socket;
		private bool _running = true;

		public Client()
		{
			IPEndPoint endPoint = new IPEndPoint(DataManager.Settings._serverAddress, DataManager.Settings.ServerPort);
			this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this._socket.Connect(endPoint);
			this.Listen();

			Message message = new Message(MessageType.Connect, _clientEncryption.GetPublicKey());
			this.SendMessage(message, false);
		}

		private void OnNewMessage(string username, string message, MessageType messageType, Image image = null)
		{
			this.NewMessage?.Invoke(this, new NewMessageEventArgs(username, message, messageType, image));
		}

		private void Listen()
		{
			_ = Task.Run(() =>
			{
				while (this._running)
				{
					SocketError socketError;
					byte[] buffer = new byte[4];
					int received = this._socket.Receive(buffer, 0, buffer.Length, SocketFlags.None, out socketError);

					if (socketError != SocketError.Success)
					{
						this.HandleSocketError(socketError);
						continue;
					}

					int length = BitConverter.ToInt32(buffer, 0);
					buffer = new byte[length];
					received = this._socket.Receive(buffer, 0, buffer.Length, SocketFlags.None, out socketError);

					if (socketError != SocketError.Success)
					{
						this.HandleSocketError(socketError);
						continue;
					}

					if (socketError == SocketError.Success)
					{
						_ = Task.Run(() =>
						{
							if (received > 0)
							{
								Message message = ReadMessage(buffer, received);

								if (message._messageType == MessageType.Connect)
								{
									string[] rijndaelValues = JsonConvert.DeserializeObject<string[]>(message._payload.ToString());
									_encryption.SetKeyFromString(rijndaelValues[0]);
									_encryption.SetVectorFromString(rijndaelValues[1]);
								}
								else if (message._messageType == MessageType.Authenticate)
								{
									if (message._payload.GetType() == typeof(bool))
									{
										if (!(bool)message._payload)
										{
											this.OnNewMessage("ERROR", "Authentication failed", MessageType.Error);
										}
									}
								}
								else if (message._messageType == MessageType.ServerMessage || message._messageType == MessageType.TextMessage || message._messageType == MessageType.Error)
								{
									string user = message._sender;

									if (message._messageType == MessageType.ServerMessage)
									{
										user = "~Server~";
									}
									else if (message._messageType == MessageType.Error)
									{
										user = "ERROR";
									}

									this.OnNewMessage(user, message._payload.ToString(), message._messageType);
								}
								else if (message._messageType == MessageType.Image)
								{
									string user = message._sender;
									Image image = ImageManager.ImageFromBase64(message._payload.ToString());
									this.OnNewMessage(user, string.Empty, message._messageType, image);
								}
								else
								{
									this.OnNewMessage("ERROR", string.Format("Unbekannter MessageType: {0}", message._messageType._messageType), MessageType.Error);
								}
							}
						});
					}
				}
			});
		}

		private void HandleSocketError(SocketError socketError)
		{
			this._running = false;

			if (socketError == SocketError.ConnectionReset)
			{
				this.OnNewMessage("ERROR", "Connection has been reset by remote host.", MessageType.Error);
			}
			else
			{
				this.OnNewMessage("ERROR", "Unknown error. Connection has been terminated.", MessageType.Error);
			}
		}

		private static Message ReadMessage(byte[] buffer, int received)
		{
			string rawMessage = Encoding.UTF8.GetString(buffer, 0, received);

			if (rawMessage.StartsWith(RsaEncryption.PREFIX_UNENCRYPTED))
			{
				rawMessage = rawMessage.Remove(0, RsaEncryption.PREFIX_UNENCRYPTED.Length);
			}
			else if (rawMessage.StartsWith(RsaEncryption.PREFIX_RSA))
			{
				rawMessage = rawMessage.Remove(0, RsaEncryption.PREFIX_RSA.Length);
				rawMessage = _clientEncryption.Decrypt(rawMessage);
			}
			else if (rawMessage.StartsWith(RijndaelEncryption.PREFIX_RIJNDAEL))
			{
				rawMessage = rawMessage.Remove(0, RijndaelEncryption.PREFIX_RIJNDAEL.Length);
				rawMessage = _encryption.Decrypt(rawMessage);
			}

			Message message = JsonConvert.DeserializeObject<Message>(rawMessage);
			return message;
		}

		public void SendTextMessage(string text)
		{
			Message message = new Message(MessageType.TextMessage, text);
			this.SendMessage(message);
		}

		public void SendImageMessage(Image img)
		{
			string payload = ImageManager.ImageToBase64(img);
			Message message = new Message(MessageType.Image, payload);
			this.SendMessage(message);
		}

		public void SendAuthMessage(string username, string password)
		{
			Message authMessage = new Message(MessageType.Authenticate, new KeyValuePair<string, string>(username, password));
			this.SendMessage(authMessage);
		}

		private void SendMessage(Message message, bool encrypt = true)
		{
			string text = JsonConvert.SerializeObject(message);

			if (encrypt)
			{
				text = RijndaelEncryption.PREFIX_RIJNDAEL + _encryption.Encrypt(text);
			}
			else
			{
				text = RsaEncryption.PREFIX_UNENCRYPTED + text;
			}

			byte[] encodedText = Encoding.UTF8.GetBytes(text);

			if (encodedText.Length >= Constants.MAX_MESSSAGE_SIZE)
			{
				throw new Exception("Image too big, max 10MB!");
			}

			byte[] length = BitConverter.GetBytes(encodedText.Length);
			byte[] final = length.Concat(encodedText).ToArray();
			_ = this._socket.Send(final);
		}

		public void ShutDown()
		{
			this._running = false;

			if (this._socket != null && this._socket.Connected)
			{
				this._socket.Close();
			}
		}
	}
}