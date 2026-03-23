namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal sealed class Sha2EngineProvider : ISphincsPlusEngineProvider
{
	private readonly bool robust;

	private readonly int n;

	private readonly uint w;

	private readonly uint d;

	private readonly int a;

	private readonly int k;

	private readonly uint h;

	public int N => n;

	internal Sha2EngineProvider(bool robust, int n, uint w, uint d, int a, int k, uint h)
	{
		this.robust = robust;
		this.n = n;
		this.w = w;
		this.d = d;
		this.a = a;
		this.k = k;
		this.h = h;
	}

	public SphincsPlusEngine Get()
	{
		return new SphincsPlusEngine.Sha2Engine(robust, n, w, d, a, k, h);
	}
}
