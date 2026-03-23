namespace Mirror.BouncyCastle.Bcpg.Sig;

public class Exportable : SignatureSubpacket
{
	public Exportable(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.Exportable, critical, isLongLength, data)
	{
	}

	public Exportable(bool critical, bool isExportable)
		: base(SignatureSubpacketTag.Exportable, critical, isLongLength: false, Utilities.BooleanToBytes(isExportable))
	{
	}

	public bool IsExportable()
	{
		return Utilities.BooleanFromBytes(data);
	}
}
