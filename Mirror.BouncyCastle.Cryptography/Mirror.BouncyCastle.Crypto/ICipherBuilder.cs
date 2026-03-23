using System.IO;

namespace Mirror.BouncyCastle.Crypto;

public interface ICipherBuilder
{
	object AlgorithmDetails { get; }

	int GetMaxOutputSize(int inputLen);

	ICipher BuildCipher(Stream stream);
}
