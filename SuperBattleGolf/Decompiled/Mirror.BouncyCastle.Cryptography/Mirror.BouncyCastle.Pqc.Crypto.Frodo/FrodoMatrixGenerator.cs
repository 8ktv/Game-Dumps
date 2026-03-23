using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Frodo;

public abstract class FrodoMatrixGenerator
{
	internal class Shake128MatrixGenerator : FrodoMatrixGenerator
	{
		public Shake128MatrixGenerator(int n, int q)
			: base(n, q)
		{
		}

		internal override short[] GenMatrix(byte[] seedA)
		{
			short[] array = new short[n * n];
			byte[] array2 = new byte[16 * n / 8];
			byte[] array3 = new byte[2 + seedA.Length];
			Array.Copy(seedA, 0, array3, 2, seedA.Length);
			uint num = (uint)((q - 1 << 16) | (ushort)(q - 1));
			IXof xof = new ShakeDigest(128);
			for (ushort num2 = 0; num2 < n; num2++)
			{
				Pack.UInt16_To_LE(num2, array3);
				xof.BlockUpdate(array3, 0, array3.Length);
				xof.OutputFinal(array2, 0, array2.Length);
				for (ushort num3 = 0; num3 < n; num3 += 8)
				{
					uint num4 = Pack.LE_To_UInt32(array2, 2 * num3) & num;
					uint num5 = Pack.LE_To_UInt32(array2, 2 * num3 + 4) & num;
					uint num6 = Pack.LE_To_UInt32(array2, 2 * num3 + 8) & num;
					uint num7 = Pack.LE_To_UInt32(array2, 2 * num3 + 12) & num;
					array[num2 * n + num3] = (short)num4;
					array[num2 * n + num3 + 1] = (short)(num4 >> 16);
					array[num2 * n + num3 + 2] = (short)num5;
					array[num2 * n + num3 + 3] = (short)(num5 >> 16);
					array[num2 * n + num3 + 4] = (short)num6;
					array[num2 * n + num3 + 5] = (short)(num6 >> 16);
					array[num2 * n + num3 + 6] = (short)num7;
					array[num2 * n + num3 + 7] = (short)(num7 >> 16);
				}
			}
			return array;
		}
	}

	internal class Aes128MatrixGenerator : FrodoMatrixGenerator
	{
		public Aes128MatrixGenerator(int n, int q)
			: base(n, q)
		{
		}

		internal override short[] GenMatrix(byte[] seedA)
		{
			short[] array = new short[n * n];
			byte[] array2 = new byte[16];
			byte[] array3 = new byte[16];
			uint num = (uint)((q - 1 << 16) | (ushort)(q - 1));
			IBlockCipher blockCipher = AesUtilities.CreateEngine();
			blockCipher.Init(forEncryption: true, new KeyParameter(seedA));
			for (int i = 0; i < n; i++)
			{
				Pack.UInt16_To_LE((ushort)i, array2, 0);
				for (int j = 0; j < n; j += 8)
				{
					Pack.UInt16_To_LE((ushort)j, array2, 2);
					blockCipher.ProcessBlock(array2, 0, array3, 0);
					uint num2 = Pack.LE_To_UInt32(array3, 0) & num;
					uint num3 = Pack.LE_To_UInt32(array3, 4) & num;
					uint num4 = Pack.LE_To_UInt32(array3, 8) & num;
					uint num5 = Pack.LE_To_UInt32(array3, 12) & num;
					array[i * n + j] = (short)num2;
					array[i * n + j + 1] = (short)(num2 >> 16);
					array[i * n + j + 2] = (short)num3;
					array[i * n + j + 3] = (short)(num3 >> 16);
					array[i * n + j + 4] = (short)num4;
					array[i * n + j + 5] = (short)(num4 >> 16);
					array[i * n + j + 6] = (short)num5;
					array[i * n + j + 7] = (short)(num5 >> 16);
				}
			}
			return array;
		}
	}

	private int n;

	private int q;

	public FrodoMatrixGenerator(int n, int q)
	{
		this.n = n;
		this.q = q;
	}

	internal abstract short[] GenMatrix(byte[] seedA);
}
