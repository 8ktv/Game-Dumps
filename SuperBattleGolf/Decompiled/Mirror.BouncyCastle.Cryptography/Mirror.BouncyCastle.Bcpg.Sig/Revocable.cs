namespace Mirror.BouncyCastle.Bcpg.Sig;

public class Revocable : SignatureSubpacket
{
	public Revocable(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.Revocable, critical, isLongLength, data)
	{
	}

	public Revocable(bool critical, bool isRevocable)
		: base(SignatureSubpacketTag.Revocable, critical, isLongLength: false, Utilities.BooleanToBytes(isRevocable))
	{
	}

	public bool IsRevocable()
	{
		return Utilities.BooleanFromBytes(data);
	}
}
