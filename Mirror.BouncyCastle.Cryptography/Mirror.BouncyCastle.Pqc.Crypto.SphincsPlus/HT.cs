using System.Collections.Generic;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal class HT
{
	private byte[] skSeed;

	private byte[] pkSeed;

	private SphincsPlusEngine engine;

	private WotsPlus wots;

	internal byte[] HTPubKey;

	internal HT(SphincsPlusEngine engine, byte[] skSeed, byte[] pkSeed)
	{
		this.skSeed = skSeed;
		this.pkSeed = pkSeed;
		this.engine = engine;
		wots = new WotsPlus(engine);
		Adrs adrs = new Adrs();
		adrs.SetLayerAddress(engine.D - 1);
		adrs.SetTreeAddress(0uL);
		if (skSeed != null)
		{
			HTPubKey = xmss_PKgen(skSeed, pkSeed, adrs);
		}
		else
		{
			HTPubKey = null;
		}
	}

	internal byte[] Sign(byte[] M, ulong idx_tree, uint idx_leaf)
	{
		Adrs adrs = new Adrs();
		adrs.SetLayerAddress(0u);
		adrs.SetTreeAddress(idx_tree);
		SIG_XMSS sIG_XMSS = xmss_sign(M, skSeed, idx_leaf, pkSeed, adrs);
		SIG_XMSS[] array = new SIG_XMSS[engine.D];
		array[0] = sIG_XMSS;
		adrs.SetLayerAddress(0u);
		adrs.SetTreeAddress(idx_tree);
		byte[] m = xmss_pkFromSig(idx_leaf, sIG_XMSS, M, pkSeed, adrs);
		for (uint num = 1u; num < engine.D; num++)
		{
			idx_leaf = (uint)(idx_tree & (ulong)((1 << (int)engine.H_PRIME) - 1));
			idx_tree >>= (int)engine.H_PRIME;
			adrs.SetLayerAddress(num);
			adrs.SetTreeAddress(idx_tree);
			sIG_XMSS = (array[num] = xmss_sign(m, skSeed, idx_leaf, pkSeed, adrs));
			if (num < engine.D - 1)
			{
				m = xmss_pkFromSig(idx_leaf, sIG_XMSS, m, pkSeed, adrs);
			}
		}
		byte[][] array2 = new byte[array.Length][];
		for (int i = 0; i != array2.Length; i++)
		{
			array2[i] = Arrays.Concatenate(array[i].sig, Arrays.ConcatenateAll(array[i].auth));
		}
		return Arrays.ConcatenateAll(array2);
	}

	private byte[] xmss_PKgen(byte[] skSeed, byte[] pkSeed, Adrs adrs)
	{
		return TreeHash(skSeed, 0u, engine.H_PRIME, pkSeed, adrs);
	}

	private byte[] xmss_pkFromSig(uint idx, SIG_XMSS sig_xmss, byte[] M, byte[] pkSeed, Adrs paramAdrs)
	{
		Adrs adrs = new Adrs(paramAdrs);
		adrs.SetAdrsType(Adrs.WOTS_HASH);
		adrs.SetKeyPairAddress(idx);
		byte[] wotsSig = sig_xmss.WotsSig;
		byte[][] xmssAuth = sig_xmss.XmssAuth;
		byte[] array = new byte[engine.N];
		wots.PKFromSig(wotsSig, M, pkSeed, adrs, array);
		adrs.SetAdrsType(Adrs.TREE);
		adrs.SetTreeIndex(idx);
		for (uint num = 0u; num < engine.H_PRIME; num++)
		{
			adrs.SetTreeHeight(num + 1);
			if (idx / (1 << (int)num) % 2 == 0L)
			{
				adrs.SetTreeIndex(adrs.GetTreeIndex() / 2);
				engine.H(pkSeed, adrs, array, xmssAuth[num], array);
			}
			else
			{
				adrs.SetTreeIndex((adrs.GetTreeIndex() - 1) / 2);
				engine.H(pkSeed, adrs, xmssAuth[num], array, array);
			}
		}
		return array;
	}

	private SIG_XMSS xmss_sign(byte[] M, byte[] skSeed, uint idx, byte[] pkSeed, Adrs paramAdrs)
	{
		byte[][] array = new byte[engine.H_PRIME][];
		Adrs adrs = new Adrs(paramAdrs);
		adrs.SetAdrsType(Adrs.TREE);
		adrs.SetLayerAddress(paramAdrs.GetLayerAddress());
		adrs.SetTreeAddress(paramAdrs.GetTreeAddress());
		for (int i = 0; i < engine.H_PRIME; i++)
		{
			uint num = (uint)((int)(idx / (1 << i)) ^ 1);
			array[i] = TreeHash(skSeed, num * (uint)(1 << i), (uint)i, pkSeed, adrs);
		}
		adrs = new Adrs(paramAdrs);
		adrs.SetAdrsType(Adrs.WOTS_PK);
		adrs.SetKeyPairAddress(idx);
		return new SIG_XMSS(wots.Sign(M, skSeed, pkSeed, adrs), array);
	}

	private byte[] TreeHash(byte[] skSeed, uint s, uint z, byte[] pkSeed, Adrs adrsParam)
	{
		if (s % (1 << (int)z) != 0L)
		{
			return null;
		}
		Stack<NodeEntry> stack = new Stack<NodeEntry>();
		Adrs adrs = new Adrs(adrsParam);
		for (uint num = 0u; num < 1 << (int)z; num++)
		{
			adrs.SetAdrsType(Adrs.WOTS_HASH);
			adrs.SetKeyPairAddress(s + num);
			byte[] array = new byte[engine.N];
			wots.PKGen(skSeed, pkSeed, adrs, array);
			adrs.SetAdrsType(Adrs.TREE);
			adrs.SetTreeHeight(1u);
			adrs.SetTreeIndex(s + num);
			uint num2 = 1u;
			uint num3 = s + num;
			while (stack.Count > 0 && stack.Peek().nodeHeight == num2)
			{
				num3 = (num3 - 1) / 2;
				adrs.SetTreeIndex(num3);
				engine.H(pkSeed, adrs, stack.Pop().nodeValue, array, array);
				adrs.SetTreeHeight(++num2);
			}
			stack.Push(new NodeEntry(array, num2));
		}
		return stack.Peek().nodeValue;
	}

	internal bool Verify(byte[] M, SIG_XMSS[] sig_ht, byte[] pkSeed, ulong idx_tree, uint idx_leaf, byte[] PK_HT)
	{
		Adrs adrs = new Adrs();
		SIG_XMSS sig_xmss = sig_ht[0];
		adrs.SetLayerAddress(0u);
		adrs.SetTreeAddress(idx_tree);
		byte[] array = xmss_pkFromSig(idx_leaf, sig_xmss, M, pkSeed, adrs);
		for (uint num = 1u; num < engine.D; num++)
		{
			idx_leaf = (uint)(idx_tree & (ulong)((1 << (int)engine.H_PRIME) - 1));
			idx_tree >>= (int)engine.H_PRIME;
			sig_xmss = sig_ht[num];
			adrs.SetLayerAddress(num);
			adrs.SetTreeAddress(idx_tree);
			array = xmss_pkFromSig(idx_leaf, sig_xmss, array, pkSeed, adrs);
		}
		return Arrays.AreEqual(PK_HT, array);
	}
}
