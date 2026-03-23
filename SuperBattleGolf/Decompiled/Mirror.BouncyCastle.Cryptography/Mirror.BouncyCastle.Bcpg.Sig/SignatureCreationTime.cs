using System;
using Mirror.BouncyCastle.Utilities.Date;

namespace Mirror.BouncyCastle.Bcpg.Sig;

public class SignatureCreationTime : SignatureSubpacket
{
	[Obsolete("Will be removed")]
	protected static byte[] TimeToBytes(DateTime time)
	{
		return Utilities.TimeToBytes((uint)(DateTimeUtilities.DateTimeToUnixMs(time) / 1000));
	}

	public SignatureCreationTime(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.CreationTime, critical, isLongLength, data)
	{
	}

	public SignatureCreationTime(bool critical, DateTime date)
		: base(SignatureSubpacketTag.CreationTime, critical, isLongLength: false, TimeToBytes(date))
	{
	}

	public DateTime GetTime()
	{
		return DateTimeUtilities.UnixMsToDateTime((long)Utilities.TimeFromBytes(data) * 1000L);
	}
}
