using System;
using Mirror.BouncyCastle.Asn1.X500;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class SignerLocation : Asn1Encodable
{
	private readonly DirectoryString m_countryName;

	private readonly DirectoryString m_localityName;

	private readonly Asn1Sequence m_postalAddress;

	public DirectoryString Country => m_countryName;

	public DirectoryString Locality => m_localityName;

	public Asn1Sequence PostalAddress => m_postalAddress;

	public static SignerLocation GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SignerLocation result)
		{
			return result;
		}
		return new SignerLocation(Asn1Sequence.GetInstance(obj));
	}

	public static SignerLocation GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new SignerLocation(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public SignerLocation(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 0 || count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_countryName = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, DirectoryString.GetInstance);
		m_localityName = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 1, state: true, DirectoryString.GetInstance);
		m_postalAddress = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 2, state: true, Asn1Sequence.GetInstance);
		if (m_postalAddress != null)
		{
			if (m_postalAddress.Count > 6)
			{
				throw new ArgumentException("postal address must contain less than 6 strings");
			}
			m_postalAddress.MapElements((Asn1Encodable element) => DirectoryString.GetInstance(element.ToAsn1Object()));
		}
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	private SignerLocation(DirectoryString countryName, DirectoryString localityName, Asn1Sequence postalAddress)
	{
		if (postalAddress != null && postalAddress.Count > 6)
		{
			throw new ArgumentException("postal address must contain less than 6 strings");
		}
		m_countryName = countryName;
		m_localityName = localityName;
		m_postalAddress = postalAddress;
	}

	public SignerLocation(DirectoryString countryName, DirectoryString localityName, DirectoryString[] postalAddress)
		: this(countryName, localityName, new DerSequence(postalAddress))
	{
	}

	public SignerLocation(DerUtf8String countryName, DerUtf8String localityName, Asn1Sequence postalAddress)
		: this(DirectoryString.GetInstance(countryName), DirectoryString.GetInstance(localityName), postalAddress)
	{
	}

	public DirectoryString[] GetPostal()
	{
		return m_postalAddress?.MapElements((Asn1Encodable element) => DirectoryString.GetInstance(element.ToAsn1Object()));
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_countryName);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_localityName);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 2, m_postalAddress);
		return DerSequence.FromVector(asn1EncodableVector);
	}
}
