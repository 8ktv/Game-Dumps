using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class FalconKeygen
{
	private readonly FprEngine fpre;

	private readonly FalconFFT ffte;

	private readonly FalconSmallPrime[] PRIMES;

	private readonly FalconCodec codec;

	private readonly FalconVrfy vrfy;

	internal ushort[] REV10 = new ushort[1024]
	{
		0, 512, 256, 768, 128, 640, 384, 896, 64, 576,
		320, 832, 192, 704, 448, 960, 32, 544, 288, 800,
		160, 672, 416, 928, 96, 608, 352, 864, 224, 736,
		480, 992, 16, 528, 272, 784, 144, 656, 400, 912,
		80, 592, 336, 848, 208, 720, 464, 976, 48, 560,
		304, 816, 176, 688, 432, 944, 112, 624, 368, 880,
		240, 752, 496, 1008, 8, 520, 264, 776, 136, 648,
		392, 904, 72, 584, 328, 840, 200, 712, 456, 968,
		40, 552, 296, 808, 168, 680, 424, 936, 104, 616,
		360, 872, 232, 744, 488, 1000, 24, 536, 280, 792,
		152, 664, 408, 920, 88, 600, 344, 856, 216, 728,
		472, 984, 56, 568, 312, 824, 184, 696, 440, 952,
		120, 632, 376, 888, 248, 760, 504, 1016, 4, 516,
		260, 772, 132, 644, 388, 900, 68, 580, 324, 836,
		196, 708, 452, 964, 36, 548, 292, 804, 164, 676,
		420, 932, 100, 612, 356, 868, 228, 740, 484, 996,
		20, 532, 276, 788, 148, 660, 404, 916, 84, 596,
		340, 852, 212, 724, 468, 980, 52, 564, 308, 820,
		180, 692, 436, 948, 116, 628, 372, 884, 244, 756,
		500, 1012, 12, 524, 268, 780, 140, 652, 396, 908,
		76, 588, 332, 844, 204, 716, 460, 972, 44, 556,
		300, 812, 172, 684, 428, 940, 108, 620, 364, 876,
		236, 748, 492, 1004, 28, 540, 284, 796, 156, 668,
		412, 924, 92, 604, 348, 860, 220, 732, 476, 988,
		60, 572, 316, 828, 188, 700, 444, 956, 124, 636,
		380, 892, 252, 764, 508, 1020, 2, 514, 258, 770,
		130, 642, 386, 898, 66, 578, 322, 834, 194, 706,
		450, 962, 34, 546, 290, 802, 162, 674, 418, 930,
		98, 610, 354, 866, 226, 738, 482, 994, 18, 530,
		274, 786, 146, 658, 402, 914, 82, 594, 338, 850,
		210, 722, 466, 978, 50, 562, 306, 818, 178, 690,
		434, 946, 114, 626, 370, 882, 242, 754, 498, 1010,
		10, 522, 266, 778, 138, 650, 394, 906, 74, 586,
		330, 842, 202, 714, 458, 970, 42, 554, 298, 810,
		170, 682, 426, 938, 106, 618, 362, 874, 234, 746,
		490, 1002, 26, 538, 282, 794, 154, 666, 410, 922,
		90, 602, 346, 858, 218, 730, 474, 986, 58, 570,
		314, 826, 186, 698, 442, 954, 122, 634, 378, 890,
		250, 762, 506, 1018, 6, 518, 262, 774, 134, 646,
		390, 902, 70, 582, 326, 838, 198, 710, 454, 966,
		38, 550, 294, 806, 166, 678, 422, 934, 102, 614,
		358, 870, 230, 742, 486, 998, 22, 534, 278, 790,
		150, 662, 406, 918, 86, 598, 342, 854, 214, 726,
		470, 982, 54, 566, 310, 822, 182, 694, 438, 950,
		118, 630, 374, 886, 246, 758, 502, 1014, 14, 526,
		270, 782, 142, 654, 398, 910, 78, 590, 334, 846,
		206, 718, 462, 974, 46, 558, 302, 814, 174, 686,
		430, 942, 110, 622, 366, 878, 238, 750, 494, 1006,
		30, 542, 286, 798, 158, 670, 414, 926, 94, 606,
		350, 862, 222, 734, 478, 990, 62, 574, 318, 830,
		190, 702, 446, 958, 126, 638, 382, 894, 254, 766,
		510, 1022, 1, 513, 257, 769, 129, 641, 385, 897,
		65, 577, 321, 833, 193, 705, 449, 961, 33, 545,
		289, 801, 161, 673, 417, 929, 97, 609, 353, 865,
		225, 737, 481, 993, 17, 529, 273, 785, 145, 657,
		401, 913, 81, 593, 337, 849, 209, 721, 465, 977,
		49, 561, 305, 817, 177, 689, 433, 945, 113, 625,
		369, 881, 241, 753, 497, 1009, 9, 521, 265, 777,
		137, 649, 393, 905, 73, 585, 329, 841, 201, 713,
		457, 969, 41, 553, 297, 809, 169, 681, 425, 937,
		105, 617, 361, 873, 233, 745, 489, 1001, 25, 537,
		281, 793, 153, 665, 409, 921, 89, 601, 345, 857,
		217, 729, 473, 985, 57, 569, 313, 825, 185, 697,
		441, 953, 121, 633, 377, 889, 249, 761, 505, 1017,
		5, 517, 261, 773, 133, 645, 389, 901, 69, 581,
		325, 837, 197, 709, 453, 965, 37, 549, 293, 805,
		165, 677, 421, 933, 101, 613, 357, 869, 229, 741,
		485, 997, 21, 533, 277, 789, 149, 661, 405, 917,
		85, 597, 341, 853, 213, 725, 469, 981, 53, 565,
		309, 821, 181, 693, 437, 949, 117, 629, 373, 885,
		245, 757, 501, 1013, 13, 525, 269, 781, 141, 653,
		397, 909, 77, 589, 333, 845, 205, 717, 461, 973,
		45, 557, 301, 813, 173, 685, 429, 941, 109, 621,
		365, 877, 237, 749, 493, 1005, 29, 541, 285, 797,
		157, 669, 413, 925, 93, 605, 349, 861, 221, 733,
		477, 989, 61, 573, 317, 829, 189, 701, 445, 957,
		125, 637, 381, 893, 253, 765, 509, 1021, 3, 515,
		259, 771, 131, 643, 387, 899, 67, 579, 323, 835,
		195, 707, 451, 963, 35, 547, 291, 803, 163, 675,
		419, 931, 99, 611, 355, 867, 227, 739, 483, 995,
		19, 531, 275, 787, 147, 659, 403, 915, 83, 595,
		339, 851, 211, 723, 467, 979, 51, 563, 307, 819,
		179, 691, 435, 947, 115, 627, 371, 883, 243, 755,
		499, 1011, 11, 523, 267, 779, 139, 651, 395, 907,
		75, 587, 331, 843, 203, 715, 459, 971, 43, 555,
		299, 811, 171, 683, 427, 939, 107, 619, 363, 875,
		235, 747, 491, 1003, 27, 539, 283, 795, 155, 667,
		411, 923, 91, 603, 347, 859, 219, 731, 475, 987,
		59, 571, 315, 827, 187, 699, 443, 955, 123, 635,
		379, 891, 251, 763, 507, 1019, 7, 519, 263, 775,
		135, 647, 391, 903, 71, 583, 327, 839, 199, 711,
		455, 967, 39, 551, 295, 807, 167, 679, 423, 935,
		103, 615, 359, 871, 231, 743, 487, 999, 23, 535,
		279, 791, 151, 663, 407, 919, 87, 599, 343, 855,
		215, 727, 471, 983, 55, 567, 311, 823, 183, 695,
		439, 951, 119, 631, 375, 887, 247, 759, 503, 1015,
		15, 527, 271, 783, 143, 655, 399, 911, 79, 591,
		335, 847, 207, 719, 463, 975, 47, 559, 303, 815,
		175, 687, 431, 943, 111, 623, 367, 879, 239, 751,
		495, 1007, 31, 543, 287, 799, 159, 671, 415, 927,
		95, 607, 351, 863, 223, 735, 479, 991, 63, 575,
		319, 831, 191, 703, 447, 959, 127, 639, 383, 895,
		255, 767, 511, 1023
	};

	internal ulong[] gauss_1024_12289 = new ulong[27]
	{
		1283868770400643928uL, 6416574995475331444uL, 4078260278032692663uL, 2353523259288686585uL, 1227179971273316331uL, 575931623374121527uL, 242543240509105209uL, 91437049221049666uL, 30799446349977173uL, 9255276791179340uL,
		2478152334826140uL, 590642893610164uL, 125206034929641uL, 23590435911403uL, 3948334035941uL, 586753615614uL, 77391054539uL, 9056793210uL, 940121950uL, 86539696uL,
		7062824uL, 510971uL, 32764uL, 1862uL, 94uL, 4uL, 0uL
	};

	internal int[] MAX_BL_SMALL = new int[11]
	{
		1, 1, 2, 2, 4, 7, 14, 27, 53, 106,
		209
	};

	internal int[] MAX_BL_LARGE = new int[10] { 2, 2, 5, 7, 12, 21, 40, 78, 157, 308 };

	internal int[] BITLENGTH_avg = new int[11]
	{
		4, 11, 24, 50, 102, 202, 401, 794, 1577, 3138,
		6308
	};

	internal int[] BITLENGTH_std = new int[11]
	{
		0, 1, 1, 1, 1, 2, 4, 5, 8, 13,
		25
	};

	internal const int DEPTH_INT_FG = 4;

	internal FalconKeygen()
	{
		fpre = new FprEngine();
		PRIMES = new FalconSmallPrimes().PRIMES;
		ffte = new FalconFFT(fpre);
		codec = new FalconCodec();
		vrfy = new FalconVrfy();
	}

	internal FalconKeygen(FalconCodec codec, FalconVrfy vrfy)
	{
		fpre = new FprEngine();
		PRIMES = new FalconSmallPrimes().PRIMES;
		ffte = new FalconFFT();
		this.codec = codec;
		this.vrfy = vrfy;
	}

	internal uint modp_set(int x, uint p)
	{
		return (uint)(x + (int)(p & (0L - (long)(uint)(x >>> 31))));
	}

	internal int modp_norm(uint x, uint p)
	{
		return (int)(x - (p & ((x - (p + 1 >> 1) >> 31) - 1)));
	}

	internal uint modp_ninv31(uint p)
	{
		uint num = 2 - p;
		num *= 2 - p * num;
		num *= 2 - p * num;
		num *= 2 - p * num;
		num *= 2 - p * num;
		return (uint)(0x7FFFFFFF & (0L - (long)num));
	}

	internal uint modp_R(uint p)
	{
		return 2147483648u - p;
	}

	internal uint modp_add(uint a, uint b, uint p)
	{
		uint num = a + b - p;
		return num + (uint)(int)(p & (0L - (long)(num >> 31)));
	}

	internal uint modp_sub(uint a, uint b, uint p)
	{
		uint num = a - b;
		return num + (uint)(int)(p & (0L - (long)(num >> 31)));
	}

	internal uint modp_montymul(uint a, uint b, uint p, uint p0i)
	{
		long num = (long)a * (long)b;
		ulong num2 = (ulong)(((num * p0i) & 0x7FFFFFFF) * p);
		uint num3 = (uint)(int)(num + (long)num2 >>> 31) - p;
		return num3 + (uint)(int)(p & (0L - (long)(num3 >> 31)));
	}

	internal uint modp_R2(uint p, uint p0i)
	{
		uint num = modp_R(p);
		num = modp_add(num, num, p);
		num = modp_montymul(num, num, p, p0i);
		num = modp_montymul(num, num, p, p0i);
		num = modp_montymul(num, num, p, p0i);
		num = modp_montymul(num, num, p, p0i);
		num = modp_montymul(num, num, p, p0i);
		return (uint)(num + (p & (0L - (long)(num & 1))) >> 1);
	}

	internal uint modp_Rx(uint x, uint p, uint p0i, uint R2)
	{
		x--;
		uint num = R2;
		uint num2 = modp_R(p);
		for (int i = 0; (uint)(1 << i) <= x; i++)
		{
			if ((x & (uint)(1 << i)) != 0)
			{
				num2 = modp_montymul(num2, num, p, p0i);
			}
			num = modp_montymul(num, num, p, p0i);
		}
		return num2;
	}

	internal uint modp_div(uint a, uint b, uint p, uint p0i, uint R)
	{
		uint num = p - 2;
		uint num2 = R;
		for (int num3 = 30; num3 >= 0; num3--)
		{
			num2 = modp_montymul(num2, num2, p, p0i);
			uint num4 = modp_montymul(num2, b, p, p0i);
			num2 ^= (uint)(int)((num2 ^ num4) & (0L - (long)((num >> num3) & 1)));
		}
		num2 = modp_montymul(num2, 1u, p, p0i);
		return modp_montymul(a, num2, p, p0i);
	}

	internal void modp_mkgm2(uint[] gmsrc, int gm, uint[] igmsrc, int igm, uint logn, uint g, uint p, uint p0i)
	{
		int num = 1 << (int)logn;
		uint num2 = modp_R2(p, p0i);
		g = modp_montymul(g, num2, p, p0i);
		uint num3;
		for (num3 = logn; num3 < 10; num3++)
		{
			g = modp_montymul(g, g, p, p0i);
		}
		uint b = modp_div(num2, g, p, p0i, modp_R(p));
		num3 = 10 - logn;
		uint num5;
		uint num4 = (num5 = modp_R(p));
		for (int i = 0; i < num; i++)
		{
			int num6 = REV10[i << (int)num3];
			gmsrc[gm + num6] = num4;
			igmsrc[igm + num6] = num5;
			num4 = modp_montymul(num4, g, p, p0i);
			num5 = modp_montymul(num5, b, p, p0i);
		}
	}

	internal void modp_NTT2_ext(uint[] asrc, int a, int stride, uint[] gmsrc, int gm, uint logn, uint p, uint p0i)
	{
		if (logn == 0)
		{
			return;
		}
		int num = 1 << (int)logn;
		int num2 = num;
		for (int num3 = 1; num3 < num; num3 <<= 1)
		{
			int num4 = num2 >> 1;
			int num5 = 0;
			int num6 = 0;
			while (num5 < num3)
			{
				uint b = gmsrc[gm + num3 + num5];
				int num7 = a + num6 * stride;
				int num8 = num7 + num4 * stride;
				int num9 = 0;
				while (num9 < num4)
				{
					uint a2 = asrc[num7];
					uint b2 = modp_montymul(asrc[num8], b, p, p0i);
					asrc[num7] = modp_add(a2, b2, p);
					asrc[num8] = modp_sub(a2, b2, p);
					num9++;
					num7 += stride;
					num8 += stride;
				}
				num5++;
				num6 += num2;
			}
			num2 = num4;
		}
	}

	internal void modp_iNTT2_ext(uint[] asrc, int a, int stride, uint[] igmsrc, int igm, uint logn, uint p, uint p0i)
	{
		if (logn == 0)
		{
			return;
		}
		int num = 1 << (int)logn;
		int num2 = 1;
		for (int num3 = num; num3 > 1; num3 >>= 1)
		{
			int num4 = num3 >> 1;
			int num5 = num2 << 1;
			int num6 = 0;
			int num7 = 0;
			while (num6 < num4)
			{
				uint b = igmsrc[igm + num4 + num6];
				int num8 = a + num7 * stride;
				int num9 = num8 + num2 * stride;
				int num10 = 0;
				while (num10 < num2)
				{
					uint a2 = asrc[num8];
					uint b2 = asrc[num9];
					asrc[num8] = modp_add(a2, b2, p);
					asrc[num9] = modp_montymul(modp_sub(a2, b2, p), b, p, p0i);
					num10++;
					num8 += stride;
					num9 += stride;
				}
				num6++;
				num7 += num5;
			}
			num2 = num5;
		}
		uint b3 = (uint)(1 << (int)(31 - logn));
		int num11 = 0;
		int num12 = a;
		while (num11 < num)
		{
			asrc[num12] = modp_montymul(asrc[num12], b3, p, p0i);
			num11++;
			num12 += stride;
		}
	}

	internal void modp_NTT2(uint[] asrc, int a, uint[] gmsrc, int gm, uint logn, uint p, uint p0i)
	{
		modp_NTT2_ext(asrc, a, 1, gmsrc, gm, logn, p, p0i);
	}

	internal void modp_iNTT2(uint[] asrc, int a, uint[] igmsrc, int igm, uint logn, uint p, uint p0i)
	{
		modp_iNTT2_ext(asrc, a, 1, igmsrc, igm, logn, p, p0i);
	}

	internal void modp_poly_rec_res(uint[] fsrc, int f, uint logn, uint p, uint p0i, uint R2)
	{
		int num = 1 << (int)(logn - 1);
		for (int i = 0; i < num; i++)
		{
			uint a = fsrc[f + (i << 1)];
			uint b = fsrc[f + (i << 1) + 1];
			fsrc[f + i] = modp_montymul(modp_montymul(a, b, p, p0i), R2, p, p0i);
		}
	}

	internal uint zint_sub(uint[] asrc, int a, uint[] bsrc, int b, int len, uint ctl)
	{
		uint num = 0u;
		uint num2 = (uint)(0uL - (ulong)ctl);
		for (int i = 0; i < len; i++)
		{
			uint num3 = asrc[a + i];
			uint num4 = num3 - bsrc[b + i] - num;
			num = num4 >> 31;
			num3 ^= ((num4 & 0x7FFFFFFF) ^ num3) & num2;
			asrc[a + i] = num3;
		}
		return num;
	}

	internal uint zint_mul_small(uint[] msrc, int m, int mlen, uint x)
	{
		uint num = 0u;
		for (int i = 0; i < mlen; i++)
		{
			ulong num2 = (ulong)((long)msrc[m + i] * (long)x + num);
			msrc[m + i] = (uint)((int)num2 & 0x7FFFFFFF);
			num = (uint)(num2 >> 31);
		}
		return num;
	}

	internal uint zint_mod_small_uint(uint[] dsrc, int d, int dlen, uint p, uint p0i, uint R2)
	{
		uint num = 0u;
		int num2 = dlen;
		while (num2-- > 0)
		{
			num = modp_montymul(num, R2, p, p0i);
			uint num3 = dsrc[d + num2] - p;
			num3 += (uint)(int)(p & (0L - (long)(num3 >> 31)));
			num = modp_add(num, num3, p);
		}
		return num;
	}

	internal uint zint_mod_small_signed(uint[] dsrc, int d, int dlen, uint p, uint p0i, uint R2, uint Rx)
	{
		if (dlen == 0)
		{
			return 0u;
		}
		uint a = zint_mod_small_uint(dsrc, d, dlen, p, p0i, R2);
		return modp_sub(a, (uint)(Rx & (0L - (long)(dsrc[d + dlen - 1] >> 30))), p);
	}

	internal void zint_add_mul_small(uint[] xsrc, int x, uint[] ysrc, int y, int len, uint s)
	{
		uint num = 0u;
		for (int i = 0; i < len; i++)
		{
			uint num2 = xsrc[x + i];
			ulong num3 = (ulong)((long)ysrc[y + i] * (long)s + num2 + num);
			xsrc[x + i] = (uint)((int)num3 & 0x7FFFFFFF);
			num = (uint)(num3 >> 31);
		}
		xsrc[x + len] = num;
	}

	internal void zint_norm_zero(uint[] xsrc, int x, uint[] psrc, int p, int len)
	{
		uint num = 0u;
		uint num2 = 0u;
		int num3 = len;
		while (num3-- > 0)
		{
			uint num4 = xsrc[x + num3];
			uint num5 = (psrc[p + num3] >> 1) | (num2 << 30);
			num2 = psrc[p + num3] & 1;
			uint num6 = num5 - num4;
			num6 = ((uint)(0L - (long)num6) >> 31) | (uint)(int)(0L - (long)(num6 >> 31));
			num |= num6 & ((num & 1) - 1);
		}
		zint_sub(xsrc, x, psrc, p, len, num >> 31);
	}

	internal void zint_rebuild_CRT(uint[] xxsrc, int xx, int xlen, int xstride, int num, FalconSmallPrime[] primes, int normalize_signed, uint[] tmpsrc, int tmp)
	{
		tmpsrc[tmp] = primes[0].p;
		for (int i = 1; i < xlen; i++)
		{
			uint p = primes[i].p;
			uint s = primes[i].s;
			uint p0i = modp_ninv31(p);
			uint r = modp_R2(p, p0i);
			int num2 = 0;
			int num3 = xx;
			while (num2 < num)
			{
				uint a = xxsrc[num3 + i];
				uint b = zint_mod_small_uint(xxsrc, num3, i, p, p0i, r);
				uint s2 = modp_montymul(s, modp_sub(a, b, p), p, p0i);
				zint_add_mul_small(xxsrc, num3, tmpsrc, tmp, i, s2);
				num2++;
				num3 += xstride;
			}
			tmpsrc[tmp + i] = zint_mul_small(tmpsrc, tmp, i, p);
		}
		if (normalize_signed != 0)
		{
			int i = 0;
			int num3 = xx;
			while (i < num)
			{
				zint_norm_zero(xxsrc, num3, tmpsrc, tmp, xlen);
				i++;
				num3 += xstride;
			}
		}
	}

	internal void zint_negate(uint[] asrc, int a, int len, uint ctl)
	{
		uint num = ctl;
		uint num2 = (uint)(0L - (long)ctl) >> 1;
		for (int i = 0; i < len; i++)
		{
			uint num3 = asrc[a + i];
			num3 = (num3 ^ num2) + num;
			asrc[a + i] = num3 & 0x7FFFFFFF;
			num = num3 >> 31;
		}
	}

	internal uint zint_co_reduce(uint[] asrc, int a, uint[] bsrc, int b, int len, long xa, long xb, long ya, long yb)
	{
		long num = 0L;
		long num2 = 0L;
		for (int i = 0; i < len; i++)
		{
			uint num3 = asrc[a + i];
			uint num4 = bsrc[b + i];
			ulong num5 = (ulong)(num3 * xa + num4 * xb + num);
			ulong num6 = (ulong)(num3 * ya + num4 * yb + num2);
			if (i > 0)
			{
				asrc[a + i - 1] = (uint)((int)num5 & 0x7FFFFFFF);
				bsrc[b + i - 1] = (uint)((int)num6 & 0x7FFFFFFF);
			}
			num = (long)num5 >> 31;
			num2 = (long)num6 >> 31;
		}
		asrc[a + len - 1] = (uint)num;
		bsrc[b + len - 1] = (uint)num2;
		uint num7 = (uint)((ulong)num >> 63);
		uint num8 = (uint)((ulong)num2 >> 63);
		zint_negate(asrc, a, len, num7);
		zint_negate(bsrc, b, len, num8);
		return num7 | (num8 << 1);
	}

	internal void zint_finish_mod(uint[] asrc, int a, int len, uint[] msrc, int m, uint neg)
	{
		uint num = 0u;
		for (int i = 0; i < len; i++)
		{
			num = asrc[a + i] - msrc[m + i] - num >> 31;
		}
		uint num2 = (uint)(0L - (long)neg) >> 1;
		uint num3 = (uint)(0uL - (ulong)(neg | (1 - num)));
		num = neg;
		for (int i = 0; i < len; i++)
		{
			uint num4 = asrc[a + i];
			uint num5 = (msrc[m + i] ^ num2) & num3;
			num4 = num4 - num5 - num;
			asrc[a + i] = num4 & 0x7FFFFFFF;
			num = num4 >> 31;
		}
	}

	internal void zint_co_reduce_mod(uint[] asrc, int a, uint[] bsrc, int b, uint[] msrc, int m, int len, uint m0i, long xa, long xb, long ya, long yb)
	{
		long num = 0L;
		long num2 = 0L;
		uint num3 = (uint)((((int)asrc[a] * (int)xa + (int)bsrc[b] * (int)xb) * (int)m0i) & 0x7FFFFFFF);
		uint num4 = (uint)((((int)asrc[a] * (int)ya + (int)bsrc[b] * (int)yb) * (int)m0i) & 0x7FFFFFFF);
		for (int i = 0; i < len; i++)
		{
			uint num5 = asrc[a + i];
			uint num6 = bsrc[b + i];
			ulong num7 = (ulong)(num5 * xa + num6 * xb + (long)msrc[m + i] * (long)num3 + num);
			ulong num8 = (ulong)(num5 * ya + num6 * yb + (long)msrc[m + i] * (long)num4 + num2);
			if (i > 0)
			{
				asrc[a + i - 1] = (uint)((int)num7 & 0x7FFFFFFF);
				bsrc[b + i - 1] = (uint)((int)num8 & 0x7FFFFFFF);
			}
			num = (long)num7 >> 31;
			num2 = (long)num8 >> 31;
		}
		asrc[a + len - 1] = (uint)num;
		bsrc[b + len - 1] = (uint)num2;
		zint_finish_mod(asrc, a, len, msrc, m, (uint)((ulong)num >> 63));
		zint_finish_mod(bsrc, b, len, msrc, m, (uint)((ulong)num2 >> 63));
	}

	internal int zint_bezout(uint[] usrc, int u, uint[] vsrc, int v, uint[] xsrc, int x, uint[] ysrc, int y, int len, uint[] tmpsrc, int tmp)
	{
		if (len == 0)
		{
			return 0;
		}
		int num = tmp + len;
		int num2 = num + len;
		int num3 = num2 + len;
		uint m0i = modp_ninv31(xsrc[x]);
		uint m0i2 = modp_ninv31(ysrc[y]);
		Array.Copy(xsrc, x, tmpsrc, num2, len);
		Array.Copy(ysrc, y, tmpsrc, num3, len);
		usrc[u] = 1u;
		for (int i = 1; i < len; i++)
		{
			usrc[u + i] = 0u;
			vsrc[v + i] = 0u;
		}
		vsrc[v] = 0u;
		Array.Copy(ysrc, y, tmpsrc, tmp, len);
		Array.Copy(xsrc, x, tmpsrc, num, len);
		tmpsrc[num]--;
		for (uint num4 = (uint)(62 * len + 30); num4 >= 30; num4 -= 30)
		{
			uint num5 = uint.MaxValue;
			uint num6 = uint.MaxValue;
			uint num7 = 0u;
			uint num8 = 0u;
			uint num9 = 0u;
			uint num10 = 0u;
			int num11 = len;
			while (num11-- > 0)
			{
				uint num12 = tmpsrc[num2 + num11];
				uint num13 = tmpsrc[num3 + num11];
				num7 ^= (num7 ^ num12) & num5;
				num8 ^= (num8 ^ num12) & num6;
				num9 ^= (num9 ^ num13) & num5;
				num10 ^= (num10 ^ num13) & num6;
				num6 = num5;
				num5 &= ((num12 | num13) + int.MaxValue >> 31) - 1;
			}
			num8 |= num7 & num6;
			num7 &= ~num6;
			num10 |= num9 & num6;
			num9 &= ~num6;
			ulong num14 = ((ulong)num7 << 31) + num8;
			ulong num15 = ((ulong)num9 << 31) + num10;
			uint num16 = tmpsrc[num2];
			uint num17 = tmpsrc[num3];
			long num18 = 1L;
			long num19 = 0L;
			long num20 = 0L;
			long num21 = 1L;
			for (int j = 0; j < 31; j++)
			{
				ulong num22 = num15 - num14;
				uint num23 = (uint)((num22 ^ ((num14 ^ num15) & (num14 ^ num22))) >> 63);
				uint num24 = (num16 >> j) & 1;
				uint num25 = (num17 >> j) & 1;
				uint num26 = num24 & num25 & num23;
				uint num27 = (uint)(num24 & num25 & (int)(~num23));
				uint num28 = num26 | (num24 ^ 1);
				num16 -= (uint)(int)(num17 & (0L - (long)num26));
				num14 -= num15 & (ulong)(0L - (long)num26);
				num18 -= num20 & (0L - (long)num26);
				num19 -= num21 & (0L - (long)num26);
				num17 -= (uint)(int)(num16 & (0L - (long)num27));
				num15 -= num14 & (ulong)(0L - (long)num27);
				num20 -= num18 & (0L - (long)num27);
				num21 -= num19 & (0L - (long)num27);
				num16 += num16 & (num28 - 1);
				num18 += num18 & ((long)num28 - 1L);
				num19 += num19 & ((long)num28 - 1L);
				num14 ^= (num14 ^ (num14 >> 1)) & (ulong)(0L - (long)num28);
				num17 += (uint)(int)(num17 & (0L - (long)num28));
				num20 += num20 & (0L - (long)num28);
				num21 += num21 & (0L - (long)num28);
				num15 ^= (num15 ^ (num15 >> 1)) & (ulong)((long)num28 - 1L);
			}
			uint num29 = zint_co_reduce(tmpsrc, num2, tmpsrc, num3, len, num18, num19, num20, num21);
			num18 -= (num18 + num18) & (0L - (long)(num29 & 1));
			num19 -= (num19 + num19) & (0L - (long)(num29 & 1));
			num20 -= (num20 + num20) & (0L - (long)(num29 >> 1));
			num21 -= (num21 + num21) & (0L - (long)(num29 >> 1));
			zint_co_reduce_mod(usrc, u, tmpsrc, tmp, ysrc, y, len, m0i2, num18, num19, num20, num21);
			zint_co_reduce_mod(vsrc, v, tmpsrc, num, xsrc, x, len, m0i, num18, num19, num20, num21);
		}
		uint num30 = tmpsrc[num2] ^ 1;
		for (int num11 = 1; num11 < len; num11++)
		{
			num30 |= tmpsrc[num2 + num11];
		}
		return (int)((1 - ((num30 | (0L - (long)num30)) >> 31)) & xsrc[x] & ysrc[y]);
	}

	internal void zint_add_scaled_mul_small(uint[] xsrc, int x, int xlen, uint[] ysrc, int y, int ylen, int k, uint sch, uint scl)
	{
		if (ylen != 0)
		{
			uint num = (uint)(0L - (long)(ysrc[y + ylen - 1] >> 30)) >> 1;
			uint num2 = 0u;
			int num3 = 0;
			for (int i = (int)sch; i < xlen; i++)
			{
				int num4 = i - (int)sch;
				uint num5 = ((num4 < ylen) ? ysrc[y + num4] : num);
				uint num6 = ((num5 << (int)scl) & 0x7FFFFFFF) | num2;
				num2 = num5 >> (int)(31 - scl);
				ulong num7 = (ulong)(num6 * k + xsrc[x + i] + num3);
				xsrc[x + i] = (uint)((int)num7 & 0x7FFFFFFF);
				num3 = (int)(num7 >> 31);
			}
		}
	}

	internal void zint_sub_scaled(uint[] xsrc, int x, int xlen, uint[] ysrc, int y, int ylen, uint sch, uint scl)
	{
		if (ylen != 0)
		{
			uint num = (uint)(0L - (long)(ysrc[y + ylen - 1] >> 30) >> 1);
			uint num2 = 0u;
			uint num3 = 0u;
			for (int i = (int)sch; i < xlen; i++)
			{
				int num4 = i - (int)sch;
				uint num5 = ((num4 < ylen) ? ysrc[y + num4] : num);
				uint num6 = ((num5 << (int)scl) & 0x7FFFFFFF) | num2;
				num2 = num5 >> (int)(31 - scl);
				uint num7 = xsrc[x + i] - num6 - num3;
				xsrc[x + i] = num7 & 0x7FFFFFFF;
				num3 = num7 >> 31;
			}
		}
	}

	internal int zint_one_to_plain(uint[] xsrc, int x)
	{
		uint num = xsrc[x];
		return (int)(num | ((num & 0x40000000) << 1));
	}

	internal void poly_big_to_fp(FalconFPR[] dsrc, int d, uint[] fsrc, int f, int flen, int fstride, uint logn)
	{
		int num = 1 << (int)logn;
		int i;
		if (flen == 0)
		{
			for (i = 0; i < num; i++)
			{
				dsrc[d + i] = fpre.fpr_zero;
			}
			return;
		}
		i = 0;
		while (i < num)
		{
			uint num2 = (uint)(0uL - (ulong)(fsrc[f + flen - 1] >> 30));
			uint num3 = num2 >> 1;
			uint num4 = num2 & 1;
			FalconFPR falconFPR = fpre.fpr_zero;
			FalconFPR falconFPR2 = fpre.fpr_one;
			int num5 = 0;
			while (num5 < flen)
			{
				uint num6 = (fsrc[f + num5] ^ num3) + num4;
				num4 = num6 >> 31;
				num6 &= 0x7FFFFFFF;
				num6 -= (num6 << 1) & num2;
				falconFPR = fpre.fpr_add(falconFPR, fpre.fpr_mul(fpre.fpr_of((int)num6), falconFPR2));
				num5++;
				falconFPR2 = fpre.fpr_mul(falconFPR2, fpre.fpr_ptwo31);
			}
			dsrc[d + i] = falconFPR;
			i++;
			f += fstride;
		}
	}

	internal int poly_big_to_small(sbyte[] dsrc, int d, uint[] ssrc, int s, int lim, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = 0; i < num; i++)
		{
			int num2 = zint_one_to_plain(ssrc, s + i);
			if (num2 < -lim || num2 > lim)
			{
				return 0;
			}
			dsrc[d + i] = (sbyte)num2;
		}
		return 1;
	}

	internal void poly_sub_scaled(uint[] Fsrc, int F, int Flen, int Fstride, uint[] fsrc, int f, int flen, int fstride, int[] ksrc, int k, uint sch, uint scl, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = 0; i < num; i++)
		{
			int num2 = -ksrc[k + i];
			int num3 = F + i * Fstride;
			int num4 = f;
			for (int j = 0; j < num; j++)
			{
				zint_add_scaled_mul_small(Fsrc, num3, Flen, fsrc, num4, flen, num2, sch, scl);
				if (i + j == num - 1)
				{
					num3 = F;
					num2 = -num2;
				}
				else
				{
					num3 += Fstride;
				}
				num4 += fstride;
			}
		}
	}

	internal void poly_sub_scaled_ntt(uint[] Fsrc, int F, int Flen, int Fstride, uint[] fsrc, int f, int flen, int fstride, int[] ksrc, int k, uint sch, uint scl, uint logn, uint[] tmpsrc, int tmp)
	{
		int num = 1 << (int)logn;
		int num2 = flen + 1;
		int num3 = tmp + num;
		int num4 = num3 + num;
		int num5 = num4 + num * num2;
		FalconSmallPrime[] pRIMES = PRIMES;
		int i;
		int num8;
		int num7;
		for (i = 0; i < num2; i++)
		{
			uint p = pRIMES[i].p;
			uint p0i = modp_ninv31(p);
			uint num6 = modp_R2(p, p0i);
			uint rx = modp_Rx((uint)flen, p, p0i, num6);
			modp_mkgm2(tmpsrc, tmp, tmpsrc, num3, logn, pRIMES[i].g, p, p0i);
			int j;
			for (j = 0; j < num; j++)
			{
				tmpsrc[num5 + j] = modp_set(ksrc[k + j], p);
			}
			modp_NTT2(tmpsrc, num5, tmpsrc, tmp, logn, p, p0i);
			j = 0;
			num7 = f;
			num8 = num4 + i;
			while (j < num)
			{
				tmpsrc[num8] = zint_mod_small_signed(tmpsrc, num7, flen, p, p0i, num6, rx);
				j++;
				num7 += fstride;
				num8 += num2;
			}
			modp_NTT2_ext(tmpsrc, num4 + i, num2, tmpsrc, tmp, logn, p, p0i);
			j = 0;
			num8 = num4 + i;
			while (j < num)
			{
				tmpsrc[num8] = modp_montymul(modp_montymul(tmpsrc[num5 + j], tmpsrc[num8], p, p0i), num6, p, p0i);
				j++;
				num8 += num2;
			}
			modp_iNTT2_ext(tmpsrc, num4 + i, num2, tmpsrc, num3, logn, p, p0i);
		}
		zint_rebuild_CRT(tmpsrc, num4, num2, num2, num, pRIMES, 1, tmpsrc, num5);
		i = 0;
		num8 = F;
		num7 = num4;
		while (i < num)
		{
			zint_sub_scaled(tmpsrc, num8, Flen, tmpsrc, num7, num2, sch, scl);
			i++;
			num8 += Fstride;
			num7 += num2;
		}
	}

	internal ulong get_rng_u64(SHAKE256 rng)
	{
		byte[] array = new byte[8];
		rng.i_shake256_extract(array, 0, 8);
		return array[0] | ((ulong)array[1] << 8) | ((ulong)array[2] << 16) | ((ulong)array[3] << 24) | ((ulong)array[4] << 32) | ((ulong)array[5] << 40) | ((ulong)array[6] << 48) | ((ulong)array[7] << 56);
	}

	internal int mkgauss(SHAKE256 rng, uint logn)
	{
		uint num = (uint)(1 << (int)(10 - logn));
		int num2 = 0;
		for (uint num3 = 0u; num3 < num; num3++)
		{
			ulong num4 = get_rng_u64(rng);
			uint num5 = (uint)(num4 >> 63);
			num4 &= 0x7FFFFFFFFFFFFFFFL;
			uint num6 = (uint)(num4 - gauss_1024_12289[0] >> 63);
			uint num7 = 0u;
			num4 = get_rng_u64(rng);
			num4 &= 0x7FFFFFFFFFFFFFFFL;
			for (uint num8 = 1u; num8 < gauss_1024_12289.Length; num8++)
			{
				uint num9 = (uint)((int)(num4 - gauss_1024_12289[num8] >> 63) ^ 1);
				num7 |= (uint)(int)(num8 & (0L - (long)(num9 & (num6 ^ 1))));
				num6 |= num9;
			}
			num7 = (uint)((num7 ^ (0L - (long)num5)) + num5);
			num2 += (int)num7;
		}
		return num2;
	}

	internal uint poly_small_sqnorm(sbyte[] fsrc, int f, uint logn)
	{
		int num = 1 << (int)logn;
		uint num2 = 0u;
		uint num3 = 0u;
		for (int i = 0; i < num; i++)
		{
			int num4 = fsrc[f + i];
			num2 += (uint)(num4 * num4);
			num3 |= num2;
		}
		return (uint)(num2 | (0L - (long)(num3 >> 31)));
	}

	internal void poly_small_to_fp(FalconFPR[] xsrc, int x, sbyte[] fsrc, int f, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = 0; i < num; i++)
		{
			xsrc[x + i] = fpre.fpr_of(fsrc[f + i]);
		}
	}

	internal void make_fg_step(uint[] datasrc, int data, uint logn, uint depth, int in_ntt, int out_ntt)
	{
		int num = 1 << (int)logn;
		int num2 = num >> 1;
		int num3 = MAX_BL_SMALL[depth];
		int num4 = MAX_BL_SMALL[depth + 1];
		FalconSmallPrime[] pRIMES = PRIMES;
		int num5 = data + num2 * num4;
		int num6 = num5 + num2 * num4;
		int num7 = num6 + num * num3;
		int num8 = num7 + num * num3;
		int num9 = num8 + num;
		int num10 = num9 + num;
		Array.Copy(datasrc, data, datasrc, num6, 2 * num * num3);
		for (int i = 0; i < num3; i++)
		{
			uint p = pRIMES[i].p;
			uint p0i = modp_ninv31(p);
			uint b = modp_R2(p, p0i);
			modp_mkgm2(datasrc, num8, datasrc, num9, logn, pRIMES[i].g, p, p0i);
			int num11 = 0;
			int num12 = num6 + i;
			while (num11 < num)
			{
				datasrc[num10 + num11] = datasrc[num12];
				num11++;
				num12 += num3;
			}
			if (in_ntt == 0)
			{
				modp_NTT2(datasrc, num10, datasrc, num8, logn, p, p0i);
			}
			num11 = 0;
			num12 = data + i;
			while (num11 < num2)
			{
				uint a = datasrc[num10 + (num11 << 1)];
				uint b2 = datasrc[num10 + (num11 << 1) + 1];
				datasrc[num12] = modp_montymul(modp_montymul(a, b2, p, p0i), b, p, p0i);
				num11++;
				num12 += num4;
			}
			if (in_ntt != 0)
			{
				modp_iNTT2_ext(datasrc, num6 + i, num3, datasrc, num9, logn, p, p0i);
			}
			num11 = 0;
			num12 = num7 + i;
			while (num11 < num)
			{
				datasrc[num10 + num11] = datasrc[num12];
				num11++;
				num12 += num3;
			}
			if (in_ntt == 0)
			{
				modp_NTT2(datasrc, num10, datasrc, num8, logn, p, p0i);
			}
			num11 = 0;
			num12 = num5 + i;
			while (num11 < num2)
			{
				uint a2 = datasrc[num10 + (num11 << 1)];
				uint b3 = datasrc[num10 + (num11 << 1) + 1];
				datasrc[num12] = modp_montymul(modp_montymul(a2, b3, p, p0i), b, p, p0i);
				num11++;
				num12 += num4;
			}
			if (in_ntt != 0)
			{
				modp_iNTT2_ext(datasrc, num7 + i, num3, datasrc, num9, logn, p, p0i);
			}
			if (out_ntt == 0)
			{
				modp_iNTT2_ext(datasrc, data + i, num4, datasrc, num9, logn - 1, p, p0i);
				modp_iNTT2_ext(datasrc, num5 + i, num4, datasrc, num9, logn - 1, p, p0i);
			}
		}
		zint_rebuild_CRT(datasrc, num6, num3, num3, num, pRIMES, 1, datasrc, num8);
		zint_rebuild_CRT(datasrc, num7, num3, num3, num, pRIMES, 1, datasrc, num8);
		for (int i = num3; i < num4; i++)
		{
			uint p2 = pRIMES[i].p;
			uint p0i2 = modp_ninv31(p2);
			uint num13 = modp_R2(p2, p0i2);
			uint rx = modp_Rx((uint)num3, p2, p0i2, num13);
			modp_mkgm2(datasrc, num8, datasrc, num9, logn, pRIMES[i].g, p2, p0i2);
			int num14 = 0;
			int num15 = num6;
			while (num14 < num)
			{
				datasrc[num10 + num14] = zint_mod_small_signed(datasrc, num15, num3, p2, p0i2, num13, rx);
				num14++;
				num15 += num3;
			}
			modp_NTT2(datasrc, num10, datasrc, num8, logn, p2, p0i2);
			num14 = 0;
			num15 = data + i;
			while (num14 < num2)
			{
				uint a3 = datasrc[num10 + (num14 << 1)];
				uint b4 = datasrc[num10 + (num14 << 1) + 1];
				datasrc[num15] = modp_montymul(modp_montymul(a3, b4, p2, p0i2), num13, p2, p0i2);
				num14++;
				num15 += num4;
			}
			num14 = 0;
			num15 = num7;
			while (num14 < num)
			{
				datasrc[num10 + num14] = zint_mod_small_signed(datasrc, num15, num3, p2, p0i2, num13, rx);
				num14++;
				num15 += num3;
			}
			modp_NTT2(datasrc, num10, datasrc, num8, logn, p2, p0i2);
			num14 = 0;
			num15 = num5 + i;
			while (num14 < num2)
			{
				uint a4 = datasrc[num10 + (num14 << 1)];
				uint b5 = datasrc[num10 + (num14 << 1) + 1];
				datasrc[num15] = modp_montymul(modp_montymul(a4, b5, p2, p0i2), num13, p2, p0i2);
				num14++;
				num15 += num4;
			}
			if (out_ntt == 0)
			{
				modp_iNTT2_ext(datasrc, data + i, num4, datasrc, num9, logn - 1, p2, p0i2);
				modp_iNTT2_ext(datasrc, num5 + i, num4, datasrc, num9, logn - 1, p2, p0i2);
			}
		}
	}

	internal void make_fg(uint[] datasrc, int data, sbyte[] fsrc, int f, sbyte[] gsrc, int g, uint logn, uint depth, int out_ntt)
	{
		int num = 1 << (int)logn;
		int num2 = data + num;
		FalconSmallPrime[] pRIMES = PRIMES;
		uint p = pRIMES[0].p;
		for (int i = 0; i < num; i++)
		{
			datasrc[data + i] = modp_set(fsrc[f + i], p);
			datasrc[num2 + i] = modp_set(gsrc[g + i], p);
		}
		if (depth == 0 && out_ntt != 0)
		{
			uint p2 = pRIMES[0].p;
			uint p0i = modp_ninv31(p2);
			int num3 = num2 + num;
			int igm = num3 + num;
			modp_mkgm2(datasrc, num3, datasrc, igm, logn, pRIMES[0].g, p2, p0i);
			modp_NTT2(datasrc, data, datasrc, num3, logn, p2, p0i);
			modp_NTT2(datasrc, num2, datasrc, num3, logn, p2, p0i);
		}
		else
		{
			for (uint num4 = 0u; num4 < depth; num4++)
			{
				make_fg_step(datasrc, data, logn - num4, num4, (num4 != 0) ? 1 : 0, (num4 + 1 < depth || out_ntt != 0) ? 1 : 0);
			}
		}
	}

	internal int solve_NTRU_deepest(uint logn_top, sbyte[] fsrc, int f, sbyte[] gsrc, int g, uint[] tmpsrc, int tmp)
	{
		int num = MAX_BL_SMALL[logn_top];
		FalconSmallPrime[] pRIMES = PRIMES;
		int num2 = tmp + num;
		int num3 = num2 + num;
		int num4 = num3 + num;
		int tmp2 = num4 + num;
		make_fg(tmpsrc, num3, fsrc, f, gsrc, g, logn_top, logn_top, 0);
		zint_rebuild_CRT(tmpsrc, num3, num, num, 2, pRIMES, 0, tmpsrc, tmp2);
		if (zint_bezout(tmpsrc, num2, tmpsrc, tmp, tmpsrc, num3, tmpsrc, num4, num, tmpsrc, tmp2) == 0)
		{
			return 0;
		}
		uint x = 12289u;
		if (zint_mul_small(tmpsrc, tmp, num, x) != 0 || zint_mul_small(tmpsrc, num2, num, x) != 0)
		{
			return 0;
		}
		return 1;
	}

	internal int solve_NTRU_intermediate(uint logn_top, sbyte[] fsrc, int f, sbyte[] gsrc, int g, uint depth, uint[] tmpsrc, int tmp)
	{
		uint num = logn_top - depth;
		int num2 = 1 << (int)num;
		int num3 = num2 >> 1;
		int num4 = MAX_BL_SMALL[depth];
		int num5 = MAX_BL_SMALL[depth + 1];
		int num6 = MAX_BL_LARGE[depth];
		FalconSmallPrime[] pRIMES = PRIMES;
		int num7 = tmp;
		int num8 = num7 + num5 * num3;
		int num9 = num8 + num5 * num3;
		make_fg(tmpsrc, num9, fsrc, f, gsrc, g, logn_top, depth, 1);
		int num10 = tmp;
		int num11 = num10 + num2 * num6;
		int num12 = num11 + num2 * num6;
		Array.Copy(tmpsrc, num9, tmpsrc, num12, 2 * num2 * num4);
		num9 = num12;
		int num13 = num9 + num4 * num2;
		num12 = num13 + num4 * num2;
		Array.Copy(tmpsrc, num7, tmpsrc, num12, 2 * num3 * num5);
		num7 = num12;
		num8 = num7 + num3 * num5;
		int i;
		for (i = 0; i < num6; i++)
		{
			uint p = pRIMES[i].p;
			uint p0i = modp_ninv31(p);
			uint r = modp_R2(p, p0i);
			uint rx = modp_Rx((uint)num5, p, p0i, r);
			int num14 = 0;
			int num15 = num7;
			int num16 = num8;
			int num17 = num10 + i;
			int num18 = num11 + i;
			while (num14 < num3)
			{
				tmpsrc[num17] = zint_mod_small_signed(tmpsrc, num15, num5, p, p0i, r, rx);
				tmpsrc[num18] = zint_mod_small_signed(tmpsrc, num16, num5, p, p0i, r, rx);
				num14++;
				num15 += num5;
				num16 += num5;
				num17 += num6;
				num18 += num6;
			}
		}
		int num25;
		int num26;
		for (i = 0; i < num6; i++)
		{
			uint p2 = pRIMES[i].p;
			uint p0i2 = modp_ninv31(p2);
			uint num19 = modp_R2(p2, p0i2);
			if (i == num4)
			{
				zint_rebuild_CRT(tmpsrc, num9, num4, num4, num2, pRIMES, 1, tmpsrc, num12);
				zint_rebuild_CRT(tmpsrc, num13, num4, num4, num2, pRIMES, 1, tmpsrc, num12);
			}
			int num20 = num12;
			int num21 = num20 + num2;
			int num22 = num21 + num2;
			int num23 = num22 + num2;
			modp_mkgm2(tmpsrc, num20, tmpsrc, num21, num, pRIMES[i].g, p2, p0i2);
			int num24;
			if (i < num4)
			{
				num24 = 0;
				num25 = num9 + i;
				num26 = num13 + i;
				while (num24 < num2)
				{
					tmpsrc[num22 + num24] = tmpsrc[num25];
					tmpsrc[num23 + num24] = tmpsrc[num26];
					num24++;
					num25 += num4;
					num26 += num4;
				}
				modp_iNTT2_ext(tmpsrc, num9 + i, num4, tmpsrc, num21, num, p2, p0i2);
				modp_iNTT2_ext(tmpsrc, num13 + i, num4, tmpsrc, num21, num, p2, p0i2);
			}
			else
			{
				uint rx2 = modp_Rx((uint)num4, p2, p0i2, num19);
				num24 = 0;
				num25 = num9;
				num26 = num13;
				while (num24 < num2)
				{
					tmpsrc[num22 + num24] = zint_mod_small_signed(tmpsrc, num25, num4, p2, p0i2, num19, rx2);
					tmpsrc[num23 + num24] = zint_mod_small_signed(tmpsrc, num26, num4, p2, p0i2, num19, rx2);
					num24++;
					num25 += num4;
					num26 += num4;
				}
				modp_NTT2(tmpsrc, num22, tmpsrc, num20, num, p2, p0i2);
				modp_NTT2(tmpsrc, num23, tmpsrc, num20, num, p2, p0i2);
			}
			int num27 = num23 + num2;
			int num28 = num27 + num3;
			num24 = 0;
			num25 = num10 + i;
			num26 = num11 + i;
			while (num24 < num3)
			{
				tmpsrc[num27 + num24] = tmpsrc[num25];
				tmpsrc[num28 + num24] = tmpsrc[num26];
				num24++;
				num25 += num6;
				num26 += num6;
			}
			modp_NTT2(tmpsrc, num27, tmpsrc, num20, num - 1, p2, p0i2);
			modp_NTT2(tmpsrc, num28, tmpsrc, num20, num - 1, p2, p0i2);
			num24 = 0;
			num25 = num10 + i;
			num26 = num11 + i;
			while (num24 < num3)
			{
				uint a = tmpsrc[num22 + (num24 << 1)];
				uint a2 = tmpsrc[num22 + (num24 << 1) + 1];
				uint a3 = tmpsrc[num23 + (num24 << 1)];
				uint a4 = tmpsrc[num23 + (num24 << 1) + 1];
				uint b = modp_montymul(tmpsrc[num27 + num24], num19, p2, p0i2);
				uint b2 = modp_montymul(tmpsrc[num28 + num24], num19, p2, p0i2);
				tmpsrc[num25] = modp_montymul(a4, b, p2, p0i2);
				tmpsrc[num25 + num6] = modp_montymul(a3, b, p2, p0i2);
				tmpsrc[num26] = modp_montymul(a2, b2, p2, p0i2);
				tmpsrc[num26 + num6] = modp_montymul(a, b2, p2, p0i2);
				num24++;
				num25 += num6 << 1;
				num26 += num6 << 1;
			}
			modp_iNTT2_ext(tmpsrc, num10 + i, num6, tmpsrc, num21, num, p2, p0i2);
			modp_iNTT2_ext(tmpsrc, num11 + i, num6, tmpsrc, num21, num, p2, p0i2);
		}
		zint_rebuild_CRT(tmpsrc, num10, num6, num6, num2, pRIMES, 1, tmpsrc, num12);
		zint_rebuild_CRT(tmpsrc, num11, num6, num6, num2, pRIMES, 1, tmpsrc, num12);
		FalconFPR[] array = new FalconFPR[num2];
		FalconFPR[] array2 = new FalconFPR[num2];
		FalconFPR[] array3 = new FalconFPR[num2];
		FalconFPR[] array4 = new FalconFPR[num2];
		FalconFPR[] array5 = new FalconFPR[num2 >> 1];
		int[] array6 = new int[num2];
		int num29 = ((num4 > 10) ? 10 : num4);
		poly_big_to_fp(array3, 0, tmpsrc, num9 + num4 - num29, num29, num4, num);
		poly_big_to_fp(array4, 0, tmpsrc, num13 + num4 - num29, num29, num4, num);
		int num30 = 31 * (num4 - num29);
		int num31 = BITLENGTH_avg[depth] - 6 * BITLENGTH_std[depth];
		int num32 = BITLENGTH_avg[depth] + 6 * BITLENGTH_std[depth];
		ffte.FFT(array3, 0, num);
		ffte.FFT(array4, 0, num);
		ffte.poly_invnorm2_fft(array5, 0, array3, 0, array4, 0, num);
		ffte.poly_adj_fft(array3, 0, num);
		ffte.poly_adj_fft(array4, 0, num);
		int num33 = num6;
		int num34 = 31 * num6;
		int num35 = num34 - num31;
		while (true)
		{
			num29 = ((num33 > 10) ? 10 : num33);
			int num36 = 31 * (num33 - num29);
			poly_big_to_fp(array, 0, tmpsrc, num10 + num33 - num29, num29, num6, num);
			poly_big_to_fp(array2, 0, tmpsrc, num11 + num33 - num29, num29, num6, num);
			ffte.FFT(array, 0, num);
			ffte.FFT(array2, 0, num);
			ffte.poly_mul_fft(array, 0, array3, 0, num);
			ffte.poly_mul_fft(array2, 0, array4, 0, num);
			ffte.poly_add(array2, 0, array, 0, num);
			ffte.poly_mul_autoadj_fft(array2, 0, array5, 0, num);
			ffte.iFFT(array2, 0, num);
			int num37 = num35 - num36 + num30;
			FalconFPR falconFPR;
			if (num37 < 0)
			{
				num37 = -num37;
				falconFPR = fpre.fpr_two;
			}
			else
			{
				falconFPR = fpre.fpr_onehalf;
			}
			FalconFPR falconFPR2 = fpre.fpr_one;
			while (num37 != 0)
			{
				if ((num37 & 1) != 0)
				{
					falconFPR2 = fpre.fpr_mul(falconFPR2, falconFPR);
				}
				num37 >>= 1;
				falconFPR = fpre.fpr_sqr(falconFPR);
			}
			for (i = 0; i < num2; i++)
			{
				FalconFPR falconFPR3 = fpre.fpr_mul(array2[i], falconFPR2);
				if (!fpre.fpr_lt(fpre.fpr_mtwo31m1, falconFPR3) || !fpre.fpr_lt(falconFPR3, fpre.fpr_ptwo31m1))
				{
					return 0;
				}
				array6[i] = (int)fpre.fpr_rint(falconFPR3);
			}
			uint sch = (uint)(num35 / 31);
			uint scl = (uint)(num35 % 31);
			if (depth <= 4)
			{
				poly_sub_scaled_ntt(tmpsrc, num10, num33, num6, tmpsrc, num9, num4, num4, array6, 0, sch, scl, num, tmpsrc, num12);
				poly_sub_scaled_ntt(tmpsrc, num11, num33, num6, tmpsrc, num13, num4, num4, array6, 0, sch, scl, num, tmpsrc, num12);
			}
			else
			{
				poly_sub_scaled(tmpsrc, num10, num33, num6, tmpsrc, num9, num4, num4, array6, 0, sch, scl, num);
				poly_sub_scaled(tmpsrc, num11, num33, num6, tmpsrc, num13, num4, num4, array6, 0, sch, scl, num);
			}
			int num38 = num35 + num32 + 10;
			if (num38 < num34)
			{
				num34 = num38;
				if (num33 * 31 >= num34 + 31)
				{
					num33--;
				}
			}
			if (num35 <= 0)
			{
				break;
			}
			num35 -= 25;
			if (num35 < 0)
			{
				num35 = 0;
			}
		}
		if (num33 < num4)
		{
			i = 0;
			while (i < num2)
			{
				uint num39 = (uint)(0L - (long)(tmpsrc[num10 + num33 - 1] >> 30)) >> 1;
				for (int j = num33; j < num4; j++)
				{
					tmpsrc[num10 + j] = num39;
				}
				num39 = (uint)(0L - (long)(tmpsrc[num11 + num33 - 1] >> 30)) >> 1;
				for (int j = num33; j < num4; j++)
				{
					tmpsrc[num11 + j] = num39;
				}
				i++;
				num10 += num6;
				num11 += num6;
			}
		}
		i = 0;
		num25 = tmp;
		num26 = tmp;
		while (i < num2 << 1)
		{
			Array.Copy(tmpsrc, num26, tmpsrc, num25, num4);
			i++;
			num25 += num4;
			num26 += num6;
		}
		return 1;
	}

	internal int solve_NTRU_binary_depth1(uint logn_top, sbyte[] fsrc, int f, sbyte[] gsrc, int g, uint[] tmpsrc, int tmp)
	{
		uint num = 1u;
		int num2 = 1 << (int)logn_top;
		uint num3 = logn_top - num;
		int num4 = 1 << (int)num3;
		int num5 = num4 >> 1;
		int num6 = MAX_BL_SMALL[num];
		int num7 = MAX_BL_SMALL[num + 1];
		int num8 = MAX_BL_LARGE[num];
		int num9 = tmp + num7 * num5;
		int num10 = num9 + num7 * num5;
		int num11 = num10 + num8 * num4;
		for (int i = 0; i < num8; i++)
		{
			uint p = PRIMES[i].p;
			uint p0i = modp_ninv31(p);
			uint r = modp_R2(p, p0i);
			uint rx = modp_Rx((uint)num7, p, p0i, r);
			int num12 = 0;
			int num13 = tmp;
			int num14 = num9;
			int num15 = num10 + i;
			int num16 = num11 + i;
			while (num12 < num5)
			{
				tmpsrc[num15] = zint_mod_small_signed(tmpsrc, num13, num7, p, p0i, r, rx);
				tmpsrc[num16] = zint_mod_small_signed(tmpsrc, num14, num7, p, p0i, r, rx);
				num12++;
				num13 += num7;
				num14 += num7;
				num15 += num8;
				num16 += num8;
			}
		}
		Array.Copy(tmpsrc, num10, tmpsrc, tmp, num8 * num4);
		num10 = tmp;
		Array.Copy(tmpsrc, num11, tmpsrc, num10 + num8 * num4, num8 * num4);
		num11 = num10 + num8 * num4;
		int num17 = num11 + num8 * num4;
		int num18 = num17 + num6 * num4;
		int num19 = num18 + num6 * num4;
		for (int i = 0; i < num8; i++)
		{
			uint p2 = PRIMES[i].p;
			uint p0i2 = modp_ninv31(p2);
			uint num20 = modp_R2(p2, p0i2);
			int num21 = num19;
			int num22 = num21 + num2;
			int num23 = num22 + num4;
			int num24 = num23 + num2;
			modp_mkgm2(tmpsrc, num21, tmpsrc, num22, logn_top, PRIMES[i].g, p2, p0i2);
			int j;
			for (j = 0; j < num2; j++)
			{
				tmpsrc[num23 + j] = modp_set(fsrc[f + j], p2);
				tmpsrc[num24 + j] = modp_set(gsrc[g + j], p2);
			}
			modp_NTT2(tmpsrc, num23, tmpsrc, num21, logn_top, p2, p0i2);
			modp_NTT2(tmpsrc, num24, tmpsrc, num21, logn_top, p2, p0i2);
			for (uint num25 = logn_top; num25 > num3; num25--)
			{
				modp_poly_rec_res(tmpsrc, num23, num25, p2, p0i2, num20);
				modp_poly_rec_res(tmpsrc, num24, num25, p2, p0i2, num20);
			}
			if (num != 0)
			{
				Array.Copy(tmpsrc, num22, tmpsrc, num21 + num4, num4);
				num22 = num21 + num4;
				Array.Copy(tmpsrc, num23, tmpsrc, num22 + num4, num4);
				num23 = num22 + num4;
				Array.Copy(tmpsrc, num24, tmpsrc, num23 + num4, num4);
				num24 = num23 + num4;
			}
			int num26 = num24 + num4;
			int num27 = num26 + num5;
			j = 0;
			int num28 = num10 + i;
			int num29 = num11 + i;
			while (j < num5)
			{
				tmpsrc[num26 + j] = tmpsrc[num28];
				tmpsrc[num27 + j] = tmpsrc[num29];
				j++;
				num28 += num8;
				num29 += num8;
			}
			modp_NTT2(tmpsrc, num26, tmpsrc, num21, num3 - 1, p2, p0i2);
			modp_NTT2(tmpsrc, num27, tmpsrc, num21, num3 - 1, p2, p0i2);
			j = 0;
			num28 = num10 + i;
			num29 = num11 + i;
			while (j < num5)
			{
				uint a = tmpsrc[num23 + (j << 1)];
				uint a2 = tmpsrc[num23 + (j << 1) + 1];
				uint a3 = tmpsrc[num24 + (j << 1)];
				uint a4 = tmpsrc[num24 + (j << 1) + 1];
				uint b = modp_montymul(tmpsrc[num26 + j], num20, p2, p0i2);
				uint b2 = modp_montymul(tmpsrc[num27 + j], num20, p2, p0i2);
				tmpsrc[num28] = modp_montymul(a4, b, p2, p0i2);
				tmpsrc[num28 + num8] = modp_montymul(a3, b, p2, p0i2);
				tmpsrc[num29] = modp_montymul(a2, b2, p2, p0i2);
				tmpsrc[num29 + num8] = modp_montymul(a, b2, p2, p0i2);
				j++;
				num28 += num8 << 1;
				num29 += num8 << 1;
			}
			modp_iNTT2_ext(tmpsrc, num10 + i, num8, tmpsrc, num22, num3, p2, p0i2);
			modp_iNTT2_ext(tmpsrc, num11 + i, num8, tmpsrc, num22, num3, p2, p0i2);
			if (i < num6)
			{
				modp_iNTT2(tmpsrc, num23, tmpsrc, num22, num3, p2, p0i2);
				modp_iNTT2(tmpsrc, num24, tmpsrc, num22, num3, p2, p0i2);
				j = 0;
				num28 = num17 + i;
				num29 = num18 + i;
				while (j < num4)
				{
					tmpsrc[num28] = tmpsrc[num23 + j];
					tmpsrc[num29] = tmpsrc[num24 + j];
					j++;
					num28 += num6;
					num29 += num6;
				}
			}
		}
		zint_rebuild_CRT(tmpsrc, num10, num8, num8, num4 << 1, PRIMES, 1, tmpsrc, num19);
		zint_rebuild_CRT(tmpsrc, num17, num6, num6, num4 << 1, PRIMES, 1, tmpsrc, num19);
		FalconFPR[] array = new FalconFPR[num4];
		FalconFPR[] array2 = new FalconFPR[num4];
		poly_big_to_fp(array, 0, tmpsrc, num10, num8, num8, num3);
		poly_big_to_fp(array2, 0, tmpsrc, num11, num8, num8, num3);
		Array.Copy(tmpsrc, num17, tmpsrc, tmp, 2 * num6 * num4);
		num17 = tmp;
		num18 = num17 + num6 * num4;
		FalconFPR[] array3 = new FalconFPR[num4];
		FalconFPR[] array4 = new FalconFPR[num4];
		poly_big_to_fp(array3, 0, tmpsrc, num17, num6, num6, num3);
		poly_big_to_fp(array4, 0, tmpsrc, num18, num6, num6, num3);
		ffte.FFT(array, 0, num3);
		ffte.FFT(array2, 0, num3);
		ffte.FFT(array3, 0, num3);
		ffte.FFT(array4, 0, num3);
		FalconFPR[] array5 = new FalconFPR[num4];
		FalconFPR[] array6 = new FalconFPR[num4];
		ffte.poly_add_muladj_fft(array5, 0, array, 0, array2, 0, array3, 0, array4, 0, num3);
		ffte.poly_invnorm2_fft(array6, 0, array3, 0, array4, 0, num3);
		ffte.poly_mul_autoadj_fft(array5, 0, array6, 0, num3);
		ffte.iFFT(array5, 0, num3);
		for (int i = 0; i < num4; i++)
		{
			FalconFPR falconFPR = array5[i];
			if (!fpre.fpr_lt(falconFPR, fpre.fpr_ptwo63m1) || !fpre.fpr_lt(fpre.fpr_mtwo63m1, falconFPR))
			{
				return 0;
			}
			array5[i] = fpre.fpr_of(fpre.fpr_rint(falconFPR));
		}
		ffte.FFT(array5, 0, num3);
		ffte.poly_mul_fft(array3, 0, array5, 0, num3);
		ffte.poly_mul_fft(array4, 0, array5, 0, num3);
		ffte.poly_sub(array, 0, array3, 0, num3);
		ffte.poly_sub(array2, 0, array4, 0, num3);
		ffte.iFFT(array, 0, num3);
		ffte.iFFT(array2, 0, num3);
		num10 = tmp;
		num11 = num10 + num4;
		for (int i = 0; i < num4; i++)
		{
			tmpsrc[num10 + i] = (uint)fpre.fpr_rint(array[i]);
			tmpsrc[num11 + i] = (uint)fpre.fpr_rint(array2[i]);
		}
		return 1;
	}

	internal int solve_NTRU_binary_depth0(uint logn, sbyte[] fsrc, int f, sbyte[] gsrc, int g, uint[] tmpsrc, int tmp)
	{
		int num = 1 << (int)logn;
		int num2 = num >> 1;
		uint p = PRIMES[0].p;
		uint p0i = modp_ninv31(p);
		uint b = modp_R2(p, p0i);
		int num3 = tmp + num2;
		int num4 = num3 + num2;
		int num5 = num4 + num;
		int num6 = num5 + num;
		int igm = num6 + num;
		modp_mkgm2(tmpsrc, num6, tmpsrc, igm, logn, PRIMES[0].g, p, p0i);
		for (int i = 0; i < num2; i++)
		{
			tmpsrc[tmp + i] = modp_set(zint_one_to_plain(tmpsrc, tmp + i), p);
			tmpsrc[num3 + i] = modp_set(zint_one_to_plain(tmpsrc, num3 + i), p);
		}
		modp_NTT2(tmpsrc, tmp, tmpsrc, num6, logn - 1, p, p0i);
		modp_NTT2(tmpsrc, num3, tmpsrc, num6, logn - 1, p, p0i);
		for (int i = 0; i < num; i++)
		{
			tmpsrc[num4 + i] = modp_set(fsrc[f + i], p);
			tmpsrc[num5 + i] = modp_set(gsrc[g + i], p);
		}
		modp_NTT2(tmpsrc, num4, tmpsrc, num6, logn, p, p0i);
		modp_NTT2(tmpsrc, num5, tmpsrc, num6, logn, p, p0i);
		for (int i = 0; i < num; i += 2)
		{
			uint a = tmpsrc[num4 + i];
			uint a2 = tmpsrc[num4 + i + 1];
			uint a3 = tmpsrc[num5 + i];
			uint a4 = tmpsrc[num5 + i + 1];
			uint b2 = modp_montymul(tmpsrc[tmp + (i >> 1)], b, p, p0i);
			uint b3 = modp_montymul(tmpsrc[num3 + (i >> 1)], b, p, p0i);
			tmpsrc[num4 + i] = modp_montymul(a4, b2, p, p0i);
			tmpsrc[num4 + i + 1] = modp_montymul(a3, b2, p, p0i);
			tmpsrc[num5 + i] = modp_montymul(a2, b3, p, p0i);
			tmpsrc[num5 + i + 1] = modp_montymul(a, b3, p, p0i);
		}
		modp_iNTT2(tmpsrc, num4, tmpsrc, igm, logn, p, p0i);
		modp_iNTT2(tmpsrc, num5, tmpsrc, igm, logn, p, p0i);
		num3 = tmp + num;
		int num7 = num3 + num;
		Array.Copy(tmpsrc, num4, tmpsrc, tmp, 2 * num);
		int num8 = num7 + num;
		int num9 = num8 + num;
		int num10 = num9 + num;
		int num11 = num10 + num;
		modp_mkgm2(tmpsrc, num7, tmpsrc, num8, logn, PRIMES[0].g, p, p0i);
		modp_NTT2(tmpsrc, tmp, tmpsrc, num7, logn, p, p0i);
		modp_NTT2(tmpsrc, num3, tmpsrc, num7, logn, p, p0i);
		tmpsrc[num10] = (tmpsrc[num11] = modp_set(fsrc[f], p));
		for (int i = 1; i < num; i++)
		{
			tmpsrc[num10 + i] = modp_set(fsrc[f + i], p);
			tmpsrc[num11 + num - i] = modp_set(-fsrc[f + i], p);
		}
		modp_NTT2(tmpsrc, num10, tmpsrc, num7, logn, p, p0i);
		modp_NTT2(tmpsrc, num11, tmpsrc, num7, logn, p, p0i);
		for (int i = 0; i < num; i++)
		{
			uint a5 = modp_montymul(tmpsrc[num11 + i], b, p, p0i);
			tmpsrc[num8 + i] = modp_montymul(a5, tmpsrc[tmp + i], p, p0i);
			tmpsrc[num9 + i] = modp_montymul(a5, tmpsrc[num10 + i], p, p0i);
		}
		tmpsrc[num10] = (tmpsrc[num11] = modp_set(gsrc[g], p));
		for (int i = 1; i < num; i++)
		{
			tmpsrc[num10 + i] = modp_set(gsrc[g + i], p);
			tmpsrc[num11 + num - i] = modp_set(-gsrc[g + i], p);
		}
		modp_NTT2(tmpsrc, num10, tmpsrc, num7, logn, p, p0i);
		modp_NTT2(tmpsrc, num11, tmpsrc, num7, logn, p, p0i);
		for (int i = 0; i < num; i++)
		{
			uint a6 = modp_montymul(tmpsrc[num11 + i], b, p, p0i);
			tmpsrc[num8 + i] = modp_add(tmpsrc[num8 + i], modp_montymul(a6, tmpsrc[num3 + i], p, p0i), p);
			tmpsrc[num9 + i] = modp_add(tmpsrc[num9 + i], modp_montymul(a6, tmpsrc[num10 + i], p, p0i), p);
		}
		modp_mkgm2(tmpsrc, num7, tmpsrc, num10, logn, PRIMES[0].g, p, p0i);
		modp_iNTT2(tmpsrc, num8, tmpsrc, num10, logn, p, p0i);
		modp_iNTT2(tmpsrc, num9, tmpsrc, num10, logn, p, p0i);
		for (int i = 0; i < num; i++)
		{
			tmpsrc[num7 + i] = (uint)modp_norm(tmpsrc[num8 + i], p);
			tmpsrc[num8 + i] = (uint)modp_norm(tmpsrc[num9 + i], p);
		}
		FalconFPR[] array = new FalconFPR[2 * num];
		int num12 = num;
		for (int i = 0; i < num; i++)
		{
			array[num12 + i] = fpre.fpr_of((int)tmpsrc[num8 + i]);
		}
		ffte.FFT(array, num12, logn);
		int num13 = 0;
		Array.Copy(array, num12, array, num13, num2);
		num12 = num13 + num2;
		for (int i = 0; i < num; i++)
		{
			array[num12 + i] = fpre.fpr_of((int)tmpsrc[num7 + i]);
		}
		ffte.FFT(array, num12, logn);
		ffte.poly_div_autoadj_fft(array, num12, array, num13, logn);
		ffte.iFFT(array, num12, logn);
		for (int i = 0; i < num; i++)
		{
			tmpsrc[num7 + i] = modp_set((int)fpre.fpr_rint(array[num12 + i]), p);
		}
		num8 = num7 + num;
		num9 = num8 + num;
		num10 = num9 + num;
		num11 = num10 + num;
		modp_mkgm2(tmpsrc, num8, tmpsrc, num9, logn, PRIMES[0].g, p, p0i);
		for (int i = 0; i < num; i++)
		{
			tmpsrc[num10 + i] = modp_set(fsrc[f + i], p);
			tmpsrc[num11 + i] = modp_set(gsrc[g + i], p);
		}
		modp_NTT2(tmpsrc, num7, tmpsrc, num8, logn, p, p0i);
		modp_NTT2(tmpsrc, num10, tmpsrc, num8, logn, p, p0i);
		modp_NTT2(tmpsrc, num11, tmpsrc, num8, logn, p, p0i);
		for (int i = 0; i < num; i++)
		{
			uint a7 = modp_montymul(tmpsrc[num7 + i], b, p, p0i);
			tmpsrc[tmp + i] = modp_sub(tmpsrc[tmp + i], modp_montymul(a7, tmpsrc[num10 + i], p, p0i), p);
			tmpsrc[num3 + i] = modp_sub(tmpsrc[num3 + i], modp_montymul(a7, tmpsrc[num11 + i], p, p0i), p);
		}
		modp_iNTT2(tmpsrc, tmp, tmpsrc, num9, logn, p, p0i);
		modp_iNTT2(tmpsrc, num3, tmpsrc, num9, logn, p, p0i);
		for (int i = 0; i < num; i++)
		{
			tmpsrc[tmp + i] = (uint)modp_norm(tmpsrc[tmp + i], p);
			tmpsrc[num3 + i] = (uint)modp_norm(tmpsrc[num3 + i], p);
		}
		return 1;
	}

	internal int solve_NTRU(uint logn, sbyte[] Fsrc, int F, sbyte[] Gsrc, int G, sbyte[] fsrc, int f, sbyte[] gsrc, int g, int lim, uint[] tmpsrc, int tmp)
	{
		int num = 1 << (int)logn;
		if (solve_NTRU_deepest(logn, fsrc, f, gsrc, g, tmpsrc, tmp) == 0)
		{
			return 0;
		}
		if (logn <= 2)
		{
			uint depth = logn;
			while (depth-- != 0)
			{
				if (solve_NTRU_intermediate(logn, fsrc, f, gsrc, g, depth, tmpsrc, tmp) == 0)
				{
					return 0;
				}
			}
		}
		else
		{
			uint depth2 = logn;
			while (depth2-- > 2)
			{
				if (solve_NTRU_intermediate(logn, fsrc, f, gsrc, g, depth2, tmpsrc, tmp) == 0)
				{
					return 0;
				}
			}
			if (solve_NTRU_binary_depth1(logn, fsrc, f, gsrc, g, tmpsrc, tmp) == 0)
			{
				return 0;
			}
			if (solve_NTRU_binary_depth0(logn, fsrc, f, gsrc, g, tmpsrc, tmp) == 0)
			{
				return 0;
			}
		}
		if (Gsrc == null)
		{
			G = 0;
			Gsrc = new sbyte[num];
		}
		if (poly_big_to_small(Fsrc, F, tmpsrc, tmp, lim, logn) == 0 || poly_big_to_small(Gsrc, G, tmpsrc, tmp + num, lim, logn) == 0)
		{
			return 0;
		}
		int num2 = tmp + num;
		int num3 = num2 + num;
		int num4 = num3 + num;
		int gm = num4 + num;
		FalconSmallPrime[] pRIMES = PRIMES;
		uint p = pRIMES[0].p;
		uint p0i = modp_ninv31(p);
		modp_mkgm2(tmpsrc, gm, tmpsrc, tmp, logn, pRIMES[0].g, p, p0i);
		for (int i = 0; i < num; i++)
		{
			tmpsrc[tmp + i] = modp_set(Gsrc[G + i], p);
		}
		for (int i = 0; i < num; i++)
		{
			tmpsrc[num2 + i] = modp_set(fsrc[f + i], p);
			tmpsrc[num3 + i] = modp_set(gsrc[g + i], p);
			tmpsrc[num4 + i] = modp_set(Fsrc[F + i], p);
		}
		modp_NTT2(tmpsrc, num2, tmpsrc, gm, logn, p, p0i);
		modp_NTT2(tmpsrc, num3, tmpsrc, gm, logn, p, p0i);
		modp_NTT2(tmpsrc, num4, tmpsrc, gm, logn, p, p0i);
		modp_NTT2(tmpsrc, tmp, tmpsrc, gm, logn, p, p0i);
		uint num5 = modp_montymul(12289u, 1u, p, p0i);
		for (int i = 0; i < num; i++)
		{
			if (modp_sub(modp_montymul(tmpsrc[num2 + i], tmpsrc[tmp + i], p, p0i), modp_montymul(tmpsrc[num3 + i], tmpsrc[num4 + i], p, p0i), p) != num5)
			{
				return 0;
			}
		}
		return 1;
	}

	internal void poly_small_mkgauss(SHAKE256 rng, sbyte[] fsrc, int f, uint logn)
	{
		int num = 1 << (int)logn;
		uint num2 = 0u;
		for (int i = 0; i < num; i++)
		{
			int num3;
			while (true)
			{
				num3 = mkgauss(rng, logn);
				if (num3 >= -127 && num3 <= 127)
				{
					if (i != num - 1)
					{
						num2 ^= (uint)(num3 & 1);
						break;
					}
					if ((num2 ^ (uint)(num3 & 1)) != 0)
					{
						break;
					}
				}
			}
			fsrc[f + i] = (sbyte)num3;
		}
	}

	internal void keygen(SHAKE256 rng, sbyte[] fsrc, int f, sbyte[] gsrc, int g, sbyte[] Fsrc, int F, sbyte[] Gsrc, int G, ushort[] hsrc, int h, uint logn)
	{
		int num = 1 << (int)logn;
		while (true)
		{
			poly_small_mkgauss(rng, fsrc, f, logn);
			poly_small_mkgauss(rng, gsrc, g, logn);
			int num2 = 1 << codec.max_fg_bits[logn] - 1;
			for (int i = 0; i < num; i++)
			{
				if (fsrc[f + i] >= num2 || fsrc[f + i] <= -num2 || gsrc[g + i] >= num2 || gsrc[g + i] <= -num2)
				{
					num2 = -1;
					break;
				}
			}
			if (num2 < 0)
			{
				continue;
			}
			uint num3 = poly_small_sqnorm(fsrc, f, logn);
			uint num4 = poly_small_sqnorm(gsrc, g, logn);
			if ((uint)((num3 + num4) | (0L - (long)((num3 | num4) >> 31))) >= 16823)
			{
				continue;
			}
			FalconFPR[] array = new FalconFPR[3 * num];
			int num5 = 0;
			int num6 = num5 + num;
			int num7 = num6 + num;
			poly_small_to_fp(array, num5, fsrc, f, logn);
			poly_small_to_fp(array, num6, gsrc, g, logn);
			ffte.FFT(array, num5, logn);
			ffte.FFT(array, num6, logn);
			ffte.poly_invnorm2_fft(array, num7, array, num5, array, num6, logn);
			ffte.poly_adj_fft(array, num5, logn);
			ffte.poly_adj_fft(array, num6, logn);
			ffte.poly_mulconst(array, num5, fpre.fpr_q, logn);
			ffte.poly_mulconst(array, num6, fpre.fpr_q, logn);
			ffte.poly_mul_autoadj_fft(array, num5, array, num7, logn);
			ffte.poly_mul_autoadj_fft(array, num6, array, num7, logn);
			ffte.iFFT(array, num5, logn);
			ffte.iFFT(array, num6, logn);
			FalconFPR x = fpre.fpr_zero;
			for (int i = 0; i < num; i++)
			{
				x = fpre.fpr_add(x, fpre.fpr_sqr(array[num5 + i]));
				x = fpre.fpr_add(x, fpre.fpr_sqr(array[num6 + i]));
			}
			if (!fpre.fpr_lt(x, fpre.fpr_bnorm_max))
			{
				continue;
			}
			ushort[] array2;
			int num8;
			ushort[] hsrc2;
			int tmp;
			if (hsrc == null)
			{
				array2 = new ushort[2 * num];
				num8 = 0;
				hsrc2 = array2;
				tmp = num8 + num;
			}
			else
			{
				array2 = new ushort[num];
				num8 = h;
				hsrc2 = hsrc;
				tmp = 0;
			}
			if (vrfy.compute_public(hsrc2, num8, fsrc, f, gsrc, g, logn, array2, tmp) != 0)
			{
				uint[] tmpsrc = ((logn > 2) ? new uint[28 * num] : new uint[28 * num * 3]);
				num2 = (1 << codec.max_FG_bits[logn] - 1) - 1;
				if (solve_NTRU(logn, Fsrc, F, Gsrc, G, fsrc, f, gsrc, g, num2, tmpsrc, 0) != 0)
				{
					break;
				}
			}
		}
	}
}
