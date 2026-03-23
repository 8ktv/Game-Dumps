using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Bike;

public sealed class BikeKemExtractor : IEncapsulatedSecretExtractor
{
	private readonly BikeKeyParameters key;

	public int EncapsulationLength => key.Parameters.RByte + key.Parameters.LByte;

	public BikeKemExtractor(BikePrivateKeyParameters privParams)
	{
		key = privParams;
	}

	public byte[] ExtractSecret(byte[] encapsulation)
	{
		BikeParameters parameters = key.Parameters;
		BikeEngine bikeEngine = parameters.BikeEngine;
		int defaultKeySize = parameters.DefaultKeySize;
		byte[] array = new byte[bikeEngine.SessionKeySize];
		BikePrivateKeyParameters bikePrivateKeyParameters = (BikePrivateKeyParameters)key;
		byte[] c = Arrays.CopyOfRange(encapsulation, 0, bikePrivateKeyParameters.Parameters.RByte);
		byte[] c2 = Arrays.CopyOfRange(encapsulation, bikePrivateKeyParameters.Parameters.RByte, encapsulation.Length);
		byte[] h = bikePrivateKeyParameters.m_h0;
		byte[] h2 = bikePrivateKeyParameters.m_h1;
		byte[] sigma = bikePrivateKeyParameters.m_sigma;
		bikeEngine.Decaps(array, h, h2, sigma, c, c2);
		return Arrays.CopyOfRange(array, 0, defaultKeySize / 8);
	}
}
