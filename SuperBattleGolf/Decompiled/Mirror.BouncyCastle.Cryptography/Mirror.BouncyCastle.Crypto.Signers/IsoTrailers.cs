using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Crypto.Signers;

public class IsoTrailers
{
	public const int TRAILER_IMPLICIT = 188;

	public const int TRAILER_RIPEMD160 = 12748;

	public const int TRAILER_RIPEMD128 = 13004;

	public const int TRAILER_SHA1 = 13260;

	public const int TRAILER_SHA256 = 13516;

	public const int TRAILER_SHA512 = 13772;

	public const int TRAILER_SHA384 = 14028;

	public const int TRAILER_WHIRLPOOL = 14284;

	public const int TRAILER_SHA224 = 14540;

	public const int TRAILER_SHA512_224 = 14796;

	public const int TRAILER_SHA512_256 = 16588;

	private static readonly IDictionary<string, int> TrailerMap = CreateTrailerMap();

	private static IDictionary<string, int> CreateTrailerMap()
	{
		return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
		{
			{ "RIPEMD128", 13004 },
			{ "RIPEMD160", 12748 },
			{ "SHA-1", 13260 },
			{ "SHA-224", 14540 },
			{ "SHA-256", 13516 },
			{ "SHA-384", 14028 },
			{ "SHA-512", 13772 },
			{ "SHA-512/224", 14796 },
			{ "SHA-512/256", 16588 },
			{ "Whirlpool", 14284 }
		};
	}

	public static int GetTrailer(IDigest digest)
	{
		if (TrailerMap.TryGetValue(digest.AlgorithmName, out var value))
		{
			return value;
		}
		throw new InvalidOperationException("No trailer for digest");
	}

	public static bool NoTrailerAvailable(IDigest digest)
	{
		return !TrailerMap.ContainsKey(digest.AlgorithmName);
	}
}
