using System.Collections.Generic;

namespace ChatServerCore.ClientCommands
{
	public abstract class ClientCommand
	{
		public string Command;

		public ClientCommand()
		{
			this.Command = string.Empty;
		}

		public abstract void Execute(ChatClient client, List<KeyValuePair<string, string>> parameters);
	}
}