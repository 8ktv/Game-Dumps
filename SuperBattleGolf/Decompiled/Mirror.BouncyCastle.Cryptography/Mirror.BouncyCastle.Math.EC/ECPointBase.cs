using System;

namespace Mirror.BouncyCastle.Math.EC;

public abstract class ECPointBase : ECPoint
{
	protected internal ECPointBase(ECCurve curve, ECFieldElement x, ECFieldElement y)
		: base(curve, x, y)
	{
	}

	protected internal ECPointBase(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs)
		: base(curve, x, y, zs)
	{
	}

	public override byte[] GetEncoded(bool compressed)
	{
		if (base.IsInfinity)
		{
			return new byte[1];
		}
		ECPoint eCPoint = Normalize();
		byte[] encoded = eCPoint.XCoord.GetEncoded();
		if (compressed)
		{
			byte[] array = new byte[encoded.Length + 1];
			array[0] = (byte)(eCPoint.CompressionYTilde ? 3u : 2u);
			Array.Copy(encoded, 0, array, 1, encoded.Length);
			return array;
		}
		byte[] encoded2 = eCPoint.YCoord.GetEncoded();
		byte[] array2 = new byte[encoded.Length + encoded2.Length + 1];
		array2[0] = 4;
		Array.Copy(encoded, 0, array2, 1, encoded.Length);
		Array.Copy(encoded2, 0, array2, encoded.Length + 1, encoded2.Length);
		return array2;
	}

	public override int GetEncodedLength(bool compressed)
	{
		if (base.IsInfinity)
		{
			return 1;
		}
		if (compressed)
		{
			return 1 + XCoord.GetEncodedLength();
		}
		return 1 + XCoord.GetEncodedLength() + YCoord.GetEncodedLength();
	}

	public override void EncodeTo(bool compressed, byte[] buf, int off)
	{
		if (base.IsInfinity)
		{
			buf[off] = 0;
			return;
		}
		ECPoint eCPoint = Normalize();
		ECFieldElement xCoord = eCPoint.XCoord;
		ECFieldElement yCoord = eCPoint.YCoord;
		if (compressed)
		{
			buf[off] = (byte)(eCPoint.CompressionYTilde ? 3u : 2u);
			xCoord.EncodeTo(buf, off + 1);
		}
		else
		{
			buf[off] = 4;
			xCoord.EncodeTo(buf, off + 1);
			yCoord.EncodeTo(buf, off + 1 + xCoord.GetEncodedLength());
		}
	}

	public override ECPoint Multiply(BigInteger k)
	{
		return Curve.GetMultiplier().Multiply(this, k);
	}
}
