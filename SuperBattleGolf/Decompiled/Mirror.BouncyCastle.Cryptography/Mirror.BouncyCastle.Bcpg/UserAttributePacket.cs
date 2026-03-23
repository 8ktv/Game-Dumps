using System.Collections.Generic;
using System.IO;

namespace Mirror.BouncyCastle.Bcpg;

public class UserAttributePacket : ContainedPacket
{
	private readonly UserAttributeSubpacket[] subpackets;

	public UserAttributePacket(BcpgInputStream bcpgIn)
	{
		UserAttributeSubpacketsParser userAttributeSubpacketsParser = new UserAttributeSubpacketsParser(bcpgIn);
		List<UserAttributeSubpacket> list = new List<UserAttributeSubpacket>();
		UserAttributeSubpacket item;
		while ((item = userAttributeSubpacketsParser.ReadPacket()) != null)
		{
			list.Add(item);
		}
		subpackets = list.ToArray();
	}

	public UserAttributePacket(UserAttributeSubpacket[] subpackets)
	{
		this.subpackets = subpackets;
	}

	public UserAttributeSubpacket[] GetSubpackets()
	{
		return subpackets;
	}

	public override void Encode(BcpgOutputStream bcpgOut)
	{
		MemoryStream memoryStream = new MemoryStream();
		for (int i = 0; i != subpackets.Length; i++)
		{
			subpackets[i].Encode(memoryStream);
		}
		bcpgOut.WritePacket(PacketTag.UserAttribute, memoryStream.ToArray());
	}
}
