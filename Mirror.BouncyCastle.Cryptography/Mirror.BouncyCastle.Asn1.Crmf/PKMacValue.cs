using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Crmf;

public class PKMacValue : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_algID;

	private readonly DerBitString m_macValue;

	public virtual AlgorithmIdentifier AlgID => m_algID;

	public virtual DerBitString MacValue => m_macValue;

	public static PKMacValue GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PKMacValue result)
		{
			return result;
		}
		return new PKMacValue(Asn1Sequence.GetInstance(obj));
	}

	public static PKMacValue GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return new PKMacValue(Asn1Sequence.GetInstance(obj, isExplicit));
	}

	private PKMacValue(Asn1Sequence seq)
	{
		m_algID = AlgorithmIdentifier.GetInstance(seq[0]);
		m_macValue = DerBitString.GetInstance(seq[1]);
	}

	public PKMacValue(PbmParameter pbmParams, DerBitString macValue)
		: this(new AlgorithmIdentifier(CmpObjectIdentifiers.passwordBasedMac, pbmParams), macValue)
	{
	}

	public PKMacValue(AlgorithmIdentifier algID, DerBitString macValue)
	{
		m_algID = algID;
		m_macValue = macValue;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_algID, m_macValue);
	}
}
