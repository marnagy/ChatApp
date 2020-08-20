using ChatLib.Responses;
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
	public partial class HomePage : ContentPage
	{
		private readonly App app;
		public HomePage(App app, AccountInfoResponse aiResp)
		{
			this.app = app;
			InitializeComponent();
		}
	}
}