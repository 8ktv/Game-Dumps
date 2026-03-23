using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto;

public interface IDsa
{
	string AlgorithmName { get; }

	BigInteger Order { get; }

	void Init(bool forSigning, ICipherParameters parameters);

	BigInteger[] GenerateSignature(byte[] message);

	bool VerifySignature(byte[] message, BigInteger r, BigInteger s);
}
