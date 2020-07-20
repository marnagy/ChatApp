using System;
using System.Collections.Generic;
using System.Text;

namespace ChatLib
{
	public struct Password
	{
		private readonly string Value;
		public Password(string value)
		{
			Value = value;
		}
		public override string ToString()
		{
			return Value;
		}
	}
}
