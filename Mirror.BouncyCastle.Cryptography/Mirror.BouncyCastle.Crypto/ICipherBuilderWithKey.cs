namespace Mirror.BouncyCastle.Crypto;

public interface ICipherBuilderWithKey : ICipherBuilder
{
	ICipherParameters Key { get; }
}
