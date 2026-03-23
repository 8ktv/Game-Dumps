using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Bike;

public sealed class BikeKemGenerator : IEncapsulatedSecretGenerator
{
	private sealed class SecretWithEncapsulationImpl : ISecretWithEncapsulation, IDisposable
	{
		private volatile bool hasBeenDestroyed;

		private byte[] sessionKey;

		private byte[] cipher_text;

		public SecretWithEncapsulationImpl(byte[] sessionKey, byte[] cipher_text)
		{
			this.sessionKey = sessionKey;
			this.cipher_text = cipher_text;
		}

		public byte[] GetSecret()
		{
			CheckDestroyed();
			return Arrays.Clone(sessionKey);
		}

		public byte[] GetEncapsulation()
		{
			CheckDestroyed();
			return Arrays.Clone(cipher_text);
		}

		public void Dispose()
		{
			if (!hasBeenDestroyed)
			{
				Arrays.Clear(sessionKey);
				Arrays.Clear(cipher_text);
				hasBeenDestroyed = true;
			}
			GC.SuppressFinalize(this);
		}

		public bool IsDestroyed()
		{
			return hasBeenDestroyed;
		}

		private void CheckDestroyed()
		{
			if (IsDestroyed())
			{
				throw new Exception("data has been destroyed");
			}
		}
	}

	private readonly SecureRandom sr;

	public BikeKemGenerator(SecureRandom random)
	{
		sr = random;
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey)
	{
		BikePublicKeyParameters bikePublicKeyParameters = (BikePublicKeyParameters)recipientKey;
		BikeParameters parameters = bikePublicKeyParameters.Parameters;
		BikeEngine bikeEngine = parameters.BikeEngine;
		byte[] array = new byte[parameters.LByte];
		byte[] array2 = new byte[parameters.RByte + parameters.LByte];
		byte[] publicKey = bikePublicKeyParameters.m_publicKey;
		bikeEngine.Encaps(array2, array, publicKey, sr);
		return new SecretWithEncapsulationImpl(Arrays.CopyOfRange(array, 0, parameters.DefaultKeySize / 8), array2);
	}
}
