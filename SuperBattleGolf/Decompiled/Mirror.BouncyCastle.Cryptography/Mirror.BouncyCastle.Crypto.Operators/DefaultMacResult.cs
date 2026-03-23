using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Operators;

public sealed class DefaultMacResult : IBlockResult
{
	private readonly IMac m_mac;

	public DefaultMacResult(IMac mac)
	{
		m_mac = mac;
	}

	public byte[] Collect()
	{
		return MacUtilities.DoFinal(m_mac);
	}

	public int Collect(byte[] buf, int off)
	{
		return m_mac.DoFinal(buf, off);
	}

	public int GetMaxResultLength()
	{
		return m_mac.GetMacSize();
	}
}
