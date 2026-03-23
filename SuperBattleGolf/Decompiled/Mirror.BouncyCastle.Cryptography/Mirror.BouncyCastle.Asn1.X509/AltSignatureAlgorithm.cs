namespace Mirror.BouncyCastle.Asn1.X509;

public class AltSignatureAlgorithm : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_algorithm;

	public AlgorithmIdentifier Algorithm => m_algorithm;

	public static AltSignatureAlgorithm GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is AltSignatureAlgorithm result)
		{
			return result;
		}
		return new AltSignatureAlgorithm(AlgorithmIdentifier.GetInstance(obj));
	}

	public static AltSignatureAlgorithm GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(AlgorithmIdentifier.GetInstance(taggedObject, declaredExplicit));
	}

	public static AltSignatureAlgorithm FromExtensions(X509Extensions extensions)
	{
		return GetInstance(X509Extensions.GetExtensionParsedValue(extensions, X509Extensions.AltSignatureAlgorithm));
	}

	public AltSignatureAlgorithm(AlgorithmIdentifier algorithm)
	{
		m_algorithm = algorithm;
	}

	public AltSignatureAlgorithm(DerObjectIdentifier algorithm)
		: this(algorithm, null)
	{
	}

	public AltSignatureAlgorithm(DerObjectIdentifier algorithm, Asn1Encodable parameters)
	{
		m_algorithm = new AlgorithmIdentifier(algorithm, parameters);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_algorithm.ToAsn1Object();
	}
}
