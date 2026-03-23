using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Tls.Crypto;

public class TlsMacSink : BaseOutputStream
{
	private readonly TlsMac m_mac;

	public virtual TlsMac Mac => m_mac;

	public TlsMacSink(TlsMac mac)
	{
		m_mac = mac;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (count > 0)
		{
			m_mac.Update(buffer, offset, count);
		}
	}

	public override void WriteByte(byte value)
	{
		m_mac.Update(new byte[1] { value }, 0, 1);
	}
}
