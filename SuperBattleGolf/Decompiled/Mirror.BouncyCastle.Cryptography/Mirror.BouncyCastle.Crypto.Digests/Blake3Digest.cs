using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class Blake3Digest : IDigest, IMemoable, IXof
{
	private const string ERR_OUTPUTTING = "Already outputting";

	private const int NUMWORDS = 8;

	private const int ROUNDS = 7;

	private const int BLOCKLEN = 64;

	private const int CHUNKLEN = 1024;

	private const int CHUNKSTART = 1;

	private const int CHUNKEND = 2;

	private const int PARENT = 4;

	private const int ROOT = 8;

	private const int KEYEDHASH = 16;

	private const int DERIVECONTEXT = 32;

	private const int DERIVEKEY = 64;

	private const int CHAINING0 = 0;

	private const int CHAINING1 = 1;

	private const int CHAINING2 = 2;

	private const int CHAINING3 = 3;

	private const int CHAINING4 = 4;

	private const int CHAINING5 = 5;

	private const int CHAINING6 = 6;

	private const int CHAINING7 = 7;

	private const int IV0 = 8;

	private const int IV1 = 9;

	private const int IV2 = 10;

	private const int IV3 = 11;

	private const int COUNT0 = 12;

	private const int COUNT1 = 13;

	private const int DATALEN = 14;

	private const int FLAGS = 15;

	private static readonly byte[] SIGMA = new byte[16]
	{
		2, 6, 3, 10, 7, 0, 4, 13, 1, 11,
		12, 5, 9, 14, 15, 8
	};

	private static readonly uint[] IV = new uint[8] { 1779033703u, 3144134277u, 1013904242u, 2773480762u, 1359893119u, 2600822924u, 528734635u, 1541459225u };

	private readonly byte[] m_theBuffer = new byte[64];

	private readonly uint[] m_theK = new uint[8];

	private readonly uint[] m_theChaining = new uint[8];

	private readonly uint[] m_theV = new uint[16];

	private readonly uint[] m_theM = new uint[16];

	private readonly byte[] m_theIndices = new byte[16];

	private readonly List<uint[]> m_theStack = new List<uint[]>();

	private readonly int m_theDigestLen;

	private bool m_outputting;

	private long m_outputAvailable;

	private int m_theMode;

	private int m_theOutputMode;

	private int m_theOutputDataLen;

	private long m_theCounter;

	private int m_theCurrBytes;

	private int m_thePos;

	public string AlgorithmName => "BLAKE3";

	public Blake3Digest()
		: this(256)
	{
	}

	public Blake3Digest(int pDigestSize)
	{
		m_theDigestLen = pDigestSize / 8;
		Init(null);
	}

	public Blake3Digest(Blake3Digest pSource)
	{
		m_theDigestLen = pSource.m_theDigestLen;
		Reset(pSource);
	}

	public int GetByteLength()
	{
		return 64;
	}

	public int GetDigestSize()
	{
		return m_theDigestLen;
	}

	public void Init(Blake3Parameters pParams)
	{
		byte[] array = pParams?.GetKey();
		byte[] array2 = pParams?.GetContext();
		Reset();
		if (array != null)
		{
			InitKey(array);
			Arrays.Fill(array, 0);
		}
		else if (array2 != null)
		{
			InitNullKey();
			m_theMode = 32;
			BlockUpdate(array2, 0, array2.Length);
			DoFinal(m_theBuffer, 0);
			InitKeyFromContext();
			Reset();
		}
		else
		{
			InitNullKey();
			m_theMode = 0;
		}
	}

	public void Update(byte b)
	{
		if (m_outputting)
		{
			throw new InvalidOperationException("Already outputting");
		}
		if (m_theBuffer.Length - m_thePos == 0)
		{
			CompressBlock(m_theBuffer, 0);
			Arrays.Fill(m_theBuffer, 0);
			m_thePos = 0;
		}
		m_theBuffer[m_thePos] = b;
		m_thePos++;
	}

	public void BlockUpdate(byte[] pMessage, int pOffset, int pLen)
	{
		if (pMessage == null || pLen == 0)
		{
			return;
		}
		if (m_outputting)
		{
			throw new InvalidOperationException("Already outputting");
		}
		int num = 0;
		if (m_thePos != 0)
		{
			num = 64 - m_thePos;
			if (num >= pLen)
			{
				Array.Copy(pMessage, pOffset, m_theBuffer, m_thePos, pLen);
				m_thePos += pLen;
				return;
			}
			Array.Copy(pMessage, pOffset, m_theBuffer, m_thePos, num);
			CompressBlock(m_theBuffer, 0);
			m_thePos = 0;
			Arrays.Fill(m_theBuffer, 0);
		}
		int num2 = pOffset + pLen - 64;
		int i;
		for (i = pOffset + num; i < num2; i += 64)
		{
			CompressBlock(pMessage, i);
		}
		int num3 = pLen - i;
		Array.Copy(pMessage, i, m_theBuffer, 0, pOffset + num3);
		m_thePos += pOffset + num3;
	}

	public int DoFinal(byte[] pOutput, int pOutOffset)
	{
		return OutputFinal(pOutput, pOutOffset, GetDigestSize());
	}

	public int OutputFinal(byte[] pOut, int pOutOffset, int pOutLen)
	{
		int result = Output(pOut, pOutOffset, pOutLen);
		Reset();
		return result;
	}

	public int Output(byte[] pOut, int pOutOffset, int pOutLen)
	{
		Check.OutputLength(pOut, pOutOffset, pOutLen, "output buffer too short");
		if (!m_outputting)
		{
			CompressFinalBlock(m_thePos);
		}
		if (pOutLen < 0 || (m_outputAvailable >= 0 && pOutLen > m_outputAvailable))
		{
			throw new ArgumentException("Insufficient bytes remaining");
		}
		int num = pOutLen;
		int num2 = pOutOffset;
		if (m_thePos < 64)
		{
			int num3 = System.Math.Min(num, 64 - m_thePos);
			Array.Copy(m_theBuffer, m_thePos, pOut, num2, num3);
			m_thePos += num3;
			num2 += num3;
			num -= num3;
		}
		while (num > 0)
		{
			NextOutputBlock();
			int num4 = System.Math.Min(num, 64);
			Array.Copy(m_theBuffer, 0, pOut, num2, num4);
			m_thePos += num4;
			num2 += num4;
			num -= num4;
		}
		m_outputAvailable -= pOutLen;
		return pOutLen;
	}

	public void Reset()
	{
		ResetBlockCount();
		m_thePos = 0;
		m_outputting = false;
		Arrays.Fill(m_theBuffer, 0);
	}

	public void Reset(IMemoable pSource)
	{
		Blake3Digest blake3Digest = (Blake3Digest)pSource;
		m_theCounter = blake3Digest.m_theCounter;
		m_theCurrBytes = blake3Digest.m_theCurrBytes;
		m_theMode = blake3Digest.m_theMode;
		m_outputting = blake3Digest.m_outputting;
		m_outputAvailable = blake3Digest.m_outputAvailable;
		m_theOutputMode = blake3Digest.m_theOutputMode;
		m_theOutputDataLen = blake3Digest.m_theOutputDataLen;
		Array.Copy(blake3Digest.m_theChaining, 0, m_theChaining, 0, m_theChaining.Length);
		Array.Copy(blake3Digest.m_theK, 0, m_theK, 0, m_theK.Length);
		Array.Copy(blake3Digest.m_theM, 0, m_theM, 0, m_theM.Length);
		m_theStack.Clear();
		foreach (uint[] item in blake3Digest.m_theStack)
		{
			m_theStack.Add(Arrays.Clone(item));
		}
		Array.Copy(blake3Digest.m_theBuffer, 0, m_theBuffer, 0, m_theBuffer.Length);
		m_thePos = blake3Digest.m_thePos;
	}

	public IMemoable Copy()
	{
		return new Blake3Digest(this);
	}

	private void CompressBlock(byte[] pMessage, int pMsgPos)
	{
		InitChunkBlock(64, pFinal: false);
		InitM(pMessage, pMsgPos);
		Compress();
		if (m_theCurrBytes == 0)
		{
			AdjustStack();
		}
	}

	private void InitM(byte[] pMessage, int pMsgPos)
	{
		Pack.LE_To_UInt32(pMessage, pMsgPos, m_theM);
	}

	private void AdjustStack()
	{
		long num = m_theCounter;
		while (num > 0 && (num & 1) != 1)
		{
			uint[] sourceArray = m_theStack[m_theStack.Count - 1];
			m_theStack.RemoveAt(m_theStack.Count - 1);
			Array.Copy(sourceArray, 0, m_theM, 0, 8);
			Array.Copy(m_theChaining, 0, m_theM, 8, 8);
			InitParentBlock();
			Compress();
			num >>= 1;
		}
		m_theStack.Add(Arrays.CopyOf(m_theChaining, 8));
	}

	private void CompressFinalBlock(int pDataLen)
	{
		InitChunkBlock(pDataLen, pFinal: true);
		InitM(m_theBuffer, 0);
		Compress();
		ProcessStack();
	}

	private void ProcessStack()
	{
		while (m_theStack.Count > 0)
		{
			uint[] sourceArray = m_theStack[m_theStack.Count - 1];
			m_theStack.RemoveAt(m_theStack.Count - 1);
			Array.Copy(sourceArray, 0, m_theM, 0, 8);
			Array.Copy(m_theChaining, 0, m_theM, 8, 8);
			InitParentBlock();
			if (m_theStack.Count < 1)
			{
				SetRoot();
			}
			Compress();
		}
	}

	private void Compress()
	{
		InitIndices();
		for (int i = 0; i < 6; i++)
		{
			PerformRound();
			PermuteIndices();
		}
		PerformRound();
		AdjustChaining();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void PerformRound()
	{
		MixG(0, 0, 4, 8, 12);
		MixG(1, 1, 5, 9, 13);
		MixG(2, 2, 6, 10, 14);
		MixG(3, 3, 7, 11, 15);
		MixG(4, 0, 5, 10, 15);
		MixG(5, 1, 6, 11, 12);
		MixG(6, 2, 7, 8, 13);
		MixG(7, 3, 4, 9, 14);
	}

	private void AdjustChaining()
	{
		if (m_outputting)
		{
			for (int i = 0; i < 8; i++)
			{
				m_theV[i] ^= m_theV[i + 8];
				m_theV[i + 8] ^= m_theChaining[i];
			}
			Pack.UInt32_To_LE(m_theV, m_theBuffer, 0);
			m_thePos = 0;
		}
		else
		{
			for (int j = 0; j < 8; j++)
			{
				m_theChaining[j] = m_theV[j] ^ m_theV[j + 8];
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MixG(int msgIdx, int posA, int posB, int posC, int posD)
	{
		int num = msgIdx << 1;
		m_theV[posA] += m_theV[posB] + m_theM[m_theIndices[num++]];
		m_theV[posD] = Integers.RotateRight(m_theV[posD] ^ m_theV[posA], 16);
		m_theV[posC] += m_theV[posD];
		m_theV[posB] = Integers.RotateRight(m_theV[posB] ^ m_theV[posC], 12);
		m_theV[posA] += m_theV[posB] + m_theM[m_theIndices[num]];
		m_theV[posD] = Integers.RotateRight(m_theV[posD] ^ m_theV[posA], 8);
		m_theV[posC] += m_theV[posD];
		m_theV[posB] = Integers.RotateRight(m_theV[posB] ^ m_theV[posC], 7);
	}

	private void InitIndices()
	{
		for (byte b = 0; b < m_theIndices.Length; b++)
		{
			m_theIndices[b] = b;
		}
	}

	private void PermuteIndices()
	{
		for (byte b = 0; b < m_theIndices.Length; b++)
		{
			m_theIndices[b] = SIGMA[m_theIndices[b]];
		}
	}

	private void InitNullKey()
	{
		Array.Copy(IV, 0, m_theK, 0, 8);
	}

	private void InitKey(byte[] pKey)
	{
		Pack.LE_To_UInt32(pKey, 0, m_theK);
		m_theMode = 16;
	}

	private void InitKeyFromContext()
	{
		Array.Copy(m_theV, 0, m_theK, 0, 8);
		m_theMode = 64;
	}

	private void InitChunkBlock(int pDataLen, bool pFinal)
	{
		Array.Copy((m_theCurrBytes == 0) ? m_theK : m_theChaining, 0, m_theV, 0, 8);
		Array.Copy(IV, 0, m_theV, 8, 4);
		m_theV[12] = (uint)m_theCounter;
		m_theV[13] = (uint)(m_theCounter >> 32);
		m_theV[14] = (uint)pDataLen;
		m_theV[15] = (uint)(m_theMode + ((m_theCurrBytes == 0) ? 1 : 0) + (pFinal ? 2 : 0));
		m_theCurrBytes += pDataLen;
		if (m_theCurrBytes >= 1024)
		{
			IncrementBlockCount();
			m_theV[15] |= 2u;
		}
		if (pFinal && m_theStack.Count < 1)
		{
			SetRoot();
		}
	}

	private void InitParentBlock()
	{
		Array.Copy(m_theK, 0, m_theV, 0, 8);
		Array.Copy(IV, 0, m_theV, 8, 4);
		m_theV[12] = 0u;
		m_theV[13] = 0u;
		m_theV[14] = 64u;
		m_theV[15] = (uint)(m_theMode | 4);
	}

	private void NextOutputBlock()
	{
		m_theCounter++;
		Array.Copy(m_theChaining, 0, m_theV, 0, 8);
		Array.Copy(IV, 0, m_theV, 8, 4);
		m_theV[12] = (uint)m_theCounter;
		m_theV[13] = (uint)(m_theCounter >> 32);
		m_theV[14] = (uint)m_theOutputDataLen;
		m_theV[15] = (uint)m_theOutputMode;
		Compress();
	}

	private void IncrementBlockCount()
	{
		m_theCounter++;
		m_theCurrBytes = 0;
	}

	private void ResetBlockCount()
	{
		m_theCounter = 0L;
		m_theCurrBytes = 0;
	}

	private void SetRoot()
	{
		m_theV[15] |= 8u;
		m_theOutputMode = (int)m_theV[15];
		m_theOutputDataLen = (int)m_theV[14];
		m_theCounter = 0L;
		m_outputting = true;
		m_outputAvailable = -1L;
		Array.Copy(m_theV, 0, m_theChaining, 0, 8);
	}
}
