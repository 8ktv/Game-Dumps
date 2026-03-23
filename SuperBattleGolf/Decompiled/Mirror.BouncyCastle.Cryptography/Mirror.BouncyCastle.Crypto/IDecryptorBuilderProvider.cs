namespace Mirror.BouncyCastle.Crypto;

public interface IDecryptorBuilderProvider
{
	ICipherBuilder CreateDecryptorBuilder(object algorithmDetails);
}
