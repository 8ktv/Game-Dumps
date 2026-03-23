using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls;

public sealed class ByteQueue
{
	private byte[] m_databuf;

	private int m_skipped;

	private int m_available;

	private bool m_readOnlyBuf;

	public int Available => m_available;

	private static int GetAllocationSize(int i)
	{
		return Integers.HighestOneBit((0x100 | i) << 1);
	}

	public ByteQueue()
		: this(0)
	{
	}

	public ByteQueue(int capacity)
	{
		m_databuf = ((capacity == 0) ? TlsUtilities.EmptyBytes : new byte[capacity]);
	}

	public ByteQueue(byte[] buf, int off, int len)
	{
		m_databuf = buf;
		m_skipped = off;
		m_available = len;
		m_readOnlyBuf = true;
	}

	public void AddData(byte[] buf, int off, int len)
	{
		if (m_readOnlyBuf)
		{
			throw new InvalidOperationException("Cannot add data to read-only buffer");
		}
		if (m_available == 0)
		{
			if (len > m_databuf.Length)
			{
				int allocationSize = GetAllocationSize(len);
				m_databuf = new byte[allocationSize];
			}
			m_skipped = 0;
		}
		else if (m_skipped + m_available + len > m_databuf.Length)
		{
			int allocationSize2 = GetAllocationSize(m_available + len);
			if (allocationSize2 > m_databuf.Length)
			{
				byte[] array = new byte[allocationSize2];
				Array.Copy(m_databuf, m_skipped, array, 0, m_available);
				m_databuf = array;
			}
			else
			{
				Array.Copy(m_databuf, m_skipped, m_databuf, 0, m_available);
			}
			m_skipped = 0;
		}
		Array.Copy(buf, off, m_databuf, m_skipped + m_available, len);
		m_available += len;
	}

	public void CopyTo(Stream output, int length)
	{
		if (length > m_available)
		{
			throw new InvalidOperationException("Cannot copy " + length + " bytes, only got " + m_available);
		}
		output.Write(m_databuf, m_skipped, length);
	}

	public void Read(byte[] buf, int offset, int len, int skip)
	{
		if (buf.Length - offset < len)
		{
			throw new ArgumentException("Buffer size of " + buf.Length + " is too small for a read of " + len + " bytes");
		}
		if (m_available - skip < len)
		{
			throw new InvalidOperationException("Not enough data to read");
		}
		Array.Copy(m_databuf, m_skipped + skip, buf, offset, len);
	}

	internal HandshakeMessageInput ReadHandshakeMessage(int length)
	{
		if (length > m_available)
		{
			throw new InvalidOperationException("Cannot read " + length + " bytes, only got " + m_available);
		}
		int skipped = m_skipped;
		m_available -= length;
		m_skipped += length;
		return new HandshakeMessageInput(m_databuf, skipped, length);
	}

	public int ReadInt32()
	{
		if (m_available < 4)
		{
			throw new InvalidOperationException("Not enough data to read");
		}
		return TlsUtilities.ReadInt32(m_databuf, m_skipped);
	}

	public short ReadUint8(int skip)
	{
		if (m_available < skip + 1)
		{
			throw new InvalidOperationException("Not enough data to read");
		}
		return TlsUtilities.ReadUint8(m_databuf, m_skipped + skip);
	}

	public int ReadUint16(int skip)
	{
		if (m_available < skip + 2)
		{
			throw new InvalidOperationException("Not enough data to read");
		}
		return TlsUtilities.ReadUint16(m_databuf, m_skipped + skip);
	}

	public void RemoveData(int i)
	{
		if (i > m_available)
		{
			throw new InvalidOperationException("Cannot remove " + i + " bytes, only got " + m_available);
		}
		m_available -= i;
		m_skipped += i;
	}

	public void RemoveData(byte[] buf, int off, int len, int skip)
	{
		Read(buf, off, len, skip);
		RemoveData(skip + len);
	}

	public byte[] RemoveData(int len, int skip)
	{
		byte[] array = new byte[len];
		RemoveData(array, 0, len, skip);
		return array;
	}

	public void Shrink()
	{
		if (m_available == 0)
		{
			m_databuf = TlsUtilities.EmptyBytes;
			m_skipped = 0;
			return;
		}
		int allocationSize = GetAllocationSize(m_available);
		if (allocationSize < m_databuf.Length)
		{
			byte[] array = new byte[allocationSize];
			Array.Copy(m_databuf, m_skipped, array, 0, m_available);
			m_databuf = array;
			m_skipped = 0;
		}
	}
}
