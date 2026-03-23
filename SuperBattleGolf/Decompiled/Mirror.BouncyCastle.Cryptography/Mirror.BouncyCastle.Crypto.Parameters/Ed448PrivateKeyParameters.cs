using System;
using System.IO;
using Mirror.BouncyCastle.Math.EC.Rfc8032;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public sealed class Ed448PrivateKeyParameters : AsymmetricKeyParameter
{
	public static readonly int KeySize = Ed448.SecretKeySize;

	public static readonly int SignatureSize = Ed448.SignatureSize;

	private readonly byte[] data = new byte[KeySize];

	private Ed448PublicKeyParameters cachedPublicKey;

	public Ed448PrivateKeyParameters(SecureRandom random)
		: base(privateKey: true)
	{
		Ed448.GeneratePrivateKey(random, data);
	}

	public Ed448PrivateKeyParameters(byte[] buf)
		: this(Validate(buf), 0)
	{
	}

	public Ed448PrivateKeyParameters(byte[] buf, int off)
		: base(privateKey: true)
	{
		Array.Copy(buf, off, data, 0, KeySize);
	}

	public Ed448PrivateKeyParameters(Stream input)
		: base(privateKey: true)
	{
		if (KeySize != Streams.ReadFully(input, data))
		{
			throw new EndOfStreamException("EOF encountered in middle of Ed448 private key");
		}
	}

	public void Encode(byte[] buf, int off)
	{
		Array.Copy(data, 0, buf, off, KeySize);
	}

	public byte[] GetEncoded()
	{
		return Arrays.Clone(data);
	}

	public Ed448PublicKeyParameters GeneratePublicKey()
	{
		return Objects.EnsureSingletonInitialized(ref cachedPublicKey, data, CreatePublicKey);
	}

	public void Sign(Ed448.Algorithm algorithm, byte[] ctx, byte[] msg, int msgOff, int msgLen, byte[] sig, int sigOff)
	{
		Ed448PublicKeyParameters ed448PublicKeyParameters = GeneratePublicKey();
		byte[] array = new byte[Ed448.PublicKeySize];
		ed448PublicKeyParameters.Encode(array, 0);
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
			Ed448.Sign(data, 0, array, 0, ctx, msg, msgOff, msgLen, sig, sigOff);
			break;
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
			Ed448.SignPrehash(data, 0, array, 0, ctx, msg, msgOff, sig, sigOff);
			break;
		default:
			throw new ArgumentOutOfRangeException("algorithm");
		}
	}

	private static Ed448PublicKeyParameters CreatePublicKey(byte[] data)
	{
		return new Ed448PublicKeyParameters(Ed448.GeneratePublicKey(data, 0));
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
