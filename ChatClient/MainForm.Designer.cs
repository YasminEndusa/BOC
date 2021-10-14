namespace ChatClient
{
	partial class MainForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.btnUploadFile = new ChatClient.ImageButton();
			this.btnUploadImage = new ChatClient.ImageButton();
			this.pnlMessages = new ChatClient.ChatMessagePanel();
			this.SuspendLayout();
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtMessage.Location = new System.Drawing.Point(72, 444);
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(620, 20);
			this.txtMessage.TabIndex = 1;
			this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtMessage_KeyDown);
			// 
			// btnSend
			// 
			this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSend.Location = new System.Drawing.Point(698, 442);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(75, 23);
			this.btnSend.TabIndex = 2;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.SendButton_Click);
			// 
			// btnUploadFile
			// 
			this.btnUploadFile.Location = new System.Drawing.Point(42, 441);
			this.btnUploadFile.Name = "btnUploadFile";
			this.btnUploadFile.Size = new System.Drawing.Size(24, 24);
			this.btnUploadFile.TabIndex = 6;
			this.btnUploadFile.UseVisualStyleBackColor = true;
			this.btnUploadFile.Click += new System.EventHandler(this.btnUploadFile_Click);
			// 
			// btnUploadImage
			// 
			this.btnUploadImage.Location = new System.Drawing.Point(12, 441);
			this.btnUploadImage.Name = "btnUploadImage";
			this.btnUploadImage.Size = new System.Drawing.Size(24, 24);
			this.btnUploadImage.TabIndex = 5;
			this.btnUploadImage.UseVisualStyleBackColor = true;
			this.btnUploadImage.Click += new System.EventHandler(this.btnUploadImage_Click);
			// 
			// pnlMessages
			// 
			this.pnlMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pnlMessages.AutoScroll = true;
			this.pnlMessages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlMessages.Location = new System.Drawing.Point(12, 12);
			this.pnlMessages.Name = "pnlMessages";
			this.pnlMessages.Size = new System.Drawing.Size(761, 424);
			this.pnlMessages.TabIndex = 4;
			this.pnlMessages.OpenImage += new System.EventHandler<ChatClient.OpenImageEventArgs>(this.pnlMessages_OpenImage);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(785, 477);
			this.Controls.Add(this.btnUploadFile);
			this.Controls.Add(this.btnUploadImage);
			this.Controls.Add(this.pnlMessages);
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.txtMessage);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "BO Chat";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.Button btnSend;
		private ChatMessagePanel pnlMessages;
		private ImageButton btnUploadImage;
		private ImageButton btnUploadFile;
	}
}