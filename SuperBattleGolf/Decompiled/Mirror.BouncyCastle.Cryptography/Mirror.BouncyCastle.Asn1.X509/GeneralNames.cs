using System.Text;

namespace Mirror.BouncyCastle.Asn1.X509;

public class GeneralNames : Asn1Encodable
{
	private readonly GeneralName[] m_names;

	public static GeneralNames GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is GeneralNames result)
		{
			return result;
		}
		return new GeneralNames(Asn1Sequence.GetInstance(obj));
	}

	public static GeneralNames GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public static GeneralNames FromExtensions(X509Extensions extensions, DerObjectIdentifier extOid)
	{
		return GetInstance(X509Extensions.GetExtensionParsedValue(extensions, extOid));
	}

	private static GeneralName[] Copy(GeneralName[] names)
	{
		return (GeneralName[])names.Clone();
	}

	public GeneralNames(GeneralName name)
	{
		m_names = new GeneralName[1] { name };
	}

	public GeneralNames(GeneralName[] names)
	{
		m_names = Copy(names);
	}

	private GeneralNames(Asn1Sequence seq)
	{
		m_names = seq.MapElements(GeneralName.GetInstance);
	}

	public GeneralName[] GetNames()
	{
		return Copy(m_names);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1Encodable[] names = m_names;
		return new DerSequence(names);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("GeneralNames:");
		GeneralName[] names = m_names;
		foreach (GeneralName value in names)
		{
			stringBuilder.Append("    ").Append(value).AppendLine();
		}
		return stringBuilder.ToString();
	}
}
