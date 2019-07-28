using System;
using System.Windows.Forms;

namespace ChatClient
{
	public partial class LoginForm : Form
	{
		private string _username;

		public LoginForm()
		{
			this.InitializeComponent();
		}

		private void LoginButton_Click(object sender, EventArgs e)
		{
			if(!string.IsNullOrEmpty(this.txtUsername.Text))
			{
				this._username = this.txtUsername.Text;
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			else
			{
				MessageBox.Show("Invalid Username!");
			}
		}

		public string GetUsername()
		{
			return this._username;
		}
	}
}