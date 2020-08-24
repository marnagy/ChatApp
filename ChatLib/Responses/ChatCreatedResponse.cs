using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Responses
{
	[Serializable]
	public class ChatCreatedResponse : Response
	{
		private const ResponseType type = ResponseType.ChatCreated;
		public readonly ChatInfo Info;
		public ChatCreatedResponse(ChatInfo info, long sessionID) : base(type, sessionID)
		{
			Info = info;
		}
		public ChatCreatedResponse(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			Info = (ChatInfo)info.GetValue( ChatInfoSerializationName, typeof(ChatInfo));
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);
			info.AddValue( ChatInfoSerializationName, Info);
		}
	}
}
