using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Bike;

public sealed class BikeKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private SecureRandom random;

	private int r;

	private int l;

	private int L_BYTE;

	private int R_BYTE;

	private BikeKeyGenerationParameters bikeKeyGenerationParameters;

	public void Init(KeyGenerationParameters param)
	{
		bikeKeyGenerationParameters = (BikeKeyGenerationParameters)param;
		random = param.Random;
		r = bikeKeyGenerationParameters.Parameters.R;
		l = bikeKeyGenerationParameters.Parameters.L;
		L_BYTE = l / 8;
		R_BYTE = (r + 7) / 8;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		BikeParameters parameters = bikeKeyGenerationParameters.Parameters;
		BikeEngine bikeEngine = parameters.BikeEngine;
		byte[] h = new byte[R_BYTE];
		byte[] h2 = new byte[R_BYTE];
		byte[] array = new byte[R_BYTE];
		byte[] sigma = new byte[L_BYTE];
		bikeEngine.GenKeyPair(h, h2, sigma, array, random);
		BikePublicKeyParameters publicParameter = new BikePublicKeyParameters(parameters, array);
		BikePrivateKeyParameters privateParameter = new BikePrivateKeyParameters(parameters, h, h2, sigma);
		return new AsymmetricCipherKeyPair(publicParameter, privateParameter);
	}
}
