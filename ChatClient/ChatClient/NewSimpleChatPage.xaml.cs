using ChatLib;
using ChatLib.BinaryFormatters;
using ChatLib.Requests;
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
	public partial class NewSimpleChatPage : ContentPage
	{
		Username myUsername;
		BinaryFormatterWriter writer;
		long sessionID;
		
		public NewSimpleChatPage( Username myUsername, BinaryFormatterWriter writer, long sessionID)
		{
			this.myUsername = myUsername;
			this.writer = writer;
			this.sessionID = sessionID;

			InitializeComponent();
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			if (usernameEntry.Text.Trim().Length > 0)
			{
				var usr = usernameEntry.Text.Trim();
				Navigation.PopModalAsync(animated: true);
				Task.Run( () => {
					var req = new NewChatRequest(myUsername, usr.ToUsername(), sessionID);
					writer.Write(req);
				});
			}
		}
	}
}