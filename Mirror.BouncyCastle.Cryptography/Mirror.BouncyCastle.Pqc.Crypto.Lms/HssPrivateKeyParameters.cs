using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public class HssPrivateKeyParameters : LmsKeyParameters, ILmsContextBasedSigner
{
	private readonly int m_level;

	private readonly bool m_isShard;

	private IList<LmsPrivateKeyParameters> m_keys;

	private IList<LmsSignature> m_sig;

	private readonly long m_indexLimit;

	private long m_index;

	private HssPublicKeyParameters m_publicKey;

	[Obsolete("Use 'Level' instead")]
	public int L => m_level;

	public int Level => m_level;

	public long IndexLimit => m_indexLimit;

	public HssPrivateKeyParameters(int l, IList<LmsPrivateKeyParameters> keys, IList<LmsSignature> sig, long index, long indexLimit)
		: base(isPrivateKey: true)
	{
		m_level = l;
		m_isShard = false;
		m_keys = new List<LmsPrivateKeyParameters>(keys);
		m_sig = new List<LmsSignature>(sig);
		m_index = index;
		m_indexLimit = indexLimit;
		ResetKeyToIndex();
	}

	private HssPrivateKeyParameters(int l, IList<LmsPrivateKeyParameters> keys, IList<LmsSignature> sig, long index, long indexLimit, bool isShard)
		: base(isPrivateKey: true)
	{
		m_level = l;
		m_isShard = isShard;
		m_keys = new List<LmsPrivateKeyParameters>(keys);
		m_sig = new List<LmsSignature>(sig);
		m_index = index;
		m_indexLimit = indexLimit;
	}

	public static HssPrivateKeyParameters GetInstance(byte[] privEnc, byte[] pubEnc)
	{
		HssPrivateKeyParameters instance = GetInstance(privEnc);
		instance.m_publicKey = HssPublicKeyParameters.GetInstance(pubEnc);
		return instance;
	}

	public static HssPrivateKeyParameters GetInstance(object src)
	{
		if (src is HssPrivateKeyParameters result)
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

	internal static HssPrivateKeyParameters Parse(BinaryReader binaryReader)
	{
		if (BinaryReaders.ReadInt32BigEndian(binaryReader) != 0)
		{
			throw new Exception("unknown version for HSS private key");
		}
		int num = BinaryReaders.ReadInt32BigEndian(binaryReader);
		long index = BinaryReaders.ReadInt64BigEndian(binaryReader);
		long indexLimit = BinaryReaders.ReadInt64BigEndian(binaryReader);
		bool isShard = binaryReader.ReadBoolean();
		List<LmsPrivateKeyParameters> list = new List<LmsPrivateKeyParameters>(num);
		for (int i = 0; i < num; i++)
		{
			list.Add(LmsPrivateKeyParameters.Parse(binaryReader));
		}
		List<LmsSignature> list2 = new List<LmsSignature>(num - 1);
		for (int j = 1; j < num; j++)
		{
			list2.Add(LmsSignature.Parse(binaryReader));
		}
		return new HssPrivateKeyParameters(num, list, list2, index, indexLimit, isShard);
	}

	public long GetIndex()
	{
		lock (this)
		{
			return m_index;
		}
	}

	public LmsParameters[] GetLmsParameters()
	{
		lock (this)
		{
			int count = m_keys.Count;
			LmsParameters[] array = new LmsParameters[count];
			for (int i = 0; i < count; i++)
			{
				LmsPrivateKeyParameters lmsPrivateKeyParameters = m_keys[i];
				array[i] = new LmsParameters(lmsPrivateKeyParameters.SigParameters, lmsPrivateKeyParameters.OtsParameters);
			}
			return array;
		}
	}

	internal void IncIndex()
	{
		lock (this)
		{
			m_index++;
		}
	}

	private static HssPrivateKeyParameters MakeCopy(HssPrivateKeyParameters privateKeyParameters)
	{
		return GetInstance(privateKeyParameters.GetEncoded());
	}

	protected void UpdateHierarchy(IList<LmsPrivateKeyParameters> newKeys, IList<LmsSignature> newSig)
	{
		lock (this)
		{
			m_keys = new List<LmsPrivateKeyParameters>(newKeys);
			m_sig = new List<LmsSignature>(newSig);
		}
	}

	public bool IsShard()
	{
		return m_isShard;
	}

	public long GetUsagesRemaining()
	{
		return m_indexLimit - m_index;
	}

	internal LmsPrivateKeyParameters GetRootKey()
	{
		return m_keys[0];
	}

	public HssPrivateKeyParameters ExtractKeyShard(int usageCount)
	{
		lock (this)
		{
			if (GetUsagesRemaining() < usageCount)
			{
				throw new ArgumentException("usageCount exceeds usages remaining in current leaf");
			}
			long indexLimit = m_index + usageCount;
			long index = m_index;
			m_index += usageCount;
			List<LmsPrivateKeyParameters> keys = new List<LmsPrivateKeyParameters>(GetKeys());
			List<LmsSignature> sig = new List<LmsSignature>(GetSig());
			HssPrivateKeyParameters result = MakeCopy(new HssPrivateKeyParameters(m_level, keys, sig, index, indexLimit, isShard: true));
			ResetKeyToIndex();
			return result;
		}
	}

	public IList<LmsPrivateKeyParameters> GetKeys()
	{
		lock (this)
		{
			return m_keys;
		}
	}

	internal IList<LmsSignature> GetSig()
	{
		lock (this)
		{
			return m_sig;
		}
	}

	private void ResetKeyToIndex()
	{
		IList<LmsPrivateKeyParameters> keys = GetKeys();
		long[] array = new long[keys.Count];
		long num = GetIndex();
		for (int num2 = keys.Count - 1; num2 >= 0; num2--)
		{
			LMSigParameters sigParameters = keys[num2].SigParameters;
			int num3 = (1 << sigParameters.H) - 1;
			array[num2] = num & num3;
			num >>= sigParameters.H;
		}
		bool flag = false;
		LmsPrivateKeyParameters rootKey = GetRootKey();
		if (m_keys[0].GetIndex() - 1 != array[0])
		{
			m_keys[0] = Lms.GenerateKeys(rootKey.SigParameters, rootKey.OtsParameters, (int)array[0], rootKey.GetI(), rootKey.GetMasterSecret());
			flag = true;
		}
		for (int i = 1; i < array.Length; i++)
		{
			LmsPrivateKeyParameters lmsPrivateKeyParameters = m_keys[i - 1];
			int n = lmsPrivateKeyParameters.OtsParameters.N;
			byte[] array2 = new byte[16];
			byte[] array3 = new byte[n];
			SeedDerive seedDerive = new SeedDerive(lmsPrivateKeyParameters.GetI(), lmsPrivateKeyParameters.GetMasterSecret(), LmsUtilities.GetDigest(lmsPrivateKeyParameters.OtsParameters));
			seedDerive.Q = (int)array[i - 1];
			seedDerive.J = -2;
			seedDerive.DeriveSeed(incJ: true, array3, 0);
			byte[] array4 = new byte[n];
			seedDerive.DeriveSeed(incJ: false, array4, 0);
			Array.Copy(array4, 0, array2, 0, array2.Length);
			bool flag2 = ((i < array.Length - 1) ? (array[i] == m_keys[i].GetIndex() - 1) : (array[i] == m_keys[i].GetIndex()));
			if (!Arrays.AreEqual(array2, m_keys[i].GetI()) || !Arrays.AreEqual(array3, m_keys[i].GetMasterSecret()))
			{
				m_keys[i] = Lms.GenerateKeys(keys[i].SigParameters, keys[i].OtsParameters, (int)array[i], array2, array3);
				m_sig[i - 1] = Lms.GenerateSign(m_keys[i - 1], m_keys[i].GetPublicKey().ToByteArray());
				flag = true;
			}
			else if (!flag2)
			{
				m_keys[i] = Lms.GenerateKeys(keys[i].SigParameters, keys[i].OtsParameters, (int)array[i], array2, array3);
				flag = true;
			}
		}
		if (flag)
		{
			UpdateHierarchy(m_keys, m_sig);
		}
	}

	public HssPublicKeyParameters GetPublicKey()
	{
		lock (this)
		{
			return new HssPublicKeyParameters(m_level, GetRootKey().GetPublicKey());
		}
	}

	internal void ReplaceConsumedKey(int d)
	{
		LMOtsPrivateKey currentOtsKey = m_keys[d - 1].GetCurrentOtsKey();
		int n = currentOtsKey.Parameters.N;
		SeedDerive derivationFunction = currentOtsKey.GetDerivationFunction();
		derivationFunction.J = -2;
		byte[] array = new byte[n];
		derivationFunction.DeriveSeed(incJ: true, array, 0);
		byte[] array2 = new byte[n];
		derivationFunction.DeriveSeed(incJ: false, array2, 0);
		byte[] array3 = new byte[16];
		Array.Copy(array2, 0, array3, 0, array3.Length);
		List<LmsPrivateKeyParameters> list = new List<LmsPrivateKeyParameters>(m_keys);
		LmsPrivateKeyParameters lmsPrivateKeyParameters = m_keys[d];
		list[d] = Lms.GenerateKeys(lmsPrivateKeyParameters.SigParameters, lmsPrivateKeyParameters.OtsParameters, 0, array3, array);
		List<LmsSignature> list2 = new List<LmsSignature>(m_sig);
		list2[d - 1] = Lms.GenerateSign(list[d - 1], list[d].GetPublicKey().ToByteArray());
		m_keys = new List<LmsPrivateKeyParameters>(list);
		m_sig = new List<LmsSignature>(list2);
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj is HssPrivateKeyParameters hssPrivateKeyParameters && m_level == hssPrivateKeyParameters.m_level && m_isShard == hssPrivateKeyParameters.m_isShard && m_indexLimit == hssPrivateKeyParameters.m_indexLimit && m_index == hssPrivateKeyParameters.m_index && CompareLists(m_keys, hssPrivateKeyParameters.m_keys))
		{
			return CompareLists(m_sig, hssPrivateKeyParameters.m_sig);
		}
		return false;
	}

	public override byte[] GetEncoded()
	{
		lock (this)
		{
			Composer composer = Composer.Compose().U32Str(0).U32Str(m_level)
				.U64Str(m_index)
				.U64Str(m_indexLimit)
				.Boolean(m_isShard);
			foreach (LmsPrivateKeyParameters key in m_keys)
			{
				composer.Bytes(key);
			}
			foreach (LmsSignature item in m_sig)
			{
				composer.Bytes(item);
			}
			return composer.Build();
		}
	}

	public override int GetHashCode()
	{
		int level = m_level;
		int num = 31 * level;
		bool isShard = m_isShard;
		level = num + isShard.GetHashCode();
		level = 31 * level + m_keys.GetHashCode();
		level = 31 * level + m_sig.GetHashCode();
		int num2 = 31 * level;
		long indexLimit = m_indexLimit;
		level = num2 + indexLimit.GetHashCode();
		return 31 * level + m_index.GetHashCode();
	}

	protected object Clone()
	{
		return MakeCopy(this);
	}

	public LmsContext GenerateLmsContext()
	{
		int level = Level;
		LmsPrivateKeyParameters lmsPrivateKeyParameters;
		LmsSignedPubKey[] array;
		lock (this)
		{
			Hss.RangeTestKeys(this);
			IList<LmsPrivateKeyParameters> keys = GetKeys();
			IList<LmsSignature> sig = GetSig();
			lmsPrivateKeyParameters = GetKeys()[level - 1];
			int i = 0;
			array = new LmsSignedPubKey[level - 1];
			for (; i < level - 1; i++)
			{
				array[i] = new LmsSignedPubKey(sig[i], keys[i + 1].GetPublicKey());
			}
			IncIndex();
		}
		return lmsPrivateKeyParameters.GenerateLmsContext().WithSignedPublicKeys(array);
	}

	public byte[] GenerateSignature(LmsContext context)
	{
		try
		{
			return Hss.GenerateSignature(Level, context).GetEncoded();
		}
		catch (IOException ex)
		{
			throw new Exception("unable to encode signature: " + ex.Message, ex);
		}
	}

	private static bool CompareLists<T>(IList<T> arr1, IList<T> arr2)
	{
		for (int i = 0; i < arr1.Count && i < arr2.Count; i++)
		{
			if (!object.Equals(arr1[i], arr2[i]))
			{
				return false;
			}
		}
		return true;
	}
}
