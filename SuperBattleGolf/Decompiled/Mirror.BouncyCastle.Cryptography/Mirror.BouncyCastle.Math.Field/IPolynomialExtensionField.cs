namespace Mirror.BouncyCastle.Math.Field;

public interface IPolynomialExtensionField : IExtensionField, IFiniteField
{
	IPolynomial MinimalPolynomial { get; }
}
