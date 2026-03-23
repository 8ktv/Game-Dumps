using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Tls;

public sealed class ByteQueueOutputStream : BaseOutputStream
{
	private readonly ByteQueue m_buffer;

	public ByteQueue Buffer => m_buffer;

	public ByteQueueOutputStream()
	{
		m_buffer = new ByteQueue();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		m_buffer.AddData(buffer, offset, count);
	}

	public override void WriteByte(byte value)
	{
		m_buffer.AddData(new byte[1] { value }, 0, 1);
	}
}
