using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

public sealed class DilithiumPrivateKeyParameters : DilithiumKeyParameters
{
	internal byte[] m_rho;

	internal byte[] m_k;

	internal byte[] m_tr;

	internal byte[] m_s1;

	internal byte[] m_s2;

	internal byte[] m_t0;

	private byte[] m_t1;

	public byte[] K => Arrays.Clone(m_k);

	public byte[] Rho => Arrays.Clone(m_rho);

	public byte[] S1 => Arrays.Clone(m_s1);

	public byte[] S2 => Arrays.Clone(m_s2);

	public byte[] T0 => Arrays.Clone(m_t0);

	public byte[] T1 => Arrays.Clone(m_t1);

	public byte[] Tr => Arrays.Clone(m_tr);

	public DilithiumPrivateKeyParameters(DilithiumParameters parameters, byte[] rho, byte[] K, byte[] tr, byte[] s1, byte[] s2, byte[] t0, byte[] t1)
		: base(isPrivate: true, parameters)
	{
		m_rho = Arrays.Clone(rho);
		m_k = Arrays.Clone(K);
		m_tr = Arrays.Clone(tr);
		m_s1 = Arrays.Clone(s1);
		m_s2 = Arrays.Clone(s2);
		m_t0 = Arrays.Clone(t0);
		m_t1 = Arrays.Clone(t1);
	}

	public DilithiumPrivateKeyParameters(DilithiumParameters parameters, byte[] encoding, DilithiumPublicKeyParameters pubKey)
		: base(isPrivate: true, parameters)
	{
		DilithiumEngine engine = parameters.GetEngine(null);
		int num = 0;
		m_rho = Arrays.CopyOfRange(encoding, 0, 32);
		num += 32;
		m_k = Arrays.CopyOfRange(encoding, num, num + 32);
		num += 32;
		m_tr = Arrays.CopyOfRange(encoding, num, num + 64);
		num += 64;
		int num2 = engine.L * engine.PolyEtaPackedBytes;
		m_s1 = Arrays.CopyOfRange(encoding, num, num + num2);
		num += num2;
		num2 = engine.K * engine.PolyEtaPackedBytes;
		m_s2 = Arrays.CopyOfRange(encoding, num, num + num2);
		num += num2;
		num2 = engine.K * 416;
		m_t0 = Arrays.CopyOfRange(encoding, num, num + num2);
		if (pubKey != null)
		{
			m_t1 = Arrays.Clone(pubKey.GetT1());
		}
	}

	public byte[] GetEncoded()
	{
		return Arrays.ConcatenateAll(m_rho, m_k, m_tr, m_s1, m_s2, m_t0);
	}

	public byte[] GetPublicKey()
	{
		return DilithiumPublicKeyParameters.GetEncoded(m_rho, m_t1);
	}

	public DilithiumPublicKeyParameters GetPublicKeyParameters()
	{
		return new DilithiumPublicKeyParameters(base.Parameters, m_rho, m_t1);
	}
}
