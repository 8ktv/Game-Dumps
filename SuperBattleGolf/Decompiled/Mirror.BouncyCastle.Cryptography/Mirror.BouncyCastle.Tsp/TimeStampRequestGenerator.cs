using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Tsp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Tsp;

public class TimeStampRequestGenerator
{
	private DerObjectIdentifier reqPolicy;

	private DerBoolean certReq;

	private Dictionary<DerObjectIdentifier, X509Extension> m_extensions = new Dictionary<DerObjectIdentifier, X509Extension>();

	private List<DerObjectIdentifier> m_ordering = new List<DerObjectIdentifier>();

	public void SetReqPolicy(string reqPolicy)
	{
		this.reqPolicy = new DerObjectIdentifier(reqPolicy);
	}

	public void SetCertReq(bool certReq)
	{
		this.certReq = DerBoolean.GetInstance(certReq);
	}

	public virtual void AddExtension(DerObjectIdentifier oid, bool critical, Asn1Encodable extValue)
	{
		AddExtension(oid, critical, extValue.GetEncoded());
	}

	public virtual void AddExtension(DerObjectIdentifier oid, bool critical, byte[] extValue)
	{
		m_extensions.Add(oid, new X509Extension(critical, new DerOctetString(extValue)));
		m_ordering.Add(oid);
	}

	public TimeStampRequest Generate(string digestAlgorithm, byte[] digest)
	{
		return Generate(digestAlgorithm, digest, null);
	}

	public TimeStampRequest Generate(string digestAlgorithmOid, byte[] digest, BigInteger nonce)
	{
		if (digestAlgorithmOid == null)
		{
			throw new ArgumentException("No digest algorithm specified");
		}
		MessageImprint messageImprint = new MessageImprint(new AlgorithmIdentifier(new DerObjectIdentifier(digestAlgorithmOid), DerNull.Instance), digest);
		X509Extensions extensions = null;
		if (m_ordering.Count > 0)
		{
			extensions = new X509Extensions(m_ordering, m_extensions);
		}
		return new TimeStampRequest(new TimeStampReq(nonce: (nonce == null) ? null : new DerInteger(nonce), messageImprint: messageImprint, tsaPolicy: reqPolicy, certReq: certReq, extensions: extensions));
	}

	public virtual TimeStampRequest Generate(DerObjectIdentifier digestAlgorithm, byte[] digest)
	{
		return Generate(digestAlgorithm.Id, digest);
	}

	public virtual TimeStampRequest Generate(DerObjectIdentifier digestAlgorithm, byte[] digest, BigInteger nonce)
	{
		return Generate(digestAlgorithm.Id, digest, nonce);
	}
}
