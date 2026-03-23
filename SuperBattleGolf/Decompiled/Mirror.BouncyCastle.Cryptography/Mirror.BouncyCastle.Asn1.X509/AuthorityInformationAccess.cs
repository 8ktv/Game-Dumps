using System;
using System.Text;

namespace Mirror.BouncyCastle.Asn1.X509;

public class AuthorityInformationAccess : Asn1Encodable
{
	private readonly AccessDescription[] descriptions;

	private static AccessDescription[] Copy(AccessDescription[] descriptions)
	{
		return (AccessDescription[])descriptions.Clone();
	}

	public static AuthorityInformationAccess GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is AuthorityInformationAccess result)
		{
			return result;
		}
		return new AuthorityInformationAccess(Asn1Sequence.GetInstance(obj));
	}

	public static AuthorityInformationAccess FromExtensions(X509Extensions extensions)
	{
		return GetInstance(X509Extensions.GetExtensionParsedValue(extensions, X509Extensions.AuthorityInfoAccess));
	}

	private AuthorityInformationAccess(Asn1Sequence seq)
	{
		if (seq.Count < 1)
		{
			throw new ArgumentException("sequence may not be empty");
		}
		descriptions = seq.MapElements(AccessDescription.GetInstance);
	}

	public AuthorityInformationAccess(AccessDescription description)
	{
		descriptions = new AccessDescription[1] { description };
	}

	public AuthorityInformationAccess(AccessDescription[] descriptions)
	{
		this.descriptions = Copy(descriptions);
	}

	public AuthorityInformationAccess(DerObjectIdentifier oid, GeneralName location)
		: this(new AccessDescription(oid, location))
	{
	}

	public AccessDescription[] GetAccessDescriptions()
	{
		return Copy(descriptions);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1Encodable[] elements = descriptions;
		return new DerSequence(elements);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("AuthorityInformationAccess:");
		AccessDescription[] array = descriptions;
		foreach (AccessDescription value in array)
		{
			stringBuilder.Append("    ").Append(value).AppendLine();
		}
		return stringBuilder.ToString();
	}
}
