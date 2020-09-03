using ChatLib.Messages;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Responses
{
	[Serializable]
	public class DeleteMessageResponse : Response
	{
		private const ResponseType type = ResponseType.DeleteMessage;
		public readonly ChatType chatType;
		public readonly long chatID;
		public readonly DateTime dateTime;
		public DeleteMessageResponse(ChatType chatType, long chatID, DateTime dateTime, long sessionID) : base(type, sessionID)
		{
			this.chatType = chatType;
			this.chatID = chatID;
			this.dateTime = dateTime;
		}
		public DeleteMessageResponse(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
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
