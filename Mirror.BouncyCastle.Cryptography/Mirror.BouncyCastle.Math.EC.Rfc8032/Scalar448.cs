using System;
using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Math.EC.Rfc8032;

internal static class Scalar448
{
	internal const int Size = 14;

	private const int ScalarBytes = 57;

	private const ulong M26UL = 67108863uL;

	private const ulong M28UL = 268435455uL;

	private const int TargetLength = 447;

	private static readonly uint[] L = new uint[14]
	{
		2874688755u, 595116690u, 2378534741u, 560775794u, 2933274256u, 3293502281u, 2093622249u, 4294967295u, 4294967295u, 4294967295u,
		4294967295u, 4294967295u, 4294967295u, 1073741823u
	};

	private static readonly uint[] LSq = new uint[28]
	{
		463601321u, 3249404856u, 1239460018u, 3105617207u, 3882145813u, 1160071467u, 2729996653u, 1256291574u, 3124512708u, 4054436884u,
		2118977290u, 2449812427u, 2676112242u, 3275762323u, 1437344377u, 2445041993u, 1189267370u, 280387897u, 3614120776u, 3794234788u,
		3194294772u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 268435455u
	};

	private const int L_0 = 78101261;

	private const int L_1 = 141809365;

	private const int L_2 = 175155932;

	private const int L_3 = 64542499;

	private const int L_4 = 158326419;

	private const int L_5 = 191173276;

	private const int L_6 = 104575268;

	private const int L_7 = 137584065;

	private const int L4_0 = 43969588;

	private const int L4_1 = 30366549;

	private const int L4_2 = 163752818;

	private const int L4_3 = 258169998;

	private const int L4_4 = 96434764;

	private const int L4_5 = 227822194;

	private const int L4_6 = 149865618;

	private const int L4_7 = 550336261;

	internal static bool CheckVar(byte[] s, uint[] n)
	{
		if (s[56] != 0)
		{
			return false;
		}
		Decode(s, n);
		return !Nat.Gte(14, n, L);
	}

	internal static void Decode(byte[] k, uint[] n)
	{
		Codec.Decode32(k, 0, n, 0, 14);
	}

	internal static void GetOrderWnafVar(int width, sbyte[] ws)
	{
		Wnaf.GetSignedVar(L, width, ws);
	}

	internal static void Multiply225Var(uint[] x, uint[] y225, uint[] z)
	{
		uint[] array = new uint[22];
		Nat.Mul(y225, 0, 8, x, 0, 14, array, 0);
		if ((int)y225[7] < 0)
		{
			Nat.AddTo(14, L, 0, array, 8);
			Nat.SubFrom(14, x, 0, array, 8);
		}
		byte[] array2 = new byte[88];
		Codec.Encode32(array, 0, 22, array2, 0);
		Decode(Reduce704(array2), z);
	}

	internal static byte[] Reduce704(byte[] n)
	{
		byte[] array = new byte[57];
		ulong num = Codec.Decode32(n, 0);
		ulong num2 = Codec.Decode24(n, 4) << 4;
		ulong num3 = Codec.Decode32(n, 7);
		ulong num4 = Codec.Decode24(n, 11) << 4;
		ulong num5 = Codec.Decode32(n, 14);
		ulong num6 = Codec.Decode24(n, 18) << 4;
		ulong num7 = Codec.Decode32(n, 21);
		ulong num8 = Codec.Decode24(n, 25) << 4;
		ulong num9 = Codec.Decode32(n, 28);
		ulong num10 = Codec.Decode24(n, 32) << 4;
		ulong num11 = Codec.Decode32(n, 35);
		ulong num12 = Codec.Decode24(n, 39) << 4;
		ulong num13 = Codec.Decode32(n, 42);
		ulong num14 = Codec.Decode24(n, 46) << 4;
		ulong num15 = Codec.Decode32(n, 49);
		ulong num16 = Codec.Decode24(n, 53) << 4;
		ulong num17 = Codec.Decode32(n, 56);
		ulong num18 = Codec.Decode24(n, 60) << 4;
		ulong num19 = Codec.Decode32(n, 63);
		ulong num20 = Codec.Decode24(n, 67) << 4;
		ulong num21 = Codec.Decode32(n, 70);
		ulong num22 = Codec.Decode24(n, 74) << 4;
		ulong num23 = Codec.Decode32(n, 77);
		ulong num24 = Codec.Decode24(n, 81) << 4;
		ulong num25 = Codec.Decode32(n, 84);
		ulong num26 = 0uL;
		num26 += num25 >> 28;
		num25 &= 0xFFFFFFF;
		num10 += num26 * 43969588;
		num11 += num26 * 30366549;
		num12 += num26 * 163752818;
		num13 += num26 * 258169998;
		num14 += num26 * 96434764;
		num15 += num26 * 227822194;
		num16 += num26 * 149865618;
		num17 += num26 * 550336261;
		num22 += num21 >> 28;
		num21 &= 0xFFFFFFF;
		num23 += num22 >> 28;
		num22 &= 0xFFFFFFF;
		num24 += num23 >> 28;
		num23 &= 0xFFFFFFF;
		num25 += num24 >> 28;
		num24 &= 0xFFFFFFF;
		num9 += num25 * 43969588;
		num10 += num25 * 30366549;
		num11 += num25 * 163752818;
		num12 += num25 * 258169998;
		num13 += num25 * 96434764;
		num14 += num25 * 227822194;
		num15 += num25 * 149865618;
		num16 += num25 * 550336261;
		num8 += num24 * 43969588;
		num9 += num24 * 30366549;
		num10 += num24 * 163752818;
		num11 += num24 * 258169998;
		num12 += num24 * 96434764;
		num13 += num24 * 227822194;
		num14 += num24 * 149865618;
		num15 += num24 * 550336261;
		num7 += num23 * 43969588;
		num8 += num23 * 30366549;
		num9 += num23 * 163752818;
		num10 += num23 * 258169998;
		num11 += num23 * 96434764;
		num12 += num23 * 227822194;
		num13 += num23 * 149865618;
		num14 += num23 * 550336261;
		num19 += num18 >> 28;
		num18 &= 0xFFFFFFF;
		num20 += num19 >> 28;
		num19 &= 0xFFFFFFF;
		num21 += num20 >> 28;
		num20 &= 0xFFFFFFF;
		num22 += num21 >> 28;
		num21 &= 0xFFFFFFF;
		num6 += num22 * 43969588;
		num7 += num22 * 30366549;
		num8 += num22 * 163752818;
		num9 += num22 * 258169998;
		num10 += num22 * 96434764;
		num11 += num22 * 227822194;
		num12 += num22 * 149865618;
		num13 += num22 * 550336261;
		num5 += num21 * 43969588;
		num6 += num21 * 30366549;
		num7 += num21 * 163752818;
		num8 += num21 * 258169998;
		num9 += num21 * 96434764;
		num10 += num21 * 227822194;
		num11 += num21 * 149865618;
		num12 += num21 * 550336261;
		num4 += num20 * 43969588;
		num5 += num20 * 30366549;
		num6 += num20 * 163752818;
		num7 += num20 * 258169998;
		num8 += num20 * 96434764;
		num9 += num20 * 227822194;
		num10 += num20 * 149865618;
		num11 += num20 * 550336261;
		num16 += num15 >> 28;
		num15 &= 0xFFFFFFF;
		num17 += num16 >> 28;
		num16 &= 0xFFFFFFF;
		num18 += num17 >> 28;
		num17 &= 0xFFFFFFF;
		num19 += num18 >> 28;
		num18 &= 0xFFFFFFF;
		num3 += num19 * 43969588;
		num4 += num19 * 30366549;
		num5 += num19 * 163752818;
		num6 += num19 * 258169998;
		num7 += num19 * 96434764;
		num8 += num19 * 227822194;
		num9 += num19 * 149865618;
		num10 += num19 * 550336261;
		num2 += num18 * 43969588;
		num3 += num18 * 30366549;
		num4 += num18 * 163752818;
		num5 += num18 * 258169998;
		num6 += num18 * 96434764;
		num7 += num18 * 227822194;
		num8 += num18 * 149865618;
		num9 += num18 * 550336261;
		num17 *= 4;
		num17 += num16 >> 26;
		num16 &= 0x3FFFFFF;
		num17++;
		num += num17 * 78101261;
		num2 += num17 * 141809365;
		num3 += num17 * 175155932;
		num4 += num17 * 64542499;
		num5 += num17 * 158326419;
		num6 += num17 * 191173276;
		num7 += num17 * 104575268;
		num8 += num17 * 137584065;
		num2 += num >> 28;
		num &= 0xFFFFFFF;
		num3 += num2 >> 28;
		num2 &= 0xFFFFFFF;
		num4 += num3 >> 28;
		num3 &= 0xFFFFFFF;
		num5 += num4 >> 28;
		num4 &= 0xFFFFFFF;
		num6 += num5 >> 28;
		num5 &= 0xFFFFFFF;
		num7 += num6 >> 28;
		num6 &= 0xFFFFFFF;
		num8 += num7 >> 28;
		num7 &= 0xFFFFFFF;
		num9 += num8 >> 28;
		num8 &= 0xFFFFFFF;
		num10 += num9 >> 28;
		num9 &= 0xFFFFFFF;
		num11 += num10 >> 28;
		num10 &= 0xFFFFFFF;
		num12 += num11 >> 28;
		num11 &= 0xFFFFFFF;
		num13 += num12 >> 28;
		num12 &= 0xFFFFFFF;
		num14 += num13 >> 28;
		num13 &= 0xFFFFFFF;
		num15 += num14 >> 28;
		num14 &= 0xFFFFFFF;
		num16 += num15 >> 28;
		num15 &= 0xFFFFFFF;
		num17 = num16 >> 26;
		num16 &= 0x3FFFFFF;
		num17--;
		num -= num17 & 0x4A7BB0D;
		num2 -= num17 & 0x873D6D5;
		num3 -= num17 & 0xA70AADC;
		num4 -= num17 & 0x3D8D723;
		num5 -= num17 & 0x96FDE93;
		num6 -= num17 & 0xB65129C;
		num7 -= num17 & 0x63BB124;
		num8 -= num17 & 0x8335DC1;
		num2 += (ulong)((long)num >> 28);
		num &= 0xFFFFFFF;
		num3 += (ulong)((long)num2 >> 28);
		num2 &= 0xFFFFFFF;
		num4 += (ulong)((long)num3 >> 28);
		num3 &= 0xFFFFFFF;
		num5 += (ulong)((long)num4 >> 28);
		num4 &= 0xFFFFFFF;
		num6 += (ulong)((long)num5 >> 28);
		num5 &= 0xFFFFFFF;
		num7 += (ulong)((long)num6 >> 28);
		num6 &= 0xFFFFFFF;
		num8 += (ulong)((long)num7 >> 28);
		num7 &= 0xFFFFFFF;
		num9 += (ulong)((long)num8 >> 28);
		num8 &= 0xFFFFFFF;
		num10 += (ulong)((long)num9 >> 28);
		num9 &= 0xFFFFFFF;
		num11 += (ulong)((long)num10 >> 28);
		num10 &= 0xFFFFFFF;
		num12 += (ulong)((long)num11 >> 28);
		num11 &= 0xFFFFFFF;
		num13 += (ulong)((long)num12 >> 28);
		num12 &= 0xFFFFFFF;
		num14 += (ulong)((long)num13 >> 28);
		num13 &= 0xFFFFFFF;
		num15 += (ulong)((long)num14 >> 28);
		num14 &= 0xFFFFFFF;
		num16 += (ulong)((long)num15 >> 28);
		num15 &= 0xFFFFFFF;
		Codec.Encode56(num | (num2 << 28), array, 0);
		Codec.Encode56(num3 | (num4 << 28), array, 7);
		Codec.Encode56(num5 | (num6 << 28), array, 14);
		Codec.Encode56(num7 | (num8 << 28), array, 21);
		Codec.Encode56(num9 | (num10 << 28), array, 28);
		Codec.Encode56(num11 | (num12 << 28), array, 35);
		Codec.Encode56(num13 | (num14 << 28), array, 42);
		Codec.Encode56(num15 | (num16 << 28), array, 49);
		return array;
	}

	internal static byte[] Reduce912(byte[] n)
	{
		byte[] array = new byte[57];
		ulong num = Codec.Decode32(n, 0);
		ulong num2 = Codec.Decode24(n, 4) << 4;
		ulong num3 = Codec.Decode32(n, 7);
		ulong num4 = Codec.Decode24(n, 11) << 4;
		ulong num5 = Codec.Decode32(n, 14);
		ulong num6 = Codec.Decode24(n, 18) << 4;
		ulong num7 = Codec.Decode32(n, 21);
		ulong num8 = Codec.Decode24(n, 25) << 4;
		ulong num9 = Codec.Decode32(n, 28);
		ulong num10 = Codec.Decode24(n, 32) << 4;
		ulong num11 = Codec.Decode32(n, 35);
		ulong num12 = Codec.Decode24(n, 39) << 4;
		ulong num13 = Codec.Decode32(n, 42);
		ulong num14 = Codec.Decode24(n, 46) << 4;
		ulong num15 = Codec.Decode32(n, 49);
		ulong num16 = Codec.Decode24(n, 53) << 4;
		ulong num17 = Codec.Decode32(n, 56);
		ulong num18 = Codec.Decode24(n, 60) << 4;
		ulong num19 = Codec.Decode32(n, 63);
		ulong num20 = Codec.Decode24(n, 67) << 4;
		ulong num21 = Codec.Decode32(n, 70);
		ulong num22 = Codec.Decode24(n, 74) << 4;
		ulong num23 = Codec.Decode32(n, 77);
		ulong num24 = Codec.Decode24(n, 81) << 4;
		ulong num25 = Codec.Decode32(n, 84);
		ulong num26 = Codec.Decode24(n, 88) << 4;
		ulong num27 = Codec.Decode32(n, 91);
		ulong num28 = Codec.Decode24(n, 95) << 4;
		ulong num29 = Codec.Decode32(n, 98);
		ulong num30 = Codec.Decode24(n, 102) << 4;
		ulong num31 = Codec.Decode32(n, 105);
		ulong num32 = Codec.Decode24(n, 109) << 4;
		ulong num33 = Codec.Decode16(n, 112);
		num17 += num33 * 43969588;
		num18 += num33 * 30366549;
		num19 += num33 * 163752818;
		num20 += num33 * 258169998;
		num21 += num33 * 96434764;
		num22 += num33 * 227822194;
		num23 += num33 * 149865618;
		num24 += num33 * 550336261;
		num32 += num31 >> 28;
		num31 &= 0xFFFFFFF;
		num16 += num32 * 43969588;
		num17 += num32 * 30366549;
		num18 += num32 * 163752818;
		num19 += num32 * 258169998;
		num20 += num32 * 96434764;
		num21 += num32 * 227822194;
		num22 += num32 * 149865618;
		num23 += num32 * 550336261;
		num15 += num31 * 43969588;
		num16 += num31 * 30366549;
		num17 += num31 * 163752818;
		num18 += num31 * 258169998;
		num19 += num31 * 96434764;
		num20 += num31 * 227822194;
		num21 += num31 * 149865618;
		num22 += num31 * 550336261;
		num30 += num29 >> 28;
		num29 &= 0xFFFFFFF;
		num14 += num30 * 43969588;
		num15 += num30 * 30366549;
		num16 += num30 * 163752818;
		num17 += num30 * 258169998;
		num18 += num30 * 96434764;
		num19 += num30 * 227822194;
		num20 += num30 * 149865618;
		num21 += num30 * 550336261;
		num13 += num29 * 43969588;
		num14 += num29 * 30366549;
		num15 += num29 * 163752818;
		num16 += num29 * 258169998;
		num17 += num29 * 96434764;
		num18 += num29 * 227822194;
		num19 += num29 * 149865618;
		num20 += num29 * 550336261;
		num28 += num27 >> 28;
		num27 &= 0xFFFFFFF;
		num12 += num28 * 43969588;
		num13 += num28 * 30366549;
		num14 += num28 * 163752818;
		num15 += num28 * 258169998;
		num16 += num28 * 96434764;
		num17 += num28 * 227822194;
		num18 += num28 * 149865618;
		num19 += num28 * 550336261;
		num11 += num27 * 43969588;
		num12 += num27 * 30366549;
		num13 += num27 * 163752818;
		num14 += num27 * 258169998;
		num15 += num27 * 96434764;
		num16 += num27 * 227822194;
		num17 += num27 * 149865618;
		num18 += num27 * 550336261;
		num26 += num25 >> 28;
		num25 &= 0xFFFFFFF;
		num10 += num26 * 43969588;
		num11 += num26 * 30366549;
		num12 += num26 * 163752818;
		num13 += num26 * 258169998;
		num14 += num26 * 96434764;
		num15 += num26 * 227822194;
		num16 += num26 * 149865618;
		num17 += num26 * 550336261;
		num22 += num21 >> 28;
		num21 &= 0xFFFFFFF;
		num23 += num22 >> 28;
		num22 &= 0xFFFFFFF;
		num24 += num23 >> 28;
		num23 &= 0xFFFFFFF;
		num25 += num24 >> 28;
		num24 &= 0xFFFFFFF;
		num9 += num25 * 43969588;
		num10 += num25 * 30366549;
		num11 += num25 * 163752818;
		num12 += num25 * 258169998;
		num13 += num25 * 96434764;
		num14 += num25 * 227822194;
		num15 += num25 * 149865618;
		num16 += num25 * 550336261;
		num8 += num24 * 43969588;
		num9 += num24 * 30366549;
		num10 += num24 * 163752818;
		num11 += num24 * 258169998;
		num12 += num24 * 96434764;
		num13 += num24 * 227822194;
		num14 += num24 * 149865618;
		num15 += num24 * 550336261;
		num7 += num23 * 43969588;
		num8 += num23 * 30366549;
		num9 += num23 * 163752818;
		num10 += num23 * 258169998;
		num11 += num23 * 96434764;
		num12 += num23 * 227822194;
		num13 += num23 * 149865618;
		num14 += num23 * 550336261;
		num19 += num18 >> 28;
		num18 &= 0xFFFFFFF;
		num20 += num19 >> 28;
		num19 &= 0xFFFFFFF;
		num21 += num20 >> 28;
		num20 &= 0xFFFFFFF;
		num22 += num21 >> 28;
		num21 &= 0xFFFFFFF;
		num6 += num22 * 43969588;
		num7 += num22 * 30366549;
		num8 += num22 * 163752818;
		num9 += num22 * 258169998;
		num10 += num22 * 96434764;
		num11 += num22 * 227822194;
		num12 += num22 * 149865618;
		num13 += num22 * 550336261;
		num5 += num21 * 43969588;
		num6 += num21 * 30366549;
		num7 += num21 * 163752818;
		num8 += num21 * 258169998;
		num9 += num21 * 96434764;
		num10 += num21 * 227822194;
		num11 += num21 * 149865618;
		num12 += num21 * 550336261;
		num4 += num20 * 43969588;
		num5 += num20 * 30366549;
		num6 += num20 * 163752818;
		num7 += num20 * 258169998;
		num8 += num20 * 96434764;
		num9 += num20 * 227822194;
		num10 += num20 * 149865618;
		num11 += num20 * 550336261;
		num16 += num15 >> 28;
		num15 &= 0xFFFFFFF;
		num17 += num16 >> 28;
		num16 &= 0xFFFFFFF;
		num18 += num17 >> 28;
		num17 &= 0xFFFFFFF;
		num19 += num18 >> 28;
		num18 &= 0xFFFFFFF;
		num3 += num19 * 43969588;
		num4 += num19 * 30366549;
		num5 += num19 * 163752818;
		num6 += num19 * 258169998;
		num7 += num19 * 96434764;
		num8 += num19 * 227822194;
		num9 += num19 * 149865618;
		num10 += num19 * 550336261;
		num2 += num18 * 43969588;
		num3 += num18 * 30366549;
		num4 += num18 * 163752818;
		num5 += num18 * 258169998;
		num6 += num18 * 96434764;
		num7 += num18 * 227822194;
		num8 += num18 * 149865618;
		num9 += num18 * 550336261;
		num17 *= 4;
		num17 += num16 >> 26;
		num16 &= 0x3FFFFFF;
		num17++;
		num += num17 * 78101261;
		num2 += num17 * 141809365;
		num3 += num17 * 175155932;
		num4 += num17 * 64542499;
		num5 += num17 * 158326419;
		num6 += num17 * 191173276;
		num7 += num17 * 104575268;
		num8 += num17 * 137584065;
		num2 += num >> 28;
		num &= 0xFFFFFFF;
		num3 += num2 >> 28;
		num2 &= 0xFFFFFFF;
		num4 += num3 >> 28;
		num3 &= 0xFFFFFFF;
		num5 += num4 >> 28;
		num4 &= 0xFFFFFFF;
		num6 += num5 >> 28;
		num5 &= 0xFFFFFFF;
		num7 += num6 >> 28;
		num6 &= 0xFFFFFFF;
		num8 += num7 >> 28;
		num7 &= 0xFFFFFFF;
		num9 += num8 >> 28;
		num8 &= 0xFFFFFFF;
		num10 += num9 >> 28;
		num9 &= 0xFFFFFFF;
		num11 += num10 >> 28;
		num10 &= 0xFFFFFFF;
		num12 += num11 >> 28;
		num11 &= 0xFFFFFFF;
		num13 += num12 >> 28;
		num12 &= 0xFFFFFFF;
		num14 += num13 >> 28;
		num13 &= 0xFFFFFFF;
		num15 += num14 >> 28;
		num14 &= 0xFFFFFFF;
		num16 += num15 >> 28;
		num15 &= 0xFFFFFFF;
		num17 = num16 >> 26;
		num16 &= 0x3FFFFFF;
		num17--;
		num -= num17 & 0x4A7BB0D;
		num2 -= num17 & 0x873D6D5;
		num3 -= num17 & 0xA70AADC;
		num4 -= num17 & 0x3D8D723;
		num5 -= num17 & 0x96FDE93;
		num6 -= num17 & 0xB65129C;
		num7 -= num17 & 0x63BB124;
		num8 -= num17 & 0x8335DC1;
		num2 += (ulong)((long)num >> 28);
		num &= 0xFFFFFFF;
		num3 += (ulong)((long)num2 >> 28);
		num2 &= 0xFFFFFFF;
		num4 += (ulong)((long)num3 >> 28);
		num3 &= 0xFFFFFFF;
		num5 += (ulong)((long)num4 >> 28);
		num4 &= 0xFFFFFFF;
		num6 += (ulong)((long)num5 >> 28);
		num5 &= 0xFFFFFFF;
		num7 += (ulong)((long)num6 >> 28);
		num6 &= 0xFFFFFFF;
		num8 += (ulong)((long)num7 >> 28);
		num7 &= 0xFFFFFFF;
		num9 += (ulong)((long)num8 >> 28);
		num8 &= 0xFFFFFFF;
		num10 += (ulong)((long)num9 >> 28);
		num9 &= 0xFFFFFFF;
		num11 += (ulong)((long)num10 >> 28);
		num10 &= 0xFFFFFFF;
		num12 += (ulong)((long)num11 >> 28);
		num11 &= 0xFFFFFFF;
		num13 += (ulong)((long)num12 >> 28);
		num12 &= 0xFFFFFFF;
		num14 += (ulong)((long)num13 >> 28);
		num13 &= 0xFFFFFFF;
		num15 += (ulong)((long)num14 >> 28);
		num14 &= 0xFFFFFFF;
		num16 += (ulong)((long)num15 >> 28);
		num15 &= 0xFFFFFFF;
		Codec.Encode56(num | (num2 << 28), array, 0);
		Codec.Encode56(num3 | (num4 << 28), array, 7);
		Codec.Encode56(num5 | (num6 << 28), array, 14);
		Codec.Encode56(num7 | (num8 << 28), array, 21);
		Codec.Encode56(num9 | (num10 << 28), array, 28);
		Codec.Encode56(num11 | (num12 << 28), array, 35);
		Codec.Encode56(num13 | (num14 << 28), array, 42);
		Codec.Encode56(num15 | (num16 << 28), array, 49);
		return array;
	}

	internal static bool ReduceBasisVar(uint[] k, uint[] z0, uint[] z1)
	{
		uint[] x = new uint[28];
		Array.Copy(LSq, x, 28);
		uint[] y = new uint[28];
		Nat448.Square(k, y);
		y[0]++;
		uint[] array = new uint[28];
		Nat448.Mul(L, k, array);
		uint[] t = new uint[28];
		uint[] x2 = new uint[8];
		Array.Copy(L, x2, 8);
		uint[] x3 = new uint[8];
		uint[] y2 = new uint[8];
		Array.Copy(k, y2, 8);
		uint[] y3 = new uint[8] { 1u, 0u, 0u, 0u, 0u, 0u, 0u, 0u };
		int num = 1788;
		int num2 = 27;
		int bitLengthPositive = ScalarUtilities.GetBitLengthPositive(num2, y);
		while (bitLengthPositive > 447)
		{
			if (--num < 0)
			{
				return false;
			}
			int num3 = ScalarUtilities.GetBitLength(num2, array) - bitLengthPositive;
			num3 &= ~(num3 >> 31);
			if ((int)array[num2] < 0)
			{
				ScalarUtilities.AddShifted_NP(num2, num3, x, y, array, t);
				ScalarUtilities.AddShifted_UV(7, num3, x2, x3, y2, y3);
			}
			else
			{
				ScalarUtilities.SubShifted_NP(num2, num3, x, y, array, t);
				ScalarUtilities.SubShifted_UV(7, num3, x2, x3, y2, y3);
			}
			if (ScalarUtilities.LessThan(num2, x, y))
			{
				ScalarUtilities.Swap(ref x2, ref y2);
				ScalarUtilities.Swap(ref x3, ref y3);
				ScalarUtilities.Swap(ref x, ref y);
				num2 = bitLengthPositive >> 5;
				bitLengthPositive = ScalarUtilities.GetBitLengthPositive(num2, y);
			}
		}
		Array.Copy(y2, z0, 8);
		Array.Copy(y3, z1, 8);
		return true;
	}

	internal static void ToSignedDigits(int bits, uint[] x, uint[] z)
	{
		z[14] = (uint)(1 << bits - 448) + Nat.CAdd(14, (int)(~x[0] & 1), x, L, z);
		Nat.ShiftDownBit(15, z, 0u);
	}
}
