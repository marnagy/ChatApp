using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace ChatLib.Messages
{
	public sealed class TextMessage : Message
	{
		//private const string _XmlElementName = "TextMessage";
		private const MessageType type = MessageType.TextMessage;
		
		public readonly string Text;

		public TextMessage(Username sender, ChatID id, string text) : base(type, id, sender)
		{
			this.Text = text;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Type", Type);
			info.AddValue("senderUserName", SenderUsername);
			info.AddValue("chatID", ID);
			info.AddValue("Text", Text);
		}
	}
}
