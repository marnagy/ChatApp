using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Responses
{
	[Serializable]
	public class OnlineContactsResponse : Response
	{
		private const ResponseType type = ResponseType.OnlineContacts;
		public readonly Username[] users;
		public OnlineContactsResponse(Username[] users, long sessionID) : base(type, sessionID)
		{
			this.users = users;
		}
		public OnlineContactsResponse(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			users = (Username[])info.GetValue( UsersSerializationName, typeof(Username[]));
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);
			info.AddValue( UsersSerializationName, users);
		}
	}
}