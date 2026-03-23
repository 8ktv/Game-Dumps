namespace Mirror.BouncyCastle.Bcpg.Sig;

public class PrimaryUserId : SignatureSubpacket
{
	public PrimaryUserId(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.PrimaryUserId, critical, isLongLength, data)
	{
	}

	public PrimaryUserId(bool critical, bool isPrimaryUserId)
		: base(SignatureSubpacketTag.PrimaryUserId, critical, isLongLength: false, Utilities.BooleanToBytes(isPrimaryUserId))
	{
	}

	public bool IsPrimaryUserId()
	{
		return Utilities.BooleanFromBytes(data);
	}
}
