using System;
using System.IO;
using Mirror.BouncyCastle.Math.EC.Rfc8032;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public sealed class Ed25519PublicKeyParameters : AsymmetricKeyParameter
{
	public static readonly int KeySize = Ed25519.PublicKeySize;

	private readonly Ed25519.PublicPoint m_publicPoint;

	public Ed25519PublicKeyParameters(byte[] buf)
		: this(Validate(buf), 0)
	{
	}

	public Ed25519PublicKeyParameters(byte[] buf, int off)
		: base(privateKey: false)
	{
		m_publicPoint = Parse(buf, off);
	}

	public Ed25519PublicKeyParameters(Stream input)
		: base(privateKey: false)
	{
		byte[] buf = new byte[KeySize];
		if (KeySize != Streams.ReadFully(input, buf))
		{
			throw new EndOfStreamException("EOF encountered in middle of Ed25519 public key");
		}
		m_publicPoint = Parse(buf, 0);
	}

	public Ed25519PublicKeyParameters(Ed25519.PublicPoint publicPoint)
		: base(privateKey: false)
	{
		m_publicPoint = publicPoint ?? throw new ArgumentNullException("publicPoint");
	}

	public void Encode(byte[] buf, int off)
	{
		Ed25519.EncodePublicPoint(m_publicPoint, buf, off);
	}

	public byte[] GetEncoded()
	{
		byte[] array = new byte[KeySize];
		Encode(array, 0);
		return array;
	}

	public bool Verify(Ed25519.Algorithm algorithm, byte[] ctx, byte[] msg, int msgOff, int msgLen, byte[] sig, int sigOff)
	{
		switch (algorithm)
		{
		case Ed25519.Algorithm.Ed25519:
			if (ctx != null)
			{
				throw new ArgumentOutOfRangeException("ctx");
			}
			return Ed25519.Verify(sig, sigOff, m_publicPoint, msg, msgOff, msgLen);
		case Ed25519.Algorithm.Ed25519ctx:
			if (ctx == null)
			{
				throw new ArgumentNullException("ctx");
			}
			if (ctx.Length > 255)
			{
				throw new ArgumentOutOfRangeException("ctx");
			}
			return Ed25519.Verify(sig, sigOff, m_publicPoint, ctx, msg, msgOff, msgLen);
		case Ed25519.Algorithm.Ed25519ph:
			if (ctx == null)
			{
				throw new ArgumentNullException("ctx");
			}
			if (ctx.Length > 255)
			{
				throw new ArgumentOutOfRangeException("ctx");
			}
			if (Ed25519.PrehashSize != msgLen)
			{
				throw new ArgumentOutOfRangeException("msgLen");
			}
			return Ed25519.VerifyPrehash(sig, sigOff, m_publicPoint, ctx, msg, msgOff);
		default:
			throw new ArgumentOutOfRangeException("algorithm");
		}
	}

	private static Ed25519.PublicPoint Parse(byte[] buf, int off)
	{
		return Ed25519.ValidatePublicKeyPartialExport(buf, off) ?? throw new ArgumentException("invalid public key");
	}

	private static byte[] Validate(byte[] buf)
	{
		if (buf.Length != KeySize)
		{
			int keySize = KeySize;
			throw new ArgumentException("must have length " + keySize, "buf");
		}
		return buf;
	}
}
