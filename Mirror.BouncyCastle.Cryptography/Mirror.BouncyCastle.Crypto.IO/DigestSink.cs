using System;
using System.Threading;
using System.Threading.Tasks;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.IO;

public sealed class DigestSink : BaseOutputStream
{
	private readonly IDigest m_digest;

	public IDigest Digest => m_digest;

	public DigestSink(IDigest digest)
	{
		m_digest = digest ?? throw new ArgumentNullException("digest");
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (count > 0)
		{
			m_digest.BlockUpdate(buffer, offset, count);
		}
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		return Streams.WriteAsyncDirect(this, buffer, offset, count, cancellationToken);
	}

	public override void WriteByte(byte value)
	{
		m_digest.Update(value);
	}
}
