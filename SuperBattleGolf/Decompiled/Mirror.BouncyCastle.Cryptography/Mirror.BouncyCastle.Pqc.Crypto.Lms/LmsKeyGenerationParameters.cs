using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public class LmsKeyGenerationParameters : KeyGenerationParameters
{
	private readonly LmsParameters m_lmsParameters;

	public LmsParameters LmsParameters => m_lmsParameters;

	public LmsKeyGenerationParameters(LmsParameters lmsParameters, SecureRandom random)
		: base(random, LmsUtilities.CalculateStrength(lmsParameters))
	{
		m_lmsParameters = lmsParameters;
	}
}
