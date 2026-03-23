namespace Mirror.BouncyCastle.Crypto.Engines;

public class AesWrapEngine : Rfc3394WrapEngine
{
	public AesWrapEngine()
		: base(AesUtilities.CreateEngine())
	{
	}

	public AesWrapEngine(bool useReverseDirection)
		: base(AesUtilities.CreateEngine(), useReverseDirection)
	{
	}
}
