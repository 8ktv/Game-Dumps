using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PollReqContent : Asn1Encodable
{
	private readonly Asn1Sequence m_content;

	public static PollReqContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PollReqContent result)
		{
			return result;
		}
		return new PollReqContent(Asn1Sequence.GetInstance(obj));
	}

	public static PollReqContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PollReqContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private PollReqContent(Asn1Sequence seq)
	{
		m_content = seq;
	}

	public PollReqContent(DerInteger certReqId)
		: this(new DerSequence(new DerSequence(certReqId)))
	{
	}

	public PollReqContent(DerInteger[] certReqIds)
		: this(new DerSequence(IntsToSequence(certReqIds)))
	{
	}

	public PollReqContent(BigInteger certReqId)
		: this(new DerInteger(certReqId))
	{
	}

	public PollReqContent(BigInteger[] certReqIds)
		: this(IntsToAsn1(certReqIds))
	{
	}

	public virtual DerInteger[][] GetCertReqIDs()
	{
		return m_content.MapElements((Asn1Encodable element) => Asn1Sequence.GetInstance(element).MapElements(DerInteger.GetInstance));
	}

	public virtual BigInteger[] GetCertReqIDValues()
	{
		return m_content.MapElements((Asn1Encodable element) => DerInteger.GetInstance(Asn1Sequence.GetInstance(element)[0]).Value);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_content;
	}

	private static DerSequence[] IntsToSequence(DerInteger[] ids)
	{
		DerSequence[] array = new DerSequence[ids.Length];
		for (int i = 0; i != array.Length; i++)
		{
			array[i] = new DerSequence(ids[i]);
		}
		return array;
	}

	private static DerInteger[] IntsToAsn1(BigInteger[] ids)
	{
		DerInteger[] array = new DerInteger[ids.Length];
		for (int i = 0; i != array.Length; i++)
		{
			array[i] = new DerInteger(ids[i]);
		}
		return array;
	}
}
