using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class Challenge : Asn1Encodable
{
	public class Rand : Asn1Encodable
	{
		private readonly DerInteger m_intVal;

		private readonly GeneralName m_sender;

		public virtual DerInteger IntVal => m_intVal;

		public virtual GeneralName Sender => m_sender;

		public static Rand GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is Rand result)
			{
				return result;
			}
			return new Rand(Asn1Sequence.GetInstance(obj));
		}

		public static Rand GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
		{
			return new Rand(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
		}

		public Rand(DerInteger intVal, GeneralName sender)
		{
			m_intVal = intVal;
			m_sender = sender;
		}

		public Rand(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("expected sequence size of 2", "seq");
			}
			m_intVal = DerInteger.GetInstance(seq[0]);
			m_sender = GeneralName.GetInstance(seq[1]);
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(m_intVal, m_sender);
		}
	}

	private readonly AlgorithmIdentifier m_owf;

	private readonly Asn1OctetString m_witness;

	private readonly Asn1OctetString m_challenge;

	public virtual AlgorithmIdentifier Owf => m_owf;

	public virtual Asn1OctetString Witness => m_witness;

	public virtual Asn1OctetString ChallengeValue => m_challenge;

	public static Challenge GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Challenge result)
		{
			return result;
		}
		return new Challenge(Asn1Sequence.GetInstance(obj));
	}

	public static Challenge GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new Challenge(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private Challenge(Asn1Sequence seq)
	{
		int index = 0;
		if (seq.Count == 3)
		{
			m_owf = AlgorithmIdentifier.GetInstance(seq[index++]);
		}
		m_witness = Asn1OctetString.GetInstance(seq[index++]);
		m_challenge = Asn1OctetString.GetInstance(seq[index]);
	}

	public Challenge(byte[] witness, byte[] challenge)
		: this(null, witness, challenge)
	{
	}

	public Challenge(AlgorithmIdentifier owf, byte[] witness, byte[] challenge)
	{
		m_owf = owf;
		m_witness = new DerOctetString(witness);
		m_challenge = new DerOctetString(challenge);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.AddOptional(m_owf);
		asn1EncodableVector.Add(m_witness, m_challenge);
		return new DerSequence(asn1EncodableVector);
	}
}
