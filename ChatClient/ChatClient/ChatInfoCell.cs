using ChatLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient
{
	public class ChatInfoCell
	{
		public ChatInfoCell(ChatInfo info)
		{
			Info = info;
		}
		public string Text {get => Info.Name;}
		public string Detail {get => Info.ID.ToString();}
		public ChatInfo Info {get; private set;}
	}
}
