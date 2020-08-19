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
		public readonly ResponseType Type;
		public readonly long SessionID;
		protected Response(ResponseType Type, long sessionID)
		{
			this.Type = Type;
			this.SessionID = sessionID;
		}
		public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
	}
}
