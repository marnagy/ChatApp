using ChatLib;
using ChatLib.BinaryFormatters;
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
	public partial class NewGroupChatPage : ContentPage
	{
		Username myUsername;
		BinaryFormatterWriter writer;
		long sessionID;
		private readonly ObservableCollection<StringCell> collection = new ObservableCollection<StringCell>();
		public NewGroupChatPage(Username myUsername, BinaryFormatterWriter writer, long sessionID)
		{
			this.myUsername = myUsername;
			this.writer = writer;
			this.sessionID = sessionID;

			collection.Add( new StringCell(){ value = string.Empty} );
			collection.Add( new StringCell(){ value = string.Empty} );

			InitializeComponent();
			list.ItemsSource = collection;
		}
		private void AddEntry(object sender, EventArgs e)
		{
			collection.Add( new StringCell(){ value = string.Empty} );
		}
		private void RemoveEntry(object sender, EventArgs e)
		{
			collection.RemoveAt(collection.Count - 1);
		}
		private void CreateGroupChat(object sender, EventArgs e)
		{
			var users = new List<Username>(collection.Count);
			Navigation.PopModalAsync(animated: true);

			Task.Run( () => {
				string val;
				for (int i = 0; i < collection.Count; i++)
				{
					val = collection[i].value.Trim();
					if ( val.Length == 0)
					{
						Device.InvokeOnMainThreadAsync( () => DisplayAlert("Error", "No username has empty username.", "Okay") );
						users = null;
						return;
					}
					users.Add( val.ToUsername() );
				}

				writer.Write( new NewChatRequest( myUsername, users.ToArray(), sessionID) );
			});
		}
	}
}