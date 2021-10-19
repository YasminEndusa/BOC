using System;
using System.Net;

namespace ChatServerCore
{
	public class ClientDisconnectedEventArgs : EventArgs
	{
		public ChatClient Client { get; private set; }
		public string Message { get; private set; }

		public ClientDisconnectedEventArgs(ChatClient client, string message)
		{
			this.Client = client;
			this.Message = message;
		}
	}
}