namespace Mirror.BouncyCastle.Asn1;

public interface Asn1TaggedObjectParser : IAsn1Convertible
{
	int TagClass { get; }

	int TagNo { get; }

	bool HasContextTag(int tagNo);

	bool HasTag(int tagClass, int tagNo);

	IAsn1Convertible ParseBaseUniversal(bool declaredExplicit, int baseTagNo);

	IAsn1Convertible ParseExplicitBaseObject();

	Asn1TaggedObjectParser ParseExplicitBaseTagged();

	Asn1TaggedObjectParser ParseImplicitBaseTagged(int baseTagClass, int baseTagNo);
}
