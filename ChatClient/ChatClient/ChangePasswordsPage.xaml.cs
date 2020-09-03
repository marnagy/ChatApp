using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatLib;
using ChatLib.BinaryFormatters;
using ChatLib.Requests;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChangePasswordsPage : ContentPage
	{
		private readonly Username myUsername;
		private readonly BinaryFormatterWriter writer;
		private readonly long sessionID;

		public ChangePasswordsPage(Username myUsername, BinaryFormatterWriter writer, long sessionID)
		{
			InitializeComponent();
			this.myUsername = myUsername;
			this.writer = writer;
			this.sessionID = sessionID;
		}

		private void ChangePasswordButton_Clicked(object sender, EventArgs e)
		{
			if ( oldPasswordEntry.Text.Length > 0 
				&& newPasswordEntry.Text.Length > 0
				&& newPasswordCheckEntry.Text.Length > 0
				)
			{
				if (newPasswordEntry.Text == newPasswordCheckEntry.Text)
				{
					//Task.Run( () =>
					//{
						writer.Write(new ChangePasswordRequest(myUsername, oldPasswordEntry.Text, newPasswordEntry.Text, sessionID) );
					//});
					Navigation.PopAsync(animated: true);
				}
				else
				{
					DisplayAlert("New password error", "New passwords do not match.", "Okay");
				}
			}
			else
			{
				DisplayAlert("Password format error", "Password cannot be empty text.", "Okay");
			}
		}
	}
}