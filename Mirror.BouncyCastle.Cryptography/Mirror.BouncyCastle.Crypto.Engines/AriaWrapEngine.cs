namespace Mirror.BouncyCastle.Crypto.Engines;

public class AriaWrapEngine : Rfc3394WrapEngine
{
	public AriaWrapEngine()
		: base(new AriaEngine())
	{
	}

	public AriaWrapEngine(bool useReverseDirection)
		: base(new AriaEngine(), useReverseDirection)
	{
	}
}
