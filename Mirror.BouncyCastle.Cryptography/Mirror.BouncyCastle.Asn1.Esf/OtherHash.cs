using System;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class OtherHash : Asn1Encodable, IAsn1Choice
{
	private readonly Asn1OctetString m_sha1Hash;

	private readonly OtherHashAlgAndValue m_otherHash;

	public AlgorithmIdentifier HashAlgorithm => m_otherHash?.HashAlgorithm ?? new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1);

	public static OtherHash GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OtherHash result)
		{
			return result;
		}
		if (obj is Asn1OctetString sha1Hash)
		{
			return new OtherHash(sha1Hash);
		}
		return new OtherHash(OtherHashAlgAndValue.GetInstance(obj));
	}

	public static OtherHash GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	public OtherHash(byte[] sha1Hash)
	{
		if (sha1Hash == null)
		{
			throw new ArgumentNullException("sha1Hash");
		}
		m_sha1Hash = new DerOctetString(sha1Hash);
	}

	public OtherHash(Asn1OctetString sha1Hash)
	{
		m_sha1Hash = sha1Hash ?? throw new ArgumentNullException("sha1Hash");
	}

	public OtherHash(OtherHashAlgAndValue otherHash)
	{
		m_otherHash = otherHash ?? throw new ArgumentNullException("otherHash");
	}

	public byte[] GetHashValue()
	{
		return m_otherHash?.GetHashValue() ?? m_sha1Hash.GetOctets();
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_otherHash?.ToAsn1Object() ?? m_sha1Hash;
	}
}
