using System;
using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Bcpg;

public class BcpgOutputStream : BaseOutputStream
{
	private Stream outStr;

	private bool useOldFormat;

	private byte[] partialBuffer;

	private int partialBufferLength;

	private int partialPower;

	private int partialOffset;

	private const int BufferSizePower = 16;

	internal static BcpgOutputStream Wrap(Stream outStr)
	{
		if (outStr is BcpgOutputStream result)
		{
			return result;
		}
		return new BcpgOutputStream(outStr);
	}

	public BcpgOutputStream(Stream outStr)
		: this(outStr, newFormatOnly: false)
	{
	}

	public BcpgOutputStream(Stream outStr, bool newFormatOnly)
	{
		this.outStr = outStr ?? throw new ArgumentNullException("outStr");
		useOldFormat = !newFormatOnly;
	}

	public BcpgOutputStream(Stream outStr, PacketTag tag)
	{
		this.outStr = outStr ?? throw new ArgumentNullException("outStr");
		WriteHeader(tag, oldPackets: true, partial: true, 0L);
	}

	public BcpgOutputStream(Stream outStr, PacketTag tag, long length, bool oldFormat)
	{
		this.outStr = outStr ?? throw new ArgumentNullException("outStr");
		if (length > uint.MaxValue)
		{
			WriteHeader(tag, oldPackets: false, partial: true, 0L);
			partialBufferLength = 65536;
			partialBuffer = new byte[partialBufferLength];
			partialPower = 16;
			partialOffset = 0;
		}
		else
		{
			WriteHeader(tag, oldFormat, partial: false, length);
		}
	}

	public BcpgOutputStream(Stream outStr, PacketTag tag, long length)
	{
		this.outStr = outStr ?? throw new ArgumentNullException("outStr");
		WriteHeader(tag, oldPackets: false, partial: false, length);
	}

	public BcpgOutputStream(Stream outStr, PacketTag tag, byte[] buffer)
	{
		this.outStr = outStr ?? throw new ArgumentNullException("outStr");
		WriteHeader(tag, oldPackets: false, partial: true, 0L);
		partialBuffer = buffer;
		uint num = (uint)partialBuffer.Length;
		partialPower = 0;
		while (num != 1)
		{
			num >>= 1;
			partialPower++;
		}
		if (partialPower > 30)
		{
			throw new IOException("Buffer cannot be greater than 2^30 in length.");
		}
		partialBufferLength = 1 << partialPower;
		partialOffset = 0;
	}

	private void WriteNewPacketLength(long bodyLen)
	{
		if (bodyLen < 192)
		{
			outStr.WriteByte((byte)bodyLen);
		}
		else if (bodyLen <= 8383)
		{
			bodyLen -= 192;
			outStr.WriteByte((byte)(((bodyLen >> 8) & 0xFF) + 192));
			outStr.WriteByte((byte)bodyLen);
		}
		else
		{
			outStr.WriteByte(byte.MaxValue);
			outStr.WriteByte((byte)(bodyLen >> 24));
			outStr.WriteByte((byte)(bodyLen >> 16));
			outStr.WriteByte((byte)(bodyLen >> 8));
			outStr.WriteByte((byte)bodyLen);
		}
	}

	private void WriteHeader(PacketTag packetTag, bool oldPackets, bool partial, long bodyLen)
	{
		int num = 128;
		if (partialBuffer != null)
		{
			PartialFlushLast();
			partialBuffer = null;
		}
		if (packetTag <= (PacketTag)15 && oldPackets)
		{
			num |= (int)packetTag << 2;
			if (partial)
			{
				WriteByte((byte)(num | 3));
			}
			else if (bodyLen <= 255)
			{
				WriteByte((byte)num);
				WriteByte((byte)bodyLen);
			}
			else if (bodyLen <= 65535)
			{
				WriteByte((byte)(num | 1));
				WriteByte((byte)(bodyLen >> 8));
				WriteByte((byte)bodyLen);
			}
			else
			{
				WriteByte((byte)(num | 2));
				WriteByte((byte)(bodyLen >> 24));
				WriteByte((byte)(bodyLen >> 16));
				WriteByte((byte)(bodyLen >> 8));
				WriteByte((byte)bodyLen);
			}
		}
		else
		{
			num |= (int)((PacketTag)64 | packetTag);
			WriteByte((byte)num);
			if (partial)
			{
				partialOffset = 0;
			}
			else
			{
				WriteNewPacketLength(bodyLen);
			}
		}
	}

	private void PartialFlush()
	{
		outStr.WriteByte((byte)(0xE0 | partialPower));
		outStr.Write(partialBuffer, 0, partialBufferLength);
		partialOffset = 0;
	}

	private void PartialFlushLast()
	{
		WriteNewPacketLength(partialOffset);
		outStr.Write(partialBuffer, 0, partialOffset);
		partialOffset = 0;
	}

	private void PartialWrite(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (partialOffset == partialBufferLength)
		{
			PartialFlush();
		}
		if (count <= partialBufferLength - partialOffset)
		{
			Array.Copy(buffer, offset, partialBuffer, partialOffset, count);
			partialOffset += count;
			return;
		}
		int num = partialBufferLength - partialOffset;
		Array.Copy(buffer, offset, partialBuffer, partialOffset, num);
		offset += num;
		count -= num;
		PartialFlush();
		while (count > partialBufferLength)
		{
			Array.Copy(buffer, offset, partialBuffer, 0, partialBufferLength);
			offset += partialBufferLength;
			count -= partialBufferLength;
			PartialFlush();
		}
		Array.Copy(buffer, offset, partialBuffer, 0, count);
		partialOffset = count;
	}

	private void PartialWriteByte(byte value)
	{
		if (partialOffset == partialBufferLength)
		{
			PartialFlush();
		}
		partialBuffer[partialOffset++] = value;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (partialBuffer != null)
		{
			PartialWrite(buffer, offset, count);
		}
		else
		{
			outStr.Write(buffer, offset, count);
		}
	}

	public override void WriteByte(byte value)
	{
		if (partialBuffer != null)
		{
			PartialWriteByte(value);
		}
		else
		{
			outStr.WriteByte(value);
		}
	}

	internal virtual void WriteShort(short n)
	{
		Write((byte)(n >> 8), (byte)n);
	}

	internal virtual void WriteInt(int n)
	{
		Write((byte)(n >> 24), (byte)(n >> 16), (byte)(n >> 8), (byte)n);
	}

	internal virtual void WriteLong(long n)
	{
		Write((byte)(n >> 56), (byte)(n >> 48), (byte)(n >> 40), (byte)(n >> 32), (byte)(n >> 24), (byte)(n >> 16), (byte)(n >> 8), (byte)n);
	}

	public void WritePacket(ContainedPacket p)
	{
		p.Encode(this);
	}

	internal void WritePacket(PacketTag tag, byte[] body)
	{
		WritePacket(tag, body, useOldFormat);
	}

	internal void WritePacket(PacketTag tag, byte[] body, bool oldFormat)
	{
		WriteHeader(tag, oldFormat, partial: false, body.Length);
		Write(body);
	}

	public void WriteObject(BcpgObject bcpgObject)
	{
		bcpgObject.Encode(this);
	}

	public void WriteObjects(params BcpgObject[] v)
	{
		for (int i = 0; i < v.Length; i++)
		{
			v[i].Encode(this);
		}
	}

	public override void Flush()
	{
		outStr.Flush();
	}

	public void Finish()
	{
		if (partialBuffer != null)
		{
			PartialFlushLast();
			Array.Clear(partialBuffer, 0, partialBuffer.Length);
			partialBuffer = null;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			Finish();
			outStr.Flush();
			outStr.Dispose();
		}
		base.Dispose(disposing);
	}
}
