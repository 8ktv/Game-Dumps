using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class HssKeyGenerationParameters : KeyGenerationParameters
{
	private readonly LmsParameters[] m_lmsParameters;

	public int Depth => m_lmsParameters.Length;

	private static LmsParameters[] ValidateLmsParameters(LmsParameters[] lmsParameters)
	{
		if (lmsParameters == null)
		{
			throw new ArgumentNullException("lmsParameters");
		}
		if (lmsParameters.Length < 1 || lmsParameters.Length > 8)
		{
			throw new ArgumentException("length should be between 1 and 8 inclusive", "lmsParameters");
		}
		return lmsParameters;
	}

	public HssKeyGenerationParameters(LmsParameters[] lmsParameters, SecureRandom random)
		: base(random, LmsUtilities.CalculateStrength(ValidateLmsParameters(lmsParameters)[0]))
	{
		m_lmsParameters = lmsParameters;
	}

	public LmsParameters GetLmsParameters(int index)
	{
		if (index < 0 || index >= m_lmsParameters.Length)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		return m_lmsParameters[index];
	}
}
