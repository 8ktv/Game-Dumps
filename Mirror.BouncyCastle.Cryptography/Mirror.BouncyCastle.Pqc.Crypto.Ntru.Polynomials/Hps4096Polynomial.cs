using Mirror.BouncyCastle.Pqc.Crypto.Ntru.ParameterSets;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru.Polynomials;

internal class Hps4096Polynomial : HpsPolynomial
{
	internal Hps4096Polynomial(NtruParameterSet parameterSet)
		: base(parameterSet)
	{
	}

	public override byte[] SqToBytes(int len)
	{
		byte[] array = new byte[len];
		uint q = (uint)ParameterSet.Q();
		for (int i = 0; i < ParameterSet.PackDegree() / 2; i++)
		{
			array[3 * i] = (byte)(Polynomial.ModQ((uint)(coeffs[2 * i] & 0xFFFF), q) & 0xFF);
			array[3 * i + 1] = (byte)((Polynomial.ModQ((uint)(coeffs[2 * i] & 0xFFFF), q) >> 8) | ((Polynomial.ModQ((uint)(coeffs[2 * i + 1] & 0xFFFF), q) & 0xF) << 4));
			array[3 * i + 2] = (byte)(Polynomial.ModQ((uint)(coeffs[2 * i + 1] & 0xFFFF), q) >> 4);
		}
		return array;
	}

	public override void SqFromBytes(byte[] a)
	{
		for (int i = 0; i < ParameterSet.PackDegree() / 2; i++)
		{
			coeffs[2 * i] = (ushort)((a[3 * i] & 0xFF) | (((ushort)(a[3 * i + 1] & 0xFF) & 0xF) << 8));
			coeffs[2 * i + 1] = (ushort)(((a[3 * i + 1] & 0xFF) >> 4) | (((ushort)(a[3 * i + 2] & 0xFF) & 0xFF) << 4));
		}
		coeffs[ParameterSet.N - 1] = 0;
	}
}
