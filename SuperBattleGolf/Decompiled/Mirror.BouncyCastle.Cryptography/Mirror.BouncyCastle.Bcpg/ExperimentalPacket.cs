namespace Mirror.BouncyCastle.Bcpg;

public class ExperimentalPacket : ContainedPacket
{
	private readonly PacketTag m_tag;

	private readonly byte[] m_contents;

	public PacketTag Tag => m_tag;

	internal ExperimentalPacket(PacketTag tag, BcpgInputStream bcpgIn)
	{
		m_tag = tag;
		m_contents = bcpgIn.ReadAll();
	}

	public byte[] GetContents()
	{
		return (byte[])m_contents.Clone();
	}

	public override void Encode(BcpgOutputStream bcpgOut)
	{
		bcpgOut.WritePacket(m_tag, m_contents);
	}
}
