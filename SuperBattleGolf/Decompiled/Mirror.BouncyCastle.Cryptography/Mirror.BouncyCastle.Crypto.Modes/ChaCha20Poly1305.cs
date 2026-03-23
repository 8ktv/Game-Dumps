using System;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Macs;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Modes;

public class ChaCha20Poly1305 : IAeadCipher
{
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

	private const int BufSize = 64;

	private const int KeySize = 32;

	private const int NonceSize = 12;

	private const int MacSize = 16;

	private static readonly byte[] Zeroes = new byte[15];

	private const ulong AadLimit = ulong.MaxValue;

	private const ulong DataLimit = 274877906880uL;

	private readonly ChaCha7539Engine mChacha20;

	private readonly IMac mPoly1305;

	private readonly byte[] mKey = new byte[32];

	private readonly byte[] mNonce = new byte[12];

	private readonly byte[] mBuf = new byte[80];

	private readonly byte[] mMac = new byte[16];

	private byte[] mInitialAad;

	private ulong mAadCount;

	private ulong mDataCount;

	private State mState;

	private int mBufPos;

	public virtual string AlgorithmName => "ChaCha20Poly1305";

	public ChaCha20Poly1305()
		: this(new Poly1305())
	{
	}

	public ChaCha20Poly1305(IMac poly1305)
	{
		if (poly1305 == null)
		{
			throw new ArgumentNullException("poly1305");
		}
		if (16 != poly1305.GetMacSize())
		{
			throw new ArgumentException("must be a 128-bit MAC", "poly1305");
		}
		mChacha20 = new ChaCha7539Engine();
		mPoly1305 = poly1305;
	}

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		KeyParameter keyParameter;
		byte[] array;
		ICipherParameters parameters2;
		if (parameters is AeadParameters { MacSize: var macSize } aeadParameters)
		{
			if (128 != macSize)
			{
				throw new ArgumentException("Invalid value for MAC size: " + macSize);
			}
			keyParameter = aeadParameters.Key;
			array = aeadParameters.GetNonce();
			parameters2 = new ParametersWithIV(keyParameter, array);
			mInitialAad = aeadParameters.GetAssociatedText();
		}
		else
		{
			if (!(parameters is ParametersWithIV parametersWithIV))
			{
				throw new ArgumentException("invalid parameters passed to ChaCha20Poly1305", "parameters");
			}
			keyParameter = (KeyParameter)parametersWithIV.Parameters;
			array = parametersWithIV.GetIV();
			parameters2 = parametersWithIV;
			mInitialAad = null;
		}
		if (keyParameter == null)
		{
			if (mState == State.Uninitialized)
			{
				throw new ArgumentException("Key must be specified in initial init");
			}
		}
		else if (32 != keyParameter.KeyLength)
		{
			throw new ArgumentException("Key must be 256 bits");
		}
		if (12 != array.Length)
		{
			throw new ArgumentException("Nonce must be 96 bits");
		}
		if (mState != State.Uninitialized && forEncryption && Arrays.AreEqual(mNonce, array) && (keyParameter == null || keyParameter.FixedTimeEquals(mKey)))
		{
			throw new ArgumentException("cannot reuse nonce for ChaCha20Poly1305 encryption");
		}
		keyParameter?.CopyTo(mKey, 0, 32);
		Array.Copy(array, 0, mNonce, 0, 12);
		mChacha20.Init(forEncryption: true, parameters2);
		mState = (forEncryption ? State.EncInit : State.DecInit);
		Reset(clearMac: true, resetCipher: false);
	}

	public virtual int GetOutputSize(int len)
	{
		int num = System.Math.Max(0, len);
		switch (mState)
		{
		case State.DecInit:
		case State.DecAad:
			return System.Math.Max(0, num - 16);
		case State.DecData:
		case State.DecFinal:
			return System.Math.Max(0, num + mBufPos - 16);
		case State.EncData:
		case State.EncFinal:
			return num + mBufPos + 16;
		default:
			return num + 16;
		}
	}

	public virtual int GetUpdateOutputSize(int len)
	{
		int num = System.Math.Max(0, len);
		switch (mState)
		{
		case State.DecInit:
		case State.DecAad:
			num = System.Math.Max(0, num - 16);
			break;
		case State.DecData:
		case State.DecFinal:
			num = System.Math.Max(0, num + mBufPos - 16);
			break;
		case State.EncData:
		case State.EncFinal:
			num += mBufPos;
			break;
		}
		return num - num % 64;
	}

	public virtual void ProcessAadByte(byte input)
	{
		CheckAad();
		mAadCount = IncrementCount(mAadCount, 1u, ulong.MaxValue);
		mPoly1305.Update(input);
	}

	public virtual void ProcessAadBytes(byte[] inBytes, int inOff, int len)
	{
		if (inBytes == null)
		{
			throw new ArgumentNullException("inBytes");
		}
		if (inOff < 0)
		{
			throw new ArgumentException("cannot be negative", "inOff");
		}
		if (len < 0)
		{
			throw new ArgumentException("cannot be negative", "len");
		}
		Check.DataLength(inBytes, inOff, len, "input buffer too short");
		CheckAad();
		if (len > 0)
		{
			mAadCount = IncrementCount(mAadCount, (uint)len, ulong.MaxValue);
			mPoly1305.BlockUpdate(inBytes, inOff, len);
		}
	}

	public virtual int ProcessByte(byte input, byte[] outBytes, int outOff)
	{
		CheckData();
		switch (mState)
		{
		case State.DecData:
			mBuf[mBufPos] = input;
			if (++mBufPos == mBuf.Length)
			{
				mPoly1305.BlockUpdate(mBuf, 0, 64);
				ProcessBlock(mBuf, 0, outBytes, outOff);
				Array.Copy(mBuf, 64, mBuf, 0, 16);
				mBufPos = 16;
				return 64;
			}
			return 0;
		case State.EncData:
			mBuf[mBufPos] = input;
			if (++mBufPos == 64)
			{
				ProcessBlock(mBuf, 0, outBytes, outOff);
				mPoly1305.BlockUpdate(outBytes, outOff, 64);
				mBufPos = 0;
				return 64;
			}
			return 0;
		default:
			throw new InvalidOperationException();
		}
	}

	public virtual int ProcessBytes(byte[] inBytes, int inOff, int len, byte[] outBytes, int outOff)
	{
		if (inBytes == null)
		{
			throw new ArgumentNullException("inBytes");
		}
		if (inOff < 0)
		{
			throw new ArgumentException("cannot be negative", "inOff");
		}
		if (len < 0)
		{
			throw new ArgumentException("cannot be negative", "len");
		}
		Check.DataLength(inBytes, inOff, len, "input buffer too short");
		if (outOff < 0)
		{
			throw new ArgumentException("cannot be negative", "outOff");
		}
		CheckData();
		int num = 0;
		switch (mState)
		{
		case State.DecData:
		{
			int num5 = mBuf.Length - mBufPos;
			if (len < num5)
			{
				Array.Copy(inBytes, inOff, mBuf, mBufPos, len);
				mBufPos += len;
				break;
			}
			if (mBufPos >= 64)
			{
				mPoly1305.BlockUpdate(mBuf, 0, 64);
				ProcessBlock(mBuf, 0, outBytes, outOff);
				Array.Copy(mBuf, 64, mBuf, 0, mBufPos -= 64);
				num = 64;
				num5 += 64;
				if (len < num5)
				{
					Array.Copy(inBytes, inOff, mBuf, mBufPos, len);
					mBufPos += len;
					break;
				}
			}
			int num6 = inOff + len - mBuf.Length;
			int num7 = num6 - 64;
			num5 = 64 - mBufPos;
			Array.Copy(inBytes, inOff, mBuf, mBufPos, num5);
			mPoly1305.BlockUpdate(mBuf, 0, 64);
			ProcessBlock(mBuf, 0, outBytes, outOff + num);
			inOff += num5;
			num += 64;
			while (inOff <= num7)
			{
				mPoly1305.BlockUpdate(inBytes, inOff, 128);
				ProcessBlocks2(inBytes, inOff, outBytes, outOff + num);
				inOff += 128;
				num += 128;
			}
			if (inOff <= num6)
			{
				mPoly1305.BlockUpdate(inBytes, inOff, 64);
				ProcessBlock(inBytes, inOff, outBytes, outOff + num);
				inOff += 64;
				num += 64;
			}
			mBufPos = mBuf.Length + num6 - inOff;
			Array.Copy(inBytes, inOff, mBuf, 0, mBufPos);
			break;
		}
		case State.EncData:
		{
			int num2 = 64 - mBufPos;
			if (len < num2)
			{
				Array.Copy(inBytes, inOff, mBuf, mBufPos, len);
				mBufPos += len;
				break;
			}
			int num3 = inOff + len - 64;
			int num4 = num3 - 64;
			if (mBufPos > 0)
			{
				Array.Copy(inBytes, inOff, mBuf, mBufPos, num2);
				ProcessBlock(mBuf, 0, outBytes, outOff);
				inOff += num2;
				num = 64;
			}
			while (inOff <= num4)
			{
				ProcessBlocks2(inBytes, inOff, outBytes, outOff + num);
				inOff += 128;
				num += 128;
			}
			if (inOff <= num3)
			{
				ProcessBlock(inBytes, inOff, outBytes, outOff + num);
				inOff += 64;
				num += 64;
			}
			mPoly1305.BlockUpdate(outBytes, outOff, num);
			mBufPos = 64 + num3 - inOff;
			Array.Copy(inBytes, inOff, mBuf, 0, mBufPos);
			break;
		}
		default:
			throw new InvalidOperationException();
		}
		return num;
	}

	public virtual int DoFinal(byte[] outBytes, int outOff)
	{
		if (outBytes == null)
		{
			throw new ArgumentNullException("outBytes");
		}
		if (outOff < 0)
		{
			throw new ArgumentException("cannot be negative", "outOff");
		}
		CheckData();
		Array.Clear(mMac, 0, 16);
		int num = 0;
		switch (mState)
		{
		case State.DecData:
			if (mBufPos < 16)
			{
				throw new InvalidCipherTextException("data too short");
			}
			num = mBufPos - 16;
			Check.OutputLength(outBytes, outOff, num, "output buffer too short");
			if (num > 0)
			{
				mPoly1305.BlockUpdate(mBuf, 0, num);
				ProcessData(mBuf, 0, num, outBytes, outOff);
			}
			FinishData(State.DecFinal);
			if (!Arrays.FixedTimeEquals(16, mMac, 0, mBuf, num))
			{
				throw new InvalidCipherTextException("mac check in ChaCha20Poly1305 failed");
			}
			break;
		case State.EncData:
			num = mBufPos + 16;
			Check.OutputLength(outBytes, outOff, num, "output buffer too short");
			if (mBufPos > 0)
			{
				ProcessData(mBuf, 0, mBufPos, outBytes, outOff);
				mPoly1305.BlockUpdate(outBytes, outOff, mBufPos);
			}
			FinishData(State.EncFinal);
			Array.Copy(mMac, 0, outBytes, outOff + mBufPos, 16);
			break;
		default:
			throw new InvalidOperationException();
		}
		Reset(clearMac: false, resetCipher: true);
		return num;
	}

	public virtual byte[] GetMac()
	{
		return Arrays.Clone(mMac);
	}

	public virtual void Reset()
	{
		Reset(clearMac: true, resetCipher: true);
	}

	private void CheckAad()
	{
		switch (mState)
		{
		case State.DecInit:
			mState = State.DecAad;
			break;
		case State.EncInit:
			mState = State.EncAad;
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

	private void CheckData()
	{
		switch (mState)
		{
		case State.DecInit:
		case State.DecAad:
			FinishAad(State.DecData);
			break;
		case State.EncInit:
		case State.EncAad:
			FinishAad(State.EncData);
			break;
		case State.EncFinal:
			throw new InvalidOperationException(AlgorithmName + " cannot be reused for encryption");
		default:
			throw new InvalidOperationException(AlgorithmName + " needs to be initialized");
		case State.EncData:
		case State.DecData:
			break;
		}
	}

	private void FinishAad(State nextState)
	{
		PadMac(mAadCount);
		mState = nextState;
	}

	private void FinishData(State nextState)
	{
		PadMac(mDataCount);
		byte[] array = new byte[16];
		Pack.UInt64_To_LE(mAadCount, array, 0);
		Pack.UInt64_To_LE(mDataCount, array, 8);
		mPoly1305.BlockUpdate(array, 0, 16);
		mPoly1305.DoFinal(mMac, 0);
		mState = nextState;
	}

	private ulong IncrementCount(ulong count, uint increment, ulong limit)
	{
		if (count > limit - increment)
		{
			throw new InvalidOperationException("Limit exceeded");
		}
		return count + increment;
	}

	private void InitMac()
	{
		byte[] array = new byte[64];
		try
		{
			mChacha20.ProcessBytes(array, 0, 64, array, 0);
			mPoly1305.Init(new KeyParameter(array, 0, 32));
		}
		finally
		{
			Array.Clear(array, 0, 64);
		}
	}

	private void PadMac(ulong count)
	{
		int num = (int)count & 0xF;
		if (num != 0)
		{
			mPoly1305.BlockUpdate(Zeroes, 0, 16 - num);
		}
	}

	private void ProcessBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
	{
		Check.OutputLength(outBytes, outOff, 64, "output buffer too short");
		mChacha20.ProcessBlock(inBytes, inOff, outBytes, outOff);
		mDataCount = IncrementCount(mDataCount, 64u, 274877906880uL);
	}

	private void ProcessBlocks2(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
	{
		Check.OutputLength(outBytes, outOff, 128, "output buffer too short");
		mChacha20.ProcessBlocks2(inBytes, inOff, outBytes, outOff);
		mDataCount = IncrementCount(mDataCount, 128u, 274877906880uL);
	}

	private void ProcessData(byte[] inBytes, int inOff, int inLen, byte[] outBytes, int outOff)
	{
		Check.OutputLength(outBytes, outOff, inLen, "output buffer too short");
		mChacha20.ProcessBytes(inBytes, inOff, inLen, outBytes, outOff);
		mDataCount = IncrementCount(mDataCount, (uint)inLen, 274877906880uL);
	}

	private void Reset(bool clearMac, bool resetCipher)
	{
		Array.Clear(mBuf, 0, mBuf.Length);
		if (clearMac)
		{
			Array.Clear(mMac, 0, mMac.Length);
		}
		mAadCount = 0uL;
		mDataCount = 0uL;
		mBufPos = 0;
		switch (mState)
		{
		case State.DecAad:
		case State.DecData:
		case State.DecFinal:
			mState = State.DecInit;
			break;
		case State.EncAad:
		case State.EncData:
		case State.EncFinal:
			mState = State.EncFinal;
			return;
		default:
			throw new InvalidOperationException(AlgorithmName + " needs to be initialized");
		case State.EncInit:
		case State.DecInit:
			break;
		}
		if (resetCipher)
		{
			mChacha20.Reset();
		}
		InitMac();
		if (mInitialAad != null)
		{
			ProcessAadBytes(mInitialAad, 0, mInitialAad.Length);
		}
	}
}
