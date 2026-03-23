using System;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.X509.Store;

namespace Mirror.BouncyCastle.Cms;

public class RecipientID : X509CertStoreSelector, IEquatable<RecipientID>
{
	private byte[] m_keyIdentifier;

	public byte[] KeyIdentifier
	{
		get
		{
			return Arrays.Clone(m_keyIdentifier);
		}
		set
		{
			m_keyIdentifier = Arrays.Clone(value);
		}
	}

	public virtual bool Equals(RecipientID other)
	{
		if (other != null)
		{
			if (other != this)
			{
				if (Arrays.AreEqual(m_keyIdentifier, other.m_keyIdentifier) && MatchesSubjectKeyIdentifier(other) && MatchesSerialNumber(other))
				{
					return MatchesIssuer(other);
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as RecipientID);
	}

	public override int GetHashCode()
	{
		return Arrays.GetHashCode(m_keyIdentifier) ^ GetHashCodeOfSubjectKeyIdentifier() ^ Objects.GetHashCode(base.SerialNumber) ^ Objects.GetHashCode(base.Issuer);
	}
}
