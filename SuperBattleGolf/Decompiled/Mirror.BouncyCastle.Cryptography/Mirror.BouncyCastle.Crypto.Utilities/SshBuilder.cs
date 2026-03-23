using System;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto.Utilities;

internal class SshBuilder
{
	private readonly MemoryStream bos = new MemoryStream();

	public void U32(uint value)
	{
		bos.WriteByte(Convert.ToByte((value >> 24) & 0xFF));
		bos.WriteByte(Convert.ToByte((value >> 16) & 0xFF));
		bos.WriteByte(Convert.ToByte((value >> 8) & 0xFF));
		bos.WriteByte(Convert.ToByte(value & 0xFF));
	}

	public void WriteMpint(BigInteger n)
	{
		WriteBlock(n.ToByteArray());
	}

	public void WriteBlock(byte[] value)
	{
		U32((uint)value.Length);
		WriteBytes(value);
	}

	public void WriteBytes(byte[] value)
	{
		try
		{
			bos.Write(value, 0, value.Length);
		}
		catch (IOException ex)
		{
			throw new InvalidOperationException(ex.Message, ex);
		}
	}

	public void WriteStringAscii(string str)
	{
		WriteBlock(Encoding.ASCII.GetBytes(str));
	}

	public void WriteStringUtf8(string str)
	{
		WriteBlock(Encoding.UTF8.GetBytes(str));
	}

	public byte[] GetBytes()
	{
		return bos.ToArray();
	}

	public byte[] GetPaddedBytes()
	{
		return GetPaddedBytes(8);
	}

	public byte[] GetPaddedBytes(int blockSize)
	{
		int num = (int)bos.Length % blockSize;
		if (num != 0)
		{
			int num2 = blockSize - num;
			for (int i = 1; i <= num2; i++)
			{
				bos.WriteByte(Convert.ToByte(i));
			}
		}
		return bos.ToArray();
	}
}
