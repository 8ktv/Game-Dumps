namespace Mirror.BouncyCastle.Crypto.Modes;

public interface IBlockCipherMode : IBlockCipher
{
	IBlockCipher UnderlyingCipher { get; }

	bool IsPartialBlockOkay { get; }

	void Reset();
}
