using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Math.EC.Custom.Sec;

internal class SecT163K1Point : AbstractF2mPoint
{
	public override ECFieldElement YCoord
	{
		get
		{
			ECFieldElement rawXCoord = base.RawXCoord;
			ECFieldElement rawYCoord = base.RawYCoord;
			if (base.IsInfinity || rawXCoord.IsZero)
			{
				return rawYCoord;
			}
			ECFieldElement eCFieldElement = rawYCoord.Add(rawXCoord).Multiply(rawXCoord);
			ECFieldElement eCFieldElement2 = base.RawZCoords[0];
			if (!eCFieldElement2.IsOne)
			{
				eCFieldElement = eCFieldElement.Divide(eCFieldElement2);
			}
			return eCFieldElement;
		}
	}

	protected internal override bool CompressionYTilde
	{
		get
		{
			ECFieldElement rawXCoord = base.RawXCoord;
			if (rawXCoord.IsZero)
			{
				return false;
			}
			return base.RawYCoord.TestBitZero() != rawXCoord.TestBitZero();
		}
	}

	internal SecT163K1Point(ECCurve curve, ECFieldElement x, ECFieldElement y)
		: base(curve, x, y)
	{
	}

	internal SecT163K1Point(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs)
		: base(curve, x, y, zs)
	{
	}

	protected override ECPoint Detach()
	{
		return new SecT163K1Point(null, AffineXCoord, AffineYCoord);
	}

	public override ECPoint Add(ECPoint b)
	{
		if (base.IsInfinity)
		{
			return b;
		}
		if (b.IsInfinity)
		{
			return this;
		}
		ECCurve curve = Curve;
		SecT163FieldElement secT163FieldElement = (SecT163FieldElement)base.RawXCoord;
		SecT163FieldElement secT163FieldElement2 = (SecT163FieldElement)b.RawXCoord;
		if (secT163FieldElement.IsZero)
		{
			if (secT163FieldElement2.IsZero)
			{
				return curve.Infinity;
			}
			return b.Add(this);
		}
		SecT163FieldElement secT163FieldElement3 = (SecT163FieldElement)base.RawYCoord;
		SecT163FieldElement secT163FieldElement4 = (SecT163FieldElement)base.RawZCoords[0];
		SecT163FieldElement secT163FieldElement5 = (SecT163FieldElement)b.RawYCoord;
		SecT163FieldElement secT163FieldElement6 = (SecT163FieldElement)b.RawZCoords[0];
		ulong[] array = Nat192.CreateExt64();
		ulong[] array2 = Nat192.Create64();
		ulong[] array3 = Nat192.Create64();
		ulong[] array4 = Nat192.Create64();
		bool isOne = secT163FieldElement4.IsOne;
		if (isOne)
		{
			Nat192.Copy64(secT163FieldElement2.x, array2);
			Nat192.Copy64(secT163FieldElement5.x, array3);
		}
		else
		{
			SecT163Field.Multiply(secT163FieldElement2.x, secT163FieldElement4.x, array2);
			SecT163Field.Multiply(secT163FieldElement5.x, secT163FieldElement4.x, array3);
		}
		bool isOne2 = secT163FieldElement6.IsOne;
		if (isOne2)
		{
			Nat192.Copy64(secT163FieldElement.x, array4);
			Nat192.Copy64(secT163FieldElement3.x, array);
		}
		else
		{
			SecT163Field.Multiply(secT163FieldElement.x, secT163FieldElement6.x, array4);
			SecT163Field.Multiply(secT163FieldElement3.x, secT163FieldElement6.x, array);
		}
		SecT163Field.AddTo(array, array3);
		SecT163Field.Add(array4, array2, array);
		if (Nat192.IsZero64(array))
		{
			if (Nat192.IsZero64(array3))
			{
				return Twice();
			}
			return curve.Infinity;
		}
		if (secT163FieldElement2.IsZero)
		{
			ECPoint eCPoint = Normalize();
			secT163FieldElement = (SecT163FieldElement)eCPoint.XCoord;
			ECFieldElement yCoord = eCPoint.YCoord;
			ECFieldElement b2 = secT163FieldElement5;
			ECFieldElement eCFieldElement = yCoord.Add(b2).Divide(secT163FieldElement);
			ECFieldElement eCFieldElement2 = eCFieldElement.Square().Add(eCFieldElement).Add(secT163FieldElement);
			if (eCFieldElement2.IsZero)
			{
				return new SecT163K1Point(curve, eCFieldElement2, curve.B);
			}
			ECFieldElement y = eCFieldElement.Multiply(secT163FieldElement.Add(eCFieldElement2)).Add(eCFieldElement2).Add(yCoord)
				.Divide(eCFieldElement2)
				.Add(eCFieldElement2);
			ECFieldElement eCFieldElement3 = curve.FromBigInteger(BigInteger.One);
			return new SecT163K1Point(curve, eCFieldElement2, y, new ECFieldElement[1] { eCFieldElement3 });
		}
		SecT163Field.Square(array, array);
		SecT163Field.Multiply(array4, array3, array4);
		SecT163Field.Multiply(array2, array3, array2);
		ulong[] array5 = array4;
		SecT163Field.Multiply(array5, array2, array5);
		if (Nat192.IsZero64(array5))
		{
			return new SecT163K1Point(curve, new SecT163FieldElement(array5), curve.B);
		}
		ulong[] array6 = array3;
		SecT163Field.Multiply(array6, array, array6);
		if (!isOne2)
		{
			SecT163Field.Multiply(array6, secT163FieldElement6.x, array6);
		}
		ulong[] array7 = array2;
		SecT163Field.AddTo(array, array7);
		SecT163Field.SquareExt(array7, array);
		SecT163Field.Add(secT163FieldElement3.x, secT163FieldElement4.x, array7);
		SecT163Field.MultiplyAddToExt(array6, array7, array);
		SecT163Field.Reduce(array, array7);
		if (!isOne)
		{
			SecT163Field.Multiply(array6, secT163FieldElement4.x, array6);
		}
		return new SecT163K1Point(curve, new SecT163FieldElement(array5), new SecT163FieldElement(array7), new ECFieldElement[1]
		{
			new SecT163FieldElement(array6)
		});
	}

	public override ECPoint Twice()
	{
		if (base.IsInfinity)
		{
			return this;
		}
		ECCurve curve = Curve;
		ECFieldElement rawXCoord = base.RawXCoord;
		if (rawXCoord.IsZero)
		{
			return curve.Infinity;
		}
		ECFieldElement rawYCoord = base.RawYCoord;
		ECFieldElement eCFieldElement = base.RawZCoords[0];
		bool isOne = eCFieldElement.IsOne;
		ECFieldElement b = (isOne ? rawYCoord : rawYCoord.Multiply(eCFieldElement));
		ECFieldElement b2 = (isOne ? eCFieldElement : eCFieldElement.Square());
		ECFieldElement eCFieldElement2 = rawYCoord.Square().Add(b).Add(b2);
		if (eCFieldElement2.IsZero)
		{
			return new SecT163K1Point(curve, eCFieldElement2, curve.B);
		}
		ECFieldElement eCFieldElement3 = eCFieldElement2.Square();
		ECFieldElement eCFieldElement4 = (isOne ? eCFieldElement2 : eCFieldElement2.Multiply(b2));
		ECFieldElement eCFieldElement5 = rawYCoord.Add(rawXCoord).Square();
		ECFieldElement y = eCFieldElement5.Add(eCFieldElement2).Add(b2).Multiply(eCFieldElement5)
			.Add(eCFieldElement3);
		return new SecT163K1Point(curve, eCFieldElement3, y, new ECFieldElement[1] { eCFieldElement4 });
	}

	public override ECPoint TwicePlus(ECPoint b)
	{
		if (base.IsInfinity)
		{
			return b;
		}
		if (b.IsInfinity)
		{
			return Twice();
		}
		ECCurve curve = Curve;
		ECFieldElement rawXCoord = base.RawXCoord;
		if (rawXCoord.IsZero)
		{
			return b;
		}
		ECFieldElement rawXCoord2 = b.RawXCoord;
		ECFieldElement eCFieldElement = b.RawZCoords[0];
		if (rawXCoord2.IsZero || !eCFieldElement.IsOne)
		{
			return Twice().Add(b);
		}
		ECFieldElement rawYCoord = base.RawYCoord;
		ECFieldElement eCFieldElement2 = base.RawZCoords[0];
		ECFieldElement rawYCoord2 = b.RawYCoord;
		ECFieldElement x = rawXCoord.Square();
		ECFieldElement b2 = rawYCoord.Square();
		ECFieldElement eCFieldElement3 = eCFieldElement2.Square();
		ECFieldElement b3 = rawYCoord.Multiply(eCFieldElement2);
		ECFieldElement b4 = eCFieldElement3.Add(b2).Add(b3);
		ECFieldElement eCFieldElement4 = rawYCoord2.Multiply(eCFieldElement3).Add(b2).MultiplyPlusProduct(b4, x, eCFieldElement3);
		ECFieldElement eCFieldElement5 = rawXCoord2.Multiply(eCFieldElement3);
		ECFieldElement eCFieldElement6 = eCFieldElement5.Add(b4).Square();
		if (eCFieldElement6.IsZero)
		{
			if (eCFieldElement4.IsZero)
			{
				return b.Twice();
			}
			return curve.Infinity;
		}
		if (eCFieldElement4.IsZero)
		{
			return new SecT163K1Point(curve, eCFieldElement4, curve.B);
		}
		ECFieldElement x2 = eCFieldElement4.Square().Multiply(eCFieldElement5);
		ECFieldElement eCFieldElement7 = eCFieldElement4.Multiply(eCFieldElement6).Multiply(eCFieldElement3);
		ECFieldElement y = eCFieldElement4.Add(eCFieldElement6).Square().MultiplyPlusProduct(b4, rawYCoord2.AddOne(), eCFieldElement7);
		return new SecT163K1Point(curve, x2, y, new ECFieldElement[1] { eCFieldElement7 });
	}

	public override ECPoint Negate()
	{
		if (base.IsInfinity)
		{
			return this;
		}
		ECFieldElement rawXCoord = base.RawXCoord;
		if (rawXCoord.IsZero)
		{
			return this;
		}
		ECFieldElement rawYCoord = base.RawYCoord;
		ECFieldElement eCFieldElement = base.RawZCoords[0];
		return new SecT163K1Point(Curve, rawXCoord, rawYCoord.Add(eCFieldElement), new ECFieldElement[1] { eCFieldElement });
	}
}
