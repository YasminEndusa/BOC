using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServerCore.ConsoleCommands
{
	class ManageUserCommand : ConsoleCommand
	{
		public const string COMMAND_MANAGEUSER = "user";
		public const string SUBCOMMAND_ADDUSER = "add";
		public const string SUBCOMMAND_CHANGEPASSWD = "passwd";

		public ManageUserCommand()
		{
			this.Command = COMMAND_MANAGEUSER;
		}

		public override void Execute(List<KeyValuePair<string, string>> parameters)
		{
			if (parameters[0].Key == SUBCOMMAND_ADDUSER)
			{
				if (parameters.Count - 1 == 2)
				{
					string username = parameters[1].Key;
					string password = parameters[2].Key;
					password = Helpers.ComputeHash(password);
					DataManager.Users.Add(username, password);
					DataManager.SaveUsers();
				}
				else
				{
					Console.WriteLine("ERROR: Wrong amount of parameters. {0} requires 2 parameters. {1} parameters were provided.", SUBCOMMAND_ADDUSER, parameters.Count - 1);
				}
			}

			if (parameters[0].Key == SUBCOMMAND_CHANGEPASSWD)
			{
				if (parameters.Count - 1 == 2)
				{
					string username = parameters[1].Key;
					string password = parameters[2].Key;
					password = Helpers.ComputeHash(password);

					if (DataManager.Users.ContainsKey(username))
					{
						DataManager.Users[username] = password;
						DataManager.SaveUsers();
					}
					else
					{
						Console.WriteLine("ERROR: User '{0}' does not exist.", username);
					}
				}
				else
				{
					Console.WriteLine("ERROR: Wrong amount of parameters. {0} requires 2 parameters. {1} parameters were provided.", SUBCOMMAND_CHANGEPASSWD, parameters.Count - 1);
				}
			}
		}
	}
}
