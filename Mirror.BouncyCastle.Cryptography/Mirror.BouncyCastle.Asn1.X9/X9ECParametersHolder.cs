using Mirror.BouncyCastle.Math.EC;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.X9;

public abstract class X9ECParametersHolder
{
	private ECCurve m_curve;

	private X9ECParameters m_parameters;

	public ECCurve Curve => Objects.EnsureSingletonInitialized(ref m_curve, this, (X9ECParametersHolder self) => self.CreateCurve());

	public X9ECParameters Parameters => Objects.EnsureSingletonInitialized(ref m_parameters, this, (X9ECParametersHolder self) => self.CreateParameters());

	protected virtual ECCurve CreateCurve()
	{
		return Parameters.Curve;
	}

	protected abstract X9ECParameters CreateParameters();
}
