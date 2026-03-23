namespace Mirror.BouncyCastle.Crypto;

public interface IEncapsulatedSecretExtractor
{
	int EncapsulationLength { get; }

	byte[] ExtractSecret(byte[] encapsulation);
}
