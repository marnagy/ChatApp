using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ChatLib.BinaryFormatters
{
	public class BinaryFormatterReader
	{
		private readonly BinaryFormatter deformatter;
		private readonly Stream stream;
		public bool IsClosed { get; private set;} = false;
		public bool IsEmpty => stream.Length == 0;
		public BinaryFormatterReader(BinaryFormatter bf, Stream stream)
		{
			if ( stream == null) throw new ArgumentException("Stream cannot be null.");
			if (!stream.CanRead) throw new ArgumentException("Stream cannot be written to.");
			this.stream = stream;
			deformatter = bf;
		}

		public object Read()
		{
			return deformatter.Deserialize(stream);
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
