using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Crypto.Modes.Gcm;

[Obsolete("Will be removed")]
public class Tables1kGcmExponentiator : IGcmExponentiator
{
	private IList<GcmUtilities.FieldElement> lookupPowX2;

	public void Init(byte[] x)
	{
		GcmUtilities.AsFieldElement(x, out var z);
		if (lookupPowX2 == null || !z.Equals(lookupPowX2[0]))
		{
			lookupPowX2 = new List<GcmUtilities.FieldElement>(8);
			lookupPowX2.Add(z);
		}
	}

	public void ExponentiateX(long pow, byte[] output)
	{
		GcmUtilities.One(out var x);
		int num = 0;
		while (pow > 0)
		{
			if ((pow & 1) != 0L)
			{
				EnsureAvailable(num);
				GcmUtilities.FieldElement y = lookupPowX2[num];
				GcmUtilities.Multiply(ref x, ref y);
			}
			num++;
			pow >>= 1;
		}
		GcmUtilities.AsBytes(ref x, output);
	}

	private void EnsureAvailable(int bit)
	{
		int num = lookupPowX2.Count;
		if (num <= bit)
		{
			GcmUtilities.FieldElement x = lookupPowX2[num - 1];
			do
			{
				GcmUtilities.Square(ref x);
				lookupPowX2.Add(x);
			}
			while (++num <= bit);
		}
	}
}
