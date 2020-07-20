using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatLib.Responses
{
	public class FailResponse : IResponse
	{
		const ResponseType type = ResponseType.Fail;
		private readonly long sessionID;
		public readonly string Reason;
		public FailResponse(string reason, long sessionID)
		{
			this.sessionID = sessionID;
			this.Reason = reason;
		}
		public void Send(BinaryWriter writer)
		{
			lock (writer)
			{
				writer.Write(sessionID);
				writer.Write(type.ToString());
				writer.Write(Reason);
				writer.Flush();
			}
		}

		public static FailResponse Read(BinaryReader reader, long sessionID)
		{
			return new FailResponse(reader.ReadString(), sessionID);
		}
	}
}
