namespace Mirror.BouncyCastle.Asn1;

internal sealed class Asn1UniversalTypes
{
	private Asn1UniversalTypes()
	{
	}

	internal static Asn1UniversalType Get(int tagNo)
	{
		return tagNo switch
		{
			1 => DerBoolean.Meta.Instance, 
			2 => DerInteger.Meta.Instance, 
			3 => DerBitString.Meta.Instance, 
			4 => Asn1OctetString.Meta.Instance, 
			5 => Asn1Null.Meta.Instance, 
			6 => DerObjectIdentifier.Meta.Instance, 
			7 => Asn1ObjectDescriptor.Meta.Instance, 
			8 => DerExternal.Meta.Instance, 
			10 => DerEnumerated.Meta.Instance, 
			12 => DerUtf8String.Meta.Instance, 
			13 => Asn1RelativeOid.Meta.Instance, 
			16 => Asn1Sequence.Meta.Instance, 
			17 => Asn1Set.Meta.Instance, 
			18 => DerNumericString.Meta.Instance, 
			19 => DerPrintableString.Meta.Instance, 
			20 => DerT61String.Meta.Instance, 
			21 => DerVideotexString.Meta.Instance, 
			22 => DerIA5String.Meta.Instance, 
			23 => Asn1UtcTime.Meta.Instance, 
			24 => Asn1GeneralizedTime.Meta.Instance, 
			25 => DerGraphicString.Meta.Instance, 
			26 => DerVisibleString.Meta.Instance, 
			27 => DerGeneralString.Meta.Instance, 
			28 => DerUniversalString.Meta.Instance, 
			30 => DerBmpString.Meta.Instance, 
			_ => null, 
		};
	}
}
