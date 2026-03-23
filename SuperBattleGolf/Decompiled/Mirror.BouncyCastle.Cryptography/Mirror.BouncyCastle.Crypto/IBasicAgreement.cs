using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto;

public interface IBasicAgreement
{
	void Init(ICipherParameters parameters);

	int GetFieldSize();

	BigInteger CalculateAgreement(ICipherParameters pubKey);
}
