using ChatLib.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ChatClient
{
	public class TextMessageViewCell : ViewCell
	{
		private readonly TextMessage message;
		public string horizontalOpt {get;}
		public string Sender { get => message.SenderUsername.ToString(); }
		public string Text { get => message.text; }
		public TextMessageViewCell(TextMessage msg, string horizontalOpt)
		{
			this.horizontalOpt = horizontalOpt;
			message = msg;
		}
	}
}
