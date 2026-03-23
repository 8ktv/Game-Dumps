using System;

namespace Mirror.BouncyCastle.Asn1.X509;

public class SubjectPublicKeyInfo : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_algorithm;

	private readonly DerBitString m_publicKey;

	public AlgorithmIdentifier Algorithm => m_algorithm;

	[Obsolete("Use 'Algorithm' instead")]
	public AlgorithmIdentifier AlgorithmID => m_algorithm;

	public DerBitString PublicKey => m_publicKey;

	[Obsolete("Use 'PublicKey' instead")]
	public DerBitString PublicKeyData => m_publicKey;

	public static SubjectPublicKeyInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SubjectPublicKeyInfo result)
		{
			return result;
		}
		return new SubjectPublicKeyInfo(Asn1Sequence.GetInstance(obj));
	}

	public static SubjectPublicKeyInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new SubjectPublicKeyInfo(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public SubjectPublicKeyInfo(AlgorithmIdentifier algID, DerBitString publicKey)
	{
		m_algorithm = algID;
		m_publicKey = publicKey;
	}

	public SubjectPublicKeyInfo(AlgorithmIdentifier algID, Asn1Encodable publicKey)
	{
		m_algorithm = algID;
		m_publicKey = new DerBitString(publicKey);
	}

	public SubjectPublicKeyInfo(AlgorithmIdentifier algID, byte[] publicKey)
	{
		m_algorithm = algID;
		m_publicKey = new DerBitString(publicKey);
	}

	private SubjectPublicKeyInfo(Asn1Sequence seq)
	{
		if (seq.Count != 2)
		{
			throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
		}
		m_algorithm = AlgorithmIdentifier.GetInstance(seq[0]);
		m_publicKey = DerBitString.GetInstance(seq[1]);
	}

	public Asn1Object ParsePublicKey()
	{
		return Asn1Object.FromByteArray(m_publicKey.GetOctets());
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_algorithm, m_publicKey);
	}
}
