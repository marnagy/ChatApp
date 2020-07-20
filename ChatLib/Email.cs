using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace ChatLib
{
	public struct  Email
	{
		private readonly MailAddress Address;
		public Email(string value)
		{
			Address = new MailAddress(value);
		}
		public override string ToString()
		{
			return Address.ToString();
		}
	}
}
