using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

namespace ChatLib.Requests
{
	[Serializable]
	public abstract class Request : ISerializable
	{
		protected const string SessionIDSerializationName = "SessionID";
		protected const string TypeSerializationName = "Type";
		protected const string EmailSerializationName = "E-mail";
		protected const string UsernameSerializationName = "Username";
		protected const string PasswordSerializationName = "Password";
		protected const string ParticipantsSerializationName = "Participants";

		public readonly RequestType Type;
		public readonly long SessionID;
		protected Request(RequestType Type, long sessionID)
		{
			this.Type = Type;
			this.SessionID = sessionID;
		}
		protected Request( ValueTuple<RequestType, long> args )
		{
			Type = args.Item1;
			SessionID = args.Item2;
		}
		protected static (RequestType type, long sessionID) LoadParentAttributes(SerializationInfo info, StreamingContext context)
		{
			var sessionID = (long)info.GetValue( SessionIDSerializationName, typeof(long));
			var type = (RequestType)info.GetValue( TypeSerializationName, typeof(RequestType));
			return (type, sessionID);
		}
		public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
	}
}
