using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Crmf;

public class CertificateRequestMessage
{
	public static readonly int popRaVerified = 0;

	public static readonly int popSigningKey = 1;

	public static readonly int popKeyEncipherment = 2;

	public static readonly int popKeyAgreement = 3;

	private readonly CertReqMsg m_certReqMsg;

	private readonly Controls m_controls;

	public bool HasControls => m_controls != null;

	public bool HasProofOfPossession => m_certReqMsg.Pop != null;

	public int ProofOfPossession => m_certReqMsg.Pop.Type;

	public bool HasSigningKeyProofOfPossessionWithPkMac
	{
		get
		{
			ProofOfPossession pop = m_certReqMsg.Pop;
			if (pop.Type != popSigningKey)
			{
				return false;
			}
			return PopoSigningKey.GetInstance(pop.Object).PoposkInput.PublicKeyMac != null;
		}
	}

	private static CertReqMsg ParseBytes(byte[] encoding)
	{
		return CertReqMsg.GetInstance(encoding);
	}

	public CertificateRequestMessage(byte[] encoded)
		: this(ParseBytes(encoded))
	{
	}

	public CertificateRequestMessage(CertReqMsg certReqMsg)
	{
		m_certReqMsg = certReqMsg;
		m_controls = certReqMsg.CertReq.Controls;
	}

	public CertReqMsg ToAsn1Structure()
	{
		return m_certReqMsg;
	}

	public DerInteger GetCertReqID()
	{
		return m_certReqMsg.CertReq.CertReqID;
	}

	public CertTemplate GetCertTemplate()
	{
		return m_certReqMsg.CertReq.CertTemplate;
	}

	public bool HasControl(DerObjectIdentifier objectIdentifier)
	{
		return FindControl(objectIdentifier) != null;
	}

	public IControl GetControl(DerObjectIdentifier type)
	{
		AttributeTypeAndValue attributeTypeAndValue = FindControl(type);
		if (attributeTypeAndValue != null)
		{
			DerObjectIdentifier type2 = attributeTypeAndValue.Type;
			if (CrmfObjectIdentifiers.id_regCtrl_pkiArchiveOptions.Equals(type2))
			{
				return new PkiArchiveControl(PkiArchiveOptions.GetInstance(attributeTypeAndValue.Value));
			}
			if (CrmfObjectIdentifiers.id_regCtrl_regToken.Equals(type2))
			{
				return new RegTokenControl(DerUtf8String.GetInstance(attributeTypeAndValue.Value));
			}
			if (CrmfObjectIdentifiers.id_regCtrl_authenticator.Equals(type2))
			{
				return new AuthenticatorControl(DerUtf8String.GetInstance(attributeTypeAndValue.Value));
			}
		}
		return null;
	}

	public AttributeTypeAndValue FindControl(DerObjectIdentifier type)
	{
		if (m_controls == null)
		{
			return null;
		}
		AttributeTypeAndValue[] array = m_controls.ToAttributeTypeAndValueArray();
		AttributeTypeAndValue result = null;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Type.Equals(type))
			{
				result = array[i];
				break;
			}
		}
		return result;
	}

	public bool IsValidSigningKeyPop(IVerifierFactoryProvider verifierProvider)
	{
		ProofOfPossession pop = m_certReqMsg.Pop;
		if (pop.Type != popSigningKey)
		{
			throw new InvalidOperationException("not Signing Key type of proof of possession");
		}
		PopoSigningKey instance = PopoSigningKey.GetInstance(pop.Object);
		if (instance.PoposkInput != null && instance.PoposkInput.PublicKeyMac != null)
		{
			throw new InvalidOperationException("verification requires password check");
		}
		return VerifySignature(verifierProvider, instance);
	}

	public bool IsValidSigningKeyPop(IVerifierFactoryProvider verifierProvider, PKMacBuilder macBuilder, char[] password)
	{
		ProofOfPossession pop = m_certReqMsg.Pop;
		if (pop.Type != popSigningKey)
		{
			throw new InvalidOperationException("not Signing Key type of proof of possession");
		}
		PopoSigningKey instance = PopoSigningKey.GetInstance(pop.Object);
		if (instance.PoposkInput == null || instance.PoposkInput.Sender != null)
		{
			throw new InvalidOperationException("no PKMAC present in proof of possession");
		}
		PKMacValue publicKeyMac = instance.PoposkInput.PublicKeyMac;
		if (new PKMacValueVerifier(macBuilder).IsValid(publicKeyMac, password, GetCertTemplate().PublicKey))
		{
			return VerifySignature(verifierProvider, instance);
		}
		return false;
	}

	private bool VerifySignature(IVerifierFactoryProvider verifierFactoryProvider, PopoSigningKey signKey)
	{
		IVerifierFactory verifierFactory = verifierFactoryProvider.CreateVerifierFactory(signKey.AlgorithmIdentifier);
		Asn1Encodable asn1Encodable = signKey.PoposkInput;
		if (asn1Encodable == null)
		{
			asn1Encodable = m_certReqMsg.CertReq;
		}
		return X509Utilities.VerifySignature(verifierFactory, asn1Encodable, signKey.Signature);
	}

	public byte[] GetEncoded()
	{
		return m_certReqMsg.GetEncoded();
	}
}
