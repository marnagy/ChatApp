using ChatServer.Databases;
using ChatServer.DatabaseRequests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ChatLib;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using ChatLib.Requests;
using ChatLib.Responses;

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
		
		private readonly ConcurrentDictionary<string, BinaryWriter> writersPerUsername = new ConcurrentDictionary<string, BinaryWriter>();
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
			//Thread DBThread = new Thread(new ThreadStart(() => {
			//	DBRequestsHandler(databaseRequests, database, ref killDB);
			//}));
			//DBThread.Start();
		}
		private void DBRequestsHandler(Queue<(ServerSession, IDatabaseRequest)> queue, IDatabase database, ref bool killDB)
		{
			while (!killDB)
			{
				while (queue.Count == 0)
				{
					Thread.Sleep(50);
				}
				var (session, request) = queue.Dequeue();
				var requestType = request.type;
				switch (requestType)
				{
					case DatabaseRequestType.CreateAccount:
						var currRequest = (DatabaseNewAccountRequest)request;
						if (!database.Contains(email: currRequest.email)
							&& !database.Contains(username: currRequest.username))
						{
							database.AddUser(currRequest.email, currRequest.username,
								currRequest.password);
							session.addUserResult = true;
						}
						else
						{
							session.addUserResult = false;
						}
						break;
					case DatabaseRequestType.Login:

						break;
					default:
						break;
				}
			}
		}
		public void Run()
		{
			TcpListener listener = new TcpListener(port);

			//Console.WriteLine("Database initialized.");

			long sessionID = 0;
			TcpClient client;
			listener.Start();
			long currSessionID;
			while (true)
			{
				Console.WriteLine("Waiting for connection...");
				client = listener.AcceptTcpClient();
				Console.WriteLine("Connection accepted.");
				lock (activeSessions)
				{
					while ( activeSessions.Contains(sessionID))
					{
						sessionID++;
					}
					currSessionID = sessionID++;
				}
				Task.Run(() => {
					new ServerSession(client, this, currSessionID).HandleConnection();
				});
				Console.WriteLine("New session created.");
			}
		}
		public class ServerSession
		{
			private const int sleepConst = 50;

			private long sessionID;
			private TcpClient client;
			private ChatServer server;
			private bool loggedIn = false;

			public bool? addUserResult = null;
			public bool? loginResult = null;

			private readonly BinaryReader br;
			BinaryWriter bw;
			internal ServerSession(TcpClient client, ChatServer server, long sessionID)
			{
				this.sessionID = sessionID;
				this.server = server;
				this.client = client;
				br = new BinaryReader(client.GetStream());
				bw = new BinaryWriter(client.GetStream());
			}
			internal void HandleConnection()
			{
				bw.Write(sessionID);
				bw.Flush();

				Response resp;
				while (!loggedIn)
				{
					resp = null;
					if (br.ReadInt64() != sessionID)
					{
						return;
					}

					switch ((RequestType)Enum.Parse(typeof(RequestType), br.ReadString()))
					{
						case RequestType.NewAccount:
							{
								NewAccountRequest req = NewAccountRequest.Read(br, sessionID);
								var (success, reason) = server.database.AddUser(req.email, req.username, req.password);
								if (success)
								{
									resp = new SuccessResponse(sessionID);
								}
								else
								{
									resp = new FailResponse(reason, sessionID);
								}
							}
							break;
						case RequestType.SignIn:
							{
								SignInRequest req = SignInRequest.Read(br, sessionID);
								var (success, reason) = server.database.SignIn(req.username, req.password);
								if (success)
								{
									resp = GetAccountInfo(req.username);
								}
							}
							break;
						default:
							break;
					}
					if (resp != null)
					{
						resp.Send(bw);
					}
				}
				// logged in
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
