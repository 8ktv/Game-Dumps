using System;
using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

public sealed class CmceParameters : ICipherParameters
{
	private static readonly int[] poly3488 = new int[3] { 3, 1, 0 };

	private static readonly int[] poly4608 = new int[4] { 10, 9, 6, 0 };

	private static readonly int[] poly6688 = new int[4] { 7, 2, 1, 0 };

	private static readonly int[] poly6960 = new int[2] { 8, 0 };

	private static readonly int[] poly8192 = new int[4] { 7, 2, 1, 0 };

	public static readonly CmceParameters mceliece348864r3 = new CmceParameters("mceliece348864", 12, 3488, 64, poly3488, usePivots: false, 128);

	public static readonly CmceParameters mceliece348864fr3 = new CmceParameters("mceliece348864f", 12, 3488, 64, poly3488, usePivots: true, 128);

	public static readonly CmceParameters mceliece460896r3 = new CmceParameters("mceliece460896", 13, 4608, 96, poly4608, usePivots: false, 192);

	public static readonly CmceParameters mceliece460896fr3 = new CmceParameters("mceliece460896f", 13, 4608, 96, poly4608, usePivots: true, 192);

	public static readonly CmceParameters mceliece6688128r3 = new CmceParameters("mceliece6688128", 13, 6688, 128, poly6688, usePivots: false, 256);

	public static readonly CmceParameters mceliece6688128fr3 = new CmceParameters("mceliece6688128f", 13, 6688, 128, poly6688, usePivots: true, 256);

	public static readonly CmceParameters mceliece6960119r3 = new CmceParameters("mceliece6960119", 13, 6960, 119, poly6960, usePivots: false, 256);

	public static readonly CmceParameters mceliece6960119fr3 = new CmceParameters("mceliece6960119f", 13, 6960, 119, poly6960, usePivots: true, 256);

	public static readonly CmceParameters mceliece8192128r3 = new CmceParameters("mceliece8192128", 13, 8192, 128, poly8192, usePivots: false, 256);

	public static readonly CmceParameters mceliece8192128fr3 = new CmceParameters("mceliece8192128f", 13, 8192, 128, poly8192, usePivots: true, 256);

	private readonly string name;

	private readonly int m;

	private readonly int n;

	private readonly int t;

	private readonly bool usePivots;

	private readonly int defaultKeySize;

	private readonly ICmceEngine engine;

	public string Name => name;

	public int M => m;

	public int N => n;

	public int T => t;

	public int Mu
	{
		get
		{
			if (!usePivots)
			{
				return 0;
			}
			return 32;
		}
	}

	public int Nu
	{
		get
		{
			if (!usePivots)
			{
				return 0;
			}
			return 64;
		}
	}

	public int DefaultKeySize => defaultKeySize;

	internal ICmceEngine Engine => engine;

	private CmceParameters(string name, int m, int n, int t, int[] p, bool usePivots, int defaultKeySize)
	{
		this.name = name;
		this.m = m;
		this.n = n;
		this.t = t;
		this.usePivots = usePivots;
		this.defaultKeySize = defaultKeySize;
		switch (m)
		{
		case 12:
			engine = new CmceEngine<GF12>(m, n, t, p, usePivots, defaultKeySize);
			break;
		case 13:
			engine = new CmceEngine<GF13>(m, n, t, p, usePivots, defaultKeySize);
			break;
		default:
			throw new ArgumentException();
		}
	}
}
