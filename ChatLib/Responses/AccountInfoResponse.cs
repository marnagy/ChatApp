﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Responses
{
	public sealed class AccountInfoResponse : Response
	{
		private const ResponseType type = ResponseType.AccountInfo;
		public readonly ChatInfo[] SimpleChats;
		public readonly ChatInfo[] GroupChats;
		public AccountInfoResponse(ChatInfo[] simpleChats, ChatInfo[] groupChats,
			long sessionID) : base(type, sessionID)
		{
			if (simpleChats == null || groupChats == null) throw new ArgumentNullException();
			SimpleChats = simpleChats;
			GroupChats = groupChats;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("SessionID", SessionID);
			info.AddValue("Type", Type);
			info.AddValue("SimpleChats", SimpleChats);
			info.AddValue("GroupChats", GroupChats);
		}
	}
}
