using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Operators;

public class Asn1DigestFactory : IDigestFactory
{
	private readonly IDigest m_digest;

	private readonly DerObjectIdentifier m_oid;

	public virtual object AlgorithmDetails => new AlgorithmIdentifier(m_oid);

	public virtual int DigestLength => m_digest.GetDigestSize();

	public static Asn1DigestFactory Get(DerObjectIdentifier oid)
	{
		return new Asn1DigestFactory(DigestUtilities.GetDigest(oid), oid);
	}

	public static Asn1DigestFactory Get(string mechanism)
	{
		return Get(DigestUtilities.GetObjectIdentifier(mechanism));
	}

	public Asn1DigestFactory(IDigest digest, DerObjectIdentifier oid)
	{
		m_digest = digest;
		m_oid = oid;
	}

	public virtual IStreamCalculator<IBlockResult> CreateCalculator()
	{
		return new DefaultDigestCalculator(m_digest);
	}
}
