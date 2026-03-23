using System.IO;
using Mirror.BouncyCastle.Crypto.IO;

namespace Mirror.BouncyCastle.Crypto.Operators;

public class BufferedCipherWrapper : ICipher
{
	private readonly IBufferedCipher bufferedCipher;

	private readonly CipherStream stream;

	public Stream Stream => stream;

	public BufferedCipherWrapper(IBufferedCipher bufferedCipher, Stream source)
	{
		this.bufferedCipher = bufferedCipher;
		stream = new CipherStream(source, bufferedCipher, bufferedCipher);
	}

	public int GetMaxOutputSize(int inputLen)
	{
		return bufferedCipher.GetOutputSize(inputLen);
	}

	public int GetUpdateOutputSize(int inputLen)
	{
		return bufferedCipher.GetUpdateOutputSize(inputLen);
	}
}
