using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class ServiceLocator : Asn1Encodable
{
	private readonly X509Name m_issuer;

	private readonly Asn1Object m_locator;

	public X509Name Issuer => m_issuer;

	public Asn1Object Locator => m_locator;

	public static ServiceLocator GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ServiceLocator result)
		{
			return result;
		}
		return new ServiceLocator(Asn1Sequence.GetInstance(obj));
	}

	public static ServiceLocator GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new ServiceLocator(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public ServiceLocator(X509Name issuer)
		: this(issuer, null)
	{
	}

	public ServiceLocator(X509Name issuer, Asn1Object locator)
	{
		m_issuer = issuer ?? throw new ArgumentNullException("issuer");
		m_locator = locator;
	}

	private ServiceLocator(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int num = 0;
		m_issuer = X509Name.GetInstance(seq[num++]);
		if (num < count)
		{
			m_locator = seq[num++].ToAsn1Object();
		}
		if (num != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_issuer);
		asn1EncodableVector.AddOptional(m_locator);
		return new DerSequence(asn1EncodableVector);
	}
}
