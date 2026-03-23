using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X500;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.IsisMtt.X509;

public class NamingAuthority : Asn1Encodable
{
	public static readonly DerObjectIdentifier IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttATNamingAuthorities?.ToString() + ".1");

	private readonly DerObjectIdentifier namingAuthorityID;

	private readonly string namingAuthorityUrl;

	private readonly DirectoryString namingAuthorityText;

	public virtual DerObjectIdentifier NamingAuthorityID => namingAuthorityID;

	public virtual DirectoryString NamingAuthorityText => namingAuthorityText;

	public virtual string NamingAuthorityUrl => namingAuthorityUrl;

	public static NamingAuthority GetInstance(object obj)
	{
		if (obj == null || obj is NamingAuthority)
		{
			return (NamingAuthority)obj;
		}
		if (obj is Asn1Sequence seq)
		{
			return new NamingAuthority(seq);
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	public static NamingAuthority GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
	}

	private NamingAuthority(Asn1Sequence seq)
	{
		if (seq.Count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + seq.Count);
		}
		IEnumerator<Asn1Encodable> enumerator = seq.GetEnumerator();
		if (enumerator.MoveNext())
		{
			Asn1Encodable current = enumerator.Current;
			if (current is DerObjectIdentifier derObjectIdentifier)
			{
				namingAuthorityID = derObjectIdentifier;
			}
			else if (current is DerIA5String derIA5String)
			{
				namingAuthorityUrl = derIA5String.GetString();
			}
			else
			{
				if (!(current is IAsn1String))
				{
					throw new ArgumentException("Bad object encountered: " + Platform.GetTypeName(current));
				}
				namingAuthorityText = DirectoryString.GetInstance(current);
			}
		}
		if (enumerator.MoveNext())
		{
			Asn1Encodable current2 = enumerator.Current;
			if (current2 is DerIA5String derIA5String2)
			{
				namingAuthorityUrl = derIA5String2.GetString();
			}
			else
			{
				if (!(current2 is IAsn1String))
				{
					throw new ArgumentException("Bad object encountered: " + Platform.GetTypeName(current2));
				}
				namingAuthorityText = DirectoryString.GetInstance(current2);
			}
		}
		if (enumerator.MoveNext())
		{
			Asn1Encodable current3 = enumerator.Current;
			if (!(current3 is IAsn1String))
			{
				throw new ArgumentException("Bad object encountered: " + Platform.GetTypeName(current3));
			}
			namingAuthorityText = DirectoryString.GetInstance(current3);
		}
	}

	public NamingAuthority(DerObjectIdentifier namingAuthorityID, string namingAuthorityUrl, DirectoryString namingAuthorityText)
	{
		this.namingAuthorityID = namingAuthorityID;
		this.namingAuthorityUrl = namingAuthorityUrl;
		this.namingAuthorityText = namingAuthorityText;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.AddOptional(namingAuthorityID);
		if (namingAuthorityUrl != null)
		{
			asn1EncodableVector.Add(new DerIA5String(namingAuthorityUrl, validate: true));
		}
		asn1EncodableVector.AddOptional(namingAuthorityText);
		return new DerSequence(asn1EncodableVector);
	}
}
