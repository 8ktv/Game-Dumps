using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crmf;

internal sealed class PKMacFactory : IMacFactory
{
	private readonly KeyParameter m_key;

	private readonly PbmParameter m_parameters;

	public object AlgorithmDetails => new AlgorithmIdentifier(CmpObjectIdentifiers.passwordBasedMac, m_parameters);

	internal PKMacFactory(byte[] key, PbmParameter parameters)
	{
		m_key = new KeyParameter(key);
		m_parameters = parameters;
	}

	public IStreamCalculator<IBlockResult> CreateCalculator()
	{
		IMac mac = MacUtilities.GetMac(m_parameters.Mac.Algorithm);
		mac.Init(m_key);
		return new DefaultMacCalculator(mac);
	}
}
