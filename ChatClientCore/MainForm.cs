using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClientCore
{
	public partial class MainForm : Form
	{
		private readonly string _username;
		private Socket _socket;
		private bool _running = true;

		public MainForm()
		{
			this.InitializeComponent();

			LoginForm loginForm = new LoginForm();
			DialogResult dialogResult = loginForm.ShowDialog();

			if(dialogResult == DialogResult.OK)
			{
				this._username = loginForm.GetUsername();
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.178.21"), 15489);
			this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this._socket.Connect(endPoint);
			this.Listen();
			this.SendMessage("/setname " + this._username);
		}

		private void Listen()
		{
			Task.Run(() =>
			{
				while (this._running)
				{
					byte[] buffer = new byte[1024];
					int received = this._socket.Receive(buffer);

					Task.Run(() =>
					{
						if (received > 0)
						{
							string rawMessage = Encoding.ASCII.GetString(buffer, 0, received);
							this.AddChatMessage(rawMessage);
						}
					});
				}
			});
		}

		private void SendButton_Click(object sender, EventArgs e)
		{
			string text = this.txtMessage.Text;

			if (!string.IsNullOrEmpty(text))
			{
				this.txtMessage.Text = string.Empty;
				this.SendMessage(text);
			}
		}

		private void SendMessage(string text)
		{
			this._socket.Send(Encoding.ASCII.GetBytes(text));
		}

		private void AddChatMessage(string message)
		{
			this.rtbChat.InvokeIfRequired(() =>
			{
				this.rtbChat.AppendText(string.Format("[{0:hh:MM:ss}] {1}{2}", DateTime.Now, message, Environment.NewLine));
			});
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this._running = false;
			this._socket.Close();
		}

		private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.btnSend.PerformClick();
			}
		}
	}
}