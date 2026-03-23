using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public sealed class MgfParameters : IDerivationParameters
{
	private readonly byte[] m_seed;

	public int SeedLength => m_seed.Length;

	public MgfParameters(byte[] seed)
		: this(seed, 0, seed.Length)
	{
	}

	public MgfParameters(byte[] seed, int off, int len)
	{
		m_seed = Arrays.CopyOfRange(seed, off, len);
	}

	public byte[] GetSeed()
	{
		return (byte[])m_seed.Clone();
	}

	public void GetSeed(byte[] buffer, int offset)
	{
		m_seed.CopyTo(buffer, offset);
	}
}
