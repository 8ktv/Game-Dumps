using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

public sealed class FalconPublicKeyParameters : FalconKeyParameters
{
	private readonly byte[] publicKey;

	public FalconPublicKeyParameters(FalconParameters parameters, byte[] h)
		: base(isprivate: false, parameters)
	{
		publicKey = Arrays.Clone(h);
	}

	public byte[] GetEncoded()
	{
		return Arrays.Clone(publicKey);
	}
}
