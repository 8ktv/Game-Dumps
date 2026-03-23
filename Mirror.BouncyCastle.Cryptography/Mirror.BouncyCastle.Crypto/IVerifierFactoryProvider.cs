namespace Mirror.BouncyCastle.Crypto;

public interface IVerifierFactoryProvider
{
	IVerifierFactory CreateVerifierFactory(object algorithmDetails);
}
