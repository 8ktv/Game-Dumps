using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Bcpg.Sig;

public class IssuerKeyId : SignatureSubpacket
{
	public long KeyId => (long)Pack.BE_To_UInt64(data);

	protected static byte[] KeyIdToBytes(long keyId)
	{
		return Pack.UInt64_To_BE((ulong)keyId);
	}

	public IssuerKeyId(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.IssuerKeyId, critical, isLongLength, data)
	{
	}

	public IssuerKeyId(bool critical, long keyId)
		: base(SignatureSubpacketTag.IssuerKeyId, critical, isLongLength: false, KeyIdToBytes(keyId))
	{
	}
}
