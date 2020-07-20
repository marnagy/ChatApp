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
		(bool successful, string reasonOfFail) AddUser(Email email, Username username, Password password);
		(bool successful, string reasonOfFail) SignIn(Username username, Password password);
		ChatInfo[] GetChats(Username username, ChatType type);
		bool MakeOnline(User user);
		bool AddMessage(ChatInfo info, IMessage message);
		/// <summary>
		/// Creates new chat of 2 users
		/// </summary>
		/// <param name="user1"></param>
		/// <param name="user2"></param>
		/// <returns>ValueTuple of success and name of created chat if success.</returns>
		(bool success, string reason) CreateChat(User user1, User user2, IMessage msg);
		/// <summary>
		/// Method to initialize group chat.
		/// </summary>
		/// <param name="users">Participants of the group chat.</param>
		/// <returns>Name of chat identifier.</returns>
		string CreateChat(User[] users);
		/// <summary>
		/// Checks if there exists user with the given username
		/// </summary>
		/// <param name="username"></param>
		/// <returns>If there is a user with given username.</returns>
		bool Contains(Username username);
		/// <summary>
		/// Checks if there exists user with the given email address.
		/// </summary>
		/// <param name="email"></param>
		/// <returns>If there is a user with given email address.</returns>
		bool Contains(Email email);
	}
}
