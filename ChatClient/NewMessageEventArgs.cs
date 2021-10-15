using System.Drawing;
using ChatAPI;

namespace ChatClient
{
	internal class NewMessageEventArgs
	{
		public string Username { get; private set; }
		public string Message { get; private set; }
		public MessageType MessageType { get; private set; }
		public Image Image { get; private set; }

		public NewMessageEventArgs(string username, string message, MessageType messageType, Image image = null)
		{
			this.Username = username;
			this.Message = message;
			this.MessageType = messageType;
			this.Image = image;
		}
	}
}