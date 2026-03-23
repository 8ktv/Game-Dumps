namespace Mirror.BouncyCastle.Bcpg;

public class MarkerPacket : ContainedPacket
{
	private readonly byte[] marker = new byte[3] { 80, 71, 80 };

	public MarkerPacket(BcpgInputStream bcpgIn)
	{
		bcpgIn.ReadFully(marker);
	}

	public override void Encode(BcpgOutputStream bcpgOut)
	{
		bcpgOut.WritePacket(PacketTag.Marker, marker);
	}
}
