using System;
using System.Drawing;
using System.Windows.Forms;
using ChatAPI;

namespace ChatClient
{
	public class ChatMessagePanel : Panel
	{
		public event EventHandler<OpenImageEventArgs> OpenImage;

		public ChatMessagePanel()
		{
			this.VScroll = true;
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			for (int i = 0; i < this.Controls.Count; i++)
			{
				ChatMessageControl msgCtrl = (ChatMessageControl)this.Controls[i];
				//TODO height/location, because resizing might cause text wrapping to change
				msgCtrl.Width = this.ClientSize.Width;
				msgCtrl.Location = new Point(msgCtrl.Location.X, msgCtrl.Location.Y);
				msgCtrl.Invalidate();
			}
		}

		public void AddChatMessage(string username, string message, MessageType messageType, Image image = null)
		{
			_ = this.Invoke(new Action(() =>
			{
				Color chatColor = Color.Black;

				if (messageType == MessageType.ServerMessage)
				{
					chatColor = Color.Purple;
				}
				else if (messageType == MessageType.Error)
				{
					chatColor = Color.Red;
				}

				int locY = 0;

				if (this.Controls.Count > 0)
				{
					Control msgControl = this.Controls[this.Controls.Count - 1];
					locY = msgControl.Bottom;
				}

				string displayMessage = string.Format("[{0:HH:mm:ss}] {1}", DateTime.Now, message);
				ChatMessageControl chatMessageControl = new ChatMessageControl(username, displayMessage, image);
				chatMessageControl.OpenImage += this.ChatMessageControl_OpenImage;
				chatMessageControl.Width = this.ClientSize.Width;
				chatMessageControl.Location = new Point(0, locY);

				this.Controls.Add(chatMessageControl);
				this.ScrollControlIntoView(chatMessageControl);
			}));
		}

		private void ChatMessageControl_OpenImage(object sender, OpenImageEventArgs e)
		{
			this.OpenImage?.Invoke(this, e);
		}
	}
}
