using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;

namespace Mirror.BouncyCastle.Bcpg;

public class ECDsaPublicBcpgKey : ECPublicBcpgKey
{
	protected internal ECDsaPublicBcpgKey(BcpgInputStream bcpgIn)
		: base(bcpgIn)
	{
	}

	public ECDsaPublicBcpgKey(DerObjectIdentifier oid, ECPoint point)
		: base(oid, point)
	{
	}

	public ECDsaPublicBcpgKey(DerObjectIdentifier oid, BigInteger encodedPoint)
		: base(oid, encodedPoint)
	{
	}
}
