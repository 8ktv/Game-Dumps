using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.IsisMtt.X509;

public class Admissions : Asn1Encodable
{
	private readonly GeneralName admissionAuthority;

	private readonly NamingAuthority namingAuthority;

	private readonly Asn1Sequence professionInfos;

	public virtual GeneralName AdmissionAuthority => admissionAuthority;

	public virtual NamingAuthority NamingAuthority => namingAuthority;

	public static Admissions GetInstance(object obj)
	{
		if (obj == null || obj is Admissions)
		{
			return (Admissions)obj;
		}
		if (obj is Asn1Sequence seq)
		{
			return new Admissions(seq);
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	private Admissions(Asn1Sequence seq)
	{
		if (seq.Count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + seq.Count);
		}
		IEnumerator<Asn1Encodable> enumerator = seq.GetEnumerator();
		enumerator.MoveNext();
		Asn1Encodable current = enumerator.Current;
		if (current is Asn1TaggedObject { TagNo: var tagNo } asn1TaggedObject)
		{
			switch (tagNo)
			{
			case 0:
				admissionAuthority = GeneralName.GetInstance(asn1TaggedObject, explicitly: true);
				break;
			case 1:
				namingAuthority = NamingAuthority.GetInstance(asn1TaggedObject, isExplicit: true);
				break;
			default:
				throw new ArgumentException("Bad tag number: " + asn1TaggedObject.TagNo);
			}
			enumerator.MoveNext();
			current = enumerator.Current;
		}
		if (current is Asn1TaggedObject asn1TaggedObject2)
		{
			if (asn1TaggedObject2.TagNo != 1)
			{
				throw new ArgumentException("Bad tag number: " + asn1TaggedObject2.TagNo);
			}
			namingAuthority = NamingAuthority.GetInstance(asn1TaggedObject2, isExplicit: true);
			enumerator.MoveNext();
			current = enumerator.Current;
		}
		professionInfos = Asn1Sequence.GetInstance(current);
		if (enumerator.MoveNext())
		{
			throw new ArgumentException("Bad object encountered: " + Platform.GetTypeName(enumerator.Current));
		}
	}

	public Admissions(GeneralName admissionAuthority, NamingAuthority namingAuthority, ProfessionInfo[] professionInfos)
	{
		this.admissionAuthority = admissionAuthority;
		this.namingAuthority = namingAuthority;
		this.professionInfos = new DerSequence(professionInfos);
	}

	public ProfessionInfo[] GetProfessionInfos()
	{
		ProfessionInfo[] array = new ProfessionInfo[professionInfos.Count];
		int num = 0;
		foreach (Asn1Encodable professionInfo in professionInfos)
		{
			array[num++] = ProfessionInfo.GetInstance(professionInfo);
		}
		return array;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, admissionAuthority);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, namingAuthority);
		asn1EncodableVector.Add(professionInfos);
		return new DerSequence(asn1EncodableVector);
	}
}
