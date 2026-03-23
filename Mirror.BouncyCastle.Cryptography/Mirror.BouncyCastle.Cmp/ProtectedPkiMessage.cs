using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crmf;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cmp;

public class ProtectedPkiMessage
{
	private readonly PkiMessage m_pkiMessage;

	public virtual PkiHeader Header => m_pkiMessage.Header;

	public virtual PkiBody Body => m_pkiMessage.Body;

	public virtual bool HasPasswordBasedMacProtected => CmpObjectIdentifiers.passwordBasedMac.Equals(ProtectionAlgorithm.Algorithm);

	public virtual AlgorithmIdentifier ProtectionAlgorithm => m_pkiMessage.Header.ProtectionAlg;

	public ProtectedPkiMessage(GeneralPkiMessage pkiMessage)
	{
		if (!pkiMessage.HasProtection)
		{
			throw new ArgumentException("GeneralPkiMessage not protected", "pkiMessage");
		}
		m_pkiMessage = pkiMessage.ToAsn1Structure();
	}

	public ProtectedPkiMessage(PkiMessage pkiMessage)
	{
		if (pkiMessage.Header.ProtectionAlg == null)
		{
			throw new ArgumentException("PkiMessage not protected", "pkiMessage");
		}
		m_pkiMessage = pkiMessage;
	}

	public virtual PkiMessage ToAsn1Message()
	{
		return m_pkiMessage;
	}

	public virtual X509Certificate[] GetCertificates()
	{
		CmpCertificate[] extraCerts = m_pkiMessage.GetExtraCerts();
		if (extraCerts == null)
		{
			return new X509Certificate[0];
		}
		return Array.ConvertAll(extraCerts, (CmpCertificate cmpCertificate) => new X509Certificate(cmpCertificate.X509v3PKCert));
	}

	public virtual bool Verify(IVerifierFactory verifierFactory)
	{
		return X509Utilities.VerifySignature(verifierFactory, CreateProtected(), m_pkiMessage.Protection);
	}

	public virtual bool Verify(PKMacBuilder pkMacBuilder, char[] password)
	{
		AlgorithmIdentifier protectionAlgorithm = ProtectionAlgorithm;
		if (!CmpObjectIdentifiers.passwordBasedMac.Equals(protectionAlgorithm.Algorithm))
		{
			throw new InvalidOperationException("protection algorithm is not mac based");
		}
		PbmParameter instance = PbmParameter.GetInstance(protectionAlgorithm.Parameters);
		pkMacBuilder.SetParameters(instance);
		return X509Utilities.VerifyMac(pkMacBuilder.Build(password), CreateProtected(), m_pkiMessage.Protection);
	}

	private DerSequence CreateProtected()
	{
		return new DerSequence(m_pkiMessage.Header, m_pkiMessage.Body);
	}
}
