using ChatLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatServer.DatabaseRequests
{
	class DatabaseNewAccountRequest : IDatabaseRequest
	{
		public readonly Email email;
		public readonly Username username;
		public readonly Password password;
		public DatabaseRequestType type => DatabaseRequestType.Login;
		public DatabaseNewAccountRequest(Email email,Username username, Password password)
		{
			this.email = email;
			this.username = username;
			this.password = password;
		}
	}
}
