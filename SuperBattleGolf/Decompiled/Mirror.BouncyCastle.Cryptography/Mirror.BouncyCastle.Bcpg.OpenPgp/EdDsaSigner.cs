using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

internal sealed class EdDsaSigner : ISigner
{
	private readonly ISigner m_signer;

	private readonly IDigest m_digest;

	public string AlgorithmName => m_signer.AlgorithmName;

	internal EdDsaSigner(ISigner signer, IDigest digest)
	{
		m_signer = signer;
		m_digest = digest;
	}

	public void Init(bool forSigning, ICipherParameters cipherParameters)
	{
		m_signer.Init(forSigning, cipherParameters);
		m_digest.Reset();
	}

	public void Update(byte b)
	{
		m_digest.Update(b);
	}

	public void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		m_digest.BlockUpdate(input, inOff, inLen);
	}

	public int GetMaxSignatureSize()
	{
		return m_signer.GetMaxSignatureSize();
	}

	public byte[] GenerateSignature()
	{
		FinalizeDigest();
		return m_signer.GenerateSignature();
	}

	public bool VerifySignature(byte[] signature)
	{
		FinalizeDigest();
		return m_signer.VerifySignature(signature);
	}

	public void Reset()
	{
		m_signer.Reset();
		m_digest.Reset();
	}

	private void FinalizeDigest()
	{
		byte[] array = DigestUtilities.DoFinal(m_digest);
		m_signer.BlockUpdate(array, 0, array.Length);
	}
}
