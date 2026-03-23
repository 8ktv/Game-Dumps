using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Bcpg.Sig;

public class IntendedRecipientFingerprint : SignatureSubpacket
{
	public int KeyVersion => data[0];

	public IntendedRecipientFingerprint(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.IntendedRecipientFingerprint, critical, isLongLength, data)
	{
	}

	public IntendedRecipientFingerprint(bool critical, int keyVersion, byte[] fingerprint)
		: base(SignatureSubpacketTag.IntendedRecipientFingerprint, critical, isLongLength: false, Arrays.Prepend(fingerprint, (byte)keyVersion))
	{
	}

	public byte[] GetFingerprint()
	{
		return Arrays.CopyOfRange(data, 1, data.Length);
	}
}
