using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

public sealed class CmceKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private CmceKeyGenerationParameters m_cmceParams;

	private SecureRandom random;

	private void Initialize(KeyGenerationParameters param)
	{
		m_cmceParams = (CmceKeyGenerationParameters)param;
		random = param.Random;
	}

	private AsymmetricCipherKeyPair GenKeyPair()
	{
		ICmceEngine engine = m_cmceParams.Parameters.Engine;
		byte[] array = new byte[engine.PrivateKeySize];
		byte[] array2 = new byte[engine.PublicKeySize];
		engine.KemKeypair(array2, array, random);
		CmcePublicKeyParameters publicParameter = new CmcePublicKeyParameters(m_cmceParams.Parameters, array2);
		CmcePrivateKeyParameters privateParameter = new CmcePrivateKeyParameters(m_cmceParams.Parameters, array);
		return new AsymmetricCipherKeyPair(publicParameter, privateParameter);
	}

	public void Init(KeyGenerationParameters param)
	{
		Initialize(param);
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		return GenKeyPair();
	}
}
