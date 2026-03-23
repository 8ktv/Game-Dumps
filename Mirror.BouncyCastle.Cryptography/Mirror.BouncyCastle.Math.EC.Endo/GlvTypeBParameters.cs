namespace Mirror.BouncyCastle.Math.EC.Endo;

public class GlvTypeBParameters
{
	protected readonly BigInteger m_beta;

	protected readonly BigInteger m_lambda;

	protected readonly ScalarSplitParameters m_splitParams;

	public virtual BigInteger Beta => m_beta;

	public virtual BigInteger Lambda => m_lambda;

	public virtual ScalarSplitParameters SplitParams => m_splitParams;

	public GlvTypeBParameters(BigInteger beta, BigInteger lambda, ScalarSplitParameters splitParams)
	{
		m_beta = beta;
		m_lambda = lambda;
		m_splitParams = splitParams;
	}
}
