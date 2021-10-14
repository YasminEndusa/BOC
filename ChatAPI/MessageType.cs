namespace ChatAPI
{
	public class MessageType
	{
		public static MessageType Connect = new MessageType(0);
		public static MessageType Authenticate = new MessageType(1);
		public static MessageType TextMessage = new MessageType(2);
		public static MessageType Error = new MessageType(3);
		public static MessageType ServerMessage = new MessageType(4);
		public static MessageType Image = new MessageType(5);
		public static MessageType Blob = new MessageType(6);

		public int _messageType;

		public MessageType(int messageType)
		{
			this._messageType = messageType;
		}

		public static bool operator ==(MessageType messageType1, MessageType messageType2)
		{
			return messageType1.Equals(messageType2);
		}

		public static bool operator !=(MessageType messageType1, MessageType messageType2)
		{
			return !messageType1.Equals(messageType2);
		}

		public override bool Equals(object obj)
		{
			return obj.GetType() == typeof(MessageType) && ((MessageType)obj)._messageType == this._messageType;
		}

		public override int GetHashCode()
		{
			return this._messageType.GetHashCode();
		}
	}
}