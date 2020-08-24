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
		List<Username> list;
		BinaryFormatterWriter writer;
		long sessionID;
		
		public NewSimpleChatPage(List<Username> list, BinaryFormatterWriter writer, long sessionID)
		{
			this.list = list;
			this.writer = writer;
			this.sessionID = sessionID;

			InitializeComponent();
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			list.Add( usernameEntry.Text.ToUsername() );
			Navigation.PopModalAsync(animated: true);

			var req = new NewChatRequest(list[0], list[1], sessionID);
			writer.Write(req);
		}
	}
}