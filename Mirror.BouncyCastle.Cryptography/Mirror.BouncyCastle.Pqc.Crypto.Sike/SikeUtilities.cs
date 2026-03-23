namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

internal static class SikeUtilities
{
	internal static ulong[][] InitArray(uint size1, uint size2)
	{
		ulong[][] array = new ulong[size1][];
		for (int i = 0; i < size1; i++)
		{
			array[i] = new ulong[size2];
		}
		return array;
	}

	internal static ulong[][][] InitArray(uint size1, uint size2, uint size3)
	{
		ulong[][][] array = new ulong[size1][][];
		for (int i = 0; i < size1; i++)
		{
			array[i] = new ulong[size2][];
			for (int j = 0; j < size2; j++)
			{
				array[i][j] = new ulong[size3];
			}
		}
		return array;
	}

	internal static ulong[][][][] InitArray(uint size1, uint size2, uint size3, uint size4)
	{
		ulong[][][][] array = new ulong[size1][][][];
		for (int i = 0; i < size1; i++)
		{
			array[i] = new ulong[size2][][];
			for (int j = 0; j < size2; j++)
			{
				array[i][j] = new ulong[size3][];
				for (int k = 0; k < size3; k++)
				{
					array[i][j][k] = new ulong[size4];
				}
			}
		}
		return array;
	}
}
