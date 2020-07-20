using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatLib
{
	public class ChatInfo : ISendable
	{
		public readonly ChatType type;
		public readonly string chatID;
		public readonly Username[] participants;
		public ChatInfo(ChatType type, string chatID, Username[] participants)
		{
			if (participants == null) throw new ArgumentNullException();
			this.type = type;
			this.chatID = chatID;
			this.participants = participants;
		}

		public void Send(BinaryWriter writer)
		{
			lock (writer)
			{
				writer.Write(type.ToString());
				writer.Write(chatID);
				writer.Write(participants.Length);
				foreach (var username in participants)
				{
					writer.Write(username.ToString());
				}

				writer.Flush();
			}
		}
	}
}
