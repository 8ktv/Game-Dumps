using System;

namespace Mirror.BouncyCastle.Crypto;

public sealed class SimpleBlockResult : IBlockResult
{
	private readonly byte[] result;

	public SimpleBlockResult(byte[] result)
	{
		this.result = result;
	}

	public byte[] Collect()
	{
		return result;
	}

	public int Collect(byte[] buf, int off)
	{
		Array.Copy(result, 0, buf, off, result.Length);
		return result.Length;
	}

	public int GetMaxResultLength()
	{
		return result.Length;
	}
}
