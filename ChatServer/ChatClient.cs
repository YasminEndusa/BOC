using System;
using System.Net.Sockets;

namespace ChatServer
{
	class ChatClient
	{
		private string _username;
		private readonly Socket _socket;

		public string Username
		{
			get
			{
				return this._username;
			}
		}

		public Socket Socket
		{
			get
			{
				return this._socket;
			}
		}

		public ChatClient(string _username, Socket socket)
		{
			this._username = _username;
			this._socket = socket;
		}

		internal void SetUsername(string newName)
		{
			this._username = newName;
		}
	}
}