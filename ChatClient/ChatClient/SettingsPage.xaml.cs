using ChatLib;
using ChatLib.BinaryFormatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		private readonly Username myUsername;
		private readonly BinaryFormatterWriter writer;
		private readonly long SessionID;

		public SettingsPage(Username myUsername, BinaryFormatterWriter writer, long sessionID)
		{
			InitializeComponent();
			this.myUsername = myUsername;
			this.writer = writer;
			SessionID = sessionID;
		}


		private void ChangePasswordButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync( new ChangePasswordsPage( myUsername, writer, SessionID) , animated: true);
		}
	}
}