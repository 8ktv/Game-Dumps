using System;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PkiFreeText : Asn1Encodable
{
	private readonly Asn1Sequence m_strings;

	public virtual int Count => m_strings.Count;

	public DerUtf8String this[int index] => (DerUtf8String)m_strings[index];

	public static PkiFreeText GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PkiFreeText result)
		{
			return result;
		}
		return new PkiFreeText(Asn1Sequence.GetInstance(obj));
	}

	public static PkiFreeText GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PkiFreeText(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	internal PkiFreeText(Asn1Sequence seq)
	{
		foreach (Asn1Encodable item in seq)
		{
			if (!(item is DerUtf8String))
			{
				throw new ArgumentException("attempt to insert non UTF8 STRING into PkiFreeText");
			}
		}
		m_strings = seq;
	}

	public PkiFreeText(DerUtf8String p)
	{
		m_strings = new DerSequence(p);
	}

	public PkiFreeText(string p)
		: this(new DerUtf8String(p))
	{
	}

	public PkiFreeText(DerUtf8String[] strs)
	{
		m_strings = new DerSequence(strs);
	}

	public PkiFreeText(string[] strs)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(strs.Length);
		for (int i = 0; i < strs.Length; i++)
		{
			asn1EncodableVector.Add(new DerUtf8String(strs[i]));
		}
		m_strings = new DerSequence(asn1EncodableVector);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_strings;
	}
}
