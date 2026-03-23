using System;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.ParameterSets;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru.Polynomials;

internal abstract class Polynomial
{
	internal ushort[] coeffs;

	private protected readonly NtruParameterSet ParameterSet;

	internal Polynomial(NtruParameterSet parameterSet)
	{
		coeffs = new ushort[parameterSet.N];
		ParameterSet = parameterSet;
	}

	internal static short BothNegativeMask(short x, short y)
	{
		return (short)((x & y) >> 15);
	}

	internal static ushort Mod3(ushort a)
	{
		return Mod((int)a, 3.0);
	}

	internal static byte Mod3(byte a)
	{
		return (byte)Mod((int)a, 3.0);
	}

	internal static uint ModQ(uint x, uint q)
	{
		return Mod(x, q);
	}

	internal void Mod3PhiN()
	{
		int n = ParameterSet.N;
		for (int i = 0; i < n; i++)
		{
			coeffs[i] = Mod3((ushort)(coeffs[i] + 2 * coeffs[n - 1]));
		}
	}

	internal void ModQPhiN()
	{
		int n = ParameterSet.N;
		for (int i = 0; i < n; i++)
		{
			coeffs[i] -= coeffs[n - 1];
		}
	}

	internal static ushort Mod(double a, double b)
	{
		return (ushort)(a - b * System.Math.Floor(a / b));
	}

	public abstract byte[] SqToBytes(int len);

	public abstract void SqFromBytes(byte[] a);

	public byte[] RqSumZeroToBytes(int len)
	{
		return SqToBytes(len);
	}

	public void RqSumZeroFromBytes(byte[] a)
	{
		int num = coeffs.Length;
		SqFromBytes(a);
		coeffs[num - 1] = 0;
		for (int i = 0; i < ParameterSet.PackDegree(); i++)
		{
			coeffs[num - 1] -= coeffs[i];
		}
	}

	public byte[] S3ToBytes(int messageSize)
	{
		byte[] array = new byte[messageSize];
		for (int i = 0; i < ParameterSet.PackDegree() / 5; i++)
		{
			byte b = (byte)(coeffs[5 * i + 4] & 0xFF);
			b = (byte)((3 * b + coeffs[5 * i + 3]) & 0xFF);
			b = (byte)((3 * b + coeffs[5 * i + 2]) & 0xFF);
			b = (byte)((3 * b + coeffs[5 * i + 1]) & 0xFF);
			b = (byte)((3 * b + coeffs[5 * i]) & 0xFF);
			array[i] = b;
		}
		if (ParameterSet.PackDegree() > ParameterSet.PackDegree() / 5 * 5)
		{
			int num = ParameterSet.PackDegree() / 5;
			byte b = 0;
			for (int num2 = ParameterSet.PackDegree() - 5 * num - 1; num2 >= 0; num2--)
			{
				b = (byte)((3 * b + coeffs[5 * num + num2]) & 0xFF);
			}
			array[num] = b;
		}
		return array;
	}

	public void S3FromBytes(byte[] msg)
	{
		int num = coeffs.Length;
		for (int i = 0; i < ParameterSet.PackDegree() / 5; i++)
		{
			byte b = msg[i];
			coeffs[5 * i] = b;
			coeffs[5 * i + 1] = (ushort)(b * 171 >> 9);
			coeffs[5 * i + 2] = (ushort)(b * 57 >> 9);
			coeffs[5 * i + 3] = (ushort)(b * 19 >> 9);
			coeffs[5 * i + 4] = (ushort)(b * 203 >> 14);
		}
		if (ParameterSet.PackDegree() > ParameterSet.PackDegree() / 5 * 5)
		{
			int num2 = ParameterSet.PackDegree() / 5;
			byte b = msg[num2];
			for (int j = 0; 5 * num2 + j < ParameterSet.PackDegree(); j++)
			{
				coeffs[5 * num2 + j] = b;
				b = (byte)(b * 171 >> 9);
			}
		}
		coeffs[num - 1] = 0;
		Mod3PhiN();
	}

	public void RqMul(Polynomial a, Polynomial b)
	{
		int num = coeffs.Length;
		for (int i = 0; i < num; i++)
		{
			coeffs[i] = 0;
			for (int j = 1; j < num - i; j++)
			{
				coeffs[i] += (ushort)(a.coeffs[i + j] * b.coeffs[num - j]);
			}
			for (int j = 0; j < i + 1; j++)
			{
				coeffs[i] += (ushort)(a.coeffs[i - j] * b.coeffs[j]);
			}
		}
	}

	public void SqMul(Polynomial a, Polynomial b)
	{
		RqMul(a, b);
		ModQPhiN();
	}

	public void S3Mul(Polynomial a, Polynomial b)
	{
		RqMul(a, b);
		Mod3PhiN();
	}

	public abstract void Lift(Polynomial a);

	public void RqToS3(Polynomial a)
	{
		int num = coeffs.Length;
		for (int i = 0; i < num; i++)
		{
			coeffs[i] = (ushort)ModQ(a.coeffs[i], (uint)ParameterSet.Q());
			ushort num2 = (ushort)(coeffs[i] >> ParameterSet.LogQ - 1);
			coeffs[i] += (ushort)(num2 << 1 - (ParameterSet.LogQ & 1));
		}
		Mod3PhiN();
	}

	public abstract void R2Inv(Polynomial a);

	internal void R2Inv(Polynomial a, Polynomial f, Polynomial g, Polynomial v, Polynomial w)
	{
		int num = coeffs.Length;
		w.coeffs[0] = 1;
		for (int i = 0; i < num; i++)
		{
			f.coeffs[i] = 1;
		}
		for (int i = 0; i < num - 1; i++)
		{
			g.coeffs[num - 2 - i] = (ushort)((a.coeffs[i] ^ a.coeffs[num - 1]) & 1);
		}
		g.coeffs[num - 1] = 0;
		short num2 = 1;
		for (int j = 0; j < 2 * (num - 1) - 1; j++)
		{
			for (int i = num - 1; i > 0; i--)
			{
				v.coeffs[i] = v.coeffs[i - 1];
			}
			v.coeffs[0] = 0;
			short num3 = (short)(g.coeffs[0] & f.coeffs[0]);
			short num4 = BothNegativeMask((short)(-num2), (short)(-g.coeffs[0]));
			num2 ^= (short)(num4 & (num2 ^ -num2));
			num2++;
			for (int i = 0; i < num; i++)
			{
				short num5 = (short)(num4 & (f.coeffs[i] ^ g.coeffs[i]));
				f.coeffs[i] ^= (ushort)num5;
				g.coeffs[i] ^= (ushort)num5;
				num5 = (short)(num4 & (v.coeffs[i] ^ w.coeffs[i]));
				v.coeffs[i] ^= (ushort)num5;
				w.coeffs[i] ^= (ushort)num5;
			}
			for (int i = 0; i < num; i++)
			{
				g.coeffs[i] = (ushort)(g.coeffs[i] ^ (num3 & f.coeffs[i]));
			}
			for (int i = 0; i < num; i++)
			{
				w.coeffs[i] = (ushort)(w.coeffs[i] ^ (num3 & v.coeffs[i]));
			}
			for (int i = 0; i < num - 1; i++)
			{
				g.coeffs[i] = g.coeffs[i + 1];
			}
			g.coeffs[num - 1] = 0;
		}
		for (int i = 0; i < num - 1; i++)
		{
			coeffs[i] = v.coeffs[num - 2 - i];
		}
		coeffs[num - 1] = 0;
	}

	public abstract void RqInv(Polynomial a);

	internal void RqInv(Polynomial a, Polynomial ai2, Polynomial b, Polynomial c, Polynomial s)
	{
		ai2.R2Inv(a);
		R2InvToRqInv(ai2, a, b, c, s);
	}

	private void R2InvToRqInv(Polynomial ai, Polynomial a, Polynomial b, Polynomial c, Polynomial s)
	{
		int num = coeffs.Length;
		for (int i = 0; i < num; i++)
		{
			b.coeffs[i] = (ushort)(-a.coeffs[i]);
		}
		for (int i = 0; i < num; i++)
		{
			coeffs[i] = ai.coeffs[i];
		}
		c.RqMul(this, b);
		c.coeffs[0] += 2;
		s.RqMul(c, this);
		c.RqMul(s, b);
		c.coeffs[0] += 2;
		RqMul(c, s);
		c.RqMul(this, b);
		c.coeffs[0] += 2;
		s.RqMul(c, this);
		c.RqMul(s, b);
		c.coeffs[0] += 2;
		RqMul(c, s);
	}

	public abstract void S3Inv(Polynomial a);

	internal void S3Inv(Polynomial a, Polynomial f, Polynomial g, Polynomial v, Polynomial w)
	{
		int num = coeffs.Length;
		w.coeffs[0] = 1;
		for (int i = 0; i < num; i++)
		{
			f.coeffs[i] = 1;
		}
		for (int i = 0; i < num - 1; i++)
		{
			g.coeffs[num - 2 - i] = Mod3((ushort)((a.coeffs[i] & 3) + 2 * (a.coeffs[num - 1] & 3)));
		}
		g.coeffs[num - 1] = 0;
		short num2 = 1;
		short num3;
		for (int j = 0; j < 2 * (num - 1) - 1; j++)
		{
			for (int i = num - 1; i > 0; i--)
			{
				v.coeffs[i] = v.coeffs[i - 1];
			}
			v.coeffs[0] = 0;
			num3 = Mod3((byte)(2 * g.coeffs[0] * f.coeffs[0]));
			short num4 = BothNegativeMask((short)(-num2), (short)(-g.coeffs[0]));
			num2 ^= (short)(num4 & (num2 ^ -num2));
			num2++;
			for (int i = 0; i < num; i++)
			{
				short num5 = (short)(num4 & (f.coeffs[i] ^ g.coeffs[i]));
				f.coeffs[i] ^= (ushort)num5;
				g.coeffs[i] ^= (ushort)num5;
				num5 = (short)(num4 & (v.coeffs[i] ^ w.coeffs[i]));
				v.coeffs[i] ^= (ushort)num5;
				w.coeffs[i] ^= (ushort)num5;
			}
			for (int i = 0; i < num; i++)
			{
				g.coeffs[i] = Mod3((byte)(g.coeffs[i] + num3 * f.coeffs[i]));
			}
			for (int i = 0; i < num; i++)
			{
				w.coeffs[i] = Mod3((byte)(w.coeffs[i] + num3 * v.coeffs[i]));
			}
			for (int i = 0; i < num - 1; i++)
			{
				g.coeffs[i] = g.coeffs[i + 1];
			}
			g.coeffs[num - 1] = 0;
		}
		num3 = (short)f.coeffs[0];
		for (int i = 0; i < num - 1; i++)
		{
			coeffs[i] = Mod3((byte)(num3 * v.coeffs[num - 2 - i]));
		}
		coeffs[num - 1] = 0;
	}

	public void Z3ToZq()
	{
		int num = coeffs.Length;
		for (int i = 0; i < num; i++)
		{
			coeffs[i] = (ushort)(coeffs[i] | (-(coeffs[i] >> 1) & (ParameterSet.Q() - 1)));
		}
	}

	public void TrinaryZqToZ3()
	{
		int num = coeffs.Length;
		for (int i = 0; i < num; i++)
		{
			coeffs[i] = (ushort)ModQ((uint)(coeffs[i] & 0xFFFF), (uint)ParameterSet.Q());
			coeffs[i] = (ushort)(3 & (coeffs[i] ^ (coeffs[i] >> ParameterSet.LogQ - 1)));
		}
	}
}
