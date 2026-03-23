using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal abstract class HarakaSBase
{
	private static readonly byte[] RC0 = Hex.DecodeStrict("0684704ce620c00ab2c5fef075817b9d");

	private static readonly byte[] RC1 = Hex.DecodeStrict("8b66b4e188f3a06b640f6ba42f08f717");

	private static readonly byte[] RC2 = Hex.DecodeStrict("3402de2d53f28498cf029d609f029114");

	private static readonly byte[] RC3 = Hex.DecodeStrict("0ed6eae62e7b4f08bbf3bcaffd5b4f79");

	private static readonly byte[] RC4 = Hex.DecodeStrict("cbcfb0cb4872448b79eecd1cbe397044");

	private static readonly byte[] RC5 = Hex.DecodeStrict("7eeacdee6e9032b78d5335ed2b8a057b");

	private static readonly byte[] RC6 = Hex.DecodeStrict("67c28f435e2e7cd0e2412761da4fef1b");

	private static readonly byte[] RC7 = Hex.DecodeStrict("2924d9b0afcacc07675ffde21fc70b3b");

	private static readonly byte[] RC8 = Hex.DecodeStrict("ab4d63f1e6867fe9ecdb8fcab9d465ee");

	private static readonly byte[] RC9 = Hex.DecodeStrict("1c30bf84d4b7cd645b2a404fad037e33");

	private static readonly byte[] RC10 = Hex.DecodeStrict("b2cc0bb9941723bf69028b2e8df69800");

	private static readonly byte[] RC11 = Hex.DecodeStrict("fa0478a6de6f55724aaa9ec85c9d2d8a");

	private static readonly byte[] RC12 = Hex.DecodeStrict("dfb49f2b6b772a120efa4f2e29129fd4");

	private static readonly byte[] RC13 = Hex.DecodeStrict("1ea10344f449a23632d611aebb6a12ee");

	private static readonly byte[] RC14 = Hex.DecodeStrict("af0449884b0500845f9600c99ca8eca6");

	private static readonly byte[] RC15 = Hex.DecodeStrict("21025ed89d199c4f78a2c7e327e593ec");

	private static readonly byte[] RC16 = Hex.DecodeStrict("bf3aaaf8a759c9b7b9282ecd82d40173");

	private static readonly byte[] RC17 = Hex.DecodeStrict("6260700d6186b01737f2efd910307d6b");

	private static readonly byte[] RC18 = Hex.DecodeStrict("5aca45c22130044381c29153f6fc9ac6");

	private static readonly byte[] RC19 = Hex.DecodeStrict("9223973c226b68bb2caf92e836d1943a");

	private static readonly byte[] RC20 = Hex.DecodeStrict("d3bf9238225886eb6cbab958e51071b4");

	private static readonly byte[] RC21 = Hex.DecodeStrict("db863ce5aef0c677933dfddd24e1128d");

	private static readonly byte[] RC22 = Hex.DecodeStrict("bb606268ffeba09c83e48de3cb2212b1");

	private static readonly byte[] RC23 = Hex.DecodeStrict("734bd3dce2e4d19c2db91a4ec72bf77d");

	private static readonly byte[] RC24 = Hex.DecodeStrict("43bb47c361301b434b1415c42cb3924e");

	private static readonly byte[] RC25 = Hex.DecodeStrict("dba775a8e707eff603b231dd16eb6899");

	private static readonly byte[] RC26 = Hex.DecodeStrict("6df3614b3c7559778e5e23027eca472c");

	private static readonly byte[] RC27 = Hex.DecodeStrict("cda75a17d6de7d776d1be5b9b88617f9");

	private static readonly byte[] RC28 = Hex.DecodeStrict("ec6b43f06ba8e9aa9d6c069da946ee5d");

	private static readonly byte[] RC29 = Hex.DecodeStrict("cb1e6950f957332ba25311593bf327c1");

	private static readonly byte[] RC30 = Hex.DecodeStrict("2cee0c7500da619ce4ed0353600ed0d9");

	private static readonly byte[] RC31 = Hex.DecodeStrict("f0b1a5a196e90cab80bbbabc63a4a350");

	private static readonly byte[] RC32 = Hex.DecodeStrict("ae3db1025e962988ab0dde30938dca39");

	private static readonly byte[] RC33 = Hex.DecodeStrict("17bb8f38d554a40b8814f3a82e75b442");

	private static readonly byte[] RC34 = Hex.DecodeStrict("34bb8a5b5f427fd7aeb6b779360a16f6");

	private static readonly byte[] RC35 = Hex.DecodeStrict("26f65241cbe5543843ce5918ffbaafde");

	private static readonly byte[] RC36 = Hex.DecodeStrict("4ce99a54b9f3026aa2ca9cf7839ec978");

	private static readonly byte[] RC37 = Hex.DecodeStrict("ae51a51a1bdff7be40c06e2822901235");

	private static readonly byte[] RC38 = Hex.DecodeStrict("a0c1613cba7ed22bc173bc0f48a659cf");

	private static readonly byte[] RC39 = Hex.DecodeStrict("756acc03022882884ad6bdfde9c59da1");

	private static readonly byte[][] RoundConstants = new byte[40][]
	{
		RC0, RC1, RC2, RC3, RC4, RC5, RC6, RC7, RC8, RC9,
		RC10, RC11, RC12, RC13, RC14, RC15, RC16, RC17, RC18, RC19,
		RC20, RC21, RC22, RC23, RC24, RC25, RC26, RC27, RC28, RC29,
		RC30, RC31, RC32, RC33, RC34, RC35, RC36, RC37, RC38, RC39
	};

	internal ulong[][] haraka512_rc = new ulong[10][]
	{
		new ulong[8] { 2652350495371256459uL, 13679383618923496322uL, 15667935350676443303uL, 12307783811503579017uL, 4944264682582508575uL, 5312892415214084856uL, 390034814247088728uL, 2584105839607850161uL },
		new ulong[8] { 15616813271728675694uL, 9137660425067592590uL, 7974068014816832049uL, 13780800007984394558uL, 2602240152241800734uL, 16921049717778260714uL, 8634660511727056099uL, 1757945485816280992uL },
		new ulong[8] { 1181946526362588450uL, 15681551453717171323uL, 3395396416743122529uL, 13330470973160179193uL, 17161289763912047618uL, 15083446463894380355uL, 10085908215316552625uL, 16075391737095583129uL },
		new ulong[8] { 15945890618932795584uL, 8465221333286591414uL, 8817016078209461823uL, 9067727467981428858uL, 4244107674518258433uL, 14099417613138662078uL, 1711371409274742987uL, 6486926172609168623uL },
		new ulong[8] { 1689001080716996467uL, 17955247947431300943uL, 1273395568185090836uL, 5805238412293617850uL, 15005454302784166761uL, 4592753210857527691uL, 7062886034259989751uL, 10472350096676379060uL },
		new ulong[8] { 17648925974889833326uL, 18405283813057758144uL, 476036171179798187uL, 7391697506481003962uL, 17591081798538862141uL, 14957403234123739981uL, 13555218339221595128uL, 9110006695579921767uL },
		new ulong[8] { 17559805991765990826uL, 4212830408327159617uL, 14900069586142268981uL, 16491364651582513327uL, 3174578079917510314uL, 5156046680874954380uL, 18128198267874729785uL, 12270330065560089274uL },
		new ulong[8] { 2529785914229181047uL, 2966313764524854080uL, 6363694428402697361uL, 8292109690175819701uL, 9949197741574092029uL, 15235635597554736000uL, 12919805279922909295uL, 13470774230082493846uL },
		new ulong[8] { 3357847021085574721uL, 13681906861144364558uL, 17820352244308902924uL, 2124133995575340009uL, 7425858999829294301uL, 15014711204803913845uL, 1119301198758921294uL, 1907812968586478892uL },
		new ulong[8] { 9460219246996718814uL, 3356175496741300052uL, 12682143756069655254uL, 4002747967109689317uL, 9727818913976054419uL, 16508680301122176955uL, 10442994283813605781uL, 7302960353763723932uL }
	};

	internal uint[][] haraka256_rc = new uint[10][];

	protected readonly byte[] buffer;

	protected int off;

	protected HarakaSBase()
	{
		buffer = new byte[64];
		off = 0;
		byte[] array = new byte[640];
		_ = new byte[16];
		for (int i = 0; i < 40; i++)
		{
			Arrays.Reverse(RoundConstants[i]).CopyTo(array, i << 4);
		}
		for (int j = 0; j < 10; j++)
		{
			InterleaveConstant(haraka512_rc[j], array, j << 6);
		}
	}

	protected void Reset()
	{
		off = 0;
		Arrays.Clear(buffer);
	}

	protected static void InterleaveConstant(ulong[] output, byte[] input, int startPos)
	{
		uint[] array = new uint[16];
		Pack.LE_To_UInt32(input, startPos, array);
		for (int i = 0; i < 4; i++)
		{
			BrAesCt64InterleaveIn(output, i, array, i << 2);
		}
		BrAesCt64Ortho(output);
	}

	protected static void InterleaveConstant32(uint[] output, byte[] input, int startPos)
	{
		for (int i = 0; i < 4; i++)
		{
			output[i << 1] = Pack.LE_To_UInt32(input, startPos + (i << 2));
			output[(i << 1) + 1] = Pack.LE_To_UInt32(input, startPos + (i << 2) + 16);
		}
		BrAesCtOrtho(output);
	}

	internal void Haraka512Perm(byte[] output)
	{
		uint[] array = new uint[16];
		ulong[] array2 = new ulong[8];
		Pack.LE_To_UInt32(buffer, 0, array);
		for (int i = 0; i < 4; i++)
		{
			BrAesCt64InterleaveIn(array2, i, array, i << 2);
		}
		BrAesCt64Ortho(array2);
		for (int j = 0; j < 5; j++)
		{
			for (int k = 0; k < 2; k++)
			{
				BrAesCt64BitsliceSbox(array2);
				ShiftRows(array2);
				MixColumns(array2);
				AddRoundKey(array2, haraka512_rc[(j << 1) + k]);
			}
			for (int l = 0; l < 8; l++)
			{
				ulong num = array2[l];
				array2[l] = ((num & 0x1000100010001L) << 5) | ((num & 0x2000200020002L) << 12) | ((num & 0x4000400040004L) >> 1) | ((num & 0x8000800080008L) << 6) | ((num & 0x20002000200020L) << 9) | ((num & 0x40004000400040L) >> 4) | ((num & 0x80008000800080L) << 3) | ((num & 0x2100210021002100L) >> 5) | ((num & 0x210021002100210L) << 2) | ((num & 0x800080008000800L) << 4) | ((num & 0x1000100010001000L) >> 12) | ((num & 0x4000400040004000L) >> 10) | ((num & 0x8400840084008400uL) >> 3);
			}
		}
		BrAesCt64Ortho(array2);
		for (int m = 0; m < 4; m++)
		{
			BrAesCt64InterleaveOut(array, array2, m);
		}
		for (int n = 0; n < 16; n++)
		{
			for (int num2 = 0; num2 < 4; num2++)
			{
				output[(n << 2) + num2] = (byte)(array[n] >> (num2 << 3));
			}
		}
	}

	internal void Haraka256Perm(byte[] output)
	{
		uint[] array = new uint[8];
		InterleaveConstant32(array, buffer, 0);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				BrAesCtBitsliceSbox(array);
				ShiftRows32(array);
				MixColumns32(array);
				AddRoundKey32(array, haraka256_rc[(i << 1) + j]);
			}
			for (int k = 0; k < 8; k++)
			{
				uint x = Bits.BitPermuteStep(array[k], 202116108u, 2);
				array[k] = Bits.BitPermuteStep(x, 572662306u, 1);
			}
		}
		BrAesCtOrtho(array);
		for (int l = 0; l < 4; l++)
		{
			Pack.UInt32_To_LE(array[l << 1], output, l << 2);
			Pack.UInt32_To_LE(array[(l << 1) + 1], output, (l << 2) + 16);
		}
	}

	private static void BrAesCt64InterleaveIn(ulong[] q, int qPos, uint[] w, int startPos)
	{
		ulong num = (ulong)w[startPos] & 0xFFFFFFFFuL;
		ulong num2 = (ulong)w[startPos + 1] & 0xFFFFFFFFuL;
		ulong num3 = (ulong)w[startPos + 2] & 0xFFFFFFFFuL;
		ulong num4 = (ulong)w[startPos + 3] & 0xFFFFFFFFuL;
		num |= num << 16;
		num2 |= num2 << 16;
		num3 |= num3 << 16;
		num4 |= num4 << 16;
		num &= 0xFFFF0000FFFFL;
		num2 &= 0xFFFF0000FFFFL;
		num3 &= 0xFFFF0000FFFFL;
		num4 &= 0xFFFF0000FFFFL;
		num |= num << 8;
		num2 |= num2 << 8;
		num3 |= num3 << 8;
		num4 |= num4 << 8;
		num &= 0xFF00FF00FF00FFL;
		num2 &= 0xFF00FF00FF00FFL;
		num3 &= 0xFF00FF00FF00FFL;
		num4 &= 0xFF00FF00FF00FFL;
		q[qPos] = num | (num3 << 8);
		q[qPos + 4] = num2 | (num4 << 8);
	}

	private static void BrAesCtBitsliceSbox(uint[] q)
	{
		uint num = q[7];
		uint num2 = q[6];
		uint num3 = q[5];
		uint num4 = q[4];
		uint num5 = q[3];
		uint num6 = q[2];
		uint num7 = q[1];
		uint num8 = q[0];
		uint num9 = num4 ^ num6;
		uint num10 = num ^ num7;
		uint num11 = num ^ num4;
		uint num12 = num ^ num6;
		uint num13 = num2 ^ num3;
		uint num14 = num13 ^ num8;
		uint num15 = num14 ^ num4;
		uint num16 = num10 ^ num9;
		uint num17 = num14 ^ num;
		uint num18 = num14 ^ num7;
		uint num19 = num18 ^ num12;
		uint num20 = num5 ^ num16;
		uint num21 = num20 ^ num6;
		uint num22 = num20 ^ num2;
		uint num23 = num21 ^ num8;
		uint num24 = num21 ^ num13;
		uint num25 = num22 ^ num11;
		uint num26 = num8 ^ num25;
		uint num27 = num24 ^ num25;
		uint num28 = num24 ^ num12;
		uint num29 = num13 ^ num25;
		uint num30 = num10 ^ num29;
		uint num31 = num ^ num29;
		uint num32 = num16 & num21;
		uint num33 = (num19 & num23) ^ num32;
		uint num34 = (num15 & num8) ^ num32;
		uint num35 = num10 & num29;
		uint num36 = (num18 & num14) ^ num35;
		uint num37 = (num17 & num26) ^ num35;
		uint num38 = num11 & num25;
		uint num39 = (num9 & num27) ^ num38;
		uint num40 = (num12 & num24) ^ num38;
		uint num41 = num33 ^ num39;
		uint num42 = num34 ^ num40;
		uint num43 = num36 ^ num39;
		uint num44 = num37 ^ num40;
		uint num45 = num41 ^ num22;
		uint num46 = num42 ^ num28;
		uint num47 = num43 ^ num30;
		uint num48 = num44 ^ num31;
		uint num49 = num45 ^ num46;
		uint num50 = num45 & num47;
		uint num51 = num48 ^ num50;
		uint num52 = (num49 & num51) ^ num46;
		uint num53 = num47 ^ num48;
		uint num54 = ((num46 ^ num50) & num53) ^ num48;
		uint num55 = num47 ^ num54;
		uint num56 = num51 ^ num54;
		uint num57 = num48 & num56;
		uint num58 = num57 ^ num55;
		uint num59 = num51 ^ num57;
		uint num60 = num52 & num59;
		uint num61 = num49 ^ num60;
		uint num62 = num61 ^ num58;
		uint num63 = num52 ^ num54;
		uint num64 = num52 ^ num61;
		uint num65 = num54 ^ num58;
		uint num66 = num63 ^ num62;
		uint num67 = num65 & num21;
		uint num68 = num58 & num23;
		uint num69 = num54 & num8;
		uint num70 = num64 & num29;
		uint num71 = num61 & num14;
		uint num72 = num52 & num26;
		uint num73 = num63 & num25;
		uint num74 = num66 & num27;
		uint num75 = num62 & num24;
		uint num76 = num65 & num16;
		uint num77 = num58 & num19;
		uint num78 = num54 & num15;
		uint num79 = num64 & num10;
		uint num80 = num61 & num18;
		uint num81 = num52 & num17;
		uint num82 = num63 & num11;
		uint num83 = num66 & num9;
		uint num84 = num62 & num12;
		uint num85 = num82 ^ num83;
		uint num86 = num77 ^ num78;
		uint num87 = num72 ^ num80;
		uint num88 = num76 ^ num77;
		uint num89 = num69 ^ num79;
		uint num90 = num69 ^ num72;
		uint num91 = num74 ^ num75;
		uint num92 = num67 ^ num70;
		uint num93 = num73 ^ num74;
		uint num94 = num83 ^ num84;
		uint num95 = num79 ^ num87;
		uint num96 = num89 ^ num92;
		uint num97 = num71 ^ num85;
		uint num98 = num70 ^ num93;
		uint num99 = num85 ^ num96;
		uint num100 = num81 ^ num96;
		uint num101 = num91 ^ num97;
		uint num102 = num88 ^ num97;
		uint num103 = num71 ^ num98;
		uint num104 = num100 ^ num101;
		uint num105 = num68 ^ num102;
		uint num106 = num98 ^ num102;
		uint num107 = num95 ^ ~num101;
		uint num108 = num87 ^ ~num99;
		uint num109 = num103 ^ num104;
		uint num110 = num92 ^ num105;
		uint num111 = num90 ^ num105;
		uint num112 = num86 ^ num104;
		uint num113 = num103 ^ ~num110;
		uint num114 = num94 ^ ~num109;
		q[7] = num106;
		q[6] = num113;
		q[5] = num114;
		q[4] = num110;
		q[3] = num111;
		q[2] = num112;
		q[1] = num107;
		q[0] = num108;
	}

	private static void ShiftRows32(uint[] q)
	{
		for (int i = 0; i < 8; i++)
		{
			uint x = Bits.BitPermuteStep(q[i], 202310400u, 4);
			q[i] = Bits.BitPermuteStep(x, 855651072u, 2);
		}
	}

	private static void MixColumns32(uint[] q)
	{
		uint num = q[0];
		uint num2 = Integers.RotateRight(num, 8);
		uint num3 = num ^ num2;
		uint num4 = q[1];
		uint num5 = Integers.RotateRight(num4, 8);
		uint num6 = num4 ^ num5;
		uint num7 = q[2];
		uint num8 = Integers.RotateRight(num7, 8);
		uint num9 = num7 ^ num8;
		uint num10 = q[3];
		uint num11 = Integers.RotateRight(num10, 8);
		uint num12 = num10 ^ num11;
		uint num13 = q[4];
		uint num14 = Integers.RotateRight(num13, 8);
		uint num15 = num13 ^ num14;
		uint num16 = q[5];
		uint num17 = Integers.RotateRight(num16, 8);
		uint num18 = num16 ^ num17;
		uint num19 = q[6];
		uint num20 = Integers.RotateRight(num19, 8);
		uint num21 = num19 ^ num20;
		uint num22 = q[7];
		uint num23 = Integers.RotateRight(num22, 8);
		uint num24 = num22 ^ num23;
		q[0] = num2 ^ num24 ^ Integers.RotateRight(num3, 16);
		q[1] = num5 ^ num3 ^ num24 ^ Integers.RotateRight(num6, 16);
		q[2] = num8 ^ num6 ^ Integers.RotateRight(num9, 16);
		q[3] = num11 ^ num9 ^ num24 ^ Integers.RotateRight(num12, 16);
		q[4] = num14 ^ num12 ^ num24 ^ Integers.RotateRight(num15, 16);
		q[5] = num17 ^ num15 ^ Integers.RotateRight(num18, 16);
		q[6] = num20 ^ num18 ^ Integers.RotateRight(num21, 16);
		q[7] = num23 ^ num21 ^ Integers.RotateRight(num24, 16);
	}

	private static void AddRoundKey32(uint[] q, uint[] sk)
	{
		q[0] ^= sk[0];
		q[1] ^= sk[1];
		q[2] ^= sk[2];
		q[3] ^= sk[3];
		q[4] ^= sk[4];
		q[5] ^= sk[5];
		q[6] ^= sk[6];
		q[7] ^= sk[7];
	}

	private static void BrAesCt64Ortho(ulong[] q)
	{
		ulong lo = q[0];
		ulong hi = q[1];
		ulong lo2 = q[2];
		ulong hi2 = q[3];
		ulong lo3 = q[4];
		ulong hi3 = q[5];
		ulong lo4 = q[6];
		ulong hi4 = q[7];
		Bits.BitPermuteStep2(ref hi, ref lo, 6148914691236517205uL, 1);
		Bits.BitPermuteStep2(ref hi2, ref lo2, 6148914691236517205uL, 1);
		Bits.BitPermuteStep2(ref hi3, ref lo3, 6148914691236517205uL, 1);
		Bits.BitPermuteStep2(ref hi4, ref lo4, 6148914691236517205uL, 1);
		Bits.BitPermuteStep2(ref lo2, ref lo, 3689348814741910323uL, 2);
		Bits.BitPermuteStep2(ref hi2, ref hi, 3689348814741910323uL, 2);
		Bits.BitPermuteStep2(ref lo4, ref lo3, 3689348814741910323uL, 2);
		Bits.BitPermuteStep2(ref hi4, ref hi3, 3689348814741910323uL, 2);
		Bits.BitPermuteStep2(ref lo3, ref lo, 1085102592571150095uL, 4);
		Bits.BitPermuteStep2(ref hi3, ref hi, 1085102592571150095uL, 4);
		Bits.BitPermuteStep2(ref lo4, ref lo2, 1085102592571150095uL, 4);
		Bits.BitPermuteStep2(ref hi4, ref hi2, 1085102592571150095uL, 4);
		q[0] = lo;
		q[1] = hi;
		q[2] = lo2;
		q[3] = hi2;
		q[4] = lo3;
		q[5] = hi3;
		q[6] = lo4;
		q[7] = hi4;
	}

	private static void BrAesCtOrtho(uint[] q)
	{
		uint lo = q[0];
		uint hi = q[1];
		uint lo2 = q[2];
		uint hi2 = q[3];
		uint lo3 = q[4];
		uint hi3 = q[5];
		uint lo4 = q[6];
		uint hi4 = q[7];
		Bits.BitPermuteStep2(ref hi, ref lo, 1431655765u, 1);
		Bits.BitPermuteStep2(ref hi2, ref lo2, 1431655765u, 1);
		Bits.BitPermuteStep2(ref hi3, ref lo3, 1431655765u, 1);
		Bits.BitPermuteStep2(ref hi4, ref lo4, 1431655765u, 1);
		Bits.BitPermuteStep2(ref lo2, ref lo, 858993459u, 2);
		Bits.BitPermuteStep2(ref hi2, ref hi, 858993459u, 2);
		Bits.BitPermuteStep2(ref lo4, ref lo3, 858993459u, 2);
		Bits.BitPermuteStep2(ref hi4, ref hi3, 858993459u, 2);
		Bits.BitPermuteStep2(ref lo3, ref lo, 252645135u, 4);
		Bits.BitPermuteStep2(ref hi3, ref hi, 252645135u, 4);
		Bits.BitPermuteStep2(ref lo4, ref lo2, 252645135u, 4);
		Bits.BitPermuteStep2(ref hi4, ref hi2, 252645135u, 4);
		q[0] = lo;
		q[1] = hi;
		q[2] = lo2;
		q[3] = hi2;
		q[4] = lo3;
		q[5] = hi3;
		q[6] = lo4;
		q[7] = hi4;
	}

	private static void BrAesCt64BitsliceSbox(ulong[] q)
	{
		ulong num = q[7];
		ulong num2 = q[6];
		ulong num3 = q[5];
		ulong num4 = q[4];
		ulong num5 = q[3];
		ulong num6 = q[2];
		ulong num7 = q[1];
		ulong num8 = q[0];
		ulong num9 = num4 ^ num6;
		ulong num10 = num ^ num7;
		ulong num11 = num ^ num4;
		ulong num12 = num ^ num6;
		ulong num13 = num2 ^ num3;
		ulong num14 = num13 ^ num8;
		ulong num15 = num14 ^ num4;
		ulong num16 = num10 ^ num9;
		ulong num17 = num14 ^ num;
		ulong num18 = num14 ^ num7;
		ulong num19 = num18 ^ num12;
		ulong num20 = num5 ^ num16;
		ulong num21 = num20 ^ num6;
		ulong num22 = num20 ^ num2;
		ulong num23 = num21 ^ num8;
		ulong num24 = num21 ^ num13;
		ulong num25 = num22 ^ num11;
		ulong num26 = num8 ^ num25;
		ulong num27 = num24 ^ num25;
		ulong num28 = num24 ^ num12;
		ulong num29 = num13 ^ num25;
		ulong num30 = num10 ^ num29;
		ulong num31 = num ^ num29;
		ulong num32 = num16 & num21;
		ulong num33 = (num19 & num23) ^ num32;
		ulong num34 = (num15 & num8) ^ num32;
		ulong num35 = num10 & num29;
		ulong num36 = (num18 & num14) ^ num35;
		ulong num37 = (num17 & num26) ^ num35;
		ulong num38 = num11 & num25;
		ulong num39 = (num9 & num27) ^ num38;
		ulong num40 = (num12 & num24) ^ num38;
		ulong num41 = num33 ^ num39;
		ulong num42 = num34 ^ num40;
		ulong num43 = num36 ^ num39;
		ulong num44 = num37 ^ num40;
		ulong num45 = num41 ^ num22;
		ulong num46 = num42 ^ num28;
		ulong num47 = num43 ^ num30;
		ulong num48 = num44 ^ num31;
		ulong num49 = num45 ^ num46;
		ulong num50 = num45 & num47;
		ulong num51 = num48 ^ num50;
		ulong num52 = (num49 & num51) ^ num46;
		ulong num53 = num47 ^ num48;
		ulong num54 = ((num46 ^ num50) & num53) ^ num48;
		ulong num55 = num47 ^ num54;
		ulong num56 = num51 ^ num54;
		ulong num57 = num48 & num56;
		ulong num58 = num57 ^ num55;
		ulong num59 = num51 ^ num57;
		ulong num60 = num52 & num59;
		ulong num61 = num49 ^ num60;
		ulong num62 = num61 ^ num58;
		ulong num63 = num52 ^ num54;
		ulong num64 = num52 ^ num61;
		ulong num65 = num54 ^ num58;
		ulong num66 = num63 ^ num62;
		ulong num67 = num65 & num21;
		ulong num68 = num58 & num23;
		ulong num69 = num54 & num8;
		ulong num70 = num64 & num29;
		ulong num71 = num61 & num14;
		ulong num72 = num52 & num26;
		ulong num73 = num63 & num25;
		ulong num74 = num66 & num27;
		ulong num75 = num62 & num24;
		ulong num76 = num65 & num16;
		ulong num77 = num58 & num19;
		ulong num78 = num54 & num15;
		ulong num79 = num64 & num10;
		ulong num80 = num61 & num18;
		ulong num81 = num52 & num17;
		ulong num82 = num63 & num11;
		ulong num83 = num66 & num9;
		ulong num84 = num62 & num12;
		ulong num85 = num82 ^ num83;
		ulong num86 = num77 ^ num78;
		ulong num87 = num72 ^ num80;
		ulong num88 = num76 ^ num77;
		ulong num89 = num69 ^ num79;
		ulong num90 = num69 ^ num72;
		ulong num91 = num74 ^ num75;
		ulong num92 = num67 ^ num70;
		ulong num93 = num73 ^ num74;
		ulong num94 = num83 ^ num84;
		ulong num95 = num79 ^ num87;
		ulong num96 = num89 ^ num92;
		ulong num97 = num71 ^ num85;
		ulong num98 = num70 ^ num93;
		ulong num99 = num85 ^ num96;
		ulong num100 = num81 ^ num96;
		ulong num101 = num91 ^ num97;
		ulong num102 = num88 ^ num97;
		ulong num103 = num71 ^ num98;
		ulong num104 = num100 ^ num101;
		ulong num105 = num68 ^ num102;
		ulong num106 = num98 ^ num102;
		ulong num107 = num95 ^ ~num101;
		ulong num108 = num87 ^ ~num99;
		ulong num109 = num103 ^ num104;
		ulong num110 = num92 ^ num105;
		ulong num111 = num90 ^ num105;
		ulong num112 = num86 ^ num104;
		ulong num113 = num103 ^ ~num110;
		ulong num114 = num94 ^ ~num109;
		q[7] = num106;
		q[6] = num113;
		q[5] = num114;
		q[4] = num110;
		q[3] = num111;
		q[2] = num112;
		q[1] = num107;
		q[0] = num108;
	}

	private static void ShiftRows(ulong[] q)
	{
		for (int i = 0; i < 8; i++)
		{
			ulong x = Bits.BitPermuteStep(q[i], 67555089628200960uL, 8);
			q[i] = Bits.BitPermuteStep(x, 1085086035472220160uL, 4);
		}
	}

	private static void MixColumns(ulong[] q)
	{
		ulong num = q[0];
		ulong num2 = Longs.RotateRight(num, 16);
		ulong num3 = num ^ num2;
		ulong num4 = q[1];
		ulong num5 = Longs.RotateRight(num4, 16);
		ulong num6 = num4 ^ num5;
		ulong num7 = q[2];
		ulong num8 = Longs.RotateRight(num7, 16);
		ulong num9 = num7 ^ num8;
		ulong num10 = q[3];
		ulong num11 = Longs.RotateRight(num10, 16);
		ulong num12 = num10 ^ num11;
		ulong num13 = q[4];
		ulong num14 = Longs.RotateRight(num13, 16);
		ulong num15 = num13 ^ num14;
		ulong num16 = q[5];
		ulong num17 = Longs.RotateRight(num16, 16);
		ulong num18 = num16 ^ num17;
		ulong num19 = q[6];
		ulong num20 = Longs.RotateRight(num19, 16);
		ulong num21 = num19 ^ num20;
		ulong num22 = q[7];
		ulong num23 = Longs.RotateRight(num22, 16);
		ulong num24 = num22 ^ num23;
		q[0] = num2 ^ num24 ^ Longs.RotateRight(num3, 32);
		q[1] = num5 ^ num3 ^ num24 ^ Longs.RotateRight(num6, 32);
		q[2] = num8 ^ num6 ^ Longs.RotateRight(num9, 32);
		q[3] = num11 ^ num9 ^ num24 ^ Longs.RotateRight(num12, 32);
		q[4] = num14 ^ num12 ^ num24 ^ Longs.RotateRight(num15, 32);
		q[5] = num17 ^ num15 ^ Longs.RotateRight(num18, 32);
		q[6] = num20 ^ num18 ^ Longs.RotateRight(num21, 32);
		q[7] = num23 ^ num21 ^ Longs.RotateRight(num24, 32);
	}

	private static void AddRoundKey(ulong[] q, ulong[] sk)
	{
		q[0] ^= sk[0];
		q[1] ^= sk[1];
		q[2] ^= sk[2];
		q[3] ^= sk[3];
		q[4] ^= sk[4];
		q[5] ^= sk[5];
		q[6] ^= sk[6];
		q[7] ^= sk[7];
	}

	private static void BrAesCt64InterleaveOut(uint[] w, ulong[] q, int pos)
	{
		ulong num = q[pos] & 0xFF00FF00FF00FFL;
		ulong num2 = q[pos + 4] & 0xFF00FF00FF00FFL;
		ulong num3 = (q[pos] >> 8) & 0xFF00FF00FF00FFL;
		ulong num4 = (q[pos + 4] >> 8) & 0xFF00FF00FF00FFL;
		num |= num >> 8;
		num2 |= num2 >> 8;
		num3 |= num3 >> 8;
		num4 |= num4 >> 8;
		num &= 0xFFFF0000FFFFL;
		num2 &= 0xFFFF0000FFFFL;
		num3 &= 0xFFFF0000FFFFL;
		num4 &= 0xFFFF0000FFFFL;
		pos <<= 2;
		w[pos] = (uint)(num | (num >> 16));
		w[pos + 1] = (uint)(num2 | (num2 >> 16));
		w[pos + 2] = (uint)(num3 | (num3 >> 16));
		w[pos + 3] = (uint)(num4 | (num4 >> 16));
	}

	protected static void Xor(byte[] x, int xOff, byte[] y, int yOff, byte[] z, int zOff, int zLen)
	{
		for (int i = 0; i < zLen; i++)
		{
			z[zOff + i] = (byte)(x[xOff + i] ^ y[yOff + i]);
		}
	}
}
