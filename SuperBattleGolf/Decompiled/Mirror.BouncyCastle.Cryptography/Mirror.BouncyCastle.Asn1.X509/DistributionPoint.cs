using System.Text;

namespace Mirror.BouncyCastle.Asn1.X509;

public class DistributionPoint : Asn1Encodable
{
	private readonly DistributionPointName m_distributionPoint;

	private readonly ReasonFlags m_reasons;

	private readonly GeneralNames m_crlIssuer;

	public DistributionPointName DistributionPointName => m_distributionPoint;

	public ReasonFlags Reasons => m_reasons;

	public GeneralNames CrlIssuer => m_crlIssuer;

	public static DistributionPoint GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DistributionPoint result)
		{
			return result;
		}
		return new DistributionPoint(Asn1Sequence.GetInstance(obj));
	}

	public static DistributionPoint GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
	}

	private DistributionPoint(Asn1Sequence seq)
	{
		for (int i = 0; i != seq.Count; i++)
		{
			Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[i]);
			switch (instance.TagNo)
			{
			case 0:
				m_distributionPoint = DistributionPointName.GetInstance(instance, explicitly: true);
				break;
			case 1:
				m_reasons = new ReasonFlags(DerBitString.GetInstance(instance, isExplicit: false));
				break;
			case 2:
				m_crlIssuer = GeneralNames.GetInstance(instance, explicitly: false);
				break;
			}
		}
	}

	public DistributionPoint(DistributionPointName distributionPointName, ReasonFlags reasons, GeneralNames crlIssuer)
	{
		m_distributionPoint = distributionPointName;
		m_reasons = reasons;
		m_crlIssuer = crlIssuer;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_distributionPoint);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, m_reasons);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 2, m_crlIssuer);
		return new DerSequence(asn1EncodableVector);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("DistributionPoint: [");
		if (m_distributionPoint != null)
		{
			AppendObject(stringBuilder, "distributionPoint", m_distributionPoint.ToString());
		}
		if (m_reasons != null)
		{
			AppendObject(stringBuilder, "reasons", m_reasons.ToString());
		}
		if (m_crlIssuer != null)
		{
			AppendObject(stringBuilder, "cRLIssuer", m_crlIssuer.ToString());
		}
		stringBuilder.AppendLine("]");
		return stringBuilder.ToString();
	}

	private void AppendObject(StringBuilder buf, string name, string val)
	{
		string value = "    ";
		buf.Append(value);
		buf.Append(name);
		buf.AppendLine(":");
		buf.Append(value);
		buf.Append(value);
		buf.Append(val);
		buf.AppendLine();
	}
}
