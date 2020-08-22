using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using ChatLib.Messages;
using System.Linq;

namespace ChatLib
{
	[Serializable]
	public sealed class ChatInfo : ISerializable, IComparable<ChatInfo>
	{
		public static readonly DateTime defaultDT = new DateTime(9999,1,1);

		public readonly ChatType Type;
		public readonly long ID;
		public readonly string Name;
		public DateTime lastMessageTime { get {
				lock (messages)
				{
					if (messages.Count == 0)
					{
						return defaultDT;
					}
					else
					{
						return messages[messages.Count - 1].Datetime;
					}
				}
			}}
		public readonly Username[] participants;
		private List<Message> messages;
		//public Message lastMessage { get
		//	{

		//	}
		//}
		public ChatInfo(ChatType type, long chatID, string name, Username[] participants, List<Message> messages)
		{
			if ( chatID == null || name == null || participants == null || messages == null )
				throw new ArgumentNullException();
			var now = DateTime.UtcNow;
			//if ( now < lastMessageTime )
			//	throw new ArgumentException("Invalid DateTime of last message.");
			this.Type = type;
			this.ID = chatID;
			this.Name = name;
			this.participants = participants;
			this.messages = messages;
		}
		public ChatInfo(ChatType type, long chatID, string name, Username[] participants) : this(type, chatID, name, participants, new List<Message>())
		{
		}

		public List<Message> GetMessages() => messages;

		public void AddMessage(Message message) {
			if ( messages.Count == 0 )
			{
				messages.Add(message);
			}
			else
			{
				if (message.Datetime < lastMessageTime)
				{
					lock (messages)
					{
						//messages.Add(message);

						for (int i = 0; i < messages.Count; i++)
						{
							if (messages[i].Datetime > message.Datetime)
							{
								messages.Insert(i, message);
								break;
							}
						}
					}
				}
				else
				{
					lock (messages)
					{
						messages.Add(message);
					}
				}
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ChatType", Type);
			info.AddValue("ChatID", ID);
			info.AddValue("ChatName", Name);
			info.AddValue("LastMessageDateTime", lastMessageTime);
			info.AddValue("Participants", participants);
		}

		public int CompareTo(ChatInfo other)
		{
			return DateTime.Compare(this.lastMessageTime, other.lastMessageTime);
		}
	}
}
