using System;

namespace Mirror.BouncyCastle.Bcpg.Sig;

public class SignatureExpirationTime : SignatureSubpacket
{
	public long Time => Utilities.TimeFromBytes(data);

	[Obsolete("Will be removed")]
	protected static byte[] TimeToBytes(long t)
	{
		return Utilities.TimeToBytes((uint)t);
	}

	public SignatureExpirationTime(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.ExpireTime, critical, isLongLength, data)
	{
	}

	public SignatureExpirationTime(bool critical, long seconds)
		: base(SignatureSubpacketTag.ExpireTime, critical, isLongLength: false, Utilities.TimeToBytes((uint)seconds))
	{
	}
}
