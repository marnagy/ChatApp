using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Runtime.Serialization;

namespace ChatLib
{
	[Serializable]
	public struct  Email : ISerializable
	{
		const string ValueSerializationName = "E-mail";
		private readonly MailAddress Address;
		public Email(string value)
		{
			Address = new MailAddress(value);
		}
		public Email(SerializationInfo info, StreamingContext context)
		{
			Address = new MailAddress( (string)info.GetValue( ValueSerializationName, typeof(string) ) );
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( ValueSerializationName, Address.ToString());
		}
		public override string ToString()
		{
			return Address.ToString();
		}
	}
}
