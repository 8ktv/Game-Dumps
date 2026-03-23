using Mirror.BouncyCastle.Asn1;

namespace Mirror.BouncyCastle.Crmf;

public interface IControl
{
	DerObjectIdentifier Type { get; }

	Asn1Encodable Value { get; }
}
