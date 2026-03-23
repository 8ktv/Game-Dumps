using System;
using Mirror.BouncyCastle.Crypto.Modes.Gcm;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Modes;

public sealed class GcmBlockCipher : IAeadBlockCipher, IAeadCipher
{
	private const int BlockSize = 16;

	private readonly IBlockCipher cipher;

	private readonly IGcmMultiplier multiplier;

	private IGcmExponentiator exp;

	private bool forEncryption;

	private bool initialised;

	private int macSize;

	private byte[] lastKey;

	private byte[] nonce;

	private byte[] initialAssociatedText;

	private byte[] H;

	private byte[] J0;

	private byte[] bufBlock;

	private byte[] macBlock;

	private byte[] S;

	private byte[] S_at;

	private byte[] S_atPre;

	private byte[] counter;

	private uint counter32;

	private uint blocksRemaining;

	private int bufOff;

	private ulong totalLength;

	private byte[] atBlock;

	private int atBlockPos;

	private ulong atLength;

	private ulong atLengthPre;

	public string AlgorithmName => cipher.AlgorithmName + "/GCM";

	public IBlockCipher UnderlyingCipher => cipher;

	internal static IGcmMultiplier CreateGcmMultiplier()
	{
		if (BasicGcmMultiplier.IsHardwareAccelerated)
		{
			return new BasicGcmMultiplier();
		}
		return new Tables4kGcmMultiplier();
	}

	public GcmBlockCipher(IBlockCipher c)
		: this(c, null)
	{
	}

	[Obsolete("Will be removed")]
	public GcmBlockCipher(IBlockCipher c, IGcmMultiplier m)
	{
		if (c.GetBlockSize() != 16)
		{
			throw new ArgumentException("cipher required with a block size of " + 16 + ".");
		}
		if (m == null)
		{
			m = CreateGcmMultiplier();
		}
		cipher = c;
		multiplier = m;
	}

	public int GetBlockSize()
	{
		return 16;
	}

	public void Init(bool forEncryption, ICipherParameters parameters)
	{
		this.forEncryption = forEncryption;
		macBlock = null;
		initialised = true;
		byte[] iV;
		KeyParameter keyParameter;
		if (parameters is AeadParameters aeadParameters)
		{
			iV = aeadParameters.GetNonce();
			initialAssociatedText = aeadParameters.GetAssociatedText();
			int num = aeadParameters.MacSize;
			if (num < 32 || num > 128 || num % 8 != 0)
			{
				throw new ArgumentException("Invalid value for MAC size: " + num);
			}
			macSize = num / 8;
			keyParameter = aeadParameters.Key;
		}
		else
		{
			if (!(parameters is ParametersWithIV parametersWithIV))
			{
				throw new ArgumentException("invalid parameters passed to GCM");
			}
			iV = parametersWithIV.GetIV();
			initialAssociatedText = null;
			macSize = 16;
			keyParameter = (KeyParameter)parametersWithIV.Parameters;
		}
		int num2 = (forEncryption ? 16 : (16 + macSize));
		bufBlock = new byte[num2];
		if (iV.Length < 1)
		{
			throw new ArgumentException("IV must be at least 1 byte");
		}
		if (forEncryption && nonce != null && Arrays.AreEqual(nonce, iV))
		{
			if (keyParameter == null)
			{
				throw new ArgumentException("cannot reuse nonce for GCM encryption");
			}
			if (lastKey != null && keyParameter.FixedTimeEquals(lastKey))
			{
				throw new ArgumentException("cannot reuse nonce for GCM encryption");
			}
		}
		nonce = iV;
		if (keyParameter != null)
		{
			lastKey = keyParameter.GetKey();
		}
		if (keyParameter != null)
		{
			cipher.Init(forEncryption: true, keyParameter);
			H = new byte[16];
			cipher.ProcessBlock(H, 0, H, 0);
			multiplier.Init(H);
			exp = null;
		}
		else if (H == null)
		{
			throw new ArgumentException("Key must be specified in initial Init");
		}
		J0 = new byte[16];
		if (nonce.Length == 12)
		{
			Array.Copy(nonce, 0, J0, 0, nonce.Length);
			J0[15] = 1;
		}
		else
		{
			gHASH(J0, nonce, nonce.Length);
			byte[] array = new byte[16];
			Pack.UInt64_To_BE((ulong)nonce.Length * 8uL, array, 8);
			gHASHBlock(J0, array);
		}
		S = new byte[16];
		S_at = new byte[16];
		S_atPre = new byte[16];
		atBlock = new byte[16];
		atBlockPos = 0;
		atLength = 0uL;
		atLengthPre = 0uL;
		counter = Arrays.Clone(J0);
		counter32 = Pack.BE_To_UInt32(counter, 12);
		blocksRemaining = 4294967294u;
		bufOff = 0;
		totalLength = 0uL;
		if (initialAssociatedText != null)
		{
			ProcessAadBytes(initialAssociatedText, 0, initialAssociatedText.Length);
		}
	}

	public byte[] GetMac()
	{
		if (macBlock != null)
		{
			return (byte[])macBlock.Clone();
		}
		return new byte[macSize];
	}

	public int GetOutputSize(int len)
	{
		int num = len + bufOff;
		if (forEncryption)
		{
			return num + macSize;
		}
		if (num >= macSize)
		{
			return num - macSize;
		}
		return 0;
	}

	public int GetUpdateOutputSize(int len)
	{
		int num = len + bufOff;
		if (!forEncryption)
		{
			if (num < macSize)
			{
				return 0;
			}
			num -= macSize;
		}
		return num - num % 16;
	}

	public void ProcessAadByte(byte input)
	{
		CheckStatus();
		atBlock[atBlockPos] = input;
		if (++atBlockPos == 16)
		{
			gHASHBlock(S_at, atBlock);
			atBlockPos = 0;
			atLength += 16uL;
		}
	}

	public void ProcessAadBytes(byte[] inBytes, int inOff, int len)
	{
		CheckStatus();
		if (atBlockPos > 0)
		{
			int num = 16 - atBlockPos;
			if (len < num)
			{
				Array.Copy(inBytes, inOff, atBlock, atBlockPos, len);
				atBlockPos += len;
				return;
			}
			Array.Copy(inBytes, inOff, atBlock, atBlockPos, num);
			gHASHBlock(S_at, atBlock);
			atLength += 16uL;
			inOff += num;
			len -= num;
		}
		int num2 = inOff + len - 16;
		while (inOff <= num2)
		{
			gHASHBlock(S_at, inBytes, inOff);
			atLength += 16uL;
			inOff += 16;
		}
		atBlockPos = 16 + num2 - inOff;
		Array.Copy(inBytes, inOff, atBlock, 0, atBlockPos);
	}

	private void InitCipher()
	{
		if (atLength != 0)
		{
			Array.Copy(S_at, 0, S_atPre, 0, 16);
			atLengthPre = atLength;
		}
		if (atBlockPos > 0)
		{
			gHASHPartial(S_atPre, atBlock, 0, atBlockPos);
			atLengthPre += (uint)atBlockPos;
		}
		if (atLengthPre != 0)
		{
			Array.Copy(S_atPre, 0, S, 0, 16);
		}
	}

	public int ProcessByte(byte input, byte[] output, int outOff)
	{
		CheckStatus();
		bufBlock[bufOff] = input;
		if (++bufOff == bufBlock.Length)
		{
			Check.OutputLength(output, outOff, 16, "output buffer too short");
			if (blocksRemaining == 0)
			{
				throw new InvalidOperationException("Attempt to process too many blocks");
			}
			blocksRemaining--;
			if (totalLength == 0L)
			{
				InitCipher();
			}
			if (forEncryption)
			{
				EncryptBlock(bufBlock, 0, output, outOff);
				bufOff = 0;
			}
			else
			{
				DecryptBlock(bufBlock, 0, output, outOff);
				Array.Copy(bufBlock, 16, bufBlock, 0, macSize);
				bufOff = macSize;
			}
			totalLength += 16uL;
			return 16;
		}
		return 0;
	}

	public int ProcessBytes(byte[] input, int inOff, int len, byte[] output, int outOff)
	{
		CheckStatus();
		Check.DataLength(input, inOff, len, "input buffer too short");
		int num = bufOff + len;
		if (forEncryption)
		{
			num &= -16;
			if (num > 0)
			{
				Check.OutputLength(output, outOff, num, "output buffer too short");
				uint num2 = (uint)num >> 4;
				if (blocksRemaining < num2)
				{
					throw new InvalidOperationException("Attempt to process too many blocks");
				}
				blocksRemaining -= num2;
				if (totalLength == 0L)
				{
					InitCipher();
				}
			}
			if (bufOff > 0)
			{
				int num3 = 16 - bufOff;
				if (len < num3)
				{
					Array.Copy(input, inOff, bufBlock, bufOff, len);
					bufOff += len;
					return 0;
				}
				Array.Copy(input, inOff, bufBlock, bufOff, num3);
				inOff += num3;
				len -= num3;
				EncryptBlock(bufBlock, 0, output, outOff);
				outOff += 16;
			}
			int num4 = inOff + len - 16;
			int num5 = num4 - 16;
			while (inOff <= num5)
			{
				EncryptBlocks2(input, inOff, output, outOff);
				inOff += 32;
				outOff += 32;
			}
			if (inOff <= num4)
			{
				EncryptBlock(input, inOff, output, outOff);
				inOff += 16;
			}
			bufOff = 16 + num4 - inOff;
			Array.Copy(input, inOff, bufBlock, 0, bufOff);
		}
		else
		{
			num -= macSize;
			num &= -16;
			if (num > 0)
			{
				Check.OutputLength(output, outOff, num, "output buffer too short");
				uint num6 = (uint)num >> 4;
				if (blocksRemaining < num6)
				{
					throw new InvalidOperationException("Attempt to process too many blocks");
				}
				blocksRemaining -= num6;
				if (totalLength == 0L)
				{
					InitCipher();
				}
			}
			int num7 = bufBlock.Length - bufOff;
			if (len < num7)
			{
				Array.Copy(input, inOff, bufBlock, bufOff, len);
				bufOff += len;
				return 0;
			}
			if (bufOff >= 16)
			{
				DecryptBlock(bufBlock, 0, output, outOff);
				outOff += 16;
				bufOff -= 16;
				Array.Copy(bufBlock, 16, bufBlock, 0, bufOff);
				num7 += 16;
				if (len < num7)
				{
					Array.Copy(input, inOff, bufBlock, bufOff, len);
					bufOff += len;
					totalLength += 16uL;
					return 16;
				}
			}
			int num8 = inOff + len - bufBlock.Length;
			int num9 = num8 - 16;
			num7 = 16 - bufOff;
			Array.Copy(input, inOff, bufBlock, bufOff, num7);
			inOff += num7;
			DecryptBlock(bufBlock, 0, output, outOff);
			outOff += 16;
			while (inOff <= num9)
			{
				DecryptBlocks2(input, inOff, output, outOff);
				inOff += 32;
				outOff += 32;
			}
			if (inOff <= num8)
			{
				DecryptBlock(input, inOff, output, outOff);
				inOff += 16;
			}
			bufOff = bufBlock.Length + num8 - inOff;
			Array.Copy(input, inOff, bufBlock, 0, bufOff);
		}
		totalLength += (uint)num;
		return num;
	}

	public int DoFinal(byte[] output, int outOff)
	{
		CheckStatus();
		int num = bufOff;
		if (forEncryption)
		{
			Check.OutputLength(output, outOff, num + macSize, "output buffer too short");
		}
		else
		{
			if (num < macSize)
			{
				throw new InvalidCipherTextException("data too short");
			}
			num -= macSize;
			Check.OutputLength(output, outOff, num, "output buffer too short");
		}
		if (totalLength == 0L)
		{
			InitCipher();
		}
		if (num > 0)
		{
			if (blocksRemaining == 0)
			{
				throw new InvalidOperationException("Attempt to process too many blocks");
			}
			blocksRemaining--;
			ProcessPartial(bufBlock, 0, num, output, outOff);
		}
		atLength += (uint)atBlockPos;
		if (atLength > atLengthPre)
		{
			if (atBlockPos > 0)
			{
				gHASHPartial(S_at, atBlock, 0, atBlockPos);
			}
			if (atLengthPre != 0)
			{
				GcmUtilities.Xor(S_at, S_atPre);
			}
			long pow = (long)(totalLength * 8 + 127 >> 7);
			byte[] array = new byte[16];
			if (exp == null)
			{
				exp = new BasicGcmExponentiator();
				exp.Init(H);
			}
			exp.ExponentiateX(pow, array);
			GcmUtilities.Multiply(S_at, array);
			GcmUtilities.Xor(S, S_at);
		}
		byte[] array2 = new byte[16];
		Pack.UInt64_To_BE(atLength * 8, array2, 0);
		Pack.UInt64_To_BE(totalLength * 8, array2, 8);
		gHASHBlock(S, array2);
		byte[] array3 = new byte[16];
		cipher.ProcessBlock(J0, 0, array3, 0);
		GcmUtilities.Xor(array3, S);
		int num2 = num;
		macBlock = new byte[macSize];
		Array.Copy(array3, 0, macBlock, 0, macSize);
		if (forEncryption)
		{
			Array.Copy(macBlock, 0, output, outOff + bufOff, macSize);
			num2 += macSize;
		}
		else
		{
			byte[] array4 = new byte[macSize];
			Array.Copy(bufBlock, num, array4, 0, macSize);
			if (!Arrays.FixedTimeEquals(macBlock, array4))
			{
				throw new InvalidCipherTextException("mac check in GCM failed");
			}
		}
		Reset(clearMac: false);
		return num2;
	}

	public void Reset()
	{
		Reset(clearMac: true);
	}

	private void Reset(bool clearMac)
	{
		S = new byte[16];
		S_at = new byte[16];
		S_atPre = new byte[16];
		atBlock = new byte[16];
		atBlockPos = 0;
		atLength = 0uL;
		atLengthPre = 0uL;
		counter = Arrays.Clone(J0);
		counter32 = Pack.BE_To_UInt32(counter, 12);
		blocksRemaining = 4294967294u;
		bufOff = 0;
		totalLength = 0uL;
		if (bufBlock != null)
		{
			Arrays.Fill(bufBlock, 0);
		}
		if (clearMac)
		{
			macBlock = null;
		}
		if (forEncryption)
		{
			initialised = false;
		}
		else if (initialAssociatedText != null)
		{
			ProcessAadBytes(initialAssociatedText, 0, initialAssociatedText.Length);
		}
	}

	private void DecryptBlock(byte[] inBuf, int inOff, byte[] outBuf, int outOff)
	{
		byte[] array = new byte[16];
		GetNextCtrBlock(array);
		for (int i = 0; i < 16; i += 4)
		{
			byte b = inBuf[inOff + i];
			byte b2 = inBuf[inOff + i + 1];
			byte b3 = inBuf[inOff + i + 2];
			byte b4 = inBuf[inOff + i + 3];
			S[i] ^= b;
			S[i + 1] ^= b2;
			S[i + 2] ^= b3;
			S[i + 3] ^= b4;
			outBuf[outOff + i] = (byte)(b ^ array[i]);
			outBuf[outOff + i + 1] = (byte)(b2 ^ array[i + 1]);
			outBuf[outOff + i + 2] = (byte)(b3 ^ array[i + 2]);
			outBuf[outOff + i + 3] = (byte)(b4 ^ array[i + 3]);
		}
		multiplier.MultiplyH(S);
	}

	private void DecryptBlocks2(byte[] inBuf, int inOff, byte[] outBuf, int outOff)
	{
		byte[] array = new byte[16];
		GetNextCtrBlock(array);
		for (int i = 0; i < 16; i += 4)
		{
			byte b = inBuf[inOff + i];
			byte b2 = inBuf[inOff + i + 1];
			byte b3 = inBuf[inOff + i + 2];
			byte b4 = inBuf[inOff + i + 3];
			S[i] ^= b;
			S[i + 1] ^= b2;
			S[i + 2] ^= b3;
			S[i + 3] ^= b4;
			outBuf[outOff + i] = (byte)(b ^ array[i]);
			outBuf[outOff + i + 1] = (byte)(b2 ^ array[i + 1]);
			outBuf[outOff + i + 2] = (byte)(b3 ^ array[i + 2]);
			outBuf[outOff + i + 3] = (byte)(b4 ^ array[i + 3]);
		}
		multiplier.MultiplyH(S);
		inOff += 16;
		outOff += 16;
		GetNextCtrBlock(array);
		for (int j = 0; j < 16; j += 4)
		{
			byte b5 = inBuf[inOff + j];
			byte b6 = inBuf[inOff + j + 1];
			byte b7 = inBuf[inOff + j + 2];
			byte b8 = inBuf[inOff + j + 3];
			S[j] ^= b5;
			S[j + 1] ^= b6;
			S[j + 2] ^= b7;
			S[j + 3] ^= b8;
			outBuf[outOff + j] = (byte)(b5 ^ array[j]);
			outBuf[outOff + j + 1] = (byte)(b6 ^ array[j + 1]);
			outBuf[outOff + j + 2] = (byte)(b7 ^ array[j + 2]);
			outBuf[outOff + j + 3] = (byte)(b8 ^ array[j + 3]);
		}
		multiplier.MultiplyH(S);
	}

	private void EncryptBlock(byte[] inBuf, int inOff, byte[] outBuf, int outOff)
	{
		byte[] array = new byte[16];
		GetNextCtrBlock(array);
		for (int i = 0; i < 16; i += 4)
		{
			byte b = (byte)(array[i] ^ inBuf[inOff + i]);
			byte b2 = (byte)(array[i + 1] ^ inBuf[inOff + i + 1]);
			byte b3 = (byte)(array[i + 2] ^ inBuf[inOff + i + 2]);
			byte b4 = (byte)(array[i + 3] ^ inBuf[inOff + i + 3]);
			S[i] ^= b;
			S[i + 1] ^= b2;
			S[i + 2] ^= b3;
			S[i + 3] ^= b4;
			outBuf[outOff + i] = b;
			outBuf[outOff + i + 1] = b2;
			outBuf[outOff + i + 2] = b3;
			outBuf[outOff + i + 3] = b4;
		}
		multiplier.MultiplyH(S);
	}

	private void EncryptBlocks2(byte[] inBuf, int inOff, byte[] outBuf, int outOff)
	{
		byte[] array = new byte[16];
		GetNextCtrBlock(array);
		for (int i = 0; i < 16; i += 4)
		{
			byte b = (byte)(array[i] ^ inBuf[inOff + i]);
			byte b2 = (byte)(array[i + 1] ^ inBuf[inOff + i + 1]);
			byte b3 = (byte)(array[i + 2] ^ inBuf[inOff + i + 2]);
			byte b4 = (byte)(array[i + 3] ^ inBuf[inOff + i + 3]);
			S[i] ^= b;
			S[i + 1] ^= b2;
			S[i + 2] ^= b3;
			S[i + 3] ^= b4;
			outBuf[outOff + i] = b;
			outBuf[outOff + i + 1] = b2;
			outBuf[outOff + i + 2] = b3;
			outBuf[outOff + i + 3] = b4;
		}
		multiplier.MultiplyH(S);
		inOff += 16;
		outOff += 16;
		GetNextCtrBlock(array);
		for (int j = 0; j < 16; j += 4)
		{
			byte b5 = (byte)(array[j] ^ inBuf[inOff + j]);
			byte b6 = (byte)(array[j + 1] ^ inBuf[inOff + j + 1]);
			byte b7 = (byte)(array[j + 2] ^ inBuf[inOff + j + 2]);
			byte b8 = (byte)(array[j + 3] ^ inBuf[inOff + j + 3]);
			S[j] ^= b5;
			S[j + 1] ^= b6;
			S[j + 2] ^= b7;
			S[j + 3] ^= b8;
			outBuf[outOff + j] = b5;
			outBuf[outOff + j + 1] = b6;
			outBuf[outOff + j + 2] = b7;
			outBuf[outOff + j + 3] = b8;
		}
		multiplier.MultiplyH(S);
	}

	private void GetNextCtrBlock(byte[] block)
	{
		Pack.UInt32_To_BE(++counter32, counter, 12);
		cipher.ProcessBlock(counter, 0, block, 0);
	}

	private void ProcessPartial(byte[] buf, int off, int len, byte[] output, int outOff)
	{
		byte[] array = new byte[16];
		GetNextCtrBlock(array);
		if (forEncryption)
		{
			GcmUtilities.Xor(buf, off, array, 0, len);
			gHASHPartial(S, buf, off, len);
		}
		else
		{
			gHASHPartial(S, buf, off, len);
			GcmUtilities.Xor(buf, off, array, 0, len);
		}
		Array.Copy(buf, off, output, outOff, len);
		totalLength += (uint)len;
	}

	private void gHASH(byte[] Y, byte[] b, int len)
	{
		for (int i = 0; i < len; i += 16)
		{
			int len2 = System.Math.Min(len - i, 16);
			gHASHPartial(Y, b, i, len2);
		}
	}

	private void gHASHBlock(byte[] Y, byte[] b)
	{
		GcmUtilities.Xor(Y, b);
		multiplier.MultiplyH(Y);
	}

	private void gHASHBlock(byte[] Y, byte[] b, int off)
	{
		GcmUtilities.Xor(Y, b, off);
		multiplier.MultiplyH(Y);
	}

	private void gHASHPartial(byte[] Y, byte[] b, int off, int len)
	{
		GcmUtilities.Xor(Y, b, off, len);
		multiplier.MultiplyH(Y);
	}

	private void CheckStatus()
	{
		if (!initialised)
		{
			if (forEncryption)
			{
				throw new InvalidOperationException("GCM cipher cannot be reused for encryption");
			}
			throw new InvalidOperationException("GCM cipher needs to be initialized");
		}
	}
}
