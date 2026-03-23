using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Utilities.Bzip2;

public class CBZip2OutputStream : BaseOutputStream
{
	internal class StackElem
	{
		internal int ll;

		internal int hh;

		internal int dd;
	}

	protected const int SETMASK = 2097152;

	protected const int CLEARMASK = -2097153;

	protected const int GREATER_ICOST = 15;

	protected const int LESSER_ICOST = 0;

	protected const int SMALL_THRESH = 20;

	protected const int DEPTH_THRESH = 10;

	internal static readonly ushort[] RNums = new ushort[512]
	{
		619, 720, 127, 481, 931, 816, 813, 233, 566, 247,
		985, 724, 205, 454, 863, 491, 741, 242, 949, 214,
		733, 859, 335, 708, 621, 574, 73, 654, 730, 472,
		419, 436, 278, 496, 867, 210, 399, 680, 480, 51,
		878, 465, 811, 169, 869, 675, 611, 697, 867, 561,
		862, 687, 507, 283, 482, 129, 807, 591, 733, 623,
		150, 238, 59, 379, 684, 877, 625, 169, 643, 105,
		170, 607, 520, 932, 727, 476, 693, 425, 174, 647,
		73, 122, 335, 530, 442, 853, 695, 249, 445, 515,
		909, 545, 703, 919, 874, 474, 882, 500, 594, 612,
		641, 801, 220, 162, 819, 984, 589, 513, 495, 799,
		161, 604, 958, 533, 221, 400, 386, 867, 600, 782,
		382, 596, 414, 171, 516, 375, 682, 485, 911, 276,
		98, 553, 163, 354, 666, 933, 424, 341, 533, 870,
		227, 730, 475, 186, 263, 647, 537, 686, 600, 224,
		469, 68, 770, 919, 190, 373, 294, 822, 808, 206,
		184, 943, 795, 384, 383, 461, 404, 758, 839, 887,
		715, 67, 618, 276, 204, 918, 873, 777, 604, 560,
		951, 160, 578, 722, 79, 804, 96, 409, 713, 940,
		652, 934, 970, 447, 318, 353, 859, 672, 112, 785,
		645, 863, 803, 350, 139, 93, 354, 99, 820, 908,
		609, 772, 154, 274, 580, 184, 79, 626, 630, 742,
		653, 282, 762, 623, 680, 81, 927, 626, 789, 125,
		411, 521, 938, 300, 821, 78, 343, 175, 128, 250,
		170, 774, 972, 275, 999, 639, 495, 78, 352, 126,
		857, 956, 358, 619, 580, 124, 737, 594, 701, 612,
		669, 112, 134, 694, 363, 992, 809, 743, 168, 974,
		944, 375, 748, 52, 600, 747, 642, 182, 862, 81,
		344, 805, 988, 739, 511, 655, 814, 334, 249, 515,
		897, 955, 664, 981, 649, 113, 974, 459, 893, 228,
		433, 837, 553, 268, 926, 240, 102, 654, 459, 51,
		686, 754, 806, 760, 493, 403, 415, 394, 687, 700,
		946, 670, 656, 610, 738, 392, 760, 799, 887, 653,
		978, 321, 576, 617, 626, 502, 894, 679, 243, 440,
		680, 879, 194, 572, 640, 724, 926, 56, 204, 700,
		707, 151, 457, 449, 797, 195, 791, 558, 945, 679,
		297, 59, 87, 824, 713, 663, 412, 693, 342, 606,
		134, 108, 571, 364, 631, 212, 174, 643, 304, 329,
		343, 97, 430, 751, 497, 314, 983, 374, 822, 928,
		140, 206, 73, 263, 980, 736, 876, 478, 430, 305,
		170, 514, 364, 692, 829, 82, 855, 953, 676, 246,
		369, 970, 294, 750, 807, 827, 150, 790, 288, 923,
		804, 378, 215, 828, 592, 281, 565, 555, 710, 82,
		896, 831, 547, 261, 524, 462, 293, 465, 502, 56,
		661, 821, 976, 991, 658, 869, 905, 758, 745, 193,
		768, 550, 608, 933, 378, 286, 215, 979, 792, 961,
		61, 688, 793, 644, 986, 403, 106, 366, 905, 644,
		372, 567, 466, 434, 645, 210, 389, 550, 919, 135,
		780, 773, 635, 389, 707, 100, 626, 958, 165, 504,
		920, 176, 193, 713, 857, 265, 203, 50, 668, 108,
		645, 990, 626, 197, 510, 357, 358, 850, 858, 364,
		936, 638
	};

	private static readonly int[] Incs = new int[14]
	{
		1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524,
		88573, 265720, 797161, 2391484
	};

	private bool finished;

	private int count;

	private int origPtr;

	private readonly int blockSize100k;

	private readonly int allowableBlockSize;

	private bool blockRandomised;

	private readonly IList<StackElem> blocksortStack = new List<StackElem>();

	private int bsBuff;

	private int bsLivePos;

	private readonly CRC m_blockCrc = new CRC();

	private bool[] inUse = new bool[256];

	private int nInUse;

	private byte[] m_selectors = new byte[18002];

	private byte[] blockBytes;

	private ushort[] quadrantShorts;

	private int[] zptr;

	private int[] szptr;

	private int[] ftab;

	private int nMTF;

	private int[] mtfFreq = new int[258];

	private int workFactor;

	private int workDone;

	private int workLimit;

	private bool firstAttempt;

	private int currentByte = -1;

	private int runLength;

	private int m_streamCrc;

	private bool closed;

	private Stream bsStream;

	protected static void HbMakeCodeLengths(byte[] len, int[] freq, int alphaSize, int maxLen)
	{
		int[] array = new int[260];
		int[] array2 = new int[516];
		int[] array3 = new int[516];
		for (int i = 0; i < alphaSize; i++)
		{
			array2[i + 1] = ((freq[i] == 0) ? 1 : freq[i]) << 8;
		}
		while (true)
		{
			int num = alphaSize;
			int num2 = 0;
			array[0] = 0;
			array2[0] = 0;
			array3[0] = -2;
			for (int j = 1; j <= alphaSize; j++)
			{
				array3[j] = -1;
				array[++num2] = j;
				int num3 = num2;
				int num4 = array[num3];
				while (array2[num4] < array2[array[num3 >> 1]])
				{
					array[num3] = array[num3 >> 1];
					num3 >>= 1;
				}
				array[num3] = num4;
			}
			if (num2 >= 260)
			{
				throw new InvalidOperationException();
			}
			while (num2 > 1)
			{
				int num5 = array[1];
				array[1] = array[num2--];
				int num6 = 1;
				int num7 = array[num6];
				while (true)
				{
					int num8 = num6 << 1;
					if (num8 > num2)
					{
						break;
					}
					if (num8 < num2 && array2[array[num8 + 1]] < array2[array[num8]])
					{
						num8++;
					}
					if (array2[num7] < array2[array[num8]])
					{
						break;
					}
					array[num6] = array[num8];
					num6 = num8;
				}
				array[num6] = num7;
				int num9 = array[1];
				array[1] = array[num2--];
				int num10 = 1;
				int num11 = array[num10];
				while (true)
				{
					int num12 = num10 << 1;
					if (num12 > num2)
					{
						break;
					}
					if (num12 < num2 && array2[array[num12 + 1]] < array2[array[num12]])
					{
						num12++;
					}
					if (array2[num11] < array2[array[num12]])
					{
						break;
					}
					array[num10] = array[num12];
					num10 = num12;
				}
				array[num10] = num11;
				num++;
				array3[num5] = (array3[num9] = num);
				array2[num] = (int)((array2[num5] & 0xFFFFFF00u) + (array2[num9] & 0xFFFFFF00u)) | (1 + (((array2[num5] & 0xFF) > (array2[num9] & 0xFF)) ? (array2[num5] & 0xFF) : (array2[num9] & 0xFF)));
				array3[num] = -1;
				array[++num2] = num;
				int num13 = num2;
				int num14 = array[num13];
				while (array2[num14] < array2[array[num13 >> 1]])
				{
					array[num13] = array[num13 >> 1];
					num13 >>= 1;
				}
				array[num13] = num14;
			}
			if (num >= 516)
			{
				throw new InvalidOperationException();
			}
			int num15 = 0;
			for (int k = 1; k <= alphaSize; k++)
			{
				int num16 = 0;
				int num17 = k;
				while (array3[num17] >= 0)
				{
					num17 = array3[num17];
					num16++;
				}
				len[k - 1] = (byte)num16;
				num15 |= maxLen - num16;
			}
			if (num15 < 0)
			{
				for (int l = 1; l <= alphaSize; l++)
				{
					int num18 = array2[l] >> 8;
					num18 = 1 + num18 / 2;
					array2[l] = num18 << 8;
				}
				continue;
			}
			break;
		}
	}

	public CBZip2OutputStream(Stream outStream)
		: this(outStream, 9)
	{
	}

	public CBZip2OutputStream(Stream outStream, int blockSize)
	{
		blockBytes = null;
		quadrantShorts = null;
		zptr = null;
		ftab = null;
		outStream.WriteByte(66);
		outStream.WriteByte(90);
		bsStream = outStream;
		bsBuff = 0;
		bsLivePos = 32;
		workFactor = 50;
		if (blockSize > 9)
		{
			blockSize = 9;
		}
		else if (blockSize < 1)
		{
			blockSize = 1;
		}
		blockSize100k = blockSize;
		allowableBlockSize = 100000 * blockSize100k - 20;
		int num = 100000 * blockSize100k;
		blockBytes = new byte[num + 1 + 20];
		quadrantShorts = new ushort[num + 1 + 20];
		zptr = new int[num];
		ftab = new int[65537];
		szptr = zptr;
		outStream.WriteByte(104);
		outStream.WriteByte((byte)(48 + blockSize100k));
		m_streamCrc = 0;
		InitBlock();
	}

	public override void WriteByte(byte value)
	{
		if (currentByte == value)
		{
			if (++runLength > 254)
			{
				WriteRun();
				currentByte = -1;
				runLength = 0;
			}
		}
		else
		{
			if (currentByte >= 0)
			{
				WriteRun();
			}
			currentByte = value;
			runLength = 1;
		}
	}

	private void WriteRun()
	{
		if (count > allowableBlockSize)
		{
			EndBlock();
			InitBlock();
		}
		inUse[currentByte] = true;
		switch (runLength)
		{
		case 1:
			blockBytes[++count] = (byte)currentByte;
			m_blockCrc.Update((byte)currentByte);
			break;
		case 2:
			blockBytes[++count] = (byte)currentByte;
			blockBytes[++count] = (byte)currentByte;
			m_blockCrc.Update((byte)currentByte);
			m_blockCrc.Update((byte)currentByte);
			break;
		case 3:
			blockBytes[++count] = (byte)currentByte;
			blockBytes[++count] = (byte)currentByte;
			blockBytes[++count] = (byte)currentByte;
			m_blockCrc.Update((byte)currentByte);
			m_blockCrc.Update((byte)currentByte);
			m_blockCrc.Update((byte)currentByte);
			break;
		default:
			blockBytes[++count] = (byte)currentByte;
			blockBytes[++count] = (byte)currentByte;
			blockBytes[++count] = (byte)currentByte;
			blockBytes[++count] = (byte)currentByte;
			blockBytes[++count] = (byte)(runLength - 4);
			inUse[runLength - 4] = true;
			m_blockCrc.UpdateRun((byte)currentByte, runLength);
			break;
		}
	}

	protected void Detach(bool disposing)
	{
		if (disposing)
		{
			ImplDisposing(disposeOutput: false);
		}
		base.Dispose(disposing);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ImplDisposing(disposeOutput: true);
		}
		base.Dispose(disposing);
	}

	private void ImplDisposing(bool disposeOutput)
	{
		if (!closed)
		{
			Finish();
			closed = true;
			if (disposeOutput)
			{
				bsStream.Dispose();
			}
		}
	}

	public void Finish()
	{
		if (!finished)
		{
			if (runLength > 0)
			{
				WriteRun();
			}
			currentByte = -1;
			if (count > 0)
			{
				EndBlock();
			}
			EndCompression();
			finished = true;
			Flush();
		}
	}

	public override void Flush()
	{
		bsStream.Flush();
	}

	private void InitBlock()
	{
		m_blockCrc.Initialise();
		count = 0;
		for (int i = 0; i < 256; i++)
		{
			inUse[i] = false;
		}
	}

	private void EndBlock()
	{
		int final = m_blockCrc.GetFinal();
		m_streamCrc = Integers.RotateLeft(m_streamCrc, 1) ^ final;
		DoReversibleTransformation();
		BsPutLong48(54156738319193L);
		BsPutInt32(final);
		BsPutBit(blockRandomised ? 1 : 0);
		MoveToFrontCodeAndSend();
	}

	private void EndCompression()
	{
		BsPutLong48(25779555029136L);
		BsPutInt32(m_streamCrc);
		BsFinishedWithStream();
	}

	private void HbAssignCodes(int[] code, byte[] length, int minLen, int maxLen, int alphaSize)
	{
		int num = 0;
		for (int i = minLen; i <= maxLen; i++)
		{
			for (int j = 0; j < alphaSize; j++)
			{
				if (length[j] == i)
				{
					code[j] = num++;
				}
			}
			num <<= 1;
		}
	}

	private void BsFinishedWithStream()
	{
		if (bsLivePos < 32)
		{
			bsStream.WriteByte((byte)(bsBuff >> 24));
			bsBuff = 0;
			bsLivePos = 32;
		}
	}

	private void BsPutBit(int v)
	{
		bsLivePos--;
		bsBuff |= v << bsLivePos;
		if (bsLivePos <= 24)
		{
			bsStream.WriteByte((byte)(bsBuff >> 24));
			bsBuff <<= 8;
			bsLivePos += 8;
		}
	}

	private void BsPutBits(int n, int v)
	{
		bsLivePos -= n;
		bsBuff |= v << bsLivePos;
		while (bsLivePos <= 24)
		{
			bsStream.WriteByte((byte)(bsBuff >> 24));
			bsBuff <<= 8;
			bsLivePos += 8;
		}
	}

	private void BsPutBitsSmall(int n, int v)
	{
		bsLivePos -= n;
		bsBuff |= v << bsLivePos;
		if (bsLivePos <= 24)
		{
			bsStream.WriteByte((byte)(bsBuff >> 24));
			bsBuff <<= 8;
			bsLivePos += 8;
		}
	}

	private void BsPutInt32(int u)
	{
		BsPutBits(16, (u >> 16) & 0xFFFF);
		BsPutBits(16, u & 0xFFFF);
	}

	private void BsPutLong48(long u)
	{
		BsPutBits(24, (int)(u >> 24) & 0xFFFFFF);
		BsPutBits(24, (int)u & 0xFFFFFF);
	}

	private void SendMtfValues()
	{
		int num = nInUse + 2;
		if (nMTF <= 0)
		{
			throw new InvalidOperationException();
		}
		int num2 = ((nMTF < 200) ? 2 : ((nMTF < 600) ? 3 : ((nMTF < 1200) ? 4 : ((nMTF >= 2400) ? 6 : 5))));
		byte[][] array = CreateByteArray(num2, num);
		for (int i = 0; i < num2; i++)
		{
			Arrays.Fill(array[i], 15);
		}
		int num3 = num2;
		int num4 = nMTF;
		int num5 = -1;
		while (num3 > 0)
		{
			int num6 = num5 + 1;
			int j = 0;
			for (int num7 = num4 / num3; j < num7; j += mtfFreq[++num5])
			{
				if (num5 >= num - 1)
				{
					break;
				}
			}
			if (num5 > num6 && num3 != num2 && num3 != 1 && (num2 - num3) % 2 == 1)
			{
				j -= mtfFreq[num5--];
			}
			byte[] array2 = array[num3 - 1];
			for (int k = 0; k < num; k++)
			{
				if (k >= num6 && k <= num5)
				{
					array2[k] = 0;
				}
				else
				{
					array2[k] = 15;
				}
			}
			num3--;
			num4 -= j;
		}
		int[][] array3 = CBZip2InputStream.CreateIntArray(6, 258);
		int[] array4 = new int[6];
		short[] array5 = new short[6];
		int num8 = 0;
		for (int l = 0; l < 4; l++)
		{
			for (int i = 0; i < num2; i++)
			{
				array4[i] = 0;
				int[] array6 = array3[i];
				for (int k = 0; k < num; k++)
				{
					array6[k] = 0;
				}
			}
			num8 = 0;
			int num9 = 0;
			while (num9 < nMTF)
			{
				int num10 = System.Math.Min(num9 + 50 - 1, nMTF - 1);
				if (num2 == 6)
				{
					byte[] array7 = array[0];
					byte[] array8 = array[1];
					byte[] array9 = array[2];
					byte[] array10 = array[3];
					byte[] array11 = array[4];
					byte[] array12 = array[5];
					short num11 = 0;
					short num12 = 0;
					short num13 = 0;
					short num14 = 0;
					short num15 = 0;
					short num16 = 0;
					for (int m = num9; m <= num10; m++)
					{
						int num17 = szptr[m];
						num11 += array7[num17];
						num12 += array8[num17];
						num13 += array9[num17];
						num14 += array10[num17];
						num15 += array11[num17];
						num16 += array12[num17];
					}
					array5[0] = num11;
					array5[1] = num12;
					array5[2] = num13;
					array5[3] = num14;
					array5[4] = num15;
					array5[5] = num16;
				}
				else
				{
					for (int i = 0; i < num2; i++)
					{
						array5[i] = 0;
					}
					for (int m = num9; m <= num10; m++)
					{
						int num18 = szptr[m];
						for (int i = 0; i < num2; i++)
						{
							ref short reference = ref array5[i];
							reference += array[i][num18];
						}
					}
				}
				int num19 = array5[0];
				int num20 = 0;
				for (int i = 1; i < num2; i++)
				{
					short num21 = array5[i];
					if (num21 < num19)
					{
						num19 = num21;
						num20 = i;
					}
				}
				array4[num20]++;
				m_selectors[num8] = (byte)num20;
				num8++;
				int[] array13 = array3[num20];
				for (int m = num9; m <= num10; m++)
				{
					array13[szptr[m]]++;
				}
				num9 = num10 + 1;
			}
			for (int i = 0; i < num2; i++)
			{
				HbMakeCodeLengths(array[i], array3[i], num, 17);
			}
		}
		if (num2 >= 8 || num2 > 6)
		{
			throw new InvalidOperationException();
		}
		if (num8 >= 32768 || num8 > 18002)
		{
			throw new InvalidOperationException();
		}
		int[][] array14 = CBZip2InputStream.CreateIntArray(6, 258);
		for (int i = 0; i < num2; i++)
		{
			int num22 = 0;
			int num23 = 32;
			byte[] array15 = array[i];
			for (int m = 0; m < num; m++)
			{
				int val = array15[m];
				num22 = System.Math.Max(num22, val);
				num23 = System.Math.Min(num23, val);
			}
			if (num23 < 1 || num22 > 17)
			{
				throw new InvalidOperationException();
			}
			HbAssignCodes(array14[i], array15, num23, num22, num);
		}
		bool[] array16 = new bool[16];
		for (int m = 0; m < 16; m++)
		{
			array16[m] = false;
			int num24 = m * 16;
			for (int n = 0; n < 16; n++)
			{
				if (inUse[num24 + n])
				{
					array16[m] = true;
					break;
				}
			}
		}
		for (int m = 0; m < 16; m++)
		{
			BsPutBit(array16[m] ? 1 : 0);
		}
		for (int m = 0; m < 16; m++)
		{
			if (array16[m])
			{
				int num25 = m * 16;
				for (int n = 0; n < 16; n++)
				{
					BsPutBit(inUse[num25 + n] ? 1 : 0);
				}
			}
		}
		BsPutBitsSmall(3, num2);
		BsPutBits(15, num8);
		int num26 = 6636321;
		for (int m = 0; m < num8; m++)
		{
			int num27 = m_selectors[m] << 2;
			int num28 = (num26 >> num27) & 0xF;
			if (num28 != 1)
			{
				int num29 = (8947848 - num26 + 1118481 * num28) & 0x888888;
				num26 = num26 - (num28 << num27) + (num29 >> 3);
			}
			BsPutBitsSmall(num28, (1 << num28) - 2);
		}
		for (int i = 0; i < num2; i++)
		{
			byte[] array17 = array[i];
			int num30 = array17[0];
			BsPutBitsSmall(6, num30 << 1);
			for (int m = 1; m < num; m++)
			{
				int num31;
				for (num31 = array17[m]; num30 < num31; num30++)
				{
					BsPutBitsSmall(2, 2);
				}
				while (num30 > num31)
				{
					BsPutBitsSmall(2, 3);
					num30--;
				}
				BsPutBit(0);
			}
		}
		int num32 = 0;
		int num33 = 0;
		while (num33 < nMTF)
		{
			int num34 = System.Math.Min(num33 + 50 - 1, nMTF - 1);
			int num35 = m_selectors[num32];
			byte[] array18 = array[num35];
			int[] array19 = array14[num35];
			for (int m = num33; m <= num34; m++)
			{
				int num36 = szptr[m];
				BsPutBits(array18[num36], array19[num36]);
			}
			num33 = num34 + 1;
			num32++;
		}
		if (num32 != num8)
		{
			throw new InvalidOperationException();
		}
	}

	private void MoveToFrontCodeAndSend()
	{
		BsPutBits(24, origPtr);
		GenerateMtfValues();
		SendMtfValues();
	}

	private void SimpleSort(int lo, int hi, int d)
	{
		int num = hi - lo + 1;
		if (num < 2)
		{
			return;
		}
		int i;
		for (i = 0; Incs[i] < num; i++)
		{
		}
		for (i--; i >= 0; i--)
		{
			int num2 = Incs[i];
			int num3 = lo + num2;
			while (num3 <= hi)
			{
				int num4 = zptr[num3];
				int num5 = num3;
				while (FullGtU(zptr[num5 - num2] + d, num4 + d))
				{
					zptr[num5] = zptr[num5 - num2];
					num5 -= num2;
					if (num5 <= lo + num2 - 1)
					{
						break;
					}
				}
				zptr[num5] = num4;
				if (++num3 > hi)
				{
					break;
				}
				num4 = zptr[num3];
				num5 = num3;
				while (FullGtU(zptr[num5 - num2] + d, num4 + d))
				{
					zptr[num5] = zptr[num5 - num2];
					num5 -= num2;
					if (num5 <= lo + num2 - 1)
					{
						break;
					}
				}
				zptr[num5] = num4;
				if (++num3 > hi)
				{
					break;
				}
				num4 = zptr[num3];
				num5 = num3;
				while (FullGtU(zptr[num5 - num2] + d, num4 + d))
				{
					zptr[num5] = zptr[num5 - num2];
					num5 -= num2;
					if (num5 <= lo + num2 - 1)
					{
						break;
					}
				}
				zptr[num5] = num4;
				num3++;
				if (workDone > workLimit && firstAttempt)
				{
					return;
				}
			}
		}
	}

	private void Vswap(int p1, int p2, int n)
	{
		while (--n >= 0)
		{
			int num = zptr[p1];
			int num2 = zptr[p2];
			zptr[p1++] = num2;
			zptr[p2++] = num;
		}
	}

	private int Med3(int a, int b, int c)
	{
		if (a <= b)
		{
			if (c >= a)
			{
				if (c <= b)
				{
					return c;
				}
				return b;
			}
			return a;
		}
		if (c >= b)
		{
			if (c <= a)
			{
				return c;
			}
			return a;
		}
		return b;
	}

	private static void PushStackElem(IList<StackElem> stack, int stackCount, int ll, int hh, int dd)
	{
		StackElem stackElem;
		if (stackCount < stack.Count)
		{
			stackElem = stack[stackCount];
		}
		else
		{
			stackElem = new StackElem();
			stack.Add(stackElem);
		}
		stackElem.ll = ll;
		stackElem.hh = hh;
		stackElem.dd = dd;
	}

	private void QSort3(int loSt, int hiSt, int dSt)
	{
		IList<StackElem> list = blocksortStack;
		int num = 0;
		int num2 = loSt;
		int num3 = hiSt;
		int num4 = dSt;
		while (true)
		{
			if (num3 - num2 < 20 || num4 > 10)
			{
				SimpleSort(num2, num3, num4);
				if (num < 1 || (workDone > workLimit && firstAttempt))
				{
					break;
				}
				StackElem stackElem = list[--num];
				num2 = stackElem.ll;
				num3 = stackElem.hh;
				num4 = stackElem.dd;
				continue;
			}
			int num5 = num4 + 1;
			int num6 = Med3(blockBytes[zptr[num2] + num5], blockBytes[zptr[num3] + num5], blockBytes[zptr[num2 + num3 >> 1] + num5]);
			int num8;
			int num7 = (num8 = num2);
			int num10;
			int num9 = (num10 = num3);
			int num12;
			while (true)
			{
				if (num7 <= num9)
				{
					int num11 = zptr[num7];
					num12 = blockBytes[num11 + num5] - num6;
					if (num12 <= 0)
					{
						if (num12 == 0)
						{
							zptr[num7] = zptr[num8];
							zptr[num8++] = num11;
						}
						num7++;
						continue;
					}
				}
				while (num7 <= num9)
				{
					int num13 = zptr[num9];
					num12 = blockBytes[num13 + num5] - num6;
					if (num12 < 0)
					{
						break;
					}
					if (num12 == 0)
					{
						zptr[num9] = zptr[num10];
						zptr[num10--] = num13;
					}
					num9--;
				}
				if (num7 > num9)
				{
					break;
				}
				int num14 = zptr[num7];
				zptr[num7++] = zptr[num9];
				zptr[num9--] = num14;
			}
			if (num10 < num8)
			{
				num4 = num5;
				continue;
			}
			num12 = System.Math.Min(num8 - num2, num7 - num8);
			Vswap(num2, num7 - num12, num12);
			int num15 = System.Math.Min(num3 - num10, num10 - num9);
			Vswap(num7, num3 - num15 + 1, num15);
			num12 = num2 + (num7 - num8);
			num15 = num3 - (num10 - num9);
			PushStackElem(list, num++, num2, num12 - 1, num4);
			PushStackElem(list, num++, num12, num15, num5);
			num2 = num15 + 1;
		}
	}

	private void MainSort()
	{
		int[] array = new int[256];
		int[] array2 = new int[256];
		bool[] array3 = new bool[256];
		for (int i = 0; i < 20; i++)
		{
			blockBytes[count + i + 1] = blockBytes[i % count + 1];
		}
		for (int i = 0; i <= count + 20; i++)
		{
			quadrantShorts[i] = 0;
		}
		blockBytes[0] = blockBytes[count];
		if (count <= 4000)
		{
			for (int i = 0; i < count; i++)
			{
				zptr[i] = i;
			}
			firstAttempt = false;
			workDone = (workLimit = 0);
			SimpleSort(0, count - 1, 0);
			return;
		}
		for (int i = 0; i <= 255; i++)
		{
			array3[i] = false;
		}
		for (int i = 0; i <= 65536; i++)
		{
			ftab[i] = 0;
		}
		int num = blockBytes[0];
		for (int i = 1; i <= count; i++)
		{
			int num2 = blockBytes[i];
			ftab[(num << 8) + num2]++;
			num = num2;
		}
		for (int i = 0; i < 65536; i++)
		{
			ftab[i + 1] += ftab[i];
		}
		num = blockBytes[1];
		int num3;
		for (int i = 0; i < count - 1; i++)
		{
			int num2 = blockBytes[i + 2];
			num3 = (num << 8) + num2;
			num = num2;
			ftab[num3]--;
			zptr[ftab[num3]] = i;
		}
		num3 = (blockBytes[count] << 8) + blockBytes[1];
		ftab[num3]--;
		zptr[ftab[num3]] = count - 1;
		for (int i = 0; i <= 255; i++)
		{
			array[i] = i;
		}
		int num4 = 1;
		do
		{
			num4 = 3 * num4 + 1;
		}
		while (num4 <= 256);
		do
		{
			num4 /= 3;
			for (int i = num4; i <= 255; i++)
			{
				int num5 = array[i];
				num3 = i;
				while (ftab[array[num3 - num4] + 1 << 8] - ftab[array[num3 - num4] << 8] > ftab[num5 + 1 << 8] - ftab[num5 << 8])
				{
					array[num3] = array[num3 - num4];
					num3 -= num4;
					if (num3 < num4)
					{
						break;
					}
				}
				array[num3] = num5;
			}
		}
		while (num4 != 1);
		for (int i = 0; i <= 255; i++)
		{
			int num6 = array[i];
			for (num3 = 0; num3 <= 255; num3++)
			{
				int num7 = (num6 << 8) + num3;
				if ((ftab[num7] & 0x200000) == 2097152)
				{
					continue;
				}
				int num8 = ftab[num7] & -2097153;
				int num9 = (ftab[num7 + 1] & -2097153) - 1;
				if (num9 > num8)
				{
					QSort3(num8, num9, 2);
					if (workDone > workLimit && firstAttempt)
					{
						return;
					}
				}
				ftab[num7] |= 2097152;
			}
			array3[num6] = true;
			if (i < 255)
			{
				int num10 = ftab[num6 << 8] & -2097153;
				int num11 = (ftab[num6 + 1 << 8] & -2097153) - num10;
				int j;
				for (j = 0; num11 >> j > 65534; j++)
				{
				}
				for (num3 = 0; num3 < num11; num3++)
				{
					int num12 = zptr[num10 + num3] + 1;
					ushort num13 = (ushort)(num3 >> j);
					quadrantShorts[num12] = num13;
					if (num12 <= 20)
					{
						quadrantShorts[num12 + count] = num13;
					}
				}
				if (num11 - 1 >> j > 65535)
				{
					throw new InvalidOperationException();
				}
			}
			for (num3 = 0; num3 <= 255; num3++)
			{
				array2[num3] = ftab[(num3 << 8) + num6] & -2097153;
			}
			for (num3 = ftab[num6 << 8] & -2097153; num3 < (ftab[num6 + 1 << 8] & -2097153); num3++)
			{
				int num14 = zptr[num3];
				num = blockBytes[num14];
				if (!array3[num])
				{
					zptr[array2[num]] = ((num14 == 0) ? count : num14) - 1;
					array2[num]++;
				}
			}
			for (num3 = 0; num3 <= 255; num3++)
			{
				ftab[(num3 << 8) + num6] |= 2097152;
			}
		}
	}

	private void RandomiseBlock()
	{
		for (int i = 0; i < 256; i++)
		{
			inUse[i] = false;
		}
		int num = 0;
		int num2 = 0;
		for (int j = 1; j <= count; j++)
		{
			if (num == 0)
			{
				num = RNums[num2++];
				num2 &= 0x1FF;
			}
			num--;
			blockBytes[j] ^= ((num == 1) ? ((byte)1) : ((byte)0));
			inUse[blockBytes[j]] = true;
		}
	}

	private void DoReversibleTransformation()
	{
		workLimit = workFactor * (count - 1);
		workDone = 0;
		blockRandomised = false;
		firstAttempt = true;
		MainSort();
		if (workDone > workLimit && firstAttempt)
		{
			RandomiseBlock();
			workLimit = (workDone = 0);
			blockRandomised = true;
			firstAttempt = false;
			MainSort();
		}
		origPtr = -1;
		for (int i = 0; i < count; i++)
		{
			if (zptr[i] == 0)
			{
				origPtr = i;
				break;
			}
		}
		if (origPtr == -1)
		{
			throw new InvalidOperationException();
		}
	}

	private bool FullGtU(int i1, int i2)
	{
		int num = blockBytes[++i1];
		int num2 = blockBytes[++i2];
		if (num != num2)
		{
			return num > num2;
		}
		num = blockBytes[++i1];
		num2 = blockBytes[++i2];
		if (num != num2)
		{
			return num > num2;
		}
		num = blockBytes[++i1];
		num2 = blockBytes[++i2];
		if (num != num2)
		{
			return num > num2;
		}
		num = blockBytes[++i1];
		num2 = blockBytes[++i2];
		if (num != num2)
		{
			return num > num2;
		}
		num = blockBytes[++i1];
		num2 = blockBytes[++i2];
		if (num != num2)
		{
			return num > num2;
		}
		num = blockBytes[++i1];
		num2 = blockBytes[++i2];
		if (num != num2)
		{
			return num > num2;
		}
		int num3 = count;
		do
		{
			num = blockBytes[++i1];
			num2 = blockBytes[++i2];
			if (num != num2)
			{
				return num > num2;
			}
			int num4 = quadrantShorts[i1];
			int num5 = quadrantShorts[i2];
			if (num4 != num5)
			{
				return num4 > num5;
			}
			num = blockBytes[++i1];
			num2 = blockBytes[++i2];
			if (num != num2)
			{
				return num > num2;
			}
			num4 = quadrantShorts[i1];
			num5 = quadrantShorts[i2];
			if (num4 != num5)
			{
				return num4 > num5;
			}
			num = blockBytes[++i1];
			num2 = blockBytes[++i2];
			if (num != num2)
			{
				return num > num2;
			}
			num4 = quadrantShorts[i1];
			num5 = quadrantShorts[i2];
			if (num4 != num5)
			{
				return num4 > num5;
			}
			num = blockBytes[++i1];
			num2 = blockBytes[++i2];
			if (num != num2)
			{
				return num > num2;
			}
			num4 = quadrantShorts[i1];
			num5 = quadrantShorts[i2];
			if (num4 != num5)
			{
				return num4 > num5;
			}
			if (i1 >= count)
			{
				i1 -= count;
			}
			if (i2 >= count)
			{
				i2 -= count;
			}
			num3 -= 4;
			workDone++;
		}
		while (num3 >= 0);
		return false;
	}

	private void GenerateMtfValues()
	{
		nInUse = 0;
		byte[] array = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			if (inUse[i])
			{
				array[nInUse++] = (byte)i;
			}
		}
		int num = nInUse + 1;
		for (int i = 0; i <= num; i++)
		{
			mtfFreq[i] = 0;
		}
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < count; i++)
		{
			byte b = blockBytes[zptr[i]];
			byte b2 = array[0];
			if (b == b2)
			{
				num3++;
				continue;
			}
			int num4 = 1;
			do
			{
				byte b3 = b2;
				b2 = array[num4];
				array[num4++] = b3;
			}
			while (b != b2);
			array[0] = b2;
			while (num3 > 0)
			{
				int num5 = --num3 & 1;
				szptr[num2++] = num5;
				mtfFreq[num5]++;
				num3 >>= 1;
			}
			szptr[num2++] = num4;
			mtfFreq[num4]++;
		}
		while (num3 > 0)
		{
			int num6 = --num3 & 1;
			szptr[num2++] = num6;
			mtfFreq[num6]++;
			num3 >>= 1;
		}
		szptr[num2++] = num;
		mtfFreq[num]++;
		nMTF = num2;
	}

	internal static byte[][] CreateByteArray(int n1, int n2)
	{
		byte[][] array = new byte[n1][];
		for (int i = 0; i < n1; i++)
		{
			array[i] = new byte[n2];
		}
		return array;
	}
}
