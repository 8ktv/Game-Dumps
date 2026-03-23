using Mirror.BouncyCastle.Math.EC;

namespace Mirror.BouncyCastle.Asn1.X9;

public class X9FieldElement : Asn1Encodable
{
	private ECFieldElement f;

	public ECFieldElement Value => f;

	public X9FieldElement(ECFieldElement f)
	{
		this.f = f;
	}

	public override Asn1Object ToAsn1Object()
	{
		int byteLength = X9IntegerConverter.GetByteLength(f);
		return new DerOctetString(X9IntegerConverter.IntegerToBytes(f.ToBigInteger(), byteLength));
	}
}
