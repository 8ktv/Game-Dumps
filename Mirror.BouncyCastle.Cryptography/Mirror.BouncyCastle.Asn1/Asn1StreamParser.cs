using System;
using System.IO;

namespace Mirror.BouncyCastle.Asn1;

public class Asn1StreamParser
{
	private readonly Stream _in;

	private readonly int _limit;

	private readonly byte[][] tmpBuffers;

	public Asn1StreamParser(Stream input)
		: this(input, Asn1InputStream.FindLimit(input))
	{
	}

	public Asn1StreamParser(byte[] encoding)
		: this(new MemoryStream(encoding, writable: false), encoding.Length)
	{
	}

	public Asn1StreamParser(Stream input, int limit)
		: this(input, limit, new byte[16][])
	{
	}

	internal Asn1StreamParser(Stream input, int limit, byte[][] tmpBuffers)
	{
		if (!input.CanRead)
		{
			throw new ArgumentException("Expected stream to be readable", "input");
		}
		_in = input;
		_limit = limit;
		this.tmpBuffers = tmpBuffers;
	}

	public virtual IAsn1Convertible ReadObject()
	{
		int num = _in.ReadByte();
		if (num < 0)
		{
			return null;
		}
		return ImplParseObject(num);
	}

	internal IAsn1Convertible ImplParseObject(int tagHdr)
	{
		Set00Check(enabled: false);
		int num = Asn1InputStream.ReadTagNumber(_in, tagHdr);
		int num2 = Asn1InputStream.ReadLength(_in, _limit, num == 3 || num == 4 || num == 16 || num == 17 || num == 8);
		if (num2 < 0)
		{
			if ((tagHdr & 0x20) == 0)
			{
				throw new IOException("indefinite-length primitive encoding encountered");
			}
			Asn1StreamParser asn1StreamParser = new Asn1StreamParser(new IndefiniteLengthInputStream(_in, _limit), _limit, tmpBuffers);
			int num3 = tagHdr & 0xC0;
			if (num3 != 0)
			{
				return new BerTaggedObjectParser(num3, num, asn1StreamParser);
			}
			return asn1StreamParser.ParseImplicitConstructedIL(num);
		}
		DefiniteLengthInputStream definiteLengthInputStream = new DefiniteLengthInputStream(_in, num2, _limit);
		if ((tagHdr & 0xE0) == 0)
		{
			return ParseImplicitPrimitive(num, definiteLengthInputStream);
		}
		Asn1StreamParser asn1StreamParser2 = new Asn1StreamParser(definiteLengthInputStream, definiteLengthInputStream.Remaining, tmpBuffers);
		int num4 = tagHdr & 0xC0;
		if (num4 != 0)
		{
			bool constructed = (tagHdr & 0x20) != 0;
			return new DLTaggedObjectParser(num4, num, constructed, asn1StreamParser2);
		}
		return asn1StreamParser2.ParseImplicitConstructedDL(num);
	}

	internal Asn1Object LoadTaggedDL(int tagClass, int tagNo, bool constructed)
	{
		if (!constructed)
		{
			byte[] contentsOctets = ((DefiniteLengthInputStream)_in).ToArray();
			return Asn1TaggedObject.CreatePrimitive(tagClass, tagNo, contentsOctets);
		}
		Asn1EncodableVector contentsElements = ReadVector();
		return Asn1TaggedObject.CreateConstructedDL(tagClass, tagNo, contentsElements);
	}

	internal Asn1Object LoadTaggedIL(int tagClass, int tagNo)
	{
		Asn1EncodableVector contentsElements = ReadVector();
		return Asn1TaggedObject.CreateConstructedIL(tagClass, tagNo, contentsElements);
	}

	internal IAsn1Convertible ParseImplicitConstructedDL(int univTagNo)
	{
		return univTagNo switch
		{
			3 => new BerBitStringParser(this), 
			8 => new DerExternalParser(this), 
			4 => new BerOctetStringParser(this), 
			17 => new DerSetParser(this), 
			16 => new DerSequenceParser(this), 
			_ => throw new Asn1Exception("unknown DL object encountered: 0x" + univTagNo.ToString("X")), 
		};
	}

	internal IAsn1Convertible ParseImplicitConstructedIL(int univTagNo)
	{
		return univTagNo switch
		{
			3 => new BerBitStringParser(this), 
			8 => new DerExternalParser(this), 
			4 => new BerOctetStringParser(this), 
			16 => new BerSequenceParser(this), 
			17 => new BerSetParser(this), 
			_ => throw new Asn1Exception("unknown BER object encountered: 0x" + univTagNo.ToString("X")), 
		};
	}

	internal IAsn1Convertible ParseImplicitPrimitive(int univTagNo)
	{
		return ParseImplicitPrimitive(univTagNo, (DefiniteLengthInputStream)_in);
	}

	internal IAsn1Convertible ParseImplicitPrimitive(int univTagNo, DefiniteLengthInputStream defIn)
	{
		switch (univTagNo)
		{
		case 3:
			return new DLBitStringParser(defIn);
		case 8:
			throw new Asn1Exception("externals must use constructed encoding (see X.690 8.18)");
		case 4:
			return new DerOctetStringParser(defIn);
		case 17:
			throw new Asn1Exception("sequences must use constructed encoding (see X.690 8.9.1/8.10.1)");
		case 16:
			throw new Asn1Exception("sets must use constructed encoding (see X.690 8.11.1/8.12.1)");
		default:
			try
			{
				return Asn1InputStream.CreatePrimitiveDerObject(univTagNo, defIn, tmpBuffers);
			}
			catch (ArgumentException innerException)
			{
				throw new Asn1Exception("corrupted stream detected", innerException);
			}
		}
	}

	internal IAsn1Convertible ParseObject(int univTagNo)
	{
		if (univTagNo < 0 || univTagNo > 30)
		{
			throw new ArgumentException("invalid universal tag number: " + univTagNo, "univTagNo");
		}
		int num = _in.ReadByte();
		if (num < 0)
		{
			return null;
		}
		if ((num & -33) != univTagNo)
		{
			throw new IOException("unexpected identifier encountered: " + num);
		}
		return ImplParseObject(num);
	}

	internal Asn1TaggedObjectParser ParseTaggedObject()
	{
		int num = _in.ReadByte();
		if (num < 0)
		{
			return null;
		}
		if ((num & 0xC0) == 0)
		{
			throw new Asn1Exception("no tagged object found");
		}
		return (Asn1TaggedObjectParser)ImplParseObject(num);
	}

	internal Asn1EncodableVector ReadVector()
	{
		int num = _in.ReadByte();
		if (num < 0)
		{
			return new Asn1EncodableVector(0);
		}
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		do
		{
			IAsn1Convertible asn1Convertible = ImplParseObject(num);
			asn1EncodableVector.Add(asn1Convertible.ToAsn1Object());
		}
		while ((num = _in.ReadByte()) >= 0);
		return asn1EncodableVector;
	}

	private void Set00Check(bool enabled)
	{
		if (_in is IndefiniteLengthInputStream indefiniteLengthInputStream)
		{
			indefiniteLengthInputStream.SetEofOn00(enabled);
		}
	}
}
