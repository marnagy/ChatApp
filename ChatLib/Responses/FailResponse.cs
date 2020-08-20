using ChatLib.BinaryFormatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Responses
{
	[Serializable]
	public class FailResponse : Response
	{
		private const string ReasonSerializationName = "Reason";
		private const ResponseType type = ResponseType.Fail;
		public readonly string Reason;
		public FailResponse(string reason, long sessionID) : base(type, sessionID)
		{
			this.Reason = reason;
		}
		public FailResponse(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			Reason = (string)info.GetValue( ReasonSerializationName, typeof(string));
		}

		public static FailResponse Read(BinaryFormatterReader reader, long sessionID)
		{
			return new FailResponse( (string)reader.Read(), sessionID);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);
			info.AddValue( ReasonSerializationName, Reason);
		}
	}
}
