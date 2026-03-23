using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class FalconRNG
{
	private byte[] bd;

	private byte[] sd;

	private int ptr;

	private FalconConversions convertor;

	internal FalconRNG()
	{
		bd = new byte[512];
		sd = new byte[256];
		convertor = new FalconConversions();
	}

	internal void prng_init(SHAKE256 src)
	{
		byte[] array = new byte[56];
		src.i_shake256_extract(array, 0, 56);
		for (int i = 0; i < 14; i++)
		{
			uint x = (uint)(array[i << 2] | (array[(i << 2) + 1] << 8) | (array[(i << 2) + 2] << 16) | (array[(i << 2) + 3] << 24));
			Array.Copy(convertor.int_to_bytes((int)x), 0, sd, i << 2, 4);
		}
		ulong num = convertor.bytes_to_uint(sd, 48);
		ulong num2 = convertor.bytes_to_uint(sd, 52);
		Array.Copy(convertor.ulong_to_bytes(num + (num2 << 32)), 0, sd, 48, 8);
		prng_refill();
	}

	private void QROUND(uint[] state, int a, int b, int c, int d)
	{
		state[a] += state[b];
		state[d] ^= state[a];
		state[d] = (state[d] << 16) | (state[d] >> 16);
		state[c] += state[d];
		state[b] ^= state[c];
		state[b] = (state[b] << 12) | (state[b] >> 20);
		state[a] += state[b];
		state[d] ^= state[a];
		state[d] = (state[d] << 8) | (state[d] >> 24);
		state[c] += state[d];
		state[b] ^= state[c];
		state[b] = (state[b] << 7) | (state[b] >> 25);
	}

	private void prng_refill()
	{
		uint[] array = new uint[4] { 1634760805u, 857760878u, 2036477234u, 1797285236u };
		ulong num = convertor.bytes_to_ulong(sd, 48);
		for (int i = 0; i < 8; i++)
		{
			uint[] array2 = new uint[16];
			Array.Copy(array, 0, array2, 0, 4);
			Array.Copy(convertor.bytes_to_uint_array(sd, 0, 12), 0, array2, 4, 12);
			array2[14] ^= (uint)(int)num;
			array2[15] ^= (uint)(int)(num >> 32);
			for (int j = 0; j < 10; j++)
			{
				QROUND(array2, 0, 4, 8, 12);
				QROUND(array2, 1, 5, 9, 13);
				QROUND(array2, 2, 6, 10, 14);
				QROUND(array2, 3, 7, 11, 15);
				QROUND(array2, 0, 5, 10, 15);
				QROUND(array2, 1, 6, 11, 12);
				QROUND(array2, 2, 7, 8, 13);
				QROUND(array2, 3, 4, 9, 14);
			}
			for (int k = 0; k < 4; k++)
			{
				array2[k] += array[k];
			}
			for (int k = 4; k < 14; k++)
			{
				array2[k] += convertor.bytes_to_uint(sd, 4 * k - 16);
			}
			array2[14] += (uint)(int)(convertor.bytes_to_uint(sd, 40) ^ (int)num);
			array2[15] += (uint)(int)(convertor.bytes_to_uint(sd, 44) ^ (int)(num >> 32));
			num++;
			for (int k = 0; k < 16; k++)
			{
				bd[(i << 2) + (k << 5)] = (byte)array2[k];
				bd[(i << 2) + (k << 5) + 1] = (byte)(array2[k] >> 8);
				bd[(i << 2) + (k << 5) + 2] = (byte)(array2[k] >> 16);
				bd[(i << 2) + (k << 5) + 3] = (byte)(array2[k] >> 24);
			}
		}
		Array.Copy(convertor.ulong_to_bytes(num), 0, sd, 48, 8);
		ptr = 0;
	}

	internal void prng_get_bytes(byte[] dstsrc, int dst, int len)
	{
		int num = dst;
		while (len > 0)
		{
			int num2 = bd.Length - ptr;
			if (num2 > len)
			{
				num2 = len;
			}
			Array.Copy(bd, 0, dstsrc, num, num2);
			num += num2;
			len -= num2;
			ptr += num2;
			if (ptr == bd.Length)
			{
				prng_refill();
			}
		}
	}

	internal ulong prng_get_u64()
	{
		int num = ptr;
		if (num >= bd.Length - 9)
		{
			prng_refill();
			num = 0;
		}
		ptr = num + 8;
		return bd[num] | ((ulong)bd[num + 1] << 8) | ((ulong)bd[num + 2] << 16) | ((ulong)bd[num + 3] << 24) | ((ulong)bd[num + 4] << 32) | ((ulong)bd[num + 5] << 40) | ((ulong)bd[num + 6] << 48) | ((ulong)bd[num + 7] << 56);
	}

	internal uint prng_get_u8()
	{
		byte result = bd[ptr++];
		if (ptr == bd.Length)
		{
			prng_refill();
		}
		return result;
	}
}
