using System;
using Mirror.BouncyCastle.Math.Field;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Math.EC;

public abstract class AbstractF2mCurve : ECCurve
{
	public virtual bool IsKoblitz
	{
		get
		{
			if (m_order != null && m_cofactor != null && m_b.IsOne)
			{
				if (!m_a.IsZero)
				{
					return m_a.IsOne;
				}
				return true;
			}
			return false;
		}
	}

	public static BigInteger Inverse(int m, int[] ks, BigInteger x)
	{
		return new LongArray(x).ModInverse(m, ks).ToBigInteger();
	}

	private static IFiniteField BuildField(int m, int k1, int k2, int k3)
	{
		int num = ECCurve.ImplGetInteger("Mirror.BouncyCastle.EC.F2m_MaxSize", 1142);
		if (m > num)
		{
			throw new ArgumentException("F2m m value out of range");
		}
		return FiniteFields.GetBinaryExtensionField(((k2 | k3) != 0) ? new int[5] { 0, k1, k2, k3, m } : new int[3] { 0, k1, m });
	}

	protected AbstractF2mCurve(int m, int k1, int k2, int k3)
		: base(BuildField(m, k1, k2, k3))
	{
	}

	public override ECPoint CreatePoint(BigInteger x, BigInteger y)
	{
		ECFieldElement eCFieldElement = FromBigInteger(x);
		ECFieldElement eCFieldElement2 = FromBigInteger(y);
		int coordinateSystem = CoordinateSystem;
		if ((uint)(coordinateSystem - 5) <= 1u)
		{
			if (eCFieldElement.IsZero)
			{
				if (!eCFieldElement2.Square().Equals(B))
				{
					throw new ArgumentException();
				}
			}
			else
			{
				eCFieldElement2 = eCFieldElement2.Divide(eCFieldElement).Add(eCFieldElement);
			}
		}
		return CreateRawPoint(eCFieldElement, eCFieldElement2);
	}

	public override bool IsValidFieldElement(BigInteger x)
	{
		if (x != null && x.SignValue >= 0)
		{
			return x.BitLength <= FieldSize;
		}
		return false;
	}

	public override ECFieldElement RandomFieldElement(SecureRandom r)
	{
		int fieldSize = FieldSize;
		return FromBigInteger(BigIntegers.CreateRandomBigInteger(fieldSize, r));
	}

	public override ECFieldElement RandomFieldElementMult(SecureRandom r)
	{
		int fieldSize = FieldSize;
		ECFieldElement eCFieldElement = FromBigInteger(ImplRandomFieldElementMult(r, fieldSize));
		ECFieldElement b = FromBigInteger(ImplRandomFieldElementMult(r, fieldSize));
		return eCFieldElement.Multiply(b);
	}

	protected override ECPoint DecompressPoint(int yTilde, BigInteger X1)
	{
		ECFieldElement eCFieldElement = FromBigInteger(X1);
		ECFieldElement eCFieldElement2 = null;
		if (eCFieldElement.IsZero)
		{
			eCFieldElement2 = B.Sqrt();
		}
		else
		{
			ECFieldElement beta = eCFieldElement.Square().Invert().Multiply(B)
				.Add(A)
				.Add(eCFieldElement);
			ECFieldElement eCFieldElement3 = SolveQuadraticEquation(beta);
			if (eCFieldElement3 != null)
			{
				if (eCFieldElement3.TestBitZero() != (yTilde == 1))
				{
					eCFieldElement3 = eCFieldElement3.AddOne();
				}
				int coordinateSystem = CoordinateSystem;
				eCFieldElement2 = (((uint)(coordinateSystem - 5) > 1u) ? eCFieldElement3.Multiply(eCFieldElement) : eCFieldElement3.Add(eCFieldElement));
			}
		}
		if (eCFieldElement2 == null)
		{
			throw new ArgumentException("Invalid point compression");
		}
		return CreateRawPoint(eCFieldElement, eCFieldElement2);
	}

	internal ECFieldElement SolveQuadraticEquation(ECFieldElement beta)
	{
		AbstractF2mFieldElement abstractF2mFieldElement = (AbstractF2mFieldElement)beta;
		bool hasFastTrace = abstractF2mFieldElement.HasFastTrace;
		if (hasFastTrace && abstractF2mFieldElement.Trace() != 0)
		{
			return null;
		}
		int fieldSize = FieldSize;
		if ((fieldSize & 1) != 0)
		{
			ECFieldElement eCFieldElement = abstractF2mFieldElement.HalfTrace();
			if (hasFastTrace || eCFieldElement.Square().Add(eCFieldElement).Add(beta)
				.IsZero)
			{
				return eCFieldElement;
			}
			return null;
		}
		if (beta.IsZero)
		{
			return beta;
		}
		ECFieldElement eCFieldElement2 = FromBigInteger(BigInteger.Zero);
		ECFieldElement eCFieldElement3;
		ECFieldElement eCFieldElement6;
		do
		{
			ECFieldElement b = FromBigInteger(BigInteger.Arbitrary(fieldSize));
			eCFieldElement3 = eCFieldElement2;
			ECFieldElement eCFieldElement4 = beta;
			for (int i = 1; i < fieldSize; i++)
			{
				ECFieldElement eCFieldElement5 = eCFieldElement4.Square();
				eCFieldElement3 = eCFieldElement3.Square().Add(eCFieldElement5.Multiply(b));
				eCFieldElement4 = eCFieldElement5.Add(beta);
			}
			if (!eCFieldElement4.IsZero)
			{
				return null;
			}
			eCFieldElement6 = eCFieldElement3.Square().Add(eCFieldElement3);
		}
		while (eCFieldElement6.IsZero);
		return eCFieldElement3;
	}

	private static BigInteger ImplRandomFieldElementMult(SecureRandom r, int m)
	{
		BigInteger bigInteger;
		do
		{
			bigInteger = BigIntegers.CreateRandomBigInteger(m, r);
		}
		while (bigInteger.SignValue <= 0);
		return bigInteger;
	}
}
