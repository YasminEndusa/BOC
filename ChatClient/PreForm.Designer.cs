namespace ChatClient
{
	partial class PreForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblServerAddress = new System.Windows.Forms.Label();
			this.lblServerPort = new System.Windows.Forms.Label();
			this.txtServerAddress = new System.Windows.Forms.TextBox();
			this.txtServerPort = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(174, 67);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "Connect";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(255, 67);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// lblServerAddress
			// 
			this.lblServerAddress.AutoSize = true;
			this.lblServerAddress.Location = new System.Drawing.Point(12, 15);
			this.lblServerAddress.Name = "lblServerAddress";
			this.lblServerAddress.Size = new System.Drawing.Size(82, 13);
			this.lblServerAddress.TabIndex = 4;
			this.lblServerAddress.Text = "Server Address:";
			// 
			// lblServerPort
			// 
			this.lblServerPort.AutoSize = true;
			this.lblServerPort.Location = new System.Drawing.Point(12, 41);
			this.lblServerPort.Name = "lblServerPort";
			this.lblServerPort.Size = new System.Drawing.Size(63, 13);
			this.lblServerPort.TabIndex = 5;
			this.lblServerPort.Text = "Server Port:";
			// 
			// txtServerAddress
			// 
			this.txtServerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtServerAddress.Location = new System.Drawing.Point(100, 12);
			this.txtServerAddress.Name = "txtServerAddress";
			this.txtServerAddress.Size = new System.Drawing.Size(230, 20);
			this.txtServerAddress.TabIndex = 6;
			this.txtServerAddress.TextChanged += new System.EventHandler(this.txtServerAddress_TextChanged);
			this.txtServerAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtServerAddress_KeyPress);
			// 
			// txtServerPort
			// 
			this.txtServerPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtServerPort.Location = new System.Drawing.Point(100, 38);
			this.txtServerPort.Name = "txtServerPort";
			this.txtServerPort.Size = new System.Drawing.Size(230, 20);
			this.txtServerPort.TabIndex = 7;
			this.txtServerPort.TextChanged += new System.EventHandler(this.txtServerPort_TextChanged);
			this.txtServerPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtServerPort_KeyPress);
			// 
			// PreForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(342, 102);
			this.Controls.Add(this.txtServerPort);
			this.Controls.Add(this.txtServerAddress);
			this.Controls.Add(this.lblServerPort);
			this.Controls.Add(this.lblServerAddress);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Name = "PreForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "BO Chat";
			this.Load += new System.EventHandler(this.PreForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblServerAddress;
		private System.Windows.Forms.Label lblServerPort;
		private System.Windows.Forms.TextBox txtServerAddress;
		private System.Windows.Forms.TextBox txtServerPort;
	}
}