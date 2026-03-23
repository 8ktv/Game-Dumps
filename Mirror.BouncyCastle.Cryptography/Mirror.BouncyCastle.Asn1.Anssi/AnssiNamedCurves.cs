using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;
using Mirror.BouncyCastle.Math.EC.Multiplier;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Asn1.Anssi;

public static class AnssiNamedCurves
{
	internal class Frp256v1Holder : X9ECParametersHolder
	{
		internal static readonly X9ECParametersHolder Instance = new Frp256v1Holder();

		private Frp256v1Holder()
		{
		}

		protected override ECCurve CreateCurve()
		{
			BigInteger q = FromHex("F1FD178C0B3AD58F10126DE8CE42435B3961ADBCABC8CA6DE8FCF353D86E9C03");
			BigInteger a = FromHex("F1FD178C0B3AD58F10126DE8CE42435B3961ADBCABC8CA6DE8FCF353D86E9C00");
			BigInteger b = FromHex("EE353FCA5428A9300D4ABA754A44C00FDFEC0C9AE4B1A1803075ED967B7BB73F");
			BigInteger order = FromHex("F1FD178C0B3AD58F10126DE8CE42435B53DC67E140D2BF941FFDD459C6D655E1");
			BigInteger one = BigInteger.One;
			return ConfigureCurve(new FpCurve(q, a, b, order, one, isInternal: true));
		}

		protected override X9ECParameters CreateParameters()
		{
			byte[] seed = null;
			ECCurve curve = base.Curve;
			X9ECPoint g = ConfigureBasepoint(curve, "04B6B3D4C356C139EB31183D4749D423958C27D2DCAF98B70164C97A2DD98F5CFF6142E0F7C8B204911F9271F0F3ECEF8C2701C307E8E4C9E183115A1554062CFB");
			return new X9ECParameters(curve, g, curve.Order, curve.Cofactor, seed);
		}
	}

	private static readonly Dictionary<string, DerObjectIdentifier> objIds;

	private static readonly Dictionary<DerObjectIdentifier, X9ECParametersHolder> curves;

	private static readonly Dictionary<DerObjectIdentifier, string> names;

	public static IEnumerable<string> Names => CollectionUtilities.Proxy(objIds.Keys);

	private static X9ECPoint ConfigureBasepoint(ECCurve curve, string encoding)
	{
		X9ECPoint x9ECPoint = new X9ECPoint(curve, Hex.DecodeStrict(encoding));
		WNafUtilities.ConfigureBasepoint(x9ECPoint.Point);
		return x9ECPoint;
	}

	private static ECCurve ConfigureCurve(ECCurve curve)
	{
		return curve;
	}

	private static BigInteger FromHex(string hex)
	{
		return new BigInteger(1, Hex.DecodeStrict(hex));
	}

	private static void DefineCurve(string name, DerObjectIdentifier oid, X9ECParametersHolder holder)
	{
		objIds.Add(name, oid);
		names.Add(oid, name);
		curves.Add(oid, holder);
	}

	static AnssiNamedCurves()
	{
		objIds = new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);
		curves = new Dictionary<DerObjectIdentifier, X9ECParametersHolder>();
		names = new Dictionary<DerObjectIdentifier, string>();
		DefineCurve("FRP256v1", AnssiObjectIdentifiers.FRP256v1, Frp256v1Holder.Instance);
	}

	public static X9ECParameters GetByName(string name)
	{
		DerObjectIdentifier oid = GetOid(name);
		if (oid != null)
		{
			return GetByOid(oid);
		}
		return null;
	}

	public static X9ECParametersHolder GetByNameLazy(string name)
	{
		DerObjectIdentifier oid = GetOid(name);
		if (oid != null)
		{
			return GetByOidLazy(oid);
		}
		return null;
	}

	public static X9ECParameters GetByOid(DerObjectIdentifier oid)
	{
		return GetByOidLazy(oid)?.Parameters;
	}

	public static X9ECParametersHolder GetByOidLazy(DerObjectIdentifier oid)
	{
		return CollectionUtilities.GetValueOrNull(curves, oid);
	}

	public static string GetName(DerObjectIdentifier oid)
	{
		return CollectionUtilities.GetValueOrNull(names, oid);
	}

	public static DerObjectIdentifier GetOid(string name)
	{
		return CollectionUtilities.GetValueOrNull(objIds, name);
	}
}
