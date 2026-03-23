using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.Misc;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Smime;

public class SmimeCapabilities : Asn1Encodable
{
	public static readonly DerObjectIdentifier PreferSignedData = PkcsObjectIdentifiers.PreferSignedData;

	public static readonly DerObjectIdentifier CannotDecryptAny = PkcsObjectIdentifiers.CannotDecryptAny;

	public static readonly DerObjectIdentifier SmimeCapabilitesVersions = PkcsObjectIdentifiers.SmimeCapabilitiesVersions;

	public static readonly DerObjectIdentifier Aes256Cbc = NistObjectIdentifiers.IdAes256Cbc;

	public static readonly DerObjectIdentifier Aes192Cbc = NistObjectIdentifiers.IdAes192Cbc;

	public static readonly DerObjectIdentifier Aes128Cbc = NistObjectIdentifiers.IdAes128Cbc;

	public static readonly DerObjectIdentifier IdeaCbc = MiscObjectIdentifiers.as_sys_sec_alg_ideaCBC;

	public static readonly DerObjectIdentifier Cast5Cbc = MiscObjectIdentifiers.cast5CBC;

	public static readonly DerObjectIdentifier DesCbc = OiwObjectIdentifiers.DesCbc;

	public static readonly DerObjectIdentifier DesEde3Cbc = PkcsObjectIdentifiers.DesEde3Cbc;

	public static readonly DerObjectIdentifier RC2Cbc = PkcsObjectIdentifiers.RC2Cbc;

	private Asn1Sequence capabilities;

	public static SmimeCapabilities GetInstance(object obj)
	{
		if (obj == null || obj is SmimeCapabilities)
		{
			return (SmimeCapabilities)obj;
		}
		if (obj is Asn1Sequence)
		{
			return new SmimeCapabilities((Asn1Sequence)obj);
		}
		if (obj is AttributeX509)
		{
			return new SmimeCapabilities((Asn1Sequence)((AttributeX509)obj).AttrValues[0]);
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	public SmimeCapabilities(Asn1Sequence seq)
	{
		capabilities = seq;
	}

	public IList<SmimeCapability> GetCapabilitiesForOid(DerObjectIdentifier capability)
	{
		List<SmimeCapability> list = new List<SmimeCapability>();
		DoGetCapabilitiesForOid(capability, list);
		return list;
	}

	private void DoGetCapabilitiesForOid(DerObjectIdentifier capability, IList<SmimeCapability> list)
	{
		foreach (Asn1Encodable capability2 in capabilities)
		{
			SmimeCapability instance = SmimeCapability.GetInstance(capability2);
			if (capability == null || capability.Equals(instance.CapabilityID))
			{
				list.Add(instance);
			}
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		return capabilities;
	}
}
