using System;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class SignaturePolicyIdentifier : Asn1Encodable, IAsn1Choice
{
	private readonly SignaturePolicyId m_sigPolicy;

	public SignaturePolicyId SignaturePolicyId => m_sigPolicy;

	public static SignaturePolicyIdentifier GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SignaturePolicyIdentifier result)
		{
			return result;
		}
		if (obj is Asn1Null)
		{
			return new SignaturePolicyIdentifier();
		}
		return new SignaturePolicyIdentifier(SignaturePolicyId.GetInstance(obj));
	}

	public static SignaturePolicyIdentifier GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	public SignaturePolicyIdentifier()
	{
		m_sigPolicy = null;
	}

	public SignaturePolicyIdentifier(SignaturePolicyId signaturePolicyId)
	{
		m_sigPolicy = signaturePolicyId ?? throw new ArgumentNullException("signaturePolicyId");
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_sigPolicy?.ToAsn1Object() ?? DerNull.Instance;
	}
}
