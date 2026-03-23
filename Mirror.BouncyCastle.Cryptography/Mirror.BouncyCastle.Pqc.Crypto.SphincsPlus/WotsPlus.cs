using System;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal class WotsPlus
{
	private SphincsPlusEngine engine;

	private uint w;

	internal WotsPlus(SphincsPlusEngine engine)
	{
		this.engine = engine;
		w = this.engine.WOTS_W;
	}

	internal void PKGen(byte[] skSeed, byte[] pkSeed, Adrs paramAdrs, byte[] output)
	{
		Adrs adrs = new Adrs(paramAdrs);
		byte[][] array = new byte[engine.WOTS_LEN][];
		byte[] array2 = new byte[engine.N];
		for (uint num = 0u; num < engine.WOTS_LEN; num++)
		{
			Adrs adrs2 = new Adrs(paramAdrs);
			adrs2.SetAdrsType(Adrs.WOTS_PRF);
			adrs2.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());
			adrs2.SetChainAddress(num);
			adrs2.SetHashAddress(0u);
			engine.PRF(pkSeed, skSeed, adrs2, array2, 0);
			adrs2.SetAdrsType(Adrs.WOTS_HASH);
			adrs2.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());
			adrs2.SetChainAddress(num);
			adrs2.SetHashAddress(0u);
			array[num] = Chain(array2, 0u, w - 1, pkSeed, adrs2);
		}
		adrs.SetAdrsType(Adrs.WOTS_PK);
		adrs.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());
		engine.T_l(pkSeed, adrs, Arrays.ConcatenateAll(array), output);
	}

	private byte[] Chain(byte[] X, uint i, uint s, byte[] pkSeed, Adrs adrs)
	{
		if (s == 0)
		{
			return Arrays.Clone(X);
		}
		if (i + s > w - 1)
		{
			return null;
		}
		byte[] array = X;
		for (uint num = 0u; num < s; num++)
		{
			adrs.SetHashAddress(i + num);
			array = engine.F(pkSeed, adrs, array);
		}
		return array;
	}

	internal byte[] Sign(byte[] M, byte[] skSeed, byte[] pkSeed, Adrs paramAdrs)
	{
		Adrs adrs = new Adrs(paramAdrs);
		uint[] array = new uint[engine.WOTS_LEN];
		BaseW(M, 0, w, array, 0, engine.WOTS_LEN1);
		uint num = 0u;
		for (int i = 0; i < engine.WOTS_LEN1; i++)
		{
			num += w - 1 - array[i];
		}
		if (engine.WOTS_LOGW % 8 != 0)
		{
			num <<= 8 - engine.WOTS_LEN2 * engine.WOTS_LOGW % 8;
		}
		int num2 = (engine.WOTS_LEN2 * engine.WOTS_LOGW + 7) / 8;
		byte[] x = Pack.UInt32_To_BE(num);
		BaseW(x, 4 - num2, w, array, engine.WOTS_LEN1, engine.WOTS_LEN2);
		byte[][] array2 = new byte[engine.WOTS_LEN][];
		byte[] array3 = new byte[engine.N];
		for (int j = 0; j < engine.WOTS_LEN; j++)
		{
			adrs.SetAdrsType(Adrs.WOTS_PRF);
			adrs.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());
			adrs.SetChainAddress((uint)j);
			adrs.SetHashAddress(0u);
			engine.PRF(pkSeed, skSeed, adrs, array3, 0);
			adrs.SetAdrsType(Adrs.WOTS_HASH);
			adrs.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());
			adrs.SetChainAddress((uint)j);
			adrs.SetHashAddress(0u);
			array2[j] = Chain(array3, 0u, array[j], pkSeed, adrs);
		}
		return Arrays.ConcatenateAll(array2);
	}

	internal void BaseW(byte[] X, int XOff, uint w, uint[] output, int outOff, int outLen)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < outLen; i++)
		{
			if (num2 == 0)
			{
				num = X[XOff++];
				num2 += 8;
			}
			num2 -= engine.WOTS_LOGW;
			output[outOff++] = (uint)((num >> num2) & (w - 1));
		}
	}

	internal void PKFromSig(byte[] sig, byte[] M, byte[] pkSeed, Adrs adrs, byte[] output)
	{
		Adrs adrs2 = new Adrs(adrs);
		uint[] array = new uint[engine.WOTS_LEN];
		BaseW(M, 0, w, array, 0, engine.WOTS_LEN1);
		uint num = 0u;
		for (int i = 0; i < engine.WOTS_LEN1; i++)
		{
			num += w - 1 - array[i];
		}
		num <<= 8 - engine.WOTS_LEN2 * engine.WOTS_LOGW % 8;
		int num2 = (engine.WOTS_LEN2 * engine.WOTS_LOGW + 7) / 8;
		byte[] x = Pack.UInt32_To_BE(num);
		BaseW(x, 4 - num2, w, array, engine.WOTS_LEN1, engine.WOTS_LEN2);
		byte[] array2 = new byte[engine.N];
		byte[][] array3 = new byte[engine.WOTS_LEN][];
		for (int j = 0; j < engine.WOTS_LEN; j++)
		{
			adrs.SetChainAddress((uint)j);
			int sourceIndex = engine.N * j;
			Array.Copy(sig, sourceIndex, array2, 0, engine.N);
			array3[j] = Chain(array2, array[j], w - 1 - array[j], pkSeed, adrs);
		}
		adrs2.SetAdrsType(Adrs.WOTS_PK);
		adrs2.SetKeyPairAddress(adrs.GetKeyPairAddress());
		engine.T_l(pkSeed, adrs2, Arrays.ConcatenateAll(array3), output);
	}
}
