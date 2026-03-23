namespace Mirror.BouncyCastle.Crypto.Prng.Drbg;

public interface ISP80090Drbg
{
	int BlockSize { get; }

	int Generate(byte[] output, int outputOff, int outputLen, byte[] additionalInput, bool predictionResistant);

	void Reseed(byte[] additionalInput);
}
