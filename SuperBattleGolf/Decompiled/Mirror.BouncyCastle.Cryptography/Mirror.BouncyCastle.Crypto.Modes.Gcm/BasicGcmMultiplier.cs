using System;

namespace Mirror.BouncyCastle.Crypto.Modes.Gcm;

[Obsolete("Will be removed")]
public class BasicGcmMultiplier : IGcmMultiplier
{
	private GcmUtilities.FieldElement H;

	internal static bool IsHardwareAccelerated => false;

	public void Init(byte[] H)
	{
		GcmUtilities.AsFieldElement(H, out this.H);
	}

	public void MultiplyH(byte[] x)
	{
		GcmUtilities.AsFieldElement(x, out var z);
		GcmUtilities.Multiply(ref z, ref H);
		GcmUtilities.AsBytes(ref z, x);
	}
}
