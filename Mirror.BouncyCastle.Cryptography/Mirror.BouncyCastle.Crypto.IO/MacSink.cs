using System;
using System.Threading;
using System.Threading.Tasks;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.IO;

public sealed class MacSink : BaseOutputStream
{
	private readonly IMac m_mac;

	public IMac Mac => m_mac;

	public MacSink(IMac mac)
	{
		m_mac = mac ?? throw new ArgumentNullException("mac");
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (count > 0)
		{
			m_mac.BlockUpdate(buffer, offset, count);
		}
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		return Streams.WriteAsyncDirect(this, buffer, offset, count, cancellationToken);
	}

	public override void WriteByte(byte value)
	{
		m_mac.Update(value);
	}
}
