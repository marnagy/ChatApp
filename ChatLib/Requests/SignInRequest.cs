using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatLib.Requests
{
	public struct SignInRequest : IRequest
	{
		public const RequestType type = RequestType.SignIn;
		public readonly Username username;
		public readonly Password password;
		long sessionID;
		public SignInRequest(Username username, Password password, long sessionID)
		{
			this.username = username;
			this.password = password;
			this.sessionID = sessionID;
		}
		public void Send(BinaryWriter writer)
		{
			lock (writer)
			{
				writer.Write(sessionID);
				writer.Write(type.ToString());
				writer.Write(username.ToString());
				writer.Write(password.ToString());
				writer.Flush();
			}
		}
		public static SignInRequest Read(BinaryReader reader, long sessionID)
		{
			Username username = reader.ReadString().ToUsername();
			Password password = reader.ReadString().ToPassword();
			return new SignInRequest(username, password, sessionID);
		}
	}
}
