using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Tsp;

public class TstInfo : Asn1Encodable
{
	private readonly DerInteger m_version;

	private readonly DerObjectIdentifier m_tsaPolicyID;

	private readonly MessageImprint m_messageImprint;

	private readonly DerInteger m_serialNumber;

	private readonly Asn1GeneralizedTime m_genTime;

	private readonly Accuracy m_accuracy;

	private readonly DerBoolean m_ordering;

	private readonly DerInteger m_nonce;

	private readonly GeneralName m_tsa;

	private readonly X509Extensions m_extensions;

	public DerInteger Version => m_version;

	public MessageImprint MessageImprint => m_messageImprint;

	public DerObjectIdentifier Policy => m_tsaPolicyID;

	public DerInteger SerialNumber => m_serialNumber;

	public Accuracy Accuracy => m_accuracy;

	public Asn1GeneralizedTime GenTime => m_genTime;

	public DerBoolean Ordering => m_ordering;

	public DerInteger Nonce => m_nonce;

	public GeneralName Tsa => m_tsa;

	public X509Extensions Extensions => m_extensions;

	public static TstInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is TstInfo result)
		{
			return result;
		}
		return new TstInfo(Asn1Sequence.GetInstance(obj));
	}

	public static TstInfo GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new TstInfo(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private TstInfo(Asn1Sequence seq)
	{
		IEnumerator<Asn1Encodable> enumerator = seq.GetEnumerator();
		enumerator.MoveNext();
		m_version = DerInteger.GetInstance(enumerator.Current);
		enumerator.MoveNext();
		m_tsaPolicyID = DerObjectIdentifier.GetInstance(enumerator.Current);
		enumerator.MoveNext();
		m_messageImprint = MessageImprint.GetInstance(enumerator.Current);
		enumerator.MoveNext();
		m_serialNumber = DerInteger.GetInstance(enumerator.Current);
		enumerator.MoveNext();
		m_genTime = Asn1GeneralizedTime.GetInstance(enumerator.Current);
		m_ordering = DerBoolean.False;
		while (enumerator.MoveNext())
		{
			Asn1Object asn1Object = (Asn1Object)enumerator.Current;
			if (asn1Object is Asn1TaggedObject { TagNo: var tagNo } asn1TaggedObject)
			{
				switch (tagNo)
				{
				case 0:
					m_tsa = GeneralName.GetInstance(asn1TaggedObject, explicitly: true);
					break;
				case 1:
					m_extensions = X509Extensions.GetInstance(asn1TaggedObject, declaredExplicit: false);
					break;
				default:
					throw new ArgumentException("Unknown tag value " + asn1TaggedObject.TagNo);
				}
			}
			if (asn1Object is Asn1Sequence)
			{
				m_accuracy = Accuracy.GetInstance(asn1Object);
			}
			if (asn1Object is DerBoolean)
			{
				m_ordering = DerBoolean.GetInstance(asn1Object);
			}
			if (asn1Object is DerInteger)
			{
				m_nonce = DerInteger.GetInstance(asn1Object);
			}
		}
	}

	public TstInfo(DerObjectIdentifier tsaPolicyId, MessageImprint messageImprint, DerInteger serialNumber, Asn1GeneralizedTime genTime, Accuracy accuracy, DerBoolean ordering, DerInteger nonce, GeneralName tsa, X509Extensions extensions)
	{
		m_version = new DerInteger(1);
		m_tsaPolicyID = tsaPolicyId;
		m_messageImprint = messageImprint;
		m_serialNumber = serialNumber;
		m_genTime = genTime;
		m_accuracy = accuracy;
		m_ordering = ordering;
		m_nonce = nonce;
		m_tsa = tsa;
		m_extensions = extensions;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(10);
		asn1EncodableVector.Add(m_version, m_tsaPolicyID, m_messageImprint, m_serialNumber, m_genTime);
		asn1EncodableVector.AddOptional(m_accuracy);
		if (m_ordering != null && m_ordering.IsTrue)
		{
			asn1EncodableVector.Add(m_ordering);
		}
		asn1EncodableVector.AddOptional(m_nonce);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_tsa);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, m_extensions);
		return new DerSequence(asn1EncodableVector);
	}
}
