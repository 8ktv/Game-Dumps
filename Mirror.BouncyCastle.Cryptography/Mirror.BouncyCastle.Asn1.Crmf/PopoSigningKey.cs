using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Crmf;

public class PopoSigningKey : Asn1Encodable
{
	private readonly PopoSigningKeyInput m_poposkInput;

	private readonly AlgorithmIdentifier m_algorithmIdentifier;

	private readonly DerBitString m_signature;

	public virtual PopoSigningKeyInput PoposkInput => m_poposkInput;

	public virtual AlgorithmIdentifier AlgorithmIdentifier => m_algorithmIdentifier;

	public virtual DerBitString Signature => m_signature;

	public static PopoSigningKey GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PopoSigningKey result)
		{
			return result;
		}
		return new PopoSigningKey(Asn1Sequence.GetInstance(obj));
	}

	public static PopoSigningKey GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return new PopoSigningKey(Asn1Sequence.GetInstance(obj, isExplicit));
	}

	private PopoSigningKey(Asn1Sequence seq)
	{
		int num = 0;
		if (seq[num] is Asn1TaggedObject taggedObject)
		{
			num++;
			m_poposkInput = PopoSigningKeyInput.GetInstance(Asn1Utilities.GetContextBaseUniversal(taggedObject, 0, declaredExplicit: false, 16));
		}
		m_algorithmIdentifier = AlgorithmIdentifier.GetInstance(seq[num++]);
		m_signature = DerBitString.GetInstance(seq[num]);
	}

	public PopoSigningKey(PopoSigningKeyInput poposkIn, AlgorithmIdentifier aid, DerBitString signature)
	{
		m_poposkInput = poposkIn;
		m_algorithmIdentifier = aid;
		m_signature = signature;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, m_poposkInput);
		asn1EncodableVector.Add(m_algorithmIdentifier);
		asn1EncodableVector.Add(m_signature);
		return new DerSequence(asn1EncodableVector);
	}
}
