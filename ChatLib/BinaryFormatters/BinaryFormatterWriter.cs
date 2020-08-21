using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ChatLib.BinaryFormatters
{
	public class BinaryFormatterWriter
	{
		private readonly BinaryFormatter formatter;
		private readonly Stream stream;
		public bool IsClosed { get; private set;} = false;
		public BinaryFormatterWriter(BinaryFormatter bf, Stream stream)
		{
			if (  stream == null ) throw new ArgumentNullException("Stream cannot be null.");
			if ( !stream.CanWrite) throw new ArgumentException("Stream cannot be written to.");
			this.stream = stream;
			formatter = bf;
		}

		public void Write(object obj)
		{
			if (obj == null) throw new ArgumentNullException();
			formatter.Serialize(stream, obj);
		}

		public bool Close()
		{
			lock (stream)
			{
				if (!IsClosed)
				{
					if (stream.Length == 0) {
						stream.Close();
						IsClosed = true;
						return true;
					}
					else
						return false;
				}
				else
					return true;
			}
		}
	}
}
