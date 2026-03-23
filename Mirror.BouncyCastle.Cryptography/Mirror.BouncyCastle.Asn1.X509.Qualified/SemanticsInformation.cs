using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.X509.Qualified;

public class SemanticsInformation : Asn1Encodable
{
	private readonly DerObjectIdentifier semanticsIdentifier;

	private readonly GeneralName[] nameRegistrationAuthorities;

	public DerObjectIdentifier SemanticsIdentifier => semanticsIdentifier;

	public static SemanticsInformation GetInstance(object obj)
	{
		if (obj == null || obj is SemanticsInformation)
		{
			return (SemanticsInformation)obj;
		}
		if (obj is Asn1Sequence)
		{
			return new SemanticsInformation(Asn1Sequence.GetInstance(obj));
		}
		throw new ArgumentException("unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public SemanticsInformation(Asn1Sequence seq)
	{
		if (seq.Count < 1)
		{
			throw new ArgumentException("no objects in SemanticsInformation");
		}
		IEnumerator<Asn1Encodable> enumerator = seq.GetEnumerator();
		enumerator.MoveNext();
		Asn1Encodable asn1Encodable = enumerator.Current;
		if (asn1Encodable is DerObjectIdentifier derObjectIdentifier)
		{
			semanticsIdentifier = derObjectIdentifier;
			asn1Encodable = ((!enumerator.MoveNext()) ? null : enumerator.Current);
		}
		if (asn1Encodable != null)
		{
			nameRegistrationAuthorities = Asn1Sequence.GetInstance(asn1Encodable).MapElements(GeneralName.GetInstance);
		}
	}

	public SemanticsInformation(DerObjectIdentifier semanticsIdentifier, GeneralName[] generalNames)
	{
		this.semanticsIdentifier = semanticsIdentifier;
		nameRegistrationAuthorities = generalNames;
	}

	public SemanticsInformation(DerObjectIdentifier semanticsIdentifier)
	{
		this.semanticsIdentifier = semanticsIdentifier;
	}

	public SemanticsInformation(GeneralName[] generalNames)
	{
		nameRegistrationAuthorities = generalNames;
	}

	public GeneralName[] GetNameRegistrationAuthorities()
	{
		return nameRegistrationAuthorities;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.AddOptional(semanticsIdentifier);
		if (nameRegistrationAuthorities != null)
		{
			Asn1Encodable[] elements = nameRegistrationAuthorities;
			asn1EncodableVector.Add(new DerSequence(elements));
		}
		return new DerSequence(asn1EncodableVector);
	}
}
