using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public abstract class DefaultTlsClient : AbstractTlsClient
{
	private static readonly int[] DefaultCipherSuites = new int[17]
	{
		4865, 4867, 49195, 52393, 49187, 49161, 49199, 52392, 49191, 49171,
		158, 52394, 103, 51, 156, 60, 47
	};

	public DefaultTlsClient(TlsCrypto crypto)
		: base(crypto)
	{
	}

	protected override int[] GetSupportedCipherSuites()
	{
		return TlsUtilities.GetSupportedCipherSuites(Crypto, DefaultCipherSuites);
	}
}
