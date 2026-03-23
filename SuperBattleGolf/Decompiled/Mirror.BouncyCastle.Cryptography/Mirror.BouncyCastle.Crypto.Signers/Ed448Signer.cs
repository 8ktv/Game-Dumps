using System;
using System.IO;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math.EC.Rfc8032;

namespace Mirror.BouncyCastle.Crypto.Signers;

public class Ed448Signer : ISigner
{
	private sealed class Buffer : MemoryStream
	{
		internal byte[] GenerateSignature(Ed448PrivateKeyParameters privateKey, byte[] ctx)
		{
			lock (this)
			{
				byte[] buffer = GetBuffer();
				int msgLen = Convert.ToInt32(Length);
				byte[] array = new byte[Ed448PrivateKeyParameters.SignatureSize];
				privateKey.Sign(Ed448.Algorithm.Ed448, ctx, buffer, 0, msgLen, array, 0);
				Reset();
				return array;
			}
		}

		internal bool VerifySignature(Ed448PublicKeyParameters publicKey, byte[] ctx, byte[] signature)
		{
			if (Ed448.SignatureSize != signature.Length)
			{
				Reset();
				return false;
			}
			lock (this)
			{
				byte[] buffer = GetBuffer();
				int msgLen = Convert.ToInt32(Length);
				bool result = publicKey.Verify(Ed448.Algorithm.Ed448, ctx, buffer, 0, msgLen, signature, 0);
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

	private readonly byte[] context;

	private bool forSigning;

	private Ed448PrivateKeyParameters privateKey;

	private Ed448PublicKeyParameters publicKey;

	public virtual string AlgorithmName => "Ed448";

	public Ed448Signer(byte[] context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		this.context = (byte[])context.Clone();
	}

	public virtual void Init(bool forSigning, ICipherParameters parameters)
	{
		this.forSigning = forSigning;
		if (forSigning)
		{
			privateKey = (Ed448PrivateKeyParameters)parameters;
			publicKey = null;
		}
		else
		{
			privateKey = null;
			publicKey = (Ed448PublicKeyParameters)parameters;
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
		return Ed448.SignatureSize;
	}

	public virtual byte[] GenerateSignature()
	{
		if (!forSigning || privateKey == null)
		{
			throw new InvalidOperationException("Ed448Signer not initialised for signature generation.");
		}
		return buffer.GenerateSignature(privateKey, context);
	}

	public virtual bool VerifySignature(byte[] signature)
	{
		if (forSigning || publicKey == null)
		{
			throw new InvalidOperationException("Ed448Signer not initialised for verification");
		}
		return buffer.VerifySignature(publicKey, context, signature);
	}

	public virtual void Reset()
	{
		buffer.Reset();
	}
}
