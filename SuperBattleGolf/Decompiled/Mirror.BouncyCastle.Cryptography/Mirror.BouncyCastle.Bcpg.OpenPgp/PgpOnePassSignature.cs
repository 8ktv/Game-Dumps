using System;
using System.IO;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpOnePassSignature
{
	private readonly OnePassSignaturePacket sigPack;

	private readonly int signatureType;

	private ISigner sig;

	private byte lastb;

	public long KeyId => sigPack.KeyId;

	public int SignatureType => sigPack.SignatureType;

	public HashAlgorithmTag HashAlgorithm => sigPack.HashAlgorithm;

	public PublicKeyAlgorithmTag KeyAlgorithm => sigPack.KeyAlgorithm;

	private static OnePassSignaturePacket Cast(Packet packet)
	{
		if (packet is OnePassSignaturePacket result)
		{
			return result;
		}
		throw new IOException("unexpected packet in stream: " + packet);
	}

	internal PgpOnePassSignature(BcpgInputStream bcpgInput)
		: this(Cast(bcpgInput.ReadPacket()))
	{
	}

	internal PgpOnePassSignature(OnePassSignaturePacket sigPack)
	{
		this.sigPack = sigPack;
		signatureType = sigPack.SignatureType;
	}

	public void InitVerify(PgpPublicKey pubKey)
	{
		lastb = 0;
		AsymmetricKeyParameter key = pubKey.GetKey();
		try
		{
			sig = PgpUtilities.CreateSigner(sigPack.KeyAlgorithm, sigPack.HashAlgorithm, key);
		}
		catch (Exception innerException)
		{
			throw new PgpException("can't set up signature object.", innerException);
		}
		try
		{
			sig.Init(forSigning: false, key);
		}
		catch (InvalidKeyException innerException2)
		{
			throw new PgpException("invalid key.", innerException2);
		}
	}

	public void Update(byte b)
	{
		if (signatureType == 1)
		{
			DoCanonicalUpdateByte(b);
		}
		else
		{
			sig.Update(b);
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
			sig.Update(b);
			break;
		}
		lastb = b;
	}

	private void DoUpdateCRLF()
	{
		sig.Update(13);
		sig.Update(10);
	}

	public void Update(params byte[] bytes)
	{
		Update(bytes, 0, bytes.Length);
	}

	public void Update(byte[] bytes, int off, int length)
	{
		if (signatureType == 1)
		{
			int num = off + length;
			for (int i = off; i != num; i++)
			{
				DoCanonicalUpdateByte(bytes[i]);
			}
		}
		else
		{
			sig.BlockUpdate(bytes, off, length);
		}
	}

	public bool Verify(PgpSignature pgpSig)
	{
		byte[] signatureTrailer = pgpSig.GetSignatureTrailer();
		sig.BlockUpdate(signatureTrailer, 0, signatureTrailer.Length);
		return sig.VerifySignature(pgpSig.GetSignature());
	}

	public byte[] GetEncoded()
	{
		MemoryStream memoryStream = new MemoryStream();
		Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public void Encode(Stream outStr)
	{
		BcpgOutputStream.Wrap(outStr).WritePacket(sigPack);
	}
}
