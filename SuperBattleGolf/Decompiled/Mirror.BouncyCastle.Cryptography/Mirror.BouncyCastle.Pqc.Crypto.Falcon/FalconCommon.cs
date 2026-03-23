namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class FalconCommon
{
	internal uint[] l2bound = new uint[11]
	{
		0u, 101498u, 208714u, 428865u, 892039u, 1852696u, 3842630u, 7959734u, 16468416u, 34034726u,
		70265242u
	};

	internal void hash_to_point_vartime(SHAKE256 sc, ushort[] xsrc, int x, uint logn)
	{
		int num = 1 << (int)logn;
		while (num > 0)
		{
			byte[] array = new byte[2];
			sc.i_shake256_extract(array, 0, 2);
			uint num2 = (uint)((array[0] << 8) | array[1]);
			if (num2 < 61445)
			{
				while (num2 >= 12289)
				{
					num2 -= 12289;
				}
				xsrc[x++] = (ushort)num2;
				num--;
			}
		}
	}

	internal bool is_short(short[] s1src, int s1, short[] s2src, int s2, uint logn)
	{
		int num = 1 << (int)logn;
		uint num2 = 0u;
		uint num3 = 0u;
		for (int i = 0; i < num; i++)
		{
			int num4 = s1src[s1 + i];
			num2 += (uint)(num4 * num4);
			num3 |= num2;
			num4 = s2src[s2 + i];
			num2 += (uint)(num4 * num4);
			num3 |= num2;
		}
		num2 |= (uint)(int)(0L - (long)(num3 >> 31));
		return num2 <= l2bound[logn];
	}

	internal bool is_short_half(uint sqn, short[] s2src, int s2, uint logn)
	{
		int num = 1 << (int)logn;
		uint num2 = (uint)(0uL - (ulong)(sqn >> 31));
		for (int i = 0; i < num; i++)
		{
			int num3 = s2src[s2 + i];
			sqn += (uint)(num3 * num3);
			num2 |= sqn;
		}
		sqn |= (uint)(int)(0L - (long)(num2 >> 31));
		return sqn <= l2bound[logn];
	}
}
