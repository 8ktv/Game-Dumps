using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru;

public class NtruKeyGenerationParameters : KeyGenerationParameters
{
	internal NtruParameters NtruParameters { get; }

	public NtruKeyGenerationParameters(SecureRandom random, NtruParameters ntruParameters)
		: base(random, 1)
	{
		NtruParameters = ntruParameters;
	}

	public NtruParameters GetParameters()
	{
		return NtruParameters;
	}
}
