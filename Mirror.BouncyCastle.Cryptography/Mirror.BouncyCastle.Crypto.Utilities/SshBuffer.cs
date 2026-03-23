using System;
using System.Text;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Utilities;

internal class SshBuffer
{
	private readonly byte[] buffer;

	private int pos;

	internal SshBuffer(byte[] magic, byte[] buffer)
	{
		this.buffer = buffer;
		for (int i = 0; i != magic.Length; i++)
		{
			if (magic[i] != buffer[i])
			{
				throw new ArgumentException("magic-number incorrect");
			}
		}
		pos += magic.Length;
	}

	internal SshBuffer(byte[] buffer)
	{
		this.buffer = buffer;
	}

	public int ReadU32()
	{
		if (pos > buffer.Length - 4)
		{
			throw new ArgumentOutOfRangeException("4 bytes for U32 exceeds buffer.");
		}
		uint result = Pack.BE_To_UInt32(buffer, pos);
		pos += 4;
		return (int)result;
	}

	public string ReadStringAscii()
	{
		return Encoding.ASCII.GetString(ReadBlock());
	}

	public string ReadStringUtf8()
	{
		return Encoding.UTF8.GetString(ReadBlock());
	}

	public byte[] ReadBlock()
	{
		int num = ReadU32();
		if (num == 0)
		{
			return Arrays.EmptyBytes;
		}
		if (pos > buffer.Length - num)
		{
			throw new InvalidOperationException("not enough data for block");
		}
		int num2 = pos;
		pos += num;
		return Arrays.CopyOfRange(buffer, num2, pos);
	}

	public void SkipBlock()
	{
		int num = ReadU32();
		if (pos > buffer.Length - num)
		{
			throw new InvalidOperationException("not enough data for block");
		}
		pos += num;
	}

	public byte[] ReadPaddedBlock()
	{
		return ReadPaddedBlock(8);
	}

	public byte[] ReadPaddedBlock(int blockSize)
	{
		int num = ReadU32();
		if (num == 0)
		{
			return Arrays.EmptyBytes;
		}
		if (pos > buffer.Length - num)
		{
			throw new InvalidOperationException("not enough data for block");
		}
		if (num % blockSize != 0)
		{
			throw new InvalidOperationException("missing padding");
		}
		int num2 = pos;
		pos += num;
		int num3 = pos;
		if (num > 0)
		{
			int num4 = buffer[pos - 1] & 0xFF;
			if (0 < num4 && num4 < blockSize)
			{
				int num5 = num4;
				num3 -= num5;
				int num6 = 1;
				int num7 = num3;
				while (num6 <= num5)
				{
					if (num6 != (buffer[num7] & 0xFF))
					{
						throw new InvalidOperationException("incorrect padding");
					}
					num6++;
					num7++;
				}
			}
		}
		return Arrays.CopyOfRange(buffer, num2, num3);
	}

	public BigInteger ReadMpint()
	{
		int num = ReadU32();
		if (pos > buffer.Length - num)
		{
			throw new InvalidOperationException("not enough data for big num");
		}
		switch (num)
		{
		case 0:
			return BigInteger.Zero;
		case 1:
			if (buffer[pos] == 0)
			{
				throw new InvalidOperationException("Zero MUST be stored with length 0");
			}
			break;
		}
		if (num > 1 && buffer[pos] == (byte)(-(buffer[pos + 1] >> 7)))
		{
			throw new InvalidOperationException("Unnecessary leading bytes MUST NOT be included");
		}
		int offset = pos;
		pos += num;
		return new BigInteger(buffer, offset, num);
	}

	public BigInteger ReadMpintPositive()
	{
		BigInteger bigInteger = ReadMpint();
		if (bigInteger.SignValue < 0)
		{
			throw new InvalidOperationException("Expected a positive mpint");
		}
		return bigInteger;
	}

	public bool HasRemaining()
	{
		return pos < buffer.Length;
	}
}
