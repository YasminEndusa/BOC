using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ChatAPI;
using Newtonsoft.Json;

namespace ChatServerCore
{
	public class ChatClient
	{
		public string Username { get; private set; }
		public Socket Socket { get; }
		public bool IsAuthenticated { get; set; }
		public string PublicKey { get; private set; }
		public string RijndaelKey { get; private set; }
		public string RijndaelIV { get; private set; }

		public IPAddress Address { get { return ((IPEndPoint)this.Socket.RemoteEndPoint).Address; } }

		public ChatClient(string _username, Socket socket)
		{
			this.Username = _username;
			this.Socket = socket;
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
	}
}