using System;
using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Utilities.Bzip2;

public class CBZip2InputStream : BaseInputStream
{
	private int last;

	private int origPtr;

	private int blockSize100k;

	private int bsBuff;

	private int bsLive;

	private readonly CRC m_blockCrc = new CRC();

	private int nInUse;

	private byte[] seqToUnseq = new byte[256];

	private byte[] m_selectors = new byte[18002];

	private int[] tt;

	private byte[] ll8;

	private int[] unzftab = new int[256];

	private int[][] limit = CreateIntArray(6, 21);

	private int[][] basev = CreateIntArray(6, 21);

	private int[][] perm = CreateIntArray(6, 258);

	private int[] minLens = new int[6];

	private Stream bsStream;

	private bool streamEnd;

	private int currentByte = -1;

	private const int RAND_PART_B_STATE = 1;

	private const int RAND_PART_C_STATE = 2;

	private const int NO_RAND_PART_B_STATE = 3;

	private const int NO_RAND_PART_C_STATE = 4;

	private int currentState;

	private int m_expectedBlockCrc;

	private int m_expectedStreamCrc;

	private int m_streamCrc;

	private int i2;

	private int count;

	private int chPrev;

	private int ch2;

	private int i;

	private int tPos;

	private int rNToGo;

	private int rTPos;

	private int j2;

	private int z;

	public CBZip2InputStream(Stream zStream)
	{
		ll8 = null;
		tt = null;
		bsStream = zStream;
		bsLive = 0;
		bsBuff = 0;
		int num = bsStream.ReadByte();
		int num2 = bsStream.ReadByte();
		int num3 = bsStream.ReadByte();
		int num4 = bsStream.ReadByte();
		if (num4 < 0)
		{
			throw new EndOfStreamException();
		}
		if (num != 66 || num2 != 90 || num3 != 104 || num4 < 49 || num4 > 57)
		{
			throw new IOException("Invalid stream header");
		}
		blockSize100k = num4 - 48;
		int num5 = 100000 * blockSize100k;
		ll8 = new byte[num5];
		tt = new int[num5];
		m_streamCrc = 0;
		BeginBlock();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		int num = 0;
		while (num < count)
		{
			int num2 = ReadByte();
			if (num2 < 0)
			{
				break;
			}
			buffer[offset + num++] = (byte)num2;
		}
		return num;
	}

	public override int ReadByte()
	{
		if (streamEnd)
		{
			return -1;
		}
		int result = currentByte;
		switch (currentState)
		{
		case 1:
			SetupRandPartB();
			break;
		case 2:
			SetupRandPartC();
			break;
		case 3:
			SetupNoRandPartB();
			break;
		case 4:
			SetupNoRandPartC();
			break;
		default:
			throw new InvalidOperationException();
		}
		return result;
	}

	private void BeginBlock()
	{
		switch (BsGetLong48())
		{
		default:
			throw new IOException("Block header error");
		case 25779555029136L:
			m_expectedStreamCrc = BsGetInt32();
			if (m_expectedStreamCrc != m_streamCrc)
			{
				throw new IOException("Stream CRC error");
			}
			streamEnd = true;
			break;
		case 54156738319193L:
		{
			m_expectedBlockCrc = BsGetInt32();
			bool flag = BsGetBit() == 1;
			GetAndMoveToFrontDecode();
			m_blockCrc.Initialise();
			int[] array = new int[257];
			int num = 0;
			array[0] = 0;
			for (i = 0; i < 256; i++)
			{
				num += unzftab[i];
				array[i + 1] = num;
			}
			if (num != last + 1)
			{
				throw new InvalidOperationException();
			}
			for (i = 0; i <= last; i++)
			{
				byte b = ll8[i];
				tt[array[b]++] = i;
			}
			tPos = tt[origPtr];
			count = 0;
			i2 = 0;
			ch2 = 256;
			if (flag)
			{
				rNToGo = 0;
				rTPos = 0;
				SetupRandPartA();
			}
			else
			{
				SetupNoRandPartA();
			}
			break;
		}
		}
	}

	private void EndBlock()
	{
		int final = m_blockCrc.GetFinal();
		if (m_expectedBlockCrc != final)
		{
			throw new IOException("Block CRC error");
		}
		m_streamCrc = Integers.RotateLeft(m_streamCrc, 1) ^ final;
	}

	protected void Detach(bool disposing)
	{
		if (disposing)
		{
			ImplDisposing(disposeInput: false);
		}
		base.Dispose(disposing);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ImplDisposing(disposeInput: true);
		}
		base.Dispose(disposing);
	}

	private void ImplDisposing(bool disposeInput)
	{
		if (bsStream != null)
		{
			if (disposeInput)
			{
				bsStream.Dispose();
			}
			bsStream = null;
		}
	}

	private int BsGetBit()
	{
		if (bsLive == 0)
		{
			bsBuff = RequireByte();
			bsLive = 7;
			return bsBuff >>> 7;
		}
		bsLive--;
		return (bsBuff >> bsLive) & 1;
	}

	private int BsGetBits(int n)
	{
		while (bsLive < n)
		{
			bsBuff = (bsBuff << 8) | RequireByte();
			bsLive += 8;
		}
		bsLive -= n;
		return (bsBuff >> bsLive) & ((1 << n) - 1);
	}

	private int BsGetBitsSmall(int n)
	{
		if (bsLive < n)
		{
			bsBuff = (bsBuff << 8) | RequireByte();
			bsLive += 8;
		}
		bsLive -= n;
		return (bsBuff >> bsLive) & ((1 << n) - 1);
	}

	private int BsGetInt32()
	{
		return (BsGetBits(16) << 16) | BsGetBits(16);
	}

	private long BsGetLong48()
	{
		return ((long)BsGetBits(24) << 24) | BsGetBits(24);
	}

	private void HbCreateDecodeTables(int[] limit, int[] basev, int[] perm, byte[] length, int minLen, int maxLen, int alphaSize)
	{
		Array.Clear(basev, 0, basev.Length);
		Array.Clear(limit, 0, limit.Length);
		int num = 0;
		int num2 = 0;
		for (int i = minLen; i <= maxLen; i++)
		{
			for (int j = 0; j < alphaSize; j++)
			{
				if (length[j] == i)
				{
					perm[num++] = j;
				}
			}
			basev[i] = num2;
			limit[i] = num2 + num;
			num2 += num2 + num;
		}
	}

	private int RecvDecodingTables()
	{
		nInUse = 0;
		int num = BsGetBits(16);
		for (int i = 0; i < 16; i++)
		{
			if ((num & (32768 >> i)) == 0)
			{
				continue;
			}
			int num2 = BsGetBits(16);
			int num3 = i * 16;
			for (int j = 0; j < 16; j++)
			{
				if ((num2 & (32768 >> j)) != 0)
				{
					seqToUnseq[nInUse++] = (byte)(num3 + j);
				}
			}
		}
		if (nInUse < 1)
		{
			throw new InvalidOperationException();
		}
		int num4 = nInUse + 2;
		int num5 = BsGetBitsSmall(3);
		if (num5 < 2 || num5 > 6)
		{
			throw new InvalidOperationException();
		}
		int num6 = BsGetBits(15);
		if (num6 < 1)
		{
			throw new InvalidOperationException();
		}
		uint num7 = 5517840u;
		for (int i = 0; i < num6; i++)
		{
			int num8 = 0;
			while (BsGetBit() == 1)
			{
				if (++num8 >= num5)
				{
					throw new InvalidOperationException();
				}
			}
			if (i < 18002)
			{
				switch (num8)
				{
				case 1:
					num7 = ((num7 >> 4) & 0xF) | ((num7 << 4) & 0xF0) | (num7 & 0xFFFF00);
					break;
				case 2:
					num7 = ((num7 >> 8) & 0xF) | ((num7 << 4) & 0xFF0) | (num7 & 0xFFF000);
					break;
				case 3:
					num7 = ((num7 >> 12) & 0xF) | ((num7 << 4) & 0xFFF0) | (num7 & 0xFF0000);
					break;
				case 4:
					num7 = ((num7 >> 16) & 0xF) | ((num7 << 4) & 0xFFFF0) | (num7 & 0xF00000);
					break;
				case 5:
					num7 = ((num7 >> 20) & 0xF) | ((num7 << 4) & 0xFFFFF0);
					break;
				default:
					throw new InvalidOperationException();
				case 0:
					break;
				}
				m_selectors[i] = (byte)(num7 & 0xF);
			}
		}
		byte[] array = new byte[num4];
		for (int k = 0; k < num5; k++)
		{
			int num9 = 0;
			int num10 = 32;
			int num11 = BsGetBitsSmall(5);
			if (num11 < 1 || num11 > 20)
			{
				throw new InvalidOperationException();
			}
			for (int i = 0; i < num4; i++)
			{
				int num12 = BsGetBit();
				while (num12 != 0)
				{
					int num13 = BsGetBitsSmall(2);
					num11 += 1 - (num13 & 2);
					if (num11 < 1 || num11 > 20)
					{
						throw new InvalidOperationException();
					}
					num12 = num13 & 1;
				}
				array[i] = (byte)num11;
				num9 = System.Math.Max(num9, num11);
				num10 = System.Math.Min(num10, num11);
			}
			HbCreateDecodeTables(limit[k], basev[k], perm[k], array, num10, num9, num4);
			minLens[k] = num10;
		}
		return num6;
	}

	private void GetAndMoveToFrontDecode()
	{
		int num = 100000 * blockSize100k;
		origPtr = BsGetBits(24);
		if (origPtr > 10 + num)
		{
			throw new InvalidOperationException();
		}
		int num2 = RecvDecodingTables();
		int num3 = nInUse + 2;
		int num4 = nInUse + 1;
		Array.Clear(unzftab, 0, unzftab.Length);
		byte[] array = new byte[nInUse];
		for (int i = 0; i < nInUse; i++)
		{
			array[i] = seqToUnseq[i];
		}
		last = -1;
		int num5 = 0;
		int num6 = 49;
		int num7 = m_selectors[num5];
		int num8 = minLens[num7];
		int[] array2 = limit[num7];
		int[] array3 = perm[num7];
		int[] array4 = basev[num7];
		int num9 = num8;
		int num10;
		for (num10 = BsGetBits(num8); num10 >= array2[num9]; num10 = (num10 << 1) | BsGetBit())
		{
			if (++num9 > 20)
			{
				throw new InvalidOperationException();
			}
		}
		int num11 = num10 - array4[num9];
		if (num11 >= num3)
		{
			throw new InvalidOperationException();
		}
		int num12 = array3[num11];
		while (num12 != num4)
		{
			if (num12 <= 1)
			{
				int num13 = 1;
				int num14 = 0;
				do
				{
					if (num13 > 1048576)
					{
						throw new InvalidOperationException();
					}
					num14 += num13 << num12;
					num13 <<= 1;
					if (num6 == 0)
					{
						if (++num5 >= num2)
						{
							throw new InvalidOperationException();
						}
						num6 = 50;
						num7 = m_selectors[num5];
						num8 = minLens[num7];
						array2 = limit[num7];
						array3 = perm[num7];
						array4 = basev[num7];
					}
					num6--;
					int num15 = num8;
					int num16;
					for (num16 = BsGetBits(num8); num16 >= array2[num15]; num16 = (num16 << 1) | BsGetBit())
					{
						if (++num15 > 20)
						{
							throw new InvalidOperationException();
						}
					}
					int num17 = num16 - array4[num15];
					if (num17 >= num3)
					{
						throw new InvalidOperationException();
					}
					num12 = array3[num17];
				}
				while (num12 <= 1);
				byte b = array[0];
				unzftab[b] += num14;
				if (last >= num - num14)
				{
					throw new InvalidOperationException("Block overrun");
				}
				while (--num14 >= 0)
				{
					ll8[++last] = b;
				}
				continue;
			}
			if (++last >= num)
			{
				throw new InvalidOperationException("Block overrun");
			}
			byte b2 = array[num12 - 1];
			unzftab[b2]++;
			ll8[last] = b2;
			if (num12 <= 16)
			{
				for (int num18 = num12 - 1; num18 > 0; num18--)
				{
					array[num18] = array[num18 - 1];
				}
			}
			else
			{
				Array.Copy(array, 0, array, 1, num12 - 1);
			}
			array[0] = b2;
			if (num6 == 0)
			{
				if (++num5 >= num2)
				{
					throw new InvalidOperationException();
				}
				num6 = 50;
				num7 = m_selectors[num5];
				num8 = minLens[num7];
				array2 = limit[num7];
				array3 = perm[num7];
				array4 = basev[num7];
			}
			num6--;
			int num19 = num8;
			int num20;
			for (num20 = BsGetBits(num8); num20 >= array2[num19]; num20 = (num20 << 1) | BsGetBit())
			{
				if (++num19 > 20)
				{
					throw new InvalidOperationException();
				}
			}
			int num21 = num20 - array4[num19];
			if (num21 >= num3)
			{
				throw new InvalidOperationException();
			}
			num12 = array3[num21];
		}
		if (origPtr > last)
		{
			throw new InvalidOperationException();
		}
		int num22 = last + 1;
		int num23 = 0;
		for (int i = 0; i <= 255; i++)
		{
			int num24 = unzftab[i];
			num23 |= num24;
			num23 |= num22 - num24;
		}
		if (num23 < 0)
		{
			throw new InvalidOperationException();
		}
	}

	private int RequireByte()
	{
		int num = bsStream.ReadByte();
		if (num < 0)
		{
			throw new EndOfStreamException();
		}
		return num & 0xFF;
	}

	private void SetupRandPartA()
	{
		if (i2 <= last)
		{
			chPrev = ch2;
			ch2 = ll8[tPos];
			tPos = tt[tPos];
			if (rNToGo == 0)
			{
				rNToGo = CBZip2OutputStream.RNums[rTPos++];
				rTPos &= 511;
			}
			rNToGo--;
			ch2 ^= ((rNToGo == 1) ? 1 : 0);
			i2++;
			currentByte = ch2;
			currentState = 1;
			m_blockCrc.Update((byte)ch2);
		}
		else
		{
			EndBlock();
			BeginBlock();
		}
	}

	private void SetupNoRandPartA()
	{
		if (i2 <= last)
		{
			chPrev = ch2;
			ch2 = ll8[tPos];
			tPos = tt[tPos];
			i2++;
			currentByte = ch2;
			currentState = 3;
			m_blockCrc.Update((byte)ch2);
		}
		else
		{
			EndBlock();
			BeginBlock();
		}
	}

	private void SetupRandPartB()
	{
		if (ch2 != chPrev)
		{
			count = 1;
			SetupRandPartA();
			return;
		}
		if (++count < 4)
		{
			SetupRandPartA();
			return;
		}
		z = ll8[tPos];
		tPos = tt[tPos];
		if (rNToGo == 0)
		{
			rNToGo = CBZip2OutputStream.RNums[rTPos++];
			rTPos &= 511;
		}
		rNToGo--;
		z ^= ((rNToGo == 1) ? 1 : 0);
		j2 = 0;
		currentState = 2;
		SetupRandPartC();
	}

	private void SetupNoRandPartB()
	{
		if (ch2 != chPrev)
		{
			count = 1;
			SetupNoRandPartA();
			return;
		}
		if (++count < 4)
		{
			SetupNoRandPartA();
			return;
		}
		z = ll8[tPos];
		tPos = tt[tPos];
		currentState = 4;
		j2 = 0;
		SetupNoRandPartC();
	}

	private void SetupRandPartC()
	{
		if (j2 < z)
		{
			currentByte = ch2;
			m_blockCrc.Update((byte)ch2);
			j2++;
		}
		else
		{
			i2++;
			count = 0;
			SetupRandPartA();
		}
	}

	private void SetupNoRandPartC()
	{
		if (j2 < z)
		{
			currentByte = ch2;
			m_blockCrc.Update((byte)ch2);
			j2++;
		}
		else
		{
			i2++;
			count = 0;
			SetupNoRandPartA();
		}
	}

	internal static int[][] CreateIntArray(int n1, int n2)
	{
		int[][] array = new int[n1][];
		for (int i = 0; i < n1; i++)
		{
			array[i] = new int[n2];
		}
		return array;
	}
}
