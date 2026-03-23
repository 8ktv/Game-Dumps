using System;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public sealed class AsconEngine : IAeadCipher
{
	public enum AsconParameters
	{
		ascon80pq,
		ascon128a,
		ascon128
	}

	private enum State
	{
		Uninitialized,
		EncInit,
		EncAad,
		EncData,
		EncFinal,
		DecInit,
		DecAad,
		DecData,
		DecFinal
	}

	private readonly AsconParameters asconParameters;

	private readonly int CRYPTO_KEYBYTES;

	private readonly int CRYPTO_ABYTES;

	private readonly int ASCON_AEAD_RATE;

	private readonly int nr;

	private byte[] mac;

	private ulong K0;

	private ulong K1;

	private ulong K2;

	private ulong N0;

	private ulong N1;

	private readonly ulong ASCON_IV;

	private ulong x0;

	private ulong x1;

	private ulong x2;

	private ulong x3;

	private ulong x4;

	private string algorithmName;

	private State m_state;

	private byte[] initialAssociatedText;

	private readonly int m_bufferSizeDecrypt;

	private readonly byte[] m_buf;

	private int m_bufPos;

	public string AlgorithmName => algorithmName;

	public AsconEngine(AsconParameters asconParameters)
	{
		this.asconParameters = asconParameters;
		switch (asconParameters)
		{
		case AsconParameters.ascon80pq:
			CRYPTO_KEYBYTES = 20;
			CRYPTO_ABYTES = 16;
			ASCON_AEAD_RATE = 8;
			ASCON_IV = 11547242664487288832uL;
			algorithmName = "Ascon-80pq AEAD";
			break;
		case AsconParameters.ascon128a:
			CRYPTO_KEYBYTES = 16;
			CRYPTO_ABYTES = 16;
			ASCON_AEAD_RATE = 16;
			ASCON_IV = 9259414062373011456uL;
			algorithmName = "Ascon-128a AEAD";
			break;
		case AsconParameters.ascon128:
			CRYPTO_KEYBYTES = 16;
			CRYPTO_ABYTES = 16;
			ASCON_AEAD_RATE = 8;
			ASCON_IV = 9241399655273594880uL;
			algorithmName = "Ascon-128 AEAD";
			break;
		default:
			throw new ArgumentException("invalid parameter setting for ASCON AEAD");
		}
		nr = ((ASCON_AEAD_RATE == 8) ? 6 : 8);
		m_bufferSizeDecrypt = ASCON_AEAD_RATE + CRYPTO_ABYTES;
		m_buf = new byte[m_bufferSizeDecrypt];
	}

	public int GetKeyBytesSize()
	{
		return CRYPTO_KEYBYTES;
	}

	public int GetIVBytesSize()
	{
		return CRYPTO_ABYTES;
	}

	public void Init(bool forEncryption, ICipherParameters parameters)
	{
		KeyParameter keyParameter;
		byte[] array;
		if (parameters is AeadParameters aeadParameters)
		{
			keyParameter = aeadParameters.Key;
			array = aeadParameters.GetNonce();
			initialAssociatedText = aeadParameters.GetAssociatedText();
			int macSize = aeadParameters.MacSize;
			if (macSize != CRYPTO_ABYTES * 8)
			{
				throw new ArgumentException("Invalid value for MAC size: " + macSize);
			}
		}
		else
		{
			if (!(parameters is ParametersWithIV parametersWithIV))
			{
				throw new ArgumentException("invalid parameters passed to Ascon");
			}
			keyParameter = parametersWithIV.Parameters as KeyParameter;
			array = parametersWithIV.GetIV();
			initialAssociatedText = null;
		}
		if (keyParameter == null)
		{
			throw new ArgumentException("Ascon Init parameters must include a key");
		}
		if (array.Length != CRYPTO_ABYTES)
		{
			string text = asconParameters.ToString();
			int cRYPTO_ABYTES = CRYPTO_ABYTES;
			throw new ArgumentException(text + " requires exactly " + cRYPTO_ABYTES + " bytes of IV");
		}
		byte[] key = keyParameter.GetKey();
		if (key.Length != CRYPTO_KEYBYTES)
		{
			string text2 = asconParameters.ToString();
			int cRYPTO_ABYTES = CRYPTO_KEYBYTES;
			throw new ArgumentException(text2 + " key must be " + cRYPTO_ABYTES + " bytes long");
		}
		N0 = Pack.BE_To_UInt64(array, 0);
		N1 = Pack.BE_To_UInt64(array, 8);
		if (CRYPTO_KEYBYTES == 16)
		{
			K1 = Pack.BE_To_UInt64(key, 0);
			K2 = Pack.BE_To_UInt64(key, 8);
		}
		else
		{
			if (CRYPTO_KEYBYTES != 20)
			{
				throw new InvalidOperationException();
			}
			K0 = Pack.BE_To_UInt32(key, 0);
			K1 = Pack.BE_To_UInt64(key, 4);
			K2 = Pack.BE_To_UInt64(key, 12);
		}
		m_state = (forEncryption ? State.EncInit : State.DecInit);
		Reset(clearMac: true);
	}

	public void ProcessAadByte(byte input)
	{
		CheckAad();
		m_buf[m_bufPos] = input;
		if (++m_bufPos == ASCON_AEAD_RATE)
		{
			ProcessBufferAad(m_buf, 0);
			m_bufPos = 0;
		}
	}

	public void ProcessAadBytes(byte[] inBytes, int inOff, int len)
	{
		Check.DataLength(inBytes, inOff, len, "input buffer too short");
		if (len <= 0)
		{
			return;
		}
		CheckAad();
		if (m_bufPos > 0)
		{
			int num = ASCON_AEAD_RATE - m_bufPos;
			if (len < num)
			{
				Array.Copy(inBytes, inOff, m_buf, m_bufPos, len);
				m_bufPos += len;
				return;
			}
			Array.Copy(inBytes, inOff, m_buf, m_bufPos, num);
			inOff += num;
			len -= num;
			ProcessBufferAad(m_buf, 0);
		}
		while (len >= ASCON_AEAD_RATE)
		{
			ProcessBufferAad(inBytes, inOff);
			inOff += ASCON_AEAD_RATE;
			len -= ASCON_AEAD_RATE;
		}
		Array.Copy(inBytes, inOff, m_buf, 0, len);
		m_bufPos = len;
	}

	public int ProcessByte(byte input, byte[] outBytes, int outOff)
	{
		return ProcessBytes(new byte[1] { input }, 0, 1, outBytes, outOff);
	}

	public int ProcessBytes(byte[] inBytes, int inOff, int len, byte[] outBytes, int outOff)
	{
		Check.DataLength(inBytes, inOff, len, "input buffer too short");
		bool num = CheckData();
		int num2 = 0;
		if (num)
		{
			if (m_bufPos > 0)
			{
				int num3 = ASCON_AEAD_RATE - m_bufPos;
				if (len < num3)
				{
					Array.Copy(inBytes, inOff, m_buf, m_bufPos, len);
					m_bufPos += len;
					return 0;
				}
				Array.Copy(inBytes, inOff, m_buf, m_bufPos, num3);
				inOff += num3;
				len -= num3;
				ProcessBufferEncrypt(m_buf, 0, outBytes, outOff);
				num2 = ASCON_AEAD_RATE;
			}
			while (len >= ASCON_AEAD_RATE)
			{
				ProcessBufferEncrypt(inBytes, inOff, outBytes, outOff + num2);
				inOff += ASCON_AEAD_RATE;
				len -= ASCON_AEAD_RATE;
				num2 += ASCON_AEAD_RATE;
			}
		}
		else
		{
			int num4 = m_bufferSizeDecrypt - m_bufPos;
			if (len < num4)
			{
				Array.Copy(inBytes, inOff, m_buf, m_bufPos, len);
				m_bufPos += len;
				return 0;
			}
			while (m_bufPos >= ASCON_AEAD_RATE)
			{
				ProcessBufferDecrypt(m_buf, 0, outBytes, outOff + num2);
				m_bufPos -= ASCON_AEAD_RATE;
				Array.Copy(m_buf, ASCON_AEAD_RATE, m_buf, 0, m_bufPos);
				num2 += ASCON_AEAD_RATE;
				num4 += ASCON_AEAD_RATE;
				if (len < num4)
				{
					Array.Copy(inBytes, inOff, m_buf, m_bufPos, len);
					m_bufPos += len;
					return num2;
				}
			}
			num4 = ASCON_AEAD_RATE - m_bufPos;
			Array.Copy(inBytes, inOff, m_buf, m_bufPos, num4);
			inOff += num4;
			len -= num4;
			ProcessBufferDecrypt(m_buf, 0, outBytes, outOff + num2);
			num2 += ASCON_AEAD_RATE;
			while (len >= m_bufferSizeDecrypt)
			{
				ProcessBufferDecrypt(inBytes, inOff, outBytes, outOff + num2);
				inOff += ASCON_AEAD_RATE;
				len -= ASCON_AEAD_RATE;
				num2 += ASCON_AEAD_RATE;
			}
		}
		Array.Copy(inBytes, inOff, m_buf, 0, len);
		m_bufPos = len;
		return num2;
	}

	public int DoFinal(byte[] outBytes, int outOff)
	{
		int num;
		if (CheckData())
		{
			num = m_bufPos + CRYPTO_ABYTES;
			Check.OutputLength(outBytes, outOff, num, "output buffer too short");
			ProcessFinalEncrypt(m_buf, 0, m_bufPos, outBytes, outOff);
			mac = new byte[CRYPTO_ABYTES];
			Pack.UInt64_To_BE(x3, mac, 0);
			Pack.UInt64_To_BE(x4, mac, 8);
			Array.Copy(mac, 0, outBytes, outOff + m_bufPos, CRYPTO_ABYTES);
			Reset(clearMac: false);
		}
		else
		{
			if (m_bufPos < CRYPTO_ABYTES)
			{
				throw new InvalidCipherTextException("data too short");
			}
			m_bufPos -= CRYPTO_ABYTES;
			num = m_bufPos;
			Check.OutputLength(outBytes, outOff, num, "output buffer too short");
			ProcessFinalDecrypt(m_buf, 0, m_bufPos, outBytes, outOff);
			x3 ^= Pack.BE_To_UInt64(m_buf, m_bufPos);
			x4 ^= Pack.BE_To_UInt64(m_buf, m_bufPos + 8);
			if ((x3 | x4) != 0L)
			{
				throw new InvalidCipherTextException("mac check in " + AlgorithmName + " failed");
			}
			Reset(clearMac: true);
		}
		return num;
	}

	public byte[] GetMac()
	{
		return mac;
	}

	public int GetUpdateOutputSize(int len)
	{
		int num = System.Math.Max(0, len);
		switch (m_state)
		{
		case State.DecInit:
		case State.DecAad:
			num = System.Math.Max(0, num - CRYPTO_ABYTES);
			break;
		case State.DecData:
		case State.DecFinal:
			num = System.Math.Max(0, num + m_bufPos - CRYPTO_ABYTES);
			break;
		case State.EncData:
		case State.EncFinal:
			num += m_bufPos;
			break;
		}
		return num - num % ASCON_AEAD_RATE;
	}

	public int GetOutputSize(int len)
	{
		int num = System.Math.Max(0, len);
		switch (m_state)
		{
		case State.DecInit:
		case State.DecAad:
			return System.Math.Max(0, num - CRYPTO_ABYTES);
		case State.DecData:
		case State.DecFinal:
			return System.Math.Max(0, num + m_bufPos - CRYPTO_ABYTES);
		case State.EncData:
		case State.EncFinal:
			return num + m_bufPos + CRYPTO_ABYTES;
		default:
			return num + CRYPTO_ABYTES;
		}
	}

	public void Reset()
	{
		Reset(clearMac: true);
	}

	private void CheckAad()
	{
		switch (m_state)
		{
		case State.DecInit:
			m_state = State.DecAad;
			break;
		case State.EncInit:
			m_state = State.EncAad;
			break;
		case State.EncFinal:
			throw new InvalidOperationException(AlgorithmName + " cannot be reused for encryption");
		default:
			throw new InvalidOperationException(AlgorithmName + " needs to be initialized");
		case State.EncAad:
		case State.DecAad:
			break;
		}
	}

	private bool CheckData()
	{
		switch (m_state)
		{
		case State.DecInit:
		case State.DecAad:
			FinishAad(State.DecData);
			return false;
		case State.EncInit:
		case State.EncAad:
			FinishAad(State.EncData);
			return true;
		case State.DecData:
			return false;
		case State.EncData:
			return true;
		case State.EncFinal:
			throw new InvalidOperationException(AlgorithmName + " cannot be reused for encryption");
		default:
			throw new InvalidOperationException(AlgorithmName + " needs to be initialized");
		}
	}

	private void FinishAad(State nextState)
	{
		State state = m_state;
		if (state == State.EncAad || state == State.DecAad)
		{
			m_buf[m_bufPos] = 128;
			if (m_bufPos >= 8)
			{
				x0 ^= Pack.BE_To_UInt64(m_buf, 0);
				x1 ^= Pack.BE_To_UInt64(m_buf, 8) & (ulong)(-1L << 56 - (m_bufPos - 8 << 3));
			}
			else
			{
				x0 ^= Pack.BE_To_UInt64(m_buf, 0) & (ulong)(-1L << 56 - (m_bufPos << 3));
			}
			P(nr);
		}
		x4 ^= 1uL;
		m_bufPos = 0;
		m_state = nextState;
	}

	private void FinishData(State nextState)
	{
		switch (asconParameters)
		{
		case AsconParameters.ascon128:
			x1 ^= K1;
			x2 ^= K2;
			break;
		case AsconParameters.ascon128a:
			x2 ^= K1;
			x3 ^= K2;
			break;
		case AsconParameters.ascon80pq:
			x1 ^= (K0 << 32) | (K1 >> 32);
			x2 ^= (K1 << 32) | (K2 >> 32);
			x3 ^= K2 << 32;
			break;
		default:
			throw new InvalidOperationException();
		}
		P(12);
		x3 ^= K1;
		x4 ^= K2;
		m_state = nextState;
	}

	private void P(int nr)
	{
		if (nr >= 8)
		{
			if (nr == 12)
			{
				ROUND(240uL);
				ROUND(225uL);
				ROUND(210uL);
				ROUND(195uL);
			}
			ROUND(180uL);
			ROUND(165uL);
		}
		ROUND(150uL);
		ROUND(135uL);
		ROUND(120uL);
		ROUND(105uL);
		ROUND(90uL);
		ROUND(75uL);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ROUND(ulong c)
	{
		ulong num = x0 ^ x1 ^ x2 ^ x3 ^ c ^ (x1 & (x0 ^ x2 ^ x4 ^ c));
		ulong num2 = x0 ^ x2 ^ x3 ^ x4 ^ c ^ ((x1 ^ x2 ^ c) & (x1 ^ x3));
		ulong num3 = x1 ^ x2 ^ x4 ^ c ^ (x3 & x4);
		ulong num4 = x0 ^ x1 ^ x2 ^ c ^ (~x0 & (x3 ^ x4));
		ulong num5 = x1 ^ x3 ^ x4 ^ ((x0 ^ x4) & x1);
		x0 = num ^ Longs.RotateRight(num, 19) ^ Longs.RotateRight(num, 28);
		x1 = num2 ^ Longs.RotateRight(num2, 39) ^ Longs.RotateRight(num2, 61);
		x2 = ~(num3 ^ Longs.RotateRight(num3, 1) ^ Longs.RotateRight(num3, 6));
		x3 = num4 ^ Longs.RotateRight(num4, 10) ^ Longs.RotateRight(num4, 17);
		x4 = num5 ^ Longs.RotateRight(num5, 7) ^ Longs.RotateRight(num5, 41);
	}

	private void ascon_aeadinit()
	{
		x0 = ASCON_IV;
		if (CRYPTO_KEYBYTES == 20)
		{
			x0 ^= K0;
		}
		x1 = K1;
		x2 = K2;
		x3 = N0;
		x4 = N1;
		P(12);
		if (CRYPTO_KEYBYTES == 20)
		{
			x2 ^= K0;
		}
		x3 ^= K1;
		x4 ^= K2;
	}

	private void ProcessBufferAad(byte[] buffer, int bufOff)
	{
		x0 ^= Pack.BE_To_UInt64(buffer, bufOff);
		if (ASCON_AEAD_RATE == 16)
		{
			x1 ^= Pack.BE_To_UInt64(buffer, bufOff + 8);
		}
		P(nr);
	}

	private void ProcessBufferDecrypt(byte[] buffer, int bufOff, byte[] output, int outOff)
	{
		Check.OutputLength(output, outOff, ASCON_AEAD_RATE, "output buffer too short");
		ulong num = Pack.BE_To_UInt64(buffer, bufOff);
		Pack.UInt64_To_BE(x0 ^ num, output, outOff);
		x0 = num;
		if (ASCON_AEAD_RATE == 16)
		{
			ulong num2 = Pack.BE_To_UInt64(buffer, bufOff + 8);
			Pack.UInt64_To_BE(x1 ^ num2, output, outOff + 8);
			x1 = num2;
		}
		P(nr);
	}

	private void ProcessBufferEncrypt(byte[] buffer, int bufOff, byte[] output, int outOff)
	{
		Check.OutputLength(output, outOff, ASCON_AEAD_RATE, "output buffer too short");
		x0 ^= Pack.BE_To_UInt64(buffer, bufOff);
		Pack.UInt64_To_BE(x0, output, outOff);
		if (ASCON_AEAD_RATE == 16)
		{
			x1 ^= Pack.BE_To_UInt64(buffer, bufOff + 8);
			Pack.UInt64_To_BE(x1, output, outOff + 8);
		}
		P(nr);
	}

	private void ProcessFinalDecrypt(byte[] input, int inOff, int inLen, byte[] output, int outOff)
	{
		if (inLen >= 8)
		{
			ulong num = Pack.BE_To_UInt64(input, inOff);
			x0 ^= num;
			Pack.UInt64_To_BE(x0, output, outOff);
			x0 = num;
			inOff += 8;
			outOff += 8;
			inLen -= 8;
			x1 ^= PAD(inLen);
			if (inLen != 0)
			{
				ulong num2 = Pack.BE_To_UInt64_High(input, inOff, inLen);
				x1 ^= num2;
				Pack.UInt64_To_BE_High(x1, output, outOff, inLen);
				x1 &= ulong.MaxValue >> (inLen << 3);
				x1 ^= num2;
			}
		}
		else
		{
			x0 ^= PAD(inLen);
			if (inLen != 0)
			{
				ulong num3 = Pack.BE_To_UInt64_High(input, inOff, inLen);
				x0 ^= num3;
				Pack.UInt64_To_BE_High(x0, output, outOff, inLen);
				x0 &= ulong.MaxValue >> (inLen << 3);
				x0 ^= num3;
			}
		}
		FinishData(State.DecFinal);
	}

	private void ProcessFinalEncrypt(byte[] input, int inOff, int inLen, byte[] output, int outOff)
	{
		if (inLen >= 8)
		{
			x0 ^= Pack.BE_To_UInt64(input, inOff);
			Pack.UInt64_To_BE(x0, output, outOff);
			inOff += 8;
			outOff += 8;
			inLen -= 8;
			x1 ^= PAD(inLen);
			if (inLen != 0)
			{
				x1 ^= Pack.BE_To_UInt64_High(input, inOff, inLen);
				Pack.UInt64_To_BE_High(x1, output, outOff, inLen);
			}
		}
		else
		{
			x0 ^= PAD(inLen);
			if (inLen != 0)
			{
				x0 ^= Pack.BE_To_UInt64_High(input, inOff, inLen);
				Pack.UInt64_To_BE_High(x0, output, outOff, inLen);
			}
		}
		FinishData(State.EncFinal);
	}

	private void Reset(bool clearMac)
	{
		if (clearMac)
		{
			mac = null;
		}
		Arrays.Clear(m_buf);
		m_bufPos = 0;
		switch (m_state)
		{
		case State.DecAad:
		case State.DecData:
		case State.DecFinal:
			m_state = State.DecInit;
			break;
		case State.EncAad:
		case State.EncData:
		case State.EncFinal:
			m_state = State.EncFinal;
			return;
		default:
			throw new InvalidOperationException(AlgorithmName + " needs to be initialized");
		case State.EncInit:
		case State.DecInit:
			break;
		}
		ascon_aeadinit();
		if (initialAssociatedText != null)
		{
			ProcessAadBytes(initialAssociatedText, 0, initialAssociatedText.Length);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong PAD(int i)
	{
		return 9223372036854775808uL >> (i << 3);
	}
}
