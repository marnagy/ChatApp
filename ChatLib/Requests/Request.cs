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
		public readonly RequestType Type;
		public readonly long SessionID;
		protected Request(RequestType Type, long sessionID)
		{
			this.Type = Type;
			this.SessionID = sessionID;
		}
		public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
	}
}
