using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Ocsp;

public class RespID : IEquatable<RespID>
{
	private readonly ResponderID m_id;

	public RespID(ResponderID id)
	{
		m_id = id ?? throw new ArgumentNullException("id");
	}

	public RespID(X509Name name)
	{
		m_id = new ResponderID(name);
	}

	public RespID(AsymmetricKeyParameter publicKey)
	{
		try
		{
			byte[] bytes = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey).PublicKey.GetBytes();
			byte[] contents = DigestUtilities.CalculateDigest("SHA1", bytes);
			m_id = new ResponderID(new DerOctetString(contents));
		}
		catch (Exception ex)
		{
			throw new OcspException("problem creating ID: " + ex, ex);
		}
	}

	public ResponderID ToAsn1Object()
	{
		return m_id;
	}

	public bool Equals(RespID other)
	{
		if (this != other)
		{
			return m_id.Equals(other?.m_id);
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as RespID);
	}

	public override int GetHashCode()
	{
		return m_id.GetHashCode();
	}
}
