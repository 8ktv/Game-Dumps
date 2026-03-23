using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Signers;

public interface IDsaKCalculator
{
	bool IsDeterministic { get; }

	void Init(BigInteger n, SecureRandom random);

	void Init(BigInteger n, BigInteger d, byte[] message);

	BigInteger NextK();
}
