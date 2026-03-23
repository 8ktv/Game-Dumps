using System;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.BC;
using Mirror.BouncyCastle.Asn1.Pkcs;
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

public static class PqcPrivateKeyFactory
{
	public static AsymmetricKeyParameter CreateKey(byte[] privateKeyInfoData)
	{
		return CreateKey(PrivateKeyInfo.GetInstance(Asn1Object.FromByteArray(privateKeyInfoData)));
	}

	public static AsymmetricKeyParameter CreateKey(Stream inStr)
	{
		return CreateKey(PrivateKeyInfo.GetInstance(new Asn1InputStream(inStr).ReadObject()));
	}

	public static AsymmetricKeyParameter CreateKey(PrivateKeyInfo keyInfo)
	{
		DerObjectIdentifier algorithm = keyInfo.PrivateKeyAlgorithm.Algorithm;
		if (algorithm.Equals(PkcsObjectIdentifiers.IdAlgHssLmsHashsig))
		{
			byte[] octets = Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey()).GetOctets();
			DerBitString publicKey = keyInfo.PublicKey;
			if (Pack.BE_To_UInt32(octets, 0) == 1)
			{
				if (publicKey != null)
				{
					byte[] octets2 = publicKey.GetOctets();
					return LmsPrivateKeyParameters.GetInstance(Arrays.CopyOfRange(octets, 4, octets.Length), Arrays.CopyOfRange(octets2, 4, octets2.Length));
				}
				return LmsPrivateKeyParameters.GetInstance(Arrays.CopyOfRange(octets, 4, octets.Length));
			}
		}
		if (algorithm.On(BCObjectIdentifiers.pqc_kem_mceliece))
		{
			CmcePrivateKey instance = CmcePrivateKey.GetInstance(keyInfo.ParsePrivateKey());
			return new CmcePrivateKeyParameters(PqcUtilities.McElieceParamsLookup(algorithm), instance.Delta, instance.C, instance.G, instance.Alpha, instance.S);
		}
		if (algorithm.On(BCObjectIdentifiers.pqc_kem_frodo))
		{
			byte[] octets3 = Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey()).GetOctets();
			return new FrodoPrivateKeyParameters(PqcUtilities.FrodoParamsLookup(algorithm), octets3);
		}
		if (algorithm.On(BCObjectIdentifiers.sphincsPlus) || algorithm.On(BCObjectIdentifiers.sphincsPlus_interop))
		{
			Asn1Encodable asn1Encodable = keyInfo.ParsePrivateKey();
			SphincsPlusParameters parameters = PqcUtilities.SphincsPlusParamsLookup(algorithm);
			if (asn1Encodable is Asn1Sequence obj)
			{
				SphincsPlusPrivateKey instance2 = SphincsPlusPrivateKey.GetInstance(obj);
				SphincsPlusPublicKey publicKey2 = instance2.PublicKey;
				return new SphincsPlusPrivateKeyParameters(parameters, instance2.GetSkseed(), instance2.GetSkprf(), publicKey2.GetPkseed(), publicKey2.GetPkroot());
			}
			Asn1OctetString instance3 = Asn1OctetString.GetInstance(asn1Encodable);
			return new SphincsPlusPrivateKeyParameters(parameters, instance3.GetOctets());
		}
		if (algorithm.On(BCObjectIdentifiers.pqc_kem_saber))
		{
			byte[] octets4 = Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey()).GetOctets();
			return new SaberPrivateKeyParameters(PqcUtilities.SaberParamsLookup(algorithm), octets4);
		}
		if (algorithm.On(BCObjectIdentifiers.picnic))
		{
			byte[] octets5 = Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey()).GetOctets();
			return new PicnicPrivateKeyParameters(PqcUtilities.PicnicParamsLookup(algorithm), octets5);
		}
		if (algorithm.On(BCObjectIdentifiers.pqc_kem_sike))
		{
			byte[] octets6 = Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey()).GetOctets();
			return new SikePrivateKeyParameters(PqcUtilities.SikeParamsLookup(algorithm), octets6);
		}
		if (algorithm.On(BCObjectIdentifiers.pqc_kem_bike))
		{
			byte[] octets7 = Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey()).GetOctets();
			BikeParameters bikeParameters = PqcUtilities.BikeParamsLookup(algorithm);
			byte[] h = Arrays.CopyOfRange(octets7, 0, bikeParameters.RByte);
			byte[] h2 = Arrays.CopyOfRange(octets7, bikeParameters.RByte, 2 * bikeParameters.RByte);
			byte[] sigma = Arrays.CopyOfRange(octets7, 2 * bikeParameters.RByte, octets7.Length);
			return new BikePrivateKeyParameters(bikeParameters, h, h2, sigma);
		}
		if (algorithm.On(BCObjectIdentifiers.pqc_kem_hqc))
		{
			byte[] octets8 = Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey()).GetOctets();
			return new HqcPrivateKeyParameters(PqcUtilities.HqcParamsLookup(algorithm), octets8);
		}
		if (algorithm.On(BCObjectIdentifiers.pqc_kem_kyber))
		{
			Asn1OctetString instance4 = Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey());
			return new KyberPrivateKeyParameters(PqcUtilities.KyberParamsLookup(algorithm), instance4.GetOctets());
		}
		if (algorithm.Equals(BCObjectIdentifiers.dilithium2) || algorithm.Equals(BCObjectIdentifiers.dilithium3) || algorithm.Equals(BCObjectIdentifiers.dilithium5))
		{
			Asn1OctetString instance5 = Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey());
			DilithiumParameters dilithiumParameters = PqcUtilities.DilithiumParamsLookup(algorithm);
			DilithiumPublicKeyParameters pubKey = null;
			DerBitString publicKey3 = keyInfo.PublicKey;
			if (publicKey3 != null)
			{
				pubKey = PqcPublicKeyFactory.GetDilithiumPublicKey(dilithiumParameters, publicKey3);
			}
			return new DilithiumPrivateKeyParameters(dilithiumParameters, instance5.GetOctets(), pubKey);
		}
		if (algorithm.Equals(BCObjectIdentifiers.falcon_512) || algorithm.Equals(BCObjectIdentifiers.falcon_1024))
		{
			Asn1Sequence instance6 = Asn1Sequence.GetInstance(keyInfo.ParsePrivateKey());
			FalconParameters parameters2 = PqcUtilities.FalconParamsLookup(algorithm);
			int intValueExact = DerInteger.GetInstance(instance6[0]).IntValueExact;
			if (intValueExact != 1)
			{
				throw new IOException("unknown private key version: " + intValueExact);
			}
			return new FalconPrivateKeyParameters(parameters2, Asn1OctetString.GetInstance(instance6[1]).GetOctets(), Asn1OctetString.GetInstance(instance6[2]).GetOctets(), Asn1OctetString.GetInstance(instance6[3]).GetOctets(), keyInfo.PublicKey?.GetOctets());
		}
		throw new Exception("algorithm identifier in private key not recognised");
	}
}
