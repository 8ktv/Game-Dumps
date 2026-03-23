namespace Mirror.BouncyCastle.Crypto;

public interface IBlockResult
{
	byte[] Collect();

	int Collect(byte[] buf, int off);

	int GetMaxResultLength();
}
