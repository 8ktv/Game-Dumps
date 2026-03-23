namespace Mirror.BouncyCastle.Crypto.Generators;

public sealed class Kdf1BytesGenerator : BaseKdfBytesGenerator
{
	public Kdf1BytesGenerator(IDigest digest)
		: base(0, digest)
	{
	}
}
