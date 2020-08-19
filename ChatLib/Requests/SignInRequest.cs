using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Requests
{
	public class SignInRequest : Request
	{
		public const RequestType type = RequestType.SignIn;
		public readonly Username username;
		public readonly Password password;
		long sessionID;
		public SignInRequest(Username username, Password password, long sessionID) : base(type, sessionID)
		{
			this.username = username;
			this.password = password;
			this.sessionID = sessionID;
		}
		public static SignInRequest Read(BinaryReader reader, long sessionID)
		{
			Username username = reader.ReadString().ToUsername();
			Password password = reader.ReadString().ToPassword();
			return new SignInRequest(username, password, sessionID);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("SessionID", sessionID);
			info.AddValue("Type", Type);
			info.AddValue("Username", username);
			info.AddValue("Password", password);
		}
	}
}
