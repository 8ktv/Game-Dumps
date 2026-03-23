using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class CrlSource : Asn1Encodable, IAsn1Choice
{
	private readonly DistributionPointName m_dpn;

	private readonly GeneralNames m_issuer;

	public virtual DistributionPointName Dpn => m_dpn;

	public virtual GeneralNames Issuer => m_issuer;

	public static CrlSource GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CrlSource result)
		{
			return result;
		}
		return new CrlSource(Asn1TaggedObject.GetInstance(obj));
	}

	public static CrlSource GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	private CrlSource(Asn1TaggedObject taggedObject)
	{
		if (taggedObject.HasContextTag(0))
		{
			m_dpn = DistributionPointName.GetInstance(taggedObject, explicitly: true);
			m_issuer = null;
			return;
		}
		if (taggedObject.HasContextTag(1))
		{
			m_dpn = null;
			m_issuer = GeneralNames.GetInstance(taggedObject, explicitly: true);
			return;
		}
		throw new ArgumentException("unknown tag: " + Asn1Utilities.GetTagText(taggedObject), "taggedObject");
	}

	public CrlSource(DistributionPointName dpn, GeneralNames issuer)
	{
		if (dpn == null == (issuer == null))
		{
			throw new ArgumentException("either dpn or issuer must be set");
		}
		m_dpn = dpn;
		m_issuer = issuer;
	}

	public override Asn1Object ToAsn1Object()
	{
		if (m_dpn != null)
		{
			return new DerTaggedObject(isExplicit: true, 0, m_dpn);
		}
		if (m_issuer != null)
		{
			return new DerTaggedObject(isExplicit: true, 1, m_issuer);
		}
		throw new InvalidOperationException();
	}
}
