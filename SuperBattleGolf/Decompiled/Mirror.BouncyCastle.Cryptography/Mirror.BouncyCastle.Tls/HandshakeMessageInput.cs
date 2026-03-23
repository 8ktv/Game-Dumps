using System;
using System.IO;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public sealed class HandshakeMessageInput : MemoryStream
{
	private readonly int m_offset;

	internal HandshakeMessageInput(byte[] buf, int offset, int length)
		: base(buf, offset, length, writable: false, publiclyVisible: true)
	{
		m_offset = offset;
	}

	public void UpdateHash(TlsHash hash)
	{
		WriteTo(new TlsHashSink(hash));
	}

	internal void UpdateHashPrefix(TlsHash hash, int bindersSize)
	{
		byte[] buffer = GetBuffer();
		int num = Convert.ToInt32(Length);
		hash.Update(buffer, m_offset, num - bindersSize);
	}

	internal void UpdateHashSuffix(TlsHash hash, int bindersSize)
	{
		byte[] buffer = GetBuffer();
		int num = Convert.ToInt32(Length);
		hash.Update(buffer, m_offset + num - bindersSize, bindersSize);
	}
}
