using System;

namespace Mirror.BouncyCastle.Asn1.Tsp;

public class EncryptionInfo : Asn1Encodable
{
	private readonly DerObjectIdentifier m_encryptionInfoType;

	private readonly Asn1Encodable m_encryptionInfoValue;

	public virtual DerObjectIdentifier EncryptionInfoType => m_encryptionInfoType;

	public virtual Asn1Encodable EncryptionInfoValue => m_encryptionInfoValue;

	public static EncryptionInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is EncryptionInfo result)
		{
			return result;
		}
		return new EncryptionInfo(Asn1Sequence.GetInstance(obj));
	}

	public static EncryptionInfo GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new EncryptionInfo(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private EncryptionInfo(Asn1Sequence sequence)
	{
		if (sequence.Count != 2)
		{
			throw new ArgumentException("wrong sequence size in constructor: " + sequence.Count, "sequence");
		}
		m_encryptionInfoType = DerObjectIdentifier.GetInstance(sequence[0]);
		m_encryptionInfoValue = sequence[1];
	}

	public EncryptionInfo(DerObjectIdentifier encryptionInfoType, Asn1Encodable encryptionInfoValue)
	{
		m_encryptionInfoType = encryptionInfoType;
		m_encryptionInfoValue = encryptionInfoValue;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DLSequence(m_encryptionInfoType, m_encryptionInfoValue);
	}
}
