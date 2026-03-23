using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Crypto.Prng.Drbg;

internal class DrbgUtilities
{
	private static readonly IDictionary<string, int> MaxSecurityStrengths;

	static DrbgUtilities()
	{
		MaxSecurityStrengths = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		MaxSecurityStrengths.Add("SHA-1", 128);
		MaxSecurityStrengths.Add("SHA-224", 192);
		MaxSecurityStrengths.Add("SHA-256", 256);
		MaxSecurityStrengths.Add("SHA-384", 256);
		MaxSecurityStrengths.Add("SHA-512", 256);
		MaxSecurityStrengths.Add("SHA-512/224", 192);
		MaxSecurityStrengths.Add("SHA-512/256", 256);
	}

	internal static int GetMaxSecurityStrength(IDigest d)
	{
		return MaxSecurityStrengths[d.AlgorithmName];
	}

	internal static int GetMaxSecurityStrength(IMac m)
	{
		string algorithmName = m.AlgorithmName;
		return MaxSecurityStrengths[algorithmName.Substring(0, algorithmName.IndexOf("/"))];
	}

	internal static void HashDF(IDigest digest, byte[] seedMaterial, int seedLength, byte[] output)
	{
		int num = (seedLength + 7) / 8;
		int digestSize = digest.GetDigestSize();
		int num2 = num / digestSize;
		int num3 = 1;
		byte[] array = new byte[digestSize];
		byte[] array2 = new byte[5];
		Pack.UInt32_To_BE((uint)seedLength, array2, 1);
		int num4 = 0;
		while (num4 <= num2)
		{
			array2[0] = (byte)num3;
			digest.BlockUpdate(array2, 0, array2.Length);
			digest.BlockUpdate(seedMaterial, 0, seedMaterial.Length);
			digest.DoFinal(array, 0);
			int length = System.Math.Min(digestSize, num - num4 * digestSize);
			Array.Copy(array, 0, output, num4 * digestSize, length);
			num4++;
			num3++;
		}
		if (seedLength % 8 != 0)
		{
			int num5 = 8 - seedLength % 8;
			uint num6 = 0u;
			for (int i = 0; i != num; i++)
			{
				uint num7 = output[i];
				output[i] = (byte)((num7 >> num5) | (num6 << 8 - num5));
				num6 = num7;
			}
		}
	}
}
