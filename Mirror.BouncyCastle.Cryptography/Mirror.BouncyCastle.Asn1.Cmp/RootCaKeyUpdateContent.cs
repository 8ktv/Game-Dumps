using System;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class RootCaKeyUpdateContent : Asn1Encodable
{
	private readonly CmpCertificate m_newWithNew;

	private readonly CmpCertificate m_newWithOld;

	private readonly CmpCertificate m_oldWithNew;

	public virtual CmpCertificate NewWithNew => m_newWithNew;

	public virtual CmpCertificate NewWithOld => m_newWithOld;

	public virtual CmpCertificate OldWithNew => m_oldWithNew;

	public static RootCaKeyUpdateContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is RootCaKeyUpdateContent result)
		{
			return result;
		}
		return new RootCaKeyUpdateContent(Asn1Sequence.GetInstance(obj));
	}

	public static RootCaKeyUpdateContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new RootCaKeyUpdateContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public RootCaKeyUpdateContent(CmpCertificate newWithNew, CmpCertificate newWithOld, CmpCertificate oldWithNew)
	{
		m_newWithNew = newWithNew ?? throw new ArgumentNullException("newWithNew");
		m_newWithOld = newWithOld;
		m_oldWithNew = oldWithNew;
	}

	private RootCaKeyUpdateContent(Asn1Sequence seq)
	{
		if (seq.Count < 1 || seq.Count > 3)
		{
			throw new ArgumentException("expected sequence of 1 to 3 elements only");
		}
		CmpCertificate instance = CmpCertificate.GetInstance(seq[0]);
		CmpCertificate newWithOld = null;
		CmpCertificate oldWithNew = null;
		for (int i = 1; i < seq.Count; i++)
		{
			Asn1TaggedObject instance2 = Asn1TaggedObject.GetInstance(seq[i]);
			if (instance2.HasContextTag(0))
			{
				newWithOld = CmpCertificate.GetInstance(instance2, declaredExplicit: true);
			}
			else if (instance2.HasContextTag(1))
			{
				oldWithNew = CmpCertificate.GetInstance(instance2, declaredExplicit: true);
			}
		}
		m_newWithNew = instance;
		m_newWithOld = newWithOld;
		m_oldWithNew = oldWithNew;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(m_newWithNew);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_newWithOld);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_oldWithNew);
		return new DerSequence(asn1EncodableVector);
	}
}
