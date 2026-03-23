using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Bcpg.Sig;

public class IssuerFingerprint : SignatureSubpacket
{
	public int KeyVersion => data[0];

	public IssuerFingerprint(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.IssuerFingerprint, critical, isLongLength, data)
	{
	}

	public IssuerFingerprint(bool critical, int keyVersion, byte[] fingerprint)
		: base(SignatureSubpacketTag.IssuerFingerprint, critical, isLongLength: false, Arrays.Prepend(fingerprint, (byte)keyVersion))
	{
	}

	public byte[] GetFingerprint()
	{
		return Arrays.CopyOfRange(data, 1, data.Length);
	}
}
