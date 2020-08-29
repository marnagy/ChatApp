using ChatLib;
using ChatLib.BinaryFormatters;
using ChatLib.Messages;
using ChatServer.Databases;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace ChatServer
{
	internal class FileSystemDatabase : IDatabase
	{
		// constants
		private const string masterDirName = "ServerFiles";
		private const string usersDirName = "users";
		private const string chatsDirName = "chats";
		private const string groupChatsDirName = "group_chats";
		private const string infoFileName = ".info";
		private const string simpleChatInfoFileName = ".chInfo";
		private const string groupChatInfoFileName = ".gchInfo";
		private const string IDFileName = ".msgid";
		private const string chatNameFileName = ".nm";

		public const char simpleChatSeparator = ' ';

		private const int saltLength = 16;
		private const int hashKey = 20;
		private const int hashIterations = 20_000;

		private readonly Random rand = new Random();

		private readonly DirectoryInfo masterDir;
		private readonly DirectoryInfo usersDir;
		private readonly DirectoryInfo simpleChatsDir;
		private readonly DirectoryInfo groupChatsDir;

		private readonly RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
		private readonly SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider();


		private readonly HashSet<Email> allEmails = new HashSet<Email>();
		private readonly HashSet<Username> allUsernames = new HashSet<Username>();
		private readonly HashSet<Username> onlineUsers = new HashSet<Username>();
		private readonly HashSet<long> simpleChatIDs = new HashSet<long>();
		private readonly HashSet<long> groupChatIDs = new HashSet<long>();

		private ulong groupIDGenerator = 0;

		private FileSystemDatabase()
		{
			masterDir = Directory.CreateDirectory(Path.Combine(masterDirName));
			usersDir = masterDir.CreateSubdirectory(usersDirName);
			simpleChatsDir = masterDir.CreateSubdirectory(chatsDirName);
			groupChatsDir = masterDir.CreateSubdirectory(groupChatsDirName);
			Console.WriteLine("Database loaded.");
		}
		public static FileSystemDatabase Initialize()
		{
			var db = new FileSystemDatabase();
			

			using (var writer = new StreamWriter(File.Create(Path.Combine(db.masterDir.FullName, infoFileName))))
			{
				var dt = DateTime.UtcNow;
				writer.WriteLine($"Created:{dt}");
			}

			db.LoadUsernamesAndEmails();
			return db;
		}
		private void LoadUsernamesAndEmails()
		{
			foreach (DirectoryInfo userDir in usersDir.EnumerateDirectories())
			{
				allUsernames.Add(userDir.Name.ToUsername());
				using (var reader = new StreamReader(File.OpenRead(Path.Combine(userDir.FullName, infoFileName))))
				{
					allEmails.Add(reader.ReadLine().ToEmail());
				}
			}
		}
		private bool _initialized = false;
		public bool initialized { get => _initialized; }

		public (bool success, Username[] users, string ReasonOrID) AddMessage(ChatType type, long chatID, Message message)
		{
			lock (this)
			{
				switch (message.Type)
				{
					case MessageType.TextMessage:
						//TextMessage msg = (TextMessage)message;
						DirectoryInfo currChatDir = GetChatDir(type, chatID);
						var dt = DateTime.UtcNow;
						var chatsOfDay = string.Format("{0:D4}-{1:D2}-{2:D2}", dt.Year, dt.Month, dt.Day);
						//string currentMsgID = File.ReadAllText(Path.Combine(currChatDir.FullName, IDFileName));
						//if (long.TryParse(currentMsgID, out long MsgID))
						//{
							// save message
							BinaryFormatterWriter bfw = new BinaryFormatterWriter(new BinaryFormatter(), File.AppendText( Path.Combine(currChatDir.FullName, chatsOfDay) ).BaseStream );
							bfw.Write(message);
							bfw.Close();
							
							Username[] users = File.ReadAllLines( Path.Combine(currChatDir.FullName, infoFileName) ).Select( text => text.ToUsername() ).ToArray();

							//File.WriteAllText(Path.Combine(currChatDir.FullName, IDFileName), (++MsgID).ToString());
							return (true, users, string.Empty);
						//}
						//else
						//{
						//	return (false, null, "Unsupported messageID format.");
						//}
					default:
						return (false, null, "Unsupported message type.");
				}
			}
		}

		private DirectoryInfo GetChatDir(ChatType type, long chatID)
		{
			switch (type)
			{
				case ChatType.Simple:
					return Directory.CreateDirectory(
						Path.Combine(simpleChatsDir.FullName, chatID.ToString()));
				case ChatType.Group:
					return Directory.CreateDirectory(
						Path.Combine( groupChatsDir.FullName, chatID.ToString()));
				default:
					throw new FormatException("Unsupported chat type.");
			}
		}

		public bool Connect()
		{
			if (Directory.Exists(Path.Combine(".", masterDirName)) && Directory.Exists(Path.Combine(".", masterDirName, usersDirName))
				&& Directory.Exists(Path.Combine(".", masterDirName, chatsDirName)) && Directory.Exists(Path.Combine(".", masterDirName, groupChatsDirName)))
			{
				_initialized = true;
			}
			else
			{
				Init();
			}
			return true;
		}

		//public (bool success, string ReasonOrName) CreateChat(Username[] users, Message msg)
		//{
		//	if (users.Length == 2)
		//	{
		//		var user1 = users[0];
		//		var user2 = users[1];
		//		long chatID = GetChatID(user1, user2);
		//		lock (this)
		//		{
		//			string simpleChatName;
		//			foreach (var item in File.ReadLines(Path.Combine(
		//				usersDir.FullName, user1.ToString(), chatInfoFileName)))
		//			{
		//				(simpleChatName, _) = GetSimpleChatInfo(item);
		//				if (chatName == simpleChatName)
		//				{
		//					return (false, "Chat already exists.");
		//				}
		//			}

		//			// create chat dir
		//			var chatDirInfo = chatsDir.CreateSubdirectory(chatName);
		//			// create info file for chat
		//			using (var writer = File.CreateText(Path.Combine(chatDirInfo.FullName, infoFileName)))
		//			{
		//				var dt = DateTime.UtcNow;
		//				writer.WriteLine(dt);

		//				writer.WriteLine(user1 + " " + 0);
		//				writer.WriteLine(user2 + " " + 0);
		//			}

		//			AddFirstMessage();
		//		}
		//		return (true,chatName);
		//	}
		//	else
		//	{
		//		throw new NotImplementedException();
		//	}
		//	}

		//private (string name, long messageID) GetSimpleChatInfo(string line)
		//{
		//	if (line == null) throw new ArgumentNullException();
		//	string[] lineParts = line.Split(simpleChatSeparator);
		//	if (long.TryParse(lineParts[2], out long lastMsgID))
		//	{
		//		return (lineParts[0] + simpleChatSeparator.ToString() + lineParts[1], lastMsgID);
		//	}
		//	else
		//	{
		//		throw new FormatException("Wrong format of stored info about chat.");
		//	}
		//}
		private long GetChatID(Username user1, Username user2)
		{
			lock (simpleChatIDs)
			{
				long id = GetRandomLong();
				while ( simpleChatIDs.Contains(id) )
				{
					id = GetRandomLong();
				}
				simpleChatIDs.Add(id);
				return id;
			}
		}

		private long GetChatID(Username[] users)
		{
			lock (groupChatIDs)
			{
				long id = GetRandomLong();
				while ( groupChatIDs.Contains(id) )
				{
					id = GetRandomLong();
				}
				groupChatIDs.Add(id);
				return id;
			}
		}

		private long GetRandomLong() {
			long result = (long)rand.Next(int.MinValue, int.MaxValue);
			result = result << 32;
			result = result | (long)rand.Next(int.MinValue, int.MaxValue);
			return result;
		}

		public (bool success, ChatInfo info, string reasonOrName) CreateChat(Username[] users)
		{
			lock (allUsernames)
			{
				foreach (var user in users)
				{
					if ( !allUsernames.Contains(user))
					{
						return (false, null, "A username does not exist.");
					}
				}
			}

			lock (this)
			{
				if (users.Length == 2)
				{
					// simple chat
					var chatID = GetChatID(users[0], users[1]);
					var chatDir = simpleChatsDir.CreateSubdirectory(chatID.ToString());

					var info = new ChatInfo(ChatType.Simple, chatID, SimpleChatDefaultFileName(users[0], users[1]), users);

					using ( var infoFile = new StreamWriter( File.Create( Path.Combine(chatDir.FullName, infoFileName) ) ) )
					{
						foreach (var username in info.participants)
						{
							infoFile.WriteLine( username.ToString() );
						}
					}

					using ( var infoFile = new StreamWriter( File.Create( Path.Combine(chatDir.FullName, chatNameFileName) ) ) )
					{
						infoFile.WriteLine(info.Name);
					}

					foreach (var username in users)
					{
						AddChat(info.Type, chatID, username);
					}

					return (true, info, string.Empty);
				}
				else if ( users.Length > 2)
				{
					// group chat
					var chatID = GetChatID(users);
					var chatDir = groupChatsDir.CreateSubdirectory(chatID.ToString());

					var info = new ChatInfo(ChatType.Group, chatID, GroupChatDefaultFileName(users), users);

					using ( var infoFile = new StreamWriter( File.Create( Path.Combine(chatDir.FullName, infoFileName) ) ) )
					{
						foreach (var username in info.participants)
						{
							infoFile.WriteLine( username.ToString() );
						}
					}

					using ( var infoFile = new StreamWriter( File.Create( Path.Combine(chatDir.FullName, chatNameFileName) ) ) )
					{
						infoFile.WriteLine(info.Name);
					}

					foreach (var username in users)
					{
						AddChat(info.Type, chatID, username);
					}

					return (true, info, string.Empty);
					//return (false, null, "Not yet implemented");
				}
				else
				{
					return (false, null, "Insufficient number of users.");
				}
			}
		}

		private void AddChat(ChatType type, long chatID, Username username)
		{
			switch (type)
			{
				case ChatType.Simple:
					File.AppendAllText(Path.Combine(usersDir.FullName, username.ToString(), simpleChatInfoFileName), chatID + "\n");
					break;
				case ChatType.Group:
					File.AppendAllText(Path.Combine(usersDir.FullName, username.ToString(), groupChatInfoFileName), chatID + "\n");
					break;
				default:
					throw new ArgumentException("Invalid chat type");
			}
			
		}

		private string GroupChatDefaultFileName(Username[] users)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(users[0]);
			for (int i = 1; i < users.Length; i++)
			{
				sb.Append(", ");
				sb.Append(users[i]);
			}

			return sb.ToString();
		}

		private string SimpleChatDefaultFileName(Username username1, Username username2)
		{
			return username1.ToString() + "," + username2.ToString();
		}

		public IEnumerable<Username> GetUsers()
		{
			return allUsernames;
		}

		public bool Init()
		{
			if (_initialized) return true;
			Directory.CreateDirectory(Path.Combine(masterDirName, usersDirName));
			Directory.CreateDirectory(Path.Combine(masterDirName, chatsDirName));
			Directory.CreateDirectory(Path.Combine(masterDirName, groupChatsDirName));

			using (var writer = new StreamWriter(File.Create(Path.Combine(masterDirName, infoFileName))))
			{
				var dt = DateTime.Now;
				writer.Write($"Created on {dt.Day}.{dt.Month}.{dt.Year} at {dt.Hour}:{dt.Minute}.");
			}

			_initialized = true;
			return true;
		}

		public bool MakeOnline(User user)
		{
			return onlineUsers.Add(user.username);
		}

		public void Run()
		{
			throw new NotImplementedException();
		}

		public bool Contains(Email email)
		{
			return allEmails.Contains(email);
		}

		private void CreateGroupChatList(DirectoryInfo userDir)
		{
			File.CreateText(Path.Combine(userDir.FullName, groupChatInfoFileName)).Close();
		}

		private void CreateChatList(DirectoryInfo userDir)
		{
			File.CreateText(Path.Combine(userDir.FullName, simpleChatInfoFileName)).Close();
		}

		private void CreateUserInfo(DirectoryInfo userDir, Email email, Password password)
		{
			using (var writer = new StreamWriter(File.OpenWrite(Path.Combine(userDir.FullName, infoFileName))))
			{
				// store email
				writer.WriteLine(email.ToString());

				// generate hash of password + salt
				byte[] salt = new byte[saltLength];
				random.GetBytes(salt);

				//var pbkdf2 = new Rfc2898DeriveBytes(password.ToString(), salt, hashIterations);
				//byte[] hash = pbkdf2.GetBytes(hashKey);
				byte[] hash = new Rfc2898DeriveBytes(password.ToString(),
					salt, hashIterations).GetBytes(hashKey);

				byte[] hashBytes = new byte[saltLength + hashKey];
				Array.Copy(salt, 0, hashBytes, 0, saltLength);
				Array.Copy(hash, 0, hashBytes, saltLength, hashKey);

				// store hash
				string savedPasswordHash = Convert.ToBase64String(hashBytes);
				writer.WriteLine(savedPasswordHash);
			}

			//set file to read-only
		}
		public (bool successful, string reasonOfFail) SignIn(Username username, Password password)
		{
			const string signInFailMssg = "Username or password is incorrect.";
			lock (this)
			{
				if (allUsernames.Contains(username))
				{
					string[] lines = File.ReadAllLines(Path.Combine(usersDir.FullName, username.ToString(), infoFileName));
					// load hash
					string savedPasswordHash = lines[1];
					lines = null;
					// convert hash to byte[]
					byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
					// extract salt
					byte[] salt = new byte[saltLength];
					Array.Copy(hashBytes, 0, salt, 0, saltLength);

					byte[] hash = new Rfc2898DeriveBytes(password.ToString(),
						salt, hashIterations).GetBytes(hashKey);
					for (int i = 0; i < hashKey; i++)
					{
						if (hashBytes[i+saltLength] != hash[i])
						{
							return (false, signInFailMssg);
						}
					}
					return (true, string.Empty);
				}
				else
				{
					return (false, signInFailMssg);
				}
			}
		}

		public bool Contains(Username username)
		{
			return allUsernames.Contains(username);
		}

		public (bool successful, string reasonOfFail) AddUser(Email email, Username username, Password password)
		{
			lock (this)
			{
				if (!allUsernames.Contains(username))
				{
					if (!allEmails.Contains(email))
					{
						var userDir = usersDir.CreateSubdirectory(username.ToString());
					
						CreateUserInfo(userDir, email, password);
						CreateChatList(userDir);
						CreateGroupChatList(userDir);

						allUsernames.Add(username);
						allEmails.Add(email);
						return (true, "");
					}
					else
					{
						return (false, "Email is already signed up.");
					}
				}
				else
				{
					return (false, "Username already taken. Please, try different username.");
				}
			}
		}

		public ChatInfo[] GetChats(Username username, ChatType type)
		{
			List<ChatInfo> list = new List<ChatInfo>();
			lock (this)
			{
				string fileName;
				DirectoryInfo chatsDir;
				switch (type)
				{
					case ChatType.Simple:
						fileName = simpleChatInfoFileName;
						chatsDir = simpleChatsDir;
						break;
					case ChatType.Group:
						fileName = groupChatInfoFileName;
						chatsDir = groupChatsDir;
						break;
					default:
						throw new ArgumentException();
				}

				var chatIDs = File.ReadAllLines(Path.Combine(usersDir.FullName, username.ToString(), fileName));

				foreach (var ID in chatIDs)
				{
					list.Add( ChatInfo.FromDictionary(type, Directory.CreateDirectory(Path.Combine(chatsDir.FullName, ID) ), infoFileName, chatNameFileName ) );
				}

				return list.ToArray();
			}
		}
	}
}