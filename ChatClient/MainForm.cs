using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatAPI;
using Newtonsoft.Json;

namespace ChatClient
{
	public partial class MainForm : Form
	{
		private static readonly RsaEncryption _clientEncryption = new RsaEncryption();
		private static readonly RijndaelEncryption _encryption = new RijndaelEncryption();

		private readonly ShowImageDialog _sid = new ShowImageDialog();

		private string _username;
		private string _password;
		private Socket _socket;
		private bool _running = true;

		public MainForm()
		{
			this.InitializeComponent();

			this.btnUploadImage.Image = ImageManager.ImageIcon;
			this.btnUploadFile.Image = ImageManager.FileIcon;
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			PreForm preForm = new PreForm();
			DialogResult dr = preForm.ShowDialog();

			if (dr != DialogResult.OK)
			{
				Environment.Exit(-1);
			}

			if (DataManager.Settings.Validate())
			{
				IPEndPoint endPoint = new IPEndPoint(DataManager.Settings._serverAddress, DataManager.Settings.ServerPort);
				this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this._socket.Connect(endPoint);
				this.Listen();

				ChatAPI.Message message = new ChatAPI.Message(MessageType.Connect, _clientEncryption.GetPublicKey());
				this.SendMessage(message, false);
			}
			else
			{
				_ = MessageBox.Show("Settings failure. Please check and validate the settings.");
				Environment.Exit(-1);
			}

			LoginForm loginForm = new LoginForm();
			DialogResult dialogResult = loginForm.ShowDialog();

			if (dialogResult == DialogResult.OK)
			{
				this._username = DataManager.Settings.Username;
				this._password = DataManager.Settings.Password;

				ChatAPI.Message authMessage = new ChatAPI.Message(MessageType.Authenticate, new KeyValuePair<string, string>(this._username, this._password));
				this.SendMessage(authMessage);
			}
			else
			{
				Environment.Exit(-1);
			}
		}

		private void Listen()
		{
			_ = Task.Run(() =>
			{
				while (this._running)
				{
					byte[] buffer = new byte[Constants.MAX_MESSSAGE_SIZE];
					SocketError socketError;
					int received = this._socket.Receive(buffer, 0, buffer.Length, SocketFlags.None, out socketError);

					if (socketError == SocketError.Success)
					{
						_ = Task.Run(() =>
						{
							if (received > 0)
							{
								ChatAPI.Message message = ReadMessage(buffer, received);

								if (message._messageType == MessageType.Connect)
								{
									string[] rijndaelValues = JsonConvert.DeserializeObject<string[]>(message._payload.ToString());
									_encryption.SetKeyFromString(rijndaelValues[0]);
									_encryption.SetVectorFromString(rijndaelValues[1]);
								}
								else if (message._messageType == MessageType.Authenticate)
								{
									if (message._payload.GetType() == typeof(bool))
									{
										if (!(bool)message._payload)
										{
											this.pnlMessages.AddChatMessage("ERROR", "Authentication failed", MessageType.Error);
										}
									}
								}
								else if (message._messageType == MessageType.ServerMessage || message._messageType == MessageType.TextMessage || message._messageType == MessageType.Error)
								{
									string user = message._sender;

									if (message._messageType == MessageType.ServerMessage)
									{
										user = "~Server~";
									}
									else if (message._messageType == MessageType.Error)
									{
										user = "ERROR";
									}

									this.pnlMessages.AddChatMessage(user, message._payload.ToString(), message._messageType);
								}
								else if(message._messageType == MessageType.Image)
								{
									string user = message._sender;
									Image image = ImageManager.ImageFromBase64(message._payload.ToString());
									this.pnlMessages.AddChatMessage(user, string.Empty, message._messageType, image);
								}
								else
								{
									this.pnlMessages.AddChatMessage("ERROR", string.Format("Unbekannter MessageType: {0}", message._messageType._messageType), MessageType.Error);
								}
							}
						});
					}
					else if (socketError == SocketError.ConnectionReset)
					{
						this._running = false;
						this.pnlMessages.AddChatMessage("ERROR", "Connection has been reset by remote host.", MessageType.Error);
					}
					else
					{
						this._running = false;
						this.pnlMessages.AddChatMessage("ERROR", "Unknown error. Connection has been terminated.", MessageType.Error);
					}
				}
			});
		}

		private static ChatAPI.Message ReadMessage(byte[] buffer, int received)
		{
			string rawMessage = Encoding.UTF8.GetString(buffer, 0, received);

			if (rawMessage.StartsWith(RsaEncryption.PREFIX_UNENCRYPTED))
			{
				rawMessage = rawMessage.Remove(0, RsaEncryption.PREFIX_UNENCRYPTED.Length);
			}
			else if (rawMessage.StartsWith(RsaEncryption.PREFIX_RSA))
			{
				rawMessage = rawMessage.Remove(0, RsaEncryption.PREFIX_RSA.Length);
				rawMessage = _clientEncryption.Decrypt(rawMessage);
			}
			else if (rawMessage.StartsWith(RijndaelEncryption.PREFIX_RIJNDAEL))
			{
				rawMessage = rawMessage.Remove(0, RijndaelEncryption.PREFIX_RIJNDAEL.Length);
				rawMessage = _encryption.Decrypt(rawMessage);
			}

			ChatAPI.Message message = JsonConvert.DeserializeObject<ChatAPI.Message>(rawMessage);
			return message;
		}

		private void SendButton_Click(object sender, EventArgs e)
		{
			string text = this.txtMessage.Text;

			if (!string.IsNullOrWhiteSpace(text))
			{
				this.txtMessage.Text = string.Empty;
				this.SendTextMessage(text);
			}

			_ = this.txtMessage.Focus();
		}

		private void SendTextMessage(string text)
		{
			ChatAPI.Message message = new ChatAPI.Message(MessageType.TextMessage, text);
			this.SendMessage(message);
		}

		private void SendImageMessage(Image img)
		{
			string payload = ImageManager.ImageToBase64(img);
			ChatAPI.Message message = new ChatAPI.Message(MessageType.Image, payload);
			this.SendMessage(message);
		}

		private void SendMessage(ChatAPI.Message message, bool encrypt = true)
		{
			string text = JsonConvert.SerializeObject(message);

			if (encrypt)
			{
				text = RijndaelEncryption.PREFIX_RIJNDAEL + _encryption.Encrypt(text);
			}
			else
			{
				text = RsaEncryption.PREFIX_UNENCRYPTED + text;
			}

			byte[] encodedText = Encoding.UTF8.GetBytes(text);

			if (encodedText.Length >= Constants.MAX_MESSSAGE_SIZE)
			{
				_ = MessageBox.Show("Image too big, max 1MB!");
			}

			_ = this._socket.Send(encodedText);
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

		private void btnUploadImage_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "Bilder|*.png;*.jpg;*.jpeg";
				ofd.Multiselect = false;

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					using (Image img = Image.FromFile(ofd.FileName))
					{
						this.SendImageMessage(img);
					}
				}
			}
		}

		private void btnUploadFile_Click(object sender, EventArgs e)
		{
			//TODO
		}

		private void pnlMessages_OpenImage(object sender, OpenImageEventArgs e)
		{
			this._sid.SetImage(e.Image);
			this._sid.Show();
		}
	}
}