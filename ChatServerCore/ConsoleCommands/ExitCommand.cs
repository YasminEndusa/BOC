using System;
using System.Collections.Generic;
using System.Text;
using ChatAPI;

namespace ChatServerCore.ConsoleCommands
{
	class ExitCommand : ConsoleCommand
	{
		public const string COMMAND_EXIT = "exit";

		public ExitCommand()
		{
			this.Command = COMMAND_EXIT;
		}

		public override void Execute(List<KeyValuePair<string, string>> parameters)
		{
			if (ServerManager.Server != null)
			{
				ServerManager.Server.SendToAllClients(MessageType.ServerMessage, null, "Server closed, good bye!");
				ServerManager.Server.CloseAllClientSockets();
			}

			ServerManager.Running = false;
		}
	}
}