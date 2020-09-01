using System;
using System.Collections.Generic;
using System.Text;

namespace ChatLib.Requests
{
	public enum RequestType
	{
		NewAccount,
		SignIn,
		NewChat,
		NewMessage,
		GetOnlineContacts,
		ChangePassword
	}
}
