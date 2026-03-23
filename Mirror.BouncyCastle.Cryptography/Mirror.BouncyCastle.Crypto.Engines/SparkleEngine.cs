using System;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public sealed class SparkleEngine : IAeadCipher
{
	public enum SparkleParameters
	{
		SCHWAEMM128_128,
		SCHWAEMM256_128,
		SCHWAEMM192_192,
		SCHWAEMM256_256
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

	private static readonly uint[] RCON = new uint[8] { 3084996962u, 3211876480u, 951376470u, 844003128u, 3138487787u, 1333558103u, 3485442504u, 3266521405u };

	private string algorithmName;

	private readonly uint[] state;

	private readonly uint[] k;

	private readonly uint[] npub;

	private byte[] tag;

	private bool encrypted;

	private State m_state;

	private byte[] initialAssociatedText;

	private readonly int m_bufferSizeDecrypt;

	private readonly byte[] m_buf;

	private int m_bufPos;

	private readonly int SCHWAEMM_KEY_LEN;

	private readonly int SCHWAEMM_NONCE_LEN;

	private readonly int SPARKLE_STEPS_SLIM;

	private readonly int SPARKLE_STEPS_BIG;

	private readonly int KEY_BYTES;

	private readonly int KEY_WORDS;

	private readonly int TAG_WORDS;

	private readonly int TAG_BYTES;

	private readonly int STATE_WORDS;

	private readonly int RATE_WORDS;

	private readonly int RATE_BYTES;

	private readonly int CAP_MASK;

	private readonly uint _A0;

	private readonly uint _A1;

	private readonly uint _M2;

	private readonly uint _M3;

	public string AlgorithmName => algorithmName;

	public SparkleEngine(SparkleParameters sparkleParameters)
	{
		int num;
		int num2;
		int num3;
		switch (sparkleParameters)
		{
		case SparkleParameters.SCHWAEMM128_128:
			SCHWAEMM_KEY_LEN = 128;
			SCHWAEMM_NONCE_LEN = 128;
			num = 128;
			num2 = 256;
			num3 = 128;
			SPARKLE_STEPS_SLIM = 7;
			SPARKLE_STEPS_BIG = 10;
			algorithmName = "SCHWAEMM128-128";
			break;
		case SparkleParameters.SCHWAEMM256_128:
			SCHWAEMM_KEY_LEN = 128;
			SCHWAEMM_NONCE_LEN = 256;
			num = 128;
			num2 = 384;
			num3 = 128;
			SPARKLE_STEPS_SLIM = 7;
			SPARKLE_STEPS_BIG = 11;
			algorithmName = "SCHWAEMM256-128";
			break;
		case SparkleParameters.SCHWAEMM192_192:
			SCHWAEMM_KEY_LEN = 192;
			SCHWAEMM_NONCE_LEN = 192;
			num = 192;
			num2 = 384;
			num3 = 192;
			SPARKLE_STEPS_SLIM = 7;
			SPARKLE_STEPS_BIG = 11;
			algorithmName = "SCHWAEMM192-192";
			break;
		case SparkleParameters.SCHWAEMM256_256:
			SCHWAEMM_KEY_LEN = 256;
			SCHWAEMM_NONCE_LEN = 256;
			num = 256;
			num2 = 512;
			num3 = 256;
			SPARKLE_STEPS_SLIM = 8;
			SPARKLE_STEPS_BIG = 12;
			algorithmName = "SCHWAEMM256-256";
			break;
		default:
			throw new ArgumentException("Invalid definition of SCHWAEMM instance");
		}
		KEY_WORDS = SCHWAEMM_KEY_LEN >> 5;
		KEY_BYTES = SCHWAEMM_KEY_LEN >> 3;
		TAG_WORDS = num >> 5;
		TAG_BYTES = num >> 3;
		STATE_WORDS = num2 >> 5;
		RATE_WORDS = SCHWAEMM_NONCE_LEN >> 5;
		RATE_BYTES = SCHWAEMM_NONCE_LEN >> 3;
		int num4 = num3 >> 6;
		int num5 = num3 >> 5;
		CAP_MASK = ((RATE_WORDS > num5) ? (num5 - 1) : (-1));
		_A0 = (uint)(1 << num4 << 24);
		_A1 = (uint)((1 ^ (1 << num4)) << 24);
		_M2 = (uint)((2 ^ (1 << num4)) << 24);
		_M3 = (uint)((3 ^ (1 << num4)) << 24);
		state = new uint[STATE_WORDS];
		k = new uint[KEY_WORDS];
		npub = new uint[RATE_WORDS];
		m_bufferSizeDecrypt = RATE_BYTES + TAG_BYTES;
		m_buf = new byte[m_bufferSizeDecrypt];
	}

	public int GetKeyBytesSize()
	{
		return KEY_BYTES;
	}

	public int GetIVBytesSize()
	{
		return RATE_BYTES;
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
			if (macSize != TAG_BYTES * 8)
			{
				throw new ArgumentException("Invalid value for MAC size: " + macSize);
			}
		}
		else
		{
			if (!(parameters is ParametersWithIV parametersWithIV))
			{
				throw new ArgumentException("invalid parameters passed to Sparkle");
			}
			keyParameter = parametersWithIV.Parameters as KeyParameter;
			array = parametersWithIV.GetIV();
			initialAssociatedText = null;
		}
		if (keyParameter == null)
		{
			throw new ArgumentException("Sparkle Init parameters must include a key");
		}
		int num = KEY_WORDS * 4;
		if (num != keyParameter.KeyLength)
		{
			throw new ArgumentException(algorithmName + " requires exactly " + num + " bytes of key");
		}
		int num2 = RATE_WORDS * 4;
		if (num2 != array.Length)
		{
			throw new ArgumentException(algorithmName + " requires exactly " + num2 + " bytes of IV");
		}
		Pack.LE_To_UInt32(keyParameter.GetKey(), 0, k);
		Pack.LE_To_UInt32(array, 0, npub);
		m_state = (forEncryption ? State.EncInit : State.DecInit);
		Reset();
	}

	public void ProcessAadByte(byte input)
	{
		CheckAad();
		if (m_bufPos == RATE_BYTES)
		{
			ProcessBufferAad(m_buf, 0);
			m_bufPos = 0;
		}
		m_buf[m_bufPos++] = input;
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
			int num = RATE_BYTES - m_bufPos;
			if (len <= num)
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
		while (len > RATE_BYTES)
		{
			ProcessBufferAad(inBytes, inOff);
			inOff += RATE_BYTES;
			len -= RATE_BYTES;
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
				int num3 = RATE_BYTES - m_bufPos;
				if (len <= num3)
				{
					Array.Copy(inBytes, inOff, m_buf, m_bufPos, len);
					m_bufPos += len;
					return 0;
				}
				Array.Copy(inBytes, inOff, m_buf, m_bufPos, num3);
				inOff += num3;
				len -= num3;
				ProcessBufferEncrypt(m_buf, 0, outBytes, outOff);
				num2 = RATE_BYTES;
			}
			while (len > RATE_BYTES)
			{
				ProcessBufferEncrypt(inBytes, inOff, outBytes, outOff + num2);
				inOff += RATE_BYTES;
				len -= RATE_BYTES;
				num2 += RATE_BYTES;
			}
		}
		else
		{
			int num4 = m_bufferSizeDecrypt - m_bufPos;
			if (len <= num4)
			{
				Array.Copy(inBytes, inOff, m_buf, m_bufPos, len);
				m_bufPos += len;
				return 0;
			}
			if (m_bufPos > RATE_BYTES)
			{
				ProcessBufferDecrypt(m_buf, 0, outBytes, outOff);
				m_bufPos -= RATE_BYTES;
				Array.Copy(m_buf, RATE_BYTES, m_buf, 0, m_bufPos);
				num2 = RATE_BYTES;
				num4 += RATE_BYTES;
				if (len <= num4)
				{
					Array.Copy(inBytes, inOff, m_buf, m_bufPos, len);
					m_bufPos += len;
					return num2;
				}
			}
			num4 = RATE_BYTES - m_bufPos;
			Array.Copy(inBytes, inOff, m_buf, m_bufPos, num4);
			inOff += num4;
			len -= num4;
			ProcessBufferDecrypt(m_buf, 0, outBytes, outOff + num2);
			num2 += RATE_BYTES;
			while (len > m_bufferSizeDecrypt)
			{
				ProcessBufferDecrypt(inBytes, inOff, outBytes, outOff + num2);
				inOff += RATE_BYTES;
				len -= RATE_BYTES;
				num2 += RATE_BYTES;
			}
		}
		Array.Copy(inBytes, inOff, m_buf, 0, len);
		m_bufPos = len;
		return num2;
	}

	public int DoFinal(byte[] outBytes, int outOff)
	{
		bool flag = CheckData();
		int num;
		if (flag)
		{
			num = m_bufPos + TAG_BYTES;
		}
		else
		{
			if (m_bufPos < TAG_BYTES)
			{
				throw new InvalidCipherTextException("data too short");
			}
			m_bufPos -= TAG_BYTES;
			num = m_bufPos;
		}
		Check.OutputLength(outBytes, outOff, num, "output buffer too short");
		if (encrypted || m_bufPos > 0)
		{
			state[STATE_WORDS - 1] ^= ((m_bufPos < RATE_BYTES) ? _M2 : _M3);
			uint[] array = new uint[RATE_WORDS];
			for (int i = 0; i < m_bufPos; i++)
			{
				array[i >> 2] |= (uint)(m_buf[i] << ((i & 3) << 3));
			}
			if (m_bufPos < RATE_BYTES)
			{
				if (!flag)
				{
					int num2 = (m_bufPos & 3) << 3;
					array[m_bufPos >> 2] |= state[m_bufPos >> 2] >> num2 << num2;
					num2 = (m_bufPos >> 2) + 1;
					Array.Copy(state, num2, array, num2, RATE_WORDS - num2);
				}
				array[m_bufPos >> 2] ^= (uint)(128 << ((m_bufPos & 3) << 3));
			}
			for (int j = 0; j < RATE_WORDS / 2; j++)
			{
				int num3 = j + RATE_WORDS / 2;
				uint num4 = state[j];
				uint num5 = state[num3];
				if (flag)
				{
					state[j] = num5 ^ array[j] ^ state[RATE_WORDS + j];
					state[num3] = num4 ^ num5 ^ array[num3] ^ state[RATE_WORDS + (num3 & CAP_MASK)];
				}
				else
				{
					state[j] = num4 ^ num5 ^ array[j] ^ state[RATE_WORDS + j];
					state[num3] = num4 ^ array[num3] ^ state[RATE_WORDS + (num3 & CAP_MASK)];
				}
				array[j] ^= num4;
				array[num3] ^= num5;
			}
			for (int k = 0; k < m_bufPos; k++)
			{
				outBytes[outOff++] = (byte)(array[k >> 2] >> ((k & 3) << 3));
			}
			SparkleOpt(state, SPARKLE_STEPS_BIG);
		}
		for (int l = 0; l < KEY_WORDS; l++)
		{
			state[RATE_WORDS + l] ^= this.k[l];
		}
		tag = new byte[TAG_BYTES];
		Pack.UInt32_To_LE(state, RATE_WORDS, TAG_WORDS, tag, 0);
		if (flag)
		{
			Array.Copy(tag, 0, outBytes, outOff, TAG_BYTES);
		}
		else if (!Arrays.FixedTimeEquals(TAG_BYTES, tag, 0, m_buf, m_bufPos))
		{
			throw new InvalidCipherTextException("mac check in " + AlgorithmName + " failed");
		}
		Reset(!flag);
		return num;
	}

	public byte[] GetMac()
	{
		return tag;
	}

	public int GetUpdateOutputSize(int len)
	{
		int num = System.Math.Max(0, len) - 1;
		switch (m_state)
		{
		case State.DecInit:
		case State.DecAad:
			num = System.Math.Max(0, num - TAG_BYTES);
			break;
		case State.DecData:
		case State.DecFinal:
			num = System.Math.Max(0, num + m_bufPos - TAG_BYTES);
			break;
		case State.EncData:
		case State.EncFinal:
			num = System.Math.Max(0, num + m_bufPos);
			break;
		}
		return num - num % RATE_BYTES;
	}

	public int GetOutputSize(int len)
	{
		int num = System.Math.Max(0, len);
		switch (m_state)
		{
		case State.DecInit:
		case State.DecAad:
			return System.Math.Max(0, num - TAG_BYTES);
		case State.DecData:
		case State.DecFinal:
			return System.Math.Max(0, num + m_bufPos - TAG_BYTES);
		case State.EncData:
		case State.EncFinal:
			return num + m_bufPos + TAG_BYTES;
		default:
			return num + TAG_BYTES;
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
			ProcessFinalAad();
		}
		m_bufPos = 0;
		m_state = nextState;
	}

	private void ProcessBufferAad(byte[] buffer, int bufOff)
	{
		for (int i = 0; i < RATE_WORDS / 2; i++)
		{
			int num = i + RATE_WORDS / 2;
			uint num2 = state[i];
			uint num3 = state[num];
			uint num4 = Pack.LE_To_UInt32(buffer, bufOff + i * 4);
			uint num5 = Pack.LE_To_UInt32(buffer, bufOff + num * 4);
			state[i] = num3 ^ num4 ^ state[RATE_WORDS + i];
			state[num] = num2 ^ num3 ^ num5 ^ state[RATE_WORDS + (num & CAP_MASK)];
		}
		SparkleOpt(state, SPARKLE_STEPS_SLIM);
	}

	private void ProcessBufferDecrypt(byte[] buffer, int bufOff, byte[] output, int outOff)
	{
		Check.OutputLength(output, outOff, RATE_BYTES, "output buffer too short");
		for (int i = 0; i < RATE_WORDS / 2; i++)
		{
			int num = i + RATE_WORDS / 2;
			uint num2 = state[i];
			uint num3 = state[num];
			uint num4 = Pack.LE_To_UInt32(buffer, bufOff + i * 4);
			uint num5 = Pack.LE_To_UInt32(buffer, bufOff + num * 4);
			state[i] = num2 ^ num3 ^ num4 ^ state[RATE_WORDS + i];
			state[num] = num2 ^ num5 ^ state[RATE_WORDS + (num & CAP_MASK)];
			Pack.UInt32_To_LE(num4 ^ num2, output, outOff + i * 4);
			Pack.UInt32_To_LE(num5 ^ num3, output, outOff + num * 4);
		}
		SparkleOpt(state, SPARKLE_STEPS_SLIM);
		encrypted = true;
	}

	private void ProcessBufferEncrypt(byte[] buffer, int bufOff, byte[] output, int outOff)
	{
		Check.OutputLength(output, outOff, RATE_BYTES, "output buffer too short");
		for (int i = 0; i < RATE_WORDS / 2; i++)
		{
			int num = i + RATE_WORDS / 2;
			uint num2 = state[i];
			uint num3 = state[num];
			uint num4 = Pack.LE_To_UInt32(buffer, bufOff + i * 4);
			uint num5 = Pack.LE_To_UInt32(buffer, bufOff + num * 4);
			state[i] = num3 ^ num4 ^ state[RATE_WORDS + i];
			state[num] = num2 ^ num3 ^ num5 ^ state[RATE_WORDS + (num & CAP_MASK)];
			Pack.UInt32_To_LE(num4 ^ num2, output, outOff + i * 4);
			Pack.UInt32_To_LE(num5 ^ num3, output, outOff + num * 4);
		}
		SparkleOpt(state, SPARKLE_STEPS_SLIM);
		encrypted = true;
	}

	private void ProcessFinalAad()
	{
		if (m_bufPos < RATE_BYTES)
		{
			state[STATE_WORDS - 1] ^= _A0;
			m_buf[m_bufPos] = 128;
			while (++m_bufPos < RATE_BYTES)
			{
				m_buf[m_bufPos] = 0;
			}
		}
		else
		{
			state[STATE_WORDS - 1] ^= _A1;
		}
		for (int i = 0; i < RATE_WORDS / 2; i++)
		{
			int num = i + RATE_WORDS / 2;
			uint num2 = state[i];
			uint num3 = state[num];
			uint num4 = Pack.LE_To_UInt32(m_buf, i * 4);
			uint num5 = Pack.LE_To_UInt32(m_buf, num * 4);
			state[i] = num3 ^ num4 ^ state[RATE_WORDS + i];
			state[num] = num2 ^ num3 ^ num5 ^ state[RATE_WORDS + (num & CAP_MASK)];
		}
		SparkleOpt(state, SPARKLE_STEPS_BIG);
	}

	private void Reset(bool clearMac)
	{
		if (clearMac)
		{
			tag = null;
		}
		Arrays.Clear(m_buf);
		m_bufPos = 0;
		encrypted = false;
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
		Array.Copy(npub, 0, state, 0, RATE_WORDS);
		Array.Copy(k, 0, state, RATE_WORDS, KEY_WORDS);
		SparkleOpt(state, SPARKLE_STEPS_BIG);
		if (initialAssociatedText != null)
		{
			ProcessAadBytes(initialAssociatedText, 0, initialAssociatedText.Length);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ArxBox(uint rc, ref uint s00, ref uint s01)
	{
		s00 += Integers.RotateRight(s01, 31);
		s01 ^= Integers.RotateRight(s00, 24);
		s00 ^= rc;
		s00 += Integers.RotateRight(s01, 17);
		s01 ^= Integers.RotateRight(s00, 17);
		s00 ^= rc;
		s00 += s01;
		s01 ^= Integers.RotateRight(s00, 31);
		s00 ^= rc;
		s00 += Integers.RotateRight(s01, 24);
		s01 ^= Integers.RotateRight(s00, 16);
		s00 ^= rc;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ELL(uint x)
	{
		return Integers.RotateRight(x, 16) ^ (x & 0xFFFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void SparkleOpt(uint[] state, int steps)
	{
		switch (state.Length)
		{
		case 8:
			SparkleOpt8(state, steps);
			break;
		case 12:
			SparkleOpt12(state, steps);
			break;
		case 16:
			SparkleOpt16(state, steps);
			break;
		default:
			throw new InvalidOperationException();
		}
	}

	internal static void SparkleOpt8(uint[] state, int steps)
	{
		uint s = state[0];
		uint num = state[1];
		uint s2 = state[2];
		uint num2 = state[3];
		uint s3 = state[4];
		uint s4 = state[5];
		uint s5 = state[6];
		uint s6 = state[7];
		for (int i = 0; i < steps; i++)
		{
			num ^= RCON[i & 7];
			num2 ^= (uint)i;
			ArxBox(RCON[0], ref s, ref num);
			ArxBox(RCON[1], ref s2, ref num2);
			ArxBox(RCON[2], ref s3, ref s4);
			ArxBox(RCON[3], ref s5, ref s6);
			uint num3 = ELL(s ^ s2);
			uint num4 = ELL(num ^ num2);
			uint num5 = s ^ s3;
			uint num6 = num ^ s4;
			uint num7 = s2 ^ s5;
			uint num8 = num2 ^ s6;
			s3 = s;
			s4 = num;
			s5 = s2;
			s6 = num2;
			s = num7 ^ num4;
			num = num8 ^ num3;
			s2 = num5 ^ num4;
			num2 = num6 ^ num3;
		}
		state[0] = s;
		state[1] = num;
		state[2] = s2;
		state[3] = num2;
		state[4] = s3;
		state[5] = s4;
		state[6] = s5;
		state[7] = s6;
	}

	internal static void SparkleOpt12(uint[] state, int steps)
	{
		uint s = state[0];
		uint num = state[1];
		uint s2 = state[2];
		uint num2 = state[3];
		uint s3 = state[4];
		uint s4 = state[5];
		uint s5 = state[6];
		uint s6 = state[7];
		uint s7 = state[8];
		uint s8 = state[9];
		uint s9 = state[10];
		uint s10 = state[11];
		for (int i = 0; i < steps; i++)
		{
			num ^= RCON[i & 7];
			num2 ^= (uint)i;
			ArxBox(RCON[0], ref s, ref num);
			ArxBox(RCON[1], ref s2, ref num2);
			ArxBox(RCON[2], ref s3, ref s4);
			ArxBox(RCON[3], ref s5, ref s6);
			ArxBox(RCON[4], ref s7, ref s8);
			ArxBox(RCON[5], ref s9, ref s10);
			uint num3 = ELL(s ^ s2 ^ s3);
			uint num4 = ELL(num ^ num2 ^ s4);
			uint num5 = s ^ s5;
			uint num6 = num ^ s6;
			uint num7 = s2 ^ s7;
			uint num8 = num2 ^ s8;
			uint num9 = s3 ^ s9;
			uint num10 = s4 ^ s10;
			s5 = s;
			s6 = num;
			s7 = s2;
			s8 = num2;
			s9 = s3;
			s10 = s4;
			s = num7 ^ num4;
			num = num8 ^ num3;
			s2 = num9 ^ num4;
			num2 = num10 ^ num3;
			s3 = num5 ^ num4;
			s4 = num6 ^ num3;
		}
		state[0] = s;
		state[1] = num;
		state[2] = s2;
		state[3] = num2;
		state[4] = s3;
		state[5] = s4;
		state[6] = s5;
		state[7] = s6;
		state[8] = s7;
		state[9] = s8;
		state[10] = s9;
		state[11] = s10;
	}

	internal static void SparkleOpt16(uint[] state, int steps)
	{
		uint s = state[0];
		uint num = state[1];
		uint s2 = state[2];
		uint num2 = state[3];
		uint s3 = state[4];
		uint s4 = state[5];
		uint s5 = state[6];
		uint s6 = state[7];
		uint s7 = state[8];
		uint s8 = state[9];
		uint s9 = state[10];
		uint s10 = state[11];
		uint s11 = state[12];
		uint s12 = state[13];
		uint s13 = state[14];
		uint s14 = state[15];
		int num3 = 0;
		while (num3 < steps)
		{
			num ^= RCON[num3 & 7];
			num2 ^= (uint)num3++;
			ArxBox(RCON[0], ref s, ref num);
			ArxBox(RCON[1], ref s2, ref num2);
			ArxBox(RCON[2], ref s3, ref s4);
			ArxBox(RCON[3], ref s5, ref s6);
			ArxBox(RCON[4], ref s7, ref s8);
			ArxBox(RCON[5], ref s9, ref s10);
			ArxBox(RCON[6], ref s11, ref s12);
			ArxBox(RCON[7], ref s13, ref s14);
			uint num4 = ELL(s ^ s2 ^ s3 ^ s5);
			uint num5 = ELL(num ^ num2 ^ s4 ^ s6);
			uint num6 = s7;
			uint num7 = s8;
			s7 = s2 ^ s9 ^ num5;
			s8 = num2 ^ s10 ^ num4;
			s9 = s3 ^ s11 ^ num5;
			s10 = s4 ^ s12 ^ num4;
			s11 = s5 ^ s13 ^ num5;
			s12 = s6 ^ s14 ^ num4;
			s13 = s ^ num6 ^ num5;
			s14 = num ^ num7 ^ num4;
			s8 ^= RCON[num3 & 7];
			s10 ^= (uint)num3++;
			ArxBox(RCON[0], ref s7, ref s8);
			ArxBox(RCON[1], ref s9, ref s10);
			ArxBox(RCON[2], ref s11, ref s12);
			ArxBox(RCON[3], ref s13, ref s14);
			ArxBox(RCON[4], ref s, ref num);
			ArxBox(RCON[5], ref s2, ref num2);
			ArxBox(RCON[6], ref s3, ref s4);
			ArxBox(RCON[7], ref s5, ref s6);
			uint num8 = ELL(s7 ^ s9 ^ s11 ^ s13);
			uint num9 = ELL(s8 ^ s10 ^ s12 ^ s14);
			uint num10 = s;
			uint num11 = num;
			s = s2 ^ s9 ^ num9;
			num = num2 ^ s10 ^ num8;
			s2 = s3 ^ s11 ^ num9;
			num2 = s4 ^ s12 ^ num8;
			s3 = s5 ^ s13 ^ num9;
			s4 = s6 ^ s14 ^ num8;
			s5 = num10 ^ s7 ^ num9;
			s6 = num11 ^ s8 ^ num8;
		}
		state[0] = s;
		state[1] = num;
		state[2] = s2;
		state[3] = num2;
		state[4] = s3;
		state[5] = s4;
		state[6] = s5;
		state[7] = s6;
		state[8] = s7;
		state[9] = s8;
		state[10] = s9;
		state[11] = s10;
		state[12] = s11;
		state[13] = s12;
		state[14] = s13;
		state[15] = s14;
	}
}
