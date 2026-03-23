using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class OtherSigningCertificate : Asn1Encodable
{
	private readonly Asn1Sequence m_certs;

	private readonly Asn1Sequence m_policies;

	public static OtherSigningCertificate GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OtherSigningCertificate result)
		{
			return result;
		}
		return new OtherSigningCertificate(Asn1Sequence.GetInstance(obj));
	}

	public static OtherSigningCertificate GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new OtherSigningCertificate(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private OtherSigningCertificate(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_certs = Asn1Sequence.GetInstance(seq[0]);
		if (count > 1)
		{
			m_policies = Asn1Sequence.GetInstance(seq[1]);
		}
	}

	public OtherSigningCertificate(params OtherCertID[] certs)
		: this(certs, (PolicyInformation[])null)
	{
	}

	public OtherSigningCertificate(OtherCertID[] certs, params PolicyInformation[] policies)
	{
		if (certs == null)
		{
			throw new ArgumentNullException("certs");
		}
		Asn1Encodable[] elements = certs;
		m_certs = DerSequence.FromElements(elements);
		if (policies != null)
		{
			elements = policies;
			m_policies = DerSequence.FromElements(elements);
		}
	}

	public OtherSigningCertificate(IEnumerable<OtherCertID> certs)
		: this(certs, null)
	{
	}

	public OtherSigningCertificate(IEnumerable<OtherCertID> certs, IEnumerable<PolicyInformation> policies)
	{
		if (certs == null)
		{
			throw new ArgumentNullException("certs");
		}
		m_certs = DerSequence.FromVector(Asn1EncodableVector.FromEnumerable(certs));
		if (policies != null)
		{
			m_policies = DerSequence.FromVector(Asn1EncodableVector.FromEnumerable(policies));
		}
	}

	public OtherCertID[] GetCerts()
	{
		return m_certs.MapElements(OtherCertID.GetInstance);
	}

	public PolicyInformation[] GetPolicies()
	{
		return m_policies?.MapElements(PolicyInformation.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_certs);
		asn1EncodableVector.AddOptional(m_policies);
		return new DerSequence(asn1EncodableVector);
	}
}
