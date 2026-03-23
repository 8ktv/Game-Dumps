using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crmf;

public class CertificateRequestMessageBuilder
{
	private readonly List<IControl> m_controls = new List<IControl>();

	private readonly X509ExtensionsGenerator m_extGenerator = new X509ExtensionsGenerator();

	private readonly CertTemplateBuilder m_templateBuilder = new CertTemplateBuilder();

	private readonly BigInteger m_certReqID;

	private ISignatureFactory m_popSigner;

	private PKMacBuilder m_pkMacBuilder;

	private char[] m_password;

	private GeneralName m_sender;

	private int m_popoType = 2;

	private PopoPrivKey m_popoPrivKey;

	private Asn1Null m_popRaVerified;

	private PKMacValue m_agreeMac;

	private AttributeTypeAndValue[] m_regInfo;

	public CertificateRequestMessageBuilder(BigInteger certReqId)
	{
		m_certReqID = certReqId;
	}

	public CertificateRequestMessageBuilder SetRegInfo(AttributeTypeAndValue[] regInfo)
	{
		m_regInfo = regInfo;
		return this;
	}

	public CertificateRequestMessageBuilder SetPublicKey(SubjectPublicKeyInfo publicKeyInfo)
	{
		if (publicKeyInfo != null)
		{
			m_templateBuilder.SetPublicKey(publicKeyInfo);
		}
		return this;
	}

	public CertificateRequestMessageBuilder SetIssuer(X509Name issuer)
	{
		if (issuer != null)
		{
			m_templateBuilder.SetIssuer(issuer);
		}
		return this;
	}

	public CertificateRequestMessageBuilder SetSubject(X509Name subject)
	{
		if (subject != null)
		{
			m_templateBuilder.SetSubject(subject);
		}
		return this;
	}

	public CertificateRequestMessageBuilder SetSerialNumber(BigInteger serialNumber)
	{
		if (serialNumber != null)
		{
			m_templateBuilder.SetSerialNumber(new DerInteger(serialNumber));
		}
		return this;
	}

	public CertificateRequestMessageBuilder SetSerialNumber(DerInteger serialNumber)
	{
		if (serialNumber != null)
		{
			m_templateBuilder.SetSerialNumber(serialNumber);
		}
		return this;
	}

	public CertificateRequestMessageBuilder SetValidity(DateTime? notBefore, DateTime? notAfter)
	{
		m_templateBuilder.SetValidity(new OptionalValidity(CreateTime(notBefore), CreateTime(notAfter)));
		return this;
	}

	public CertificateRequestMessageBuilder AddExtension(DerObjectIdentifier oid, bool critical, Asn1Encodable value)
	{
		m_extGenerator.AddExtension(oid, critical, value);
		return this;
	}

	public CertificateRequestMessageBuilder AddExtension(DerObjectIdentifier oid, bool critical, byte[] value)
	{
		m_extGenerator.AddExtension(oid, critical, value);
		return this;
	}

	public CertificateRequestMessageBuilder AddControl(IControl control)
	{
		m_controls.Add(control);
		return this;
	}

	public CertificateRequestMessageBuilder SetProofOfPossessionSignKeySigner(ISignatureFactory popoSignatureFactory)
	{
		if (m_popoPrivKey != null || m_popRaVerified != null || m_agreeMac != null)
		{
			throw new InvalidOperationException("only one proof of possession is allowed.");
		}
		m_popSigner = popoSignatureFactory;
		return this;
	}

	public CertificateRequestMessageBuilder SetProofOfPossessionSubsequentMessage(SubsequentMessage msg)
	{
		if (m_popoPrivKey != null || m_popRaVerified != null || m_agreeMac != null)
		{
			throw new InvalidOperationException("only one proof of possession is allowed.");
		}
		m_popoType = 2;
		m_popoPrivKey = new PopoPrivKey(msg);
		return this;
	}

	public CertificateRequestMessageBuilder SetProofOfPossessionSubsequentMessage(int type, SubsequentMessage msg)
	{
		if (m_popoPrivKey != null || m_popRaVerified != null || m_agreeMac != null)
		{
			throw new InvalidOperationException("only one proof of possession is allowed.");
		}
		if (type != 2 && type != 3)
		{
			throw new ArgumentException("type must be ProofOfPossession.TYPE_KEY_ENCIPHERMENT or ProofOfPossession.TYPE_KEY_AGREEMENT");
		}
		m_popoType = type;
		m_popoPrivKey = new PopoPrivKey(msg);
		return this;
	}

	public CertificateRequestMessageBuilder SetProofOfPossessionAgreeMac(PKMacValue macValue)
	{
		if (m_popSigner != null || m_popRaVerified != null || m_popoPrivKey != null)
		{
			throw new InvalidOperationException("only one proof of possession allowed");
		}
		m_agreeMac = macValue;
		return this;
	}

	public CertificateRequestMessageBuilder SetProofOfPossessionRaVerified()
	{
		if (m_popSigner != null || m_popoPrivKey != null)
		{
			throw new InvalidOperationException("only one proof of possession allowed");
		}
		m_popRaVerified = DerNull.Instance;
		return this;
	}

	[Obsolete("Use 'SetAuthInfoPKMacBuilder' instead")]
	public CertificateRequestMessageBuilder SetAuthInfoPKMAC(PKMacBuilder pkmacFactory, char[] password)
	{
		return SetAuthInfoPKMacBuilder(pkmacFactory, password);
	}

	public CertificateRequestMessageBuilder SetAuthInfoPKMacBuilder(PKMacBuilder pkmacFactory, char[] password)
	{
		m_pkMacBuilder = pkmacFactory;
		m_password = password;
		return this;
	}

	public CertificateRequestMessageBuilder SetAuthInfoSender(X509Name sender)
	{
		return SetAuthInfoSender(new GeneralName(sender));
	}

	public CertificateRequestMessageBuilder SetAuthInfoSender(GeneralName sender)
	{
		m_sender = sender;
		return this;
	}

	public CertificateRequestMessage Build()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(new DerInteger(m_certReqID));
		if (!m_extGenerator.IsEmpty)
		{
			m_templateBuilder.SetExtensions(m_extGenerator.Generate());
		}
		asn1EncodableVector.Add(m_templateBuilder.Build());
		if (m_controls.Count > 0)
		{
			Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(m_controls.Count);
			foreach (IControl control in m_controls)
			{
				asn1EncodableVector2.Add(new AttributeTypeAndValue(control.Type, control.Value));
			}
			asn1EncodableVector.Add(new DerSequence(asn1EncodableVector2));
		}
		CertRequest instance = CertRequest.GetInstance(new DerSequence(asn1EncodableVector));
		ProofOfPossession popo;
		if (m_popSigner == null)
		{
			popo = ((m_popoPrivKey != null) ? new ProofOfPossession(m_popoType, m_popoPrivKey) : ((m_agreeMac != null) ? new ProofOfPossession(3, new PopoPrivKey(m_agreeMac)) : ((m_popRaVerified == null) ? new ProofOfPossession() : new ProofOfPossession())));
		}
		else
		{
			CertTemplate certTemplate = instance.CertTemplate;
			ProofOfPossessionSigningKeyBuilder proofOfPossessionSigningKeyBuilder;
			if (certTemplate.Subject == null || certTemplate.PublicKey == null)
			{
				proofOfPossessionSigningKeyBuilder = new ProofOfPossessionSigningKeyBuilder(instance.CertTemplate.PublicKey);
				if (m_sender != null)
				{
					proofOfPossessionSigningKeyBuilder.SetSender(m_sender);
				}
				else
				{
					proofOfPossessionSigningKeyBuilder.SetPublicKeyMac(m_pkMacBuilder, m_password);
				}
			}
			else
			{
				proofOfPossessionSigningKeyBuilder = new ProofOfPossessionSigningKeyBuilder(instance);
			}
			popo = new ProofOfPossession(proofOfPossessionSigningKeyBuilder.Build(m_popSigner));
		}
		return new CertificateRequestMessage(new CertReqMsg(instance, popo, m_regInfo));
	}

	private static Time CreateTime(DateTime? dateTime)
	{
		if (dateTime.HasValue)
		{
			return new Time(dateTime.Value);
		}
		return null;
	}
}
