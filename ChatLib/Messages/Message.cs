using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Xamarin.Forms;

namespace ChatLib.Messages
{
	[Serializable]
	public abstract class Message : ISerializable
	{
		public readonly MessageType Type;
		public readonly long ChatID;
		public readonly Username SenderUsername;
		public readonly DateTime Datetime;

		public string sender {get => SenderUsername.ToString();}
		public Color bcgColor { get; set; } = Color.Aqua;
		public int column { get; set; } = 0;
		public LayoutOptions horizontalOpt { get; set; } = LayoutOptions.Start;
		public TextAlignment textAlignment { get; set; } = TextAlignment.Start;
		//public LayoutOptions horizontalOpt { get; set; } = "Start";

		// class specific variables
		public string hasText {get => (text.Length > 0).ToString(); }
		public string text { get; set; } = string.Empty;
		protected Message(MessageType type, long ChatID, Username sender)
		{
			this.Type = type;
			this.ChatID = ChatID;
			this.SenderUsername = sender;
			this.Datetime = DateTime.UtcNow;
		}
		protected Message( ValueTuple<MessageType, long, Username, DateTime> args )
		{
			Type = args.Item1;
			ChatID = args.Item2;
			SenderUsername = args.Item3;
			Datetime = args.Item4;
		}
		protected static (MessageType type, long chatID, Username sender, DateTime dt) LoadParentAttributes(SerializationInfo info, StreamingContext context)
		{
			MessageType type = (MessageType)info.GetValue( "Type", typeof(MessageType));
			long chatID = (long)info.GetValue( "chatID", typeof(long));
			Username sender = (Username)info.GetValue( "senderUserName", typeof(Username));
			DateTime dt = (DateTime)info.GetValue( "DateTime", typeof(DateTime));
			return (type, chatID, sender, dt);
		}

		public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
		public abstract Message UpdateMessageDateTime();
	}
}
