using System;
using System.IO;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerEnumerated : Asn1Object
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerEnumerated), 10)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets(), clone: false);
		}
	}

	private readonly byte[] contents;

	private readonly int start;

	private static readonly DerEnumerated[] cache = new DerEnumerated[12];

	public BigInteger Value => new BigInteger(contents);

	public int IntValueExact
	{
		get
		{
			if (contents.Length - start > 4)
			{
				throw new ArithmeticException("ASN.1 Enumerated out of int range");
			}
			return DerInteger.IntValue(contents, start, -1);
		}
	}

	public static DerEnumerated GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerEnumerated result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerEnumerated result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerEnumerated)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct enumerated from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static DerEnumerated GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerEnumerated)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public DerEnumerated(int val)
	{
		if (val < 0)
		{
			throw new ArgumentException("enumerated must be non-negative", "val");
		}
		contents = BigInteger.ValueOf(val).ToByteArray();
		start = 0;
	}

	public DerEnumerated(long val)
	{
		if (val < 0)
		{
			throw new ArgumentException("enumerated must be non-negative", "val");
		}
		contents = BigInteger.ValueOf(val).ToByteArray();
		start = 0;
	}

	public DerEnumerated(BigInteger val)
	{
		if (val.SignValue < 0)
		{
			throw new ArgumentException("enumerated must be non-negative", "val");
		}
		contents = val.ToByteArray();
		start = 0;
	}

	public DerEnumerated(byte[] contents)
		: this(contents, clone: true)
	{
	}

	internal DerEnumerated(byte[] contents, bool clone)
	{
		if (DerInteger.IsMalformed(contents))
		{
			throw new ArgumentException("malformed enumerated", "contents");
		}
		if ((contents[0] & 0x80) != 0)
		{
			throw new ArgumentException("enumerated must be non-negative", "contents");
		}
		this.contents = (clone ? Arrays.Clone(contents) : contents);
		start = DerInteger.SignBytesToSkip(this.contents);
	}

	public bool HasValue(int x)
	{
		if (contents.Length - start <= 4)
		{
			return DerInteger.IntValue(contents, start, -1) == x;
		}
		return false;
	}

	public bool HasValue(BigInteger x)
	{
		if (x != null && DerInteger.IntValue(contents, start, -1) == x.IntValue)
		{
			return Value.Equals(x);
		}
		return false;
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 10, contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 10, contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, contents);
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (!(asn1Object is DerEnumerated derEnumerated))
		{
			return false;
		}
		return Arrays.AreEqual(contents, derEnumerated.contents);
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(contents);
	}

	internal static DerEnumerated CreatePrimitive(byte[] contents, bool clone)
	{
		if (contents.Length > 1)
		{
			return new DerEnumerated(contents, clone);
		}
		if (contents.Length == 0)
		{
			throw new ArgumentException("ENUMERATED has zero length", "contents");
		}
		int num = contents[0];
		if (num >= cache.Length)
		{
			return new DerEnumerated(contents, clone);
		}
		DerEnumerated derEnumerated = cache[num];
		if (derEnumerated == null)
		{
			derEnumerated = (cache[num] = new DerEnumerated(contents, clone));
		}
		return derEnumerated;
	}
}
