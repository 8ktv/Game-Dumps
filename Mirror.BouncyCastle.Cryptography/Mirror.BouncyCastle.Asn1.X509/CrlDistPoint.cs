using System.Text;

namespace Mirror.BouncyCastle.Asn1.X509;

public class CrlDistPoint : Asn1Encodable
{
	internal readonly Asn1Sequence seq;

	public static CrlDistPoint GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public static CrlDistPoint GetInstance(object obj)
	{
		if (obj is CrlDistPoint)
		{
			return (CrlDistPoint)obj;
		}
		if (obj == null)
		{
			return null;
		}
		return new CrlDistPoint(Asn1Sequence.GetInstance(obj));
	}

	public static CrlDistPoint FromExtensions(X509Extensions extensions)
	{
		return GetInstance(X509Extensions.GetExtensionParsedValue(extensions, X509Extensions.CrlDistributionPoints));
	}

	private CrlDistPoint(Asn1Sequence seq)
	{
		this.seq = seq;
	}

	public CrlDistPoint(DistributionPoint[] points)
	{
		seq = new DerSequence(points);
	}

	public DistributionPoint[] GetDistributionPoints()
	{
		return seq.MapElements(DistributionPoint.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return seq;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("CRLDistPoint:");
		DistributionPoint[] distributionPoints = GetDistributionPoints();
		foreach (DistributionPoint value in distributionPoints)
		{
			stringBuilder.Append("    ").Append(value).AppendLine();
		}
		return stringBuilder.ToString();
	}
}
