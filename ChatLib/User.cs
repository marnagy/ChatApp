using System;
using System.Collections.Generic;
using System.Text;

namespace ChatLib
{
	public class User
	{
		public readonly Email email;
		public readonly Username username;
		
		public User(string username, string email)
		{
			this.email = email.ToEmail();
			this.username = username.ToUsername();
		}
	}
}
