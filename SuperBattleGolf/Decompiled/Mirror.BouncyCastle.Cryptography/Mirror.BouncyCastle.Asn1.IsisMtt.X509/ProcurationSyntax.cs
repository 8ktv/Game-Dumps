using System;
using Mirror.BouncyCastle.Asn1.X500;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.IsisMtt.X509;

public class ProcurationSyntax : Asn1Encodable
{
	private readonly string country;

	private readonly DirectoryString typeOfSubstitution;

	private readonly GeneralName thirdPerson;

	private readonly IssuerSerial certRef;

	public virtual string Country => country;

	public virtual DirectoryString TypeOfSubstitution => typeOfSubstitution;

	public virtual GeneralName ThirdPerson => thirdPerson;

	public virtual IssuerSerial CertRef => certRef;

	public static ProcurationSyntax GetInstance(object obj)
	{
		if (obj == null || obj is ProcurationSyntax)
		{
			return (ProcurationSyntax)obj;
		}
		if (obj is Asn1Sequence seq)
		{
			return new ProcurationSyntax(seq);
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	private ProcurationSyntax(Asn1Sequence seq)
	{
		if (seq.Count < 1 || seq.Count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + seq.Count);
		}
		foreach (Asn1Encodable item in seq)
		{
			Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(item, 128);
			switch (instance.TagNo)
			{
			case 1:
				country = DerPrintableString.GetInstance(instance, declaredExplicit: true).GetString();
				break;
			case 2:
				typeOfSubstitution = DirectoryString.GetInstance(instance, isExplicit: true);
				break;
			case 3:
			{
				Asn1Encodable explicitBaseObject = instance.GetExplicitBaseObject();
				if (explicitBaseObject is Asn1TaggedObject)
				{
					thirdPerson = GeneralName.GetInstance(explicitBaseObject);
				}
				else
				{
					certRef = IssuerSerial.GetInstance(explicitBaseObject);
				}
				break;
			}
			default:
				throw new ArgumentException("Bad tag number: " + instance.TagNo);
			}
		}
	}

	public ProcurationSyntax(string country, DirectoryString typeOfSubstitution, IssuerSerial certRef)
	{
		this.country = country;
		this.typeOfSubstitution = typeOfSubstitution;
		thirdPerson = null;
		this.certRef = certRef;
	}

	public ProcurationSyntax(string country, DirectoryString typeOfSubstitution, GeneralName thirdPerson)
	{
		this.country = country;
		this.typeOfSubstitution = typeOfSubstitution;
		this.thirdPerson = thirdPerson;
		certRef = null;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		if (country != null)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: true, 1, new DerPrintableString(country, validate: true)));
		}
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 2, typeOfSubstitution);
		if (thirdPerson != null)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: true, 3, thirdPerson));
		}
		else
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: true, 3, certRef));
		}
		return new DerSequence(asn1EncodableVector);
	}
}
