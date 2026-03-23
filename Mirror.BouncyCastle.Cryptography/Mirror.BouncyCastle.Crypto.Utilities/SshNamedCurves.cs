using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Sec;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Crypto.Utilities;

public static class SshNamedCurves
{
	private static readonly Dictionary<string, DerObjectIdentifier> objIds;

	private static readonly Dictionary<DerObjectIdentifier, string> names;

	public static IEnumerable<string> Names => CollectionUtilities.Proxy(objIds.Keys);

	private static void DefineCurveAlias(string name, DerObjectIdentifier oid)
	{
		if (FindByOidLazy(oid) == null)
		{
			throw new InvalidOperationException();
		}
		objIds.Add(name, oid);
		names.Add(oid, name);
	}

	private static X9ECParametersHolder FindByOidLazy(DerObjectIdentifier oid)
	{
		return ECKeyPairGenerator.FindECCurveByOidLazy(oid);
	}

	static SshNamedCurves()
	{
		objIds = new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);
		names = new Dictionary<DerObjectIdentifier, string>();
		DefineCurveAlias("nistp192", SecObjectIdentifiers.SecP192r1);
		DefineCurveAlias("nistp224", SecObjectIdentifiers.SecP224r1);
		DefineCurveAlias("nistp256", SecObjectIdentifiers.SecP256r1);
		DefineCurveAlias("nistp384", SecObjectIdentifiers.SecP384r1);
		DefineCurveAlias("nistp521", SecObjectIdentifiers.SecP521r1);
		DefineCurveAlias("nistb233", SecObjectIdentifiers.SecT233r1);
		DefineCurveAlias("nistb409", SecObjectIdentifiers.SecT409r1);
		DefineCurveAlias("nistk163", SecObjectIdentifiers.SecT163k1);
		DefineCurveAlias("nistk233", SecObjectIdentifiers.SecT233k1);
		DefineCurveAlias("nistk283", SecObjectIdentifiers.SecT283k1);
		DefineCurveAlias("nistk409", SecObjectIdentifiers.SecT409k1);
		DefineCurveAlias("nistt571", SecObjectIdentifiers.SecT571k1);
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
		if (!names.ContainsKey(oid))
		{
			return null;
		}
		return FindByOidLazy(oid);
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
