using System;
using Mirror.BouncyCastle.Math.EC.Multiplier;

namespace Mirror.BouncyCastle.Math.EC.Abc;

internal static class Tnaf
{
	private sealed class PartModPreCompCallback : IPreCompCallback
	{
		private readonly AbstractF2mCurve m_curve;

		private readonly sbyte m_mu;

		private readonly bool m_doV;

		internal PartModPreCompCallback(AbstractF2mCurve curve, sbyte mu, bool doV)
		{
			m_curve = curve;
			m_mu = mu;
			m_doV = doV;
		}

		public PreCompInfo Precompute(PreCompInfo existing)
		{
			if (existing is PartModPreCompInfo)
			{
				return existing;
			}
			BigInteger lucas = ((!m_curve.IsKoblitz) ? GetLucas(m_mu, m_curve.FieldSize, m_doV)[1] : BigInteger.One.ShiftLeft(m_curve.FieldSize).Add(BigInteger.One).Subtract(m_curve.Order.Multiply(m_curve.Cofactor)));
			BigInteger[] si = GetSi(m_curve);
			return new PartModPreCompInfo(lucas, si[0], si[1]);
		}
	}

	private sealed class PartModPreCompInfo : PreCompInfo
	{
		private readonly BigInteger m_lucas;

		private readonly BigInteger m_s0;

		private readonly BigInteger m_s1;

		internal BigInteger Lucas => m_lucas;

		internal BigInteger S0 => m_s0;

		internal BigInteger S1 => m_s1;

		internal PartModPreCompInfo(BigInteger lucas, BigInteger s0, BigInteger s1)
		{
			m_lucas = lucas;
			m_s0 = s0;
			m_s1 = s1;
		}
	}

	private static readonly BigInteger MinusOne = BigInteger.One.Negate();

	private static readonly BigInteger MinusTwo = BigInteger.Two.Negate();

	private static readonly BigInteger MinusThree = BigInteger.Three.Negate();

	private static readonly BigInteger Four = BigInteger.ValueOf(4);

	private static readonly string PRECOMP_NAME = "bc_tnaf_partmod";

	public const sbyte Width = 4;

	public static readonly ZTauElement[] Alpha0 = new ZTauElement[16]
	{
		null,
		new ZTauElement(BigInteger.One, BigInteger.Zero),
		null,
		new ZTauElement(MinusThree, MinusOne),
		null,
		new ZTauElement(MinusOne, MinusOne),
		null,
		new ZTauElement(BigInteger.One, MinusOne),
		null,
		new ZTauElement(MinusOne, BigInteger.One),
		null,
		new ZTauElement(BigInteger.One, BigInteger.One),
		null,
		new ZTauElement(BigInteger.Three, BigInteger.One),
		null,
		new ZTauElement(MinusOne, BigInteger.Zero)
	};

	public static readonly sbyte[][] Alpha0Tnaf = new sbyte[8][]
	{
		null,
		new sbyte[1] { 1 },
		null,
		new sbyte[3] { -1, 0, 1 },
		null,
		new sbyte[3] { 1, 0, 1 },
		null,
		new sbyte[4] { -1, 0, 0, 1 }
	};

	public static readonly ZTauElement[] Alpha1 = new ZTauElement[16]
	{
		null,
		new ZTauElement(BigInteger.One, BigInteger.Zero),
		null,
		new ZTauElement(MinusThree, BigInteger.One),
		null,
		new ZTauElement(MinusOne, BigInteger.One),
		null,
		new ZTauElement(BigInteger.One, BigInteger.One),
		null,
		new ZTauElement(MinusOne, MinusOne),
		null,
		new ZTauElement(BigInteger.One, MinusOne),
		null,
		new ZTauElement(BigInteger.Three, MinusOne),
		null,
		new ZTauElement(MinusOne, BigInteger.Zero)
	};

	public static readonly sbyte[][] Alpha1Tnaf = new sbyte[8][]
	{
		null,
		new sbyte[1] { 1 },
		null,
		new sbyte[3] { -1, 0, 1 },
		null,
		new sbyte[3] { 1, 0, 1 },
		null,
		new sbyte[4] { -1, 0, 0, -1 }
	};

	public static BigInteger Norm(sbyte mu, ZTauElement lambda)
	{
		BigInteger value = lambda.u.Square();
		return mu switch
		{
			1 => lambda.v.ShiftLeft(1).Add(lambda.u).Multiply(lambda.v)
				.Add(value), 
			-1 => lambda.v.ShiftLeft(1).Subtract(lambda.u).Multiply(lambda.v)
				.Add(value), 
			_ => throw new ArgumentException("mu must be 1 or -1"), 
		};
	}

	public static SimpleBigDecimal Norm(sbyte mu, SimpleBigDecimal u, SimpleBigDecimal v)
	{
		SimpleBigDecimal simpleBigDecimal = u.Multiply(u);
		SimpleBigDecimal b = u.Multiply(v);
		SimpleBigDecimal b2 = v.Multiply(v).ShiftLeft(1);
		return mu switch
		{
			1 => simpleBigDecimal.Add(b).Add(b2), 
			-1 => simpleBigDecimal.Subtract(b).Add(b2), 
			_ => throw new ArgumentException("mu must be 1 or -1"), 
		};
	}

	public static ZTauElement Round(SimpleBigDecimal lambda0, SimpleBigDecimal lambda1, sbyte mu)
	{
		int scale = lambda0.Scale;
		if (lambda1.Scale != scale)
		{
			throw new ArgumentException("lambda0 and lambda1 do not have same scale");
		}
		if (mu != 1 && mu != -1)
		{
			throw new ArgumentException("mu must be 1 or -1");
		}
		BigInteger bigInteger = lambda0.Round();
		BigInteger bigInteger2 = lambda1.Round();
		SimpleBigDecimal simpleBigDecimal = lambda0.Subtract(bigInteger);
		SimpleBigDecimal simpleBigDecimal2 = lambda1.Subtract(bigInteger2);
		SimpleBigDecimal simpleBigDecimal3 = simpleBigDecimal.Add(simpleBigDecimal);
		simpleBigDecimal3 = ((mu != 1) ? simpleBigDecimal3.Subtract(simpleBigDecimal2) : simpleBigDecimal3.Add(simpleBigDecimal2));
		SimpleBigDecimal simpleBigDecimal4 = simpleBigDecimal2.Add(simpleBigDecimal2).Add(simpleBigDecimal2);
		SimpleBigDecimal b = simpleBigDecimal4.Add(simpleBigDecimal2);
		SimpleBigDecimal simpleBigDecimal5;
		SimpleBigDecimal simpleBigDecimal6;
		if (mu == 1)
		{
			simpleBigDecimal5 = simpleBigDecimal.Subtract(simpleBigDecimal4);
			simpleBigDecimal6 = simpleBigDecimal.Add(b);
		}
		else
		{
			simpleBigDecimal5 = simpleBigDecimal.Add(simpleBigDecimal4);
			simpleBigDecimal6 = simpleBigDecimal.Subtract(b);
		}
		sbyte value = 0;
		sbyte value2 = 0;
		if (simpleBigDecimal3.CompareTo(BigInteger.One) >= 0)
		{
			if (simpleBigDecimal5.CompareTo(MinusOne) < 0)
			{
				value2 = mu;
			}
			else
			{
				value = 1;
			}
		}
		else if (simpleBigDecimal6.CompareTo(BigInteger.Two) >= 0)
		{
			value2 = mu;
		}
		if (simpleBigDecimal3.CompareTo(MinusOne) < 0)
		{
			if (simpleBigDecimal5.CompareTo(BigInteger.One) >= 0)
			{
				value2 = (sbyte)(-mu);
			}
			else
			{
				value = -1;
			}
		}
		else if (simpleBigDecimal6.CompareTo(MinusTwo) < 0)
		{
			value2 = (sbyte)(-mu);
		}
		BigInteger u = bigInteger.Add(BigInteger.ValueOf(value));
		BigInteger v = bigInteger2.Add(BigInteger.ValueOf(value2));
		return new ZTauElement(u, v);
	}

	public static SimpleBigDecimal ApproximateDivisionByN(BigInteger k, BigInteger s, BigInteger vm, sbyte a, int m, int c)
	{
		int num = (m + 5) / 2 + c;
		BigInteger val = k.ShiftRight(m - num - 2 + a);
		BigInteger bigInteger = s.Multiply(val);
		BigInteger val2 = bigInteger.ShiftRight(m);
		BigInteger value = vm.Multiply(val2);
		BigInteger bigInteger2 = bigInteger.Add(value);
		BigInteger bigInteger3 = bigInteger2.ShiftRight(num - c);
		if (bigInteger2.TestBit(num - c - 1))
		{
			bigInteger3 = bigInteger3.Add(BigInteger.One);
		}
		return new SimpleBigDecimal(bigInteger3, c);
	}

	public static sbyte[] TauAdicNaf(sbyte mu, ZTauElement lambda)
	{
		if (mu != 1 && mu != -1)
		{
			throw new ArgumentException("mu must be 1 or -1");
		}
		int bitLength = Norm(mu, lambda).BitLength;
		sbyte[] array = new sbyte[(bitLength > 30) ? (bitLength + 4) : 34];
		int num = 0;
		int num2 = 0;
		BigInteger bigInteger = lambda.u;
		BigInteger bigInteger2 = lambda.v;
		while (!bigInteger.Equals(BigInteger.Zero) || !bigInteger2.Equals(BigInteger.Zero))
		{
			if (bigInteger.TestBit(0))
			{
				array[num] = (sbyte)BigInteger.Two.Subtract(bigInteger.Subtract(bigInteger2.ShiftLeft(1)).Mod(Four)).IntValue;
				bigInteger = ((array[num] != 1) ? bigInteger.Add(BigInteger.One) : bigInteger.ClearBit(0));
				num2 = num;
			}
			else
			{
				array[num] = 0;
			}
			BigInteger bigInteger3 = bigInteger;
			BigInteger bigInteger4 = bigInteger.ShiftRight(1);
			bigInteger = ((mu != 1) ? bigInteger2.Subtract(bigInteger4) : bigInteger2.Add(bigInteger4));
			bigInteger2 = bigInteger3.ShiftRight(1).Negate();
			num++;
		}
		num2++;
		sbyte[] array2 = new sbyte[num2];
		Array.Copy(array, 0, array2, 0, num2);
		return array2;
	}

	public static AbstractF2mPoint Tau(AbstractF2mPoint p)
	{
		return p.Tau();
	}

	public static sbyte GetMu(AbstractF2mCurve curve)
	{
		BigInteger bigInteger = curve.A.ToBigInteger();
		if (bigInteger.SignValue == 0)
		{
			return -1;
		}
		if (bigInteger.Equals(BigInteger.One))
		{
			return 1;
		}
		throw new ArgumentException("No Koblitz curve (ABC), TNAF multiplication not possible");
	}

	public static sbyte GetMu(ECFieldElement curveA)
	{
		return (sbyte)((!curveA.IsZero) ? 1 : (-1));
	}

	public static sbyte GetMu(int curveA)
	{
		return (sbyte)((curveA != 0) ? 1 : (-1));
	}

	public static BigInteger[] GetLucas(sbyte mu, int k, bool doV)
	{
		if (mu != 1 && mu != -1)
		{
			throw new ArgumentException("mu must be 1 or -1");
		}
		BigInteger bigInteger;
		BigInteger bigInteger2;
		if (doV)
		{
			bigInteger = BigInteger.Two;
			bigInteger2 = BigInteger.ValueOf(mu);
		}
		else
		{
			bigInteger = BigInteger.Zero;
			bigInteger2 = BigInteger.One;
		}
		for (int i = 1; i < k; i++)
		{
			BigInteger bigInteger3 = bigInteger2;
			if (mu < 0)
			{
				bigInteger3 = bigInteger3.Negate();
			}
			BigInteger bigInteger4 = bigInteger3.Subtract(bigInteger.ShiftLeft(1));
			bigInteger = bigInteger2;
			bigInteger2 = bigInteger4;
		}
		return new BigInteger[2] { bigInteger, bigInteger2 };
	}

	public static BigInteger GetTw(sbyte mu, int w)
	{
		if (w == 4)
		{
			if (mu == 1)
			{
				return BigInteger.Six;
			}
			return BigInteger.Ten;
		}
		BigInteger[] lucas = GetLucas(mu, w, doV: false);
		return lucas[0].ShiftLeft(1).ModDivide(lucas[1], BigInteger.One.ShiftLeft(w));
	}

	public static BigInteger[] GetSi(AbstractF2mCurve curve)
	{
		if (!curve.IsKoblitz)
		{
			throw new ArgumentException("si is defined for Koblitz curves only");
		}
		return GetSi(curve.FieldSize, curve.A.ToBigInteger().IntValue, curve.Cofactor);
	}

	public static BigInteger[] GetSi(int fieldSize, int curveA, BigInteger cofactor)
	{
		sbyte mu = GetMu(curveA);
		int shiftsForCofactor = GetShiftsForCofactor(cofactor);
		int k = fieldSize + 3 - curveA;
		BigInteger[] lucas = GetLucas(mu, k, doV: false);
		if (mu == 1)
		{
			lucas[0] = lucas[0].Negate();
			lucas[1] = lucas[1].Negate();
		}
		BigInteger bigInteger = BigInteger.One.Add(lucas[1]).ShiftRight(shiftsForCofactor);
		BigInteger bigInteger2 = BigInteger.One.Add(lucas[0]).ShiftRight(shiftsForCofactor).Negate();
		return new BigInteger[2] { bigInteger, bigInteger2 };
	}

	private static int GetShiftsForCofactor(BigInteger h)
	{
		if (h != null && h.BitLength < 4)
		{
			switch (h.IntValue)
			{
			case 2:
				return 1;
			case 4:
				return 2;
			}
		}
		throw new ArgumentException("h (Cofactor) must be 2 or 4");
	}

	public static ZTauElement PartModReduction(AbstractF2mCurve curve, BigInteger k, sbyte a, sbyte mu, sbyte c)
	{
		PartModPreCompCallback callback = new PartModPreCompCallback(curve, mu, doV: true);
		PartModPreCompInfo obj = (PartModPreCompInfo)curve.Precompute(PRECOMP_NAME, callback);
		BigInteger lucas = obj.Lucas;
		BigInteger s = obj.S0;
		BigInteger s2 = obj.S1;
		BigInteger bigInteger = ((mu != 1) ? s.Subtract(s2) : s.Add(s2));
		int fieldSize = curve.FieldSize;
		SimpleBigDecimal lambda = ApproximateDivisionByN(k, s, lucas, a, fieldSize, c);
		SimpleBigDecimal lambda2 = ApproximateDivisionByN(k, s2, lucas, a, fieldSize, c);
		ZTauElement zTauElement = Round(lambda, lambda2, mu);
		BigInteger u = k.Subtract(bigInteger.Multiply(zTauElement.u)).Subtract(s2.Multiply(zTauElement.v).ShiftLeft(1));
		BigInteger v = s2.Multiply(zTauElement.u).Subtract(s.Multiply(zTauElement.v));
		return new ZTauElement(u, v);
	}

	public static AbstractF2mPoint MultiplyRTnaf(AbstractF2mPoint p, BigInteger k)
	{
		AbstractF2mCurve obj = (AbstractF2mCurve)p.Curve;
		int intValue = obj.A.ToBigInteger().IntValue;
		sbyte mu = GetMu(intValue);
		ZTauElement lambda = PartModReduction(obj, k, (sbyte)intValue, mu, 10);
		return MultiplyTnaf(p, lambda);
	}

	public static AbstractF2mPoint MultiplyTnaf(AbstractF2mPoint p, ZTauElement lambda)
	{
		AbstractF2mCurve obj = (AbstractF2mCurve)p.Curve;
		AbstractF2mPoint pNeg = (AbstractF2mPoint)p.Negate();
		sbyte[] u = TauAdicNaf(GetMu(obj.A), lambda);
		return MultiplyFromTnaf(p, pNeg, u);
	}

	public static AbstractF2mPoint MultiplyFromTnaf(AbstractF2mPoint p, AbstractF2mPoint pNeg, sbyte[] u)
	{
		AbstractF2mPoint abstractF2mPoint = (AbstractF2mPoint)p.Curve.Infinity;
		int num = 0;
		for (int num2 = u.Length - 1; num2 >= 0; num2--)
		{
			num++;
			sbyte b = u[num2];
			if (b != 0)
			{
				abstractF2mPoint = abstractF2mPoint.TauPow(num);
				num = 0;
				ECPoint b2 = ((b > 0) ? p : pNeg);
				abstractF2mPoint = (AbstractF2mPoint)abstractF2mPoint.Add(b2);
			}
		}
		if (num > 0)
		{
			abstractF2mPoint = abstractF2mPoint.TauPow(num);
		}
		return abstractF2mPoint;
	}

	public static sbyte[] TauAdicWNaf(sbyte mu, ZTauElement lambda, int width, int tw, ZTauElement[] alpha)
	{
		if (mu != 1 && mu != -1)
		{
			throw new ArgumentException("mu must be 1 or -1");
		}
		int bitLength = Norm(mu, lambda).BitLength;
		sbyte[] array = new sbyte[(bitLength > 30) ? (bitLength + 4 + width) : (34 + width)];
		int num = (1 << width) - 1;
		int num2 = 32 - width;
		BigInteger bigInteger = lambda.u;
		BigInteger bigInteger2 = lambda.v;
		int num3 = 0;
		int[] array2 = new int[alpha.Length];
		int[] array3 = new int[alpha.Length];
		for (int i = 1; i < alpha.Length; i += 2)
		{
			array2[i] = alpha[i].u.IntValueExact;
			array3[i] = alpha[i].v.IntValueExact;
		}
		while (bigInteger.BitLength > 62 || bigInteger2.BitLength > 62)
		{
			if (bigInteger.TestBit(0))
			{
				int num4 = bigInteger.IntValue + bigInteger2.IntValue * tw;
				int num5 = num4 & num;
				array[num3] = (sbyte)(num4 << num2 >> num2);
				bigInteger = bigInteger.Subtract(alpha[num5].u);
				bigInteger2 = bigInteger2.Subtract(alpha[num5].v);
			}
			num3++;
			BigInteger bigInteger3 = bigInteger.ShiftRight(1);
			bigInteger = ((mu != 1) ? bigInteger2.Subtract(bigInteger3) : bigInteger2.Add(bigInteger3));
			bigInteger2 = bigInteger3.Negate();
		}
		long num6 = bigInteger.LongValueExact;
		long num7 = bigInteger2.LongValueExact;
		while ((num6 | num7) != 0L)
		{
			if ((num6 & 1) != 0L)
			{
				int num8 = (int)num6 + (int)num7 * tw;
				int num9 = num8 & num;
				array[num3] = (sbyte)(num8 << num2 >> num2);
				num6 -= array2[num9];
				num7 -= array3[num9];
			}
			num3++;
			long num10 = num6 >> 1;
			num6 = ((mu != 1) ? (num7 - num10) : (num7 + num10));
			num7 = -num10;
		}
		return array;
	}

	public static AbstractF2mPoint[] GetPreComp(AbstractF2mPoint p, sbyte a)
	{
		AbstractF2mPoint pNeg = (AbstractF2mPoint)p.Negate();
		sbyte[][] array = ((a == 0) ? Alpha0Tnaf : Alpha1Tnaf);
		AbstractF2mPoint[] array2 = new AbstractF2mPoint[array.Length + 1 >>> 1];
		array2[0] = p;
		int num = array.Length;
		for (uint num2 = 3u; num2 < num; num2 += 2)
		{
			array2[num2 >> 1] = MultiplyFromTnaf(p, pNeg, array[num2]);
		}
		ECCurve curve = p.Curve;
		ECPoint[] points = array2;
		curve.NormalizeAll(points);
		return array2;
	}
}
