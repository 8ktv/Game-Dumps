using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Crypto.Macs;

public class Poly1305 : IMac
{
	private const int BlockSize = 16;

	private readonly IBlockCipher cipher;

	private uint r0;

	private uint r1;

	private uint r2;

	private uint r3;

	private uint r4;

	private uint s1;

	private uint s2;

	private uint s3;

	private uint s4;

	private uint k0;

	private uint k1;

	private uint k2;

	private uint k3;

	private byte[] currentBlock = new byte[16];

	private int currentBlockOffset;

	private uint h0;

	private uint h1;

	private uint h2;

	private uint h3;

	private uint h4;

	public string AlgorithmName
	{
		get
		{
			if (cipher != null)
			{
				return "Poly1305-" + cipher.AlgorithmName;
			}
			return "Poly1305";
		}
	}

	public Poly1305()
	{
		cipher = null;
	}

	public Poly1305(IBlockCipher cipher)
	{
		if (cipher.GetBlockSize() != 16)
		{
			throw new ArgumentException("Poly1305 requires a 128 bit block cipher.");
		}
		this.cipher = cipher;
	}

	public void Init(ICipherParameters parameters)
	{
		byte[] nonce = null;
		if (cipher != null)
		{
			ParametersWithIV obj = (parameters as ParametersWithIV) ?? throw new ArgumentException("Poly1305 requires an IV when used with a block cipher.", "parameters");
			nonce = obj.GetIV();
			parameters = obj.Parameters;
		}
		if (!(parameters is KeyParameter keyParameter))
		{
			throw new ArgumentException("Poly1305 requires a key.");
		}
		SetKey(keyParameter, nonce);
		Reset();
	}

	private void SetKey(KeyParameter keyParameter, byte[] nonce)
	{
		byte[] key = keyParameter.GetKey();
		if (key.Length != 32)
		{
			throw new ArgumentException("Poly1305 key must be 256 bits.");
		}
		if (cipher != null && (nonce == null || nonce.Length != 16))
		{
			throw new ArgumentException("Poly1305 requires a 128 bit IV.");
		}
		uint num = Pack.LE_To_UInt32(key, 0);
		uint num2 = Pack.LE_To_UInt32(key, 4);
		uint num3 = Pack.LE_To_UInt32(key, 8);
		uint num4 = Pack.LE_To_UInt32(key, 12);
		r0 = num & 0x3FFFFFF;
		r1 = ((num >> 26) | (num2 << 6)) & 0x3FFFF03;
		r2 = ((num2 >> 20) | (num3 << 12)) & 0x3FFC0FF;
		r3 = ((num3 >> 14) | (num4 << 18)) & 0x3F03FFF;
		r4 = (num4 >> 8) & 0xFFFFF;
		s1 = r1 * 5;
		s2 = r2 * 5;
		s3 = r3 * 5;
		s4 = r4 * 5;
		if (cipher == null)
		{
			k0 = Pack.LE_To_UInt32(key, 16);
			k1 = Pack.LE_To_UInt32(key, 20);
			k2 = Pack.LE_To_UInt32(key, 24);
			k3 = Pack.LE_To_UInt32(key, 28);
		}
		else
		{
			byte[] array = new byte[16];
			cipher.Init(forEncryption: true, new KeyParameter(key, 16, 16));
			cipher.ProcessBlock(nonce, 0, array, 0);
			k0 = Pack.LE_To_UInt32(array, 0);
			k1 = Pack.LE_To_UInt32(array, 4);
			k2 = Pack.LE_To_UInt32(array, 8);
			k3 = Pack.LE_To_UInt32(array, 12);
		}
	}

	public int GetMacSize()
	{
		return 16;
	}

	public void Update(byte input)
	{
		currentBlock[currentBlockOffset++] = input;
		if (currentBlockOffset == 16)
		{
			ProcessBlock(currentBlock, 0);
			currentBlockOffset = 0;
		}
	}

	public void BlockUpdate(byte[] input, int inOff, int len)
	{
		Check.DataLength(input, inOff, len, "input buffer too short");
		int num = 16 - currentBlockOffset;
		if (len < num)
		{
			Array.Copy(input, inOff, currentBlock, currentBlockOffset, len);
			currentBlockOffset += len;
			return;
		}
		int num2 = 0;
		if (currentBlockOffset > 0)
		{
			Array.Copy(input, inOff, currentBlock, currentBlockOffset, num);
			num2 = num;
			ProcessBlock(currentBlock, 0);
		}
		int length;
		while ((length = len - num2) >= 16)
		{
			ProcessBlock(input, inOff + num2);
			num2 += 16;
		}
		Array.Copy(input, inOff + num2, currentBlock, 0, length);
		currentBlockOffset = length;
	}

	private void ProcessBlock(byte[] buf, int off)
	{
		uint num = Pack.LE_To_UInt32(buf, off);
		uint num2 = Pack.LE_To_UInt32(buf, off + 4);
		uint num3 = Pack.LE_To_UInt32(buf, off + 8);
		uint num4 = Pack.LE_To_UInt32(buf, off + 12);
		h0 += num & 0x3FFFFFF;
		h1 += ((num2 << 6) | (num >> 26)) & 0x3FFFFFF;
		h2 += ((num3 << 12) | (num2 >> 20)) & 0x3FFFFFF;
		h3 += ((num4 << 18) | (num3 >> 14)) & 0x3FFFFFF;
		h4 += 0x1000000 | (num4 >> 8);
		ulong num5 = (ulong)((long)h0 * (long)r0 + (long)h1 * (long)s4 + (long)h2 * (long)s3 + (long)h3 * (long)s2 + (long)h4 * (long)s1);
		ulong num6 = (ulong)((long)h0 * (long)r1 + (long)h1 * (long)r0 + (long)h2 * (long)s4 + (long)h3 * (long)s3 + (long)h4 * (long)s2);
		ulong num7 = (ulong)((long)h0 * (long)r2 + (long)h1 * (long)r1 + (long)h2 * (long)r0 + (long)h3 * (long)s4 + (long)h4 * (long)s3);
		ulong num8 = (ulong)((long)h0 * (long)r3 + (long)h1 * (long)r2 + (long)h2 * (long)r1 + (long)h3 * (long)r0 + (long)h4 * (long)s4);
		ulong num9 = (ulong)((long)h0 * (long)r4 + (long)h1 * (long)r3 + (long)h2 * (long)r2 + (long)h3 * (long)r1 + (long)h4 * (long)r0);
		h0 = (uint)((int)num5 & 0x3FFFFFF);
		num6 += num5 >> 26;
		h1 = (uint)((int)num6 & 0x3FFFFFF);
		num7 += num6 >> 26;
		h2 = (uint)((int)num7 & 0x3FFFFFF);
		num8 += num7 >> 26;
		h3 = (uint)((int)num8 & 0x3FFFFFF);
		num9 += num8 >> 26;
		h4 = (uint)((int)num9 & 0x3FFFFFF);
		h0 += (uint)((int)(num9 >> 26) * 5);
		h1 += h0 >> 26;
		h0 &= 67108863u;
	}

	public int DoFinal(byte[] output, int outOff)
	{
		Check.OutputLength(output, outOff, 16, "output buffer too short");
		if (currentBlockOffset > 0)
		{
			if (currentBlockOffset < 16)
			{
				currentBlock[currentBlockOffset++] = 1;
				while (currentBlockOffset < 16)
				{
					currentBlock[currentBlockOffset++] = 0;
				}
				h4 -= 16777216u;
			}
			ProcessBlock(currentBlock, 0);
		}
		h0 += 5u;
		h1 += h0 >> 26;
		h0 &= 67108863u;
		h2 += h1 >> 26;
		h1 &= 67108863u;
		h3 += h2 >> 26;
		h2 &= 67108863u;
		h4 += h3 >> 26;
		h3 &= 67108863u;
		long num = (int)(((h4 >> 26) - 1) * 5) + ((long)k0 + (long)(h0 | (h1 << 26)));
		Pack.UInt32_To_LE((uint)num, output, outOff);
		long num2 = (num >> 32) + ((long)k1 + (long)((h1 >> 6) | (h2 << 20)));
		Pack.UInt32_To_LE((uint)num2, output, outOff + 4);
		long num3 = (num2 >> 32) + ((long)k2 + (long)((h2 >> 12) | (h3 << 14)));
		Pack.UInt32_To_LE((uint)num3, output, outOff + 8);
		Pack.UInt32_To_LE((uint)((num3 >> 32) + ((long)k3 + (long)((h3 >> 18) | (h4 << 8)))), output, outOff + 12);
		Reset();
		return 16;
	}

	public void Reset()
	{
		currentBlockOffset = 0;
		h0 = (h1 = (h2 = (h3 = (h4 = 0u))));
	}
}
