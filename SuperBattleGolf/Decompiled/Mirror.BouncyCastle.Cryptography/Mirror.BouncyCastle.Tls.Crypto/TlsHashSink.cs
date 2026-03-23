using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Tls.Crypto;

public class TlsHashSink : BaseOutputStream
{
	private readonly TlsHash m_hash;

	public virtual TlsHash Hash => m_hash;

	public TlsHashSink(TlsHash hash)
	{
		m_hash = hash;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (count > 0)
		{
			m_hash.Update(buffer, offset, count);
		}
	}

	public override void WriteByte(byte value)
	{
		m_hash.Update(new byte[1] { value }, 0, 1);
	}
}
