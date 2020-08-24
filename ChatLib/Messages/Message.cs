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
		public readonly long ChatID;
		public readonly Username SenderUsername;
		public readonly DateTime Datetime;
		protected Message(MessageType type, long id, Username sender)
		{
			this.Type = type;
			this.ChatID = id;
			this.SenderUsername = sender;
			this.Datetime = DateTime.UtcNow;
		}
		protected Message( ValueTuple<MessageType, long, Username> args )
		{
			Type = args.Item1;
			ChatID = args.Item2;
			SenderUsername = args.Item3;
		}
		protected static (MessageType type, long chatID, Username sender) LoadParentAttributes(SerializationInfo info, StreamingContext context)
		{
			MessageType type = (MessageType)info.GetValue( "Type", typeof(MessageType));
			long chatID = (long)info.GetValue( "chatID", typeof(long));
			Username sender = (Username)info.GetValue( "senderUserName", typeof(Username));
			return (type, chatID, sender);
		}

		public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
	}
}
