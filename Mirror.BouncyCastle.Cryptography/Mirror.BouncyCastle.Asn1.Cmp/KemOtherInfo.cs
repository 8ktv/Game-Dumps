using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class KemOtherInfo : Asn1Encodable
{
	private static readonly PkiFreeText DEFAULT_staticString = new PkiFreeText("CMP-KEM");

	private readonly PkiFreeText m_staticString;

	private readonly Asn1OctetString m_transactionID;

	private readonly Asn1OctetString m_senderNonce;

	private readonly Asn1OctetString m_recipNonce;

	private readonly DerInteger m_len;

	private readonly AlgorithmIdentifier m_mac;

	private readonly Asn1OctetString m_ct;

	public virtual Asn1OctetString TransactionID => m_transactionID;

	public virtual Asn1OctetString SenderNonce => m_senderNonce;

	public virtual Asn1OctetString RecipNonce => m_recipNonce;

	public virtual DerInteger Len => m_len;

	public virtual AlgorithmIdentifier Mac => m_mac;

	public virtual Asn1OctetString Ct => m_ct;

	public static KemOtherInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is KemOtherInfo result)
		{
			return result;
		}
		return new KemOtherInfo(Asn1Sequence.GetInstance(obj));
	}

	public static KemOtherInfo GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new KemOtherInfo(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public KemOtherInfo(Asn1OctetString transactionID, Asn1OctetString senderNonce, Asn1OctetString recipNonce, DerInteger len, AlgorithmIdentifier mac, Asn1OctetString ct)
	{
		m_staticString = DEFAULT_staticString;
		m_transactionID = transactionID;
		m_senderNonce = senderNonce;
		m_recipNonce = recipNonce;
		m_len = len;
		m_mac = mac;
		m_ct = ct;
	}

	public KemOtherInfo(Asn1OctetString transactionID, Asn1OctetString senderNonce, Asn1OctetString recipNonce, long len, AlgorithmIdentifier mac, Asn1OctetString ct)
		: this(transactionID, senderNonce, recipNonce, new DerInteger(len), mac, ct)
	{
	}

	private KemOtherInfo(Asn1Sequence seq)
	{
		if (seq.Count < 4 || seq.Count > 7)
		{
			throw new ArgumentException("sequence size should be between 4 and 7 inclusive", "seq");
		}
		int num = 0;
		m_staticString = PkiFreeText.GetInstance(seq[num]);
		if (!DEFAULT_staticString.Equals(m_staticString))
		{
			throw new ArgumentException("staticString field should be " + DEFAULT_staticString);
		}
		Asn1TaggedObject asn1TaggedObject = seq[++num] as Asn1TaggedObject;
		if (asn1TaggedObject != null && Asn1Utilities.TryGetContextBaseUniversal(asn1TaggedObject, 0, declaredExplicit: true, 4, out var baseUniversal))
		{
			m_transactionID = (Asn1OctetString)baseUniversal;
			asn1TaggedObject = seq[++num] as Asn1TaggedObject;
		}
		if (asn1TaggedObject != null && Asn1Utilities.TryGetContextBaseUniversal(asn1TaggedObject, 1, declaredExplicit: true, 4, out var baseUniversal2))
		{
			m_senderNonce = (Asn1OctetString)baseUniversal2;
			asn1TaggedObject = seq[++num] as Asn1TaggedObject;
		}
		if (asn1TaggedObject != null && Asn1Utilities.TryGetContextBaseUniversal(asn1TaggedObject, 2, declaredExplicit: true, 4, out var baseUniversal3))
		{
			m_recipNonce = (Asn1OctetString)baseUniversal3;
			asn1TaggedObject = seq[++num] as Asn1TaggedObject;
		}
		if (asn1TaggedObject != null)
		{
			throw new ArgumentException("unknown tag: " + Asn1Utilities.GetTagText(asn1TaggedObject));
		}
		m_len = DerInteger.GetInstance(seq[num]);
		m_mac = AlgorithmIdentifier.GetInstance(seq[++num]);
		m_ct = Asn1OctetString.GetInstance(seq[++num]);
		if (++num != seq.Count)
		{
			throw new ArgumentException("unexpected data at end of sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(7);
		asn1EncodableVector.Add(m_staticString);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_transactionID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_senderNonce);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 2, m_recipNonce);
		asn1EncodableVector.Add(m_len);
		asn1EncodableVector.Add(m_mac);
		asn1EncodableVector.Add(m_ct);
		return new DerSequence(asn1EncodableVector);
	}
}
