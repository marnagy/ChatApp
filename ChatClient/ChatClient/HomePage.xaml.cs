using ChatLib;
using ChatLib.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace ChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
		private readonly App app;

		private readonly ObservableCollection<ChatInfo> chatViews = new ObservableCollection<ChatInfo>();
		private readonly HashSet<Username> simpleChatUsernames = new HashSet<Username>();
		private readonly long sessionID;
		private readonly Username myUsername;
		//private readonly List<ChatInfo> chats = new List<ChatInfo>();

		public HomePage(App app, Username username, AccountInfoResponse aiResp, long sessionID)
		{
			this.app = app;
			this.sessionID = sessionID;
			myUsername = username;
			InitializeComponent();

			LoadChats(aiResp.SimpleChats, aiResp.GroupChats);
			listView.ItemsSource = chatViews;
		}

		private void LoadChats(ChatInfo[] simpleChats, ChatInfo[] groupChats)
		{
			List<ChatInfo> chats = new List<ChatInfo>();
			foreach (var chat in simpleChats)
			{
				chats.Add(chat);
			}

			foreach (var chat in groupChats)
			{
				chats.Add(chat);
			}

			chats.Sort((a,b) => DateTime.Compare(a.lastMessageTime, b.lastMessageTime));

			UpdateChatsView(chatViews, chats);
		}

		private void UpdateChatsView(ObservableCollection<ChatInfo> chatViews, List<ChatInfo> chats)
		{
			chatViews.Clear();

			foreach (var item in chats)
			{
				chatViews.Add( item );
				//chatViews.Add( new TextCell{ Text = item.Name, Detail = item.ID.ToString() } );
			}
		}

		private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			lock (chatViews)
			{
				DisplayAlert("Cell tapped!", "Cell: " + chatViews[e.ItemIndex].ID, "Okay");
			}
		}

		private async void newChat_Click(object sender, EventArgs e)
		{
			/* CONTINUE DEVELOPMENT HERE */
			await Navigation.PushAsync( new NewSimpleChatPage() );
		}

		private void newGroupChat_Click(object sender, EventArgs e)
		{
			
		}
	}
}