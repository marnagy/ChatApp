using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ChatLib
{
	[Serializable]
	public struct Password : ISerializable
	{
		const string ValueSerializationName = "Passwd";
		private readonly string Value;
		public Password(string value)
		{
			Value = value;
		}
		public Password(SerializationInfo info, StreamingContext context)
		{
			Value = (string)info.GetValue( ValueSerializationName, typeof(string));
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
