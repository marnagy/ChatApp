using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Messages
{
	[Serializable]
	public abstract class Message : ISerializable
	{
		public readonly MessageType Type;
		public readonly ChatID ID;
		public readonly Username SenderUsername;
		public readonly DateTime Datetime;
		protected Message(MessageType type, ChatID id, Username sender)
		{
			this.Type = type;
			this.ID = id;
			this.SenderUsername = sender;
			this.Datetime = DateTime.UtcNow;
		}

		public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
	}
}
