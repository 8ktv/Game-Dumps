namespace Mirror.BouncyCastle.Crypto;

public interface ISignatureFactory
{
	object AlgorithmDetails { get; }

	IStreamCalculator<IBlockResult> CreateCalculator();
}
