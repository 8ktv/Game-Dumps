using System;

namespace Mirror.BouncyCastle.Crypto.Modes.Gcm;

[Obsolete("Will be removed")]
public class BasicGcmExponentiator : IGcmExponentiator
{
	private GcmUtilities.FieldElement x;

	public void Init(byte[] x)
	{
		GcmUtilities.AsFieldElement(x, out this.x);
	}

	public void ExponentiateX(long pow, byte[] output)
	{
		GcmUtilities.One(out var fieldElement);
		if (pow > 0)
		{
			GcmUtilities.FieldElement y = x;
			do
			{
				if ((pow & 1) != 0L)
				{
					GcmUtilities.Multiply(ref fieldElement, ref y);
				}
				GcmUtilities.Square(ref y);
				pow >>= 1;
			}
			while (pow > 0);
		}
		GcmUtilities.AsBytes(ref fieldElement, output);
	}
}
