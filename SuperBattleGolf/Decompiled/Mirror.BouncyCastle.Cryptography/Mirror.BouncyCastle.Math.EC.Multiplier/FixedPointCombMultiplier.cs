using System;
using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Math.EC.Multiplier;

public class FixedPointCombMultiplier : AbstractECMultiplier
{
	protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
	{
		ECCurve curve = p.Curve;
		int combSize = FixedPointUtilities.GetCombSize(curve);
		if (k.BitLength > combSize)
		{
			throw new InvalidOperationException("fixed-point comb doesn't support scalars larger than the curve order");
		}
		FixedPointPreCompInfo fixedPointPreCompInfo = FixedPointUtilities.Precompute(p);
		ECLookupTable lookupTable = fixedPointPreCompInfo.LookupTable;
		int width = fixedPointPreCompInfo.Width;
		int num = (combSize + width - 1) / width;
		int num2 = num * width;
		uint[] array = Nat.FromBigInteger(num2, k);
		ECPoint eCPoint = curve.Infinity;
		for (int i = 1; i <= num; i++)
		{
			uint num3 = 0u;
			for (int num4 = num2 - i; num4 >= 0; num4 -= num)
			{
				uint num5 = array[num4 >> 5] >> num4;
				num3 ^= num5 >> 1;
				num3 <<= 1;
				num3 ^= num5;
			}
			ECPoint b = lookupTable.Lookup((int)num3);
			eCPoint = eCPoint.TwicePlus(b);
		}
		return eCPoint.Add(fixedPointPreCompInfo.Offset);
	}
}
