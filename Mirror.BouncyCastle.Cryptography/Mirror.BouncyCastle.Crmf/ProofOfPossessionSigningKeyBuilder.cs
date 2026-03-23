using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Crmf;

public class ProofOfPossessionSigningKeyBuilder
{
	private readonly CertRequest m_certRequest;

	private readonly SubjectPublicKeyInfo m_pubKeyInfo;

	private GeneralName m_name;

	private PKMacValue m_publicKeyMac;

	public ProofOfPossessionSigningKeyBuilder(CertRequest certRequest)
	{
		m_certRequest = certRequest;
		m_pubKeyInfo = null;
	}

	public ProofOfPossessionSigningKeyBuilder(SubjectPublicKeyInfo pubKeyInfo)
	{
		m_certRequest = null;
		m_pubKeyInfo = pubKeyInfo;
	}

	public ProofOfPossessionSigningKeyBuilder SetSender(GeneralName name)
	{
		m_name = name;
		return this;
	}

	public ProofOfPossessionSigningKeyBuilder SetPublicKeyMac(PKMacBuilder generator, char[] password)
	{
		m_publicKeyMac = PKMacValueGenerator.Generate(generator, password, m_pubKeyInfo);
		return this;
	}

	public PopoSigningKey Build(ISignatureFactory signer)
	{
		if (m_name != null && m_publicKeyMac != null)
		{
			throw new InvalidOperationException("name and publicKeyMAC cannot both be set.");
		}
		PopoSigningKeyInput popoSigningKeyInput;
		Asn1Encodable asn1Encodable;
		if (m_certRequest != null)
		{
			popoSigningKeyInput = null;
			asn1Encodable = m_certRequest;
		}
		else if (m_name != null)
		{
			popoSigningKeyInput = new PopoSigningKeyInput(m_name, m_pubKeyInfo);
			asn1Encodable = popoSigningKeyInput;
		}
		else
		{
			popoSigningKeyInput = new PopoSigningKeyInput(m_publicKeyMac, m_pubKeyInfo);
			asn1Encodable = popoSigningKeyInput;
		}
		DerBitString signature = X509Utilities.GenerateSignature(signer, asn1Encodable);
		return new PopoSigningKey(popoSigningKeyInput, (AlgorithmIdentifier)signer.AlgorithmDetails, signature);
	}
}
