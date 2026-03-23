using System;
using System.IO;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math.EC.Rfc8032;

namespace Mirror.BouncyCastle.Crypto.Signers;

public class Ed25519Signer : ISigner
{
	private sealed class Buffer : MemoryStream
	{
		internal byte[] GenerateSignature(Ed25519PrivateKeyParameters privateKey)
		{
			lock (this)
			{
				byte[] buffer = GetBuffer();
				int msgLen = Convert.ToInt32(Length);
				byte[] array = new byte[Ed25519PrivateKeyParameters.SignatureSize];
				privateKey.Sign(Ed25519.Algorithm.Ed25519, null, buffer, 0, msgLen, array, 0);
				Reset();
				return array;
			}
		}

		internal bool VerifySignature(Ed25519PublicKeyParameters publicKey, byte[] signature)
		{
			if (Ed25519.SignatureSize != signature.Length)
			{
				Reset();
				return false;
			}
			lock (this)
			{
				byte[] buffer = GetBuffer();
				int msgLen = Convert.ToInt32(Length);
				bool result = publicKey.Verify(Ed25519.Algorithm.Ed25519, null, buffer, 0, msgLen, signature, 0);
				Reset();
				return result;
			}
		}

		internal void Reset()
		{
			lock (this)
			{
				int length = Convert.ToInt32(Length);
				Array.Clear(GetBuffer(), 0, length);
				SetLength(0L);
			}
		}
	}

	private readonly Buffer buffer = new Buffer();

	private bool forSigning;

	private Ed25519PrivateKeyParameters privateKey;

	private Ed25519PublicKeyParameters publicKey;

	public virtual string AlgorithmName => "Ed25519";

	public virtual void Init(bool forSigning, ICipherParameters parameters)
	{
		this.forSigning = forSigning;
		if (forSigning)
		{
			privateKey = (Ed25519PrivateKeyParameters)parameters;
			publicKey = null;
		}
		else
		{
			privateKey = null;
			publicKey = (Ed25519PublicKeyParameters)parameters;
		}
		Reset();
	}

	public virtual void Update(byte b)
	{
		buffer.WriteByte(b);
	}

	public virtual void BlockUpdate(byte[] buf, int off, int len)
	{
		buffer.Write(buf, off, len);
	}

	public virtual int GetMaxSignatureSize()
	{
		return Ed25519.SignatureSize;
	}

	public virtual byte[] GenerateSignature()
	{
		if (!forSigning || privateKey == null)
		{
			throw new InvalidOperationException("Ed25519Signer not initialised for signature generation.");
		}
		return buffer.GenerateSignature(privateKey);
	}

	public virtual bool VerifySignature(byte[] signature)
	{
		if (forSigning || publicKey == null)
		{
			throw new InvalidOperationException("Ed25519Signer not initialised for verification");
		}
		return buffer.VerifySignature(publicKey, signature);
	}

	public virtual void Reset()
	{
		buffer.Reset();
	}
}
