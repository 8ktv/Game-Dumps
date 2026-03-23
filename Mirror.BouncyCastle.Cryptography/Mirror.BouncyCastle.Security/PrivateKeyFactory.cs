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
using Mirror.BouncyCastle.Asn1.Sec;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Pkcs;

namespace Mirror.BouncyCastle.Security;

public static class PrivateKeyFactory
{
	public static AsymmetricKeyParameter CreateKey(byte[] privateKeyInfoData)
	{
		return CreateKey(PrivateKeyInfo.GetInstance(Asn1Object.FromByteArray(privateKeyInfoData)));
	}

	public static AsymmetricKeyParameter CreateKey(Stream inStr)
	{
		return CreateKey(PrivateKeyInfo.GetInstance(Asn1Object.FromStream(inStr)));
	}

	public static AsymmetricKeyParameter CreateKey(PrivateKeyInfo keyInfo)
	{
		AlgorithmIdentifier privateKeyAlgorithm = keyInfo.PrivateKeyAlgorithm;
		DerObjectIdentifier algorithm = privateKeyAlgorithm.Algorithm;
		if (algorithm.Equals(PkcsObjectIdentifiers.RsaEncryption) || algorithm.Equals(X509ObjectIdentifiers.IdEARsa) || algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss) || algorithm.Equals(PkcsObjectIdentifiers.IdRsaesOaep))
		{
			RsaPrivateKeyStructure instance = RsaPrivateKeyStructure.GetInstance(keyInfo.ParsePrivateKey());
			return new RsaPrivateCrtKeyParameters(instance.Modulus, instance.PublicExponent, instance.PrivateExponent, instance.Prime1, instance.Prime2, instance.Exponent1, instance.Exponent2, instance.Coefficient);
		}
		if (algorithm.Equals(PkcsObjectIdentifiers.DhKeyAgreement))
		{
			DHParameter dHParameter = new DHParameter(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
			DerInteger obj = (DerInteger)keyInfo.ParsePrivateKey();
			return new DHPrivateKeyParameters(parameters: new DHParameters(l: dHParameter.L?.IntValue ?? 0, p: dHParameter.P, g: dHParameter.G, q: null), x: obj.Value, algorithmOid: algorithm);
		}
		if (algorithm.Equals(OiwObjectIdentifiers.ElGamalAlgorithm))
		{
			ElGamalParameter elGamalParameter = new ElGamalParameter(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
			return new ElGamalPrivateKeyParameters(((DerInteger)keyInfo.ParsePrivateKey()).Value, new ElGamalParameters(elGamalParameter.P, elGamalParameter.G));
		}
		if (algorithm.Equals(X9ObjectIdentifiers.IdDsa))
		{
			DerInteger obj2 = (DerInteger)keyInfo.ParsePrivateKey();
			Asn1Encodable parameters = privateKeyAlgorithm.Parameters;
			DsaParameters parameters2 = null;
			if (parameters != null)
			{
				DsaParameter instance2 = DsaParameter.GetInstance(parameters.ToAsn1Object());
				parameters2 = new DsaParameters(instance2.P, instance2.Q, instance2.G);
			}
			return new DsaPrivateKeyParameters(obj2.Value, parameters2);
		}
		if (algorithm.Equals(X9ObjectIdentifiers.IdECPublicKey))
		{
			X962Parameters instance3 = X962Parameters.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object());
			X9ECParameters x9ECParameters = ((!instance3.IsNamedCurve) ? new X9ECParameters((Asn1Sequence)instance3.Parameters) : ECKeyPairGenerator.FindECCurveByOid((DerObjectIdentifier)instance3.Parameters));
			BigInteger key = ECPrivateKeyStructure.GetInstance(keyInfo.ParsePrivateKey()).GetKey();
			if (instance3.IsNamedCurve)
			{
				return new ECPrivateKeyParameters("EC", key, (DerObjectIdentifier)instance3.Parameters);
			}
			ECDomainParameters parameters3 = new ECDomainParameters(x9ECParameters.Curve, x9ECParameters.G, x9ECParameters.N, x9ECParameters.H, x9ECParameters.GetSeed());
			return new ECPrivateKeyParameters(key, parameters3);
		}
		if (algorithm.Equals(CryptoProObjectIdentifiers.GostR3410x2001) || algorithm.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512) || algorithm.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_256))
		{
			Asn1Object asn1Object = privateKeyAlgorithm.Parameters.ToAsn1Object();
			Gost3410PublicKeyAlgParameters instance4 = Gost3410PublicKeyAlgParameters.GetInstance(asn1Object);
			ECGost3410Parameters dp;
			BigInteger d;
			if (asn1Object is Asn1Sequence asn1Sequence && (asn1Sequence.Count == 2 || asn1Sequence.Count == 3))
			{
				X9ECParameters byOid = ECGost3410NamedCurves.GetByOid(instance4.PublicKeyParamSet);
				if (byOid == null)
				{
					throw new ArgumentException("Unrecognized curve OID for GostR3410x2001 private key");
				}
				dp = new ECGost3410Parameters(new ECNamedDomainParameters(instance4.PublicKeyParamSet, byOid), instance4.PublicKeyParamSet, instance4.DigestParamSet, instance4.EncryptionParamSet);
				int privateKeyLength = keyInfo.PrivateKeyLength;
				if (privateKeyLength == 32 || privateKeyLength == 64)
				{
					d = new BigInteger(1, keyInfo.PrivateKey.GetOctets(), bigEndian: false);
				}
				else
				{
					Asn1Object asn1Object2 = keyInfo.ParsePrivateKey();
					if (asn1Object2 is DerInteger derInteger)
					{
						d = derInteger.PositiveValue;
					}
					else
					{
						byte[] octets = Asn1OctetString.GetInstance(asn1Object2).GetOctets();
						d = new BigInteger(1, octets, bigEndian: false);
					}
				}
			}
			else
			{
				X962Parameters instance5 = X962Parameters.GetInstance(asn1Object);
				if (instance5.IsNamedCurve)
				{
					DerObjectIdentifier instance6 = DerObjectIdentifier.GetInstance(instance5.Parameters);
					X9ECParameters byOid2 = ECNamedCurveTable.GetByOid(instance6);
					if (byOid2 == null)
					{
						throw new ArgumentException("Unrecognized curve OID for GostR3410x2001 private key");
					}
					dp = new ECGost3410Parameters(new ECNamedDomainParameters(instance6, byOid2), instance4.PublicKeyParamSet, instance4.DigestParamSet, instance4.EncryptionParamSet);
				}
				else if (instance5.IsImplicitlyCA)
				{
					dp = null;
				}
				else
				{
					X9ECParameters instance7 = X9ECParameters.GetInstance(instance5.Parameters);
					dp = new ECGost3410Parameters(new ECNamedDomainParameters(algorithm, instance7), instance4.PublicKeyParamSet, instance4.DigestParamSet, instance4.EncryptionParamSet);
				}
				Asn1Object asn1Object3 = keyInfo.ParsePrivateKey();
				d = ((!(asn1Object3 is DerInteger derInteger2)) ? ECPrivateKeyStructure.GetInstance(asn1Object3).GetKey() : derInteger2.Value);
			}
			return new ECPrivateKeyParameters(d, new ECGost3410Parameters(dp, instance4.PublicKeyParamSet, instance4.DigestParamSet, instance4.EncryptionParamSet));
		}
		if (algorithm.Equals(CryptoProObjectIdentifiers.GostR3410x94))
		{
			Gost3410PublicKeyAlgParameters instance8 = Gost3410PublicKeyAlgParameters.GetInstance(privateKeyAlgorithm.Parameters);
			Asn1Object asn1Object4 = keyInfo.ParsePrivateKey();
			BigInteger x = ((!(asn1Object4 is DerInteger)) ? new BigInteger(1, Asn1OctetString.GetInstance(asn1Object4).GetOctets(), bigEndian: false) : DerInteger.GetInstance(asn1Object4).PositiveValue);
			return new Gost3410PrivateKeyParameters(x, instance8.PublicKeyParamSet);
		}
		if (algorithm.Equals(EdECObjectIdentifiers.id_X25519) || algorithm.Equals(CryptlibObjectIdentifiers.curvey25519))
		{
			if (X25519PrivateKeyParameters.KeySize == keyInfo.PrivateKeyLength)
			{
				return new X25519PrivateKeyParameters(keyInfo.PrivateKey.GetOctets());
			}
			return new X25519PrivateKeyParameters(GetRawKey(keyInfo));
		}
		if (algorithm.Equals(EdECObjectIdentifiers.id_X448))
		{
			if (X448PrivateKeyParameters.KeySize == keyInfo.PrivateKeyLength)
			{
				return new X448PrivateKeyParameters(keyInfo.PrivateKey.GetOctets());
			}
			return new X448PrivateKeyParameters(GetRawKey(keyInfo));
		}
		if (algorithm.Equals(EdECObjectIdentifiers.id_Ed25519) || algorithm.Equals(GnuObjectIdentifiers.Ed25519))
		{
			return new Ed25519PrivateKeyParameters(GetRawKey(keyInfo));
		}
		if (algorithm.Equals(EdECObjectIdentifiers.id_Ed448))
		{
			return new Ed448PrivateKeyParameters(GetRawKey(keyInfo));
		}
		if (algorithm.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_256) || algorithm.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512) || algorithm.Equals(RosstandartObjectIdentifiers.id_tc26_agreement_gost_3410_12_256) || algorithm.Equals(RosstandartObjectIdentifiers.id_tc26_agreement_gost_3410_12_512))
		{
			Gost3410PublicKeyAlgParameters instance9 = Gost3410PublicKeyAlgParameters.GetInstance(keyInfo.PrivateKeyAlgorithm.Parameters);
			Asn1Object asn1Object5 = keyInfo.PrivateKeyAlgorithm.Parameters.ToAsn1Object();
			ECGost3410Parameters dp2;
			BigInteger d2;
			if (asn1Object5 is Asn1Sequence && (Asn1Sequence.GetInstance(asn1Object5).Count == 2 || Asn1Sequence.GetInstance(asn1Object5).Count == 3))
			{
				X9ECParameters byOid3 = ECGost3410NamedCurves.GetByOid(instance9.PublicKeyParamSet);
				dp2 = new ECGost3410Parameters(new ECNamedDomainParameters(instance9.PublicKeyParamSet, byOid3), instance9.PublicKeyParamSet, instance9.DigestParamSet, instance9.EncryptionParamSet);
				int privateKeyLength2 = keyInfo.PrivateKeyLength;
				if (privateKeyLength2 == 32 || privateKeyLength2 == 64)
				{
					d2 = new BigInteger(1, keyInfo.PrivateKey.GetOctets(), bigEndian: false);
				}
				else
				{
					Asn1Encodable asn1Encodable = keyInfo.ParsePrivateKey();
					if (asn1Encodable is DerInteger)
					{
						d2 = DerInteger.GetInstance(asn1Encodable).PositiveValue;
					}
					else
					{
						byte[] octets2 = Asn1OctetString.GetInstance(asn1Encodable).GetOctets();
						d2 = new BigInteger(1, octets2, bigEndian: false);
					}
				}
			}
			else
			{
				X962Parameters instance10 = X962Parameters.GetInstance(keyInfo.PrivateKeyAlgorithm.Parameters);
				if (instance10.IsNamedCurve)
				{
					DerObjectIdentifier instance11 = DerObjectIdentifier.GetInstance(instance10.Parameters);
					X9ECParameters x2 = ECKeyPairGenerator.FindECCurveByOid(instance11);
					dp2 = new ECGost3410Parameters(new ECNamedDomainParameters(instance11, x2), instance9.PublicKeyParamSet, instance9.DigestParamSet, instance9.EncryptionParamSet);
				}
				else if (instance10.IsImplicitlyCA)
				{
					dp2 = null;
				}
				else
				{
					X9ECParameters instance12 = X9ECParameters.GetInstance(instance10.Parameters);
					dp2 = new ECGost3410Parameters(new ECNamedDomainParameters(algorithm, instance12), instance9.PublicKeyParamSet, instance9.DigestParamSet, instance9.EncryptionParamSet);
				}
				Asn1Encodable asn1Encodable2 = keyInfo.ParsePrivateKey();
				d2 = ((!(asn1Encodable2 is DerInteger)) ? ECPrivateKeyStructure.GetInstance(asn1Encodable2).GetKey() : DerInteger.GetInstance(asn1Encodable2).Value);
			}
			return new ECPrivateKeyParameters(d2, new ECGost3410Parameters(dp2, instance9.PublicKeyParamSet, instance9.DigestParamSet, instance9.EncryptionParamSet));
		}
		throw new SecurityUtilityException("algorithm identifier in private key not recognised");
	}

	private static byte[] GetRawKey(PrivateKeyInfo keyInfo)
	{
		return Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey()).GetOctets();
	}

	public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, EncryptedPrivateKeyInfo encInfo)
	{
		return CreateKey(PrivateKeyInfoFactory.CreatePrivateKeyInfo(passPhrase, encInfo));
	}

	public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, byte[] encryptedPrivateKeyInfoData)
	{
		return DecryptKey(passPhrase, Asn1Object.FromByteArray(encryptedPrivateKeyInfoData));
	}

	public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, Stream encryptedPrivateKeyInfoStream)
	{
		return DecryptKey(passPhrase, Asn1Object.FromStream(encryptedPrivateKeyInfoStream));
	}

	private static AsymmetricKeyParameter DecryptKey(char[] passPhrase, Asn1Object asn1Object)
	{
		return DecryptKey(passPhrase, EncryptedPrivateKeyInfo.GetInstance(asn1Object));
	}

	public static byte[] EncryptKey(DerObjectIdentifier algorithm, char[] passPhrase, byte[] salt, int iterationCount, AsymmetricKeyParameter key)
	{
		return EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(algorithm, passPhrase, salt, iterationCount, key).GetEncoded();
	}

	public static byte[] EncryptKey(string algorithm, char[] passPhrase, byte[] salt, int iterationCount, AsymmetricKeyParameter key)
	{
		return EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(algorithm, passPhrase, salt, iterationCount, key).GetEncoded();
	}
}
