﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace ChatLib.Messages
{
	[Serializable]
	public sealed class TextMessage : Message
	{
		//private const string _XmlElementName = "TextMessage";
		private const MessageType type = MessageType.TextMessage;
		public TextMessage(Username sender, long chatID, string text) : base(type, chatID, sender)
		{
			this.text = text;
		}
		private TextMessage(Username sender, long chatID, string text, DateTime dateTime) : base( (type, chatID, sender, dateTime) )
		{
			this.text = text;
		}
		public TextMessage(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			text = (string)info.GetValue( "Text", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Type", Type);
			info.AddValue("chatID", ChatID);
			info.AddValue("senderUserName", SenderUsername);
			info.AddValue("DateTime", Datetime);

			info.AddValue("Text", text);
		}

		public override Message UpdateMessageDateTime()
		{  
			return new TextMessage(SenderUsername, ChatID, text, DateTime.Now);
		}
	}
}
