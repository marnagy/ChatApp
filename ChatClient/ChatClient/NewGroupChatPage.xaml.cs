using ChatLib;
using ChatLib.BinaryFormatters;
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
	public partial class NewGroupChatPage : ContentPage
	{
		Username myUsername;
		BinaryFormatterWriter writer;
		long sessionID;
		private readonly ObservableCollection<string> collection = new ObservableCollection<string>();
		public NewGroupChatPage(Username myUsername, BinaryFormatterWriter writer, long sessionID)
		{
			this.myUsername = myUsername;
			this.writer = writer;
			this.sessionID = sessionID;

			InitializeComponent();
		}
		private void AddEntry(object sender, EventArgs e)
		{

		}
		private void RemoveEntry(object sender, EventArgs e)
		{

		}
		private void CreateGroupChat(object sender, EventArgs e)
		{
			var users = new List<Username>(collection.Count + 1);
			users[0] = myUsername;

			for (int i = 0; i < collection.Count; i++)
			{

			}
		}
	}
}