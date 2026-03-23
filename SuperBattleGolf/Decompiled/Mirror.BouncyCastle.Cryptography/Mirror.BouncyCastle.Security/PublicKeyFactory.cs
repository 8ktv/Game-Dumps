using System;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cryptlib;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.EdEC;
using Mirror.BouncyCastle.Asn1.Gnu;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;

namespace Mirror.BouncyCastle.Security;

public static class PublicKeyFactory
{
	public static AsymmetricKeyParameter CreateKey(byte[] keyInfoData)
	{
		return CreateKey(SubjectPublicKeyInfo.GetInstance(Asn1Object.FromByteArray(keyInfoData)));
	}

	public static AsymmetricKeyParameter CreateKey(Stream inStr)
	{
		return CreateKey(SubjectPublicKeyInfo.GetInstance(Asn1Object.FromStream(inStr)));
	}

	public static AsymmetricKeyParameter CreateKey(SubjectPublicKeyInfo keyInfo)
	{
		AlgorithmIdentifier algorithm = keyInfo.Algorithm;
		DerObjectIdentifier algorithm2 = algorithm.Algorithm;
		if (algorithm2.Equals(PkcsObjectIdentifiers.RsaEncryption) || algorithm2.Equals(X509ObjectIdentifiers.IdEARsa) || algorithm2.Equals(PkcsObjectIdentifiers.IdRsassaPss) || algorithm2.Equals(PkcsObjectIdentifiers.IdRsaesOaep))
		{
			RsaPublicKeyStructure instance = RsaPublicKeyStructure.GetInstance(keyInfo.ParsePublicKey());
			return new RsaKeyParameters(isPrivate: false, instance.Modulus, instance.PublicExponent);
		}
		if (algorithm2.Equals(X9ObjectIdentifiers.DHPublicNumber))
		{
			Asn1Sequence instance2 = Asn1Sequence.GetInstance(algorithm.Parameters.ToAsn1Object());
			BigInteger value = DHPublicKey.GetInstance(keyInfo.ParsePublicKey()).Y.Value;
			if (IsPkcsDHParam(instance2))
			{
				return ReadPkcsDHParam(algorithm2, value, instance2);
			}
			DHDomainParameters instance3 = DHDomainParameters.GetInstance(instance2);
			BigInteger value2 = instance3.P.Value;
			BigInteger value3 = instance3.G.Value;
			BigInteger value4 = instance3.Q.Value;
			BigInteger j = null;
			if (instance3.J != null)
			{
				j = instance3.J.Value;
			}
			DHValidationParameters validation = null;
			DHValidationParms validationParms = instance3.ValidationParms;
			if (validationParms != null)
			{
				byte[] bytes = validationParms.Seed.GetBytes();
				BigInteger value5 = validationParms.PgenCounter.Value;
				validation = new DHValidationParameters(bytes, value5.IntValue);
			}
			return new DHPublicKeyParameters(value, new DHParameters(value2, value3, value4, j, validation));
		}
		if (algorithm2.Equals(PkcsObjectIdentifiers.DhKeyAgreement))
		{
			Asn1Sequence instance4 = Asn1Sequence.GetInstance(algorithm.Parameters.ToAsn1Object());
			DerInteger derInteger = (DerInteger)keyInfo.ParsePublicKey();
			return ReadPkcsDHParam(algorithm2, derInteger.Value, instance4);
		}
		if (algorithm2.Equals(OiwObjectIdentifiers.ElGamalAlgorithm))
		{
			ElGamalParameter elGamalParameter = new ElGamalParameter(Asn1Sequence.GetInstance(algorithm.Parameters.ToAsn1Object()));
			return new ElGamalPublicKeyParameters(((DerInteger)keyInfo.ParsePublicKey()).Value, new ElGamalParameters(elGamalParameter.P, elGamalParameter.G));
		}
		if (algorithm2.Equals(X9ObjectIdentifiers.IdDsa) || algorithm2.Equals(OiwObjectIdentifiers.DsaWithSha1))
		{
			DerInteger obj = (DerInteger)keyInfo.ParsePublicKey();
			Asn1Encodable parameters = algorithm.Parameters;
			DsaParameters parameters2 = null;
			if (parameters != null)
			{
				DsaParameter instance5 = DsaParameter.GetInstance(parameters.ToAsn1Object());
				parameters2 = new DsaParameters(instance5.P, instance5.Q, instance5.G);
			}
			return new DsaPublicKeyParameters(obj.Value, parameters2);
		}
		if (algorithm2.Equals(X9ObjectIdentifiers.IdECPublicKey))
		{
			X962Parameters instance6 = X962Parameters.GetInstance(algorithm.Parameters.ToAsn1Object());
			X9ECParameters x9ECParameters = ((!instance6.IsNamedCurve) ? new X9ECParameters((Asn1Sequence)instance6.Parameters) : ECKeyPairGenerator.FindECCurveByOid((DerObjectIdentifier)instance6.Parameters));
			Asn1OctetString s = new DerOctetString(keyInfo.PublicKey.GetBytes());
			ECPoint point = new X9ECPoint(x9ECParameters.Curve, s).Point;
			if (instance6.IsNamedCurve)
			{
				return new ECPublicKeyParameters("EC", point, (DerObjectIdentifier)instance6.Parameters);
			}
			ECDomainParameters parameters3 = new ECDomainParameters(x9ECParameters);
			return new ECPublicKeyParameters(point, parameters3);
		}
		if (algorithm2.Equals(CryptoProObjectIdentifiers.GostR3410x2001))
		{
			DerObjectIdentifier publicKeyParamSet = Gost3410PublicKeyAlgParameters.GetInstance(algorithm.Parameters).PublicKeyParamSet;
			X9ECParameters byOid = ECGost3410NamedCurves.GetByOid(publicKeyParamSet);
			if (byOid == null)
			{
				return null;
			}
			Asn1OctetString asn1OctetString;
			try
			{
				asn1OctetString = (Asn1OctetString)keyInfo.ParsePublicKey();
			}
			catch (IOException innerException)
			{
				throw new ArgumentException("error recovering GOST3410_2001 public key", innerException);
			}
			int num = 32;
			int num2 = 2 * num;
			byte[] octets = asn1OctetString.GetOctets();
			if (octets.Length != num2)
			{
				throw new ArgumentException("invalid length for GOST3410_2001 public key");
			}
			byte[] array = new byte[1 + num2];
			array[0] = 4;
			for (int i = 1; i <= num; i++)
			{
				array[i] = octets[num - i];
				array[i + num] = octets[num2 - i];
			}
			ECPoint q = byOid.Curve.DecodePoint(array);
			return new ECPublicKeyParameters("ECGOST3410", q, publicKeyParamSet);
		}
		if (algorithm2.Equals(CryptoProObjectIdentifiers.GostR3410x94))
		{
			Gost3410PublicKeyAlgParameters instance7 = Gost3410PublicKeyAlgParameters.GetInstance(algorithm.Parameters);
			Asn1OctetString asn1OctetString2;
			try
			{
				asn1OctetString2 = (Asn1OctetString)keyInfo.ParsePublicKey();
			}
			catch (IOException innerException2)
			{
				throw new ArgumentException("error recovering GOST3410_94 public key", innerException2);
			}
			byte[] octets2 = asn1OctetString2.GetOctets();
			return new Gost3410PublicKeyParameters(new BigInteger(1, octets2, bigEndian: false), instance7.PublicKeyParamSet);
		}
		if (algorithm2.Equals(EdECObjectIdentifiers.id_X25519) || algorithm2.Equals(CryptlibObjectIdentifiers.curvey25519))
		{
			return new X25519PublicKeyParameters(GetRawKey(keyInfo));
		}
		if (algorithm2.Equals(EdECObjectIdentifiers.id_X448))
		{
			return new X448PublicKeyParameters(GetRawKey(keyInfo));
		}
		if (algorithm2.Equals(EdECObjectIdentifiers.id_Ed25519) || algorithm2.Equals(GnuObjectIdentifiers.Ed25519))
		{
			return new Ed25519PublicKeyParameters(GetRawKey(keyInfo));
		}
		if (algorithm2.Equals(EdECObjectIdentifiers.id_Ed448))
		{
			return new Ed448PublicKeyParameters(GetRawKey(keyInfo));
		}
		if (algorithm2.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_256) || algorithm2.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512) || algorithm2.Equals(RosstandartObjectIdentifiers.id_tc26_agreement_gost_3410_12_256) || algorithm2.Equals(RosstandartObjectIdentifiers.id_tc26_agreement_gost_3410_12_512))
		{
			Gost3410PublicKeyAlgParameters instance8 = Gost3410PublicKeyAlgParameters.GetInstance(algorithm.Parameters);
			DerObjectIdentifier publicKeyParamSet2 = instance8.PublicKeyParamSet;
			ECGost3410Parameters eCGost3410Parameters = new ECGost3410Parameters(new ECNamedDomainParameters(publicKeyParamSet2, ECGost3410NamedCurves.GetByOid(publicKeyParamSet2)), publicKeyParamSet2, instance8.DigestParamSet, instance8.EncryptionParamSet);
			Asn1OctetString asn1OctetString3;
			try
			{
				asn1OctetString3 = (Asn1OctetString)keyInfo.ParsePublicKey();
			}
			catch (IOException innerException3)
			{
				throw new ArgumentException("error recovering GOST3410_2012 public key", innerException3);
			}
			int num3 = 32;
			if (algorithm2.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512))
			{
				num3 = 64;
			}
			int num4 = 2 * num3;
			byte[] octets3 = asn1OctetString3.GetOctets();
			if (octets3.Length != num4)
			{
				throw new ArgumentException("invalid length for GOST3410_2012 public key");
			}
			byte[] array2 = new byte[1 + num4];
			array2[0] = 4;
			for (int k = 1; k <= num3; k++)
			{
				array2[k] = octets3[num3 - k];
				array2[k + num3] = octets3[num4 - k];
			}
			return new ECPublicKeyParameters(eCGost3410Parameters.Curve.DecodePoint(array2), eCGost3410Parameters);
		}
		throw new SecurityUtilityException("algorithm identifier in public key not recognised: " + algorithm2);
	}

	private static byte[] GetRawKey(SubjectPublicKeyInfo keyInfo)
	{
		return keyInfo.PublicKey.GetOctets();
	}

	private static bool IsPkcsDHParam(Asn1Sequence seq)
	{
		if (seq.Count == 2)
		{
			return true;
		}
		if (seq.Count > 3)
		{
			return false;
		}
		DerInteger instance = DerInteger.GetInstance(seq[2]);
		DerInteger instance2 = DerInteger.GetInstance(seq[0]);
		return instance.Value.CompareTo(BigInteger.ValueOf(instance2.Value.BitLength)) <= 0;
	}

	private static DHPublicKeyParameters ReadPkcsDHParam(DerObjectIdentifier algOid, BigInteger y, Asn1Sequence seq)
	{
		DHParameter dHParameter = new DHParameter(seq);
		int l = dHParameter.L?.IntValue ?? 0;
		DHParameters parameters = new DHParameters(dHParameter.P, dHParameter.G, null, l);
		return new DHPublicKeyParameters(y, parameters, algOid);
	}
}
