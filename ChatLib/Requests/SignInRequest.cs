using ChatLib.BinaryFormatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Requests
{
	[Serializable]
	public class SignInRequest : Request
	{
		

		public const RequestType type = RequestType.SignIn;
		public readonly Username username;
		public readonly Password password;
		//public readonly long sessionID;
		public SignInRequest(Username username, Password password, long sessionID) : base(type, sessionID)
		{
			this.username = username;
			this.password = password;
		}
		public SignInRequest(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			Username username = (Username)info.GetValue( UsernameSerializationName, typeof(Username));
			Password password = (Password)info.GetValue( PasswordSerializationName, typeof(Password));

			this.username = username;
			this.password = password;
		}
		public static SignInRequest Read(BinaryFormatterReader reader, long sessionID)
		{
			Username username = ((string)reader.Read()).ToUsername();
			Password password = ((string)reader.Read()).ToPassword();
			return new SignInRequest(username, password, sessionID);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);
			info.AddValue( UsernameSerializationName, username);
			info.AddValue( PasswordSerializationName, password);
		}
	}
}
