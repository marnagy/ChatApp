using ChatLib.BinaryFormatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Requests
{
	[Serializable]
	public class OnlineContactsRequest : Request
	{
		public const RequestType type = RequestType.GetOnlineContacts;
		public readonly Username username;
		//public readonly long sessionID;
		public OnlineContactsRequest(Username username, long sessionID) : base(type, sessionID)
		{
			this.username = username;
		}
		public OnlineContactsRequest(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			username = (Username)info.GetValue( UsernameSerializationName, typeof(Username));
		}
		public static OnlineContactsRequest Read(BinaryFormatterReader reader, long sessionID)
		{
			Username username = ((string)reader.Read()).ToUsername();
			return new OnlineContactsRequest(username, sessionID);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);
			info.AddValue( UsernameSerializationName, username);
		}
	}
}