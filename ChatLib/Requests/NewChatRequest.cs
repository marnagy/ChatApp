using ChatLib.Responses;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ChatLib.Requests
{
	[Serializable]
	public class NewChatRequest : Request
	{
		private const RequestType type = RequestType.NewChat;
		public readonly ChatType chatType;
		public readonly Username[] participants;

		public NewChatRequest(Username creator, Username participant, long sessionID) : base(type, sessionID)
		{
			chatType = ChatType.Simple;
			participants = new Username[] {creator, participant};
		}
		public NewChatRequest(Username creator, Username[] otherParticipants, long sessionID) : base(type, sessionID)
		{
			chatType = ChatType.Group;
			participants = new Username[1 + otherParticipants.Length];
			participants[0] = creator;
			for (int i = 0; i < otherParticipants.Length; i++)
			{
				participants[1 + i] = otherParticipants[i];
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue( SessionIDSerializationName, SessionID);
			info.AddValue( TypeSerializationName, Type);
			info.AddValue( ParticipantsSerializationName, participants);
		}
	}
}
