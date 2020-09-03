using ChatLib.Messages;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Requests
{
	[Serializable]
	public class ChangePasswordRequest : Request
	{
		private const RequestType type = RequestType.ChangePassword;
		public readonly Username username;
		public readonly Password oldPassword;
		public readonly Password newPassword;

		public ChangePasswordRequest(Username username, string oldPassword, string newPassword,
			long sessionID) : base(type, sessionID)
		{
			this.username = username;
			this.oldPassword = oldPassword.ToPassword();
			this.newPassword = newPassword.ToPassword();
		}
		public ChangePasswordRequest(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			username = (Username)info.GetValue( UsernameSerializationName, typeof(Username));
			oldPassword = (Password)info.GetValue( OldPasswordSerializationName, typeof(Password));
			newPassword = (Password)info.GetValue( NewPasswordSerializationName, typeof(Password));
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);

			info.AddValue( UsernameSerializationName, username);
			info.AddValue( OldPasswordSerializationName, oldPassword);
			info.AddValue( NewPasswordSerializationName, newPassword);
		}
	}
}
