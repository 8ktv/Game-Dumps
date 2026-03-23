using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Bcpg.Sig;

public class SignatureTarget : SignatureSubpacket
{
	public int PublicKeyAlgorithm => data[0];

	public int HashAlgorithm => data[1];

	public SignatureTarget(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.SignatureTarget, critical, isLongLength, data)
	{
	}

	public SignatureTarget(bool critical, int publicKeyAlgorithm, int hashAlgorithm, byte[] hashData)
		: base(SignatureSubpacketTag.SignatureTarget, critical, isLongLength: false, Arrays.Concatenate(new byte[2]
		{
			(byte)publicKeyAlgorithm,
			(byte)hashAlgorithm
		}, hashData))
	{
	}

	public byte[] GetHashData()
	{
		return Arrays.CopyOfRange(data, 2, data.Length);
	}
}
