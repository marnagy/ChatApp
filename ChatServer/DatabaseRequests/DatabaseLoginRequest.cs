using ChatLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer.DatabaseRequests
{
	class DatabaseLoginRequest : IDatabaseRequest
	{
		public readonly Username username;
		public readonly Password password;
		public DatabaseRequestType type => DatabaseRequestType.Login;
		public DatabaseLoginRequest(Username username, Password password)
		{
			this.username = username;
			this.password = password;
		}
	}
}
