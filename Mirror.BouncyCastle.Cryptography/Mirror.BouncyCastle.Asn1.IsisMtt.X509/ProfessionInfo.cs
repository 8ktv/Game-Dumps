using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X500;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.IsisMtt.X509;

public class ProfessionInfo : Asn1Encodable
{
	public static readonly DerObjectIdentifier Rechtsanwltin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".1");

	public static readonly DerObjectIdentifier Rechtsanwalt = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".2");

	public static readonly DerObjectIdentifier Rechtsbeistand = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".3");

	public static readonly DerObjectIdentifier Steuerberaterin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".4");

	public static readonly DerObjectIdentifier Steuerberater = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".5");

	public static readonly DerObjectIdentifier Steuerbevollmchtigte = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".6");

	public static readonly DerObjectIdentifier Steuerbevollmchtigter = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".7");

	public static readonly DerObjectIdentifier Notarin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".8");

	public static readonly DerObjectIdentifier Notar = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".9");

	public static readonly DerObjectIdentifier Notarvertreterin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".10");

	public static readonly DerObjectIdentifier Notarvertreter = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".11");

	public static readonly DerObjectIdentifier Notariatsverwalterin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".12");

	public static readonly DerObjectIdentifier Notariatsverwalter = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".13");

	public static readonly DerObjectIdentifier Wirtschaftsprferin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".14");

	public static readonly DerObjectIdentifier Wirtschaftsprfer = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".15");

	public static readonly DerObjectIdentifier VereidigteBuchprferin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".16");

	public static readonly DerObjectIdentifier VereidigterBuchprfer = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".17");

	public static readonly DerObjectIdentifier Patentanwltin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".18");

	public static readonly DerObjectIdentifier Patentanwalt = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern?.ToString() + ".19");

	private readonly NamingAuthority namingAuthority;

	private readonly Asn1Sequence professionItems;

	private readonly Asn1Sequence professionOids;

	private readonly string registrationNumber;

	private readonly Asn1OctetString addProfessionInfo;

	public virtual Asn1OctetString AddProfessionInfo => addProfessionInfo;

	public virtual NamingAuthority NamingAuthority => namingAuthority;

	public virtual string RegistrationNumber => registrationNumber;

	public static ProfessionInfo GetInstance(object obj)
	{
		if (obj == null || obj is ProfessionInfo)
		{
			return (ProfessionInfo)obj;
		}
		if (obj is Asn1Sequence seq)
		{
			return new ProfessionInfo(seq);
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	private ProfessionInfo(Asn1Sequence seq)
	{
		if (seq.Count > 5)
		{
			throw new ArgumentException("Bad sequence size: " + seq.Count);
		}
		IEnumerator<Asn1Encodable> enumerator = seq.GetEnumerator();
		enumerator.MoveNext();
		Asn1Encodable current = enumerator.Current;
		if (current is Asn1TaggedObject asn1TaggedObject)
		{
			if (asn1TaggedObject.TagNo != 0)
			{
				throw new ArgumentException("Bad tag number: " + asn1TaggedObject.TagNo);
			}
			namingAuthority = NamingAuthority.GetInstance(asn1TaggedObject, isExplicit: true);
			enumerator.MoveNext();
			current = enumerator.Current;
		}
		professionItems = Asn1Sequence.GetInstance(current);
		if (enumerator.MoveNext())
		{
			current = enumerator.Current;
			if (current is Asn1Sequence asn1Sequence)
			{
				professionOids = asn1Sequence;
			}
			else if (current is DerPrintableString derPrintableString)
			{
				registrationNumber = derPrintableString.GetString();
			}
			else
			{
				if (!(current is Asn1OctetString asn1OctetString))
				{
					throw new ArgumentException("Bad object encountered: " + Platform.GetTypeName(current));
				}
				addProfessionInfo = asn1OctetString;
			}
		}
		if (enumerator.MoveNext())
		{
			current = enumerator.Current;
			if (current is DerPrintableString derPrintableString2)
			{
				registrationNumber = derPrintableString2.GetString();
			}
			else
			{
				if (!(current is Asn1OctetString asn1OctetString2))
				{
					throw new ArgumentException("Bad object encountered: " + Platform.GetTypeName(current));
				}
				addProfessionInfo = asn1OctetString2;
			}
		}
		if (enumerator.MoveNext())
		{
			current = enumerator.Current;
			if (!(current is Asn1OctetString asn1OctetString3))
			{
				throw new ArgumentException("Bad object encountered: " + Platform.GetTypeName(current));
			}
			addProfessionInfo = asn1OctetString3;
		}
	}

	public ProfessionInfo(NamingAuthority namingAuthority, DirectoryString[] professionItems, DerObjectIdentifier[] professionOids, string registrationNumber, Asn1OctetString addProfessionInfo)
	{
		this.namingAuthority = namingAuthority;
		Asn1Encodable[] elements = professionItems;
		this.professionItems = new DerSequence(elements);
		if (professionOids != null)
		{
			elements = professionOids;
			this.professionOids = new DerSequence(elements);
		}
		this.registrationNumber = registrationNumber;
		this.addProfessionInfo = addProfessionInfo;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(5);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, namingAuthority);
		asn1EncodableVector.Add(professionItems);
		asn1EncodableVector.AddOptional(professionOids);
		if (registrationNumber != null)
		{
			asn1EncodableVector.Add(new DerPrintableString(registrationNumber, validate: true));
		}
		asn1EncodableVector.AddOptional(addProfessionInfo);
		return new DerSequence(asn1EncodableVector);
	}

	public virtual DirectoryString[] GetProfessionItems()
	{
		return professionItems.MapElements(DirectoryString.GetInstance);
	}

	public virtual DerObjectIdentifier[] GetProfessionOids()
	{
		return professionOids?.MapElements(DerObjectIdentifier.GetInstance) ?? new DerObjectIdentifier[0];
	}
}
