using System;
using System.IO;
using Mirror.BouncyCastle.Math.EC.Rfc8032;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public sealed class Ed448PublicKeyParameters : AsymmetricKeyParameter
{
	public static readonly int KeySize = Ed448.PublicKeySize;

	private readonly Ed448.PublicPoint m_publicPoint;

	public Ed448PublicKeyParameters(byte[] buf)
		: this(Validate(buf), 0)
	{
	}

	public Ed448PublicKeyParameters(byte[] buf, int off)
		: base(privateKey: false)
	{
		m_publicPoint = Parse(buf, off);
	}

	public Ed448PublicKeyParameters(Stream input)
		: base(privateKey: false)
	{
		byte[] buf = new byte[KeySize];
		if (KeySize != Streams.ReadFully(input, buf))
		{
			throw new EndOfStreamException("EOF encountered in middle of Ed448 public key");
		}
		m_publicPoint = Parse(buf, 0);
	}

	public Ed448PublicKeyParameters(Ed448.PublicPoint publicPoint)
		: base(privateKey: false)
	{
		m_publicPoint = publicPoint ?? throw new ArgumentNullException("publicPoint");
	}

	public void Encode(byte[] buf, int off)
	{
		Ed448.EncodePublicPoint(m_publicPoint, buf, off);
	}

	public byte[] GetEncoded()
	{
		byte[] array = new byte[KeySize];
		Encode(array, 0);
		return array;
	}

	public bool Verify(Ed448.Algorithm algorithm, byte[] ctx, byte[] msg, int msgOff, int msgLen, byte[] sig, int sigOff)
	{
		switch (algorithm)
		{
		case Ed448.Algorithm.Ed448:
			if (ctx == null)
			{
				throw new ArgumentNullException("ctx");
			}
			if (ctx.Length > 255)
			{
				throw new ArgumentOutOfRangeException("ctx");
			}
			return Ed448.Verify(sig, sigOff, m_publicPoint, ctx, msg, msgOff, msgLen);
		case Ed448.Algorithm.Ed448ph:
			if (ctx == null)
			{
				throw new ArgumentNullException("ctx");
			}
			if (ctx.Length > 255)
			{
				throw new ArgumentOutOfRangeException("ctx");
			}
			if (Ed448.PrehashSize != msgLen)
			{
				throw new ArgumentOutOfRangeException("msgLen");
			}
			return Ed448.VerifyPrehash(sig, sigOff, m_publicPoint, ctx, msg, msgOff);
		default:
			throw new ArgumentOutOfRangeException("algorithm");
		}
	}

	private static Ed448.PublicPoint Parse(byte[] buf, int off)
	{
		return Ed448.ValidatePublicKeyPartialExport(buf, off) ?? throw new ArgumentException("invalid public key");
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
