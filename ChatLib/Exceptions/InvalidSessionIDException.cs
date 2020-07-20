using System;
using System.Runtime.Serialization;

namespace ChatClient
{
	[Serializable]
	public class InvalidSessionIDException : Exception
	{
		public InvalidSessionIDException()
		{
		}

		public InvalidSessionIDException(string message) : base(message)
		{
		}

		public InvalidSessionIDException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidSessionIDException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}