using System;
using System.Runtime.Serialization;

namespace ChatClient
{
	[Serializable]
	public class InvalidResponseException : Exception
	{
		public InvalidResponseException()
		{
		}

		public InvalidResponseException(string message) : base(message)
		{
		}

		public InvalidResponseException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}