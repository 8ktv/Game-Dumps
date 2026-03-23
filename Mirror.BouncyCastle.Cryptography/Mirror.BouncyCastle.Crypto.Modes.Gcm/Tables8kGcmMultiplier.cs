using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Modes.Gcm;

[Obsolete("Will be removed")]
public class Tables8kGcmMultiplier : IGcmMultiplier
{
	private byte[] H;

	private GcmUtilities.FieldElement[][] T;

	public void Init(byte[] H)
	{
		if (T == null)
		{
			T = new GcmUtilities.FieldElement[2][];
		}
		else if (Arrays.AreEqual(this.H, H))
		{
			return;
		}
		this.H = Arrays.Clone(H);
		for (int i = 0; i < 2; i++)
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
		GcmUtilities.FieldElement[] array = T[0];
		GcmUtilities.FieldElement[] array2 = T[1];
		int num = x[15];
		int num2 = x[14];
		ulong num3 = array[num2].n1 ^ array2[num].n1;
		ulong num4 = array[num2].n0 ^ array2[num].n0;
		for (int num5 = 12; num5 >= 0; num5 -= 2)
		{
			num = x[num5 + 1];
			num2 = x[num5];
			ulong num6 = num3 << 48;
			num3 = array[num2].n1 ^ array2[num].n1 ^ ((num3 >> 16) | (num4 << 48));
			num4 = array[num2].n0 ^ array2[num].n0 ^ (num4 >> 16) ^ num6 ^ (num6 >> 1) ^ (num6 >> 2) ^ (num6 >> 7);
		}
		GcmUtilities.AsBytes(num4, num3, x);
	}
}
