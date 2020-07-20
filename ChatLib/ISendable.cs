using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatLib
{
	public interface ISendable
	{
		/// <summary>
		/// Send object to BinaryWriter.
		/// </summary>
		/// <param name="writer">BinaryWriter to write object to.</param>
		void Send(BinaryWriter writer);
	}
}
