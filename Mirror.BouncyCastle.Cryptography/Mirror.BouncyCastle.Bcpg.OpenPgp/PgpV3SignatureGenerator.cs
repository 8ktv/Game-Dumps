using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.Date;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpV3SignatureGenerator
{
	private readonly PublicKeyAlgorithmTag keyAlgorithm;

	private readonly HashAlgorithmTag hashAlgorithm;

	private PgpPrivateKey privKey;

	private ISigner sig;

	private IDigest dig;

	private int signatureType;

	private byte lastb;

	public PgpV3SignatureGenerator(PublicKeyAlgorithmTag keyAlgorithm, HashAlgorithmTag hashAlgorithm)
	{
		if (keyAlgorithm == PublicKeyAlgorithmTag.EdDsa)
		{
			throw new ArgumentException("Invalid algorithm for V3 signature", "keyAlgorithm");
		}
		this.keyAlgorithm = keyAlgorithm;
		this.hashAlgorithm = hashAlgorithm;
		dig = PgpUtilities.CreateDigest(hashAlgorithm);
	}

	public void InitSign(int sigType, PgpPrivateKey privKey)
	{
		InitSign(sigType, privKey, null);
	}

	public void InitSign(int sigType, PgpPrivateKey privKey, SecureRandom random)
	{
		this.privKey = privKey;
		signatureType = sigType;
		AsymmetricKeyParameter key = privKey.Key;
		sig = PgpUtilities.CreateSigner(keyAlgorithm, hashAlgorithm, key);
		try
		{
			ICipherParameters cp = key;
			cp = ParameterUtilities.WithRandom(cp, random);
			sig.Init(forSigning: true, cp);
		}
		catch (InvalidKeyException innerException)
		{
			throw new PgpException("invalid key.", innerException);
		}
		dig.Reset();
		lastb = 0;
	}

	public void Update(byte b)
	{
		if (signatureType == 1)
		{
			DoCanonicalUpdateByte(b);
		}
		else
		{
			DoUpdateByte(b);
		}
	}

	private void DoCanonicalUpdateByte(byte b)
	{
		switch (b)
		{
		case 13:
			DoUpdateCRLF();
			break;
		case 10:
			if (lastb != 13)
			{
				DoUpdateCRLF();
			}
			break;
		default:
			DoUpdateByte(b);
			break;
		}
		lastb = b;
	}

	private void DoUpdateCRLF()
	{
		DoUpdateByte(13);
		DoUpdateByte(10);
	}

	private void DoUpdateByte(byte b)
	{
		sig.Update(b);
		dig.Update(b);
	}

	public void Update(params byte[] b)
	{
		Update(b, 0, b.Length);
	}

	public void Update(byte[] b, int off, int len)
	{
		if (signatureType == 1)
		{
			int num = off + len;
			for (int i = off; i != num; i++)
			{
				DoCanonicalUpdateByte(b[i]);
			}
		}
		else
		{
			sig.BlockUpdate(b, off, len);
			dig.BlockUpdate(b, off, len);
		}
	}

	public PgpOnePassSignature GenerateOnePassVersion(bool isNested)
	{
		return new PgpOnePassSignature(new OnePassSignaturePacket(signatureType, hashAlgorithm, keyAlgorithm, privKey.KeyId, isNested));
	}

	public PgpSignature Generate()
	{
		long num = DateTimeUtilities.CurrentUnixMs() / 1000;
		byte[] array = new byte[5]
		{
			(byte)signatureType,
			(byte)(num >> 24),
			(byte)(num >> 16),
			(byte)(num >> 8),
			(byte)num
		};
		sig.BlockUpdate(array, 0, array.Length);
		dig.BlockUpdate(array, 0, array.Length);
		byte[] encoding = sig.GenerateSignature();
		byte[] array2 = DigestUtilities.DoFinal(dig);
		byte[] fingerprint = new byte[2]
		{
			array2[0],
			array2[1]
		};
		MPInteger[] signature = ((keyAlgorithm == PublicKeyAlgorithmTag.RsaSign || keyAlgorithm == PublicKeyAlgorithmTag.RsaGeneral) ? PgpUtilities.RsaSigToMpi(encoding) : PgpUtilities.DsaSigToMpi(encoding));
		return new PgpSignature(new SignaturePacket(3, signatureType, privKey.KeyId, keyAlgorithm, hashAlgorithm, num * 1000, fingerprint, signature));
	}
}
