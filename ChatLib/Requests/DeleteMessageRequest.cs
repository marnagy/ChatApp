using ChatLib.Messages;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Requests
{
	[Serializable]
	public class DeleteMessageRequest : Request
	{
		private const RequestType type = RequestType.DeleteMessage;
		public readonly ChatType chatType;
		public readonly long chatID;
		public readonly DateTime dateTime;

		public DeleteMessageRequest(ChatType chatType, long chatID, Message message,
			long sessionID) : base(type, sessionID)
		{
			this.chatType = chatType;
			this.chatID = chatID;
			this.dateTime = message.Datetime;
		}
		public DeleteMessageRequest(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			chatType = (ChatType)info.GetValue( ChatTypeSerializationName, typeof(ChatType));
			chatID = (long)info.GetValue( ChatIDSerializationName, typeof(long));
			dateTime = (DateTime)info.GetValue( DateTimeSerializationName, typeof(DateTime));
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);

			info.AddValue( ChatTypeSerializationName, chatType);
			info.AddValue( ChatIDSerializationName, chatID);
			info.AddValue( DateTimeSerializationName, dateTime);
		}
	}
}
