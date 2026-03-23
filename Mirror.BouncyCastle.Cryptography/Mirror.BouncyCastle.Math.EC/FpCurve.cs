using System;

namespace Mirror.BouncyCastle.Math.EC;

public class FpCurve : AbstractFpCurve
{
	private const int FP_DEFAULT_COORDS = 4;

	protected readonly BigInteger m_q;

	protected readonly BigInteger m_r;

	protected readonly FpPoint m_infinity;

	public virtual BigInteger Q => m_q;

	public override ECPoint Infinity => m_infinity;

	public override int FieldSize => m_q.BitLength;

	[Obsolete("Use constructor taking order/cofactor")]
	public FpCurve(BigInteger q, BigInteger a, BigInteger b)
		: this(q, a, b, null, null)
	{
	}

	public FpCurve(BigInteger q, BigInteger a, BigInteger b, BigInteger order, BigInteger cofactor)
		: this(q, a, b, order, cofactor, isInternal: false)
	{
	}

	internal FpCurve(BigInteger q, BigInteger a, BigInteger b, BigInteger order, BigInteger cofactor, bool isInternal)
		: base(q, isInternal)
	{
		m_q = q;
		m_r = FpFieldElement.CalculateResidue(q);
		m_infinity = new FpPoint(this, null, null);
		m_a = FromBigInteger(a);
		m_b = FromBigInteger(b);
		m_order = order;
		m_cofactor = cofactor;
		m_coord = 4;
	}

	internal FpCurve(BigInteger q, BigInteger r, ECFieldElement a, ECFieldElement b, BigInteger order, BigInteger cofactor)
		: base(q, isInternal: true)
	{
		m_q = q;
		m_r = r;
		m_infinity = new FpPoint(this, null, null);
		m_a = a;
		m_b = b;
		m_order = order;
		m_cofactor = cofactor;
		m_coord = 4;
	}

	protected override ECCurve CloneCurve()
	{
		return new FpCurve(m_q, m_r, m_a, m_b, m_order, m_cofactor);
	}

	public override bool SupportsCoordinateSystem(int coord)
	{
		if ((uint)coord <= 2u || coord == 4)
		{
			return true;
		}
		return false;
	}

	public override ECFieldElement FromBigInteger(BigInteger x)
	{
		if (x == null || x.SignValue < 0 || x.CompareTo(m_q) >= 0)
		{
			throw new ArgumentException("value invalid for Fp field element", "x");
		}
		return new FpFieldElement(m_q, m_r, x);
	}

	protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y)
	{
		return new FpPoint(this, x, y);
	}

	protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs)
	{
		return new FpPoint(this, x, y, zs);
	}

	public override ECPoint ImportPoint(ECPoint p)
	{
		if (this != p.Curve && CoordinateSystem == 2 && !p.IsInfinity)
		{
			int coordinateSystem = p.Curve.CoordinateSystem;
			if ((uint)(coordinateSystem - 2) <= 2u)
			{
				return new FpPoint(this, FromBigInteger(p.RawXCoord.ToBigInteger()), FromBigInteger(p.RawYCoord.ToBigInteger()), new ECFieldElement[1] { FromBigInteger(p.GetZCoord(0).ToBigInteger()) });
			}
		}
		return base.ImportPoint(p);
	}
}
