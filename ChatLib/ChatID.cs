using System;
using System.Collections.Generic;
using System.Text;

namespace ChatLib
{
	public struct ChatID
	{
		private string chatID;

		public ChatID(string chatID)
		{
			this.chatID = chatID;
		}
		public override string ToString()
		{
			return chatID;
		}
	}
}
