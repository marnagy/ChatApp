using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Responses
{
	[Serializable]
	public sealed class ChangePasswordResponse : Response
	{
		const ResponseType type = ResponseType.ChangePassword;
		private readonly bool success;
		public ChangePasswordResponse(bool success, long sessionID) : base(type, sessionID)
		{
			this.success = success;
		}
		public ChangePasswordResponse(SerializationInfo info, StreamingContext context) : base(LoadParentAttributes(info, context))
		{
			success = (bool)info.GetValue( SuccessSerializationName, typeof(bool));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);
			info.AddValue( SuccessSerializationName, success);
		}
	}
}
