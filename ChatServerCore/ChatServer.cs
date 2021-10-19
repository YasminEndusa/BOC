using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatAPI;

namespace ChatServerCore
{
	public class ChatServer
	{
		public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

		readonly RsaEncryption _encryption = new RsaEncryption();
		readonly List<ChatClient> _clients = new List<ChatClient>();
		readonly Socket _socket = null;
		readonly int _port;

		public ChatServer(int port)
		{
			Console.WriteLine("Server starting up, please wait...");
			this._port = port;

			IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, this._port);
			this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this._socket.Bind(endPoint);

			this._socket.Listen(1);

			Console.WriteLine("Server started up successfully.");
			_ = Task.Run(() =>
			{
				while (ServerManager.Running)
				{
					Socket clientSocket = this._socket.Accept();
					string username = string.Format("[{0}]", (clientSocket.RemoteEndPoint as IPEndPoint).Address.ToString());
					ChatClient client = new ChatClient(username, clientSocket, this._encryption);
					client.Disconnected += this.Client_Disconnected;
					client.ChatMessageReceived += this.Client_ChatMessageReceived;
					this._clients.Add(client);

					_ = Task.Run(() =>
					{
						client.ListenFrom();
					});
				}
			});
		}

		private void Client_ChatMessageReceived(object sender, ChatMessageReceivedEventArgs e)
		{
			this.SendToAllClients(e.Message);
		}

		private void Client_Disconnected(object sender, ClientDisconnectedEventArgs e)
		{
			_ = this._clients.Remove(e.Client);
		}

		public void SendToAllClients(Message message)
		{
			_ = Task.Run(() =>
			{
				_ = Parallel.ForEach(this._clients, (client) =>
				{
					client.SendMessage(message);
				});
			});
		}

		public void CloseAllClientSockets()
		{
			_ = Task.Run(() =>
			{
				foreach (ChatClient chatClient in this._clients)
				{
					chatClient.Socket.Close();
				}
			});
		}

		public void CloseServer()
		{
			if (this._socket != null && this._socket.IsBound)
			{
				this._socket.Close();
			}

			this._encryption.Dispose();
		}
	}
}