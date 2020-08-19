using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

namespace ChatLib
{
	[Serializable]
	public class ChatInfo : ISerializable
	{
		public readonly ChatType Type;
		public readonly string ID;
		public readonly Username[] participants;
		public ChatInfo(ChatType type, string chatID, Username[] participants)
		{
			if (participants == null) throw new ArgumentNullException();
			this.Type = type;
			this.ID = chatID;
			this.participants = participants;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ChatType", Type);
			info.AddValue("ChatID", ID);
			info.AddValue("Participants", participants);
		}
	}
}
