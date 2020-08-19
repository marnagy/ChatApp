using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Responses
{
	public class FailResponse : Response
	{
		private const ResponseType type = ResponseType.Fail;
		public readonly string Reason;
		public FailResponse(string reason, long sessionID) : base(type, sessionID)
		{
			this.Reason = reason;
		}

		public static FailResponse Read(BinaryReader reader, long sessionID)
		{
			return new FailResponse(reader.ReadString(), sessionID);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("SessionID", SessionID);
			info.AddValue("Type", Type);
			info.AddValue("Reason", Reason);
		}
	}
}
