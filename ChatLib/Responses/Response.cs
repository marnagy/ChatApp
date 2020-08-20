using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using ChatLib.Requests;

namespace ChatLib.Responses
{
	[Serializable]
	public abstract class Response : ISerializable
	{
		protected const string SessionIDSerializationName = "SessionID";
		protected const string TypeSerializationName = "Type";
		protected const string EmailSerializationName = "E-mail";
		protected const string UsernameSerializationName = "Username";
		protected const string PasswordSerializationName = "Password";
		public readonly ResponseType Type;
		public readonly long SessionID;
		protected Response(ResponseType Type, long sessionID)
		{
			this.Type = Type;
			this.SessionID = sessionID;
		}
		protected Response( ValueTuple<ResponseType, long> args )
		{
			Type = args.Item1;
			SessionID = args.Item2;
		}
		protected static (ResponseType type, long sessionID) LoadParentAttributes(SerializationInfo info, StreamingContext context)
		{
			var sessionID = (int)info.GetValue( SessionIDSerializationName, typeof(int));
			var type = (ResponseType)info.GetValue( TypeSerializationName, typeof(ResponseType));
			return (type, sessionID);
		}
		public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
	}
}
