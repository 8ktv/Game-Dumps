using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class Blake2xsDigest : IXof, IDigest
{
	public const int UnknownDigestLength = 65535;

	private const int DigestLength = 32;

	private const long MaxNumberBlocks = 4294967296L;

	private int digestLength;

	private Blake2sDigest hash;

	private byte[] h0;

	private byte[] buf = new byte[32];

	private int bufPos = 32;

	private int digestPos;

	private long blockPos;

	private long nodeOffset;

	public string AlgorithmName => "BLAKE2xs";

	public Blake2xsDigest()
		: this(65535)
	{
	}

	public Blake2xsDigest(int digestBytes)
		: this(digestBytes, null, null, null)
	{
	}

	public Blake2xsDigest(int digestBytes, byte[] key)
		: this(digestBytes, key, null, null)
	{
	}

	public Blake2xsDigest(int digestBytes, byte[] key, byte[] salt, byte[] personalization)
	{
		if (digestBytes < 1 || digestBytes > 65535)
		{
			throw new ArgumentException("BLAKE2xs digest length must be between 1 and 2^16-1");
		}
		digestLength = digestBytes;
		nodeOffset = ComputeNodeOffset();
		hash = new Blake2sDigest(32, key, salt, personalization, nodeOffset);
	}

	public Blake2xsDigest(Blake2xsDigest digest)
	{
		digestLength = digest.digestLength;
		hash = new Blake2sDigest(digest.hash);
		h0 = Arrays.Clone(digest.h0);
		buf = Arrays.Clone(digest.buf);
		bufPos = digest.bufPos;
		digestPos = digest.digestPos;
		blockPos = digest.blockPos;
		nodeOffset = digest.nodeOffset;
	}

	public int GetDigestSize()
	{
		return digestLength;
	}

	public int GetByteLength()
	{
		return hash.GetByteLength();
	}

	public long GetUnknownMaxLength()
	{
		return 137438953472L;
	}

	public void Update(byte b)
	{
		hash.Update(b);
	}

	public void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		hash.BlockUpdate(input, inOff, inLen);
	}

	public void Reset()
	{
		hash.Reset();
		h0 = null;
		bufPos = 32;
		digestPos = 0;
		blockPos = 0L;
		nodeOffset = ComputeNodeOffset();
	}

	public int DoFinal(byte[] output, int outOff)
	{
		return OutputFinal(output, outOff, digestLength);
	}

	public int OutputFinal(byte[] output, int outOff, int outLen)
	{
		int result = Output(output, outOff, outLen);
		Reset();
		return result;
	}

	public int Output(byte[] output, int outOff, int outLen)
	{
		Check.OutputLength(output, outOff, outLen, "output buffer too short");
		if (h0 == null)
		{
			h0 = new byte[hash.GetDigestSize()];
			hash.DoFinal(h0, 0);
		}
		if (digestLength != 65535)
		{
			if (digestPos + outLen > digestLength)
			{
				throw new ArgumentException("Output length is above the digest length");
			}
		}
		else if (blockPos << 5 >= GetUnknownMaxLength())
		{
			throw new ArgumentException("Maximum length is 2^32 blocks of 32 bytes");
		}
		for (int i = 0; i < outLen; i++)
		{
			if (bufPos >= 32)
			{
				Blake2sDigest blake2sDigest = new Blake2sDigest(ComputeStepLength(), 32, nodeOffset);
				blake2sDigest.BlockUpdate(h0, 0, h0.Length);
				Arrays.Fill(buf, 0);
				blake2sDigest.DoFinal(buf, 0);
				bufPos = 0;
				nodeOffset++;
				blockPos++;
			}
			output[outOff + i] = buf[bufPos];
			bufPos++;
			digestPos++;
		}
		return outLen;
	}

	private int ComputeStepLength()
	{
		if (digestLength == 65535)
		{
			return 32;
		}
		return System.Math.Min(32, digestLength - digestPos);
	}

	private long ComputeNodeOffset()
	{
		return digestLength * 4294967296L;
	}
}
