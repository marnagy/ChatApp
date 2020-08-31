using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ChatLib;

namespace ChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OnlineContactsPage : ContentPage
	{
		//private ObservableCollection<StringCell> onlineContacts;

		public OnlineContactsPage(ObservableCollection<StringCell> onlineContacts)
		{
			//this.onlineContacts = onlineContacts;
			InitializeComponent();
			listView.ItemsSource = onlineContacts;
		}
	}
}