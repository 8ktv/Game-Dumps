using System;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerBitString : DerStringBase, Asn1BitStringParser, IAsn1Convertible
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerBitString), 3)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}

		internal override Asn1Object FromImplicitConstructed(Asn1Sequence sequence)
		{
			return sequence.ToAsn1BitString();
		}
	}

	private static readonly char[] table = new char[16]
	{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'A', 'B', 'C', 'D', 'E', 'F'
	};

	internal readonly byte[] contents;

	public virtual int PadBits => contents[0];

	public virtual int IntValue
	{
		get
		{
			int num = 0;
			int num2 = System.Math.Min(5, contents.Length - 1);
			for (int i = 1; i < num2; i++)
			{
				num |= contents[i] << 8 * (i - 1);
			}
			if (1 <= num2 && num2 < 5)
			{
				int num3 = contents[0];
				byte b = (byte)(contents[num2] & (255 << num3));
				num |= b << 8 * (num2 - 1);
			}
			return num;
		}
	}

	public Asn1BitStringParser Parser => this;

	public static DerBitString GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerBitString result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerBitString result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] data)
		{
			try
			{
				return GetInstance(Asn1Object.FromByteArray(data));
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct BIT STRING from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static DerBitString GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return (DerBitString)Meta.Instance.GetContextInstance(obj, isExplicit);
	}

	public DerBitString(byte data, int padBits)
	{
		if (padBits > 7 || padBits < 0)
		{
			throw new ArgumentException("pad bits cannot be greater than 7 or less than 0", "padBits");
		}
		contents = new byte[2]
		{
			(byte)padBits,
			data
		};
	}

	public DerBitString(byte[] data)
		: this(data, 0)
	{
	}

	public DerBitString(byte[] data, int padBits)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (padBits < 0 || padBits > 7)
		{
			throw new ArgumentException("must be in the range 0 to 7", "padBits");
		}
		if (data.Length == 0 && padBits != 0)
		{
			throw new ArgumentException("if 'data' is empty, 'padBits' must be 0");
		}
		contents = Arrays.Prepend(data, (byte)padBits);
	}

	public DerBitString(int namedBits)
	{
		if (namedBits == 0)
		{
			contents = new byte[1];
			return;
		}
		int num = (32 - Integers.NumberOfLeadingZeros(namedBits) + 7) / 8;
		byte[] array = new byte[1 + num];
		for (int i = 1; i < num; i++)
		{
			array[i] = (byte)namedBits;
			namedBits >>= 8;
		}
		array[num] = (byte)namedBits;
		int j;
		for (j = 0; (namedBits & (1 << j)) == 0; j++)
		{
		}
		array[0] = (byte)j;
		contents = array;
	}

	public DerBitString(Asn1Encodable obj)
		: this(obj.GetDerEncoded())
	{
	}

	internal DerBitString(byte[] contents, bool check)
	{
		if (check)
		{
			if (contents == null)
			{
				throw new ArgumentNullException("contents");
			}
			if (contents.Length < 1)
			{
				throw new ArgumentException("cannot be empty", "contents");
			}
			int num = contents[0];
			if (num > 0)
			{
				if (contents.Length < 2)
				{
					throw new ArgumentException("zero length data with non-zero pad bits", "contents");
				}
				if (num > 7)
				{
					throw new ArgumentException("pad bits cannot be greater than 7 or less than 0", "contents");
				}
			}
		}
		this.contents = contents;
	}

	public virtual byte[] GetOctets()
	{
		if (contents[0] != 0)
		{
			throw new InvalidOperationException("attempt to get non-octet aligned data from BIT STRING");
		}
		return Arrays.CopyOfRange(contents, 1, contents.Length);
	}

	public virtual byte[] GetBytes()
	{
		if (contents.Length == 1)
		{
			return Asn1OctetString.EmptyOctets;
		}
		int num = contents[0];
		byte[] array = Arrays.CopyOfRange(contents, 1, contents.Length);
		array[^1] &= (byte)(255 << num);
		return array;
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		int num = contents[0];
		if (num != 0)
		{
			int num2 = contents.Length - 1;
			byte num3 = contents[num2];
			byte b = (byte)(num3 & (255 << num));
			if (num3 != b)
			{
				return new PrimitiveEncodingSuffixed(0, 3, contents, b);
			}
		}
		return new PrimitiveEncoding(0, 3, contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		int num = contents[0];
		if (num != 0)
		{
			int num2 = contents.Length - 1;
			byte num3 = contents[num2];
			byte b = (byte)(num3 & (255 << num));
			if (num3 != b)
			{
				return new PrimitiveEncodingSuffixed(tagClass, tagNo, contents, b);
			}
		}
		return new PrimitiveEncoding(tagClass, tagNo, contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		int num = contents[0];
		if (num != 0)
		{
			int num2 = contents.Length - 1;
			byte num3 = contents[num2];
			byte b = (byte)(num3 & (255 << num));
			if (num3 != b)
			{
				return new PrimitiveDerEncodingSuffixed(0, 3, contents, b);
			}
		}
		return new PrimitiveDerEncoding(0, 3, contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		int num = contents[0];
		if (num != 0)
		{
			int num2 = contents.Length - 1;
			byte num3 = contents[num2];
			byte b = (byte)(num3 & (255 << num));
			if (num3 != b)
			{
				return new PrimitiveDerEncodingSuffixed(tagClass, tagNo, contents, b);
			}
		}
		return new PrimitiveDerEncoding(tagClass, tagNo, contents);
	}

	protected override int Asn1GetHashCode()
	{
		if (contents.Length < 2)
		{
			return 1;
		}
		int num = contents[0];
		int num2 = contents.Length - 1;
		byte b = (byte)(contents[num2] & (255 << num));
		return (Arrays.GetHashCode(contents, 0, num2) * 257) ^ b;
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (!(asn1Object is DerBitString derBitString))
		{
			return false;
		}
		byte[] array = contents;
		byte[] array2 = derBitString.contents;
		int num = array.Length;
		if (array2.Length != num)
		{
			return false;
		}
		if (num == 1)
		{
			return true;
		}
		int num2 = num - 1;
		for (int i = 0; i < num2; i++)
		{
			if (array[i] != array2[i])
			{
				return false;
			}
		}
		int num3 = array[0];
		byte num4 = (byte)(array[num2] & (255 << num3));
		byte b = (byte)(array2[num2] & (255 << num3));
		return num4 == b;
	}

	public Stream GetBitStream()
	{
		return new MemoryStream(contents, 1, contents.Length - 1, writable: false);
	}

	public Stream GetOctetStream()
	{
		int num = contents[0] & 0xFF;
		if (num != 0)
		{
			throw new IOException("expected octet-aligned bitstring, but found padBits: " + num);
		}
		return GetBitStream();
	}

	public override string GetString()
	{
		byte[] derEncoded = GetDerEncoded();
		StringBuilder stringBuilder = new StringBuilder(1 + derEncoded.Length * 2);
		stringBuilder.Append('#');
		for (int i = 0; i != derEncoded.Length; i++)
		{
			uint num = derEncoded[i];
			stringBuilder.Append(table[num >> 4]);
			stringBuilder.Append(table[num & 0xF]);
		}
		return stringBuilder.ToString();
	}

	internal static DerBitString CreatePrimitive(byte[] contents)
	{
		int num = contents.Length;
		if (num < 1)
		{
			throw new ArgumentException("truncated BIT STRING detected", "contents");
		}
		int num2 = contents[0];
		if (num2 > 0)
		{
			if (num2 > 7 || num < 2)
			{
				throw new ArgumentException("invalid pad bits detected", "contents");
			}
			byte b = contents[num - 1];
			if (b != (byte)(b & (255 << num2)))
			{
				return new DLBitString(contents, check: false);
			}
		}
		return new DerBitString(contents, check: false);
	}
}
