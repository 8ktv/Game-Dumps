using System;
using System.IO;
using Mirror.BouncyCastle.Utilities.Date;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpLiteralData : PgpObject
{
	public const char Binary = 'b';

	public const char Text = 't';

	public const char Utf8 = 'u';

	public const string Console = "_CONSOLE";

	private readonly LiteralDataPacket data;

	public int Format => data.Format;

	public string FileName => data.FileName;

	public DateTime ModificationTime => DateTimeUtilities.UnixMsToDateTime(data.ModificationTime);

	public PgpLiteralData(BcpgInputStream bcpgInput)
	{
		Packet packet = bcpgInput.ReadPacket();
		if (!(packet is LiteralDataPacket literalDataPacket))
		{
			throw new IOException("unexpected packet in stream: " + packet);
		}
		data = literalDataPacket;
	}

	public byte[] GetRawFileName()
	{
		return data.GetRawFileName();
	}

	public Stream GetInputStream()
	{
		return data.GetInputStream();
	}

	public Stream GetDataStream()
	{
		return GetInputStream();
	}
}
