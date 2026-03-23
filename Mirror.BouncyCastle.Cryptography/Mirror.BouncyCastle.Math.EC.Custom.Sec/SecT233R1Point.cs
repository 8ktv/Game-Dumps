using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Math.EC.Custom.Sec;

internal class SecT233R1Point : AbstractF2mPoint
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

	internal SecT233R1Point(ECCurve curve, ECFieldElement x, ECFieldElement y)
		: base(curve, x, y)
	{
	}

	internal SecT233R1Point(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs)
		: base(curve, x, y, zs)
	{
	}

	protected override ECPoint Detach()
	{
		return new SecT233R1Point(null, AffineXCoord, AffineYCoord);
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
		ECFieldElement rawXCoord = base.RawXCoord;
		ECFieldElement rawXCoord2 = b.RawXCoord;
		if (rawXCoord.IsZero)
		{
			if (rawXCoord2.IsZero)
			{
				return curve.Infinity;
			}
			return b.Add(this);
		}
		ECFieldElement rawYCoord = base.RawYCoord;
		ECFieldElement eCFieldElement = base.RawZCoords[0];
		ECFieldElement rawYCoord2 = b.RawYCoord;
		ECFieldElement eCFieldElement2 = b.RawZCoords[0];
		bool isOne = eCFieldElement.IsOne;
		ECFieldElement eCFieldElement3 = rawXCoord2;
		ECFieldElement eCFieldElement4 = rawYCoord2;
		if (!isOne)
		{
			eCFieldElement3 = eCFieldElement3.Multiply(eCFieldElement);
			eCFieldElement4 = eCFieldElement4.Multiply(eCFieldElement);
		}
		bool isOne2 = eCFieldElement2.IsOne;
		ECFieldElement eCFieldElement5 = rawXCoord;
		ECFieldElement eCFieldElement6 = rawYCoord;
		if (!isOne2)
		{
			eCFieldElement5 = eCFieldElement5.Multiply(eCFieldElement2);
			eCFieldElement6 = eCFieldElement6.Multiply(eCFieldElement2);
		}
		ECFieldElement eCFieldElement7 = eCFieldElement6.Add(eCFieldElement4);
		ECFieldElement eCFieldElement8 = eCFieldElement5.Add(eCFieldElement3);
		if (eCFieldElement8.IsZero)
		{
			if (eCFieldElement7.IsZero)
			{
				return Twice();
			}
			return curve.Infinity;
		}
		ECFieldElement eCFieldElement10;
		ECFieldElement y;
		ECFieldElement eCFieldElement11;
		if (rawXCoord2.IsZero)
		{
			ECPoint eCPoint = Normalize();
			rawXCoord = eCPoint.XCoord;
			ECFieldElement yCoord = eCPoint.YCoord;
			ECFieldElement b2 = rawYCoord2;
			ECFieldElement eCFieldElement9 = yCoord.Add(b2).Divide(rawXCoord);
			eCFieldElement10 = eCFieldElement9.Square().Add(eCFieldElement9).Add(rawXCoord)
				.AddOne();
			if (eCFieldElement10.IsZero)
			{
				return new SecT233R1Point(curve, eCFieldElement10, curve.B.Sqrt());
			}
			y = eCFieldElement9.Multiply(rawXCoord.Add(eCFieldElement10)).Add(eCFieldElement10).Add(yCoord)
				.Divide(eCFieldElement10)
				.Add(eCFieldElement10);
			eCFieldElement11 = curve.FromBigInteger(BigInteger.One);
		}
		else
		{
			eCFieldElement8 = eCFieldElement8.Square();
			ECFieldElement eCFieldElement12 = eCFieldElement7.Multiply(eCFieldElement5);
			ECFieldElement eCFieldElement13 = eCFieldElement7.Multiply(eCFieldElement3);
			eCFieldElement10 = eCFieldElement12.Multiply(eCFieldElement13);
			if (eCFieldElement10.IsZero)
			{
				return new SecT233R1Point(curve, eCFieldElement10, curve.B.Sqrt());
			}
			ECFieldElement eCFieldElement14 = eCFieldElement7.Multiply(eCFieldElement8);
			if (!isOne2)
			{
				eCFieldElement14 = eCFieldElement14.Multiply(eCFieldElement2);
			}
			y = eCFieldElement13.Add(eCFieldElement8).SquarePlusProduct(eCFieldElement14, rawYCoord.Add(eCFieldElement));
			eCFieldElement11 = eCFieldElement14;
			if (!isOne)
			{
				eCFieldElement11 = eCFieldElement11.Multiply(eCFieldElement);
			}
		}
		return new SecT233R1Point(curve, eCFieldElement10, y, new ECFieldElement[1] { eCFieldElement11 });
	}

	public override ECPoint Twice()
	{
		if (base.IsInfinity)
		{
			return this;
		}
		ECCurve curve = Curve;
		SecT233FieldElement secT233FieldElement = (SecT233FieldElement)base.RawXCoord;
		if (secT233FieldElement.IsZero)
		{
			return curve.Infinity;
		}
		SecT233FieldElement secT233FieldElement2 = (SecT233FieldElement)base.RawYCoord;
		SecT233FieldElement secT233FieldElement3 = (SecT233FieldElement)base.RawZCoords[0];
		ulong[] array = Nat256.CreateExt64();
		ulong[] array2 = Nat256.Create64();
		ulong[] array3 = Nat256.Create64();
		ulong[] array4 = Nat256.Create64();
		if (secT233FieldElement3.IsOne)
		{
			SecT233Field.Square(secT233FieldElement2.x, array4);
			SecT233Field.AddBothTo(secT233FieldElement2.x, secT233FieldElement3.x, array4);
			if (Nat256.IsZero64(array4))
			{
				return new SecT233R1Point(curve, new SecT233FieldElement(array4), curve.B.Sqrt());
			}
			SecT233Field.Square(array4, array2);
			SecT233Field.SquareExt(secT233FieldElement.x, array);
			SecT233Field.MultiplyAddToExt(array4, secT233FieldElement2.x, array);
		}
		else
		{
			ulong[] array5 = Nat256.Create64();
			SecT233Field.Multiply(secT233FieldElement2.x, secT233FieldElement3.x, array5);
			SecT233Field.Square(secT233FieldElement3.x, array);
			SecT233Field.Square(secT233FieldElement2.x, array2);
			SecT233Field.AddBothTo(array5, array, array2);
			if (Nat256.IsZero64(array2))
			{
				return new SecT233R1Point(curve, new SecT233FieldElement(array2), curve.B.Sqrt());
			}
			SecT233Field.Multiply(array2, array, array4);
			SecT233Field.Multiply(secT233FieldElement.x, secT233FieldElement3.x, array);
			SecT233Field.SquareExt(array, array);
			SecT233Field.MultiplyAddToExt(array2, array5, array);
			SecT233Field.Square(array2, array2);
		}
		SecT233Field.Reduce(array, array3);
		SecT233Field.AddBothTo(array2, array4, array3);
		return new SecT233R1Point(curve, new SecT233FieldElement(array2), new SecT233FieldElement(array3), new ECFieldElement[1]
		{
			new SecT233FieldElement(array4)
		});
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
		SecT233FieldElement secT233FieldElement = (SecT233FieldElement)base.RawXCoord;
		if (secT233FieldElement.IsZero)
		{
			return b;
		}
		SecT233FieldElement secT233FieldElement2 = (SecT233FieldElement)b.RawXCoord;
		SecT233FieldElement secT233FieldElement3 = (SecT233FieldElement)b.RawZCoords[0];
		if (secT233FieldElement2.IsZero || !secT233FieldElement3.IsOne)
		{
			return Twice().Add(b);
		}
		SecT233FieldElement obj = (SecT233FieldElement)base.RawYCoord;
		SecT233FieldElement secT233FieldElement4 = (SecT233FieldElement)base.RawZCoords[0];
		SecT233FieldElement secT233FieldElement5 = (SecT233FieldElement)b.RawYCoord;
		ulong[] array = Nat256.CreateExt64();
		ulong[] array2 = Nat256.Create64();
		ulong[] array3 = Nat256.Create64();
		ulong[] array4 = Nat256.Create64();
		ulong[] array5 = Nat256.Create64();
		ulong[] array6 = Nat256.Create64();
		SecT233Field.Square(secT233FieldElement.x, array2);
		SecT233Field.Square(obj.x, array3);
		SecT233Field.Square(secT233FieldElement4.x, array4);
		SecT233Field.Multiply(obj.x, secT233FieldElement4.x, array5);
		SecT233Field.AddBothTo(array3, array4, array5);
		SecT233Field.MultiplyExt(array2, array4, array);
		SecT233Field.Multiply(secT233FieldElement5.x, array4, array2);
		SecT233Field.AddTo(array3, array2);
		SecT233Field.MultiplyAddToExt(array5, array2, array);
		SecT233Field.Reduce(array, array2);
		SecT233Field.Multiply(secT233FieldElement2.x, array4, array3);
		SecT233Field.Add(array5, array3, array6);
		SecT233Field.Square(array6, array6);
		if (Nat256.IsZero64(array6))
		{
			if (Nat256.IsZero64(array2))
			{
				return b.Twice();
			}
			return curve.Infinity;
		}
		if (Nat256.IsZero64(array2))
		{
			return new SecT233R1Point(curve, new SecT233FieldElement(array2), curve.B.Sqrt());
		}
		ulong[] array7 = array3;
		SecT233Field.Square(array2, array);
		SecT233Field.Multiply(array7, array, array7);
		ulong[] array8 = array4;
		SecT233Field.Multiply(array8, array2, array8);
		SecT233Field.Multiply(array8, array6, array8);
		ulong[] array9 = array2;
		SecT233Field.AddTo(array6, array9);
		SecT233Field.Square(array9, array9);
		SecT233Field.MultiplyExt(array9, array5, array);
		SecT233Field.MultiplyAddToExt(secT233FieldElement5.x, array8, array);
		SecT233Field.Reduce(array, array9);
		SecT233Field.AddTo(array8, array9);
		return new SecT233R1Point(curve, new SecT233FieldElement(array7), new SecT233FieldElement(array9), new ECFieldElement[1]
		{
			new SecT233FieldElement(array8)
		});
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
		return new SecT233R1Point(Curve, rawXCoord, rawYCoord.Add(eCFieldElement), new ECFieldElement[1] { eCFieldElement });
	}
}
