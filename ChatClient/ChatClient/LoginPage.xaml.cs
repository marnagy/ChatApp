using ChatLib;
using ChatLib.BinaryFormatters;
using ChatLib.Requests;
using ChatLib.Responses;
using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		private const int defaultPort = 5318;
		private const string hostname = "10.0.2.2";

		private const string ConnectionErrorTitle = "Server was not reached.";
		private const string ConnectionErrorMessage = "Server appears to be unavailable.\nPlease, try again later.";

		private const string ResponseErrorTitle = "Server' response not correct.";
		private const string ResponseErrorMessage = "Server appears to have malfunctioned.\nPlease, try again.";

		//private const string SessionErrorTitle = "Connection problem";
		//private const string SessionErrorMessage = "Server appears to be unavailable.\nPlease, try again later.";
		//private const string SessionErrorCancel = "OK";

		//private const string EmailFormatErrorTitle = "Email format failure.";
		private const string EmailFormatErrorMessage = "Your provided email address does not have correct format.\nPlease, try again.";

		private const string DefaultCancel = "OK";

		private const string FormatErrorTitle = "Format not satisfied";

		private const int lengthCond = 7;

		private const string UsernameFormatErrorMessage = "Username has to have at least 7 characters with no spaces.";
		private const string PasswordFormatErrorMessage = "Username has to have at least 7 characters with no spaces.";

		private const string AccountCreateSuccessTitle = "New account has been created.";
		private const string AccountCreateSuccessMessage = "You can now sign in.";

		private const string AccountCreateFailTitle = "New account has NOT been created.";

		private const string SignInFailTitle = "Failed to log in.";
		//private const string AccountCreateFailMessage = "You can now sign in.";

		private readonly App app;
		private TcpClient client;
		private BinaryFormatterWriter writer;
		private BinaryFormatterReader reader;
		private long sessionID;

		//private string emailLabelText = "E-mail";
		//private string emailBtnPlaceHolder = "Enter e-mail";

		//private string usernameLabelText = "Username";
		//private string usernameBtnPlaceHolder = "Enter username";

		//private const string passwordLabelText = "Password";
		//private const string passwordBtnPlaceHolder = "Enter password";

		bool creatingAccount = false;
		bool isLoading = false;
		public LoginPage(App app)
		{
			this.app = app;
			InitializeComponent();
			client = Connection.Connect(hostname, defaultPort, ref writer, ref reader);
			sessionID = (long)reader.Read();
		}

		private void newAccountBtn_Clicked(object sender, EventArgs e)
		{
			loginBtn.IsVisible = false;
			emailEntry.IsVisible = true;
			emailLabel.IsVisible = true;
			btnGrid.IsVisible = false;
			btnStack.IsVisible = true;
		}

		private void loginBtn_Clicked(object sender, EventArgs e)
		{
			indicator.IsVisible = true;
			indicator.IsRunning = true;
			//Task.Run(() => {
				try{
					if (!client.Connected)
					{
						client = Connection.Connect(hostname, defaultPort, ref writer, ref reader);
						sessionID = (long)reader.Read();
					}
					var username = usernameEntry.Text.Trim().ToUsername();
					var password = passwordEntry.Text.ToPassword();

					SignInRequest req = new SignInRequest(username, password, sessionID);
					writer.Write(req);
					//req.Send(writer);

					Response resp = (Response)reader.Read();
					if ( resp.SessionID == sessionID )
					{
						//switch ((ResponseType)Enum.Parse(typeof(ResponseType), (string)reader.Read()))
						switch ( resp.Type )
						{
						case ResponseType.AccountInfo:
							AccountInfoResponse AIResp = (AccountInfoResponse)resp; //.Read(reader, sessionID);
							var homePage = new NavigationPage(new HomePage(app, username, writer, reader, AIResp, sessionID) );
							//Device.BeginInvokeOnMainThread(() => {
								//indicator.IsVisible = false;
								//indicator.IsRunning = false;
								app.MainPage = homePage;
							//});
							break;
						case ResponseType.Fail:
								FailResponse FResp = (FailResponse)resp; //.Read(reader, sessionID);
								DisplayAlert(SignInFailTitle, FResp.Reason, DefaultCancel);
								break;
							default:
								throw new InvalidResponseException();
						}
					}
					else
					{
						Connection.Disconnect(ref client, ref reader, ref writer);
					}
				}
				catch(SocketException)
				{
					DisplayAlert(ConnectionErrorTitle, ConnectionErrorMessage, DefaultCancel);
				}
				catch(InvalidResponseException)
				{

					//Task.Run( () =>
						DisplayAlert(ResponseErrorTitle, ResponseErrorMessage, DefaultCancel);
					//);
				}
				finally
				{
					indicator.IsVisible = false;
					indicator.IsRunning = false;
				}
			//});
		}

		private void createAccountBtn_Clicked(object sender, EventArgs e)
		{
			indicator.IsVisible = true;
			indicator.IsRunning = true;
			//Task.Run(() => {
				try{
					if (!client.Connected)
					{
						client = Connection.Connect(hostname, defaultPort, ref writer, ref reader);
						sessionID = (long)reader.Read();
					}
					NewAccountRequest req = default;
					var email = emailEntry.Text.Trim();
					var username = usernameEntry.Text.Trim();
					var passwd = passwordEntry.Text;
					//if (username.Length <= 7)
					//{
					//	DisplayAlert(FormatErrorTitle, UsernameFormatErrorMessage, ConnectionErrorCancel);
					//	return;
					//}
					//if (passwd.Length <= 7)
					//{
					//	DisplayAlert(FormatErrorTitle, PasswordFormatErrorMessage, ConnectionErrorCancel);
					//	return;
					//}
					req = new NewAccountRequest(email.ToEmail(),
						username.ToUsername(), passwd.ToPassword(), sessionID);
					writer.Write(req);
					//req.Send(writer);

					Response resp = (Response)reader.Read();
					if ( resp.SessionID == sessionID)
					{
						//switch ((ResponseType)Enum.Parse(typeof(ResponseType), (string)reader.Read()))
						switch (resp.Type)
						{
							case ResponseType.Success:
								loginBtn.IsVisible = true;
								emailEntry.IsVisible = false;
								emailLabel.IsVisible = false;
								btnGrid.IsVisible = true;
								btnStack.IsVisible = false;

								emailEntry.Text = "";
								usernameEntry.Text = "";
								passwordEntry.Text = "";
								DisplayAlert(AccountCreateSuccessTitle, AccountCreateSuccessMessage, DefaultCancel);
								break;
							case ResponseType.Fail:
								FailResponse FResp = (FailResponse)resp; //.Read(reader, sessionID);
								DisplayAlert(AccountCreateFailTitle, FResp.Reason, DefaultCancel);
								break;
						}
					}
					else
					{
						Connection.Disconnect(ref client, ref reader, ref writer);
					}
				}
				catch(SocketException)
				{
					DisplayAlert(ConnectionErrorTitle, ConnectionErrorMessage, DefaultCancel);
				}
				catch (FormatException)
				{
					DisplayAlert(FormatErrorTitle, EmailFormatErrorMessage, DefaultCancel);
				}
				//catch (InvalidSessionIDException)
				//{
				//	DisplayAlert(FormatErrorTitle, EmailFormatErrorMessage, EmailFormatErrorCancel);
				//}
				finally
				{
					
				}
			//});
		}
	}
}