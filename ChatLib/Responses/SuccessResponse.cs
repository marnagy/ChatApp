using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Responses
{
	public sealed class SuccessResponse : Response
	{
		const ResponseType type = ResponseType.Success;
		public SuccessResponse(long sessionID) : base(type, sessionID)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("SessionID", SessionID);
			info.AddValue("Type", Type);
		}
	}
}
