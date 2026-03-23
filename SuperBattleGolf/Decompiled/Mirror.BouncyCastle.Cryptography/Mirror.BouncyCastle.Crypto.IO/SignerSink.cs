using System;
using System.Threading;
using System.Threading.Tasks;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.IO;

public sealed class SignerSink : BaseOutputStream
{
	private readonly ISigner m_signer;

	public ISigner Signer => m_signer;

	public SignerSink(ISigner signer)
	{
		m_signer = signer ?? throw new ArgumentNullException("signer");
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (count > 0)
		{
			m_signer.BlockUpdate(buffer, offset, count);
		}
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		return Streams.WriteAsyncDirect(this, buffer, offset, count, cancellationToken);
	}

	public override void WriteByte(byte value)
	{
		m_signer.Update(value);
	}
}
