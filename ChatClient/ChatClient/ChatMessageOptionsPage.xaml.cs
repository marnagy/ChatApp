using ChatLib;
using ChatLib.BinaryFormatters;
using ChatLib.Messages;
using ChatLib.Requests;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatMessageOptionsPage : ContentPage
	{
		private readonly BinaryFormatterWriter writer;

		private readonly Message msg;
		private readonly ChatType chatType;
		private readonly Username myUsername;
		private readonly long sessionID;

		public ChatMessageOptionsPage(BinaryFormatterWriter writer, Message msg, ChatType chatType, Username myUsername, long sessionID)
		{
			InitializeComponent();

			senderLabel.Text = "Sent by: " + msg.SenderUsername.ToString();
			datetimeLabel.Text = "Delivered: " + msg.Datetime.ToString();

			this.writer = writer;
			this.msg = msg;
			this.chatType = chatType;
			this.myUsername = myUsername;
			this.sessionID = sessionID;
		}

		private void DeleteMessageButton_Clicked(object sender, EventArgs e)
		{
			writer.Write( new DeleteMessageRequest(chatType, msg.ChatID, msg, sessionID) );
			Navigation.PopAsync(animated: true);
		}
	}
}