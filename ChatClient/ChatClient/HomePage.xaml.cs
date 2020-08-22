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

		private readonly ObservableCollection<TextCell> chatViews = new ObservableCollection<TextCell>();
		private readonly List<ChatInfo> chats = new List<ChatInfo>();

		public HomePage(App app, AccountInfoResponse aiResp)
		{
			this.app = app;
			InitializeComponent();

			LoadChats(aiResp.SimpleChats, aiResp.GroupChats);
			listView.ItemsSource = chats;
		}

		private void LoadChats(ChatInfo[] simpleChats, ChatInfo[] groupChats)
		{
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

		private void UpdateChatsView(ObservableCollection<TextCell> chatViews, List<ChatInfo> chats)
		{
			chatViews.Clear();

			foreach (var item in chats)
			{
				chatViews.Add( new TextCell{ Text = item.Name, Detail = item.ID.ToString() } );
			}
		}

		private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			lock (chats)
			{
				DisplayAlert("Cell tapped!", "Cell: " + chats[e.ItemIndex].ID, "Okay");
			}
		}

		private void newChat_Click(object sender, EventArgs e)
		{

		}

		private void newGroupChat_Click(object sender, EventArgs e)
		{

		}
	}
}