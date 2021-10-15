using System;
using System.Collections.Generic;
using ChatAPI;

namespace ChatServerCore.ClientCommands
{
	class ManageUserCommand : ClientCommand
	{
		public const string COMMAND_MANAGEUSER = "user";
		public const string SUBCOMMAND_CHANGEPASSWD = "passwd";

		public ManageUserCommand()
		{
			this.Command = COMMAND_MANAGEUSER;
		}

		public override void Execute(ChatClient client, List<KeyValuePair<string, string>> parameters)
		{
			if (parameters[0].Key == SUBCOMMAND_CHANGEPASSWD)
			{
				if (parameters.Count - 1 == 1)
				{
					string username = client.Username;
					string password = parameters[2].Key;
					password = Helpers.ComputeHash(password);

					if (DataManager.Users.ContainsKey(username))
					{
						DataManager.Users[username] = password;
						DataManager.SaveUsers();
					}
					else
					{
						Message message = new Message(MessageType.Error, string.Format("ERROR: User '{0}' does not exist.", username));
						client.SendMessage(message);
					}
				}
				else
				{
					Message message = new Message(MessageType.Error, string.Format("ERROR: Wrong amount of parameters. {0} requires 1 parameters. {1} parameters were provided.", SUBCOMMAND_CHANGEPASSWD, parameters.Count - 1));
					client.SendMessage(message);
				}
			}
		}
	}
}