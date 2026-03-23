using System;
using System.Text;

namespace Mirror.BouncyCastle.Asn1.X509;

public class DistributionPointName : Asn1Encodable, IAsn1Choice
{
	public const int FullName = 0;

	public const int NameRelativeToCrlIssuer = 1;

	private readonly int m_type;

	private readonly Asn1Encodable m_name;

	[Obsolete("Use 'Type' instead")]
	public int PointType => m_type;

	public Asn1Encodable Name => m_name;

	public int Type => m_type;

	public static DistributionPointName GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DistributionPointName result)
		{
			return result;
		}
		return new DistributionPointName(Asn1TaggedObject.GetInstance(obj));
	}

	public static DistributionPointName GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return Asn1Utilities.GetInstanceFromChoice(obj, explicitly, GetInstance);
	}

	public DistributionPointName(GeneralNames name)
		: this(0, name)
	{
	}

	public DistributionPointName(int type, Asn1Encodable name)
	{
		m_type = type;
		m_name = name;
	}

	public DistributionPointName(Asn1TaggedObject obj)
	{
		m_type = obj.TagNo;
		if (m_type == 0)
		{
			m_name = GeneralNames.GetInstance(obj, explicitly: false);
		}
		else
		{
			m_name = Asn1Set.GetInstance(obj, declaredExplicit: false);
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerTaggedObject(isExplicit: false, m_type, m_name);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("DistributionPointName: [");
		if (m_type == 0)
		{
			AppendObject(stringBuilder, "fullName", m_name.ToString());
		}
		else
		{
			AppendObject(stringBuilder, "nameRelativeToCRLIssuer", m_name.ToString());
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
