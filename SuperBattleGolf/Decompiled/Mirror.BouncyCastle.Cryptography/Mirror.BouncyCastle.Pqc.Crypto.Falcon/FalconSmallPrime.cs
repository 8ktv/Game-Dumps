namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class FalconSmallPrime
{
	internal uint p;

	internal uint g;

	internal uint s;

	internal FalconSmallPrime(uint p, uint g, uint s)
	{
		this.p = p;
		this.g = g;
		this.s = s;
	}
}
