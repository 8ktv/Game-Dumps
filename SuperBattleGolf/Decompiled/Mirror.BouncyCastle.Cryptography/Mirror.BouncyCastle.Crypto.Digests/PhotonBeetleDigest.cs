using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class PhotonBeetleDigest : IDigest
{
	private byte[] state;

	private byte[][] state_2d;

	private MemoryStream buffer = new MemoryStream();

	private const int INITIAL_RATE_INBYTES = 16;

	private int RATE_INBYTES = 4;

	private int SQUEEZE_RATE_INBYTES = 16;

	private int STATE_INBYTES = 32;

	private int TAG_INBYTES = 32;

	private int LAST_THREE_BITS_OFFSET = 5;

	private int ROUND = 12;

	private int D = 8;

	private int Dq = 3;

	private int Dr = 7;

	private int DSquare = 64;

	private int S = 4;

	private int S_1 = 3;

	private byte[][] RC = new byte[8][]
	{
		new byte[12]
		{
			1, 3, 7, 14, 13, 11, 6, 12, 9, 2,
			5, 10
		},
		new byte[12]
		{
			0, 2, 6, 15, 12, 10, 7, 13, 8, 3,
			4, 11
		},
		new byte[12]
		{
			2, 0, 4, 13, 14, 8, 5, 15, 10, 1,
			6, 9
		},
		new byte[12]
		{
			6, 4, 0, 9, 10, 12, 1, 11, 14, 5,
			2, 13
		},
		new byte[12]
		{
			14, 12, 8, 1, 2, 4, 9, 3, 6, 13,
			10, 5
		},
		new byte[12]
		{
			15, 13, 9, 0, 3, 5, 8, 2, 7, 12,
			11, 4
		},
		new byte[12]
		{
			13, 15, 11, 2, 1, 7, 10, 0, 5, 14,
			9, 6
		},
		new byte[12]
		{
			9, 11, 15, 6, 5, 3, 14, 4, 1, 10,
			13, 2
		}
	};

	private byte[][] MixColMatrix = new byte[8][]
	{
		new byte[8] { 2, 4, 2, 11, 2, 8, 5, 6 },
		new byte[8] { 12, 9, 8, 13, 7, 7, 5, 2 },
		new byte[8] { 4, 4, 13, 13, 9, 4, 13, 9 },
		new byte[8] { 1, 6, 5, 1, 12, 13, 15, 14 },
		new byte[8] { 15, 12, 9, 13, 14, 5, 14, 13 },
		new byte[8] { 9, 14, 5, 15, 4, 12, 9, 6 },
		new byte[8] { 12, 2, 2, 10, 3, 1, 1, 14 },
		new byte[8] { 15, 1, 13, 10, 5, 10, 2, 3 }
	};

	private byte[] sbox = new byte[16]
	{
		12, 5, 6, 11, 9, 0, 10, 13, 3, 14,
		15, 8, 4, 7, 1, 2
	};

	public string AlgorithmName => "Photon-Beetle Hash";

	public PhotonBeetleDigest()
	{
		state = new byte[STATE_INBYTES];
		state_2d = new byte[D][];
		for (int i = 0; i < D; i++)
		{
			state_2d[i] = new byte[D];
		}
	}

	public int GetDigestSize()
	{
		return TAG_INBYTES;
	}

	public int GetByteLength()
	{
		throw new NotImplementedException();
	}

	public void Update(byte input)
	{
		buffer.WriteByte(input);
	}

	public void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		Check.DataLength(input, inOff, inLen, "input buffer too short");
		buffer.Write(input, inOff, inLen);
	}

	public int DoFinal(byte[] output, int outOff)
	{
		Check.OutputLength(output, outOff, 32, "output buffer too short");
		byte[] array = buffer.GetBuffer();
		int num = (int)buffer.Length;
		if (num == 0)
		{
			state[STATE_INBYTES - 1] ^= (byte)(1 << LAST_THREE_BITS_OFFSET);
		}
		else if (num <= 16)
		{
			Array.Copy(array, 0, state, 0, num);
			if (num < 16)
			{
				state[num] ^= 1;
			}
			state[STATE_INBYTES - 1] ^= (byte)(((num < 16) ? 1 : 2) << LAST_THREE_BITS_OFFSET);
		}
		else
		{
			Array.Copy(array, 0, state, 0, 16);
			num -= 16;
			int num2 = (num + RATE_INBYTES - 1) / RATE_INBYTES;
			int i;
			for (i = 0; i < num2 - 1; i++)
			{
				PHOTON_Permutation();
				Bytes.XorTo(RATE_INBYTES, array, 16 + i * RATE_INBYTES, state, 0);
			}
			PHOTON_Permutation();
			int num3 = num - i * RATE_INBYTES;
			Bytes.XorTo(num3, array, 16 + i * RATE_INBYTES, state, 0);
			if (num3 < RATE_INBYTES)
			{
				state[num3] ^= 1;
			}
			state[STATE_INBYTES - 1] ^= (byte)(((num % RATE_INBYTES == 0) ? 1 : 2) << LAST_THREE_BITS_OFFSET);
		}
		PHOTON_Permutation();
		Array.Copy(state, 0, output, outOff, SQUEEZE_RATE_INBYTES);
		PHOTON_Permutation();
		Array.Copy(state, 0, output, outOff + SQUEEZE_RATE_INBYTES, TAG_INBYTES - SQUEEZE_RATE_INBYTES);
		return TAG_INBYTES;
	}

	public void Reset()
	{
		buffer.SetLength(0L);
		Arrays.Fill(state, 0);
	}

	private void PHOTON_Permutation()
	{
		for (int i = 0; i < DSquare; i++)
		{
			state_2d[i >> Dq][i & Dr] = (byte)(((state[i >> 1] & 0xFF) >> 4 * (i & 1)) & 0xF);
		}
		for (int j = 0; j < ROUND; j++)
		{
			for (int i = 0; i < D; i++)
			{
				state_2d[i][0] ^= RC[i][j];
			}
			for (int i = 0; i < D; i++)
			{
				for (int k = 0; k < D; k++)
				{
					state_2d[i][k] = sbox[state_2d[i][k]];
				}
			}
			for (int i = 1; i < D; i++)
			{
				Array.Copy(state_2d[i], 0, state, 0, D);
				Array.Copy(state, i, state_2d[i], 0, D - i);
				Array.Copy(state, 0, state_2d[i], D - i, i);
			}
			for (int k = 0; k < D; k++)
			{
				for (int i = 0; i < D; i++)
				{
					byte b = 0;
					for (int l = 0; l < D; l++)
					{
						int num = MixColMatrix[i][l];
						int num2 = 0;
						int num3 = state_2d[l][k];
						for (int m = 0; m < S; m++)
						{
							if (((num3 >> m) & 1) != 0)
							{
								num2 ^= num;
							}
							if (((num >> S_1) & 1) != 0)
							{
								num <<= 1;
								num ^= 3;
							}
							else
							{
								num <<= 1;
							}
						}
						b ^= (byte)(num2 & 0xF);
					}
					state[i] = b;
				}
				for (int i = 0; i < D; i++)
				{
					state_2d[i][k] = state[i];
				}
			}
		}
		for (int i = 0; i < DSquare; i += 2)
		{
			state[i >> 1] = (byte)((state_2d[i >> Dq][i & Dr] & 0xF) | ((state_2d[i >> Dq][(i + 1) & Dr] & 0xF) << 4));
		}
	}
}
