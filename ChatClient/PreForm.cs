using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace ChatClient
{
	public partial class PreForm : Form
	{
		private bool loadingValuesFromSettings;

		private readonly Color colorGoodInput = Color.PaleGreen;
		private readonly Color colorBadInput = Color.MistyRose;

		public PreForm()
		{
			this.InitializeComponent();
		}

		private void PreForm_Load(object sender, EventArgs e)
		{
			this.loadingValuesFromSettings = true;
			this.txtServerAddress.Text = DataManager.Settings.ServerAddress;
			this.txtServerPort.Text = DataManager.Settings.ServerPort.ToString();
			this.loadingValuesFromSettings = false;

			if (DataManager.Settings.Validate())
			{
				this.btnOK.Enabled = true;

				this.txtServerAddress.BackColor = this.colorGoodInput;
				this.txtServerPort.BackColor = this.colorGoodInput;
			}
			else
			{
				if (!DataManager.Settings.ValidateServerAddress())
				{
					this.txtServerAddress.BackColor = this.colorBadInput;
				}
				else
				{
					this.txtServerAddress.BackColor = this.colorGoodInput;
				}

				if (!DataManager.Settings.ValidateServerPort())
				{
					this.txtServerPort.BackColor = this.colorBadInput;
				}
				else
				{
					this.txtServerPort.BackColor = this.colorGoodInput;
				}

				this.btnOK.Enabled = false;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			DataManager.SaveSettings();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void txtServerAddress_TextChanged(object sender, EventArgs e)
		{
			if (this.loadingValuesFromSettings)
			{
				return;
			}

			IPAddress ip;

			if (ValidationHelper.TryParseIPAddress(this.txtServerAddress.Text, out ip))
			{
				DataManager.Settings._serverAddress = ip;
				this.btnOK.Enabled = true;
				this.txtServerAddress.BackColor = this.colorGoodInput;
			}
			else
			{
				this.btnOK.Enabled = false;
				this.txtServerAddress.BackColor = this.colorBadInput;
			}
		}

		private void txtServerPort_TextChanged(object sender, EventArgs e)
		{
			if (this.loadingValuesFromSettings)
			{
				return;
			}

			int port;

			if (ValidationHelper.TryParsePort(this.txtServerPort.Text, out port))
			{
				DataManager.Settings.ServerPort = port;
				this.btnOK.Enabled = true;
				this.txtServerPort.BackColor = this.colorGoodInput;
			}
			else
			{
				this.btnOK.Enabled = false;
				this.txtServerPort.BackColor = this.colorBadInput;
			}
		}

		private void txtServerAddress_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}
		}

		private void txtServerPort_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
			{
				e.Handled = true;
			}
		}
	}
}
