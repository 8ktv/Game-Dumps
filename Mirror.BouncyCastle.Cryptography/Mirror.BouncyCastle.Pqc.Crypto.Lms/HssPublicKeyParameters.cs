using System;
using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class HssPublicKeyParameters : LmsKeyParameters, ILmsContextBasedVerifier
{
	private readonly int m_level;

	private readonly LmsPublicKeyParameters m_lmsPublicKey;

	[Obsolete("Use 'Level' instead")]
	public int L => m_level;

	public int Level => m_level;

	public LmsPublicKeyParameters LmsPublicKey => m_lmsPublicKey;

	public HssPublicKeyParameters(int l, LmsPublicKeyParameters lmsPublicKey)
		: base(isPrivateKey: false)
	{
		m_level = l;
		m_lmsPublicKey = lmsPublicKey ?? throw new ArgumentNullException("lmsPublicKey");
	}

	public static HssPublicKeyParameters GetInstance(object src)
	{
		if (src is HssPublicKeyParameters result)
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

	internal static HssPublicKeyParameters Parse(BinaryReader binaryReader)
	{
		int l = BinaryReaders.ReadInt32BigEndian(binaryReader);
		LmsPublicKeyParameters lmsPublicKey = LmsPublicKeyParameters.Parse(binaryReader);
		return new HssPublicKeyParameters(l, lmsPublicKey);
	}

	public override bool Equals(object o)
	{
		if (this == o)
		{
			return true;
		}
		if (o is HssPublicKeyParameters hssPublicKeyParameters && m_level == hssPublicKeyParameters.m_level)
		{
			return m_lmsPublicKey.Equals(hssPublicKeyParameters.m_lmsPublicKey);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int level = m_level;
		return 31 * level + m_lmsPublicKey.GetHashCode();
	}

	public override byte[] GetEncoded()
	{
		return Composer.Compose().U32Str(m_level).Bytes(m_lmsPublicKey.GetEncoded())
			.Build();
	}

	public LmsContext GenerateLmsContext(byte[] sigEnc)
	{
		HssSignature instance;
		try
		{
			instance = HssSignature.GetInstance(sigEnc, Level);
		}
		catch (IOException ex)
		{
			throw new Exception("cannot parse signature: " + ex.Message);
		}
		LmsSignedPubKey[] signedPubKeys = instance.SignedPubKeys;
		LmsPublicKeyParameters lmsPublicKeyParameters = LmsPublicKey;
		if (signedPubKeys.Length != 0)
		{
			lmsPublicKeyParameters = signedPubKeys[^1].PublicKey;
		}
		return lmsPublicKeyParameters.GenerateOtsContext(instance.Signature).WithSignedPublicKeys(signedPubKeys);
	}

	public bool Verify(LmsContext context)
	{
		LmsSignedPubKey[] signedPubKeys = context.SignedPubKeys;
		if (signedPubKeys.Length != Level - 1)
		{
			return false;
		}
		LmsPublicKeyParameters lmsPublicKeyParameters = LmsPublicKey;
		bool flag = false;
		for (int i = 0; i < signedPubKeys.Length; i++)
		{
			LmsSignature signature = signedPubKeys[i].Signature;
			LmsPublicKeyParameters publicKey = signedPubKeys[i].PublicKey;
			if (!Lms.VerifySignature(lmsPublicKeyParameters, signature, publicKey.ToByteArray()))
			{
				flag = true;
			}
			lmsPublicKeyParameters = publicKey;
		}
		return !flag & lmsPublicKeyParameters.Verify(context);
	}
}
