namespace Mirror.BouncyCastle.Crypto.Modes;

public interface IAeadBlockCipher : IAeadCipher
{
	IBlockCipher UnderlyingCipher { get; }

	int GetBlockSize();
}
