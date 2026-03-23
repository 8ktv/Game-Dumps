using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls;

public sealed class ServerNameList
{
	private readonly IList<ServerName> m_serverNameList;

	public IList<ServerName> ServerNames => m_serverNameList;

	public ServerNameList(IList<ServerName> serverNameList)
	{
		if (serverNameList == null)
		{
			throw new ArgumentNullException("serverNameList");
		}
		m_serverNameList = serverNameList;
	}

	public void Encode(Stream output)
	{
		MemoryStream memoryStream = new MemoryStream();
		short[] array = TlsUtilities.EmptyShorts;
		foreach (ServerName serverName in ServerNames)
		{
			array = CheckNameType(array, serverName.NameType);
			if (array == null)
			{
				throw new TlsFatalAlert(80);
			}
			serverName.Encode(memoryStream);
		}
		int i = Convert.ToInt32(memoryStream.Length);
		TlsUtilities.CheckUint16(i);
		TlsUtilities.WriteUint16(i, output);
		memoryStream.WriteTo(output);
	}

	public static ServerNameList Parse(Stream input)
	{
		MemoryStream memoryStream = new MemoryStream(TlsUtilities.ReadOpaque16(input, 1), writable: false);
		short[] array = TlsUtilities.EmptyShorts;
		List<ServerName> list = new List<ServerName>();
		while (memoryStream.Position < memoryStream.Length)
		{
			ServerName serverName = ServerName.Parse(memoryStream);
			array = CheckNameType(array, serverName.NameType);
			if (array == null)
			{
				throw new TlsFatalAlert(47);
			}
			list.Add(serverName);
		}
		return new ServerNameList(list);
	}

	private static short[] CheckNameType(short[] nameTypesSeen, short nameType)
	{
		if (Arrays.Contains(nameTypesSeen, nameType))
		{
			return null;
		}
		return Arrays.Append(nameTypesSeen, nameType);
	}
}
