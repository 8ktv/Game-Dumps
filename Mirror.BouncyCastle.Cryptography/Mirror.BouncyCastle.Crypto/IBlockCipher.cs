namespace Mirror.BouncyCastle.Crypto;

public interface IBlockCipher
{
	string AlgorithmName { get; }

	void Init(bool forEncryption, ICipherParameters parameters);

	int GetBlockSize();

	int ProcessBlock(byte[] inBuf, int inOff, byte[] outBuf, int outOff);
}
