using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpEncryptedDataList : PgpObject
{
	private readonly List<PgpEncryptedData> m_list = new List<PgpEncryptedData>();

	private readonly InputStreamPacket m_data;

	public PgpEncryptedData this[int index] => m_list[index];

	public int Count => m_list.Count;

	public bool IsEmpty => m_list.Count == 0;

	public PgpEncryptedDataList(BcpgInputStream bcpgInput)
	{
		List<Packet> list = new List<Packet>();
		while (bcpgInput.NextPacketTag() == PacketTag.PublicKeyEncryptedSession || bcpgInput.NextPacketTag() == PacketTag.SymmetricKeyEncryptedSessionKey)
		{
			list.Add(bcpgInput.ReadPacket());
		}
		Packet packet = bcpgInput.ReadPacket();
		if (!(packet is InputStreamPacket data))
		{
			throw new IOException("unexpected packet in stream: " + packet);
		}
		m_data = data;
		foreach (Packet item in list)
		{
			if (item is SymmetricKeyEncSessionPacket keyData)
			{
				m_list.Add(new PgpPbeEncryptedData(keyData, m_data));
				continue;
			}
			if (item is PublicKeyEncSessionPacket keyData2)
			{
				m_list.Add(new PgpPublicKeyEncryptedData(keyData2, m_data));
				continue;
			}
			throw new InvalidOperationException();
		}
	}

	public IEnumerable<PgpEncryptedData> GetEncryptedDataObjects()
	{
		return CollectionUtilities.Proxy(m_list);
	}
}
