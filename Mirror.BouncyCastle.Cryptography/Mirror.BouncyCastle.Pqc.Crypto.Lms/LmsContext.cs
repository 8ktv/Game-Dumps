using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class LmsContext : IDigest
{
	private readonly byte[] m_c;

	private readonly LMOtsPrivateKey m_privateKey;

	private readonly LMSigParameters m_sigParams;

	private readonly byte[][] m_path;

	private readonly LMOtsPublicKey m_publicKey;

	private readonly object m_signature;

	private LmsSignedPubKey[] m_signedPubKeys;

	private volatile IDigest m_digest;

	public byte[] C => m_c;

	internal byte[][] Path => m_path;

	internal LMOtsPrivateKey PrivateKey => m_privateKey;

	public LMOtsPublicKey PublicKey => m_publicKey;

	internal LMSigParameters SigParams => m_sigParams;

	public object Signature => m_signature;

	internal LmsSignedPubKey[] SignedPubKeys => m_signedPubKeys;

	public string AlgorithmName => m_digest.AlgorithmName;

	public LmsContext(LMOtsPrivateKey privateKey, LMSigParameters sigParams, IDigest digest, byte[] C, byte[][] path)
	{
		m_privateKey = privateKey;
		m_sigParams = sigParams;
		m_digest = digest;
		m_c = C;
		m_path = path;
		m_publicKey = null;
		m_signature = null;
	}

	public LmsContext(LMOtsPublicKey publicKey, object signature, IDigest digest)
	{
		m_publicKey = publicKey;
		m_signature = signature;
		m_digest = digest;
		m_c = null;
		m_privateKey = null;
		m_sigParams = null;
		m_path = null;
	}

	public byte[] GetQ()
	{
		byte[] array = new byte[LMOts.MAX_HASH + 2];
		m_digest.DoFinal(array, 0);
		m_digest = null;
		return array;
	}

	internal LmsContext WithSignedPublicKeys(LmsSignedPubKey[] signedPubKeys)
	{
		m_signedPubKeys = signedPubKeys;
		return this;
	}

	public int GetDigestSize()
	{
		return m_digest.GetDigestSize();
	}

	public int GetByteLength()
	{
		return m_digest.GetByteLength();
	}

	public void Update(byte input)
	{
		m_digest.Update(input);
	}

	public void BlockUpdate(byte[] input, int inOff, int len)
	{
		m_digest.BlockUpdate(input, inOff, len);
	}

	public int DoFinal(byte[] output, int outOff)
	{
		return m_digest.DoFinal(output, outOff);
	}

	public void Reset()
	{
		m_digest.Reset();
	}
}
