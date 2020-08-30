using ChatLib;
using ChatLib.BinaryFormatters;
using ChatLib.Responses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace ChatClient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
		private const int sleepConst = 50;

		private readonly App app;

		private readonly ObservableCollection<ChatInfoCell> chatViews = new ObservableCollection<ChatInfoCell>();
		private readonly Dictionary<(ChatType, long), ChatInfo> chats = new Dictionary<(ChatType, long), ChatInfo>();
		private readonly Dictionary<(ChatType, long), ChatPage> pages = new Dictionary<(ChatType, long), ChatPage>();
		private readonly HashSet<Username> simpleChatUsernames = new HashSet<Username>();
		private readonly long sessionID;
		private readonly Username myUsername;
		private readonly BinaryFormatterWriter writer;

		public List<Username> NCUsernames;

		private readonly Thread ReadingThread;
		//private readonly List<ChatInfo> chats = new List<ChatInfo>();

		public HomePage(App app, Username username, BinaryFormatterWriter writer, BinaryFormatterReader reader, AccountInfoResponse aiResp, long sessionID)
		{
			this.app = app;
			myUsername = username;
			this.writer = writer;
			this.sessionID = sessionID;
			InitializeComponent();

			LoadChats(aiResp.SimpleChats, aiResp.GroupChats);
			listView.ItemsSource = chatViews;

			// not practical, but using lambda ensures noone can use this "method"
			// plus this has access to all variables without making them arguments

			ReadingThread = new Thread(new ThreadStart(() =>
				{
					while (true)
					{
						Response resp = (Response)reader.Read();
						switch (resp.Type)
						{
							case ResponseType.ChatCreated:
								ChatCreatedResponse CCResp = (ChatCreatedResponse)resp;
								var info = CCResp.Info;
								chatViews.Insert(0, new ChatInfoCell(info) );
								chats.Add((info.Type, info.ID), info);
								break;
							case ResponseType.AddMessage:
								AddMessageResponse AMResp = (AddMessageResponse)resp;
								if ( chats.TryGetValue((AMResp.chatType, AMResp.chatID), out var chat) && 
									pages.TryGetValue((AMResp.chatType, AMResp.chatID), out var page) )
								{
									var msg = AMResp.message;
									if ( msg.SenderUsername.Equals(myUsername))
									{
										msg.column = 2;
										msg.bcgColor = Color.LightBlue;
										msg.horizontalOpt = LayoutOptions.End;
										msg.textAlignment = TextAlignment.End;
									}
									chat.AddMessage(msg);
									page.listView.ScrollTo(msg, ScrollToPosition.End, animated: true);
								}
								
								break;
							default:
								break;
						}
					}
				}));
			ReadingThread.IsBackground = true;
			ReadingThread.Start();
		}

		private void LoadChats(IReadOnlyList<ChatInfo> simpleChats, IReadOnlyList<ChatInfo> groupChats)
		{
			List<ChatInfo> chats = new List<ChatInfo>();
			if (simpleChats != null)
			{
				foreach (var chat in simpleChats)
				{
					foreach (var msg in chat.GetMessages())
					{
						if ( msg.SenderUsername.Equals( myUsername ) )
						{
							msg.column = 2;
							msg.bcgColor = Color.LightBlue;
							msg.horizontalOpt = LayoutOptions.End;
							msg.textAlignment = TextAlignment.End;
						}
					}
					while (! this.chats.TryAdd((chat.Type, chat.ID), chat) );
					while (! this.pages.TryAdd((chat.Type, chat.ID), new ChatPage(this.app, myUsername, chat, this.writer, sessionID)) );
					chats.Add(chat);
				}
			}

			if (groupChats != null)
			{
				foreach (var chat in groupChats)
				{
					foreach (var msg in chat.GetMessages())
					{
						if ( msg.SenderUsername.Equals( myUsername ) )
						{
							msg.column = 2;
							msg.bcgColor = Color.LightBlue;
							msg.horizontalOpt = LayoutOptions.End;
							msg.textAlignment = TextAlignment.End;
						}
					}
					while (! this.chats.TryAdd((chat.Type, chat.ID), chat) );
					while (! this.pages.TryAdd((chat.Type, chat.ID), new ChatPage(this.app, myUsername, chat, this.writer, sessionID)) );
					chats.Add(chat);
				}
			}

			//List<ChatInfo> chatsList = new List<ChatInfo>(this.chats.Values);

			chats.Sort((a,b) => DateTime.Compare(a.lastMessageTime, b.lastMessageTime));

			UpdateChatsView(chatViews, chats);
		}

		private void UpdateChatsView(ObservableCollection<ChatInfoCell> chatViews, List<ChatInfo> chats)
		{
			chatViews.Clear();

			foreach (var item in chats)
			{
				chatViews.Add( new ChatInfoCell(item) );
			}
		}

		private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			var item = e.Item as ChatInfoCell;
			(sender as ListView).SelectedItem = null;
			ChatPage page;
			if ( pages.TryGetValue((item.Info.Type, item.Info.ID), out page))
			{
				var messages = item.Info.GetMessages();
				page.listView.ScrollTo(messages[messages.Count - 1], ScrollToPosition.End, animated: false);
			}
			else
			{
				page = new ChatPage(this.app, this.myUsername, item.Info, this.writer, sessionID);
				pages.Add((item.Info.Type, item.Info.ID), page);
			}
			Navigation.PushAsync( page );
		}

		private void newChat_Click(object sender, EventArgs e)
		{
			var page = new NewSimpleChatPage(myUsername, writer, sessionID);

			Navigation.PushModalAsync( page, animated: true);
		}

		private void newGroupChat_Click(object sender, EventArgs e)
		{
			var page = new NewGroupChatPage(myUsername, writer, sessionID);

			Navigation.PushModalAsync( page, animated: true);
			//DisplayAlert("Error", "This feature is not yet implemented.", "OK");
		}
	}
}