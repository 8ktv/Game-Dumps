using System;
using System.IO;
using Mirror.BouncyCastle.Math.EC.Rfc8032;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public sealed class Ed25519PrivateKeyParameters : AsymmetricKeyParameter
{
	public static readonly int KeySize = Ed25519.SecretKeySize;

	public static readonly int SignatureSize = Ed25519.SignatureSize;

	private readonly byte[] data = new byte[KeySize];

	private Ed25519PublicKeyParameters cachedPublicKey;

	public Ed25519PrivateKeyParameters(SecureRandom random)
		: base(privateKey: true)
	{
		Ed25519.GeneratePrivateKey(random, data);
	}

	public Ed25519PrivateKeyParameters(byte[] buf)
		: this(Validate(buf), 0)
	{
	}

	public Ed25519PrivateKeyParameters(byte[] buf, int off)
		: base(privateKey: true)
	{
		Array.Copy(buf, off, data, 0, KeySize);
	}

	public Ed25519PrivateKeyParameters(Stream input)
		: base(privateKey: true)
	{
		if (KeySize != Streams.ReadFully(input, data))
		{
			throw new EndOfStreamException("EOF encountered in middle of Ed25519 private key");
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

	public Ed25519PublicKeyParameters GeneratePublicKey()
	{
		return Objects.EnsureSingletonInitialized(ref cachedPublicKey, data, CreatePublicKey);
	}

	public void Sign(Ed25519.Algorithm algorithm, byte[] ctx, byte[] msg, int msgOff, int msgLen, byte[] sig, int sigOff)
	{
		Ed25519PublicKeyParameters ed25519PublicKeyParameters = GeneratePublicKey();
		byte[] array = new byte[Ed25519.PublicKeySize];
		ed25519PublicKeyParameters.Encode(array, 0);
		switch (algorithm)
		{
		case Ed25519.Algorithm.Ed25519:
			if (ctx != null)
			{
				throw new ArgumentOutOfRangeException("ctx");
			}
			Ed25519.Sign(data, 0, array, 0, msg, msgOff, msgLen, sig, sigOff);
			break;
		case Ed25519.Algorithm.Ed25519ctx:
			if (ctx == null)
			{
				throw new ArgumentNullException("ctx");
			}
			if (ctx.Length > 255)
			{
				throw new ArgumentOutOfRangeException("ctx");
			}
			Ed25519.Sign(data, 0, array, 0, ctx, msg, msgOff, msgLen, sig, sigOff);
			break;
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
			Ed25519.SignPrehash(data, 0, array, 0, ctx, msg, msgOff, sig, sigOff);
			break;
		default:
			throw new ArgumentOutOfRangeException("algorithm");
		}
	}

	private static Ed25519PublicKeyParameters CreatePublicKey(byte[] data)
	{
		return new Ed25519PublicKeyParameters(Ed25519.GeneratePublicKey(data, 0));
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
