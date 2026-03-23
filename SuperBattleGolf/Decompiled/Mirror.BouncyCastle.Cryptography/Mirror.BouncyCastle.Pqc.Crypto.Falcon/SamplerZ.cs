namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class SamplerZ
{
	private FalconRNG p;

	private FalconFPR sigma_min;

	private FprEngine fpre;

	internal SamplerZ(FalconRNG p, FalconFPR sigma_min, FprEngine fpre)
	{
		this.p = p;
		this.sigma_min = sigma_min;
		this.fpre = fpre;
	}

	internal int Sample(FalconFPR mu, FalconFPR isigma)
	{
		return sampler(mu, isigma);
	}

	private int gaussian0_sampler(FalconRNG p)
	{
		uint[] array = new uint[54]
		{
			10745844u, 3068844u, 3741698u, 5559083u, 1580863u, 8248194u, 2260429u, 13669192u, 2736639u, 708981u,
			4421575u, 10046180u, 169348u, 7122675u, 4136815u, 30538u, 13063405u, 7650655u, 4132u, 14505003u,
			7826148u, 417u, 16768101u, 11363290u, 31u, 8444042u, 8086568u, 1u, 12844466u, 265321u,
			0u, 1232676u, 13644283u, 0u, 38047u, 9111839u, 0u, 870u, 6138264u, 0u,
			14u, 12545723u, 0u, 0u, 3104126u, 0u, 0u, 28824u, 0u, 0u,
			198u, 0u, 0u, 1u
		};
		ulong num = p.prng_get_u64();
		uint num2 = p.prng_get_u8();
		uint num3 = (uint)((int)num & 0xFFFFFF);
		uint num4 = (uint)((int)(num >> 24) & 0xFFFFFF);
		uint num5 = (uint)(int)(num >> 48) | (num2 << 16);
		int num6 = 0;
		for (int i = 0; i < array.Length; i += 3)
		{
			uint num7 = array[i + 2];
			uint num8 = array[i + 1];
			uint num9 = array[i];
			uint num10 = num3 - num7 >> 31;
			num10 = num4 - num8 - num10 >> 31;
			num10 = num5 - num9 - num10 >> 31;
			num6 += (int)num10;
		}
		return num6;
	}

	private int BerExp(FalconRNG p, FalconFPR x, FalconFPR ccs)
	{
		int num = (int)fpre.fpr_trunc(fpre.fpr_mul(x, fpre.fpr_inv_log2));
		FalconFPR x2 = fpre.fpr_sub(x, fpre.fpr_mul(fpre.fpr_of(num), fpre.fpr_log2));
		uint num2 = (uint)num;
		num2 ^= (uint)(int)((num2 ^ 0x3F) & (0L - (long)(63 - num2 >> 31)));
		num = (int)num2;
		ulong num3 = (fpre.fpr_expm_p63(x2, ccs) << 1) - 1 >> num;
		int num4 = 64;
		uint num5;
		do
		{
			num4 -= 8;
			num5 = p.prng_get_u8() - (uint)((int)(num3 >> num4) & 0xFF);
		}
		while (num5 == 0 && num4 > 0);
		return (int)(num5 >> 31);
	}

	private int sampler(FalconFPR mu, FalconFPR isigma)
	{
		int num = (int)fpre.fpr_floor(mu);
		FalconFPR y = fpre.fpr_sub(mu, fpre.fpr_of(num));
		FalconFPR y2 = fpre.fpr_half(fpre.fpr_sqr(isigma));
		FalconFPR ccs = fpre.fpr_mul(isigma, sigma_min);
		int num4;
		FalconFPR x;
		do
		{
			int num2 = gaussian0_sampler(p);
			uint num3 = p.prng_get_u8() & 1;
			num4 = (int)num3 + (int)((num3 << 1) - 1) * num2;
			x = fpre.fpr_mul(fpre.fpr_sqr(fpre.fpr_sub(fpre.fpr_of(num4), y)), y2);
			x = fpre.fpr_sub(x, fpre.fpr_mul(fpre.fpr_of(num2 * num2), fpre.fpr_inv_2sqrsigma0));
		}
		while (BerExp(p, x, ccs) == 0);
		return num + num4;
	}
}
