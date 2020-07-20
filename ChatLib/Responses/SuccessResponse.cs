using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatLib.Responses
{
	public struct SuccessResponse : IResponse
	{
		const ResponseType type = ResponseType.Success;
		private readonly long sessionID;
		public SuccessResponse(long sessionID)
		{
			this.sessionID = sessionID;
		}
		public void Send(BinaryWriter writer)
		{
			lock (writer)
			{
				writer.Write(sessionID);
				writer.Write(type.ToString());
				writer.Flush();
			}
		}
	}
}
