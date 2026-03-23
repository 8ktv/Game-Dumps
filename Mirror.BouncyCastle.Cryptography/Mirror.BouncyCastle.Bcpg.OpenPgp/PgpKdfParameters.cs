namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

internal sealed class PgpKdfParameters
{
	private readonly HashAlgorithmTag m_hashAlgorithm;

	private readonly SymmetricKeyAlgorithmTag m_symmetricWrapAlgorithm;

	public HashAlgorithmTag HashAlgorithm => m_hashAlgorithm;

	public SymmetricKeyAlgorithmTag SymmetricWrapAlgorithm => m_symmetricWrapAlgorithm;

	public PgpKdfParameters(HashAlgorithmTag hashAlgorithm, SymmetricKeyAlgorithmTag symmetricWrapAlgorithm)
	{
		m_hashAlgorithm = hashAlgorithm;
		m_symmetricWrapAlgorithm = symmetricWrapAlgorithm;
	}
}
