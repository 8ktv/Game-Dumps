using System;

namespace Mirror.BouncyCastle.Asn1.Tsp;

public class Accuracy : Asn1Encodable
{
	protected const int MinMillis = 1;

	protected const int MaxMillis = 999;

	protected const int MinMicros = 1;

	protected const int MaxMicros = 999;

	private readonly DerInteger m_seconds;

	private readonly DerInteger m_millis;

	private readonly DerInteger m_micros;

	public DerInteger Seconds => m_seconds;

	public DerInteger Millis => m_millis;

	public DerInteger Micros => m_micros;

	public static Accuracy GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Accuracy result)
		{
			return result;
		}
		return new Accuracy(Asn1Sequence.GetInstance(obj));
	}

	public static Accuracy GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new Accuracy(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public Accuracy(DerInteger seconds, DerInteger millis, DerInteger micros)
	{
		if (millis != null)
		{
			int intValueExact = millis.IntValueExact;
			if (intValueExact < 1 || intValueExact > 999)
			{
				throw new ArgumentException("Invalid millis field : not in (1..999)");
			}
		}
		if (micros != null)
		{
			int intValueExact2 = micros.IntValueExact;
			if (intValueExact2 < 1 || intValueExact2 > 999)
			{
				throw new ArgumentException("Invalid micros field : not in (1..999)");
			}
		}
		m_seconds = seconds;
		m_millis = millis;
		m_micros = micros;
	}

	private Accuracy(Asn1Sequence seq)
	{
		DerInteger seconds = null;
		DerInteger derInteger = null;
		DerInteger derInteger2 = null;
		for (int i = 0; i < seq.Count; i++)
		{
			if (seq[i] is DerInteger derInteger3)
			{
				seconds = derInteger3;
			}
			else
			{
				if (!(seq[i] is Asn1TaggedObject { TagNo: var tagNo } asn1TaggedObject))
				{
					continue;
				}
				switch (tagNo)
				{
				case 0:
				{
					derInteger = DerInteger.GetInstance(asn1TaggedObject, declaredExplicit: false);
					int intValueExact2 = derInteger.IntValueExact;
					if (intValueExact2 < 1 || intValueExact2 > 999)
					{
						throw new ArgumentException("Invalid millis field : not in (1..999)");
					}
					break;
				}
				case 1:
				{
					derInteger2 = DerInteger.GetInstance(asn1TaggedObject, declaredExplicit: false);
					int intValueExact = derInteger2.IntValueExact;
					if (intValueExact < 1 || intValueExact > 999)
					{
						throw new ArgumentException("Invalid micros field : not in (1..999)");
					}
					break;
				}
				default:
					throw new ArgumentException("Invalid tag number");
				}
			}
		}
		m_seconds = seconds;
		m_millis = derInteger;
		m_micros = derInteger2;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.AddOptional(m_seconds);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, m_millis);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, m_micros);
		return new DerSequence(asn1EncodableVector);
	}
}
