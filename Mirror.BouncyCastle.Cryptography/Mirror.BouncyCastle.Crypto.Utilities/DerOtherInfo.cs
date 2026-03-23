using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Crypto.Utilities;

public sealed class DerOtherInfo
{
	public sealed class Builder
	{
		private readonly AlgorithmIdentifier m_algorithmID;

		private readonly Asn1OctetString m_partyUInfo;

		private readonly Asn1OctetString m_partyVInfo;

		private Asn1TaggedObject m_suppPubInfo;

		private Asn1TaggedObject m_suppPrivInfo;

		public Builder(AlgorithmIdentifier algorithmID, byte[] partyUInfo, byte[] partyVInfo)
		{
			m_algorithmID = algorithmID;
			m_partyUInfo = DerUtilities.GetOctetString(partyUInfo);
			m_partyVInfo = DerUtilities.GetOctetString(partyVInfo);
		}

		public Builder WithSuppPubInfo(byte[] suppPubInfo)
		{
			m_suppPubInfo = new DerTaggedObject(isExplicit: false, 0, DerUtilities.GetOctetString(suppPubInfo));
			return this;
		}

		public Builder WithSuppPrivInfo(byte[] suppPrivInfo)
		{
			m_suppPrivInfo = new DerTaggedObject(isExplicit: false, 1, DerUtilities.GetOctetString(suppPrivInfo));
			return this;
		}

		public DerOtherInfo Build()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(5);
			asn1EncodableVector.Add(m_algorithmID);
			asn1EncodableVector.Add(m_partyUInfo);
			asn1EncodableVector.Add(m_partyVInfo);
			asn1EncodableVector.AddOptional(m_suppPubInfo);
			asn1EncodableVector.AddOptional(m_suppPrivInfo);
			return new DerOtherInfo(new DerSequence(asn1EncodableVector));
		}
	}

	private readonly DerSequence m_sequence;

	private DerOtherInfo(DerSequence sequence)
	{
		m_sequence = sequence;
	}

	public byte[] GetEncoded()
	{
		return m_sequence.GetEncoded();
	}
}
