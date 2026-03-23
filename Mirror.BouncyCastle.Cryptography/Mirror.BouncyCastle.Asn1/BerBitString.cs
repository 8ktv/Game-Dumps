using System;

namespace Mirror.BouncyCastle.Asn1;

public class BerBitString : DLBitString
{
	private readonly DerBitString[] elements;

	public static BerBitString FromSequence(Asn1Sequence seq)
	{
		return new BerBitString(seq.MapElements(DerBitString.GetInstance));
	}

	internal static byte[] FlattenBitStrings(DerBitString[] bitStrings)
	{
		int num = bitStrings.Length;
		switch (num)
		{
		case 0:
			return new byte[1];
		case 1:
			return bitStrings[0].contents;
		default:
		{
			int num2 = num - 1;
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				byte[] array = bitStrings[i].contents;
				if (array[0] != 0)
				{
					throw new ArgumentException("only the last nested bitstring can have padding", "bitStrings");
				}
				num3 += array.Length - 1;
			}
			byte[] array2 = bitStrings[num2].contents;
			byte b = array2[0];
			num3 += array2.Length;
			byte[] array3 = new byte[num3];
			array3[0] = b;
			int num4 = 1;
			for (int j = 0; j < num; j++)
			{
				byte[] array4 = bitStrings[j].contents;
				int num5 = array4.Length - 1;
				Array.Copy(array4, 1, array3, num4, num5);
				num4 += num5;
			}
			return array3;
		}
		}
	}

	public BerBitString(byte data, int padBits)
		: base(data, padBits)
	{
		elements = null;
	}

	public BerBitString(byte[] data)
		: this(data, 0)
	{
	}

	public BerBitString(byte[] data, int padBits)
		: base(data, padBits)
	{
		elements = null;
	}

	[Obsolete("Use version without segmentLimit (which is ignored anyway)")]
	public BerBitString(byte[] data, int padBits, int segmentLimit)
		: this(data, padBits)
	{
	}

	public BerBitString(int namedBits)
		: base(namedBits)
	{
		elements = null;
	}

	public BerBitString(Asn1Encodable obj)
		: this(obj.GetDerEncoded(), 0)
	{
	}

	public BerBitString(DerBitString[] elements)
		: base(FlattenBitStrings(elements), check: false)
	{
		this.elements = elements;
	}

	[Obsolete("Use version without segmentLimit (which is ignored anyway)")]
	public BerBitString(DerBitString[] elements, int segmentLimit)
		: this(elements)
	{
	}

	internal BerBitString(byte[] contents, bool check)
		: base(contents, check)
	{
		elements = null;
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		if (1 != encoding)
		{
			return base.GetEncoding(encoding);
		}
		if (elements == null)
		{
			return new PrimitiveEncoding(0, 3, contents);
		}
		Asn1Encodable[] array = elements;
		return new ConstructedILEncoding(0, 3, Asn1OutputStream.GetContentsEncodings(encoding, array));
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
