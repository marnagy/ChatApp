using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using ChatLib.Messages;
using System.Linq;
using System.Text.RegularExpressions;
using ChatLib.BinaryFormatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatLib
{
	[Serializable]
	public sealed class ChatInfo : ISerializable, IComparable<ChatInfo>
	{
		private static readonly DateTime defaultDT = new DateTime(9999,1,1);
		private static Regex regex = null;

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

		public ChatInfo(SerializationInfo info, StreamingContext context)
		{
			//info.AddValue("ChatType", Type);
			//info.AddValue("ChatID", ID);
			//info.AddValue("ChatName", Name);
			//info.AddValue("Participants", participants);
			//info.AddValue("Messages", messages);

			var type = (ChatType)info.GetValue( "SimpleChats", typeof(ChatType));
			ID = (long)info.GetValue( "SimpleChats", typeof(long));
			Name = (string)info.GetValue( "SimpleChats", typeof(string));
			participants = (Username[])info.GetValue( "SimpleChats", typeof(Username[]));
			messages = (List<Message>)info.GetValue( "SimpleChats", typeof(List<Message>));
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
			//info.AddValue("LastMessageDateTime", lastMessageTime);
			info.AddValue("Participants", participants);
			info.AddValue("Messages", messages);
		}

		public int CompareTo(ChatInfo other)
		{
			return DateTime.Compare(this.lastMessageTime, other.lastMessageTime);
		}

		public static ChatInfo FromDictionary(ChatType type, DirectoryInfo directoryInfo, string infoFileName)
		{
			List<Username> participants = new List<Username>();
			
			var usernames = File.ReadAllLines(Path.Combine(directoryInfo.FullName, infoFileName));

			var name = usernames[0];
			for (int i = 1; i < usernames.Length; i++)
			//foreach (var username in usernames)
			{
				participants.Add(usernames[i].ToUsername());
			}

			LoadMessages(directoryInfo);

			return new ChatInfo(type, long.Parse(directoryInfo.Name), name, participants.ToArray() );
		}

		private static List<Message> LoadMessages(DirectoryInfo directoryInfo)
		{
			if (regex == null)
			{
				// Regex for format YYYY-MM-DD without checking the amount of days in month
				regex = new Regex("^([12][0-9]{3}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01]))$", RegexOptions.Compiled);
			}

			var result = new List<Message>();
			var files = new List<FileInfo>(directoryInfo.GetFiles());

			// filter
			for (int i = 0; i < files.Count; i++)
			{
				if ( !regex.IsMatch(files[i].Name) )
				{
					files.RemoveAt(i);
					i--;
					continue;
				}
			}

			BinaryFormatterReader bfr;
			foreach (var file in files)
			{
				bfr = new BinaryFormatterReader( new BinaryFormatter(), file.Open(FileMode.Open, FileAccess.Read) );

				while ( !bfr.IsEmpty)
				{
					result.Add( (Message)bfr.Read() );
				}
				bfr.Close();
			}

			return result;
		}
	}
}
