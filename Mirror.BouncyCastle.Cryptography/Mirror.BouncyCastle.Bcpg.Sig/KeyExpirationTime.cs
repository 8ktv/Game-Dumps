using System;

namespace Mirror.BouncyCastle.Bcpg.Sig;

public class KeyExpirationTime : SignatureSubpacket
{
	public long Time => Utilities.TimeFromBytes(data);

	[Obsolete("Will be removed")]
	protected static byte[] TimeToBytes(long t)
	{
		return Utilities.TimeToBytes((uint)t);
	}

	public KeyExpirationTime(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.KeyExpireTime, critical, isLongLength, data)
	{
	}

	public KeyExpirationTime(bool critical, long seconds)
		: base(SignatureSubpacketTag.KeyExpireTime, critical, isLongLength: false, Utilities.TimeToBytes((uint)seconds))
	{
	}
}
