using ChatLib;
using ChatLib.Messages;
using ChatServer.Databases;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
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
		private const string chatInfoFileName = ".chInfo";
		private const string groupChatInfoFileName = ".gchInfo";

		public const char simpleChatSeparator = ' ';

		private const int saltLength = 16;
		private const int hashKey = 20;
		private const int hashIterations = 20_000;

		private readonly DirectoryInfo masterDir;
		private readonly DirectoryInfo usersDir;
		private readonly DirectoryInfo chatsDir;
		private readonly DirectoryInfo groupChatsDir;

		private readonly RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
		private readonly SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider();


		private readonly HashSet<Email> allEmails = new HashSet<Email>();
		private readonly HashSet<Username> allUsernames = new HashSet<Username>();
		private readonly HashSet<Username> onlineUsers = new HashSet<Username>();
		//private readonly Dictionary<Username, Email>

		private ulong groupIDGenerator = 0;

		private FileSystemDatabase()
		{
			masterDir = Directory.CreateDirectory(Path.Combine(masterDirName));
			usersDir = masterDir.CreateSubdirectory(usersDirName);
			chatsDir = masterDir.CreateSubdirectory(chatsDirName);
			groupChatsDir = masterDir.CreateSubdirectory(groupChatsDirName);
		}
		public static FileSystemDatabase Initialize()
		{
			var db = new FileSystemDatabase();
			

			using (var writer = new StreamWriter(File.Create(Path.Combine(db.masterDir.FullName, infoFileName))))
			{
				var dt = DateTime.Now;
				writer.Write($"Created on {dt.Day}.{dt.Month}.{dt.Year} at {dt.Hour}:{dt.Minute}.");
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

		public bool AddMessage(ChatInfo chatInfo, IMessage msg)
		{
			throw new NotImplementedException();
		}

		//public MessageRecord AddMessage
		public bool AddMessage()
		{
			throw new NotImplementedException();
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

		public (bool success, string ReasonOrName) CreateChat(User user1, User user2, IMessage msg)
		{
			string chatName = GetChatName(user1, user2);
			lock (this)
			{
				string simpleChatName;
				foreach (var item in File.ReadLines(Path.Combine(
					usersDir.FullName, user1.username.ToString(), chatInfoFileName)))
				{
					(simpleChatName, _) = GetSimpleChatInfo(item);
					if (chatName == simpleChatName)
					{
						return (false, "Chat already exists.");
					}
				}

				var chatDirInfo = chatsDir.CreateSubdirectory(chatName);

				using (var writer = File.CreateText(Path.Combine(chatDirInfo.FullName, infoFileName)))
				{
					var dt = DateTime.UtcNow;
					writer.WriteLine(dt);
					writer.write
				}

			}
			return (true,chatName);
		}
		private (string name, long messageID) GetSimpleChatInfo(string line)
		{
			if (line == null) throw new ArgumentNullException();
			string[] lineParts = line.Split(simpleChatSeparator);
			if (long.TryParse(lineParts[2], out long lastMsgID))
			{
				return (lineParts[0] + simpleChatSeparator.ToString() + lineParts[1], lastMsgID);
			}
			else
			{
				throw new FormatException("Wrong format of stored info about chat.");
			}
		}
		private string GetChatName(User user1, User user2)
		{
			if (user1.username.CompareTo(user2.username) > 0)
			{
				return user1.username + simpleChatSeparator.ToString() + user2.username;
			}
			return user2.username + simpleChatSeparator.ToString() + user1.username;
		}

		public string CreateChat(User[] users)
		{
			//ulong groupID = groupIDGenerator++;
			//var groupDir = groupChatsDir.CreateSubdirectory(groupID.ToString());
			//using (var writer = File.CreateText(Path.Combine(groupDir.FullName, infoFileName)))
			//{
			//	foreach(User user in users)
			//	{
			//		writer.WriteLine(user.username);
			//	}
			//}

			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		private void CreateGroupChatList(DirectoryInfo userDir)
		{
			File.CreateText(Path.Combine(userDir.FullName, groupChatInfoFileName));
		}

		private void CreateChatList(DirectoryInfo userDir)
		{
			File.CreateText(Path.Combine(userDir.FullName, chatInfoFileName));
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
					return (true, "");
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


	}
}