using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.EdEC;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Asn1.TeleTrust;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkcs;

public class Pkcs10CertificationRequest : CertificationRequest
{
	internal static readonly Dictionary<string, DerObjectIdentifier> m_algorithms;

	internal static readonly Dictionary<string, Asn1Encodable> m_exParams;

	internal static readonly Dictionary<DerObjectIdentifier, string> m_keyAlgorithms;

	internal static readonly HashSet<DerObjectIdentifier> m_noParams;

	static Pkcs10CertificationRequest()
	{
		m_algorithms = new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);
		m_exParams = new Dictionary<string, Asn1Encodable>(StringComparer.OrdinalIgnoreCase);
		m_keyAlgorithms = new Dictionary<DerObjectIdentifier, string>();
		m_noParams = new HashSet<DerObjectIdentifier>();
		m_algorithms.Add("MD2WITHRSAENCRYPTION", PkcsObjectIdentifiers.MD2WithRsaEncryption);
		m_algorithms.Add("MD2WITHRSA", PkcsObjectIdentifiers.MD2WithRsaEncryption);
		m_algorithms.Add("MD5WITHRSAENCRYPTION", PkcsObjectIdentifiers.MD5WithRsaEncryption);
		m_algorithms.Add("MD5WITHRSA", PkcsObjectIdentifiers.MD5WithRsaEncryption);
		m_algorithms.Add("RSAWITHMD5", PkcsObjectIdentifiers.MD5WithRsaEncryption);
		m_algorithms.Add("SHA1WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
		m_algorithms.Add("SHA-1WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
		m_algorithms.Add("SHA1WITHRSA", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
		m_algorithms.Add("SHA-1WITHRSA", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
		m_algorithms.Add("SHA224WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
		m_algorithms.Add("SHA-224WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
		m_algorithms.Add("SHA224WITHRSA", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
		m_algorithms.Add("SHA-224WITHRSA", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
		m_algorithms.Add("SHA256WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
		m_algorithms.Add("SHA-256WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
		m_algorithms.Add("SHA256WITHRSA", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
		m_algorithms.Add("SHA-256WITHRSA", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
		m_algorithms.Add("SHA384WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
		m_algorithms.Add("SHA-384WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
		m_algorithms.Add("SHA384WITHRSA", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
		m_algorithms.Add("SHA-384WITHRSA", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
		m_algorithms.Add("SHA512WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
		m_algorithms.Add("SHA-512WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
		m_algorithms.Add("SHA512WITHRSA", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
		m_algorithms.Add("SHA-512WITHRSA", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
		m_algorithms.Add("SHA512(224)WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512_224WithRSAEncryption);
		m_algorithms.Add("SHA-512(224)WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512_224WithRSAEncryption);
		m_algorithms.Add("SHA512(224)WITHRSA", PkcsObjectIdentifiers.Sha512_224WithRSAEncryption);
		m_algorithms.Add("SHA-512(224)WITHRSA", PkcsObjectIdentifiers.Sha512_224WithRSAEncryption);
		m_algorithms.Add("SHA512(256)WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512_256WithRSAEncryption);
		m_algorithms.Add("SHA-512(256)WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512_256WithRSAEncryption);
		m_algorithms.Add("SHA512(256)WITHRSA", PkcsObjectIdentifiers.Sha512_256WithRSAEncryption);
		m_algorithms.Add("SHA-512(256)WITHRSA", PkcsObjectIdentifiers.Sha512_256WithRSAEncryption);
		m_algorithms.Add("SHA1WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
		m_algorithms.Add("SHA224WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
		m_algorithms.Add("SHA256WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
		m_algorithms.Add("SHA384WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
		m_algorithms.Add("SHA512WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
		m_algorithms.Add("RSAWITHSHA1", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
		m_algorithms.Add("RIPEMD128WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128);
		m_algorithms.Add("RIPEMD128WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128);
		m_algorithms.Add("RIPEMD160WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160);
		m_algorithms.Add("RIPEMD160WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160);
		m_algorithms.Add("RIPEMD256WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256);
		m_algorithms.Add("RIPEMD256WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256);
		m_algorithms.Add("SHA1WITHDSA", X9ObjectIdentifiers.IdDsaWithSha1);
		m_algorithms.Add("DSAWITHSHA1", X9ObjectIdentifiers.IdDsaWithSha1);
		m_algorithms.Add("SHA224WITHDSA", NistObjectIdentifiers.DsaWithSha224);
		m_algorithms.Add("SHA256WITHDSA", NistObjectIdentifiers.DsaWithSha256);
		m_algorithms.Add("SHA384WITHDSA", NistObjectIdentifiers.DsaWithSha384);
		m_algorithms.Add("SHA512WITHDSA", NistObjectIdentifiers.DsaWithSha512);
		m_algorithms.Add("SHA1WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha1);
		m_algorithms.Add("SHA224WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha224);
		m_algorithms.Add("SHA256WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha256);
		m_algorithms.Add("SHA384WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha384);
		m_algorithms.Add("SHA512WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha512);
		m_algorithms.Add("ECDSAWITHSHA1", X9ObjectIdentifiers.ECDsaWithSha1);
		m_algorithms.Add("GOST3411WITHGOST3410", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
		m_algorithms.Add("GOST3410WITHGOST3411", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
		m_algorithms.Add("GOST3411WITHECGOST3410", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		m_algorithms.Add("GOST3411WITHECGOST3410-2001", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		m_algorithms.Add("GOST3411WITHGOST3410-2001", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		m_algorithms.Add("GOST3411-2012-256WITHECGOST3410", RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_256);
		m_algorithms.Add("GOST3411-2012-256WITHECGOST3410-2012-256", RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_256);
		m_algorithms.Add("GOST3411-2012-512WITHECGOST3410", RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_512);
		m_algorithms.Add("GOST3411-2012-512WITHECGOST3410-2012-512", RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_512);
		m_algorithms.Add("Ed25519", EdECObjectIdentifiers.id_Ed25519);
		m_algorithms.Add("Ed448", EdECObjectIdentifiers.id_Ed448);
		m_keyAlgorithms.Add(PkcsObjectIdentifiers.RsaEncryption, "RSA");
		m_keyAlgorithms.Add(X9ObjectIdentifiers.IdDsa, "DSA");
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha1);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha224);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha256);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha384);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha512);
		m_noParams.Add(X9ObjectIdentifiers.IdDsaWithSha1);
		m_noParams.Add(OiwObjectIdentifiers.DsaWithSha1);
		m_noParams.Add(NistObjectIdentifiers.DsaWithSha224);
		m_noParams.Add(NistObjectIdentifiers.DsaWithSha256);
		m_noParams.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
		m_noParams.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		m_noParams.Add(EdECObjectIdentifiers.id_Ed25519);
		m_noParams.Add(EdECObjectIdentifiers.id_Ed448);
		AlgorithmIdentifier hashAlgId = new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1, DerNull.Instance);
		m_exParams.Add("SHA1WITHRSAANDMGF1", CreatePssParams(hashAlgId, 20));
		AlgorithmIdentifier hashAlgId2 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha224, DerNull.Instance);
		m_exParams.Add("SHA224WITHRSAANDMGF1", CreatePssParams(hashAlgId2, 28));
		AlgorithmIdentifier hashAlgId3 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha256, DerNull.Instance);
		m_exParams.Add("SHA256WITHRSAANDMGF1", CreatePssParams(hashAlgId3, 32));
		AlgorithmIdentifier hashAlgId4 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha384, DerNull.Instance);
		m_exParams.Add("SHA384WITHRSAANDMGF1", CreatePssParams(hashAlgId4, 48));
		AlgorithmIdentifier hashAlgId5 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha512, DerNull.Instance);
		m_exParams.Add("SHA512WITHRSAANDMGF1", CreatePssParams(hashAlgId5, 64));
	}

	private static RsassaPssParameters CreatePssParams(AlgorithmIdentifier hashAlgId, int saltSize)
	{
		return new RsassaPssParameters(hashAlgId, new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, hashAlgId), new DerInteger(saltSize), new DerInteger(1));
	}

	protected Pkcs10CertificationRequest()
	{
	}

	public Pkcs10CertificationRequest(byte[] encoded)
		: base((Asn1Sequence)Asn1Object.FromByteArray(encoded))
	{
	}

	public Pkcs10CertificationRequest(Asn1Sequence seq)
		: base(seq)
	{
	}

	public Pkcs10CertificationRequest(Stream input)
		: base((Asn1Sequence)Asn1Object.FromStream(input))
	{
	}

	public Pkcs10CertificationRequest(string signatureAlgorithm, X509Name subject, AsymmetricKeyParameter publicKey, Asn1Set attributes, AsymmetricKeyParameter signingKey)
		: this(new Asn1SignatureFactory(signatureAlgorithm, signingKey), subject, publicKey, attributes)
	{
	}

	public Pkcs10CertificationRequest(ISignatureFactory signatureFactory, X509Name subject, AsymmetricKeyParameter publicKey, Asn1Set attributes)
	{
		if (signatureFactory == null)
		{
			throw new ArgumentNullException("signatureFactory");
		}
		if (subject == null)
		{
			throw new ArgumentNullException("subject");
		}
		if (publicKey == null)
		{
			throw new ArgumentNullException("publicKey");
		}
		if (publicKey.IsPrivate)
		{
			throw new ArgumentException("expected public key", "publicKey");
		}
		Init(signatureFactory, subject, publicKey, attributes);
	}

	private void Init(ISignatureFactory signatureFactory, X509Name subject, AsymmetricKeyParameter publicKey, Asn1Set attributes)
	{
		sigAlgId = (AlgorithmIdentifier)signatureFactory.AlgorithmDetails;
		SubjectPublicKeyInfo pkInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
		reqInfo = new CertificationRequestInfo(subject, pkInfo, attributes);
		sigBits = Mirror.BouncyCastle.X509.X509Utilities.GenerateSignature(signatureFactory, reqInfo);
	}

	public AsymmetricKeyParameter GetPublicKey()
	{
		return PublicKeyFactory.CreateKey(reqInfo.SubjectPublicKeyInfo);
	}

	public bool Verify()
	{
		return Verify(GetPublicKey());
	}

	public bool Verify(AsymmetricKeyParameter publicKey)
	{
		return Verify(new Asn1VerifierFactoryProvider(publicKey));
	}

	public bool Verify(IVerifierFactoryProvider verifierProvider)
	{
		return Verify(verifierProvider.CreateVerifierFactory(sigAlgId));
	}

	public bool Verify(IVerifierFactory verifier)
	{
		try
		{
			return Mirror.BouncyCastle.X509.X509Utilities.VerifySignature(verifier, reqInfo, sigBits);
		}
		catch (Exception innerException)
		{
			throw new SignatureException("exception encoding TBS cert request", innerException);
		}
	}

	private void SetSignatureParameters(ISigner signature, Asn1Encodable asn1Params)
	{
		if (asn1Params != null && !(asn1Params is Asn1Null) && Platform.EndsWith(signature.AlgorithmName, "MGF1"))
		{
			throw new NotImplementedException("signature algorithm with MGF1");
		}
	}

	internal static string GetSignatureName(AlgorithmIdentifier sigAlgId)
	{
		Asn1Encodable parameters = sigAlgId.Parameters;
		if (parameters != null && !(parameters is Asn1Null) && sigAlgId.Algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss))
		{
			return GetDigestAlgName(RsassaPssParameters.GetInstance(parameters).HashAlgorithm.Algorithm) + "withRSAandMGF1";
		}
		return sigAlgId.Algorithm.Id;
	}

	private static string GetDigestAlgName(DerObjectIdentifier digestAlgOID)
	{
		if (PkcsObjectIdentifiers.MD5.Equals(digestAlgOID))
		{
			return "MD5";
		}
		if (OiwObjectIdentifiers.IdSha1.Equals(digestAlgOID))
		{
			return "SHA1";
		}
		if (NistObjectIdentifiers.IdSha224.Equals(digestAlgOID))
		{
			return "SHA224";
		}
		if (NistObjectIdentifiers.IdSha256.Equals(digestAlgOID))
		{
			return "SHA256";
		}
		if (NistObjectIdentifiers.IdSha384.Equals(digestAlgOID))
		{
			return "SHA384";
		}
		if (NistObjectIdentifiers.IdSha512.Equals(digestAlgOID))
		{
			return "SHA512";
		}
		if (NistObjectIdentifiers.IdSha512_224.Equals(digestAlgOID))
		{
			return "SHA512(224)";
		}
		if (NistObjectIdentifiers.IdSha512_256.Equals(digestAlgOID))
		{
			return "SHA512(256)";
		}
		if (TeleTrusTObjectIdentifiers.RipeMD128.Equals(digestAlgOID))
		{
			return "RIPEMD128";
		}
		if (TeleTrusTObjectIdentifiers.RipeMD160.Equals(digestAlgOID))
		{
			return "RIPEMD160";
		}
		if (TeleTrusTObjectIdentifiers.RipeMD256.Equals(digestAlgOID))
		{
			return "RIPEMD256";
		}
		if (CryptoProObjectIdentifiers.GostR3411.Equals(digestAlgOID))
		{
			return "GOST3411";
		}
		return digestAlgOID.Id;
	}

	public X509Extensions GetRequestedExtensions()
	{
		if (reqInfo.Attributes != null)
		{
			foreach (Asn1Encodable attribute in reqInfo.Attributes)
			{
				AttributePkcs instance;
				try
				{
					instance = AttributePkcs.GetInstance(attribute);
				}
				catch (ArgumentException innerException)
				{
					throw new ArgumentException("encountered non PKCS attribute in extensions block", innerException);
				}
				if (!PkcsObjectIdentifiers.Pkcs9AtExtensionRequest.Equals(instance.AttrType))
				{
					continue;
				}
				X509ExtensionsGenerator x509ExtensionsGenerator = new X509ExtensionsGenerator();
				Asn1Set attrValues = instance.AttrValues;
				if (attrValues == null || attrValues.Count == 0)
				{
					throw new InvalidOperationException("pkcs_9_at_extensionRequest present but has no value");
				}
				Asn1Sequence instance2 = Asn1Sequence.GetInstance(attrValues[0]);
				try
				{
					foreach (Asn1Encodable item in instance2)
					{
						Asn1Sequence instance3 = Asn1Sequence.GetInstance(item);
						if (instance3.Count == 2)
						{
							x509ExtensionsGenerator.AddExtension(DerObjectIdentifier.GetInstance(instance3[0]), critical: false, Asn1OctetString.GetInstance(instance3[1]).GetOctets());
							continue;
						}
						if (instance3.Count == 3)
						{
							bool isTrue = DerBoolean.GetInstance(instance3[1]).IsTrue;
							x509ExtensionsGenerator.AddExtension(DerObjectIdentifier.GetInstance(instance3[0]), isTrue, Asn1OctetString.GetInstance(instance3[2]).GetOctets());
							continue;
						}
						throw new InvalidOperationException("incorrect sequence size of X509Extension got " + instance3.Count + " expected 2 or 3");
					}
				}
				catch (ArgumentException ex)
				{
					throw new InvalidOperationException("asn1 processing issue: " + ex.Message, ex);
				}
				return x509ExtensionsGenerator.Generate();
			}
		}
		return null;
	}
}
