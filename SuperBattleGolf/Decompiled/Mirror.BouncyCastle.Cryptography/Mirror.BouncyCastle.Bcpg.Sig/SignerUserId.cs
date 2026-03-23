using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Bcpg.Sig;

public class SignerUserId : SignatureSubpacket
{
	public SignerUserId(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.SignerUserId, critical, isLongLength, data)
	{
	}

	public SignerUserId(bool critical, string userId)
		: base(SignatureSubpacketTag.SignerUserId, critical, isLongLength: false, Strings.ToUtf8ByteArray(userId))
	{
	}

	public string GetId()
	{
		return Strings.FromUtf8ByteArray(data);
	}

	public byte[] GetRawId()
	{
		return Arrays.Clone(data);
	}
}
