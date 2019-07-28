using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerCore
{
	class Program
	{
		const string COMMAND_EXIT = "exit";
		const string CLIENTCOMMAND_SETNAME = "/setname ";
		const string CLIENTCOMMAND_DISCONNECT = "/disconnect";

		static readonly List<ChatClient> _clients = new List<ChatClient>();
		static readonly string _motd = "Welcome to Binary Overdrive Chat Server - Test Instance.";
		static bool running = true;

		static void Main(string[] args)
		{
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 15489);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Bind(endPoint);

			socket.Listen(1);

			Task.Run(() => {
				while (running)
				{
					Socket clientSocket = socket.Accept();
					string username = string.Format("[{0}]", (clientSocket.RemoteEndPoint as IPEndPoint).Address.ToString());
					ChatClient client = new ChatClient(username, clientSocket);
					_clients.Add(client);

					Task.Run(() => {
						SendToClient(clientSocket, _motd);
						ListenFromClient(client);
					});
				}
			});

			while (running)
			{
				string input = Console.ReadLine();
				if (input == COMMAND_EXIT)
				{
					SendToAllClients("Server closed, good bye!");
					CloseAllClientSockets();
					running = false;
				}
			}

			socket.Close();
			Console.WriteLine("Server has been closed!");
			Console.ReadKey();
		}

		private static void SendToClient(Socket clientSocket, string text)
		{
			clientSocket.Send(Encoding.ASCII.GetBytes(text));
		}

		private static void ListenFromClient(ChatClient client)
		{
			try
			{
				while (running)
				{
					byte[] buffer = new byte[1024];
					int received = client.Socket.Receive(buffer);

					Task.Run(() =>
					{
						if (received > 0)
						{
							string rawMessage = Encoding.ASCII.GetString(buffer, 0, received);
							string message = string.Format("{0}: {1}", client.Username, rawMessage);

							Console.WriteLine(message);

							if (rawMessage.StartsWith(CLIENTCOMMAND_SETNAME))
							{
								string oldName = client.Username;
								string newName = rawMessage.Remove(0, CLIENTCOMMAND_SETNAME.Length);
								client.SetUsername(newName);

								message = string.Format("~[{0} changed their name to {1}]", oldName, newName);
							}
							else if (rawMessage.StartsWith(CLIENTCOMMAND_DISCONNECT))
							{
								_clients.Remove(client);
								client.Socket.Close();

								message = string.Format("~[{0} disconnected]", client.Username);
							}

							if (message != null)
							{
								if (message.StartsWith("~"))
								{
									Console.WriteLine(message);
								}

								SendToAllClients(message);
							}
						}
					});
				}
			}
			catch (SocketException se)
			{
				_clients.Remove(client);
				Console.WriteLine("Error listening on client: " + client.Username + " (" + se.Message + "). They have been disconnected!");
			}
		}

		private static void SendToAllClients(string text)
		{
			Task.Run(() =>
			{
				Parallel.ForEach(_clients, (client) =>
				{
					client.Socket.Send(Encoding.ASCII.GetBytes(text));
				});
			});
		}

		private static void CloseAllClientSockets()
		{
			Task.Run(() =>
			{
				foreach (ChatClient chatClient in _clients)
				{
					chatClient.Socket.Close();
				}
			});
		}
	}
}