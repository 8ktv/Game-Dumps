namespace Mirror.BouncyCastle.Crypto;

public interface IXof : IDigest
{
	int OutputFinal(byte[] output, int outOff, int outLen);

	int Output(byte[] output, int outOff, int outLen);
}
