namespace Mirror.BouncyCastle.Crypto;

public interface IEntropySourceProvider
{
	IEntropySource Get(int bitsRequired);
}
