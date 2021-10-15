using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatClient
{
	public partial class MainForm : Form
	{
		private readonly ShowImageDialog _sid = new ShowImageDialog();

		private string _username;
		private string _password;

		private Client _client;

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
				this._client = new Client();
				this._client.NewMessage += this.Client_NewMessage;
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

				this._client.SendAuthMessage(this._username, this._password);
			}
			else
			{
				Environment.Exit(-1);
			}
		}

		private void Client_NewMessage(object sender, NewMessageEventArgs e)
		{
			this.pnlMessages.AddChatMessage(e.Username, e.Message, e.MessageType, e.Image);
		}

		private void SendButton_Click(object sender, EventArgs e)
		{
			string text = this.txtMessage.Text;

			if (!string.IsNullOrWhiteSpace(text))
			{
				this.txtMessage.Text = string.Empty;
				this._client.SendTextMessage(text);
			}

			_ = this.txtMessage.Focus();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this._client.ShutDown();
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
						this._client.SendImageMessage(img);
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