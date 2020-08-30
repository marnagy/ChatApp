using ChatLib;
using ChatLib.BinaryFormatters;
using ChatLib.Messages;
using ChatLib.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatPage : ContentPage
	{
		private readonly App app;
		private readonly ChatInfo info;
		private readonly Username myUsername;
		private readonly BinaryFormatterWriter writer;
		private readonly long sessionID;
		private readonly ObservableCollection<Message> messages;
		public readonly ListView listView;
		public ChatPage(App app, Username user, ChatInfo info, BinaryFormatterWriter writer, long sessionID)
		{
			if ( info == null || writer == null ) throw new ArgumentNullException();
			this.app = app;
			this.myUsername = user;
			this.info = info;
			this.writer = writer;
			this.sessionID = sessionID;

			InitializeComponent();
			messages = info.GetMessages();
			listView = list;
			list.ItemsSource = messages;
			//list.ChildAdded += ScrollDown;
		}
		private void ScrollDown(object sender, EventArgs e)
		{
			list.ScrollTo(messages[messages.Count - 1], ScrollToPosition.MakeVisible, animated: true);
		}

		private void button_Clicked(object sender, EventArgs e)
		{
			if ( editor.Text.Length > 0)
			{
				string text = editor.Text.Trim();
				editor.Text = string.Empty;
				Task.Run(() => 
					writer.Write( new NewMessageRequest(info.Type, info.ID, new TextMessage(myUsername, 0, text), sessionID) )
				);
			}
		}

		private void list_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			//var msg = e.Item as Message;
			//DisplayAlert("Item tapped", msg.text, "OKAY");
		}

		private void ScrollDown(object sender, ElementEventArgs e)
		{
			var listView = e.Element as ListView;
			//listView.ScrollTo(messages[messages.Count - 1], ScrollToPosition.MakeVisible, animated: true);

		}
	}
}