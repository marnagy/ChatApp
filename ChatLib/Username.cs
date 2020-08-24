using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ChatLib
{
	[Serializable]
	public struct Username : IComparable<Username>, ISerializable
	{
		const string ValueSerializationName = "Username";
		private readonly string Value;
		public Username(string value)
		{
			Value = value;
		}
		public Username(SerializationInfo info, StreamingContext context)
		{
			Value = (string)info.GetValue( ValueSerializationName, typeof(string));
		}

		public int CompareTo(Username other)
		{
			return string.Compare(Value, other.Value);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( ValueSerializationName, Value);
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
