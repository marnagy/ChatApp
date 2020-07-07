using ChatLib;
using ChatLib.Messages;
using ChatServer.Databases;
using System;
using System.Collections.Generic;
using System.IO;

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


		private readonly DirectoryInfo masterDir;
		private readonly DirectoryInfo usersDir;
		private readonly DirectoryInfo chatsDir;
		private readonly DirectoryInfo groupChatsDir;

		private readonly HashSet<string> allUsernames = new HashSet<string>();
		private readonly HashSet<string> onlineUsers = new HashSet<string>();

		private ulong groupIDGenerator = 0;

		public FileSystemDatabase()
		{
			masterDir = Directory.CreateDirectory(Path.Combine(masterDirName));
			usersDir = masterDir.CreateSubdirectory(usersDirName);
			usersDir = masterDir.CreateSubdirectory(usersDirName);
			usersDir = masterDir.CreateSubdirectory(usersDirName);

			using (var writer = new StreamWriter(File.Create(Path.Combine(masterDir.FullName, infoFileName))))
			{
				var dt = DateTime.Now;
				writer.Write($"Created on {dt.Day}.{dt.Month}.{dt.Year} at {dt.Hour}:{dt.Minute}.");
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

		public bool AddUser(string username, char[] password, string email)
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
				Initialize();
			}
			return true;
		}

		public string CreateChat(User user1, User user2)
		{
			if (string.Compare(user1.username, user2.username) > 0)
			{
				var info = Directory.CreateDirectory(Path.Combine(".",
					masterDirName, chatsDirName, user1.username + "_" + user2.username));
				return user1.username + "_" + user2.username;
			}
			return user2.username + "_" + user1.username;
		}

		public string CreateChat(User[] users)
		{
			ulong groupID = groupIDGenerator++;
			var groupDir = groupChatsDir.CreateSubdirectory(groupID.ToString());
			using (var writer = File.CreateText(Path.Combine(groupDir.FullName, infoFileName)))
			{
				foreach(User user in users)
				{
					writer.WriteLine(user.username);
				}
			}


		}

		public IEnumerable<string> GetUsers()
		{
			return allUsernames;
		}

		public bool Initialize()
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
	}
}