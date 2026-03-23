using System;
using System.IO;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public sealed class Grain128AeadEngine : IAeadCipher
{
	private static readonly int STATE_SIZE = 4;

	private byte[] workingKey;

	private byte[] workingIV;

	private uint[] lfsr;

	private uint[] nfsr;

	private uint[] authAcc;

	private uint[] authSr;

	private bool initialised;

	private bool aadFinished;

	private MemoryStream aadData = new MemoryStream();

	private byte[] mac;

	public string AlgorithmName => "Grain-128AEAD";

	public void Init(bool forEncryption, ICipherParameters param)
	{
		ParametersWithIV obj = (param as ParametersWithIV) ?? throw new ArgumentException("Grain-128AEAD Init parameters must include an IV");
		byte[] iV = obj.GetIV();
		if (iV == null || iV.Length != 12)
		{
			throw new ArgumentException("Grain-128AEAD requires exactly 12 bytes of IV");
		}
		byte[] key = ((obj.Parameters as KeyParameter) ?? throw new ArgumentException("Grain-128AEAD Init parameters must include a key")).GetKey();
		if (key.Length != 16)
		{
			throw new ArgumentException("Grain-128AEAD key must be 128 bits long");
		}
		workingIV = new byte[key.Length];
		workingKey = key;
		lfsr = new uint[STATE_SIZE];
		nfsr = new uint[STATE_SIZE];
		authAcc = new uint[2];
		authSr = new uint[2];
		Array.Copy(iV, 0, workingIV, 0, iV.Length);
		Reset();
	}

	private void InitGrain()
	{
		for (int i = 0; i < 320; i++)
		{
			uint output = GetOutput();
			nfsr = Shift(nfsr, (GetOutputNFSR() ^ lfsr[0] ^ output) & 1);
			lfsr = Shift(lfsr, (GetOutputLFSR() ^ output) & 1);
		}
		for (int j = 0; j < 8; j++)
		{
			for (int k = 0; k < 8; k++)
			{
				uint output2 = GetOutput();
				nfsr = Shift(nfsr, (GetOutputNFSR() ^ lfsr[0] ^ output2 ^ (uint)(workingKey[j] >> k)) & 1);
				lfsr = Shift(lfsr, (GetOutputLFSR() ^ output2 ^ (uint)(workingKey[j + 8] >> k)) & 1);
			}
		}
		for (int l = 0; l < 2; l++)
		{
			for (int m = 0; m < 32; m++)
			{
				uint output3 = GetOutput();
				nfsr = Shift(nfsr, (GetOutputNFSR() ^ lfsr[0]) & 1);
				lfsr = Shift(lfsr, GetOutputLFSR() & 1);
				authAcc[l] |= output3 << m;
			}
		}
		for (int n = 0; n < 2; n++)
		{
			for (int num = 0; num < 32; num++)
			{
				uint output4 = GetOutput();
				nfsr = Shift(nfsr, (GetOutputNFSR() ^ lfsr[0]) & 1);
				lfsr = Shift(lfsr, GetOutputLFSR() & 1);
				authSr[n] |= output4 << num;
			}
		}
		initialised = true;
	}

	private uint GetOutputNFSR()
	{
		uint num = nfsr[0];
		uint num2 = nfsr[0] >> 3;
		uint num3 = nfsr[0] >> 11;
		uint num4 = nfsr[0] >> 13;
		uint num5 = nfsr[0] >> 17;
		uint num6 = nfsr[0] >> 18;
		uint num7 = nfsr[0] >> 22;
		uint num8 = nfsr[0] >> 24;
		uint num9 = nfsr[0] >> 25;
		uint num10 = nfsr[0] >> 26;
		uint num11 = nfsr[0] >> 27;
		uint num12 = nfsr[1] >> 8;
		uint num13 = nfsr[1] >> 16;
		uint num14 = nfsr[1] >> 24;
		uint num15 = nfsr[1] >> 27;
		uint num16 = nfsr[1] >> 29;
		uint num17 = nfsr[2] >> 1;
		uint num18 = nfsr[2] >> 3;
		uint num19 = nfsr[2] >> 4;
		uint num20 = nfsr[2] >> 6;
		uint num21 = nfsr[2] >> 14;
		uint num22 = nfsr[2] >> 18;
		uint num23 = nfsr[2] >> 20;
		uint num24 = nfsr[2] >> 24;
		uint num25 = nfsr[2] >> 27;
		uint num26 = nfsr[2] >> 28;
		uint num27 = nfsr[2] >> 29;
		uint num28 = nfsr[2] >> 31;
		uint num29 = nfsr[3];
		return (num ^ num10 ^ num14 ^ num25 ^ num29 ^ (num2 & num18) ^ (num3 & num4) ^ (num5 & num6) ^ (num11 & num15) ^ (num12 & num13) ^ (num16 & num17) ^ (num19 & num23) ^ (num7 & num8 & num9) ^ (num20 & num21 & num22) ^ (num24 & num26 & num27 & num28)) & 1;
	}

	private uint GetOutputLFSR()
	{
		uint num = lfsr[0];
		uint num2 = lfsr[0] >> 7;
		uint num3 = lfsr[1] >> 6;
		uint num4 = lfsr[2] >> 6;
		uint num5 = lfsr[2] >> 17;
		uint num6 = lfsr[3];
		return (num ^ num2 ^ num3 ^ num4 ^ num5 ^ num6) & 1;
	}

	private uint GetOutput()
	{
		uint num = nfsr[0] >> 2;
		uint num2 = nfsr[0] >> 12;
		uint num3 = nfsr[0] >> 15;
		uint num4 = nfsr[1] >> 4;
		uint num5 = nfsr[1] >> 13;
		uint num6 = nfsr[2];
		uint num7 = nfsr[2] >> 9;
		uint num8 = nfsr[2] >> 25;
		uint num9 = nfsr[2] >> 31;
		uint num10 = lfsr[0] >> 8;
		uint num11 = lfsr[0] >> 13;
		uint num12 = lfsr[0] >> 20;
		uint num13 = lfsr[1] >> 10;
		uint num14 = lfsr[1] >> 28;
		uint num15 = lfsr[2] >> 15;
		uint num16 = lfsr[2] >> 29;
		uint num17 = lfsr[2] >> 30;
		return ((num2 & num10) ^ (num11 & num12) ^ (num9 & num13) ^ (num14 & num15) ^ (num2 & num9 & num17) ^ num16 ^ num ^ num3 ^ num4 ^ num5 ^ num6 ^ num7 ^ num8) & 1;
	}

	private uint[] Shift(uint[] array, uint val)
	{
		array[0] = (array[0] >> 1) | (array[1] << 31);
		array[1] = (array[1] >> 1) | (array[2] << 31);
		array[2] = (array[2] >> 1) | (array[3] << 31);
		array[3] = (array[3] >> 1) | (val << 31);
		return array;
	}

	private void SetKey(byte[] keyBytes, byte[] ivBytes)
	{
		ivBytes[12] = byte.MaxValue;
		ivBytes[13] = byte.MaxValue;
		ivBytes[14] = byte.MaxValue;
		ivBytes[15] = 127;
		workingKey = keyBytes;
		workingIV = ivBytes;
		Pack.LE_To_UInt32(workingKey, 0, nfsr);
		Pack.LE_To_UInt32(workingIV, 0, lfsr);
	}

	public int ProcessBytes(byte[] input, int inOff, int len, byte[] output, int outOff)
	{
		Check.DataLength(input, inOff, len, "input buffer too short");
		Check.OutputLength(output, outOff, len, "output buffer too short");
		if (!initialised)
		{
			throw new ArgumentException(AlgorithmName + " not initialised");
		}
		if (!aadFinished)
		{
			DoProcessAADBytes(aadData.GetBuffer(), 0, (int)aadData.Length);
			aadFinished = true;
		}
		GetKeyStream(input, inOff, len, output, outOff);
		return len;
	}

	public void Reset()
	{
		Reset(clearMac: true);
	}

	private void Reset(bool clearMac)
	{
		if (clearMac)
		{
			mac = null;
		}
		aadData.SetLength(0L);
		aadFinished = false;
		SetKey(workingKey, workingIV);
		InitGrain();
	}

	private void GetKeyStream(byte[] input, int inOff, int len, byte[] ciphertext, int outOff)
	{
		for (int i = 0; i < len; i++)
		{
			uint num = 0u;
			uint num2 = input[inOff + i];
			for (int j = 0; j < 8; j++)
			{
				uint output = GetOutput();
				nfsr = Shift(nfsr, (GetOutputNFSR() ^ lfsr[0]) & 1);
				lfsr = Shift(lfsr, GetOutputLFSR() & 1);
				uint num3 = (num2 >> j) & 1;
				num |= (num3 ^ output) << j;
				uint num4 = 0 - num3;
				authAcc[0] ^= authSr[0] & num4;
				authAcc[1] ^= authSr[1] & num4;
				AuthShift(GetOutput());
				nfsr = Shift(nfsr, (GetOutputNFSR() ^ lfsr[0]) & 1);
				lfsr = Shift(lfsr, GetOutputLFSR() & 1);
			}
			ciphertext[outOff + i] = (byte)num;
		}
	}

	public byte ReturnByte(byte input)
	{
		if (!initialised)
		{
			throw new ArgumentException(AlgorithmName + " not initialised");
		}
		byte[] input2 = new byte[1] { input };
		byte[] array = new byte[1];
		GetKeyStream(input2, 0, 1, array, 0);
		return array[0];
	}

	public void ProcessAadByte(byte input)
	{
		if (aadFinished)
		{
			throw new ArgumentException("associated data must be added before plaintext/ciphertext");
		}
		aadData.WriteByte(input);
	}

	public void ProcessAadBytes(byte[] input, int inOff, int len)
	{
		if (aadFinished)
		{
			throw new ArgumentException("associated data must be added before plaintext/ciphertext");
		}
		aadData.Write(input, inOff, len);
	}

	private void Accumulate()
	{
		authAcc[0] ^= authSr[0];
		authAcc[1] ^= authSr[1];
	}

	private void AuthShift(uint val)
	{
		authSr[0] = (authSr[0] >> 1) | (authSr[1] << 31);
		authSr[1] = (authSr[1] >> 1) | (val << 31);
	}

	public int ProcessByte(byte input, byte[] output, int outOff)
	{
		return ProcessBytes(new byte[1] { input }, 0, 1, output, outOff);
	}

	private void DoProcessAADBytes(byte[] input, int inOff, int len)
	{
		byte[] array;
		int num;
		if (len < 128)
		{
			array = new byte[1 + len];
			array[0] = (byte)len;
			num = 0;
		}
		else
		{
			num = LenLength(len);
			array = new byte[num + 1 + len];
			array[0] = (byte)(0x80 | num);
			uint num2 = (uint)len;
			for (int i = 0; i < num; i++)
			{
				array[1 + i] = (byte)num2;
				num2 >>= 8;
			}
		}
		for (int j = 0; j < len; j++)
		{
			array[1 + num + j] = input[inOff + j];
		}
		foreach (uint num3 in array)
		{
			for (int l = 0; l < 8; l++)
			{
				nfsr = Shift(nfsr, (GetOutputNFSR() ^ lfsr[0]) & 1);
				lfsr = Shift(lfsr, GetOutputLFSR() & 1);
				uint num4 = (num3 >> l) & 1;
				uint num5 = 0 - num4;
				authAcc[0] ^= authSr[0] & num5;
				authAcc[1] ^= authSr[1] & num5;
				AuthShift(GetOutput());
				nfsr = Shift(nfsr, (GetOutputNFSR() ^ lfsr[0]) & 1);
				lfsr = Shift(lfsr, GetOutputLFSR() & 1);
			}
		}
	}

	public int DoFinal(byte[] output, int outOff)
	{
		if (!aadFinished)
		{
			DoProcessAADBytes(aadData.GetBuffer(), 0, (int)aadData.Length);
			aadFinished = true;
		}
		Accumulate();
		mac = Pack.UInt32_To_LE(authAcc);
		Array.Copy(mac, 0, output, outOff, mac.Length);
		Reset(clearMac: false);
		return mac.Length;
	}

	public byte[] GetMac()
	{
		return mac;
	}

	public int GetUpdateOutputSize(int len)
	{
		return len;
	}

	public int GetOutputSize(int len)
	{
		return len + 8;
	}

	private static int LenLength(int v)
	{
		if ((v & 0xFF) == v)
		{
			return 1;
		}
		if ((v & 0xFFFF) == v)
		{
			return 2;
		}
		if ((v & 0xFFFFFF) == v)
		{
			return 3;
		}
		return 4;
	}
}
