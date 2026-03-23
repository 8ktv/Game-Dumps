using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public class PskTlsServer : AbstractTlsServer
{
	private static readonly int[] DefaultCipherSuites = new int[12]
	{
		52396, 49208, 49207, 49206, 49205, 52397, 171, 170, 179, 178,
		145, 144
	};

	protected readonly TlsPskIdentityManager m_pskIdentityManager;

	public PskTlsServer(TlsCrypto crypto, TlsPskIdentityManager pskIdentityManager)
		: base(crypto)
	{
		m_pskIdentityManager = pskIdentityManager;
	}

	protected virtual TlsCredentialedDecryptor GetRsaEncryptionCredentials()
	{
		throw new TlsFatalAlert(80);
	}

	protected override ProtocolVersion[] GetSupportedVersions()
	{
		return ProtocolVersion.TLSv12.Only();
	}

	protected override int[] GetSupportedCipherSuites()
	{
		return TlsUtilities.GetSupportedCipherSuites(Crypto, DefaultCipherSuites);
	}

	public override TlsCredentials GetCredentials()
	{
		switch (m_context.SecurityParameters.KeyExchangeAlgorithm)
		{
		case 13:
		case 14:
		case 24:
			return null;
		case 15:
			return GetRsaEncryptionCredentials();
		default:
			throw new TlsFatalAlert(80);
		}
	}

	public override TlsPskIdentityManager GetPskIdentityManager()
	{
		return m_pskIdentityManager;
	}
}
