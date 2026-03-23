using System.Collections.Generic;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal class Fors
{
	private readonly SphincsPlusEngine engine;

	internal Fors(SphincsPlusEngine engine)
	{
		this.engine = engine;
	}

	internal byte[] TreeHash(byte[] skSeed, uint s, int z, byte[] pkSeed, Adrs adrsParam)
	{
		if (s % (1 << z) != 0L)
		{
			return null;
		}
		Stack<NodeEntry> stack = new Stack<NodeEntry>();
		Adrs adrs = new Adrs(adrsParam);
		byte[] array = new byte[engine.N];
		for (uint num = 0u; num < 1 << z; num++)
		{
			adrs.SetAdrsType(Adrs.FORS_PRF);
			adrs.SetKeyPairAddress(adrsParam.GetKeyPairAddress());
			adrs.SetTreeHeight(0u);
			adrs.SetTreeIndex(s + num);
			engine.PRF(pkSeed, skSeed, adrs, array, 0);
			adrs.ChangeAdrsType(Adrs.FORS_TREE);
			byte[] array2 = engine.F(pkSeed, adrs, array);
			adrs.SetTreeHeight(1u);
			uint num2 = 1u;
			uint num3 = s + num;
			while (stack.Count > 0 && stack.Peek().nodeHeight == num2)
			{
				num3 = (num3 - 1) / 2;
				adrs.SetTreeIndex(num3);
				engine.H(pkSeed, adrs, stack.Pop().nodeValue, array2, array2);
				adrs.SetTreeHeight(++num2);
			}
			stack.Push(new NodeEntry(array2, num2));
		}
		return stack.Peek().nodeValue;
	}

	internal SIG_FORS[] Sign(byte[] md, byte[] skSeed, byte[] pkSeed, Adrs paramAdrs)
	{
		Adrs adrs = new Adrs(paramAdrs);
		SIG_FORS[] array = new SIG_FORS[engine.K];
		uint t = engine.T;
		for (uint num = 0u; num < engine.K; num++)
		{
			uint messageIdx = GetMessageIdx(md, (int)num, engine.A);
			adrs.SetAdrsType(Adrs.FORS_PRF);
			adrs.SetKeyPairAddress(paramAdrs.GetKeyPairAddress());
			adrs.SetTreeHeight(0u);
			adrs.SetTreeIndex(num * t + messageIdx);
			byte[] array2 = new byte[engine.N];
			engine.PRF(pkSeed, skSeed, adrs, array2, 0);
			adrs.ChangeAdrsType(Adrs.FORS_TREE);
			byte[][] array3 = new byte[engine.A][];
			for (int i = 0; i < engine.A; i++)
			{
				uint num2 = (messageIdx >> i) ^ 1;
				array3[i] = TreeHash(skSeed, num * t + (num2 << i), i, pkSeed, adrs);
			}
			array[num] = new SIG_FORS(array2, array3);
		}
		return array;
	}

	internal byte[] PKFromSig(SIG_FORS[] sig_fors, byte[] message, byte[] pkSeed, Adrs adrs)
	{
		byte[][] array = new byte[engine.K][];
		uint t = engine.T;
		for (uint num = 0u; num < engine.K; num++)
		{
			uint messageIdx = GetMessageIdx(message, (int)num, engine.A);
			byte[] sK = sig_fors[num].SK;
			adrs.SetTreeHeight(0u);
			adrs.SetTreeIndex(num * t + messageIdx);
			byte[] array2 = engine.F(pkSeed, adrs, sK);
			byte[][] authPath = sig_fors[num].AuthPath;
			uint num2 = num * t + messageIdx;
			for (int i = 0; i < engine.A; i++)
			{
				adrs.SetTreeHeight((uint)(i + 1));
				if ((messageIdx >> i) % 2 == 0)
				{
					num2 /= 2;
					adrs.SetTreeIndex(num2);
					engine.H(pkSeed, adrs, array2, authPath[i], array2);
				}
				else
				{
					num2 = (num2 - 1) / 2;
					adrs.SetTreeIndex(num2);
					engine.H(pkSeed, adrs, authPath[i], array2, array2);
				}
			}
			array[num] = array2;
		}
		Adrs adrs2 = new Adrs(adrs);
		adrs2.SetAdrsType(Adrs.FORS_PK);
		adrs2.SetKeyPairAddress(adrs.GetKeyPairAddress());
		byte[] array3 = new byte[engine.N];
		engine.T_l(pkSeed, adrs2, Arrays.ConcatenateAll(array), array3);
		return array3;
	}

	private static uint GetMessageIdx(byte[] msg, int fors_tree, int fors_height)
	{
		int num = fors_tree * fors_height;
		uint num2 = 0u;
		for (int i = 0; i < fors_height; i++)
		{
			num2 ^= (uint)(((msg[num >> 3] >>> (num & 7)) & 1) << i);
			num++;
		}
		return num2;
	}
}
