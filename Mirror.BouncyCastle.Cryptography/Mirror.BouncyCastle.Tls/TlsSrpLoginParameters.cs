using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Tls.Crypto;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls;

public class TlsSrpLoginParameters
{
	protected byte[] m_identity;

	protected TlsSrpConfig m_srpConfig;

	protected BigInteger m_verifier;

	protected byte[] m_salt;

	public virtual TlsSrpConfig Config => m_srpConfig;

	public virtual byte[] Identity => m_identity;

	public virtual byte[] Salt => m_salt;

	public virtual BigInteger Verifier => m_verifier;

	public TlsSrpLoginParameters(byte[] identity, TlsSrpConfig srpConfig, BigInteger verifier, byte[] salt)
	{
		m_identity = Arrays.Clone(identity);
		m_srpConfig = srpConfig;
		m_verifier = verifier;
		m_salt = Arrays.Clone(salt);
	}
}
