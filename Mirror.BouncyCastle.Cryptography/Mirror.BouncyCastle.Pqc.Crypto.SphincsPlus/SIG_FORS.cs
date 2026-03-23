namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal class SIG_FORS
{
	internal byte[][] authPath;

	internal byte[] sk;

	public byte[] SK => sk;

	public byte[][] AuthPath => authPath;

	internal SIG_FORS(byte[] sk, byte[][] authPath)
	{
		this.authPath = authPath;
		this.sk = sk;
	}
}
