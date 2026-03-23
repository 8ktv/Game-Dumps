using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

public sealed class SphincsPlusKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private SecureRandom random;

	private SphincsPlusParameters parameters;

	public void Init(KeyGenerationParameters param)
	{
		random = param.Random;
		parameters = ((SphincsPlusKeyGenerationParameters)param).Parameters;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		SphincsPlusEngine engine = parameters.GetEngine();
		byte[] array3;
		SK sK;
		if (engine is SphincsPlusEngine.HarakaSEngine)
		{
			byte[] sourceArray = SecRand(engine.N * 3);
			byte[] array = new byte[engine.N];
			byte[] array2 = new byte[engine.N];
			array3 = new byte[engine.N];
			Array.Copy(sourceArray, 0, array, 0, engine.N);
			Array.Copy(sourceArray, engine.N, array2, 0, engine.N);
			Array.Copy(sourceArray, engine.N << 1, array3, 0, engine.N);
			sK = new SK(array, array2);
		}
		else
		{
			sK = new SK(SecRand(engine.N), SecRand(engine.N));
			array3 = SecRand(engine.N);
		}
		engine.Init(array3);
		PK pk = new PK(array3, new HT(engine, sK.seed, array3).HTPubKey);
		return new AsymmetricCipherKeyPair(new SphincsPlusPublicKeyParameters(parameters, pk), new SphincsPlusPrivateKeyParameters(parameters, sK, pk));
	}

	private byte[] SecRand(int n)
	{
		return SecureRandom.GetNextBytes(random, n);
	}
}
