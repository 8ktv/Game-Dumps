namespace Mirror.BouncyCastle.Crypto;

public interface ISigner
{
	string AlgorithmName { get; }

	void Init(bool forSigning, ICipherParameters parameters);

	void Update(byte input);

	void BlockUpdate(byte[] input, int inOff, int inLen);

	int GetMaxSignatureSize();

	byte[] GenerateSignature();

	bool VerifySignature(byte[] signature);

	void Reset();
}
