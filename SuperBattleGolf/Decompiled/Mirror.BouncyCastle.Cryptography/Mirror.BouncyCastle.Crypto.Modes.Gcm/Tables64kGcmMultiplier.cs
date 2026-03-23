using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Modes.Gcm;

[Obsolete("Will be removed")]
public class Tables64kGcmMultiplier : IGcmMultiplier
{
	private byte[] H;

	private GcmUtilities.FieldElement[][] T;

	public void Init(byte[] H)
	{
		if (T == null)
		{
			T = new GcmUtilities.FieldElement[16][];
		}
		else if (Arrays.AreEqual(this.H, H))
		{
			return;
		}
		this.H = Arrays.Clone(H);
		for (int i = 0; i < 16; i++)
		{
			GcmUtilities.FieldElement[] array = (T[i] = new GcmUtilities.FieldElement[256]);
			if (i == 0)
			{
				GcmUtilities.AsFieldElement(this.H, out array[1]);
				GcmUtilities.MultiplyP7(ref array[1]);
			}
			else
			{
				GcmUtilities.MultiplyP8(ref T[i - 1][1], out array[1]);
			}
			for (int j = 1; j < 128; j++)
			{
				GcmUtilities.DivideP(ref array[j], out array[j << 1]);
				GcmUtilities.Xor(ref array[j << 1], ref array[1], out array[(j << 1) + 1]);
			}
		}
	}

	public void MultiplyH(byte[] x)
	{
		GcmUtilities.FieldElement[] array = T[15];
		int num = x[15];
		ulong num2 = array[num].n0;
		ulong num3 = array[num].n1;
		for (int num4 = 14; num4 >= 0; num4--)
		{
			array = T[num4];
			num = x[num4];
			num2 ^= array[num].n0;
			num3 ^= array[num].n1;
		}
		GcmUtilities.AsBytes(num2, num3, x);
	}
}
