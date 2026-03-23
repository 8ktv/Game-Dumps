using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.TeleTrust;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto.Encodings;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Crypto.Signers;

public class RsaDigestSigner : ISigner
{
	private readonly IAsymmetricBlockCipher rsaEngine;

	private readonly AlgorithmIdentifier algId;

	private readonly IDigest digest;

	private bool forSigning;

	private static readonly IDictionary<string, DerObjectIdentifier> OidMap;

	public virtual string AlgorithmName => digest.AlgorithmName + "withRSA";

	static RsaDigestSigner()
	{
		OidMap = new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);
		OidMap["RIPEMD128"] = TeleTrusTObjectIdentifiers.RipeMD128;
		OidMap["RIPEMD160"] = TeleTrusTObjectIdentifiers.RipeMD160;
		OidMap["RIPEMD256"] = TeleTrusTObjectIdentifiers.RipeMD256;
		OidMap["SHA-1"] = X509ObjectIdentifiers.IdSha1;
		OidMap["SHA-224"] = NistObjectIdentifiers.IdSha224;
		OidMap["SHA-256"] = NistObjectIdentifiers.IdSha256;
		OidMap["SHA-384"] = NistObjectIdentifiers.IdSha384;
		OidMap["SHA-512"] = NistObjectIdentifiers.IdSha512;
		OidMap["SHA-512/224"] = NistObjectIdentifiers.IdSha512_224;
		OidMap["SHA-512/256"] = NistObjectIdentifiers.IdSha512_256;
		OidMap["SHA3-224"] = NistObjectIdentifiers.IdSha3_224;
		OidMap["SHA3-256"] = NistObjectIdentifiers.IdSha3_256;
		OidMap["SHA3-384"] = NistObjectIdentifiers.IdSha3_384;
		OidMap["SHA3-512"] = NistObjectIdentifiers.IdSha3_512;
		OidMap["MD2"] = PkcsObjectIdentifiers.MD2;
		OidMap["MD4"] = PkcsObjectIdentifiers.MD4;
		OidMap["MD5"] = PkcsObjectIdentifiers.MD5;
	}

	public RsaDigestSigner(IDigest digest)
		: this(digest, CollectionUtilities.GetValueOrNull(OidMap, digest.AlgorithmName))
	{
	}

	public RsaDigestSigner(IDigest digest, DerObjectIdentifier digestOid)
		: this(digest, new AlgorithmIdentifier(digestOid, DerNull.Instance))
	{
	}

	public RsaDigestSigner(IDigest digest, AlgorithmIdentifier algId)
		: this(new RsaCoreEngine(), digest, algId)
	{
	}

	public RsaDigestSigner(IRsa rsa, IDigest digest, DerObjectIdentifier digestOid)
		: this(rsa, digest, new AlgorithmIdentifier(digestOid, DerNull.Instance))
	{
	}

	public RsaDigestSigner(IRsa rsa, IDigest digest, AlgorithmIdentifier algId)
		: this(new RsaBlindedEngine(rsa), digest, algId)
	{
	}

	public RsaDigestSigner(IAsymmetricBlockCipher rsaEngine, IDigest digest, AlgorithmIdentifier algId)
	{
		this.rsaEngine = new Pkcs1Encoding(rsaEngine);
		this.digest = digest;
		this.algId = algId;
	}

	public virtual void Init(bool forSigning, ICipherParameters parameters)
	{
		this.forSigning = forSigning;
		AsymmetricKeyParameter asymmetricKeyParameter = ((!(parameters is ParametersWithRandom parametersWithRandom)) ? ((AsymmetricKeyParameter)parameters) : ((AsymmetricKeyParameter)parametersWithRandom.Parameters));
		if (forSigning && !asymmetricKeyParameter.IsPrivate)
		{
			throw new InvalidKeyException("Signing requires private key.");
		}
		if (!forSigning && asymmetricKeyParameter.IsPrivate)
		{
			throw new InvalidKeyException("Verification requires public key.");
		}
		Reset();
		rsaEngine.Init(forSigning, parameters);
	}

	public virtual void Update(byte input)
	{
		digest.Update(input);
	}

	public virtual void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		digest.BlockUpdate(input, inOff, inLen);
	}

	public virtual int GetMaxSignatureSize()
	{
		return rsaEngine.GetOutputBlockSize();
	}

	public virtual byte[] GenerateSignature()
	{
		if (!forSigning)
		{
			throw new InvalidOperationException("RsaDigestSigner not initialised for signature generation.");
		}
		byte[] array = new byte[digest.GetDigestSize()];
		digest.DoFinal(array, 0);
		byte[] array2 = DerEncode(array);
		return rsaEngine.ProcessBlock(array2, 0, array2.Length);
	}

	public virtual bool VerifySignature(byte[] signature)
	{
		if (forSigning)
		{
			throw new InvalidOperationException("RsaDigestSigner not initialised for verification");
		}
		byte[] array = new byte[digest.GetDigestSize()];
		digest.DoFinal(array, 0);
		byte[] array2;
		byte[] array3;
		try
		{
			array2 = rsaEngine.ProcessBlock(signature, 0, signature.Length);
			array3 = DerEncode(array);
		}
		catch (Exception)
		{
			return false;
		}
		if (array2.Length == array3.Length)
		{
			return Arrays.FixedTimeEquals(array2, array3);
		}
		if (array2.Length == array3.Length - 2)
		{
			int num = array2.Length - array.Length - 2;
			int num2 = array3.Length - array.Length - 2;
			array3[1] -= 2;
			array3[3] -= 2;
			int num3 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				num3 |= array2[num + i] ^ array3[num2 + i];
			}
			for (int j = 0; j < num; j++)
			{
				num3 |= array2[j] ^ array3[j];
			}
			return num3 == 0;
		}
		return false;
	}

	public virtual void Reset()
	{
		digest.Reset();
	}

	private byte[] DerEncode(byte[] hash)
	{
		if (algId == null)
		{
			return hash;
		}
		return new DigestInfo(algId, hash).GetDerEncoded();
	}
}
