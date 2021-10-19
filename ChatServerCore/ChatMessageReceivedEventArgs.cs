using System;

namespace ChatServerCore
{
	public class ChatMessageReceivedEventArgs : EventArgs
	{
		public ChatAPI.Message Message { get; set; }

		public ChatMessageReceivedEventArgs(ChatAPI.Message message)
		{
			this.Message = message;
		}
	}
}