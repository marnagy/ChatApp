using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Responses
{
	[Serializable]
	public sealed class SuccessResponse : Response
	{
		const ResponseType type = ResponseType.Success;
		public SuccessResponse(long sessionID) : base(type, sessionID)
		{
		}
		public SuccessResponse(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);
		}
	}
}
