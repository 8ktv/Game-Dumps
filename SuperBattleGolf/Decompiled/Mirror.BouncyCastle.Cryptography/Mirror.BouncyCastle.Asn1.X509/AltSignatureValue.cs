namespace Mirror.BouncyCastle.Asn1.X509;

public class AltSignatureValue : Asn1Encodable
{
	private readonly DerBitString m_signature;

	public DerBitString Signature => m_signature;

	public static AltSignatureValue GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is AltSignatureValue result)
		{
			return result;
		}
		return new AltSignatureValue(DerBitString.GetInstance(obj));
	}

	public static AltSignatureValue GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(DerBitString.GetInstance(taggedObject, declaredExplicit));
	}

	public static AltSignatureValue FromExtensions(X509Extensions extensions)
	{
		return GetInstance(X509Extensions.GetExtensionParsedValue(extensions, X509Extensions.AltSignatureValue));
	}

	private AltSignatureValue(DerBitString signature)
	{
		m_signature = signature;
	}

	public AltSignatureValue(byte[] signature)
	{
		m_signature = new DerBitString(signature);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_signature;
	}
}
