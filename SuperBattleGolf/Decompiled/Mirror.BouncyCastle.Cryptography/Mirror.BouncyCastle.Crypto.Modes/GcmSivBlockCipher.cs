using System;
using System.IO;
using Mirror.BouncyCastle.Crypto.Modes.Gcm;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.Modes;

public class GcmSivBlockCipher : IAeadBlockCipher, IAeadCipher
{
	private class GcmSivCache : MemoryStream
	{
		internal GcmSivCache()
		{
		}
	}

	private class GcmSivHasher
	{
		private readonly byte[] theBuffer = new byte[BUFLEN];

		private readonly byte[] theByte = new byte[1];

		private int numActive;

		private ulong numHashed;

		private readonly GcmSivBlockCipher parent;

		internal GcmSivHasher(GcmSivBlockCipher parent)
		{
			this.parent = parent;
		}

		internal ulong getBytesProcessed()
		{
			return numHashed;
		}

		internal void Reset()
		{
			numActive = 0;
			numHashed = 0uL;
		}

		internal void UpdateHash(byte pByte)
		{
			theByte[0] = pByte;
			UpdateHash(theByte, 0, 1);
		}

		internal void UpdateHash(byte[] pBuffer, int pOffset, int pLen)
		{
			int num = BUFLEN - numActive;
			int num2 = 0;
			int num3 = pLen;
			if (numActive > 0 && pLen >= num)
			{
				Array.Copy(pBuffer, pOffset, theBuffer, numActive, num);
				fillReverse(theBuffer, 0, BUFLEN, parent.theReverse);
				parent.gHASH(parent.theReverse);
				num2 += num;
				num3 -= num;
				numActive = 0;
			}
			while (num3 >= BUFLEN)
			{
				fillReverse(pBuffer, pOffset + num2, BUFLEN, parent.theReverse);
				parent.gHASH(parent.theReverse);
				num2 += BUFLEN;
				num3 -= BUFLEN;
			}
			if (num3 > 0)
			{
				Array.Copy(pBuffer, pOffset + num2, theBuffer, numActive, num3);
				numActive += num3;
			}
			numHashed += (ulong)pLen;
		}

		internal void completeHash()
		{
			if (numActive > 0)
			{
				Arrays.Fill(parent.theReverse, 0);
				fillReverse(theBuffer, 0, numActive, parent.theReverse);
				parent.gHASH(parent.theReverse);
			}
		}
	}

	private static readonly int BUFLEN = 16;

	private static readonly int HALFBUFLEN = BUFLEN >> 1;

	private static readonly int NONCELEN = 12;

	private static readonly int MAX_DATALEN = 2147483639 - BUFLEN;

	private static readonly byte MASK = 128;

	private static readonly byte ADD = 225;

	private static readonly int INIT = 1;

	private static readonly int AEAD_COMPLETE = 2;

	private readonly IBlockCipher theCipher;

	private readonly IGcmMultiplier theMultiplier;

	internal readonly byte[] theGHash = new byte[BUFLEN];

	internal readonly byte[] theReverse = new byte[BUFLEN];

	private readonly GcmSivHasher theAEADHasher;

	private readonly GcmSivHasher theDataHasher;

	private GcmSivCache thePlain;

	private GcmSivCache theEncData;

	private bool forEncryption;

	private byte[] theInitialAEAD;

	private byte[] theNonce;

	private int theFlags;

	public virtual IBlockCipher UnderlyingCipher => theCipher;

	public virtual string AlgorithmName => theCipher.AlgorithmName + "-GCM-SIV";

	public GcmSivBlockCipher()
		: this(AesUtilities.CreateEngine())
	{
	}

	public GcmSivBlockCipher(IBlockCipher pCipher)
		: this(pCipher, null)
	{
	}

	[Obsolete("Will be removed")]
	public GcmSivBlockCipher(IBlockCipher pCipher, IGcmMultiplier pMultiplier)
	{
		if (pCipher.GetBlockSize() != BUFLEN)
		{
			int bUFLEN = BUFLEN;
			throw new ArgumentException("Cipher required with a block size of " + bUFLEN + ".");
		}
		if (pMultiplier == null)
		{
			pMultiplier = GcmBlockCipher.CreateGcmMultiplier();
		}
		theCipher = pCipher;
		theMultiplier = pMultiplier;
		theAEADHasher = new GcmSivHasher(this);
		theDataHasher = new GcmSivHasher(this);
	}

	public virtual int GetBlockSize()
	{
		return theCipher.GetBlockSize();
	}

	public virtual void Init(bool pEncrypt, ICipherParameters cipherParameters)
	{
		byte[] array = null;
		byte[] array2;
		KeyParameter keyParameter;
		if (cipherParameters is AeadParameters aeadParameters)
		{
			array = aeadParameters.GetAssociatedText();
			array2 = aeadParameters.GetNonce();
			keyParameter = aeadParameters.Key;
		}
		else
		{
			if (!(cipherParameters is ParametersWithIV parametersWithIV))
			{
				throw new ArgumentException("invalid parameters passed to GCM_SIV");
			}
			array2 = parametersWithIV.GetIV();
			keyParameter = (KeyParameter)parametersWithIV.Parameters;
		}
		if (array2.Length != NONCELEN)
		{
			throw new ArgumentException("Invalid nonce");
		}
		if (keyParameter == null)
		{
			throw new ArgumentException("Invalid key");
		}
		int keyLength = keyParameter.KeyLength;
		if (keyLength != BUFLEN && keyLength != BUFLEN << 1)
		{
			throw new ArgumentException("Invalid key");
		}
		forEncryption = pEncrypt;
		theInitialAEAD = array;
		theNonce = array2;
		DeriveKeys(keyParameter);
		ResetStreams();
	}

	private void CheckAeadStatus(int pLen)
	{
		if ((theFlags & INIT) == 0)
		{
			throw new InvalidOperationException("Cipher is not initialised");
		}
		if ((theFlags & AEAD_COMPLETE) != 0)
		{
			throw new InvalidOperationException("AEAD data cannot be processed after ordinary data");
		}
		if ((long)theAEADHasher.getBytesProcessed() + long.MinValue > MAX_DATALEN - pLen + long.MinValue)
		{
			throw new InvalidOperationException("AEAD byte count exceeded");
		}
	}

	private void CheckStatus(int pLen)
	{
		if ((theFlags & INIT) == 0)
		{
			throw new InvalidOperationException("Cipher is not initialised");
		}
		if ((theFlags & AEAD_COMPLETE) == 0)
		{
			theAEADHasher.completeHash();
			theFlags |= AEAD_COMPLETE;
		}
		long num = MAX_DATALEN;
		long length = thePlain.Length;
		if (!forEncryption)
		{
			num += BUFLEN;
			length = theEncData.Length;
		}
		if (length + long.MinValue > num - pLen + long.MinValue)
		{
			throw new InvalidOperationException("byte count exceeded");
		}
	}

	public virtual void ProcessAadByte(byte pByte)
	{
		CheckAeadStatus(1);
		theAEADHasher.UpdateHash(pByte);
	}

	public virtual void ProcessAadBytes(byte[] pData, int pOffset, int pLen)
	{
		Check.DataLength(pData, pOffset, pLen, "input buffer too short");
		CheckAeadStatus(pLen);
		theAEADHasher.UpdateHash(pData, pOffset, pLen);
	}

	public virtual int ProcessByte(byte pByte, byte[] pOutput, int pOutOffset)
	{
		CheckStatus(1);
		if (forEncryption)
		{
			thePlain.WriteByte(pByte);
			theDataHasher.UpdateHash(pByte);
		}
		else
		{
			theEncData.WriteByte(pByte);
		}
		return 0;
	}

	public virtual int ProcessBytes(byte[] pData, int pOffset, int pLen, byte[] pOutput, int pOutOffset)
	{
		Check.DataLength(pData, pOffset, pLen, "input buffer too short");
		CheckStatus(pLen);
		if (forEncryption)
		{
			thePlain.Write(pData, pOffset, pLen);
			theDataHasher.UpdateHash(pData, pOffset, pLen);
		}
		else
		{
			theEncData.Write(pData, pOffset, pLen);
		}
		return 0;
	}

	public virtual int DoFinal(byte[] pOutput, int pOffset)
	{
		Check.OutputLength(pOutput, pOffset, GetOutputSize(0), "output buffer too short");
		CheckStatus(0);
		if (forEncryption)
		{
			byte[] array = CalculateTag();
			int result = BUFLEN + EncryptPlain(array, pOutput, pOffset);
			Array.Copy(array, 0, pOutput, pOffset + Convert.ToInt32(thePlain.Length), BUFLEN);
			ResetStreams();
			return result;
		}
		DecryptPlain();
		int result2 = Streams.WriteBufTo(thePlain, pOutput, pOffset);
		ResetStreams();
		return result2;
	}

	public virtual byte[] GetMac()
	{
		throw new InvalidOperationException();
	}

	public virtual int GetUpdateOutputSize(int pLen)
	{
		return 0;
	}

	public virtual int GetOutputSize(int pLen)
	{
		if (forEncryption)
		{
			return pLen + Convert.ToInt32(thePlain.Length) + BUFLEN;
		}
		int num = pLen + Convert.ToInt32(theEncData.Length);
		if (num <= BUFLEN)
		{
			return 0;
		}
		return num - BUFLEN;
	}

	public virtual void Reset()
	{
		ResetStreams();
	}

	private void ResetStreams()
	{
		if (thePlain != null)
		{
			int length = Convert.ToInt32(thePlain.Length);
			Array.Clear(thePlain.GetBuffer(), 0, length);
			thePlain.SetLength(0L);
		}
		theAEADHasher.Reset();
		theDataHasher.Reset();
		thePlain = new GcmSivCache();
		theEncData = (forEncryption ? null : new GcmSivCache());
		theFlags &= ~AEAD_COMPLETE;
		Arrays.Fill(theGHash, 0);
		if (theInitialAEAD != null)
		{
			theAEADHasher.UpdateHash(theInitialAEAD, 0, theInitialAEAD.Length);
		}
	}

	private static int bufLength(byte[] pBuffer)
	{
		if (pBuffer != null)
		{
			return pBuffer.Length;
		}
		return 0;
	}

	private int EncryptPlain(byte[] pCounter, byte[] pTarget, int pOffset)
	{
		byte[] buffer = thePlain.GetBuffer();
		int num = Convert.ToInt32(thePlain.Length);
		byte[] pRight = buffer;
		byte[] array = Arrays.Clone(pCounter);
		array[BUFLEN - 1] |= MASK;
		byte[] array2 = new byte[BUFLEN];
		long num2 = num;
		int num3 = 0;
		while (num2 > 0)
		{
			theCipher.ProcessBlock(array, 0, array2, 0);
			int num4 = (int)System.Math.Min(BUFLEN, num2);
			xorBlock(array2, pRight, num3, num4);
			Array.Copy(array2, 0, pTarget, pOffset + num3, num4);
			num2 -= num4;
			num3 += num4;
			incrementCounter(array);
		}
		return num;
	}

	private void DecryptPlain()
	{
		byte[] buffer = theEncData.GetBuffer();
		int num = Convert.ToInt32(theEncData.Length);
		byte[] array = buffer;
		int num2 = num - BUFLEN;
		if (num2 < 0)
		{
			throw new InvalidCipherTextException("Data too short");
		}
		byte[] array2 = Arrays.CopyOfRange(array, num2, num2 + BUFLEN);
		byte[] array3 = Arrays.Clone(array2);
		array3[BUFLEN - 1] |= MASK;
		byte[] array4 = new byte[BUFLEN];
		int num3 = 0;
		while (num2 > 0)
		{
			theCipher.ProcessBlock(array3, 0, array4, 0);
			int num4 = System.Math.Min(BUFLEN, num2);
			xorBlock(array4, array, num3, num4);
			thePlain.Write(array4, 0, num4);
			theDataHasher.UpdateHash(array4, 0, num4);
			num2 -= num4;
			num3 += num4;
			incrementCounter(array3);
		}
		if (!Arrays.FixedTimeEquals(CalculateTag(), array2))
		{
			Reset();
			throw new InvalidCipherTextException("mac check failed");
		}
	}

	private byte[] CalculateTag()
	{
		theDataHasher.completeHash();
		byte[] array = completePolyVal();
		byte[] array2 = new byte[BUFLEN];
		for (int i = 0; i < NONCELEN; i++)
		{
			array[i] ^= theNonce[i];
		}
		array[BUFLEN - 1] &= (byte)(MASK - 1);
		theCipher.ProcessBlock(array, 0, array2, 0);
		return array2;
	}

	private byte[] completePolyVal()
	{
		byte[] array = new byte[BUFLEN];
		gHashLengths();
		fillReverse(theGHash, 0, BUFLEN, array);
		return array;
	}

	private void gHashLengths()
	{
		byte[] array = new byte[BUFLEN];
		Pack.UInt64_To_BE(8 * theDataHasher.getBytesProcessed(), array, 0);
		Pack.UInt64_To_BE(8 * theAEADHasher.getBytesProcessed(), array, 8);
		gHASH(array);
	}

	private void gHASH(byte[] pNext)
	{
		xorBlock(theGHash, pNext);
		theMultiplier.MultiplyH(theGHash);
	}

	private static void fillReverse(byte[] pInput, int pOffset, int pLength, byte[] pOutput)
	{
		int num = 0;
		int num2 = BUFLEN - 1;
		while (num < pLength)
		{
			pOutput[num2] = pInput[pOffset + num];
			num++;
			num2--;
		}
	}

	private static void xorBlock(byte[] pLeft, byte[] pRight)
	{
		for (int i = 0; i < BUFLEN; i++)
		{
			pLeft[i] ^= pRight[i];
		}
	}

	private static void xorBlock(byte[] pLeft, byte[] pRight, int pOffset, int pLength)
	{
		for (int i = 0; i < pLength; i++)
		{
			pLeft[i] ^= pRight[i + pOffset];
		}
	}

	private static void incrementCounter(byte[] pCounter)
	{
		for (int i = 0; i < 4; i++)
		{
			if (++pCounter[i] != 0)
			{
				break;
			}
		}
	}

	private static void mulX(byte[] pValue)
	{
		byte b = 0;
		for (int i = 0; i < BUFLEN; i++)
		{
			byte b2 = pValue[i];
			pValue[i] = (byte)(((b2 >> 1) & ~MASK) | b);
			b = (byte)(((b2 & 1) != 0) ? MASK : 0);
		}
		if (b != 0)
		{
			pValue[0] ^= ADD;
		}
	}

	private void DeriveKeys(KeyParameter pKey)
	{
		byte[] array = new byte[BUFLEN];
		byte[] array2 = new byte[BUFLEN];
		byte[] array3 = new byte[BUFLEN];
		byte[] array4 = new byte[pKey.KeyLength];
		Array.Copy(theNonce, 0, array, BUFLEN - NONCELEN, NONCELEN);
		theCipher.Init(forEncryption: true, pKey);
		int num = 0;
		theCipher.ProcessBlock(array, 0, array2, 0);
		Array.Copy(array2, 0, array3, num, HALFBUFLEN);
		array[0]++;
		num += HALFBUFLEN;
		theCipher.ProcessBlock(array, 0, array2, 0);
		Array.Copy(array2, 0, array3, num, HALFBUFLEN);
		array[0]++;
		num = 0;
		theCipher.ProcessBlock(array, 0, array2, 0);
		Array.Copy(array2, 0, array4, num, HALFBUFLEN);
		array[0]++;
		num += HALFBUFLEN;
		theCipher.ProcessBlock(array, 0, array2, 0);
		Array.Copy(array2, 0, array4, num, HALFBUFLEN);
		if (array4.Length == BUFLEN << 1)
		{
			array[0]++;
			num += HALFBUFLEN;
			theCipher.ProcessBlock(array, 0, array2, 0);
			Array.Copy(array2, 0, array4, num, HALFBUFLEN);
			array[0]++;
			num += HALFBUFLEN;
			theCipher.ProcessBlock(array, 0, array2, 0);
			Array.Copy(array2, 0, array4, num, HALFBUFLEN);
		}
		theCipher.Init(forEncryption: true, new KeyParameter(array4));
		fillReverse(array3, 0, BUFLEN, array2);
		mulX(array2);
		theMultiplier.Init(array2);
		theFlags |= INIT;
	}
}
