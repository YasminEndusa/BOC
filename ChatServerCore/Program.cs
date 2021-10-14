using System;
using System.Net;
using ChatAPI;

namespace ChatServerCore
{
	class Program
	{
		static readonly string[] _commands = { CommandManager.COMMAND_START, CommandManager.COMMAND_EXIT, CommandManager.COMMAND_ADDUSER, CommandManager.COMMAND_CHANGEUSERPASS };

		static int _port = 15489;
		static bool _running = true;

		static void Main(string[] args)
		{
			if (args.Length == 1)
			{
				int tmpPort;

				if (int.TryParse(args[0], out tmpPort))
				{
					if (tmpPort >= IPEndPoint.MinPort && tmpPort <= IPEndPoint.MaxPort)
					{
						_port = tmpPort;
					}
					else
					{
						Console.WriteLine("Port has to be between {0} and {1} (given {2})!", IPEndPoint.MinPort, IPEndPoint.MaxPort, tmpPort);
						_ = Console.ReadKey();
						Environment.Exit(-1);
					}
				}
			}

			Console.WriteLine(string.Format("Please use one of the following commands: {0}", string.Join(", ", _commands)));

			while (_running)
			{
				Console.Write(">>> ");
				string input = Console.ReadLine();

				if (input == CommandManager.COMMAND_EXIT)
				{
					if (ServerManager.Server != null)
					{
						ServerManager.Server.SendToAllClients(MessageType.ServerMessage, null, "Server closed, good bye!");
						ServerManager.Server.CloseAllClientSockets();
					}
					
					_running = false;
				}

				if (input.StartsWith(CommandManager.COMMAND_START))
				{
					ServerManager.Server = new ChatServer(_port);
					ServerManager.Server.ClientDisconnected += Server_ClientDisconnected;
				}

				if (input.StartsWith(CommandManager.COMMAND_ADDUSER))
				{
					string inputs = input.Remove(0, CommandManager.COMMAND_ADDUSER.Length + 1);
					string[] inputsArray = inputs.Split(' ');

					if (inputsArray.Length == 2)
					{
						string username = inputsArray[0];
						string password = inputsArray[1];
						password = Helpers.ComputeHash(password);
						DataManager.Users.Add(username, password);
						DataManager.SaveUsers();
					}
					else
					{
						Console.WriteLine("ERROR: Wrong amount of parameters. {0} requires 2 parameters. {1} parameters were provided.", CommandManager.COMMAND_ADDUSER, inputsArray.Length);
					}
				}

				if (input.StartsWith(CommandManager.COMMAND_CHANGEUSERPASS))
				{
					string inputs = input.Remove(0, CommandManager.COMMAND_CHANGEUSERPASS.Length + 1);
					string[] inputsArray = inputs.Split(' ');

					if (inputsArray.Length == 2)
					{
						string username = inputsArray[0];
						string password = inputsArray[1];
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
						Console.WriteLine("ERROR: Wrong amount of parameters. {0} requires 2 parameters. {1} parameters were provided.", CommandManager.COMMAND_CHANGEUSERPASS, inputsArray.Length);
					}
				}
			}

			if (ServerManager.Server != null)
			{
				ServerManager.Server.CloseServer();
				ServerManager.Server = null;
			}

			Console.WriteLine("Server has been closed!");
			_ = Console.ReadKey();
		}

		private static void Server_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
		{
			Console.WriteLine("Error listening on client: '{0}', IP '{1}'. They have been disconnected! ({2})", e.Username, e.Address, e.Message);
		}
	}
}