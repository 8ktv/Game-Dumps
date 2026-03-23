using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Security;

public static class DotNetUtilities
{
	public static System.Security.Cryptography.X509Certificates.X509Certificate ToX509Certificate(X509CertificateStructure x509Struct)
	{
		return new System.Security.Cryptography.X509Certificates.X509Certificate(x509Struct.GetDerEncoded());
	}

	public static System.Security.Cryptography.X509Certificates.X509Certificate ToX509Certificate(Mirror.BouncyCastle.X509.X509Certificate x509Cert)
	{
		return new System.Security.Cryptography.X509Certificates.X509Certificate(x509Cert.GetEncoded());
	}

	public static Mirror.BouncyCastle.X509.X509Certificate FromX509Certificate(System.Security.Cryptography.X509Certificates.X509Certificate x509Cert)
	{
		return new X509CertificateParser().ReadCertificate(x509Cert.GetRawCertData());
	}

	public static AsymmetricCipherKeyPair GetDsaKeyPair(DSA dsa)
	{
		return GetDsaKeyPair(dsa.ExportParameters(includePrivateParameters: true));
	}

	public static AsymmetricCipherKeyPair GetDsaKeyPair(DSAParameters dp)
	{
		DsaPublicKeyParameters dsaPublicKey = GetDsaPublicKey(dp);
		DsaPrivateKeyParameters privateParameter = new DsaPrivateKeyParameters(new BigInteger(1, dp.X), dsaPublicKey.Parameters);
		return new AsymmetricCipherKeyPair(dsaPublicKey, privateParameter);
	}

	public static DsaPublicKeyParameters GetDsaPublicKey(DSA dsa)
	{
		return GetDsaPublicKey(dsa.ExportParameters(includePrivateParameters: false));
	}

	public static DsaPublicKeyParameters GetDsaPublicKey(DSAParameters dp)
	{
		DsaValidationParameters parameters = ((dp.Seed != null) ? new DsaValidationParameters(dp.Seed, dp.Counter) : null);
		DsaParameters parameters2 = new DsaParameters(new BigInteger(1, dp.P), new BigInteger(1, dp.Q), new BigInteger(1, dp.G), parameters);
		return new DsaPublicKeyParameters(new BigInteger(1, dp.Y), parameters2);
	}

	public static AsymmetricCipherKeyPair GetECDsaKeyPair(ECDsa ecDsa)
	{
		return GetECKeyPair("ECDSA", ecDsa.ExportParameters(includePrivateParameters: true));
	}

	public static ECPublicKeyParameters GetECDsaPublicKey(ECDsa ecDsa)
	{
		return GetECPublicKey("ECDSA", ecDsa.ExportParameters(includePrivateParameters: false));
	}

	public static AsymmetricCipherKeyPair GetECKeyPair(string algorithm, ECParameters ec)
	{
		ECPublicKeyParameters eCPublicKey = GetECPublicKey(algorithm, ec);
		ECPrivateKeyParameters privateParameter = new ECPrivateKeyParameters(eCPublicKey.AlgorithmName, new BigInteger(1, ec.D), eCPublicKey.Parameters);
		return new AsymmetricCipherKeyPair(eCPublicKey, privateParameter);
	}

	public static ECPublicKeyParameters GetECPublicKey(string algorithm, ECParameters ec)
	{
		X9ECParameters x9ECParameters = GetX9ECParameters(ec.Curve);
		if (x9ECParameters == null)
		{
			throw new NotSupportedException("Unrecognized curve");
		}
		return new ECPublicKeyParameters(algorithm, GetECPoint(x9ECParameters.Curve, ec.Q), new ECDomainParameters(x9ECParameters));
	}

	private static Mirror.BouncyCastle.Math.EC.ECPoint GetECPoint(Mirror.BouncyCastle.Math.EC.ECCurve curve, System.Security.Cryptography.ECPoint point)
	{
		return curve.CreatePoint(new BigInteger(1, point.X), new BigInteger(1, point.Y));
	}

	private static X9ECParameters GetX9ECParameters(System.Security.Cryptography.ECCurve curve)
	{
		if (!curve.IsNamed)
		{
			throw new NotSupportedException("Only named curves are supported");
		}
		Oid oid = curve.Oid;
		if (oid != null)
		{
			string value = oid.Value;
			if (value != null)
			{
				return ECKeyPairGenerator.FindECCurveByOid(new DerObjectIdentifier(value));
			}
		}
		return null;
	}

	public static AsymmetricCipherKeyPair GetRsaKeyPair(RSA rsa)
	{
		return GetRsaKeyPair(rsa.ExportParameters(includePrivateParameters: true));
	}

	public static AsymmetricCipherKeyPair GetRsaKeyPair(RSAParameters rp)
	{
		RsaKeyParameters rsaPublicKey = GetRsaPublicKey(rp);
		RsaPrivateCrtKeyParameters privateParameter = new RsaPrivateCrtKeyParameters(rsaPublicKey.Modulus, rsaPublicKey.Exponent, new BigInteger(1, rp.D), new BigInteger(1, rp.P), new BigInteger(1, rp.Q), new BigInteger(1, rp.DP), new BigInteger(1, rp.DQ), new BigInteger(1, rp.InverseQ));
		return new AsymmetricCipherKeyPair(rsaPublicKey, privateParameter);
	}

	public static RsaKeyParameters GetRsaPublicKey(RSA rsa)
	{
		return GetRsaPublicKey(rsa.ExportParameters(includePrivateParameters: false));
	}

	public static RsaKeyParameters GetRsaPublicKey(RSAParameters rp)
	{
		return new RsaKeyParameters(isPrivate: false, new BigInteger(1, rp.Modulus), new BigInteger(1, rp.Exponent));
	}

	public static AsymmetricCipherKeyPair GetKeyPair(AsymmetricAlgorithm privateKey)
	{
		if (privateKey is DSA dsa)
		{
			return GetDsaKeyPair(dsa);
		}
		if (privateKey is ECDsa ecDsa)
		{
			return GetECDsaKeyPair(ecDsa);
		}
		if (privateKey is RSA rsa)
		{
			return GetRsaKeyPair(rsa);
		}
		throw new ArgumentException("Unsupported algorithm specified", "privateKey");
	}

	public static RSA ToRSA(RsaKeyParameters rsaKey)
	{
		return CreateRSAProvider(ToRSAParameters(rsaKey));
	}

	public static RSA ToRSA(RsaKeyParameters rsaKey, CspParameters csp)
	{
		return CreateRSAProvider(ToRSAParameters(rsaKey), csp);
	}

	public static RSA ToRSA(RsaPrivateCrtKeyParameters privKey)
	{
		return CreateRSAProvider(ToRSAParameters(privKey));
	}

	public static RSA ToRSA(RsaPrivateCrtKeyParameters privKey, CspParameters csp)
	{
		return CreateRSAProvider(ToRSAParameters(privKey), csp);
	}

	public static RSA ToRSA(RsaPrivateKeyStructure privKey)
	{
		return CreateRSAProvider(ToRSAParameters(privKey));
	}

	public static RSA ToRSA(RsaPrivateKeyStructure privKey, CspParameters csp)
	{
		return CreateRSAProvider(ToRSAParameters(privKey), csp);
	}

	public static RSAParameters ToRSAParameters(RsaKeyParameters rsaKey)
	{
		RSAParameters result = new RSAParameters
		{
			Modulus = rsaKey.Modulus.ToByteArrayUnsigned()
		};
		if (rsaKey.IsPrivate)
		{
			result.D = ConvertRSAParametersField(rsaKey.Exponent, result.Modulus.Length);
		}
		else
		{
			result.Exponent = rsaKey.Exponent.ToByteArrayUnsigned();
		}
		return result;
	}

	public static RSAParameters ToRSAParameters(RsaPrivateCrtKeyParameters privKey)
	{
		RSAParameters result = default(RSAParameters);
		result.Modulus = privKey.Modulus.ToByteArrayUnsigned();
		result.Exponent = privKey.PublicExponent.ToByteArrayUnsigned();
		result.P = privKey.P.ToByteArrayUnsigned();
		result.Q = privKey.Q.ToByteArrayUnsigned();
		result.D = ConvertRSAParametersField(privKey.Exponent, result.Modulus.Length);
		result.DP = ConvertRSAParametersField(privKey.DP, result.P.Length);
		result.DQ = ConvertRSAParametersField(privKey.DQ, result.Q.Length);
		result.InverseQ = ConvertRSAParametersField(privKey.QInv, result.Q.Length);
		return result;
	}

	public static RSAParameters ToRSAParameters(RsaPrivateKeyStructure privKey)
	{
		RSAParameters result = default(RSAParameters);
		result.Modulus = privKey.Modulus.ToByteArrayUnsigned();
		result.Exponent = privKey.PublicExponent.ToByteArrayUnsigned();
		result.P = privKey.Prime1.ToByteArrayUnsigned();
		result.Q = privKey.Prime2.ToByteArrayUnsigned();
		result.D = ConvertRSAParametersField(privKey.PrivateExponent, result.Modulus.Length);
		result.DP = ConvertRSAParametersField(privKey.Exponent1, result.P.Length);
		result.DQ = ConvertRSAParametersField(privKey.Exponent2, result.Q.Length);
		result.InverseQ = ConvertRSAParametersField(privKey.Coefficient, result.Q.Length);
		return result;
	}

	private static byte[] ConvertRSAParametersField(BigInteger n, int size)
	{
		return BigIntegers.AsUnsignedByteArray(size, n);
	}

	private static RSACryptoServiceProvider CreateRSAProvider(RSAParameters rp)
	{
		CspParameters cspParameters = new CspParameters();
		cspParameters.KeyContainerName = $"BouncyCastle-{Guid.NewGuid()}";
		return CreateRSAProvider(rp, cspParameters);
	}

	private static RSACryptoServiceProvider CreateRSAProvider(RSAParameters rp, CspParameters csp)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(csp);
		rSACryptoServiceProvider.ImportParameters(rp);
		return rSACryptoServiceProvider;
	}
}
