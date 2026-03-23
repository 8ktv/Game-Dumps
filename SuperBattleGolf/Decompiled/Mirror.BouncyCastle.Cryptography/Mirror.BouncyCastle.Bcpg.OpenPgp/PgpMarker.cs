using System.IO;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpMarker : PgpObject
{
	private readonly MarkerPacket data;

	public PgpMarker(BcpgInputStream bcpgInput)
	{
		Packet packet = bcpgInput.ReadPacket();
		if (!(packet is MarkerPacket markerPacket))
		{
			throw new IOException("unexpected packet in stream: " + packet);
		}
		data = markerPacket;
	}
}
