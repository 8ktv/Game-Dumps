using System;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.X509.Store;

namespace Mirror.BouncyCastle.Cms;

public class SignerID : X509CertStoreSelector, IEquatable<SignerID>
{
	public virtual bool Equals(SignerID other)
	{
		if (other != null)
		{
			if (other != this)
			{
				if (MatchesSubjectKeyIdentifier(other) && MatchesSerialNumber(other))
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
		return Equals(obj as SignerID);
	}

	public override int GetHashCode()
	{
		return GetHashCodeOfSubjectKeyIdentifier() ^ Objects.GetHashCode(base.SerialNumber) ^ Objects.GetHashCode(base.Issuer);
	}
}
