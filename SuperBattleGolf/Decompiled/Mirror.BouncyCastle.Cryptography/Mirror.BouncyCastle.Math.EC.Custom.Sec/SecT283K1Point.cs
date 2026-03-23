using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Math.EC.Custom.Sec;

internal class SecT283K1Point : AbstractF2mPoint
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

	internal SecT283K1Point(ECCurve curve, ECFieldElement x, ECFieldElement y)
		: base(curve, x, y)
	{
	}

	internal SecT283K1Point(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs)
		: base(curve, x, y, zs)
	{
	}

	protected override ECPoint Detach()
	{
		return new SecT283K1Point(null, AffineXCoord, AffineYCoord);
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
		SecT283FieldElement secT283FieldElement = (SecT283FieldElement)base.RawXCoord;
		SecT283FieldElement secT283FieldElement2 = (SecT283FieldElement)b.RawXCoord;
		if (secT283FieldElement.IsZero)
		{
			if (secT283FieldElement2.IsZero)
			{
				return curve.Infinity;
			}
			return b.Add(this);
		}
		SecT283FieldElement secT283FieldElement3 = (SecT283FieldElement)base.RawYCoord;
		SecT283FieldElement secT283FieldElement4 = (SecT283FieldElement)base.RawZCoords[0];
		SecT283FieldElement secT283FieldElement5 = (SecT283FieldElement)b.RawYCoord;
		SecT283FieldElement secT283FieldElement6 = (SecT283FieldElement)b.RawZCoords[0];
		ulong[] array = Nat.Create64(9);
		ulong[] array2 = Nat320.Create64();
		ulong[] array3 = Nat320.Create64();
		ulong[] array4 = Nat320.Create64();
		bool isOne = secT283FieldElement4.IsOne;
		if (isOne)
		{
			Nat320.Copy64(secT283FieldElement2.x, array2);
			Nat320.Copy64(secT283FieldElement5.x, array3);
		}
		else
		{
			SecT283Field.Multiply(secT283FieldElement2.x, secT283FieldElement4.x, array2);
			SecT283Field.Multiply(secT283FieldElement5.x, secT283FieldElement4.x, array3);
		}
		bool isOne2 = secT283FieldElement6.IsOne;
		if (isOne2)
		{
			Nat320.Copy64(secT283FieldElement.x, array4);
			Nat320.Copy64(secT283FieldElement3.x, array);
		}
		else
		{
			SecT283Field.Multiply(secT283FieldElement.x, secT283FieldElement6.x, array4);
			SecT283Field.Multiply(secT283FieldElement3.x, secT283FieldElement6.x, array);
		}
		SecT283Field.AddTo(array, array3);
		SecT283Field.Add(array4, array2, array);
		if (Nat320.IsZero64(array))
		{
			if (Nat320.IsZero64(array3))
			{
				return Twice();
			}
			return curve.Infinity;
		}
		if (secT283FieldElement2.IsZero)
		{
			ECPoint eCPoint = Normalize();
			secT283FieldElement = (SecT283FieldElement)eCPoint.XCoord;
			ECFieldElement yCoord = eCPoint.YCoord;
			ECFieldElement b2 = secT283FieldElement5;
			ECFieldElement eCFieldElement = yCoord.Add(b2).Divide(secT283FieldElement);
			ECFieldElement eCFieldElement2 = eCFieldElement.Square().Add(eCFieldElement).Add(secT283FieldElement);
			if (eCFieldElement2.IsZero)
			{
				return new SecT283K1Point(curve, eCFieldElement2, curve.B);
			}
			ECFieldElement y = eCFieldElement.Multiply(secT283FieldElement.Add(eCFieldElement2)).Add(eCFieldElement2).Add(yCoord)
				.Divide(eCFieldElement2)
				.Add(eCFieldElement2);
			ECFieldElement eCFieldElement3 = curve.FromBigInteger(BigInteger.One);
			return new SecT283K1Point(curve, eCFieldElement2, y, new ECFieldElement[1] { eCFieldElement3 });
		}
		SecT283Field.Square(array, array);
		SecT283Field.Multiply(array4, array3, array4);
		SecT283Field.Multiply(array2, array3, array2);
		ulong[] array5 = array4;
		SecT283Field.Multiply(array5, array2, array5);
		if (Nat320.IsZero64(array5))
		{
			return new SecT283K1Point(curve, new SecT283FieldElement(array5), curve.B);
		}
		ulong[] array6 = array3;
		SecT283Field.Multiply(array6, array, array6);
		if (!isOne2)
		{
			SecT283Field.Multiply(array6, secT283FieldElement6.x, array6);
		}
		ulong[] array7 = array2;
		SecT283Field.AddTo(array, array7);
		SecT283Field.SquareExt(array7, array);
		SecT283Field.Add(secT283FieldElement3.x, secT283FieldElement4.x, array7);
		SecT283Field.MultiplyAddToExt(array6, array7, array);
		SecT283Field.Reduce(array, array7);
		if (!isOne)
		{
			SecT283Field.Multiply(array6, secT283FieldElement4.x, array6);
		}
		return new SecT283K1Point(curve, new SecT283FieldElement(array5), new SecT283FieldElement(array7), new ECFieldElement[1]
		{
			new SecT283FieldElement(array6)
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
		ECFieldElement eCFieldElement2 = (isOne ? eCFieldElement : eCFieldElement.Square());
		ECFieldElement eCFieldElement3 = ((!isOne) ? rawYCoord.Add(eCFieldElement).Multiply(rawYCoord) : rawYCoord.Square().Add(rawYCoord));
		if (eCFieldElement3.IsZero)
		{
			return new SecT283K1Point(curve, eCFieldElement3, curve.B);
		}
		ECFieldElement eCFieldElement4 = eCFieldElement3.Square();
		ECFieldElement eCFieldElement5 = (isOne ? eCFieldElement3 : eCFieldElement3.Multiply(eCFieldElement2));
		ECFieldElement eCFieldElement6 = rawYCoord.Add(rawXCoord).Square();
		ECFieldElement b = (isOne ? eCFieldElement : eCFieldElement2.Square());
		ECFieldElement y = eCFieldElement6.Add(eCFieldElement3).Add(eCFieldElement2).Multiply(eCFieldElement6)
			.Add(b)
			.Add(eCFieldElement4)
			.Add(eCFieldElement5);
		return new SecT283K1Point(curve, eCFieldElement4, y, new ECFieldElement[1] { eCFieldElement5 });
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
		ECFieldElement eCFieldElement3 = rawYCoord.Square();
		ECFieldElement eCFieldElement4 = eCFieldElement2.Square();
		ECFieldElement b2 = rawYCoord.Multiply(eCFieldElement2);
		ECFieldElement b3 = eCFieldElement3.Add(b2);
		ECFieldElement eCFieldElement5 = rawYCoord2.AddOne();
		ECFieldElement eCFieldElement6 = eCFieldElement5.Multiply(eCFieldElement4).Add(eCFieldElement3).MultiplyPlusProduct(b3, x, eCFieldElement4);
		ECFieldElement eCFieldElement7 = rawXCoord2.Multiply(eCFieldElement4);
		ECFieldElement eCFieldElement8 = eCFieldElement7.Add(b3).Square();
		if (eCFieldElement8.IsZero)
		{
			if (eCFieldElement6.IsZero)
			{
				return b.Twice();
			}
			return curve.Infinity;
		}
		if (eCFieldElement6.IsZero)
		{
			return new SecT283K1Point(curve, eCFieldElement6, curve.B);
		}
		ECFieldElement x2 = eCFieldElement6.Square().Multiply(eCFieldElement7);
		ECFieldElement eCFieldElement9 = eCFieldElement6.Multiply(eCFieldElement8).Multiply(eCFieldElement4);
		ECFieldElement y = eCFieldElement6.Add(eCFieldElement8).Square().MultiplyPlusProduct(b3, eCFieldElement5, eCFieldElement9);
		return new SecT283K1Point(curve, x2, y, new ECFieldElement[1] { eCFieldElement9 });
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
		return new SecT283K1Point(Curve, rawXCoord, rawYCoord.Add(eCFieldElement), new ECFieldElement[1] { eCFieldElement });
	}
}
