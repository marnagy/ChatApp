﻿using ChatServer.Databases;
using System;
using System.Collections.Generic;
using ChatLib;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using ChatLib.Requests;
using ChatLib.Responses;
using ChatLib.BinaryFormatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Microsoft.VisualBasic;
using ChatLib.Messages;
using System.Reflection.Metadata;

namespace ChatServer
{
	class ChatServer
	{
		// constants
		private const int defaultPort = 5318;

		// static variables
		private static ChatServer _instance = null;

		// instance variables
		private readonly int port;

		private bool killDB = false;
		//private readonly Queue<(ServerSession, IDatabaseRequest)> databaseRequests = new Queue<(ServerSession, IDatabaseRequest)>();
		
		private readonly HashSet<Username> loggedInUsers = new HashSet<Username>();
		private readonly ConcurrentDictionary<Username, BinaryFormatterWriter>
			writersPerUsername = new ConcurrentDictionary<Username, BinaryFormatterWriter>();
		private readonly HashSet<long> activeSessions = new HashSet<long>();

		private readonly IDatabase database;

		public static ChatServer GetInstance(IDatabase databaseFact)
		{
			return GetInstance(databaseFact, defaultPort);
		}
		public static ChatServer GetInstance(IDatabase databaseFact, int port)
		{
			if (_instance == null)
			{
				_instance = new ChatServer(databaseFact, port);
			}
			return _instance;
		}
		private ChatServer(IDatabase database, int port)
		{
			this.port = port;
			this.database = database;
		}
		public void Run()
		{
			TcpListener listener = new TcpListener(port);

			long sessionID = 1;
			TcpClient client;
			listener.Start();
			long currSessionID;
			while (true)
			{
				Console.Out.WriteLine("Waiting for connection...");
				client = listener.AcceptTcpClient();
				Console.Out.WriteLine("Connection accepted.");
				lock (activeSessions)
				{
					while ( activeSessions.Contains(sessionID) )
					{
						sessionID++;
					}
					currSessionID = sessionID++;
					activeSessions.Add( currSessionID );
				}
				Task.Run(() => {
					new ServerSession(client, this, currSessionID).HandleConnection();
				});
				Console.Out.WriteLine("New session created.");
			}
		}
		public class ServerSession
		{
			private long sessionID;
			private TcpClient client;
			private ChatServer server;
			private bool loggedIn = false;

			private readonly BinaryFormatterReader br;
			private readonly BinaryFormatterWriter bw;

			Response resp;
			Request req;
			Message msg;
			Username[] users;
			ChatInfo info;
			Username loggedUser = new Username();
			bool success; string reason;

			internal ServerSession(TcpClient client, ChatServer server, long sessionID)
			{
				this.sessionID = sessionID;
				this.server = server;
				this.client = client;
				var bf = new BinaryFormatter();
				var stream = client.GetStream();
				if (stream == null)
				{
					throw new NullReferenceException();
				}
				br = new BinaryFormatterReader(bf, stream);
				bw = new BinaryFormatterWriter(bf, stream);
			}
			internal void HandleConnection()
			{
				try
				{
					bw.Write(sessionID);

					
					while (true)
					{
						HandleNonLoggedInRequests();

						HandleLoggedInRequests();
					}
				}
				catch (SerializationException e)
				{
					if ( loggedIn )
					{
						server.loggedInUsers.Remove(loggedUser);
						server.database.MakeOffline(loggedUser);
						server.writersPerUsername.TryRemove(loggedUser, out var writer);
						server.database.UnloadContacts(loggedUser);
						writer.Close();
						loggedIn = false; // not neccessary
					}
				}
				catch ( Exception e )
				{
					Console.Error.WriteLine("Exception -> {}", e);
				}
				finally
				{
					server.activeSessions.Remove(this.sessionID);
				}
			}

			private void HandleLoggedInRequests()
			{
				while (loggedIn)
				{
					req = (Request)br.Read();
					if ( req.SessionID != sessionID)
					{
						return;
					}

					switch ( req.Type )
					{
						case RequestType.NewChat:
							NewChatRequest NCReq = (NewChatRequest)req;
							(success, info, reason) = server.database.CreateChat(NCReq.participants);

							if (success)
							{
								foreach (Username user in info.participants)
								{
									if ( server.writersPerUsername.TryGetValue(user, out var writer))
									{
										writer.Write( new ChatCreatedResponse(info, sessionID) );
									}
								}

							}
							else
							{
								resp = new FailResponse(reason, sessionID);
							}
							info = null;
							reason = null;
							break;
						case RequestType.NewMessage:
							NewMessageRequest NMReq = (NewMessageRequest)req;
							(success, users, msg, reason) = server.database.AddMessage(NMReq.chatType, NMReq.chatID, NMReq.message);

							// (future) confirm receiving the message

							foreach (Username user in users)
							{
								server.writersPerUsername.TryGetValue(user, out var writer);
								if (writer != null) {
									writer.Write( new AddMessageResponse( NMReq.chatType, NMReq.chatID, msg, sessionID) );
								}
							}

							msg = null;
							users = null;
							reason = null;
							break;
						case RequestType.DeleteMessage:
							DeleteMessageRequest DMReq = (DeleteMessageRequest)req;
							(success, users, reason) = server.database.DeleteMessage(DMReq.chatType, DMReq.chatID, DMReq.dateTime);

							if (success)
							{
								foreach (var user in users)
								{
									if (server.writersPerUsername.TryGetValue(user, out var writer))
									{
										writer.Write( new DeleteMessageResponse(DMReq.chatType, DMReq.chatID, DMReq.dateTime, sessionID) );
									}
								}
							}
							break;
						case RequestType.GetOnlineContacts:
							OnlineContactsRequest OCReq = (OnlineContactsRequest)req;
							(success, users, reason) = server.database.GetOnlineContacts(OCReq.username);

							if (success)
							{
								resp = new OnlineContactsResponse(users, sessionID);
							}
							break;
						case RequestType.ChangePassword:
							ChangePasswordRequest CPReq = (ChangePasswordRequest)req;
							(success, reason) = server.database.ChangePassword(CPReq.username, CPReq.oldPassword, CPReq.newPassword);

							if (success)
							{
								resp = new SuccessResponse(sessionID);
							}
							else
							{
								resp = new FailResponse(reason, sessionID);
							}
							break;
						default:
							resp = new FailResponse("Unsupported request type detected.", sessionID);
							break;
					}

					req = null;

					if (resp != null)
					{
						bw.Write(resp);
						//resp.Send(bw);

						resp = null;
						if (!loggedIn) return;
					}
				}
			}

			private void HandleNonLoggedInRequests()
			{
				while (!loggedIn)
				{
					req = (Request)br.Read();
					if ( req.SessionID != sessionID)
					{
						return;
					}

					switch ( req.Type )
					{
						case RequestType.NewAccount:
							NewAccountRequest NAReq = (NewAccountRequest)req;
							(success, reason) = server.database.AddUser(NAReq.email, NAReq.username, NAReq.password);
							if (success)
							{
								resp = new SuccessResponse(sessionID);
							}
							else
							{
								resp = new FailResponse(reason, sessionID);
							}
							reason = null;
							break;
						case RequestType.SignIn:
							SignInRequest SIReq = (SignInRequest)req; 
							(success, reason) = server.database.SignIn(SIReq.username, SIReq.password);
							if (success)
							{
								resp = GetAccountInfo(SIReq.username);
								if ( server.writersPerUsername.TryAdd(SIReq.username, bw) )
								{
									loggedUser = SIReq.username;
									server.loggedInUsers.Add(loggedUser);
									loggedIn = true;
								}
								
							}
							else
							{
								resp = new FailResponse(reason, sessionID);
							}
							reason = null;
							break;
						default:
							resp = new FailResponse("Unsupported request type detected.", sessionID);
							break;
					}

					req = null;

					if (resp != null)
					{
						bw.Write(resp);
						//resp.Send(bw);

						resp = null;
						if (loggedIn) return;
					}
				}
			}

			private AccountInfoResponse GetAccountInfo(Username username)
			{
				var simpleChats = server.database.GetChats(username, ChatType.Simple);
				var groupChats = server.database.GetChats(username, ChatType.Group);
				return new AccountInfoResponse(simpleChats, groupChats, sessionID);
			}
		}
		

	}
}
