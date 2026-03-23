using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.Sec;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Asn1.Nist;

public static class NistNamedCurves
{
	private static readonly Dictionary<string, DerObjectIdentifier> objIds;

	private static readonly Dictionary<DerObjectIdentifier, string> names;

	public static IEnumerable<string> Names => CollectionUtilities.Proxy(objIds.Keys);

	private static void DefineCurveAlias(string name, DerObjectIdentifier oid)
	{
		if (SecNamedCurves.GetByOidLazy(oid) == null)
		{
			throw new InvalidOperationException();
		}
		objIds.Add(name, oid);
		names.Add(oid, name);
	}

	static NistNamedCurves()
	{
		objIds = new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);
		names = new Dictionary<DerObjectIdentifier, string>();
		DefineCurveAlias("B-163", SecObjectIdentifiers.SecT163r2);
		DefineCurveAlias("B-233", SecObjectIdentifiers.SecT233r1);
		DefineCurveAlias("B-283", SecObjectIdentifiers.SecT283r1);
		DefineCurveAlias("B-409", SecObjectIdentifiers.SecT409r1);
		DefineCurveAlias("B-571", SecObjectIdentifiers.SecT571r1);
		DefineCurveAlias("K-163", SecObjectIdentifiers.SecT163k1);
		DefineCurveAlias("K-233", SecObjectIdentifiers.SecT233k1);
		DefineCurveAlias("K-283", SecObjectIdentifiers.SecT283k1);
		DefineCurveAlias("K-409", SecObjectIdentifiers.SecT409k1);
		DefineCurveAlias("K-571", SecObjectIdentifiers.SecT571k1);
		DefineCurveAlias("P-192", SecObjectIdentifiers.SecP192r1);
		DefineCurveAlias("P-224", SecObjectIdentifiers.SecP224r1);
		DefineCurveAlias("P-256", SecObjectIdentifiers.SecP256r1);
		DefineCurveAlias("P-384", SecObjectIdentifiers.SecP384r1);
		DefineCurveAlias("P-521", SecObjectIdentifiers.SecP521r1);
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
		return SecNamedCurves.GetByOidLazy(oid);
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
