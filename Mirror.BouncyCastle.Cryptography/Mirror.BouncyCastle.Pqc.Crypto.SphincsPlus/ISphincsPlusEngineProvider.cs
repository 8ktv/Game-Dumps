namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal interface ISphincsPlusEngineProvider
{
	int N { get; }

	SphincsPlusEngine Get();
}
