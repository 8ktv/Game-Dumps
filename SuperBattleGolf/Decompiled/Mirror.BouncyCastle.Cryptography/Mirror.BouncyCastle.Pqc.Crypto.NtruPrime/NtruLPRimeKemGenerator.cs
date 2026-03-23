using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.NtruPrime;

public class NtruLPRimeKemGenerator : IEncapsulatedSecretGenerator
{
	public class SecretWithEncapsulationImpl : ISecretWithEncapsulation, IDisposable
	{
		private volatile bool hasBeenDestroyed;

		private byte[] sessionKey;

		private byte[] cipherText;

		public SecretWithEncapsulationImpl(byte[] sessionKey, byte[] cipherText)
		{
			this.sessionKey = sessionKey;
			this.cipherText = cipherText;
		}

		public byte[] GetSecret()
		{
			CheckDestroyed();
			return Arrays.Clone(sessionKey);
		}

		public byte[] GetEncapsulation()
		{
			return Arrays.Clone(cipherText);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !hasBeenDestroyed)
			{
				Arrays.Clear(sessionKey);
				Arrays.Clear(cipherText);
				hasBeenDestroyed = true;
			}
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

	public NtruLPRimeKemGenerator(SecureRandom sr)
	{
		this.sr = sr;
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey)
	{
		NtruLPRimePublicKeyParameters ntruLPRimePublicKeyParameters = (NtruLPRimePublicKeyParameters)recipientKey;
		NtruPrimeEngine primeEngine = ntruLPRimePublicKeyParameters.Parameters.PrimeEngine;
		byte[] array = new byte[primeEngine.CipherTextSize];
		byte[] array2 = new byte[primeEngine.SessionKeySize];
		primeEngine.kem_enc(array, array2, ntruLPRimePublicKeyParameters.pubKey, sr);
		return new SecretWithEncapsulationImpl(array2, array);
	}
}
