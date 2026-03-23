using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Macs;

public class HMac : IMac
{
	private const byte IPAD = 54;

	private const byte OPAD = 92;

	private readonly IDigest digest;

	private readonly int digestSize;

	private readonly int blockLength;

	private IMemoable ipadState;

	private IMemoable opadState;

	private readonly byte[] inputPad;

	private readonly byte[] outputBuf;

	public virtual string AlgorithmName => digest.AlgorithmName + "/HMAC";

	public HMac(IDigest digest)
		: this(digest, digest.GetByteLength())
	{
	}

	public HMac(IDigest digest, int blockLength)
	{
		if (blockLength < 16)
		{
			throw new ArgumentException("must be at least 16 bytes", "blockLength");
		}
		this.digest = digest;
		digestSize = digest.GetDigestSize();
		this.blockLength = blockLength;
		inputPad = new byte[blockLength];
		outputBuf = new byte[blockLength + digestSize];
	}

	public virtual IDigest GetUnderlyingDigest()
	{
		return digest;
	}

	public virtual void Init(ICipherParameters parameters)
	{
		digest.Reset();
		KeyParameter keyParameter = (KeyParameter)parameters;
		int keyLength = keyParameter.KeyLength;
		if (keyLength > blockLength)
		{
			byte[] key = keyParameter.GetKey();
			digest.BlockUpdate(key, 0, keyLength);
			digest.DoFinal(inputPad, 0);
			keyLength = digestSize;
		}
		else
		{
			keyParameter.CopyTo(inputPad, 0, keyLength);
		}
		Array.Clear(inputPad, keyLength, blockLength - keyLength);
		Array.Copy(inputPad, 0, outputBuf, 0, blockLength);
		XorPad(inputPad, blockLength, 54);
		XorPad(outputBuf, blockLength, 92);
		if (digest is IMemoable memoable)
		{
			opadState = memoable.Copy();
			((IDigest)opadState).BlockUpdate(outputBuf, 0, blockLength);
			digest.BlockUpdate(inputPad, 0, inputPad.Length);
			ipadState = memoable.Copy();
		}
		else
		{
			digest.BlockUpdate(inputPad, 0, inputPad.Length);
		}
	}

	public virtual int GetMacSize()
	{
		return digestSize;
	}

	public virtual void Update(byte input)
	{
		digest.Update(input);
	}

	public virtual void BlockUpdate(byte[] input, int inOff, int len)
	{
		digest.BlockUpdate(input, inOff, len);
	}

	public virtual int DoFinal(byte[] output, int outOff)
	{
		digest.DoFinal(outputBuf, blockLength);
		if (opadState != null)
		{
			((IMemoable)digest).Reset(opadState);
			digest.BlockUpdate(outputBuf, blockLength, digestSize);
		}
		else
		{
			digest.BlockUpdate(outputBuf, 0, outputBuf.Length);
		}
		int result = digest.DoFinal(output, outOff);
		Array.Clear(outputBuf, blockLength, digestSize);
		if (ipadState != null)
		{
			((IMemoable)digest).Reset(ipadState);
			return result;
		}
		digest.BlockUpdate(inputPad, 0, inputPad.Length);
		return result;
	}

	public virtual void Reset()
	{
		if (ipadState != null)
		{
			((IMemoable)digest).Reset(ipadState);
			return;
		}
		digest.Reset();
		digest.BlockUpdate(inputPad, 0, inputPad.Length);
	}

	private static void XorPad(byte[] pad, int len, byte n)
	{
		for (int i = 0; i < len; i++)
		{
			pad[i] ^= n;
		}
	}
}
