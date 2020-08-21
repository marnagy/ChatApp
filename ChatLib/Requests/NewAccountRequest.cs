using ChatLib.BinaryFormatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Requests
{
	[Serializable]
	public class NewAccountRequest : Request
	{
		public const RequestType type = RequestType.NewAccount;
		public readonly Email email;
		public readonly Username username;
		public readonly Password password;
		public NewAccountRequest(Email email, Username username, Password password, long sessionID) : base(type, sessionID)
		{
			this.email = email;
			this.username = username;
			this.password = password;
		}
		public NewAccountRequest(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			Email email = (Email)info.GetValue( EmailSerializationName, typeof(Email));
			Username username = (Username)info.GetValue( UsernameSerializationName, typeof(Username));
			Password password = (Password)info.GetValue( PasswordSerializationName, typeof(Password));

			this.email = email;
			this.username = username;
			this.password = password;
		}

		public static NewAccountRequest Read(BinaryFormatterReader reader, long sessionID)
		{
			Email email = ((string)reader.Read()).ToEmail();
			Username username = ((string)reader.Read()).ToUsername();
			Password password = ((string)reader.Read()).ToPassword();
			return new NewAccountRequest(email, username, password, sessionID);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(SessionIDSerializationName, SessionID);
			info.AddValue(TypeSerializationName, Type);
			info.AddValue(EmailSerializationName, email);
			info.AddValue(UsernameSerializationName, username);
			info.AddValue(PasswordSerializationName, password);
		}
	}
}
