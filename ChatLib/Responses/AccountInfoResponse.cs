using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatLib.Responses
{
	public struct AccountInfoResponse : IResponse
	{
		private const ResponseType type = ResponseType.AccountInfo;
		private readonly long sessionID;
		public readonly ChatInfo[] simpleChats;
		public readonly ChatInfo[] groupChats;
		public AccountInfoResponse(ChatInfo[] simpleChats, ChatInfo[] groupChats, long sessionID)
		{
			if (simpleChats == null || groupChats == null) throw new ArgumentNullException();
			this.sessionID = sessionID;
			this.simpleChats = simpleChats;
			this.groupChats = groupChats;
		}
		public void Send(BinaryWriter writer)
		{
			lock (writer)
			{
				writer.Write(sessionID);
				writer.Write(type.ToString());

				writer.Write(simpleChats.Length);
				for (int i = 0; i < simpleChats.Length; i++)
				{
					simpleChats[i].Send(writer);
				}

				writer.Write(groupChats.Length);
				for (int i = 0; i < groupChats.Length; i++)
				{
					groupChats[i].Send(writer);
				}

				writer.Flush();
			}
		}
	}
}
