using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Hqc;

public class HqcKemGenerator : IEncapsulatedSecretGenerator
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

	private SecureRandom sr;

	public HqcKemGenerator(SecureRandom random)
	{
		sr = random;
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey)
	{
		HqcPublicKeyParameters obj = (HqcPublicKeyParameters)recipientKey;
		HqcEngine engine = obj.Parameters.Engine;
		byte[] array = new byte[obj.Parameters.Sha512Bytes];
		byte[] array2 = new byte[obj.Parameters.NBytes];
		byte[] array3 = new byte[obj.Parameters.N1n2Bytes];
		byte[] array4 = new byte[obj.Parameters.Sha512Bytes];
		byte[] array5 = new byte[obj.Parameters.SaltSizeBytes];
		byte[] publicKey = obj.PublicKey;
		byte[] array6 = new byte[48];
		sr.NextBytes(array6);
		engine.Encaps(array2, array3, array, array4, publicKey, array6, array5);
		byte[] cipher_text = Arrays.ConcatenateAll(array2, array3, array4, array5);
		return new SecretWithEncapsulationImpl(array, cipher_text);
	}
}
