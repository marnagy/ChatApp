using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer.DatabaseRequests
{
	interface IDatabaseRequest
	{
		DatabaseRequestType type {get;}
	}
}
