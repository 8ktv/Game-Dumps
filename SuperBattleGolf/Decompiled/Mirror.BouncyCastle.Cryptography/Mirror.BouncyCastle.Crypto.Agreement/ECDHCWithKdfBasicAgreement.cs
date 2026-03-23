using System;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto.Agreement;

public sealed class ECDHCWithKdfBasicAgreement : ECDHCBasicAgreement
{
	private readonly string m_algorithm;

	private readonly IDerivationFunction m_kdf;

	public ECDHCWithKdfBasicAgreement(string algorithm, IDerivationFunction kdf)
	{
		m_algorithm = algorithm ?? throw new ArgumentNullException("algorithm");
		m_kdf = kdf ?? throw new ArgumentNullException("kdf");
	}

	public override BigInteger CalculateAgreement(ICipherParameters pubKey)
	{
		BigInteger result = base.CalculateAgreement(pubKey);
		return BasicAgreementWithKdf.CalculateAgreementWithKdf(m_algorithm, m_kdf, GetFieldSize(), result);
	}
}
