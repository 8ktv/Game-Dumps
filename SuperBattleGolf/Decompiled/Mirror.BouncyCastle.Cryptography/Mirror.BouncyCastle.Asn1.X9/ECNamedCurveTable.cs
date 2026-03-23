using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.Anssi;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.GM;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Sec;
using Mirror.BouncyCastle.Asn1.TeleTrust;

namespace Mirror.BouncyCastle.Asn1.X9;

public class ECNamedCurveTable
{
	public static IEnumerable<string> Names
	{
		get
		{
			List<string> list = new List<string>();
			list.AddRange(X962NamedCurves.Names);
			list.AddRange(SecNamedCurves.Names);
			list.AddRange(NistNamedCurves.Names);
			list.AddRange(TeleTrusTNamedCurves.Names);
			list.AddRange(AnssiNamedCurves.Names);
			list.AddRange(ECGost3410NamedCurves.Names);
			list.AddRange(GMNamedCurves.Names);
			return list;
		}
	}

	public static X9ECParameters GetByName(string name)
	{
		X9ECParameters byName = X962NamedCurves.GetByName(name);
		if (byName == null)
		{
			byName = SecNamedCurves.GetByName(name);
		}
		if (byName == null)
		{
			byName = NistNamedCurves.GetByName(name);
		}
		if (byName == null)
		{
			byName = TeleTrusTNamedCurves.GetByName(name);
		}
		if (byName == null)
		{
			byName = AnssiNamedCurves.GetByName(name);
		}
		if (byName == null)
		{
			byName = ECGost3410NamedCurves.GetByName(name);
		}
		if (byName == null)
		{
			byName = GMNamedCurves.GetByName(name);
		}
		return byName;
	}

	public static X9ECParametersHolder GetByNameLazy(string name)
	{
		X9ECParametersHolder byNameLazy = X962NamedCurves.GetByNameLazy(name);
		if (byNameLazy == null)
		{
			byNameLazy = SecNamedCurves.GetByNameLazy(name);
		}
		if (byNameLazy == null)
		{
			byNameLazy = NistNamedCurves.GetByNameLazy(name);
		}
		if (byNameLazy == null)
		{
			byNameLazy = TeleTrusTNamedCurves.GetByNameLazy(name);
		}
		if (byNameLazy == null)
		{
			byNameLazy = AnssiNamedCurves.GetByNameLazy(name);
		}
		if (byNameLazy == null)
		{
			byNameLazy = ECGost3410NamedCurves.GetByNameLazy(name);
		}
		if (byNameLazy == null)
		{
			byNameLazy = GMNamedCurves.GetByNameLazy(name);
		}
		return byNameLazy;
	}

	public static X9ECParameters GetByOid(DerObjectIdentifier oid)
	{
		X9ECParameters byOid = X962NamedCurves.GetByOid(oid);
		if (byOid == null)
		{
			byOid = SecNamedCurves.GetByOid(oid);
		}
		if (byOid == null)
		{
			byOid = TeleTrusTNamedCurves.GetByOid(oid);
		}
		if (byOid == null)
		{
			byOid = AnssiNamedCurves.GetByOid(oid);
		}
		if (byOid == null)
		{
			byOid = ECGost3410NamedCurves.GetByOid(oid);
		}
		if (byOid == null)
		{
			byOid = GMNamedCurves.GetByOid(oid);
		}
		return byOid;
	}

	public static X9ECParametersHolder GetByOidLazy(DerObjectIdentifier oid)
	{
		X9ECParametersHolder byOidLazy = X962NamedCurves.GetByOidLazy(oid);
		if (byOidLazy == null)
		{
			byOidLazy = SecNamedCurves.GetByOidLazy(oid);
		}
		if (byOidLazy == null)
		{
			byOidLazy = TeleTrusTNamedCurves.GetByOidLazy(oid);
		}
		if (byOidLazy == null)
		{
			byOidLazy = AnssiNamedCurves.GetByOidLazy(oid);
		}
		if (byOidLazy == null)
		{
			byOidLazy = ECGost3410NamedCurves.GetByOidLazy(oid);
		}
		if (byOidLazy == null)
		{
			byOidLazy = GMNamedCurves.GetByOidLazy(oid);
		}
		return byOidLazy;
	}

	public static string GetName(DerObjectIdentifier oid)
	{
		string name = X962NamedCurves.GetName(oid);
		if (name == null)
		{
			name = SecNamedCurves.GetName(oid);
		}
		if (name == null)
		{
			name = NistNamedCurves.GetName(oid);
		}
		if (name == null)
		{
			name = TeleTrusTNamedCurves.GetName(oid);
		}
		if (name == null)
		{
			name = AnssiNamedCurves.GetName(oid);
		}
		if (name == null)
		{
			name = ECGost3410NamedCurves.GetName(oid);
		}
		if (name == null)
		{
			name = GMNamedCurves.GetName(oid);
		}
		return name;
	}

	public static DerObjectIdentifier GetOid(string name)
	{
		DerObjectIdentifier oid = X962NamedCurves.GetOid(name);
		if (oid == null)
		{
			oid = SecNamedCurves.GetOid(name);
		}
		if (oid == null)
		{
			oid = NistNamedCurves.GetOid(name);
		}
		if (oid == null)
		{
			oid = TeleTrusTNamedCurves.GetOid(name);
		}
		if (oid == null)
		{
			oid = AnssiNamedCurves.GetOid(name);
		}
		if (oid == null)
		{
			oid = ECGost3410NamedCurves.GetOid(name);
		}
		if (oid == null)
		{
			oid = GMNamedCurves.GetOid(name);
		}
		return oid;
	}
}
