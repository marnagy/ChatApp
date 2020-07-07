using ChatServer.Databases;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ChatLib;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

namespace ChatServer
{
	class ChatServer
	{
		// constants
		private const int defaultPort = 5318;

		// static variables
		private static ChatServer _instance = null;
		

		// instance variables
		private readonly IDatabase database;
		private readonly int port;
		
		private readonly ConcurrentDictionary<string, BinaryWriter> writersPerUsername = new ConcurrentDictionary<string, BinaryWriter>();
		private readonly HashSet<long> activeSessions = new HashSet<long>();

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
			this.database = database;
			this.port = port;
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
				client = listener.AcceptTcpClient();
				lock (activeSessions)
				{
					while ( activeSessions.Contains(sessionID))
					{
						sessionID++;
					}
					currSessionID = sessionID++;
				}
				Task.Run(() => {
					new ServerSession(client, writersPerUsername, activeSessions, database, currSessionID).HandleConnection();
				});
			}
		}
		class ServerSession
		{
			private long sessionID;
			private TcpClient client;

			readonly BinaryReader br;
			BinaryWriter bw;
			internal ServerSession(TcpClient client, ConcurrentDictionary<string, BinaryWriter> writers, ISet<long> activeSessions, IDatabase database, long sessionID)
			{
				this.sessionID = sessionID;
				this.client = client;
				br = new BinaryReader(client.GetStream());
				bw = new BinaryWriter(client.GetStream());
			}
			internal void HandleConnection()
			{
				bw.Write(sessionID);
				bw.Flush();

				while (true)
				{
					if (br.ReadInt64() != sessionID)
					{
						break;
					}
				}
			}
		}
	}
}
