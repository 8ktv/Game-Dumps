using System.IO;

namespace Mirror.BouncyCastle.Bcpg;

public abstract class ContainedPacket : Packet
{
	public byte[] GetEncoded()
	{
		MemoryStream memoryStream = new MemoryStream();
		using (BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream))
		{
			bcpgOutputStream.WritePacket(this);
		}
		return memoryStream.ToArray();
	}

	public abstract void Encode(BcpgOutputStream bcpgOut);
}
