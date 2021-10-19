using System;
using System.Net;
using ChatServerCore.ConsoleCommands;

namespace ChatServerCore
{
	class Program
	{
		static int _port = 15489;

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

			ServerManager.Port = _port;
			ServerManager.Running = true;

			((StartCommand)ConsoleCommandManager.GetCommand(StartCommand.COMMAND_START)).ClientDisconnected += ClientDisconnected;

			while (ServerManager.Running)
			{
				Console.Write(">>> ");
				string input = Console.ReadLine();

				if (input.StartsWith('/'))
				{
					ConsoleCommandManager.HandleCommandInput(input);
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

		private static void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
		{
			Console.WriteLine("Error listening on client: '{0}', IP '{1}'. They have been disconnected! ({2})", e.Client.Username, e.Client.Address, e.Message);
			Console.Write(">>> ");
		}
	}
}