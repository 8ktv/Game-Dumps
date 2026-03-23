using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Modes.Gcm;

[Obsolete("Will be removed")]
public class Tables4kGcmMultiplier : IGcmMultiplier
{
	private byte[] H;

	private GcmUtilities.FieldElement[] T;

	public void Init(byte[] H)
	{
		if (T == null)
		{
			T = new GcmUtilities.FieldElement[256];
		}
		else if (Arrays.AreEqual(this.H, H))
		{
			return;
		}
		this.H = Arrays.Clone(H);
		GcmUtilities.AsFieldElement(this.H, out T[1]);
		GcmUtilities.MultiplyP7(ref T[1]);
		for (int i = 1; i < 128; i++)
		{
			GcmUtilities.DivideP(ref T[i], out T[i << 1]);
			GcmUtilities.Xor(ref T[i << 1], ref T[1], out T[(i << 1) + 1]);
		}
	}

	public void MultiplyH(byte[] x)
	{
		int num = x[15];
		ulong num2 = T[num].n0;
		ulong num3 = T[num].n1;
		for (int num4 = 14; num4 >= 0; num4--)
		{
			num = x[num4];
			ulong num5 = num3 << 56;
			num3 = T[num].n1 ^ ((num3 >> 8) | (num2 << 56));
			num2 = T[num].n0 ^ (num2 >> 8) ^ num5 ^ (num5 >> 1) ^ (num5 >> 2) ^ (num5 >> 7);
		}
		GcmUtilities.AsBytes(num2, num3, x);
	}
}
