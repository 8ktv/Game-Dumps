using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Hqc;

public abstract class HqcKeyParameters : AsymmetricKeyParameter
{
	private readonly HqcParameters m_parameters;

	public HqcParameters Parameters => m_parameters;

	internal HqcKeyParameters(bool isPrivate, HqcParameters parameters)
		: base(isPrivate)
	{
		m_parameters = parameters;
	}
}
