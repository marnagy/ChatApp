using ChatLib.BinaryFormatters;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ChatClient
{
	public class Connection
	{
		public static TcpClient Connect(string hostname, int defaultPort, ref BinaryFormatterWriter writer, ref BinaryFormatterReader reader)
		{
			var client = new TcpClient(AddressFamily.InterNetwork);
			client.Connect(hostname, defaultPort);
			var stream = client.GetStream();
			if (stream == null)
			{
				throw new NullReferenceException();
			}
			var bf = new BinaryFormatter();
			writer = new BinaryFormatterWriter(bf, stream);
			reader = new BinaryFormatterReader(bf, stream);
			return client;
		}

		internal static void Disconnect(ref TcpClient client, ref BinaryFormatterReader reader, ref BinaryFormatterWriter writer)
		{
			client.Close();
			reader = null;
			writer = null;
		}
	}
}
