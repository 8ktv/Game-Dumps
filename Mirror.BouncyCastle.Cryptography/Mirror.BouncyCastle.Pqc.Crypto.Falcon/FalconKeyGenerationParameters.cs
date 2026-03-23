using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

public class FalconKeyGenerationParameters : KeyGenerationParameters
{
	private FalconParameters parameters;

	public FalconParameters Parameters => parameters;

	public FalconKeyGenerationParameters(SecureRandom random, FalconParameters parameters)
		: base(random, 320)
	{
		this.parameters = parameters;
	}
}
