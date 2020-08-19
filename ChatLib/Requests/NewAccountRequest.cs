using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Requests
{
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

		public static NewAccountRequest Read(BinaryReader reader, long sessionID)
		{
			Email email = reader.ReadString().ToEmail();
			Username username = reader.ReadString().ToUsername();
			Password password = reader.ReadString().ToPassword();
			return new NewAccountRequest(email, username, password, sessionID);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("SessionID", SessionID);
			info.AddValue("Type", Type);
			info.AddValue("Email", email);
			info.AddValue("Username", username);
			info.AddValue("Password", password);
		}
	}
}
