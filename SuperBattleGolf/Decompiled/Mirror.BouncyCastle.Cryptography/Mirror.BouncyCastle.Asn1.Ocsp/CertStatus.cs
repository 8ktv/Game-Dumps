using System;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class CertStatus : Asn1Encodable, IAsn1Choice
{
	private readonly int m_tagNo;

	private readonly Asn1Encodable m_value;

	public int TagNo => m_tagNo;

	public Asn1Encodable Status => m_value;

	public static CertStatus GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CertStatus result)
		{
			return result;
		}
		return new CertStatus(Asn1TaggedObject.GetInstance(obj));
	}

	public static CertStatus GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	private static Asn1Encodable GetValue(Asn1TaggedObject choice)
	{
		if (choice.HasContextTag())
		{
			switch (choice.TagNo)
			{
			case 0:
				return Asn1Null.GetInstance(choice, declaredExplicit: false);
			case 1:
				return RevokedInfo.GetInstance(choice, explicitly: false);
			case 2:
				return Asn1Null.GetInstance(choice, declaredExplicit: false);
			}
		}
		throw new ArgumentException("unknown tag: " + Asn1Utilities.GetTagText(choice), "choice");
	}

	public CertStatus()
	{
		m_tagNo = 0;
		m_value = DerNull.Instance;
	}

	public CertStatus(RevokedInfo info)
	{
		m_tagNo = 1;
		m_value = info ?? throw new ArgumentNullException("info");
	}

	public CertStatus(int tagNo, Asn1Encodable value)
	{
		m_tagNo = tagNo;
		m_value = value ?? throw new ArgumentNullException("value");
	}

	public CertStatus(Asn1TaggedObject choice)
	{
		m_tagNo = choice.TagNo;
		m_value = GetValue(choice);
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerTaggedObject(isExplicit: false, m_tagNo, m_value);
	}
}
