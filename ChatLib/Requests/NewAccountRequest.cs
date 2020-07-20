using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatLib.Requests
{
	public struct NewAccountRequest : IRequest
	{
		public const RequestType type = RequestType.NewAccount;
		public readonly Email email;
		public readonly Username username;
		public readonly Password password;
		private readonly long sessionID;
		public NewAccountRequest(Email email, Username username, Password password, long sessionID)
		{
			this.email = email;
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
				writer.Write(email.ToString());
				writer.Write(username.ToString());
				writer.Write(password.ToString());
				writer.Flush();
			}
		}
		public static NewAccountRequest Read(BinaryReader reader, long sessionID)
		{
			Email email = reader.ReadString().ToEmail();
			Username username = reader.ReadString().ToUsername();
			Password password = reader.ReadString().ToPassword();
			return new NewAccountRequest(email, username, password, sessionID);
		}
	}
}
