﻿using ChatLib.Messages;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Responses
{
	[Serializable]
	public class AddMessageResponse : Response
	{
		private const ResponseType type = ResponseType.AddMessage;
		public readonly ChatType chatType;
		public readonly long chatID;
		public readonly Message message;
		public AddMessageResponse(ChatType chatType, long chatID, Message message, long sessionID) : base(type, sessionID)
		{
			this.chatType = chatType;
			this.chatID = chatID;
			this.message = message;
		}
		public AddMessageResponse(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			chatType = (ChatType)info.GetValue( ChatTypeSerializationName, typeof(ChatType));
			chatID = (long)info.GetValue( ChatIDSerializationName, typeof(long));
			message = (Message)info.GetValue( MessageSerializationName, typeof(Message));
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);

			info.AddValue( ChatTypeSerializationName, chatType);
			info.AddValue( ChatIDSerializationName, chatID);
			info.AddValue( MessageSerializationName, message);
		}
	}
}
