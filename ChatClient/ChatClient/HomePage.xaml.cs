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

		private ObservableCollection<ChatViewCell> chats = new ObservableCollection<ChatViewCell>();

		public HomePage(App app, AccountInfoResponse aiResp)
		{
			this.app = app;
			InitializeComponent();

			LoadChats(aiResp.SimpleChats, aiResp.GroupChats);
		}

		private void LoadChats(ChatInfo[] simpleChats, ChatInfo[] groupChats)
		{
			throw new NotImplementedException();
		}

		private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{

		}
	}
}