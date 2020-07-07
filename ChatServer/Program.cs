using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer
{
	class Program
	{
		public static void Main(string[] args)
		{
			var server = ChatServer.GetInstance(new FileSystemDatabase());
			server.Run();
		}
	}
}
