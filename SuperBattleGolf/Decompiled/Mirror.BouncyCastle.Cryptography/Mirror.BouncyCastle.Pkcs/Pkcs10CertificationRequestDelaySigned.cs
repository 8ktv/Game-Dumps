using System;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkcs;

public class Pkcs10CertificationRequestDelaySigned : Pkcs10CertificationRequest
{
	protected Pkcs10CertificationRequestDelaySigned()
	{
	}

	public Pkcs10CertificationRequestDelaySigned(byte[] encoded)
		: base(encoded)
	{
	}

	public Pkcs10CertificationRequestDelaySigned(Asn1Sequence seq)
		: base(seq)
	{
	}

	public Pkcs10CertificationRequestDelaySigned(Stream input)
		: base(input)
	{
	}

	public Pkcs10CertificationRequestDelaySigned(string signatureAlgorithm, X509Name subject, AsymmetricKeyParameter publicKey, Asn1Set attributes, AsymmetricKeyParameter signingKey)
		: base(signatureAlgorithm, subject, publicKey, attributes, signingKey)
	{
	}

	public Pkcs10CertificationRequestDelaySigned(string signatureAlgorithm, X509Name subject, AsymmetricKeyParameter publicKey, Asn1Set attributes)
	{
		if (signatureAlgorithm == null)
		{
			throw new ArgumentNullException("signatureAlgorithm");
		}
		if (subject == null)
		{
			throw new ArgumentNullException("subject");
		}
		if (publicKey == null)
		{
			throw new ArgumentNullException("publicKey");
		}
		if (publicKey.IsPrivate)
		{
			throw new ArgumentException("expected public key", "publicKey");
		}
		if (!Pkcs10CertificationRequest.m_algorithms.TryGetValue(signatureAlgorithm, out var value) && !DerObjectIdentifier.TryFromID(signatureAlgorithm, out value))
		{
			throw new ArgumentException("Unknown signature type requested");
		}
		Asn1Encodable value2;
		if (Pkcs10CertificationRequest.m_noParams.Contains(value))
		{
			sigAlgId = new AlgorithmIdentifier(value);
		}
		else if (Pkcs10CertificationRequest.m_exParams.TryGetValue(signatureAlgorithm, out value2))
		{
			sigAlgId = new AlgorithmIdentifier(value, value2);
		}
		else
		{
			sigAlgId = new AlgorithmIdentifier(value, DerNull.Instance);
		}
		SubjectPublicKeyInfo pkInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
		reqInfo = new CertificationRequestInfo(subject, pkInfo, attributes);
	}

	public byte[] GetDataToSign()
	{
		return reqInfo.GetDerEncoded();
	}

	public void SignRequest(byte[] signedData)
	{
		sigBits = new DerBitString(signedData);
	}

	public void SignRequest(DerBitString signedData)
	{
		sigBits = signedData;
	}
}
