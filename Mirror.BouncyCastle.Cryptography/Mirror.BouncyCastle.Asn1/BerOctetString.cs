using System;

namespace Mirror.BouncyCastle.Asn1;

public class BerOctetString : DerOctetString
{
	private readonly Asn1OctetString[] elements;

	public static BerOctetString FromSequence(Asn1Sequence seq)
	{
		return new BerOctetString(seq.MapElements(Asn1OctetString.GetInstance));
	}

	internal static byte[] FlattenOctetStrings(Asn1OctetString[] octetStrings)
	{
		int num = octetStrings.Length;
		switch (num)
		{
		case 0:
			return Asn1OctetString.EmptyOctets;
		case 1:
			return octetStrings[0].contents;
		default:
		{
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				num2 += octetStrings[i].contents.Length;
			}
			byte[] array = new byte[num2];
			int num3 = 0;
			for (int j = 0; j < num; j++)
			{
				byte[] array2 = octetStrings[j].contents;
				Array.Copy(array2, 0, array, num3, array2.Length);
				num3 += array2.Length;
			}
			return array;
		}
		}
	}

	public BerOctetString(byte[] contents)
		: this(contents, null)
	{
	}

	public BerOctetString(Asn1OctetString[] elements)
		: this(FlattenOctetStrings(elements), elements)
	{
	}

	[Obsolete("Use version without segmentLimit (which is ignored anyway)")]
	public BerOctetString(byte[] contents, int segmentLimit)
		: this(contents)
	{
	}

	[Obsolete("Use version without segmentLimit (which is ignored anyway)")]
	public BerOctetString(Asn1OctetString[] elements, int segmentLimit)
		: this(elements)
	{
	}

	private BerOctetString(byte[] contents, Asn1OctetString[] elements)
		: base(contents)
	{
		this.elements = elements;
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		if (1 != encoding)
		{
			return base.GetEncoding(encoding);
		}
		if (elements == null)
		{
			return new PrimitiveEncoding(0, 4, contents);
		}
		Asn1Encodable[] array = elements;
		return new ConstructedILEncoding(0, 4, Asn1OutputStream.GetContentsEncodings(encoding, array));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		if (1 != encoding)
		{
			return base.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		if (elements == null)
		{
			return new PrimitiveEncoding(tagClass, tagNo, contents);
		}
		Asn1Encodable[] array = elements;
		return new ConstructedILEncoding(tagClass, tagNo, Asn1OutputStream.GetContentsEncodings(encoding, array));
	}
}
