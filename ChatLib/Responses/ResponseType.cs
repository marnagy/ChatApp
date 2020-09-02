using System;
using System.Collections.Generic;
using System.Text;

namespace ChatLib.Responses
{
	public enum ResponseType
	{
		AccountInfo,
		ChatCreated,
		AddMessage,
		DeleteMessage,
		Success,
		Fail,
		OnlineContacts,
		ChangePassword
	}
}
