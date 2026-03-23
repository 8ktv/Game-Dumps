using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

public class FalconKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private FalconKeyGenerationParameters parameters;

	private SecureRandom random;

	private FalconNist nist;

	private uint logn;

	private uint noncelen;

	private int pk_size;

	public void Init(KeyGenerationParameters param)
	{
		parameters = (FalconKeyGenerationParameters)param;
		random = param.Random;
		logn = (uint)((FalconKeyGenerationParameters)param).Parameters.LogN;
		noncelen = (uint)((FalconKeyGenerationParameters)param).Parameters.NonceLength;
		nist = new FalconNist(random, logn, noncelen);
		int num = 1 << (int)logn;
		pk_size = 1 + 14 * num / 8;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		nist.crypto_sign_keypair(out var pk, out var fEnc, out var gEnc, out var FEnc);
		FalconParameters falconParameters = parameters.Parameters;
		return new AsymmetricCipherKeyPair(privateParameter: new FalconPrivateKeyParameters(falconParameters, fEnc, gEnc, FEnc, pk), publicParameter: new FalconPublicKeyParameters(falconParameters, pk));
	}
}
