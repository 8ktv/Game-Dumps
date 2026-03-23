using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

public sealed class KyberKemGenerator : IEncapsulatedSecretGenerator
{
	private sealed class SecretWithEncapsulationImpl : ISecretWithEncapsulation, IDisposable
	{
		private volatile bool m_hasBeenDestroyed;

		private byte[] m_sessionKey;

		private byte[] m_cipherText;

		internal SecretWithEncapsulationImpl(byte[] sessionKey, byte[] cipher_text)
		{
			m_sessionKey = sessionKey;
			m_cipherText = cipher_text;
		}

		public byte[] GetSecret()
		{
			CheckDestroyed();
			return Arrays.Clone(m_sessionKey);
		}

		public byte[] GetEncapsulation()
		{
			CheckDestroyed();
			return Arrays.Clone(m_cipherText);
		}

		public void Dispose()
		{
			if (!m_hasBeenDestroyed)
			{
				Arrays.Clear(m_sessionKey);
				Arrays.Clear(m_cipherText);
				m_hasBeenDestroyed = true;
			}
			GC.SuppressFinalize(this);
		}

		internal bool IsDestroyed()
		{
			return m_hasBeenDestroyed;
		}

		private void CheckDestroyed()
		{
			if (IsDestroyed())
			{
				throw new ArgumentException("data has been destroyed");
			}
		}
	}

	private SecureRandom m_random;

	public KyberKemGenerator(SecureRandom random)
	{
		m_random = random;
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey)
	{
		KyberPublicKeyParameters kyberPublicKeyParameters = (KyberPublicKeyParameters)recipientKey;
		KyberEngine engine = kyberPublicKeyParameters.Parameters.Engine;
		engine.Init(m_random);
		byte[] array = new byte[engine.CryptoCipherTextBytes];
		byte[] array2 = new byte[engine.CryptoBytes];
		engine.KemEncrypt(array, array2, kyberPublicKeyParameters.GetEncoded());
		return new SecretWithEncapsulationImpl(array2, array);
	}
}
