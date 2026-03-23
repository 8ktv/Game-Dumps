using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

public sealed class SphincsPlusParameters
{
	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters sha2_128f;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters sha2_128s;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters sha2_192f;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters sha2_192s;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters sha2_256f;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters sha2_256s;

	public static readonly SphincsPlusParameters sha2_128f_simple;

	public static readonly SphincsPlusParameters sha2_128s_simple;

	public static readonly SphincsPlusParameters sha2_192f_simple;

	public static readonly SphincsPlusParameters sha2_192s_simple;

	public static readonly SphincsPlusParameters sha2_256f_simple;

	public static readonly SphincsPlusParameters sha2_256s_simple;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters shake_128f;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters shake_128s;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters shake_192f;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters shake_192s;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters shake_256f;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters shake_256s;

	public static readonly SphincsPlusParameters shake_128f_simple;

	public static readonly SphincsPlusParameters shake_128s_simple;

	public static readonly SphincsPlusParameters shake_192f_simple;

	public static readonly SphincsPlusParameters shake_192s_simple;

	public static readonly SphincsPlusParameters shake_256f_simple;

	public static readonly SphincsPlusParameters shake_256s_simple;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters haraka_128f;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters haraka_128s;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters haraka_192f;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters haraka_192s;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters haraka_256f;

	[Obsolete("Parameter set to be removed")]
	public static readonly SphincsPlusParameters haraka_256s;

	public static readonly SphincsPlusParameters haraka_128f_simple;

	public static readonly SphincsPlusParameters haraka_128s_simple;

	public static readonly SphincsPlusParameters haraka_192f_simple;

	public static readonly SphincsPlusParameters haraka_192s_simple;

	public static readonly SphincsPlusParameters haraka_256f_simple;

	public static readonly SphincsPlusParameters haraka_256s_simple;

	private static readonly Dictionary<int, SphincsPlusParameters> IdToParams;

	private readonly int m_id;

	private readonly string m_name;

	private readonly ISphincsPlusEngineProvider m_engineProvider;

	public int ID => m_id;

	public string Name => m_name;

	internal int N => m_engineProvider.N;

	static SphincsPlusParameters()
	{
		sha2_128f = new SphincsPlusParameters(65793, "sha2-128f-robust", new Sha2EngineProvider(robust: true, 16, 16u, 22u, 6, 33, 66u));
		sha2_128s = new SphincsPlusParameters(65794, "sha2-128s-robust", new Sha2EngineProvider(robust: true, 16, 16u, 7u, 12, 14, 63u));
		sha2_192f = new SphincsPlusParameters(65795, "sha2-192f-robust", new Sha2EngineProvider(robust: true, 24, 16u, 22u, 8, 33, 66u));
		sha2_192s = new SphincsPlusParameters(65796, "sha2-192s-robust", new Sha2EngineProvider(robust: true, 24, 16u, 7u, 14, 17, 63u));
		sha2_256f = new SphincsPlusParameters(65797, "sha2-256f-robust", new Sha2EngineProvider(robust: true, 32, 16u, 17u, 9, 35, 68u));
		sha2_256s = new SphincsPlusParameters(65798, "sha2-256s-robust", new Sha2EngineProvider(robust: true, 32, 16u, 8u, 14, 22, 64u));
		sha2_128f_simple = new SphincsPlusParameters(66049, "sha2-128f-simple", new Sha2EngineProvider(robust: false, 16, 16u, 22u, 6, 33, 66u));
		sha2_128s_simple = new SphincsPlusParameters(66050, "sha2-128s-simple", new Sha2EngineProvider(robust: false, 16, 16u, 7u, 12, 14, 63u));
		sha2_192f_simple = new SphincsPlusParameters(66051, "sha2-192f-simple", new Sha2EngineProvider(robust: false, 24, 16u, 22u, 8, 33, 66u));
		sha2_192s_simple = new SphincsPlusParameters(66052, "sha2-192s-simple", new Sha2EngineProvider(robust: false, 24, 16u, 7u, 14, 17, 63u));
		sha2_256f_simple = new SphincsPlusParameters(66053, "sha2-256f-simple", new Sha2EngineProvider(robust: false, 32, 16u, 17u, 9, 35, 68u));
		sha2_256s_simple = new SphincsPlusParameters(66054, "sha2-256s-simple", new Sha2EngineProvider(robust: false, 32, 16u, 8u, 14, 22, 64u));
		shake_128f = new SphincsPlusParameters(131329, "shake-128f-robust", new Shake256EngineProvider(robust: true, 16, 16u, 22u, 6, 33, 66u));
		shake_128s = new SphincsPlusParameters(131330, "shake-128s-robust", new Shake256EngineProvider(robust: true, 16, 16u, 7u, 12, 14, 63u));
		shake_192f = new SphincsPlusParameters(131331, "shake-192f-robust", new Shake256EngineProvider(robust: true, 24, 16u, 22u, 8, 33, 66u));
		shake_192s = new SphincsPlusParameters(131332, "shake-192s-robust", new Shake256EngineProvider(robust: true, 24, 16u, 7u, 14, 17, 63u));
		shake_256f = new SphincsPlusParameters(131333, "shake-256f-robust", new Shake256EngineProvider(robust: true, 32, 16u, 17u, 9, 35, 68u));
		shake_256s = new SphincsPlusParameters(131334, "shake-256s-robust", new Shake256EngineProvider(robust: true, 32, 16u, 8u, 14, 22, 64u));
		shake_128f_simple = new SphincsPlusParameters(131585, "shake-128f-simple", new Shake256EngineProvider(robust: false, 16, 16u, 22u, 6, 33, 66u));
		shake_128s_simple = new SphincsPlusParameters(131586, "shake-128s-simple", new Shake256EngineProvider(robust: false, 16, 16u, 7u, 12, 14, 63u));
		shake_192f_simple = new SphincsPlusParameters(131587, "shake-192f-simple", new Shake256EngineProvider(robust: false, 24, 16u, 22u, 8, 33, 66u));
		shake_192s_simple = new SphincsPlusParameters(131588, "shake-192s-simple", new Shake256EngineProvider(robust: false, 24, 16u, 7u, 14, 17, 63u));
		shake_256f_simple = new SphincsPlusParameters(131589, "shake-256f-simple", new Shake256EngineProvider(robust: false, 32, 16u, 17u, 9, 35, 68u));
		shake_256s_simple = new SphincsPlusParameters(131590, "shake-256s-simple", new Shake256EngineProvider(robust: false, 32, 16u, 8u, 14, 22, 64u));
		haraka_128f = new SphincsPlusParameters(196865, "haraka-128f-robust", new HarakaSEngineProvider(robust: true, 16, 16u, 22u, 6, 33, 66u));
		haraka_128s = new SphincsPlusParameters(196866, "haraka-128s-robust", new HarakaSEngineProvider(robust: true, 16, 16u, 7u, 12, 14, 63u));
		haraka_192f = new SphincsPlusParameters(196867, "haraka-192f-robust", new HarakaSEngineProvider(robust: true, 24, 16u, 22u, 8, 33, 66u));
		haraka_192s = new SphincsPlusParameters(196868, "haraka-192s-robust", new HarakaSEngineProvider(robust: true, 24, 16u, 7u, 14, 17, 63u));
		haraka_256f = new SphincsPlusParameters(196869, "haraka-256f-robust", new HarakaSEngineProvider(robust: true, 32, 16u, 17u, 9, 35, 68u));
		haraka_256s = new SphincsPlusParameters(196870, "haraka-256s-robust", new HarakaSEngineProvider(robust: true, 32, 16u, 8u, 14, 22, 64u));
		haraka_128f_simple = new SphincsPlusParameters(197121, "haraka-128f-simple", new HarakaSEngineProvider(robust: false, 16, 16u, 22u, 6, 33, 66u));
		haraka_128s_simple = new SphincsPlusParameters(197122, "haraka-128s-simple", new HarakaSEngineProvider(robust: false, 16, 16u, 7u, 12, 14, 63u));
		haraka_192f_simple = new SphincsPlusParameters(197123, "haraka-192f-simple", new HarakaSEngineProvider(robust: false, 24, 16u, 22u, 8, 33, 66u));
		haraka_192s_simple = new SphincsPlusParameters(197124, "haraka-192s-simple", new HarakaSEngineProvider(robust: false, 24, 16u, 7u, 14, 17, 63u));
		haraka_256f_simple = new SphincsPlusParameters(197125, "haraka-256f-simple", new HarakaSEngineProvider(robust: false, 32, 16u, 17u, 9, 35, 68u));
		haraka_256s_simple = new SphincsPlusParameters(197126, "haraka-256s-simple", new HarakaSEngineProvider(robust: false, 32, 16u, 8u, 14, 22, 64u));
		IdToParams = new Dictionary<int, SphincsPlusParameters>();
		SphincsPlusParameters[] array = new SphincsPlusParameters[36]
		{
			sha2_128f, sha2_128s, sha2_192f, sha2_192s, sha2_256f, sha2_256s, sha2_128f_simple, sha2_128s_simple, sha2_192f_simple, sha2_192s_simple,
			sha2_256f_simple, sha2_256s_simple, shake_128f, shake_128s, shake_192f, shake_192s, shake_256f, shake_256s, shake_128f_simple, shake_128s_simple,
			shake_192f_simple, shake_192s_simple, shake_256f_simple, shake_256s_simple, haraka_128f, haraka_128s, haraka_192f, haraka_192s, haraka_256f, haraka_256s,
			haraka_128f_simple, haraka_128s_simple, haraka_192f_simple, haraka_192s_simple, haraka_256f_simple, haraka_256s_simple
		};
		foreach (SphincsPlusParameters sphincsPlusParameters in array)
		{
			IdToParams.Add(sphincsPlusParameters.ID, sphincsPlusParameters);
		}
	}

	private SphincsPlusParameters(int id, string name, ISphincsPlusEngineProvider engineProvider)
	{
		m_id = id;
		m_name = name;
		m_engineProvider = engineProvider;
	}

	internal SphincsPlusEngine GetEngine()
	{
		return m_engineProvider.Get();
	}

	public static SphincsPlusParameters GetParams(int id)
	{
		return CollectionUtilities.GetValueOrNull(IdToParams, id);
	}

	[Obsolete("Use 'ID' property instead")]
	public static int GetID(SphincsPlusParameters parameters)
	{
		return parameters.ID;
	}

	public byte[] GetEncoded()
	{
		return Pack.UInt32_To_BE((uint)ID);
	}
}
