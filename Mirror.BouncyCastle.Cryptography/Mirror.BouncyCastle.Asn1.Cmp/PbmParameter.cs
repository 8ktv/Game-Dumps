using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PbmParameter : Asn1Encodable
{
	private readonly Asn1OctetString m_salt;

	private readonly AlgorithmIdentifier m_owf;

	private readonly DerInteger m_iterationCount;

	private readonly AlgorithmIdentifier m_mac;

	public virtual DerInteger IterationCount => m_iterationCount;

	public virtual AlgorithmIdentifier Mac => m_mac;

	public virtual AlgorithmIdentifier Owf => m_owf;

	public virtual Asn1OctetString Salt => m_salt;

	public static PbmParameter GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PbmParameter result)
		{
			return result;
		}
		return new PbmParameter(Asn1Sequence.GetInstance(obj));
	}

	public static PbmParameter GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PbmParameter(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private PbmParameter(Asn1Sequence seq)
	{
		m_salt = Asn1OctetString.GetInstance(seq[0]);
		m_owf = AlgorithmIdentifier.GetInstance(seq[1]);
		m_iterationCount = DerInteger.GetInstance(seq[2]);
		m_mac = AlgorithmIdentifier.GetInstance(seq[3]);
	}

	public PbmParameter(byte[] salt, AlgorithmIdentifier owf, int iterationCount, AlgorithmIdentifier mac)
		: this(new DerOctetString(salt), owf, new DerInteger(iterationCount), mac)
	{
	}

	public PbmParameter(Asn1OctetString salt, AlgorithmIdentifier owf, DerInteger iterationCount, AlgorithmIdentifier mac)
	{
		m_salt = salt;
		m_owf = owf;
		m_iterationCount = iterationCount;
		m_mac = mac;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_salt, m_owf, m_iterationCount, m_mac);
	}
}
