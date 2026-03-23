using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.X509;

public class PolicyMappings : Asn1Encodable
{
	private readonly Asn1Sequence seq;

	public PolicyMappings(Asn1Sequence seq)
	{
		this.seq = seq;
	}

	public PolicyMappings(IDictionary<string, string> mappings)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(mappings.Count);
		foreach (KeyValuePair<string, string> mapping in mappings)
		{
			string key = mapping.Key;
			string value = mapping.Value;
			asn1EncodableVector.Add(new DerSequence(new DerObjectIdentifier(key), new DerObjectIdentifier(value)));
		}
		seq = new DerSequence(asn1EncodableVector);
	}

	public override Asn1Object ToAsn1Object()
	{
		return seq;
	}
}
