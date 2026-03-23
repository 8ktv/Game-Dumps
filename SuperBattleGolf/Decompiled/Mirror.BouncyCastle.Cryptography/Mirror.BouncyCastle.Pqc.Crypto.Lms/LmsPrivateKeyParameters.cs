using System;
using System.Collections.Concurrent;
using System.IO;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class LmsPrivateKeyParameters : LmsKeyParameters, ILmsContextBasedSigner
{
	private byte[] I;

	private LMSigParameters sigParameters;

	private LMOtsParameters otsParameters;

	private int maxQ;

	private byte[] masterSecret;

	private ConcurrentDictionary<int, byte[]> tCache;

	private int maxCacheR;

	private IDigest tDigest;

	private int q;

	private readonly bool m_isPlaceholder;

	private LmsPublicKeyParameters m_publicKey;

	public LMSigParameters SigParameters => sigParameters;

	public LMOtsParameters OtsParameters => otsParameters;

	private static LmsPublicKeyParameters DerivePublicKey(LmsPrivateKeyParameters privateKey)
	{
		return new LmsPublicKeyParameters(privateKey.sigParameters, privateKey.otsParameters, privateKey.FindT(1), privateKey.I);
	}

	public LmsPrivateKeyParameters(LMSigParameters lmsParameter, LMOtsParameters otsParameters, int q, byte[] I, int maxQ, byte[] masterSecret)
		: this(lmsParameter, otsParameters, q, I, maxQ, masterSecret, isPlaceholder: false)
	{
	}

	internal LmsPrivateKeyParameters(LMSigParameters lmsParameter, LMOtsParameters otsParameters, int q, byte[] I, int maxQ, byte[] masterSecret, bool isPlaceholder)
		: base(isPrivateKey: true)
	{
		sigParameters = lmsParameter;
		this.otsParameters = otsParameters;
		this.q = q;
		this.I = Arrays.Clone(I);
		this.maxQ = maxQ;
		this.masterSecret = Arrays.Clone(masterSecret);
		maxCacheR = 1 << sigParameters.H + 1;
		tCache = new ConcurrentDictionary<int, byte[]>();
		tDigest = LmsUtilities.GetDigest(lmsParameter);
		m_isPlaceholder = isPlaceholder;
	}

	private LmsPrivateKeyParameters(LmsPrivateKeyParameters parent, int q, int maxQ)
		: base(isPrivateKey: true)
	{
		sigParameters = parent.sigParameters;
		otsParameters = parent.otsParameters;
		this.q = q;
		I = parent.I;
		this.maxQ = maxQ;
		masterSecret = parent.masterSecret;
		maxCacheR = 1 << sigParameters.H;
		tCache = parent.tCache;
		tDigest = LmsUtilities.GetDigest(sigParameters);
		m_publicKey = parent.m_publicKey;
	}

	public static LmsPrivateKeyParameters GetInstance(byte[] privEnc, byte[] pubEnc)
	{
		LmsPrivateKeyParameters instance = GetInstance(privEnc);
		instance.m_publicKey = LmsPublicKeyParameters.GetInstance(pubEnc);
		return instance;
	}

	public static LmsPrivateKeyParameters GetInstance(object src)
	{
		if (src is LmsPrivateKeyParameters result)
		{
			return result;
		}
		if (src is BinaryReader binaryReader)
		{
			return Parse(binaryReader);
		}
		if (src is Stream stream)
		{
			return BinaryReaders.Parse(Parse, stream, leaveOpen: true);
		}
		if (src is byte[] buffer)
		{
			return BinaryReaders.Parse(Parse, new MemoryStream(buffer, writable: false), leaveOpen: false);
		}
		throw new ArgumentException($"cannot parse {src}");
	}

	internal static LmsPrivateKeyParameters Parse(BinaryReader binaryReader)
	{
		if (BinaryReaders.ReadInt32BigEndian(binaryReader) != 0)
		{
			throw new Exception("unknown version for LMS private key");
		}
		LMSigParameters lmsParameter = LMSigParameters.ParseByID(binaryReader);
		LMOtsParameters lMOtsParameters = LMOtsParameters.ParseByID(binaryReader);
		byte[] i = BinaryReaders.ReadBytesFully(binaryReader, 16);
		int num = BinaryReaders.ReadInt32BigEndian(binaryReader);
		int num2 = BinaryReaders.ReadInt32BigEndian(binaryReader);
		int num3 = BinaryReaders.ReadInt32BigEndian(binaryReader);
		if (num3 < 0)
		{
			throw new Exception("secret length less than zero");
		}
		byte[] array = BinaryReaders.ReadBytesFully(binaryReader, num3);
		return new LmsPrivateKeyParameters(lmsParameter, lMOtsParameters, num, i, num2, array);
	}

	internal LMOtsPrivateKey GetCurrentOtsKey()
	{
		lock (this)
		{
			if (q >= maxQ)
			{
				throw new Exception("ots private keys expired");
			}
			return new LMOtsPrivateKey(otsParameters, I, q, masterSecret);
		}
	}

	public int GetIndex()
	{
		lock (this)
		{
			return q;
		}
	}

	internal void IncIndex()
	{
		lock (this)
		{
			q++;
		}
	}

	public LmsContext GenerateLmsContext()
	{
		int h = SigParameters.H;
		int index = GetIndex();
		LMOtsPrivateKey nextOtsPrivateKey = GetNextOtsPrivateKey();
		int num = 0;
		int num2 = (1 << h) + index;
		byte[][] array = new byte[h][];
		while (num < h)
		{
			int r = (num2 / (1 << num)) ^ 1;
			array[num++] = FindT(r);
		}
		return nextOtsPrivateKey.GetSignatureContext(sigParameters, array);
	}

	public byte[] GenerateSignature(LmsContext context)
	{
		try
		{
			return Lms.GenerateSign(context).GetEncoded();
		}
		catch (IOException ex)
		{
			throw new Exception("unable to encode signature: " + ex.Message, ex);
		}
	}

	internal LMOtsPrivateKey GetNextOtsPrivateKey()
	{
		if (m_isPlaceholder)
		{
			throw new Exception("placeholder only");
		}
		lock (this)
		{
			if (q >= maxQ)
			{
				throw new Exception("ots private key exhausted");
			}
			LMOtsPrivateKey result = new LMOtsPrivateKey(otsParameters, I, q, masterSecret);
			IncIndex();
			return result;
		}
	}

	public LmsPrivateKeyParameters ExtractKeyShard(int usageCount)
	{
		lock (this)
		{
			if (q + usageCount >= maxQ)
			{
				throw new ArgumentException("usageCount exceeds usages remaining");
			}
			LmsPrivateKeyParameters result = new LmsPrivateKeyParameters(this, q, q + usageCount);
			q += usageCount;
			return result;
		}
	}

	[Obsolete("Use 'SigParameters' instead")]
	public LMSigParameters GetSigParameters()
	{
		return sigParameters;
	}

	[Obsolete("Use 'OtsParameters' instead")]
	public LMOtsParameters GetOtsParameters()
	{
		return otsParameters;
	}

	public byte[] GetI()
	{
		return Arrays.Clone(I);
	}

	public byte[] GetMasterSecret()
	{
		return Arrays.Clone(masterSecret);
	}

	public long GetUsagesRemaining()
	{
		return maxQ - GetIndex();
	}

	public LmsPublicKeyParameters GetPublicKey()
	{
		if (m_isPlaceholder)
		{
			throw new Exception("placeholder only");
		}
		return Objects.EnsureSingletonInitialized(ref m_publicKey, this, DerivePublicKey);
	}

	internal byte[] FindT(int r)
	{
		if (r >= maxCacheR)
		{
			return CalcT(r);
		}
		return tCache.GetOrAdd(r, CalcT);
	}

	private byte[] CalcT(int r)
	{
		int h = sigParameters.H;
		int num = 1 << h;
		byte[] array = new byte[tDigest.GetDigestSize()];
		if (r >= num)
		{
			LmsUtilities.ByteArray(I, tDigest);
			LmsUtilities.U32Str(r, tDigest);
			LmsUtilities.U16Str((short)Lms.D_LEAF, tDigest);
			LmsUtilities.ByteArray(LMOts.LmsOtsGeneratePublicKey(otsParameters, I, r - num, masterSecret), tDigest);
		}
		else
		{
			byte[] array2 = FindT(2 * r);
			byte[] array3 = FindT(2 * r + 1);
			LmsUtilities.ByteArray(I, tDigest);
			LmsUtilities.U32Str(r, tDigest);
			LmsUtilities.U16Str((short)Lms.D_INTR, tDigest);
			LmsUtilities.ByteArray(array2, tDigest);
			LmsUtilities.ByteArray(array3, tDigest);
		}
		tDigest.DoFinal(array, 0);
		return array;
	}

	public override bool Equals(object o)
	{
		if (this == o)
		{
			return true;
		}
		if (o is LmsPrivateKeyParameters lmsPrivateKeyParameters && q == lmsPrivateKeyParameters.q && maxQ == lmsPrivateKeyParameters.maxQ && Arrays.AreEqual(I, lmsPrivateKeyParameters.I) && object.Equals(sigParameters, lmsPrivateKeyParameters.sigParameters) && object.Equals(otsParameters, lmsPrivateKeyParameters.otsParameters))
		{
			return Arrays.AreEqual(masterSecret, lmsPrivateKeyParameters.masterSecret);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = q;
		num = 31 * num + maxQ;
		num = 31 * num + Arrays.GetHashCode(I);
		num = 31 * num + Objects.GetHashCode(sigParameters);
		num = 31 * num + Objects.GetHashCode(otsParameters);
		return 31 * num + Arrays.GetHashCode(masterSecret);
	}

	public override byte[] GetEncoded()
	{
		return Composer.Compose().U32Str(0).U32Str(sigParameters.ID)
			.U32Str(otsParameters.ID)
			.Bytes(I)
			.U32Str(q)
			.U32Str(maxQ)
			.U32Str(masterSecret.Length)
			.Bytes(masterSecret)
			.Build();
	}
}
