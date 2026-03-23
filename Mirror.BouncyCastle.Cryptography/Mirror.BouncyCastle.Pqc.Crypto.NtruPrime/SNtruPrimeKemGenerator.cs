using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.NtruPrime;

public class SNtruPrimeKemGenerator : IEncapsulatedSecretGenerator
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

	public SNtruPrimeKemGenerator(SecureRandom sr)
	{
		this.sr = sr;
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey)
	{
		SNtruPrimePublicKeyParameters sNtruPrimePublicKeyParameters = (SNtruPrimePublicKeyParameters)recipientKey;
		NtruPrimeEngine primeEngine = sNtruPrimePublicKeyParameters.Parameters.PrimeEngine;
		byte[] array = new byte[primeEngine.CipherTextSize];
		byte[] array2 = new byte[primeEngine.SessionKeySize];
		primeEngine.kem_enc(array, array2, sNtruPrimePublicKeyParameters.pubKey, sr);
		return new NtruLPRimeKemGenerator.SecretWithEncapsulationImpl(array2, array);
	}
}
