using System;

namespace ChatAPI
{
    public class Message
    {
        public string _sender = null;
        public MessageType _messageType;
        public object _payload = null;
        public Guid? _guid = null;

        public Message()
            : this(null, null)
        {
        }

        public Message(MessageType messageType, object payload = null)
            : this(null, messageType, payload)
        {
        }

        public Message(string sender, MessageType messageType, object payload = null)
        {
            this._sender = sender;
            this._messageType = messageType;
            this._payload = payload;
        }
    }
}