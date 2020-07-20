using System;
using System.Collections.Generic;
using System.Text;

namespace ChatLib
{
	public struct Username : IComparable<Username>
	{
		private readonly string Value;
		public Username(string value)
		{
			Value = value;
		}

		public int CompareTo(Username other)
		{
			return string.Compare(Value, other.Value);
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
