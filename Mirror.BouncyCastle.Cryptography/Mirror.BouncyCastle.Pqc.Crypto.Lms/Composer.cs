using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class Composer
{
	private readonly MemoryStream bos = new MemoryStream();

	private Composer()
	{
	}

	public static Composer Compose()
	{
		return new Composer();
	}

	public Composer U64Str(long n)
	{
		U32Str((int)(n >> 32));
		U32Str((int)n);
		return this;
	}

	public Composer U32Str(int n)
	{
		bos.WriteByte((byte)(n >> 24));
		bos.WriteByte((byte)(n >> 16));
		bos.WriteByte((byte)(n >> 8));
		bos.WriteByte((byte)n);
		return this;
	}

	public Composer U16Str(int n)
	{
		n &= 0xFFFF;
		bos.WriteByte((byte)(n >> 8));
		bos.WriteByte((byte)n);
		return this;
	}

	public Composer Bytes(IEncodable[] encodable)
	{
		for (int i = 0; i < encodable.Length; i++)
		{
			byte[] encoded = encodable[i].GetEncoded();
			bos.Write(encoded, 0, encoded.Length);
		}
		return this;
	}

	public Composer Bytes(IEncodable encodable)
	{
		byte[] encoded = encodable.GetEncoded();
		bos.Write(encoded, 0, encoded.Length);
		return this;
	}

	public Composer Pad(int v, int len)
	{
		while (len >= 0)
		{
			bos.WriteByte((byte)v);
			len--;
		}
		return this;
	}

	public Composer Bytes2(byte[][] arrays)
	{
		foreach (byte[] array in arrays)
		{
			bos.Write(array, 0, array.Length);
		}
		return this;
	}

	public Composer Bytes2(byte[][] arrays, int start, int end)
	{
		for (int i = start; i != end; i++)
		{
			bos.Write(arrays[i], 0, arrays[i].Length);
		}
		return this;
	}

	public Composer Bytes(byte[] array)
	{
		bos.Write(array, 0, array.Length);
		return this;
	}

	public Composer Bytes(byte[] array, int start, int len)
	{
		bos.Write(array, start, len);
		return this;
	}

	public byte[] Build()
	{
		return bos.ToArray();
	}

	public Composer PadUntil(int v, int requiredLen)
	{
		while (bos.Length < requiredLen)
		{
			bos.WriteByte((byte)v);
		}
		return this;
	}

	public Composer Boolean(bool v)
	{
		bos.WriteByte(v ? ((byte)1) : ((byte)0));
		return this;
	}
}
