using System.Net;
using System.Net.Sockets;

namespace ChatServerCore
{
	class ChatClient
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
	}
}