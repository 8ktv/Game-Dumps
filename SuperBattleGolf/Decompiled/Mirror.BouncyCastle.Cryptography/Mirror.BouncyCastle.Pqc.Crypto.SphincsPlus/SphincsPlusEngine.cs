using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Macs;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal abstract class SphincsPlusEngine
{
	internal class Sha2Engine : SphincsPlusEngine
	{
		private HMac treeHMac;

		private Mgf1BytesGenerator mgf1;

		private byte[] hmacBuf;

		private IDigest msgDigest;

		private byte[] msgDigestBuf;

		private int bl;

		private IDigest sha256;

		private byte[] sha256Buf;

		private IMemoable msgMemo;

		private IMemoable sha256Memo;

		public Sha2Engine(bool robust, int n, uint w, uint d, int a, int k, uint h)
			: base(robust, n, w, d, a, k, h)
		{
			sha256 = new Sha256Digest();
			sha256Buf = new byte[sha256.GetDigestSize()];
			if (n == 16)
			{
				msgDigest = new Sha256Digest();
				treeHMac = new HMac(new Sha256Digest());
				mgf1 = new Mgf1BytesGenerator(new Sha256Digest());
				bl = 64;
			}
			else
			{
				msgDigest = new Sha512Digest();
				treeHMac = new HMac(new Sha512Digest());
				mgf1 = new Mgf1BytesGenerator(new Sha512Digest());
				bl = 128;
			}
			hmacBuf = new byte[treeHMac.GetMacSize()];
			msgDigestBuf = new byte[msgDigest.GetDigestSize()];
		}

		public override void Init(byte[] pkSeed)
		{
			byte[] input = new byte[bl];
			msgDigest.BlockUpdate(pkSeed, 0, pkSeed.Length);
			msgDigest.BlockUpdate(input, 0, bl - N);
			msgMemo = ((IMemoable)msgDigest).Copy();
			msgDigest.Reset();
			sha256.BlockUpdate(pkSeed, 0, pkSeed.Length);
			sha256.BlockUpdate(input, 0, 64 - N);
			sha256Memo = ((IMemoable)sha256).Copy();
			sha256.Reset();
		}

		public override byte[] F(byte[] pkSeed, Adrs adrs, byte[] m1)
		{
			byte[] array = CompressedAdrs(adrs);
			if (robust)
			{
				m1 = Bitmask256(Arrays.Concatenate(pkSeed, array), m1);
			}
			((IMemoable)sha256).Reset(sha256Memo);
			sha256.BlockUpdate(array, 0, array.Length);
			sha256.BlockUpdate(m1, 0, m1.Length);
			sha256.DoFinal(sha256Buf, 0);
			return Arrays.CopyOfRange(sha256Buf, 0, N);
		}

		public override void H(byte[] pkSeed, Adrs adrs, byte[] m1, byte[] m2, byte[] output)
		{
			byte[] array = CompressedAdrs(adrs);
			((IMemoable)msgDigest).Reset(msgMemo);
			msgDigest.BlockUpdate(array, 0, array.Length);
			if (robust)
			{
				byte[] array2 = Bitmask(Arrays.Concatenate(pkSeed, array), m1, m2);
				msgDigest.BlockUpdate(array2, 0, array2.Length);
			}
			else
			{
				msgDigest.BlockUpdate(m1, 0, m1.Length);
				msgDigest.BlockUpdate(m2, 0, m2.Length);
			}
			msgDigest.DoFinal(msgDigestBuf, 0);
			Array.Copy(msgDigestBuf, 0, output, 0, N);
		}

		public override IndexedDigest H_msg(byte[] prf, byte[] pkSeed, byte[] pkRoot, byte[] message)
		{
			int num = (A * K + 7) / 8;
			uint num2 = FH / D;
			uint num3 = FH - num2;
			uint num4 = (num2 + 7) / 8;
			uint num5 = (num3 + 7) / 8;
			int num6 = num + (int)num5 + (int)num4;
			byte[] array = new byte[msgDigest.GetDigestSize()];
			msgDigest.BlockUpdate(prf, 0, prf.Length);
			msgDigest.BlockUpdate(pkSeed, 0, pkSeed.Length);
			msgDigest.BlockUpdate(pkRoot, 0, pkRoot.Length);
			msgDigest.BlockUpdate(message, 0, message.Length);
			msgDigest.DoFinal(array, 0);
			byte[] m = new byte[num6];
			m = Bitmask(Arrays.ConcatenateAll(prf, pkSeed, array), m);
			ulong idx_tree = Pack.BE_To_UInt64_Low(m, num, (int)num5) & (ulong.MaxValue >> (int)(64 - num3));
			uint idx_leaf = Pack.BE_To_UInt32_Low(m, num + (int)num5, (int)num4) & (uint)(-1 >>> (int)(32 - num2));
			return new IndexedDigest(idx_tree, idx_leaf, Arrays.CopyOfRange(m, 0, num));
		}

		public override void T_l(byte[] pkSeed, Adrs adrs, byte[] m, byte[] output)
		{
			byte[] array = CompressedAdrs(adrs);
			if (robust)
			{
				m = Bitmask(Arrays.Concatenate(pkSeed, array), m);
			}
			((IMemoable)msgDigest).Reset(msgMemo);
			msgDigest.BlockUpdate(array, 0, array.Length);
			msgDigest.BlockUpdate(m, 0, m.Length);
			msgDigest.DoFinal(msgDigestBuf, 0);
			Array.Copy(msgDigestBuf, 0, output, 0, N);
		}

		public override void PRF(byte[] pkSeed, byte[] skSeed, Adrs adrs, byte[] prf, int prfOff)
		{
			int length = skSeed.Length;
			((IMemoable)sha256).Reset(sha256Memo);
			byte[] array = CompressedAdrs(adrs);
			sha256.BlockUpdate(array, 0, array.Length);
			sha256.BlockUpdate(skSeed, 0, skSeed.Length);
			sha256.DoFinal(sha256Buf, 0);
			Array.Copy(sha256Buf, 0, prf, prfOff, length);
		}

		public override byte[] PRF_msg(byte[] prf, byte[] randomiser, byte[] message)
		{
			treeHMac.Init(new KeyParameter(prf));
			treeHMac.BlockUpdate(randomiser, 0, randomiser.Length);
			treeHMac.BlockUpdate(message, 0, message.Length);
			treeHMac.DoFinal(hmacBuf, 0);
			return Arrays.CopyOfRange(hmacBuf, 0, N);
		}

		private byte[] CompressedAdrs(Adrs adrs)
		{
			byte[] array = new byte[22];
			Array.Copy(adrs.value, Adrs.OFFSET_LAYER + 3, array, 0, 1);
			Array.Copy(adrs.value, Adrs.OFFSET_TREE + 4, array, 1, 8);
			Array.Copy(adrs.value, Adrs.OFFSET_TYPE + 3, array, 9, 1);
			Array.Copy(adrs.value, 20, array, 10, 12);
			return array;
		}

		protected byte[] Bitmask(byte[] key, byte[] m)
		{
			byte[] array = new byte[m.Length];
			mgf1.Init(new MgfParameters(key));
			mgf1.GenerateBytes(array, 0, array.Length);
			Bytes.XorTo(m.Length, m, array);
			return array;
		}

		protected byte[] Bitmask(byte[] key, byte[] m1, byte[] m2)
		{
			byte[] array = new byte[m1.Length + m2.Length];
			mgf1.Init(new MgfParameters(key));
			mgf1.GenerateBytes(array, 0, array.Length);
			Bytes.XorTo(m1.Length, m1, array);
			Bytes.XorTo(m2.Length, m2, 0, array, m1.Length);
			return array;
		}

		protected byte[] Bitmask256(byte[] key, byte[] m)
		{
			byte[] array = new byte[m.Length];
			Mgf1BytesGenerator mgf1BytesGenerator = new Mgf1BytesGenerator(new Sha256Digest());
			mgf1BytesGenerator.Init(new MgfParameters(key));
			mgf1BytesGenerator.GenerateBytes(array, 0, array.Length);
			Bytes.XorTo(m.Length, m, array);
			return array;
		}
	}

	internal class Shake256Engine : SphincsPlusEngine
	{
		private IXof treeDigest;

		private IXof maskDigest;

		public Shake256Engine(bool robust, int n, uint w, uint d, int a, int k, uint h)
			: base(robust, n, w, d, a, k, h)
		{
			treeDigest = new ShakeDigest(256);
			maskDigest = new ShakeDigest(256);
		}

		public override void Init(byte[] pkSeed)
		{
		}

		public override byte[] F(byte[] pkSeed, Adrs adrs, byte[] m1)
		{
			byte[] array = m1;
			if (robust)
			{
				array = Bitmask(pkSeed, adrs, m1);
			}
			byte[] array2 = new byte[N];
			treeDigest.BlockUpdate(pkSeed, 0, pkSeed.Length);
			treeDigest.BlockUpdate(adrs.value, 0, adrs.value.Length);
			treeDigest.BlockUpdate(array, 0, array.Length);
			treeDigest.OutputFinal(array2, 0, array2.Length);
			return array2;
		}

		public override void H(byte[] pkSeed, Adrs adrs, byte[] m1, byte[] m2, byte[] output)
		{
			treeDigest.BlockUpdate(pkSeed, 0, pkSeed.Length);
			treeDigest.BlockUpdate(adrs.value, 0, adrs.value.Length);
			if (robust)
			{
				byte[] array = Bitmask(pkSeed, adrs, m1, m2);
				treeDigest.BlockUpdate(array, 0, array.Length);
			}
			else
			{
				treeDigest.BlockUpdate(m1, 0, m1.Length);
				treeDigest.BlockUpdate(m2, 0, m2.Length);
			}
			treeDigest.OutputFinal(output, 0, N);
		}

		public override IndexedDigest H_msg(byte[] R, byte[] pkSeed, byte[] pkRoot, byte[] message)
		{
			int num = (A * K + 7) / 8;
			uint num2 = FH / D;
			uint num3 = FH - num2;
			uint num4 = (num2 + 7) / 8;
			uint num5 = (num3 + 7) / 8;
			byte[] array = new byte[(int)(num + num5 + num4)];
			treeDigest.BlockUpdate(R, 0, R.Length);
			treeDigest.BlockUpdate(pkSeed, 0, pkSeed.Length);
			treeDigest.BlockUpdate(pkRoot, 0, pkRoot.Length);
			treeDigest.BlockUpdate(message, 0, message.Length);
			treeDigest.OutputFinal(array, 0, array.Length);
			ulong idx_tree = Pack.BE_To_UInt64_Low(array, num, (int)num5) & (ulong.MaxValue >> (int)(64 - num3));
			uint idx_leaf = Pack.BE_To_UInt32_Low(array, num + (int)num5, (int)num4) & (uint)(-1 >>> (int)(32 - num2));
			return new IndexedDigest(idx_tree, idx_leaf, Arrays.CopyOfRange(array, 0, num));
		}

		public override void T_l(byte[] pkSeed, Adrs adrs, byte[] m, byte[] output)
		{
			byte[] array = m;
			if (robust)
			{
				array = Bitmask(pkSeed, adrs, m);
			}
			treeDigest.BlockUpdate(pkSeed, 0, pkSeed.Length);
			treeDigest.BlockUpdate(adrs.value, 0, adrs.value.Length);
			treeDigest.BlockUpdate(array, 0, array.Length);
			treeDigest.OutputFinal(output, 0, N);
		}

		public override void PRF(byte[] pkSeed, byte[] skSeed, Adrs adrs, byte[] prf, int prfOff)
		{
			treeDigest.BlockUpdate(pkSeed, 0, pkSeed.Length);
			treeDigest.BlockUpdate(adrs.value, 0, adrs.value.Length);
			treeDigest.BlockUpdate(skSeed, 0, skSeed.Length);
			treeDigest.OutputFinal(prf, prfOff, N);
		}

		public override byte[] PRF_msg(byte[] prf, byte[] randomiser, byte[] message)
		{
			treeDigest.BlockUpdate(prf, 0, prf.Length);
			treeDigest.BlockUpdate(randomiser, 0, randomiser.Length);
			treeDigest.BlockUpdate(message, 0, message.Length);
			byte[] array = new byte[N];
			treeDigest.OutputFinal(array, 0, array.Length);
			return array;
		}

		protected byte[] Bitmask(byte[] pkSeed, Adrs adrs, byte[] m)
		{
			byte[] array = new byte[m.Length];
			maskDigest.BlockUpdate(pkSeed, 0, pkSeed.Length);
			maskDigest.BlockUpdate(adrs.value, 0, adrs.value.Length);
			maskDigest.OutputFinal(array, 0, array.Length);
			Bytes.XorTo(m.Length, m, array);
			return array;
		}

		protected byte[] Bitmask(byte[] pkSeed, Adrs adrs, byte[] m1, byte[] m2)
		{
			byte[] array = new byte[m1.Length + m2.Length];
			maskDigest.BlockUpdate(pkSeed, 0, pkSeed.Length);
			maskDigest.BlockUpdate(adrs.value, 0, adrs.value.Length);
			maskDigest.OutputFinal(array, 0, array.Length);
			Bytes.XorTo(m1.Length, m1, array);
			Bytes.XorTo(m2.Length, m2, 0, array, m1.Length);
			return array;
		}
	}

	internal class HarakaSEngine : SphincsPlusEngine
	{
		public HarakaSXof harakaSXof;

		public HarakaS256Digest harakaS256Digest;

		public HarakaS512Digest harakaS512Digest;

		public HarakaSEngine(bool robust, int n, uint w, uint d, int a, int k, uint h)
			: base(robust, n, w, d, a, k, h)
		{
		}

		public override void Init(byte[] pkSeed)
		{
			harakaSXof = new HarakaSXof(pkSeed);
			harakaS256Digest = new HarakaS256Digest(harakaSXof);
			harakaS512Digest = new HarakaS512Digest(harakaSXof);
		}

		public override byte[] F(byte[] pkSeed, Adrs adrs, byte[] m1)
		{
			byte[] array = new byte[32];
			harakaS512Digest.BlockUpdate(adrs.value, 0, adrs.value.Length);
			if (robust)
			{
				harakaS256Digest.BlockUpdate(adrs.value, 0, adrs.value.Length);
				harakaS256Digest.DoFinal(array, 0);
				Bytes.XorTo(m1.Length, m1, array);
				harakaS512Digest.BlockUpdate(array, 0, m1.Length);
			}
			else
			{
				harakaS512Digest.BlockUpdate(m1, 0, m1.Length);
			}
			harakaS512Digest.DoFinal(array, 0);
			if (N != 32)
			{
				return Arrays.CopyOfRange(array, 0, N);
			}
			return array;
		}

		public override void H(byte[] pkSeed, Adrs adrs, byte[] m1, byte[] m2, byte[] output)
		{
			byte[] array = new byte[m1.Length + m2.Length];
			Array.Copy(m1, 0, array, 0, m1.Length);
			Array.Copy(m2, 0, array, m1.Length, m2.Length);
			if (robust)
			{
				Bitmask(adrs, array);
			}
			harakaSXof.BlockUpdate(adrs.value, 0, adrs.value.Length);
			harakaSXof.BlockUpdate(array, 0, array.Length);
			harakaSXof.OutputFinal(output, 0, N);
		}

		public override IndexedDigest H_msg(byte[] prf, byte[] pkSeed, byte[] pkRoot, byte[] message)
		{
			int num = A * K + 7 >> 3;
			uint num2 = FH / D;
			uint num3 = FH - num2;
			uint num4 = num2 + 7 >> 3;
			uint num5 = num3 + 7 >> 3;
			byte[] array = new byte[num + num5 + num4];
			harakaSXof.BlockUpdate(prf, 0, prf.Length);
			harakaSXof.BlockUpdate(pkRoot, 0, pkRoot.Length);
			harakaSXof.BlockUpdate(message, 0, message.Length);
			harakaSXof.OutputFinal(array, 0, array.Length);
			ulong idx_tree = Pack.BE_To_UInt64_Low(array, num, (int)num5) & (ulong.MaxValue >> (int)(64 - num3));
			uint idx_leaf = Pack.BE_To_UInt32_Low(array, num + (int)num5, (int)num4) & (uint)(-1 >>> (int)(32 - num2));
			return new IndexedDigest(idx_tree, idx_leaf, Arrays.CopyOfRange(array, 0, num));
		}

		public override void T_l(byte[] pkSeed, Adrs adrs, byte[] m, byte[] output)
		{
			if (robust)
			{
				Bitmask(adrs, m);
			}
			harakaSXof.BlockUpdate(adrs.value, 0, adrs.value.Length);
			harakaSXof.BlockUpdate(m, 0, m.Length);
			harakaSXof.OutputFinal(output, 0, N);
		}

		public override void PRF(byte[] pkSeed, byte[] skSeed, Adrs adrs, byte[] prf, int prfOff)
		{
			byte[] array = new byte[32];
			harakaS512Digest.BlockUpdate(adrs.value, 0, adrs.value.Length);
			harakaS512Digest.BlockUpdate(skSeed, 0, skSeed.Length);
			harakaS512Digest.DoFinal(array, 0);
			Array.Copy(array, 0, prf, prfOff, N);
		}

		public override byte[] PRF_msg(byte[] prf, byte[] randomiser, byte[] message)
		{
			byte[] array = new byte[N];
			harakaSXof.BlockUpdate(prf, 0, prf.Length);
			harakaSXof.BlockUpdate(randomiser, 0, randomiser.Length);
			harakaSXof.BlockUpdate(message, 0, message.Length);
			harakaSXof.OutputFinal(array, 0, array.Length);
			return array;
		}

		protected void Bitmask(Adrs adrs, byte[] m)
		{
			byte[] array = new byte[m.Length];
			harakaSXof.BlockUpdate(adrs.value, 0, adrs.value.Length);
			harakaSXof.OutputFinal(array, 0, array.Length);
			Bytes.XorTo(m.Length, array, m);
		}
	}

	internal bool robust;

	internal int N;

	internal uint WOTS_W;

	internal int WOTS_LOGW;

	internal int WOTS_LEN;

	internal int WOTS_LEN1;

	internal int WOTS_LEN2;

	internal uint D;

	internal int A;

	internal int K;

	internal uint FH;

	internal uint H_PRIME;

	internal uint T;

	internal SphincsPlusEngine(bool robust, int n, uint w, uint d, int a, int k, uint h)
	{
		N = n;
		switch (w)
		{
		case 16u:
			WOTS_LOGW = 4;
			WOTS_LEN1 = 8 * N / WOTS_LOGW;
			if (N <= 8)
			{
				WOTS_LEN2 = 2;
				break;
			}
			if (N <= 136)
			{
				WOTS_LEN2 = 3;
				break;
			}
			if (N <= 256)
			{
				WOTS_LEN2 = 4;
				break;
			}
			throw new ArgumentException("cannot precompute SPX_WOTS_LEN2 for n outside {2, .., 256}");
		case 256u:
			WOTS_LOGW = 8;
			WOTS_LEN1 = 8 * N / WOTS_LOGW;
			if (N <= 1)
			{
				WOTS_LEN2 = 1;
				break;
			}
			if (N <= 256)
			{
				WOTS_LEN2 = 2;
				break;
			}
			throw new ArgumentException("cannot precompute SPX_WOTS_LEN2 for n outside {2, .., 256}");
		default:
			throw new ArgumentException("wots_w assumed 16 or 256");
		}
		WOTS_W = w;
		WOTS_LEN = WOTS_LEN1 + WOTS_LEN2;
		this.robust = robust;
		D = d;
		A = a;
		K = k;
		FH = h;
		H_PRIME = h / d;
		T = (uint)(1 << a);
	}

	public abstract void Init(byte[] pkSeed);

	public abstract byte[] F(byte[] pkSeed, Adrs adrs, byte[] m1);

	public abstract void H(byte[] pkSeed, Adrs adrs, byte[] m1, byte[] m2, byte[] output);

	public abstract IndexedDigest H_msg(byte[] prf, byte[] pkSeed, byte[] pkRoot, byte[] message);

	public abstract void T_l(byte[] pkSeed, Adrs adrs, byte[] m, byte[] output);

	public abstract void PRF(byte[] pkSeed, byte[] skSeed, Adrs adrs, byte[] prf, int prfOff);

	public abstract byte[] PRF_msg(byte[] prf, byte[] randomiser, byte[] message);
}
