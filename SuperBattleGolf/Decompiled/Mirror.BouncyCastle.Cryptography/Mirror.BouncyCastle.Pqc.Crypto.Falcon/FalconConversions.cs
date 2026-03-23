namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class FalconConversions
{
	internal FalconConversions()
	{
	}

	internal byte[] int_to_bytes(int x)
	{
		return new byte[4]
		{
			(byte)x,
			(byte)(x >> 8),
			(byte)(x >> 16),
			(byte)(x >> 24)
		};
	}

	internal uint bytes_to_uint(byte[] src, int pos)
	{
		return (uint)(src[pos] | (src[pos + 1] << 8) | (src[pos + 2] << 16) | (src[pos + 3] << 24));
	}

	internal byte[] ulong_to_bytes(ulong x)
	{
		return new byte[8]
		{
			(byte)x,
			(byte)(x >> 8),
			(byte)(x >> 16),
			(byte)(x >> 24),
			(byte)(x >> 32),
			(byte)(x >> 40),
			(byte)(x >> 48),
			(byte)(x >> 56)
		};
	}

	internal ulong bytes_to_ulong(byte[] src, int pos)
	{
		return src[pos] | ((ulong)src[pos + 1] << 8) | ((ulong)src[pos + 2] << 16) | ((ulong)src[pos + 3] << 24) | ((ulong)src[pos + 4] << 32) | ((ulong)src[pos + 5] << 40) | ((ulong)src[pos + 6] << 48) | ((ulong)src[pos + 7] << 56);
	}

	internal uint[] bytes_to_uint_array(byte[] src, int pos, int num)
	{
		uint[] array = new uint[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = bytes_to_uint(src, pos + 4 * i);
		}
		return array;
	}
}
