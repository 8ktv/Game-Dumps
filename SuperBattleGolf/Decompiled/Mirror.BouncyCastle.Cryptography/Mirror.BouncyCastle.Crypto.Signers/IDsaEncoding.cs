using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto.Signers;

public interface IDsaEncoding
{
	BigInteger[] Decode(BigInteger n, byte[] encoding);

	byte[] Encode(BigInteger n, BigInteger r, BigInteger s);

	int GetMaxEncodingSize(BigInteger n);
}
