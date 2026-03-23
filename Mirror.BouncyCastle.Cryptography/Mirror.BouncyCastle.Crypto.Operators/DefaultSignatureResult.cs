namespace Mirror.BouncyCastle.Crypto.Operators;

public sealed class DefaultSignatureResult : IBlockResult
{
	private readonly ISigner m_signer;

	public DefaultSignatureResult(ISigner signer)
	{
		m_signer = signer;
	}

	public byte[] Collect()
	{
		return m_signer.GenerateSignature();
	}

	public int Collect(byte[] buf, int off)
	{
		byte[] array = Collect();
		array.CopyTo(buf, off);
		return array.Length;
	}

	public int GetMaxResultLength()
	{
		return m_signer.GetMaxSignatureSize();
	}
}
