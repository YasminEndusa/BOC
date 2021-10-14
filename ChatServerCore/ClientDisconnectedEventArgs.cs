using System;
using System.Net;

namespace ChatServerCore
{
	public class ClientDisconnectedEventArgs : EventArgs
	{
		public string Username { get; private set; }
		public IPAddress Address { get; private set; }
		public string Message { get; private set; }

		public ClientDisconnectedEventArgs(string username, IPAddress address, string message)
		{
			this.Username = username;
			this.Address = address;
			this.Message = message;
		}
	}
}