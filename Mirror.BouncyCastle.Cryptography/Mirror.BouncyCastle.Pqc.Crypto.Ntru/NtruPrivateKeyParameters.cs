namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru;

public sealed class NtruPrivateKeyParameters : NtruKeyParameters
{
	private byte[] _privateKey;

	public byte[] PrivateKey
	{
		get
		{
			return (byte[])_privateKey.Clone();
		}
		private set
		{
			_privateKey = (byte[])value.Clone();
		}
	}

	public NtruPrivateKeyParameters(NtruParameters parameters, byte[] key)
		: base(privateKey: true, parameters)
	{
		PrivateKey = key;
	}

	public override byte[] GetEncoded()
	{
		return PrivateKey;
	}
}
