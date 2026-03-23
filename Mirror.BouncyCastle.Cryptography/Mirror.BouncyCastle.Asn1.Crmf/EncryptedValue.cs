using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Crmf;

public class EncryptedValue : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_intendedAlg;

	private readonly AlgorithmIdentifier m_symmAlg;

	private readonly DerBitString m_encSymmKey;

	private readonly AlgorithmIdentifier m_keyAlg;

	private readonly Asn1OctetString m_valueHint;

	private readonly DerBitString m_encValue;

	public virtual AlgorithmIdentifier IntendedAlg => m_intendedAlg;

	public virtual AlgorithmIdentifier SymmAlg => m_symmAlg;

	public virtual DerBitString EncSymmKey => m_encSymmKey;

	public virtual AlgorithmIdentifier KeyAlg => m_keyAlg;

	public virtual Asn1OctetString ValueHint => m_valueHint;

	public virtual DerBitString EncValue => m_encValue;

	public static EncryptedValue GetInstance(object obj)
	{
		if (obj is EncryptedValue result)
		{
			return result;
		}
		if (obj != null)
		{
			return new EncryptedValue(Asn1Sequence.GetInstance(obj));
		}
		return null;
	}

	private EncryptedValue(Asn1Sequence seq)
	{
		int i;
		for (i = 0; seq[i] is Asn1TaggedObject asn1TaggedObject; i++)
		{
			switch (asn1TaggedObject.TagNo)
			{
			case 0:
				m_intendedAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, explicitly: false);
				break;
			case 1:
				m_symmAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, explicitly: false);
				break;
			case 2:
				m_encSymmKey = DerBitString.GetInstance(asn1TaggedObject, isExplicit: false);
				break;
			case 3:
				m_keyAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, explicitly: false);
				break;
			case 4:
				m_valueHint = Asn1OctetString.GetInstance(asn1TaggedObject, declaredExplicit: false);
				break;
			}
		}
		m_encValue = DerBitString.GetInstance(seq[i]);
	}

	public EncryptedValue(AlgorithmIdentifier intendedAlg, AlgorithmIdentifier symmAlg, DerBitString encSymmKey, AlgorithmIdentifier keyAlg, Asn1OctetString valueHint, DerBitString encValue)
	{
		if (encValue == null)
		{
			throw new ArgumentNullException("encValue");
		}
		m_intendedAlg = intendedAlg;
		m_symmAlg = symmAlg;
		m_encSymmKey = encSymmKey;
		m_keyAlg = keyAlg;
		m_valueHint = valueHint;
		m_encValue = encValue;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(6);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, m_intendedAlg);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, m_symmAlg);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 2, m_encSymmKey);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 3, m_keyAlg);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 4, m_valueHint);
		asn1EncodableVector.Add(m_encValue);
		return new DerSequence(asn1EncodableVector);
	}
}
