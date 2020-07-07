using System;
using System.Collections.Generic;
using System.Text;
using ChatLib;
using ChatLib.Messages;

namespace ChatServer.Databases
{
	interface IDatabase
	{
		bool initialized { get; }
		bool AddUser(string username, char[] password);
		bool MakeOnline(User user);
		bool AddMessage(ChatInfo info, IMessage message);
		/// <summary>
		/// Method to initialize simple chat of 2 users.
		/// </summary>
		/// <param name="user1">User initializing conversation.</param>
		/// <param name="user2">User who receives first message.</param>
		/// <returns>Result if creation was successful.</returns>
		string CreateChat(User user1, User user2);
		/// <summary>
		/// Method to initialize group chat.
		/// </summary>
		/// <param name="users">Participants of the group chat.</param>
		/// <returns>Result if creation was successful.</returns>
		string CreateChat(User[] users);
		bool Contains(User user);
		IEnumerable<string> GetUsers();
	}
}
