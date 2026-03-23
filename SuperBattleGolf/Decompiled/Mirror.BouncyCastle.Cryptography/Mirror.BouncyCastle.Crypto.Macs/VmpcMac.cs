using System;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Crypto.Macs;

public class VmpcMac : IMac
{
	private byte g;

	private byte n;

	private byte[] P;

	private byte s;

	private readonly byte[] T = new byte[32];

	private byte[] workingIV;

	private byte[] workingKey;

	private byte x1;

	private byte x2;

	private byte x3;

	private byte x4;

	public virtual string AlgorithmName => "VMPC-MAC";

	public virtual int DoFinal(byte[] output, int outOff)
	{
		for (int i = 1; i < 25; i++)
		{
			s = P[(s + P[n & 0xFF]) & 0xFF];
			x4 = P[(x4 + x3 + i) & 0xFF];
			x3 = P[(x3 + x2 + i) & 0xFF];
			x2 = P[(x2 + x1 + i) & 0xFF];
			x1 = P[(x1 + s + i) & 0xFF];
			T[g & 0x1F] = (byte)(T[g & 0x1F] ^ x1);
			T[(g + 1) & 0x1F] = (byte)(T[(g + 1) & 0x1F] ^ x2);
			T[(g + 2) & 0x1F] = (byte)(T[(g + 2) & 0x1F] ^ x3);
			T[(g + 3) & 0x1F] = (byte)(T[(g + 3) & 0x1F] ^ x4);
			g = (byte)((g + 4) & 0x1F);
			byte b = P[n & 0xFF];
			P[n & 0xFF] = P[s & 0xFF];
			P[s & 0xFF] = b;
			n = (byte)((n + 1) & 0xFF);
		}
		for (int j = 0; j < 768; j++)
		{
			s = P[(s + P[j & 0xFF] + T[j & 0x1F]) & 0xFF];
			byte b2 = P[j & 0xFF];
			P[j & 0xFF] = P[s & 0xFF];
			P[s & 0xFF] = b2;
		}
		byte[] array = new byte[20];
		for (int k = 0; k < 20; k++)
		{
			s = P[(s + P[k & 0xFF]) & 0xFF];
			array[k] = P[(P[P[s & 0xFF] & 0xFF] + 1) & 0xFF];
			byte b3 = P[k & 0xFF];
			P[k & 0xFF] = P[s & 0xFF];
			P[s & 0xFF] = b3;
		}
		Array.Copy(array, 0, output, outOff, array.Length);
		Reset();
		return array.Length;
	}

	public virtual int GetMacSize()
	{
		return 20;
	}

	public virtual void Init(ICipherParameters parameters)
	{
		if (!(parameters is ParametersWithIV parametersWithIV))
		{
			throw new ArgumentException("VMPC-MAC Init parameters must include an IV", "parameters");
		}
		if (!(parametersWithIV.Parameters is KeyParameter { KeyLength: var keyLength } keyParameter))
		{
			throw new ArgumentException("VMPC-MAC Init parameters must include a key", "parameters");
		}
		if (keyLength < 16 || keyLength > 64)
		{
			throw new ArgumentException("VMPC requires 16 to 64 bytes of key");
		}
		int iVLength = parametersWithIV.IVLength;
		if (iVLength < 16 || iVLength > 64)
		{
			throw new ArgumentException("VMPC requires 16 to 64 bytes of IV");
		}
		workingKey = keyParameter.GetKey();
		workingIV = parametersWithIV.GetIV();
		Reset();
	}

	private void InitKey(byte[] keyBytes, byte[] ivBytes)
	{
		n = 0;
		s = 0;
		P = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			P[i] = (byte)i;
		}
		VmpcEngine.KsaRound(P, ref s, keyBytes);
		VmpcEngine.KsaRound(P, ref s, ivBytes);
	}

	public virtual void Reset()
	{
		InitKey(workingKey, workingIV);
		g = (x1 = (x2 = (x3 = (x4 = (n = 0)))));
		Array.Clear(T, 0, T.Length);
	}

	public virtual void Update(byte input)
	{
		byte b = P[n];
		s = P[(s + b) & 0xFF];
		byte b2 = P[s];
		byte b3 = (byte)(input ^ P[(P[b2] + 1) & 0xFF]);
		x4 = P[(x4 + x3) & 0xFF];
		x3 = P[(x3 + x2) & 0xFF];
		x2 = P[(x2 + x1) & 0xFF];
		x1 = P[(x1 + s + b3) & 0xFF];
		T[g & 0x1F] = (byte)(T[g & 0x1F] ^ x1);
		T[(g + 1) & 0x1F] = (byte)(T[(g + 1) & 0x1F] ^ x2);
		T[(g + 2) & 0x1F] = (byte)(T[(g + 2) & 0x1F] ^ x3);
		T[(g + 3) & 0x1F] = (byte)(T[(g + 3) & 0x1F] ^ x4);
		g = (byte)((g + 4) & 0x1F);
		P[n] = b2;
		P[s] = b;
		n++;
	}

	public virtual void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		Check.DataLength(input, inOff, inLen, "input buffer too short");
		for (int i = 0; i < inLen; i++)
		{
			byte b = P[n];
			s = P[(s + b) & 0xFF];
			byte b2 = P[s];
			byte b3 = (byte)(input[inOff + i] ^ P[(P[b2] + 1) & 0xFF]);
			x4 = P[(x4 + x3) & 0xFF];
			x3 = P[(x3 + x2) & 0xFF];
			x2 = P[(x2 + x1) & 0xFF];
			x1 = P[(x1 + s + b3) & 0xFF];
			T[g & 0x1F] = (byte)(T[g & 0x1F] ^ x1);
			T[(g + 1) & 0x1F] = (byte)(T[(g + 1) & 0x1F] ^ x2);
			T[(g + 2) & 0x1F] = (byte)(T[(g + 2) & 0x1F] ^ x3);
			T[(g + 3) & 0x1F] = (byte)(T[(g + 3) & 0x1F] ^ x4);
			g = (byte)((g + 4) & 0x1F);
			P[n] = b2;
			P[s] = b;
			n++;
		}
	}
}
