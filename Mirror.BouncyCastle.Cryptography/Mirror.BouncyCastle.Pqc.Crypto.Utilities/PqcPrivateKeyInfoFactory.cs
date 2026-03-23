using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
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

public static class PqcPrivateKeyInfoFactory
{
	public static PrivateKeyInfo CreatePrivateKeyInfo(AsymmetricKeyParameter privateKey)
	{
		return CreatePrivateKeyInfo(privateKey, null);
	}

	public static PrivateKeyInfo CreatePrivateKeyInfo(AsymmetricKeyParameter privateKey, Asn1Set attributes)
	{
		if (privateKey is LmsPrivateKeyParameters lmsPrivateKeyParameters)
		{
			byte[] contents = Composer.Compose().U32Str(1).Bytes(lmsPrivateKeyParameters)
				.Build();
			byte[] publicKey = Composer.Compose().U32Str(1).Bytes(lmsPrivateKeyParameters.GetPublicKey())
				.Build();
			return new PrivateKeyInfo(new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgHssLmsHashsig), new DerOctetString(contents), attributes, publicKey);
		}
		if (privateKey is HssPrivateKeyParameters { Level: var level } hssPrivateKeyParameters)
		{
			byte[] contents2 = Composer.Compose().U32Str(level).Bytes(hssPrivateKeyParameters)
				.Build();
			byte[] publicKey2 = Composer.Compose().U32Str(level).Bytes(hssPrivateKeyParameters.GetPublicKey().LmsPublicKey)
				.Build();
			return new PrivateKeyInfo(new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgHssLmsHashsig), new DerOctetString(contents2), attributes, publicKey2);
		}
		if (privateKey is SphincsPlusPrivateKeyParameters sphincsPlusPrivateKeyParameters)
		{
			return new PrivateKeyInfo(new AlgorithmIdentifier(PqcUtilities.SphincsPlusOidLookup(sphincsPlusPrivateKeyParameters.Parameters)), new DerOctetString(sphincsPlusPrivateKeyParameters.GetEncoded()), attributes);
		}
		if (privateKey is CmcePrivateKeyParameters cmcePrivateKeyParameters)
		{
			cmcePrivateKeyParameters.GetEncoded();
			AlgorithmIdentifier privateKeyAlgorithm = new AlgorithmIdentifier(PqcUtilities.McElieceOidLookup(cmcePrivateKeyParameters.Parameters));
			CmcePrivateKey privateKey2 = new CmcePrivateKey(pubKey: new CmcePublicKey(cmcePrivateKeyParameters.ReconstructPublicKey()), version: 0, delta: cmcePrivateKeyParameters.Delta, c: cmcePrivateKeyParameters.C, g: cmcePrivateKeyParameters.G, alpha: cmcePrivateKeyParameters.Alpha, s: cmcePrivateKeyParameters.S);
			return new PrivateKeyInfo(privateKeyAlgorithm, privateKey2, attributes);
		}
		if (privateKey is FrodoPrivateKeyParameters frodoPrivateKeyParameters)
		{
			byte[] encoded = frodoPrivateKeyParameters.GetEncoded();
			return new PrivateKeyInfo(new AlgorithmIdentifier(PqcUtilities.FrodoOidLookup(frodoPrivateKeyParameters.Parameters)), new DerOctetString(encoded), attributes);
		}
		if (privateKey is SaberPrivateKeyParameters saberPrivateKeyParameters)
		{
			byte[] encoded2 = saberPrivateKeyParameters.GetEncoded();
			return new PrivateKeyInfo(new AlgorithmIdentifier(PqcUtilities.SaberOidLookup(saberPrivateKeyParameters.Parameters)), new DerOctetString(encoded2), attributes);
		}
		if (privateKey is PicnicPrivateKeyParameters picnicPrivateKeyParameters)
		{
			byte[] encoded3 = picnicPrivateKeyParameters.GetEncoded();
			return new PrivateKeyInfo(new AlgorithmIdentifier(PqcUtilities.PicnicOidLookup(picnicPrivateKeyParameters.Parameters)), new DerOctetString(encoded3), attributes);
		}
		if (privateKey is SikePrivateKeyParameters sikePrivateKeyParameters)
		{
			byte[] encoded4 = sikePrivateKeyParameters.GetEncoded();
			return new PrivateKeyInfo(new AlgorithmIdentifier(PqcUtilities.SikeOidLookup(sikePrivateKeyParameters.Parameters)), new DerOctetString(encoded4), attributes);
		}
		if (privateKey is FalconPrivateKeyParameters falconPrivateKeyParameters)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(4);
			asn1EncodableVector.Add(new DerInteger(1));
			asn1EncodableVector.Add(new DerOctetString(falconPrivateKeyParameters.GetSpolyLittleF()));
			asn1EncodableVector.Add(new DerOctetString(falconPrivateKeyParameters.GetG()));
			asn1EncodableVector.Add(new DerOctetString(falconPrivateKeyParameters.GetSpolyBigF()));
			return new PrivateKeyInfo(new AlgorithmIdentifier(PqcUtilities.FalconOidLookup(falconPrivateKeyParameters.Parameters)), new DerSequence(asn1EncodableVector), attributes, falconPrivateKeyParameters.GetPublicKey());
		}
		if (privateKey is KyberPrivateKeyParameters kyberPrivateKeyParameters)
		{
			return new PrivateKeyInfo(new AlgorithmIdentifier(PqcUtilities.KyberOidLookup(kyberPrivateKeyParameters.Parameters)), new DerOctetString(kyberPrivateKeyParameters.GetEncoded()), attributes);
		}
		if (privateKey is DilithiumPrivateKeyParameters dilithiumPrivateKeyParameters)
		{
			return new PrivateKeyInfo(new AlgorithmIdentifier(PqcUtilities.DilithiumOidLookup(dilithiumPrivateKeyParameters.Parameters)), publicKey: dilithiumPrivateKeyParameters.GetPublicKeyParameters().GetEncoded(), privateKey: new DerOctetString(dilithiumPrivateKeyParameters.GetEncoded()), attributes: attributes);
		}
		if (privateKey is BikePrivateKeyParameters bikePrivateKeyParameters)
		{
			byte[] encoded5 = bikePrivateKeyParameters.GetEncoded();
			return new PrivateKeyInfo(new AlgorithmIdentifier(PqcUtilities.BikeOidLookup(bikePrivateKeyParameters.Parameters)), new DerOctetString(encoded5), attributes);
		}
		if (privateKey is HqcPrivateKeyParameters hqcPrivateKeyParameters)
		{
			AlgorithmIdentifier privateKeyAlgorithm2 = new AlgorithmIdentifier(PqcUtilities.HqcOidLookup(hqcPrivateKeyParameters.Parameters));
			byte[] privateKey3 = hqcPrivateKeyParameters.PrivateKey;
			return new PrivateKeyInfo(privateKeyAlgorithm2, new DerOctetString(privateKey3), attributes);
		}
		throw new ArgumentException("Class provided is not convertible: " + Platform.GetTypeName(privateKey));
	}
}
