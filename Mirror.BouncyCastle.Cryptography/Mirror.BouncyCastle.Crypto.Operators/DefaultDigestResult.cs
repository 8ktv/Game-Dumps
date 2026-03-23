using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Operators;

public sealed class DefaultDigestResult : IBlockResult
{
	private readonly IDigest m_digest;

	public DefaultDigestResult(IDigest digest)
	{
		m_digest = digest;
	}

	public byte[] Collect()
	{
		return DigestUtilities.DoFinal(m_digest);
	}

	public int Collect(byte[] buf, int off)
	{
		return m_digest.DoFinal(buf, off);
	}

	public int GetMaxResultLength()
	{
		return m_digest.GetDigestSize();
	}
}
