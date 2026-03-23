using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.TeleTrust;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.X509;

internal static class X509Utilities
{
	private static readonly Dictionary<string, DerObjectIdentifier> m_algorithms;

	private static readonly Dictionary<string, Asn1Encodable> m_exParams;

	private static readonly HashSet<DerObjectIdentifier> m_noParams;

	static X509Utilities()
	{
		m_algorithms = new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);
		m_exParams = new Dictionary<string, Asn1Encodable>(StringComparer.OrdinalIgnoreCase);
		m_noParams = new HashSet<DerObjectIdentifier>();
		m_algorithms.Add("MD2WITHRSAENCRYPTION", PkcsObjectIdentifiers.MD2WithRsaEncryption);
		m_algorithms.Add("MD2WITHRSA", PkcsObjectIdentifiers.MD2WithRsaEncryption);
		m_algorithms.Add("MD5WITHRSAENCRYPTION", PkcsObjectIdentifiers.MD5WithRsaEncryption);
		m_algorithms.Add("MD5WITHRSA", PkcsObjectIdentifiers.MD5WithRsaEncryption);
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
		m_algorithms.Add("RIPEMD160WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160);
		m_algorithms.Add("RIPEMD160WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160);
		m_algorithms.Add("RIPEMD128WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128);
		m_algorithms.Add("RIPEMD128WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128);
		m_algorithms.Add("RIPEMD256WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256);
		m_algorithms.Add("RIPEMD256WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256);
		m_algorithms.Add("SHA1WITHDSA", X9ObjectIdentifiers.IdDsaWithSha1);
		m_algorithms.Add("DSAWITHSHA1", X9ObjectIdentifiers.IdDsaWithSha1);
		m_algorithms.Add("SHA224WITHDSA", NistObjectIdentifiers.DsaWithSha224);
		m_algorithms.Add("SHA256WITHDSA", NistObjectIdentifiers.DsaWithSha256);
		m_algorithms.Add("SHA384WITHDSA", NistObjectIdentifiers.DsaWithSha384);
		m_algorithms.Add("SHA512WITHDSA", NistObjectIdentifiers.DsaWithSha512);
		m_algorithms.Add("SHA1WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha1);
		m_algorithms.Add("ECDSAWITHSHA1", X9ObjectIdentifiers.ECDsaWithSha1);
		m_algorithms.Add("SHA224WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha224);
		m_algorithms.Add("SHA256WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha256);
		m_algorithms.Add("SHA384WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha384);
		m_algorithms.Add("SHA512WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha512);
		m_algorithms.Add("GOST3411WITHGOST3410", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
		m_algorithms.Add("GOST3411WITHGOST3410-94", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
		m_algorithms.Add("GOST3411WITHECGOST3410", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		m_algorithms.Add("GOST3411WITHECGOST3410-2001", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		m_algorithms.Add("GOST3411WITHGOST3410-2001", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha1);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha224);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha256);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha384);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha512);
		m_noParams.Add(X9ObjectIdentifiers.IdDsaWithSha1);
		m_noParams.Add(OiwObjectIdentifiers.DsaWithSha1);
		m_noParams.Add(NistObjectIdentifiers.DsaWithSha224);
		m_noParams.Add(NistObjectIdentifiers.DsaWithSha256);
		m_noParams.Add(NistObjectIdentifiers.DsaWithSha384);
		m_noParams.Add(NistObjectIdentifiers.DsaWithSha512);
		m_noParams.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
		m_noParams.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
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

	internal static byte[] CalculateDigest(AlgorithmIdentifier digestAlgorithm, Asn1Encodable asn1Encodable)
	{
		return CalculateResult(new DefaultDigestCalculator(DigestUtilities.GetDigest(digestAlgorithm.Algorithm)), asn1Encodable).Collect();
	}

	internal static byte[] CalculateDigest(IDigestFactory digestFactory, byte[] buf, int off, int len)
	{
		return CalculateResult(digestFactory.CreateCalculator(), buf, off, len).Collect();
	}

	internal static byte[] CalculateDigest(IDigestFactory digestFactory, Asn1Encodable asn1Encodable)
	{
		return CalculateResult(digestFactory.CreateCalculator(), asn1Encodable).Collect();
	}

	internal static TResult CalculateResult<TResult>(IStreamCalculator<TResult> streamCalculator, byte[] buf, int off, int len)
	{
		using (Stream stream = streamCalculator.Stream)
		{
			stream.Write(buf, off, len);
		}
		return streamCalculator.GetResult();
	}

	internal static TResult CalculateResult<TResult>(IStreamCalculator<TResult> streamCalculator, Asn1Encodable asn1Encodable)
	{
		using (Stream output = streamCalculator.Stream)
		{
			asn1Encodable.EncodeTo(output, "DER");
		}
		return streamCalculator.GetResult();
	}

	private static RsassaPssParameters CreatePssParams(AlgorithmIdentifier hashAlgId, int saltSize)
	{
		return new RsassaPssParameters(hashAlgId, new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, hashAlgId), new DerInteger(saltSize), new DerInteger(1));
	}

	internal static DerBitString CollectDerBitString(IBlockResult result)
	{
		return new DerBitString(result.Collect());
	}

	internal static DerObjectIdentifier GetAlgorithmOid(string algorithmName)
	{
		if (m_algorithms.TryGetValue(algorithmName, out var value))
		{
			return value;
		}
		return new DerObjectIdentifier(algorithmName);
	}

	internal static AlgorithmIdentifier GetSigAlgID(DerObjectIdentifier sigOid, string algorithmName)
	{
		if (m_noParams.Contains(sigOid))
		{
			return new AlgorithmIdentifier(sigOid);
		}
		if (m_exParams.TryGetValue(algorithmName, out var value))
		{
			return new AlgorithmIdentifier(sigOid, value);
		}
		return new AlgorithmIdentifier(sigOid, DerNull.Instance);
	}

	internal static IEnumerable<string> GetAlgNames()
	{
		return CollectionUtilities.Proxy(m_algorithms.Keys);
	}

	internal static DerBitString GenerateBitString(IStreamCalculator<IBlockResult> streamCalculator, Asn1Encodable asn1Encodable)
	{
		return CollectDerBitString(CalculateResult(streamCalculator, asn1Encodable));
	}

	internal static DerBitString GenerateDigest(IDigestFactory digestFactory, Asn1Encodable asn1Encodable)
	{
		return GenerateBitString(digestFactory.CreateCalculator(), asn1Encodable);
	}

	internal static DerBitString GenerateMac(IMacFactory macFactory, Asn1Encodable asn1Encodable)
	{
		return GenerateBitString(macFactory.CreateCalculator(), asn1Encodable);
	}

	internal static DerBitString GenerateSignature(ISignatureFactory signatureFactory, Asn1Encodable asn1Encodable)
	{
		return GenerateBitString(signatureFactory.CreateCalculator(), asn1Encodable);
	}

	internal static bool VerifyMac(IMacFactory macFactory, Asn1Encodable asn1Encodable, DerBitString expected)
	{
		return Arrays.FixedTimeEquals(CalculateResult(macFactory.CreateCalculator(), asn1Encodable).Collect(), expected.GetOctets());
	}

	internal static bool VerifySignature(IVerifierFactory verifierFactory, Asn1Encodable asn1Encodable, DerBitString signature)
	{
		return CalculateResult(verifierFactory.CreateCalculator(), asn1Encodable).IsVerified(signature.GetOctets());
	}

	internal static Asn1TaggedObject TrimExtensions(int tagNo, X509Extensions exts)
	{
		Asn1Sequence instance = Asn1Sequence.GetInstance(exts.ToAsn1Object());
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		foreach (Asn1Encodable item in instance)
		{
			Asn1Sequence instance2 = Asn1Sequence.GetInstance(item);
			if (!X509Extensions.AltSignatureValue.Equals(instance2[0]))
			{
				asn1EncodableVector.Add(instance2);
			}
		}
		return new DerTaggedObject(isExplicit: true, tagNo, new DerSequence(asn1EncodableVector));
	}
}
