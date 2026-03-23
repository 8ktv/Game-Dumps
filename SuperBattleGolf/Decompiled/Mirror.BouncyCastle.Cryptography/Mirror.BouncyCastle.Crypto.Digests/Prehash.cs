using System;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class Prehash : IDigest
{
	private readonly string m_algorithmName;

	private readonly LimitedBuffer m_buf;

	public string AlgorithmName => m_algorithmName;

	public static Prehash ForDigest(IDigest digest)
	{
		return new Prehash(digest);
	}

	private Prehash(IDigest digest)
	{
		m_algorithmName = digest.AlgorithmName;
		m_buf = new LimitedBuffer(digest.GetDigestSize());
	}

	public int GetByteLength()
	{
		throw new NotSupportedException();
	}

	public int GetDigestSize()
	{
		return m_buf.Limit;
	}

	public void Update(byte input)
	{
		m_buf.WriteByte(input);
	}

	public void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		m_buf.Write(input, inOff, inLen);
	}

	public int DoFinal(byte[] output, int outOff)
	{
		try
		{
			if (GetDigestSize() != m_buf.Count)
			{
				throw new InvalidOperationException("Incorrect prehash size");
			}
			return m_buf.CopyTo(output, outOff);
		}
		finally
		{
			Reset();
		}
	}

	public void Reset()
	{
		m_buf.Reset();
	}
}
