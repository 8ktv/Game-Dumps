using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Hqc;

public sealed class HqcPrivateKeyParameters : HqcKeyParameters
{
	private byte[] sk;

	public byte[] PrivateKey => Arrays.Clone(sk);

	public HqcPrivateKeyParameters(HqcParameters param, byte[] sk)
		: base(isPrivate: true, param)
	{
		this.sk = Arrays.Clone(sk);
	}

	public byte[] GetEncoded()
	{
		return PrivateKey;
	}
}
