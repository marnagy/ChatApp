using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatLib.Responses
{
	public struct AccountInfoResponse : IResponse
	{
		private readonly long sessionID;
		
		public void Send(BinaryWriter writer)
		{
			throw new NotImplementedException();
		}
	}
}
