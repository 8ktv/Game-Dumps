using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Tsp;

public class PartialHashtree : Asn1Encodable
{
	private readonly Asn1Sequence m_values;

	public virtual int ValueCount => m_values.Count;

	public static PartialHashtree GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PartialHashtree result)
		{
			return result;
		}
		return new PartialHashtree(Asn1Sequence.GetInstance(obj));
	}

	public static PartialHashtree GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PartialHashtree(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private PartialHashtree(Asn1Sequence values)
	{
		for (int i = 0; i != values.Count; i++)
		{
			if (!(values[i] is Asn1OctetString))
			{
				throw new ArgumentException("unknown object in constructor: " + Platform.GetTypeName(values[i]));
			}
		}
		m_values = values;
	}

	public PartialHashtree(params byte[][] values)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(values.Length);
		for (int i = 0; i != values.Length; i++)
		{
			asn1EncodableVector.Add(new DerOctetString(Arrays.Clone(values[i])));
		}
		m_values = new DerSequence(asn1EncodableVector);
	}

	public virtual byte[][] GetValues()
	{
		return m_values.MapElements((Asn1Encodable element) => Arrays.Clone(Asn1OctetString.GetInstance(element).GetOctets()));
	}

	public virtual bool ContainsHash(byte[] hash)
	{
		foreach (Asn1OctetString value in m_values)
		{
			byte[] octets = value.GetOctets();
			if (Arrays.FixedTimeEquals(hash, octets))
			{
				return true;
			}
		}
		return false;
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_values;
	}
}
