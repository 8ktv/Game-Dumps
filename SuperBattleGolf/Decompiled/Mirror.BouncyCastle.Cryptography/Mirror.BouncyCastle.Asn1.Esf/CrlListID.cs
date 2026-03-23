using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class CrlListID : Asn1Encodable
{
	private readonly Asn1Sequence m_crls;

	public static CrlListID GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CrlListID result)
		{
			return result;
		}
		return new CrlListID(Asn1Sequence.GetInstance(obj));
	}

	public static CrlListID GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CrlListID(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CrlListID(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count != 1)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_crls = Asn1Sequence.GetInstance(seq[0]);
		m_crls.MapElements(CrlValidatedID.GetInstance);
	}

	public CrlListID(params CrlValidatedID[] crls)
	{
		if (crls == null)
		{
			throw new ArgumentNullException("crls");
		}
		m_crls = DerSequence.FromElements(crls);
	}

	public CrlListID(IEnumerable<CrlValidatedID> crls)
	{
		if (crls == null)
		{
			throw new ArgumentNullException("crls");
		}
		m_crls = DerSequence.FromVector(Asn1EncodableVector.FromEnumerable(crls));
	}

	public CrlValidatedID[] GetCrls()
	{
		return m_crls.MapElements(CrlValidatedID.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_crls);
	}
}
