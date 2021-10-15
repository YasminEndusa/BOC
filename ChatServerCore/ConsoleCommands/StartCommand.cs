using System;
using System.Collections.Generic;

namespace ChatServerCore.ConsoleCommands
{
	class StartCommand : ConsoleCommand
	{
		public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

		public const string COMMAND_START = "start";

		public StartCommand()
		{
			this.Command = COMMAND_START;
		}

		public override void Execute(List<KeyValuePair<string, string>> parameters)
		{
			ServerManager.Server = new ChatServer(ServerManager.Port);
			ServerManager.Server.ClientDisconnected += this.Server_ClientDisconnected;
		}

		private void Server_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
		{
			this.ClientDisconnected?.Invoke(sender, e);
		}
	}
}