using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

public sealed class KyberPrivateKeyParameters : KyberKeyParameters
{
	private readonly byte[] m_s;

	private readonly byte[] m_hpk;

	private readonly byte[] m_nonce;

	private readonly byte[] m_t;

	private readonly byte[] m_rho;

	public KyberPrivateKeyParameters(KyberParameters parameters, byte[] s, byte[] hpk, byte[] nonce, byte[] t, byte[] rho)
		: base(isPrivate: true, parameters)
	{
		m_s = Arrays.Clone(s);
		m_hpk = Arrays.Clone(hpk);
		m_nonce = Arrays.Clone(nonce);
		m_t = Arrays.Clone(t);
		m_rho = Arrays.Clone(rho);
	}

	public KyberPrivateKeyParameters(KyberParameters parameters, byte[] encoding)
		: base(isPrivate: true, parameters)
	{
		KyberEngine engine = parameters.Engine;
		int num = 0;
		m_s = Arrays.CopyOfRange(encoding, 0, engine.IndCpaSecretKeyBytes);
		num += engine.IndCpaSecretKeyBytes;
		m_t = Arrays.CopyOfRange(encoding, num, num + engine.IndCpaPublicKeyBytes - KyberEngine.SymBytes);
		num += engine.IndCpaPublicKeyBytes - KyberEngine.SymBytes;
		m_rho = Arrays.CopyOfRange(encoding, num, num + 32);
		num += 32;
		m_hpk = Arrays.CopyOfRange(encoding, num, num + 32);
		num += 32;
		m_nonce = Arrays.CopyOfRange(encoding, num, num + KyberEngine.SymBytes);
	}

	public byte[] GetEncoded()
	{
		return Arrays.ConcatenateAll(m_s, m_t, m_rho, m_hpk, m_nonce);
	}

	public byte[] GetHpk()
	{
		return Arrays.Clone(m_hpk);
	}

	public byte[] GetNonce()
	{
		return Arrays.Clone(m_nonce);
	}

	public byte[] GetPublicKey()
	{
		return KyberPublicKeyParameters.GetEncoded(m_t, m_rho);
	}

	public KyberPublicKeyParameters GetPublicKeyParameters()
	{
		return new KyberPublicKeyParameters(base.Parameters, m_t, m_rho);
	}

	public byte[] GetRho()
	{
		return Arrays.Clone(m_rho);
	}

	public byte[] GetS()
	{
		return Arrays.Clone(m_s);
	}

	public byte[] GetT()
	{
		return Arrays.Clone(m_t);
	}
}
