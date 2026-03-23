using System;
using System.IO;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Utilities;

public abstract class PqcOtherInfoGenerator
{
	public sealed class PartyU : PqcOtherInfoGenerator
	{
		private AsymmetricCipherKeyPair m_aKp;

		private IEncapsulatedSecretExtractor m_encSE;

		public PartyU(IKemParameters kemParams, AlgorithmIdentifier algorithmID, byte[] partyUInfo, byte[] partyVInfo, SecureRandom random)
			: base(algorithmID, partyUInfo, partyVInfo, random)
		{
			if (kemParams is KyberParameters kyberParameters)
			{
				KyberKeyPairGenerator kyberKeyPairGenerator = new KyberKeyPairGenerator();
				kyberKeyPairGenerator.Init(new KyberKeyGenerationParameters(random, kyberParameters));
				m_aKp = kyberKeyPairGenerator.GenerateKeyPair();
				m_encSE = new KyberKemExtractor((KyberPrivateKeyParameters)m_aKp.Private);
				return;
			}
			if (kemParams is NtruParameters ntruParameters)
			{
				NtruKeyPairGenerator ntruKeyPairGenerator = new NtruKeyPairGenerator();
				ntruKeyPairGenerator.Init(new NtruKeyGenerationParameters(random, ntruParameters));
				m_aKp = ntruKeyPairGenerator.GenerateKeyPair();
				m_encSE = new NtruKemExtractor((NtruPrivateKeyParameters)m_aKp.Private);
				return;
			}
			throw new ArgumentException("unknown IKemParameters");
		}

		public PqcOtherInfoGenerator WithSuppPubInfo(byte[] suppPubInfo)
		{
			m_otherInfoBuilder.WithSuppPubInfo(suppPubInfo);
			return this;
		}

		public byte[] GetSuppPrivInfoPartA()
		{
			return GetEncoded(m_aKp.Public);
		}

		public DerOtherInfo Generate(byte[] suppPrivInfoPartB)
		{
			m_otherInfoBuilder.WithSuppPrivInfo(m_encSE.ExtractSecret(suppPrivInfoPartB));
			return m_otherInfoBuilder.Build();
		}
	}

	public sealed class PartyV : PqcOtherInfoGenerator
	{
		private IEncapsulatedSecretGenerator m_encSG;

		public PartyV(IKemParameters kemParams, AlgorithmIdentifier algorithmID, byte[] partyUInfo, byte[] partyVInfo, SecureRandom random)
			: base(algorithmID, partyUInfo, partyVInfo, random)
		{
			if (kemParams is KyberParameters)
			{
				m_encSG = new KyberKemGenerator(random);
				return;
			}
			if (kemParams is NtruParameters)
			{
				m_encSG = new NtruKemGenerator(random);
				return;
			}
			throw new ArgumentException("unknown IKemParameters");
		}

		public PqcOtherInfoGenerator WithSuppPubInfo(byte[] suppPubInfo)
		{
			m_otherInfoBuilder.WithSuppPubInfo(suppPubInfo);
			return this;
		}

		public byte[] GetSuppPrivInfoPartB(byte[] suppPrivInfoPartA)
		{
			m_used = false;
			try
			{
				ISecretWithEncapsulation secretWithEncapsulation = m_encSG.GenerateEncapsulated(GetPublicKey(suppPrivInfoPartA));
				m_otherInfoBuilder.WithSuppPrivInfo(secretWithEncapsulation.GetSecret());
				return secretWithEncapsulation.GetEncapsulation();
			}
			catch (IOException innerException)
			{
				throw new ArgumentException("cannot decode public key", innerException);
			}
		}

		public DerOtherInfo Generate()
		{
			if (m_used)
			{
				throw new InvalidOperationException("builder already used");
			}
			m_used = true;
			return m_otherInfoBuilder.Build();
		}
	}

	protected readonly DerOtherInfo.Builder m_otherInfoBuilder;

	protected readonly SecureRandom m_random;

	protected bool m_used;

	internal PqcOtherInfoGenerator(AlgorithmIdentifier algorithmID, byte[] partyUInfo, byte[] partyVInfo, SecureRandom random)
	{
		m_otherInfoBuilder = new DerOtherInfo.Builder(algorithmID, partyUInfo, partyVInfo);
		m_random = random;
	}

	private static byte[] GetEncoded(AsymmetricKeyParameter pubKey)
	{
		try
		{
			return PqcSubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey).GetEncoded();
		}
		catch (IOException)
		{
			return null;
		}
	}

	private static AsymmetricKeyParameter GetPublicKey(byte[] enc)
	{
		return PqcPublicKeyFactory.CreateKey(enc);
	}
}
