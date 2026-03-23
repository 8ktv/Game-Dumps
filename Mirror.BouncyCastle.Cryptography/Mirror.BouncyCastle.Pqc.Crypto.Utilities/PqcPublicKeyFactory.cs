using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.BC;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Pqc.Asn1;
using Mirror.BouncyCastle.Pqc.Crypto.Bike;
using Mirror.BouncyCastle.Pqc.Crypto.Cmce;
using Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;
using Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;
using Mirror.BouncyCastle.Pqc.Crypto.Falcon;
using Mirror.BouncyCastle.Pqc.Crypto.Frodo;
using Mirror.BouncyCastle.Pqc.Crypto.Hqc;
using Mirror.BouncyCastle.Pqc.Crypto.Lms;
using Mirror.BouncyCastle.Pqc.Crypto.Picnic;
using Mirror.BouncyCastle.Pqc.Crypto.Saber;
using Mirror.BouncyCastle.Pqc.Crypto.Sike;
using Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Utilities;

public static class PqcPublicKeyFactory
{
	private delegate AsymmetricKeyParameter Converter(SubjectPublicKeyInfo keyInfo, object defaultParams);

	private static Dictionary<DerObjectIdentifier, Converter> Converters;

	static PqcPublicKeyFactory()
	{
		Converters = new Dictionary<DerObjectIdentifier, Converter>();
		Converters[PkcsObjectIdentifiers.IdAlgHssLmsHashsig] = LmsConverter;
		Converters[BCObjectIdentifiers.mceliece348864_r3] = CmceConverter;
		Converters[BCObjectIdentifiers.mceliece348864f_r3] = CmceConverter;
		Converters[BCObjectIdentifiers.mceliece460896_r3] = CmceConverter;
		Converters[BCObjectIdentifiers.mceliece460896f_r3] = CmceConverter;
		Converters[BCObjectIdentifiers.mceliece6688128_r3] = CmceConverter;
		Converters[BCObjectIdentifiers.mceliece6688128f_r3] = CmceConverter;
		Converters[BCObjectIdentifiers.mceliece6960119_r3] = CmceConverter;
		Converters[BCObjectIdentifiers.mceliece6960119f_r3] = CmceConverter;
		Converters[BCObjectIdentifiers.mceliece8192128_r3] = CmceConverter;
		Converters[BCObjectIdentifiers.mceliece8192128f_r3] = CmceConverter;
		Converters[BCObjectIdentifiers.frodokem640aes] = FrodoConverter;
		Converters[BCObjectIdentifiers.frodokem640shake] = FrodoConverter;
		Converters[BCObjectIdentifiers.frodokem976aes] = FrodoConverter;
		Converters[BCObjectIdentifiers.frodokem976shake] = FrodoConverter;
		Converters[BCObjectIdentifiers.frodokem1344aes] = FrodoConverter;
		Converters[BCObjectIdentifiers.frodokem1344shake] = FrodoConverter;
		Converters[BCObjectIdentifiers.lightsaberkem128r3] = SaberConverter;
		Converters[BCObjectIdentifiers.saberkem128r3] = SaberConverter;
		Converters[BCObjectIdentifiers.firesaberkem128r3] = SaberConverter;
		Converters[BCObjectIdentifiers.lightsaberkem192r3] = SaberConverter;
		Converters[BCObjectIdentifiers.saberkem192r3] = SaberConverter;
		Converters[BCObjectIdentifiers.firesaberkem192r3] = SaberConverter;
		Converters[BCObjectIdentifiers.lightsaberkem256r3] = SaberConverter;
		Converters[BCObjectIdentifiers.saberkem256r3] = SaberConverter;
		Converters[BCObjectIdentifiers.firesaberkem256r3] = SaberConverter;
		Converters[BCObjectIdentifiers.ulightsaberkemr3] = SaberConverter;
		Converters[BCObjectIdentifiers.usaberkemr3] = SaberConverter;
		Converters[BCObjectIdentifiers.ufiresaberkemr3] = SaberConverter;
		Converters[BCObjectIdentifiers.lightsaberkem90sr3] = SaberConverter;
		Converters[BCObjectIdentifiers.saberkem90sr3] = SaberConverter;
		Converters[BCObjectIdentifiers.firesaberkem90sr3] = SaberConverter;
		Converters[BCObjectIdentifiers.ulightsaberkem90sr3] = SaberConverter;
		Converters[BCObjectIdentifiers.usaberkem90sr3] = SaberConverter;
		Converters[BCObjectIdentifiers.ufiresaberkem90sr3] = SaberConverter;
		Converters[BCObjectIdentifiers.picnic] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnicl1fs] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnicl1ur] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnicl3fs] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnicl3ur] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnicl5fs] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnicl5ur] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnic3l1] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnic3l3] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnic3l5] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnicl1full] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnicl3full] = PicnicConverter;
		Converters[BCObjectIdentifiers.picnicl5full] = PicnicConverter;
		Converters[BCObjectIdentifiers.sikep434] = SikeConverter;
		Converters[BCObjectIdentifiers.sikep503] = SikeConverter;
		Converters[BCObjectIdentifiers.sikep610] = SikeConverter;
		Converters[BCObjectIdentifiers.sikep751] = SikeConverter;
		Converters[BCObjectIdentifiers.sikep434_compressed] = SikeConverter;
		Converters[BCObjectIdentifiers.sikep503_compressed] = SikeConverter;
		Converters[BCObjectIdentifiers.sikep610_compressed] = SikeConverter;
		Converters[BCObjectIdentifiers.sikep751_compressed] = SikeConverter;
		Converters[BCObjectIdentifiers.dilithium2] = DilithiumConverter;
		Converters[BCObjectIdentifiers.dilithium3] = DilithiumConverter;
		Converters[BCObjectIdentifiers.dilithium5] = DilithiumConverter;
		Converters[BCObjectIdentifiers.dilithium2_aes] = DilithiumConverter;
		Converters[BCObjectIdentifiers.dilithium3_aes] = DilithiumConverter;
		Converters[BCObjectIdentifiers.dilithium5_aes] = DilithiumConverter;
		Converters[BCObjectIdentifiers.falcon_512] = FalconConverter;
		Converters[BCObjectIdentifiers.falcon_1024] = FalconConverter;
		Converters[BCObjectIdentifiers.kyber512] = KyberConverter;
		Converters[BCObjectIdentifiers.kyber512_aes] = KyberConverter;
		Converters[BCObjectIdentifiers.kyber768] = KyberConverter;
		Converters[BCObjectIdentifiers.kyber768_aes] = KyberConverter;
		Converters[BCObjectIdentifiers.kyber1024] = KyberConverter;
		Converters[BCObjectIdentifiers.kyber1024_aes] = KyberConverter;
		Converters[BCObjectIdentifiers.bike128] = BikeConverter;
		Converters[BCObjectIdentifiers.bike192] = BikeConverter;
		Converters[BCObjectIdentifiers.bike256] = BikeConverter;
		Converters[BCObjectIdentifiers.hqc128] = HqcConverter;
		Converters[BCObjectIdentifiers.hqc192] = HqcConverter;
		Converters[BCObjectIdentifiers.hqc256] = HqcConverter;
		Converters[BCObjectIdentifiers.sphincsPlus] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_128s_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_128f_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_128s_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_128f_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_128s_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_128f_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_192s_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_192f_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_192s_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_192f_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_192s_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_192f_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_256s_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_256f_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_256s_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_256f_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_256s_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_256f_r3] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_128f_r3_simple] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_128s_r3_simple] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_192f_r3_simple] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_192s_r3_simple] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_256f_r3_simple] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_haraka_256s_r3_simple] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_128s] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_128f] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_128s] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_128f] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_192s] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_192f] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_192s] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_192f] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_256s] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_sha2_256f] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_256s] = SphincsPlusConverter;
		Converters[BCObjectIdentifiers.sphincsPlus_shake_256f] = SphincsPlusConverter;
	}

	public static AsymmetricKeyParameter CreateKey(byte[] keyInfoData)
	{
		return CreateKey(SubjectPublicKeyInfo.GetInstance(Asn1Object.FromByteArray(keyInfoData)));
	}

	public static AsymmetricKeyParameter CreateKey(Stream inStr)
	{
		return CreateKey(SubjectPublicKeyInfo.GetInstance(new Asn1InputStream(inStr).ReadObject()));
	}

	public static AsymmetricKeyParameter CreateKey(SubjectPublicKeyInfo keyInfo)
	{
		return CreateKey(keyInfo, null);
	}

	public static AsymmetricKeyParameter CreateKey(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		DerObjectIdentifier algorithm = keyInfo.Algorithm.Algorithm;
		if (!Converters.TryGetValue(algorithm, out var value))
		{
			throw new IOException("algorithm identifier in public key not recognised: " + algorithm);
		}
		return value(keyInfo, defaultParams);
	}

	internal static DilithiumPublicKeyParameters GetDilithiumPublicKey(DilithiumParameters dilithiumParameters, DerBitString publicKeyData)
	{
		byte[] octets = publicKeyData.GetOctets();
		try
		{
			Asn1Object asn1Object = Asn1Object.FromByteArray(octets);
			if (asn1Object is Asn1Sequence asn1Sequence)
			{
				return new DilithiumPublicKeyParameters(dilithiumParameters, Asn1OctetString.GetInstance(asn1Sequence[0]).GetOctets(), Asn1OctetString.GetInstance(asn1Sequence[1]).GetOctets());
			}
			byte[] octets2 = Asn1OctetString.GetInstance(asn1Object).GetOctets();
			return new DilithiumPublicKeyParameters(dilithiumParameters, octets2);
		}
		catch (Exception)
		{
			return new DilithiumPublicKeyParameters(dilithiumParameters, octets);
		}
	}

	private static AsymmetricKeyParameter LmsConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		byte[] array = Asn1OctetString.GetInstance(keyInfo.ParsePublicKey()).GetOctets();
		if (Pack.BE_To_UInt32(array, 0) == 1)
		{
			return LmsPublicKeyParameters.GetInstance(Arrays.CopyOfRange(array, 4, array.Length));
		}
		if (array.Length == 64)
		{
			array = Arrays.CopyOfRange(array, 4, array.Length);
		}
		return HssPublicKeyParameters.GetInstance(array);
	}

	private static AsymmetricKeyParameter SphincsPlusConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		try
		{
			byte[] octets = Asn1OctetString.GetInstance(keyInfo.ParsePublicKey()).GetOctets();
			return new SphincsPlusPublicKeyParameters(PqcUtilities.SphincsPlusParamsLookup(keyInfo.Algorithm.Algorithm), Arrays.CopyOfRange(octets, 4, octets.Length));
		}
		catch (Exception)
		{
			byte[] octets2 = keyInfo.PublicKey.GetOctets();
			return new SphincsPlusPublicKeyParameters(PqcUtilities.SphincsPlusParamsLookup(keyInfo.Algorithm.Algorithm), octets2);
		}
	}

	private static AsymmetricKeyParameter CmceConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		byte[] t = CmcePublicKey.GetInstance(keyInfo.ParsePublicKey()).T;
		return new CmcePublicKeyParameters(PqcUtilities.McElieceParamsLookup(keyInfo.Algorithm.Algorithm), t);
	}

	private static AsymmetricKeyParameter FrodoConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		byte[] octets = Asn1OctetString.GetInstance(keyInfo.ParsePublicKey()).GetOctets();
		return new FrodoPublicKeyParameters(PqcUtilities.FrodoParamsLookup(keyInfo.Algorithm.Algorithm), octets);
	}

	private static AsymmetricKeyParameter SaberConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		byte[] octets = Asn1OctetString.GetInstance(Asn1Sequence.GetInstance(keyInfo.ParsePublicKey())[0]).GetOctets();
		return new SaberPublicKeyParameters(PqcUtilities.SaberParamsLookup(keyInfo.Algorithm.Algorithm), octets);
	}

	private static AsymmetricKeyParameter PicnicConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		byte[] octets = Asn1OctetString.GetInstance(keyInfo.ParsePublicKey()).GetOctets();
		return new PicnicPublicKeyParameters(PqcUtilities.PicnicParamsLookup(keyInfo.Algorithm.Algorithm), octets);
	}

	[Obsolete("Will be removed")]
	private static AsymmetricKeyParameter SikeConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		byte[] octets = Asn1OctetString.GetInstance(keyInfo.ParsePublicKey()).GetOctets();
		return new SikePublicKeyParameters(PqcUtilities.SikeParamsLookup(keyInfo.Algorithm.Algorithm), octets);
	}

	private static AsymmetricKeyParameter DilithiumConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		return GetDilithiumPublicKey(PqcUtilities.DilithiumParamsLookup(keyInfo.Algorithm.Algorithm), keyInfo.PublicKey);
	}

	private static AsymmetricKeyParameter KyberConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		KyberParameters parameters = PqcUtilities.KyberParamsLookup(keyInfo.Algorithm.Algorithm);
		try
		{
			KyberPublicKey instance = KyberPublicKey.GetInstance(keyInfo.ParsePublicKey());
			return new KyberPublicKeyParameters(parameters, instance.T, instance.Rho);
		}
		catch (Exception)
		{
			return new KyberPublicKeyParameters(parameters, keyInfo.PublicKey.GetOctets());
		}
	}

	private static AsymmetricKeyParameter FalconConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		FalconParameters falconParameters = PqcUtilities.FalconParamsLookup(keyInfo.Algorithm.Algorithm);
		try
		{
			Asn1Object asn1Object = keyInfo.ParsePublicKey();
			if (asn1Object is Asn1Sequence)
			{
				byte[] octets = Asn1OctetString.GetInstance(Asn1Sequence.GetInstance(asn1Object)[0]).GetOctets();
				return new FalconPublicKeyParameters(falconParameters, octets);
			}
			byte[] octets2 = Asn1OctetString.GetInstance(asn1Object).GetOctets();
			if (octets2[0] != (byte)falconParameters.LogN)
			{
				throw new ArgumentException("byte[] enc of Falcon h value not tagged correctly");
			}
			return new FalconPublicKeyParameters(falconParameters, Arrays.CopyOfRange(octets2, 1, octets2.Length));
		}
		catch (Exception)
		{
			byte[] octets3 = keyInfo.PublicKey.GetOctets();
			if (octets3[0] != (byte)falconParameters.LogN)
			{
				throw new ArgumentException("byte[] enc of Falcon h value not tagged correctly");
			}
			return new FalconPublicKeyParameters(falconParameters, Arrays.CopyOfRange(octets3, 1, octets3.Length));
		}
	}

	private static AsymmetricKeyParameter BikeConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		try
		{
			byte[] octets = Asn1OctetString.GetInstance(keyInfo.ParsePublicKey()).GetOctets();
			return new BikePublicKeyParameters(PqcUtilities.BikeParamsLookup(keyInfo.Algorithm.Algorithm), octets);
		}
		catch (Exception)
		{
			byte[] octets2 = keyInfo.PublicKey.GetOctets();
			return new BikePublicKeyParameters(PqcUtilities.BikeParamsLookup(keyInfo.Algorithm.Algorithm), octets2);
		}
	}

	private static AsymmetricKeyParameter HqcConverter(SubjectPublicKeyInfo keyInfo, object defaultParams)
	{
		try
		{
			byte[] octets = Asn1OctetString.GetInstance(keyInfo.ParsePublicKey()).GetOctets();
			return new HqcPublicKeyParameters(PqcUtilities.HqcParamsLookup(keyInfo.Algorithm.Algorithm), octets);
		}
		catch (Exception)
		{
			byte[] octets2 = keyInfo.PublicKey.GetOctets();
			return new HqcPublicKeyParameters(PqcUtilities.HqcParamsLookup(keyInfo.Algorithm.Algorithm), octets2);
		}
	}
}
