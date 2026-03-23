using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

public sealed class SphincsPlusSigner : IMessageSigner
{
	private SphincsPlusPrivateKeyParameters m_privKey;

	private SphincsPlusPublicKeyParameters m_pubKey;

	private SecureRandom m_random;

	public void Init(bool forSigning, ICipherParameters param)
	{
		if (forSigning)
		{
			m_pubKey = null;
			if (param is ParametersWithRandom parametersWithRandom)
			{
				m_privKey = (SphincsPlusPrivateKeyParameters)parametersWithRandom.Parameters;
				m_random = parametersWithRandom.Random;
			}
			else
			{
				m_privKey = (SphincsPlusPrivateKeyParameters)param;
				m_random = null;
			}
		}
		else
		{
			m_pubKey = (SphincsPlusPublicKeyParameters)param;
			m_privKey = null;
			m_random = null;
		}
	}

	public byte[] GenerateSignature(byte[] message)
	{
		SphincsPlusEngine engine = m_privKey.Parameters.GetEngine();
		engine.Init(m_privKey.GetPublicSeed());
		byte[] array = new byte[engine.N];
		if (m_random != null)
		{
			m_random.NextBytes(array);
		}
		else
		{
			Array.Copy(m_privKey.m_pk.seed, 0, array, 0, array.Length);
		}
		Fors fors = new Fors(engine);
		byte[] array2 = engine.PRF_msg(m_privKey.m_sk.prf, array, message);
		IndexedDigest indexedDigest = engine.H_msg(array2, m_privKey.m_pk.seed, m_privKey.m_pk.root, message);
		byte[] digest = indexedDigest.digest;
		ulong idx_tree = indexedDigest.idx_tree;
		uint idx_leaf = indexedDigest.idx_leaf;
		Adrs adrs = new Adrs();
		adrs.SetAdrsType(Adrs.FORS_TREE);
		adrs.SetTreeAddress(idx_tree);
		adrs.SetKeyPairAddress(idx_leaf);
		SIG_FORS[] array3 = fors.Sign(digest, m_privKey.m_sk.seed, m_privKey.m_pk.seed, adrs);
		adrs = new Adrs();
		adrs.SetAdrsType(Adrs.FORS_TREE);
		adrs.SetTreeAddress(idx_tree);
		adrs.SetKeyPairAddress(idx_leaf);
		byte[] m = fors.PKFromSig(array3, digest, m_privKey.m_pk.seed, adrs);
		new Adrs().SetAdrsType(Adrs.TREE);
		byte[] array4 = new HT(engine, m_privKey.GetSeed(), m_privKey.GetPublicSeed()).Sign(m, idx_tree, idx_leaf);
		byte[][] array5 = new byte[array3.Length + 2][];
		array5[0] = array2;
		for (int i = 0; i != array3.Length; i++)
		{
			array5[1 + i] = Arrays.Concatenate(array3[i].sk, Arrays.ConcatenateAll(array3[i].authPath));
		}
		array5[^1] = array4;
		return Arrays.ConcatenateAll(array5);
	}

	public bool VerifySignature(byte[] message, byte[] signature)
	{
		SphincsPlusEngine engine = m_pubKey.Parameters.GetEngine();
		engine.Init(m_pubKey.GetSeed());
		Adrs adrs = new Adrs();
		SIG sIG = new SIG(engine.N, engine.K, engine.A, engine.D, engine.H_PRIME, engine.WOTS_LEN, signature);
		byte[] r = sIG.R;
		SIG_FORS[] sIG_FORS = sIG.SIG_FORS;
		SIG_XMSS[] sIG_HT = sIG.SIG_HT;
		IndexedDigest indexedDigest = engine.H_msg(r, m_pubKey.GetSeed(), m_pubKey.GetRoot(), message);
		byte[] digest = indexedDigest.digest;
		ulong idx_tree = indexedDigest.idx_tree;
		uint idx_leaf = indexedDigest.idx_leaf;
		adrs.SetAdrsType(Adrs.FORS_TREE);
		adrs.SetLayerAddress(0u);
		adrs.SetTreeAddress(idx_tree);
		adrs.SetKeyPairAddress(idx_leaf);
		byte[] m = new Fors(engine).PKFromSig(sIG_FORS, digest, m_pubKey.GetSeed(), adrs);
		adrs.SetAdrsType(Adrs.TREE);
		adrs.SetLayerAddress(0u);
		adrs.SetTreeAddress(idx_tree);
		adrs.SetKeyPairAddress(idx_leaf);
		return new HT(engine, null, m_pubKey.GetSeed()).Verify(m, sIG_HT, m_pubKey.GetSeed(), idx_tree, idx_leaf, m_pubKey.GetRoot());
	}
}
