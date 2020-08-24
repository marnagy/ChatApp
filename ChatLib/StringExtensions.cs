using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ChatLib
{
	public static class StringExtensions
	{
		public static Username ToUsername(this string username)
		{
			return new Username(username);
		}
		public static Email ToEmail(this string email)
		{
			return new Email(email);
		}
		public static Password ToPassword(this string password)
		{
			return new Password(password);
		}
		public static ChatID ToChatID(this string chatID)
		{
			return new ChatID(chatID);
		}
	}
}
