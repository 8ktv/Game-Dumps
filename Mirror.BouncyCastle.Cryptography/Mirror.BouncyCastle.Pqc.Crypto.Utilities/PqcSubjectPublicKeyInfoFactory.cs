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

public static class PqcSubjectPublicKeyInfoFactory
{
	public static SubjectPublicKeyInfo CreateSubjectPublicKeyInfo(AsymmetricKeyParameter publicKey)
	{
		if (publicKey == null)
		{
			throw new ArgumentNullException("publicKey");
		}
		if (publicKey.IsPrivate)
		{
			throw new ArgumentException("Private key passed - public key expected.", "publicKey");
		}
		if (publicKey is LmsPublicKeyParameters encodable)
		{
			byte[] contents = Composer.Compose().U32Str(1).Bytes(encodable)
				.Build();
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgHssLmsHashsig), new DerOctetString(contents));
		}
		if (publicKey is HssPublicKeyParameters { Level: var level } hssPublicKeyParameters)
		{
			byte[] contents2 = Composer.Compose().U32Str(level).Bytes(hssPublicKeyParameters.LmsPublicKey)
				.Build();
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgHssLmsHashsig), new DerOctetString(contents2));
		}
		if (publicKey is SphincsPlusPublicKeyParameters sphincsPlusPublicKeyParameters)
		{
			byte[] encoded = sphincsPlusPublicKeyParameters.GetEncoded();
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.SphincsPlusOidLookup(sphincsPlusPublicKeyParameters.Parameters)), encoded);
		}
		if (publicKey is CmcePublicKeyParameters cmcePublicKeyParameters)
		{
			byte[] encoded2 = cmcePublicKeyParameters.GetEncoded();
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.McElieceOidLookup(cmcePublicKeyParameters.Parameters)), new CmcePublicKey(encoded2));
		}
		if (publicKey is FrodoPublicKeyParameters frodoPublicKeyParameters)
		{
			byte[] encoded3 = frodoPublicKeyParameters.GetEncoded();
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.FrodoOidLookup(frodoPublicKeyParameters.Parameters)), new DerOctetString(encoded3));
		}
		if (publicKey is SaberPublicKeyParameters saberPublicKeyParameters)
		{
			byte[] encoded4 = saberPublicKeyParameters.GetEncoded();
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.SaberOidLookup(saberPublicKeyParameters.Parameters)), new DerSequence(new DerOctetString(encoded4)));
		}
		if (publicKey is PicnicPublicKeyParameters picnicPublicKeyParameters)
		{
			byte[] encoded5 = picnicPublicKeyParameters.GetEncoded();
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.PicnicOidLookup(picnicPublicKeyParameters.Parameters)), new DerOctetString(encoded5));
		}
		if (publicKey is SikePublicKeyParameters sikePublicKeyParameters)
		{
			byte[] encoded6 = sikePublicKeyParameters.GetEncoded();
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.SikeOidLookup(sikePublicKeyParameters.Parameters)), new DerOctetString(encoded6));
		}
		if (publicKey is FalconPublicKeyParameters falconPublicKeyParameters)
		{
			byte[] encoded7 = falconPublicKeyParameters.GetEncoded();
			byte[] array = new byte[encoded7.Length + 1];
			array[0] = (byte)falconPublicKeyParameters.Parameters.LogN;
			Array.Copy(encoded7, 0, array, 1, encoded7.Length);
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.FalconOidLookup(falconPublicKeyParameters.Parameters)), array);
		}
		if (publicKey is KyberPublicKeyParameters kyberPublicKeyParameters)
		{
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.KyberOidLookup(kyberPublicKeyParameters.Parameters)), kyberPublicKeyParameters.GetEncoded());
		}
		if (publicKey is DilithiumPublicKeyParameters dilithiumPublicKeyParameters)
		{
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.DilithiumOidLookup(dilithiumPublicKeyParameters.Parameters)), dilithiumPublicKeyParameters.GetEncoded());
		}
		if (publicKey is BikePublicKeyParameters bikePublicKeyParameters)
		{
			byte[] encoded8 = bikePublicKeyParameters.GetEncoded();
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.BikeOidLookup(bikePublicKeyParameters.Parameters)), encoded8);
		}
		if (publicKey is HqcPublicKeyParameters hqcPublicKeyParameters)
		{
			byte[] encoded9 = hqcPublicKeyParameters.GetEncoded();
			return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PqcUtilities.HqcOidLookup(hqcPublicKeyParameters.Parameters)), encoded9);
		}
		throw new ArgumentException("Class provided no convertible: " + Platform.GetTypeName(publicKey));
	}
}
