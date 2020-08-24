using ChatLib;
using ChatLib.BinaryFormatters;
using ChatLib.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace ChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
		private const int sleepConst = 50;

		private readonly App app;

		private readonly ObservableCollection<ChatInfoCell> chatViews = new ObservableCollection<ChatInfoCell>();
		private readonly HashSet<Username> simpleChatUsernames = new HashSet<Username>();
		private readonly long sessionID;
		private readonly Username myUsername;
		private readonly BinaryFormatterWriter writer;

		public List<Username> NCUsernames;

		private readonly Thread ReadingThread;
		//private readonly List<ChatInfo> chats = new List<ChatInfo>();

		public HomePage(App app, Username username, BinaryFormatterWriter writer, BinaryFormatterReader reader, AccountInfoResponse aiResp, long sessionID)
		{
			this.app = app;
			myUsername = username;
			this.writer = writer;
			this.sessionID = sessionID;
			InitializeComponent();

			LoadChats(aiResp.SimpleChats, aiResp.GroupChats);
			listView.ItemsSource = chatViews;

			// not practical, but using lambda ensures noone can use this "method"
			// plus this has access to all variables without making them arguments

			//ReadingThread = new Thread(new ThreadStart(() =>
			//	{
			//		while (true)
			//		{
			//			Response resp = (Response)reader.Read();
			//			switch ( resp.Type )
			//			{
			//				case ResponseType.ChatCreated:
			//					ChatCreatedResponse CCResp = (ChatCreatedResponse)resp;
			//					chatViews.Insert(0, CCResp.Info);
			//					break;
							
			//				default:
			//					break;
			//			}
			//		}
			//	}));
			//ReadingThread.IsBackground = true;
			//ReadingThread.Start();
		}

		private void LoadChats(ChatInfo[] simpleChats, ChatInfo[] groupChats)
		{
			List<ChatInfo> chats = new List<ChatInfo>();
			if (simpleChats != null)
			{
				foreach (var chat in simpleChats)
				{
					chats.Add(chat);
				}
			}

			if (groupChats != null)
			{
				foreach (var chat in groupChats)
				{
					chats.Add(chat);
				}
			}

			chats.Sort((a,b) => DateTime.Compare(a.lastMessageTime, b.lastMessageTime));

			UpdateChatsView(chatViews, chats);
		}

		private void UpdateChatsView(ObservableCollection<ChatInfoCell> chatViews, List<ChatInfo> chats)
		{
			chatViews.Clear();

			foreach (var item in chats)
			{
				chatViews.Add( new ChatInfoCell(item) );
				//chatViews.Add( new TextCell{ Text = item.Name, Detail = item.ID.ToString() } );
			}
		}

		private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			lock (chatViews)
			{
				DisplayAlert("Cell tapped!", "Cell: " + chatViews[e.ItemIndex].Info.ID, "Okay");
			}
		}

		private void newChat_Click(object sender, EventArgs e)
		{
			/* CONTINUE DEVELOPMENT HERE */
			//await Navigation.PushAsync( new NewSimpleChatPage() );

			NCUsernames = new List<Username>(2);
			NCUsernames.Add(myUsername);
			var page = new NewSimpleChatPage(NCUsernames, writer, sessionID);

			Navigation.PushModalAsync( page, animated: true);
		}

		private void newGroupChat_Click(object sender, EventArgs e)
		{
			
		}
	}
}