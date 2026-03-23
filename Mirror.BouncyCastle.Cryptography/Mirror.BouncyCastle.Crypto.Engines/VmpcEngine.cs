using System;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class VmpcEngine : IStreamCipher
{
	protected byte n;

	protected byte[] P;

	protected byte s;

	protected byte[] workingIV;

	protected byte[] workingKey;

	public virtual string AlgorithmName => "VMPC";

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		if (!(parameters is ParametersWithIV parametersWithIV))
		{
			throw new ArgumentException("VMPC Init parameters must include an IV");
		}
		if (!(parametersWithIV.Parameters is KeyParameter { KeyLength: var keyLength } keyParameter))
		{
			throw new ArgumentException("VMPC Init parameters must include a key");
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
		InitKey(workingKey, workingIV);
	}

	protected virtual void InitKey(byte[] keyBytes, byte[] ivBytes)
	{
		n = 0;
		s = 0;
		P = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			P[i] = (byte)i;
		}
		KsaRound(P, ref s, keyBytes);
		KsaRound(P, ref s, ivBytes);
	}

	public virtual void ProcessBytes(byte[] input, int inOff, int len, byte[] output, int outOff)
	{
		Check.DataLength(input, inOff, len, "input buffer too short");
		Check.OutputLength(output, outOff, len, "output buffer too short");
		for (int i = 0; i < len; i++)
		{
			byte b = P[n];
			s = P[(s + b) & 0xFF];
			byte b2 = P[s];
			output[outOff + i] = (byte)(input[inOff + i] ^ P[(P[b2] + 1) & 0xFF]);
			P[n] = b2;
			P[s] = b;
			n++;
		}
	}

	public virtual void Reset()
	{
		InitKey(workingKey, workingIV);
	}

	public virtual byte ReturnByte(byte input)
	{
		byte b = P[n];
		s = P[(s + b) & 0xFF];
		byte b2 = P[s];
		byte result = (byte)(input ^ P[(P[b2] + 1) & 0xFF]);
		P[n] = b2;
		P[s] = b;
		n++;
		return result;
	}

	internal static void KsaRound(byte[] P, ref byte S, byte[] input)
	{
		byte b = S;
		int num = input.Length;
		int num2 = 0;
		for (int i = 0; i < 768; i++)
		{
			byte b2 = P[i & 0xFF];
			b = P[(b + b2 + input[num2]) & 0xFF];
			int num3 = num2 + 1 - num;
			num2 = num3 + (num & (num3 >> 31));
			P[i & 0xFF] = P[b];
			P[b] = b2;
		}
		S = b;
	}
}
