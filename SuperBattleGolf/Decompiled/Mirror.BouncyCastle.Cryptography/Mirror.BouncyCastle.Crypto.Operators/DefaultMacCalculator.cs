using System.IO;
using Mirror.BouncyCastle.Crypto.IO;

namespace Mirror.BouncyCastle.Crypto.Operators;

public sealed class DefaultMacCalculator : IStreamCalculator<IBlockResult>
{
	private readonly MacSink m_macSink;

	public Stream Stream => m_macSink;

	public DefaultMacCalculator(IMac mac)
	{
		m_macSink = new MacSink(mac);
	}

	public IBlockResult GetResult()
	{
		return new DefaultMacResult(m_macSink.Mac);
	}
}
