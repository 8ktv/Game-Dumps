using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Asn1;

public class Asn1InputStream : FilterStream
{
	private readonly int limit;

	private readonly bool m_leaveOpen;

	internal byte[][] tmpBuffers;

	internal static int FindLimit(Stream input)
	{
		if (input is LimitedInputStream limitedInputStream)
		{
			return limitedInputStream.Limit;
		}
		if (input is Asn1InputStream asn1InputStream)
		{
			return asn1InputStream.limit;
		}
		if (input is MemoryStream memoryStream)
		{
			return Convert.ToInt32(memoryStream.Length - memoryStream.Position);
		}
		return int.MaxValue;
	}

	public Asn1InputStream(byte[] input)
		: this(new MemoryStream(input, writable: false), input.Length)
	{
	}

	public Asn1InputStream(Stream input)
		: this(input, FindLimit(input))
	{
	}

	public Asn1InputStream(Stream input, int limit)
		: this(input, limit, leaveOpen: false)
	{
	}

	public Asn1InputStream(Stream input, int limit, bool leaveOpen)
		: this(input, limit, leaveOpen, new byte[16][])
	{
	}

	internal Asn1InputStream(Stream input, int limit, bool leaveOpen, byte[][] tmpBuffers)
		: base(input)
	{
		if (!input.CanRead)
		{
			throw new ArgumentException("Expected stream to be readable", "input");
		}
		this.limit = limit;
		m_leaveOpen = leaveOpen;
		this.tmpBuffers = tmpBuffers;
	}

	protected override void Dispose(bool disposing)
	{
		tmpBuffers = null;
		if (m_leaveOpen)
		{
			Detach(disposing);
		}
		else
		{
			base.Dispose(disposing);
		}
	}

	private Asn1Object BuildObject(int tagHdr, int tagNo, int length)
	{
		DefiniteLengthInputStream defIn = new DefiniteLengthInputStream(s, length, limit);
		if ((tagHdr & 0xE0) == 0)
		{
			return CreatePrimitiveDerObject(tagNo, defIn, tmpBuffers);
		}
		int num = tagHdr & 0xC0;
		if (num != 0)
		{
			bool constructed = (tagHdr & 0x20) != 0;
			return ReadTaggedObjectDL(num, tagNo, constructed, defIn);
		}
		return tagNo switch
		{
			3 => BuildConstructedBitString(ReadVector(defIn)), 
			4 => BuildConstructedOctetString(ReadVector(defIn)), 
			16 => DLSequence.FromVector(ReadVector(defIn)), 
			17 => DLSet.FromVector(ReadVector(defIn)), 
			8 => DLSequence.FromVector(ReadVector(defIn)).ToAsn1External(), 
			_ => throw new IOException("unknown tag " + tagNo + " encountered"), 
		};
	}

	internal Asn1Object ReadTaggedObjectDL(int tagClass, int tagNo, bool constructed, DefiniteLengthInputStream defIn)
	{
		if (!constructed)
		{
			byte[] contentsOctets = defIn.ToArray();
			return Asn1TaggedObject.CreatePrimitive(tagClass, tagNo, contentsOctets);
		}
		Asn1EncodableVector contentsElements = ReadVector(defIn);
		return Asn1TaggedObject.CreateConstructedDL(tagClass, tagNo, contentsElements);
	}

	private Asn1EncodableVector ReadVector()
	{
		Asn1Object asn1Object = ReadObject();
		if (asn1Object == null)
		{
			return new Asn1EncodableVector(0);
		}
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		do
		{
			asn1EncodableVector.Add(asn1Object);
		}
		while ((asn1Object = ReadObject()) != null);
		return asn1EncodableVector;
	}

	private Asn1EncodableVector ReadVector(DefiniteLengthInputStream defIn)
	{
		int remaining = defIn.Remaining;
		if (remaining < 1)
		{
			return new Asn1EncodableVector(0);
		}
		using Asn1InputStream asn1InputStream = new Asn1InputStream(defIn, remaining, leaveOpen: true, tmpBuffers);
		return asn1InputStream.ReadVector();
	}

	public Asn1Object ReadObject()
	{
		int num = s.ReadByte();
		if (num <= 0)
		{
			if (num == 0)
			{
				throw new IOException("unexpected end-of-contents marker");
			}
			return null;
		}
		int num2 = ReadTagNumber(s, num);
		int num3 = ReadLength(s, limit, isParsing: false);
		if (num3 >= 0)
		{
			try
			{
				return BuildObject(num, num2, num3);
			}
			catch (ArgumentException innerException)
			{
				throw new Asn1Exception("corrupted stream detected", innerException);
			}
		}
		if ((num & 0x20) == 0)
		{
			throw new IOException("indefinite-length primitive encoding encountered");
		}
		Asn1StreamParser asn1StreamParser = new Asn1StreamParser(new IndefiniteLengthInputStream(s, limit), limit, tmpBuffers);
		int num4 = num & 0xC0;
		if (num4 != 0)
		{
			return asn1StreamParser.LoadTaggedIL(num4, num2);
		}
		return num2 switch
		{
			3 => BerBitStringParser.Parse(asn1StreamParser), 
			4 => BerOctetStringParser.Parse(asn1StreamParser), 
			16 => BerSequenceParser.Parse(asn1StreamParser), 
			17 => BerSetParser.Parse(asn1StreamParser), 
			8 => DerExternalParser.Parse(asn1StreamParser), 
			_ => throw new IOException("unknown BER object encountered"), 
		};
	}

	private DerBitString BuildConstructedBitString(Asn1EncodableVector contentsElements)
	{
		DerBitString[] array = new DerBitString[contentsElements.Count];
		for (int i = 0; i != array.Length; i++)
		{
			if (!(contentsElements[i] is DerBitString derBitString))
			{
				throw new Asn1Exception("unknown object encountered in constructed BIT STRING: " + Platform.GetTypeName(contentsElements[i]));
			}
			array[i] = derBitString;
		}
		return new DLBitString(BerBitString.FlattenBitStrings(array), check: false);
	}

	private Asn1OctetString BuildConstructedOctetString(Asn1EncodableVector contentsElements)
	{
		Asn1OctetString[] array = new Asn1OctetString[contentsElements.Count];
		for (int i = 0; i != array.Length; i++)
		{
			if (!(contentsElements[i] is Asn1OctetString asn1OctetString))
			{
				throw new Asn1Exception("unknown object encountered in constructed OCTET STRING: " + Platform.GetTypeName(contentsElements[i]));
			}
			array[i] = asn1OctetString;
		}
		return new DerOctetString(BerOctetString.FlattenOctetStrings(array));
	}

	internal static int ReadTagNumber(Stream s, int tagHdr)
	{
		int num = tagHdr & 0x1F;
		if (num == 31)
		{
			int num2 = s.ReadByte();
			if (num2 < 31)
			{
				if (num2 < 0)
				{
					throw new EndOfStreamException("EOF found inside tag value.");
				}
				throw new IOException("corrupted stream - high tag number < 31 found");
			}
			num = num2 & 0x7F;
			if (num == 0)
			{
				throw new IOException("corrupted stream - invalid high tag number found");
			}
			while ((num2 & 0x80) != 0)
			{
				if ((uint)num >> 24 != 0)
				{
					throw new IOException("Tag number more than 31 bits");
				}
				num <<= 7;
				num2 = s.ReadByte();
				if (num2 < 0)
				{
					throw new EndOfStreamException("EOF found inside tag value.");
				}
				num |= num2 & 0x7F;
			}
		}
		return num;
	}

	internal static int ReadLength(Stream s, int limit, bool isParsing)
	{
		int num = s.ReadByte();
		if (num >>> 7 == 0)
		{
			return num;
		}
		if (128 == num)
		{
			return -1;
		}
		if (num < 0)
		{
			throw new EndOfStreamException("EOF found when length expected");
		}
		if (255 == num)
		{
			throw new IOException("invalid long form definite-length 0xFF");
		}
		int num2 = num & 0x7F;
		int num3 = 0;
		num = 0;
		do
		{
			int num4 = s.ReadByte();
			if (num4 < 0)
			{
				throw new EndOfStreamException("EOF found reading length");
			}
			if ((uint)num >> 23 != 0)
			{
				throw new IOException("long form definite-length more than 31 bits");
			}
			num = (num << 8) + num4;
		}
		while (++num3 < num2);
		if (num >= limit && !isParsing)
		{
			throw new IOException("corrupted stream - out of bounds length found: " + num + " >= " + limit);
		}
		return num;
	}

	private static bool GetBuffer(DefiniteLengthInputStream defIn, byte[][] tmpBuffers, out byte[] contents)
	{
		int remaining = defIn.Remaining;
		if (remaining >= tmpBuffers.Length)
		{
			contents = defIn.ToArray();
			return false;
		}
		byte[] array = tmpBuffers[remaining];
		if (array == null)
		{
			array = (tmpBuffers[remaining] = new byte[remaining]);
		}
		defIn.ReadAllIntoByteArray(array);
		contents = array;
		return true;
	}

	internal static Asn1Object CreatePrimitiveDerObject(int tagNo, DefiniteLengthInputStream defIn, byte[][] tmpBuffers)
	{
		switch (tagNo)
		{
		case 30:
			return CreateDerBmpString(defIn);
		case 1:
		{
			GetBuffer(defIn, tmpBuffers, out var contents5);
			return DerBoolean.CreatePrimitive(contents5);
		}
		case 10:
		{
			byte[] contents4;
			bool buffer3 = GetBuffer(defIn, tmpBuffers, out contents4);
			return DerEnumerated.CreatePrimitive(contents4, buffer3);
		}
		case 6:
		{
			DerObjectIdentifier.CheckContentsLength(defIn.Remaining);
			byte[] contents3;
			bool buffer2 = GetBuffer(defIn, tmpBuffers, out contents3);
			return DerObjectIdentifier.CreatePrimitive(contents3, buffer2);
		}
		case 13:
		{
			Asn1RelativeOid.CheckContentsLength(defIn.Remaining);
			byte[] contents2;
			bool buffer = GetBuffer(defIn, tmpBuffers, out contents2);
			return Asn1RelativeOid.CreatePrimitive(contents2, buffer);
		}
		default:
		{
			byte[] contents = defIn.ToArray();
			switch (tagNo)
			{
			case 3:
				return DerBitString.CreatePrimitive(contents);
			case 24:
				return Asn1GeneralizedTime.CreatePrimitive(contents);
			case 27:
				return DerGeneralString.CreatePrimitive(contents);
			case 25:
				return DerGraphicString.CreatePrimitive(contents);
			case 22:
				return DerIA5String.CreatePrimitive(contents);
			case 2:
				return DerInteger.CreatePrimitive(contents);
			case 5:
				return Asn1Null.CreatePrimitive(contents);
			case 18:
				return DerNumericString.CreatePrimitive(contents);
			case 7:
				return Asn1ObjectDescriptor.CreatePrimitive(contents);
			case 4:
				return Asn1OctetString.CreatePrimitive(contents);
			case 19:
				return DerPrintableString.CreatePrimitive(contents);
			case 20:
				return DerT61String.CreatePrimitive(contents);
			case 28:
				return DerUniversalString.CreatePrimitive(contents);
			case 23:
				return Asn1UtcTime.CreatePrimitive(contents);
			case 12:
				return DerUtf8String.CreatePrimitive(contents);
			case 21:
				return DerVideotexString.CreatePrimitive(contents);
			case 26:
				return DerVisibleString.CreatePrimitive(contents);
			case 9:
			case 11:
			case 14:
			case 29:
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
				throw new IOException("unsupported tag " + tagNo + " encountered");
			default:
				throw new IOException("unknown tag " + tagNo + " encountered");
			}
		}
		}
	}

	private static DerBmpString CreateDerBmpString(DefiniteLengthInputStream defIn)
	{
		int num = defIn.Remaining;
		if ((num & 1) != 0)
		{
			throw new IOException("malformed BMPString encoding encountered");
		}
		char[] array = new char[num / 2];
		int num2 = 0;
		byte[] array2 = new byte[8];
		while (num >= 8)
		{
			if (Streams.ReadFully(defIn, array2, 0, 8) != 8)
			{
				throw new EndOfStreamException("EOF encountered in middle of BMPString");
			}
			array[num2] = (char)((array2[0] << 8) | (array2[1] & 0xFF));
			array[num2 + 1] = (char)((array2[2] << 8) | (array2[3] & 0xFF));
			array[num2 + 2] = (char)((array2[4] << 8) | (array2[5] & 0xFF));
			array[num2 + 3] = (char)((array2[6] << 8) | (array2[7] & 0xFF));
			num2 += 4;
			num -= 8;
		}
		if (num > 0)
		{
			if (Streams.ReadFully(defIn, array2, 0, num) != num)
			{
				throw new EndOfStreamException("EOF encountered in middle of BMPString");
			}
			int num3 = 0;
			do
			{
				int num4 = array2[num3++] << 8;
				int num5 = array2[num3++] & 0xFF;
				array[num2++] = (char)(num4 | num5);
			}
			while (num3 < num);
		}
		if (defIn.Remaining != 0 || array.Length != num2)
		{
			throw new InvalidOperationException();
		}
		return DerBmpString.CreatePrimitive(array);
	}
}
