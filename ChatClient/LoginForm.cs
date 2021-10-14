using System;
using System.Windows.Forms;

namespace ChatClient
{
	public partial class LoginForm : Form
	{
		private const char PASSWORDCHAR_NONE = '\u0000';
		private const char PASSWORDCHAR = '\u25cf';

		public LoginForm()
		{
			this.InitializeComponent();
			this.txtPassword.PasswordChar = PASSWORDCHAR;
		}

		private void LoginForm_Load(object sender, EventArgs e)
		{
			this.txtUsername.Text = DataManager.Settings.Username;
			this.txtPassword.Text = DataManager.Settings.Password;
		}

		private void LoginButton_Click(object sender, EventArgs e)
		{
			if(!string.IsNullOrEmpty(this.txtUsername.Text) && !string.IsNullOrEmpty(this.txtPassword.Text))
			{
				DataManager.Settings.Username = this.txtUsername.Text;
				DataManager.Settings.Password = this.txtPassword.Text;
				DataManager.SaveSettings();

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			else
			{
				_ = MessageBox.Show("Invalid Username or password!");
			}
		}

		private void ckbShowPassword_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ckbShowPassword.Checked)
			{
				this.txtPassword.PasswordChar = PASSWORDCHAR_NONE;
			}
			else
			{
				this.txtPassword.PasswordChar = PASSWORDCHAR;
			}
		}
	}
}