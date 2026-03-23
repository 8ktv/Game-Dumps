using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pkcs;

public class AsymmetricKeyEntry : Pkcs12Entry
{
	private readonly AsymmetricKeyParameter key;

	public AsymmetricKeyParameter Key => key;

	public AsymmetricKeyEntry(AsymmetricKeyParameter key)
		: base(new Dictionary<DerObjectIdentifier, Asn1Encodable>())
	{
		this.key = key;
	}

	public AsymmetricKeyEntry(AsymmetricKeyParameter key, IDictionary<DerObjectIdentifier, Asn1Encodable> attributes)
		: base(attributes)
	{
		this.key = key;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is AsymmetricKeyEntry asymmetricKeyEntry))
		{
			return false;
		}
		return key.Equals(asymmetricKeyEntry.key);
	}

	public override int GetHashCode()
	{
		return ~key.GetHashCode();
	}
}
