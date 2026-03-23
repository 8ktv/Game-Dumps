using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Kisa;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Nsri;
using Mirror.BouncyCastle.Asn1.Ntt;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Security;

public static class WrapperUtilities
{
	private enum WrapAlgorithm
	{
		AESRFC3211WRAP,
		AESWRAP,
		AESWRAPPAD,
		ARIARFC3211WRAP,
		ARIAWRAP,
		ARIAWRAPPAD,
		CAMELLIARFC3211WRAP,
		CAMELLIAWRAP,
		DESRFC3211WRAP,
		DESEDERFC3211WRAP,
		DESEDEWRAP,
		RC2WRAP,
		SEEDWRAP
	}

	private class BufferedCipherWrapper : IWrapper
	{
		private readonly IBufferedCipher cipher;

		private bool forWrapping;

		public string AlgorithmName => cipher.AlgorithmName;

		public BufferedCipherWrapper(IBufferedCipher cipher)
		{
			this.cipher = cipher;
		}

		public void Init(bool forWrapping, ICipherParameters parameters)
		{
			this.forWrapping = forWrapping;
			cipher.Init(forWrapping, parameters);
		}

		public byte[] Wrap(byte[] input, int inOff, int length)
		{
			if (!forWrapping)
			{
				throw new InvalidOperationException("Not initialised for wrapping");
			}
			return cipher.DoFinal(input, inOff, length);
		}

		public byte[] Unwrap(byte[] input, int inOff, int length)
		{
			if (forWrapping)
			{
				throw new InvalidOperationException("Not initialised for unwrapping");
			}
			return cipher.DoFinal(input, inOff, length);
		}
	}

	private static readonly IDictionary<string, string> Algorithms;

	static WrapperUtilities()
	{
		Algorithms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		Enums.GetArbitraryValue<WrapAlgorithm>().ToString();
		Algorithms["AESKW"] = "AESWRAP";
		Algorithms[NistObjectIdentifiers.IdAes128Wrap.Id] = "AESWRAP";
		Algorithms[NistObjectIdentifiers.IdAes192Wrap.Id] = "AESWRAP";
		Algorithms[NistObjectIdentifiers.IdAes256Wrap.Id] = "AESWRAP";
		Algorithms["AESKWP"] = "AESWRAPPAD";
		Algorithms[NistObjectIdentifiers.IdAes128WrapPad.Id] = "AESWRAPPAD";
		Algorithms[NistObjectIdentifiers.IdAes192WrapPad.Id] = "AESWRAPPAD";
		Algorithms[NistObjectIdentifiers.IdAes256WrapPad.Id] = "AESWRAPPAD";
		Algorithms["AESRFC5649WRAP"] = "AESWRAPPAD";
		Algorithms["ARIAKW"] = "ARIAWRAP";
		Algorithms[NsriObjectIdentifiers.id_aria128_kw.Id] = "ARIAWRAP";
		Algorithms[NsriObjectIdentifiers.id_aria192_kw.Id] = "ARIAWRAP";
		Algorithms[NsriObjectIdentifiers.id_aria256_kw.Id] = "ARIAWRAP";
		Algorithms["ARIAKWP"] = "ARIAWRAPPAD";
		Algorithms[NsriObjectIdentifiers.id_aria128_kwp.Id] = "ARIAWRAPPAD";
		Algorithms[NsriObjectIdentifiers.id_aria192_kwp.Id] = "ARIAWRAPPAD";
		Algorithms[NsriObjectIdentifiers.id_aria256_kwp.Id] = "ARIAWRAPPAD";
		Algorithms[NttObjectIdentifiers.IdCamellia128Wrap.Id] = "CAMELLIAWRAP";
		Algorithms[NttObjectIdentifiers.IdCamellia192Wrap.Id] = "CAMELLIAWRAP";
		Algorithms[NttObjectIdentifiers.IdCamellia256Wrap.Id] = "CAMELLIAWRAP";
		Algorithms["DESEDERFC3217WRAP"] = "DESEDEWRAP";
		Algorithms["TDEAWRAP"] = "DESEDEWRAP";
		Algorithms[PkcsObjectIdentifiers.IdAlgCms3DesWrap.Id] = "DESEDEWRAP";
		Algorithms[PkcsObjectIdentifiers.IdAlgCmsRC2Wrap.Id] = "RC2WRAP";
		Algorithms["SEEDKW"] = "SEEDWRAP";
		Algorithms[KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap.Id] = "SEEDWRAP";
	}

	public static IWrapper GetWrapper(DerObjectIdentifier oid)
	{
		return GetWrapper(oid.Id);
	}

	public static IWrapper GetWrapper(string algorithm)
	{
		if (Enums.TryGetEnumValue<WrapAlgorithm>(CollectionUtilities.GetValueOrKey(Algorithms, algorithm).ToUpperInvariant(), out var result))
		{
			return result switch
			{
				WrapAlgorithm.AESRFC3211WRAP => new Rfc3211WrapEngine(AesUtilities.CreateEngine()), 
				WrapAlgorithm.AESWRAP => new AesWrapEngine(), 
				WrapAlgorithm.AESWRAPPAD => new AesWrapPadEngine(), 
				WrapAlgorithm.ARIARFC3211WRAP => new Rfc3211WrapEngine(new AriaEngine()), 
				WrapAlgorithm.ARIAWRAP => new AriaWrapEngine(), 
				WrapAlgorithm.ARIAWRAPPAD => new AriaWrapPadEngine(), 
				WrapAlgorithm.CAMELLIARFC3211WRAP => new Rfc3211WrapEngine(new CamelliaEngine()), 
				WrapAlgorithm.CAMELLIAWRAP => new CamelliaWrapEngine(), 
				WrapAlgorithm.DESRFC3211WRAP => new Rfc3211WrapEngine(new DesEngine()), 
				WrapAlgorithm.DESEDERFC3211WRAP => new Rfc3211WrapEngine(new DesEdeEngine()), 
				WrapAlgorithm.DESEDEWRAP => new DesEdeWrapEngine(), 
				WrapAlgorithm.RC2WRAP => new RC2WrapEngine(), 
				WrapAlgorithm.SEEDWRAP => new SeedWrapEngine(), 
				_ => throw new NotImplementedException(), 
			};
		}
		IBufferedCipher cipher = CipherUtilities.GetCipher(algorithm);
		if (cipher != null)
		{
			return new BufferedCipherWrapper(cipher);
		}
		throw new SecurityUtilityException("Wrapper " + algorithm + " not recognised.");
	}

	public static string GetAlgorithmName(DerObjectIdentifier oid)
	{
		return CollectionUtilities.GetValueOrNull(Algorithms, oid.Id);
	}
}
