using System;
using System.IO;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerInteger : Asn1Object
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerInteger), 2)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	public const string AllowUnsafeProperty = "Mirror.BouncyCastle.Asn1.AllowUnsafeInteger";

	internal const int SignExtSigned = -1;

	internal const int SignExtUnsigned = 255;

	private readonly byte[] bytes;

	private readonly int start;

	public BigInteger PositiveValue => new BigInteger(1, bytes);

	public BigInteger Value => new BigInteger(bytes);

	public int IntPositiveValueExact
	{
		get
		{
			int num = bytes.Length - start;
			if (num > 4 || (num == 4 && (bytes[start] & 0x80) != 0))
			{
				throw new ArithmeticException("ASN.1 Integer out of positive int range");
			}
			return IntValue(bytes, start, 255);
		}
	}

	public int IntValueExact
	{
		get
		{
			if (bytes.Length - start > 4)
			{
				throw new ArithmeticException("ASN.1 Integer out of int range");
			}
			return IntValue(bytes, start, -1);
		}
	}

	public long LongValueExact
	{
		get
		{
			if (bytes.Length - start > 8)
			{
				throw new ArithmeticException("ASN.1 Integer out of long range");
			}
			return LongValue(bytes, start, -1);
		}
	}

	internal static bool AllowUnsafe()
	{
		string environmentVariable = Platform.GetEnvironmentVariable("Mirror.BouncyCastle.Asn1.AllowUnsafeInteger");
		if (environmentVariable != null)
		{
			return Platform.EqualsIgnoreCase("true", environmentVariable);
		}
		return false;
	}

	public static DerInteger GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerInteger result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerInteger result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] array)
		{
			try
			{
				return (DerInteger)Meta.Instance.FromByteArray(array);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct integer from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static DerInteger GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerInteger)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public DerInteger(int value)
	{
		bytes = BigInteger.ValueOf(value).ToByteArray();
		start = 0;
	}

	public DerInteger(long value)
	{
		bytes = BigInteger.ValueOf(value).ToByteArray();
		start = 0;
	}

	public DerInteger(BigInteger value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		bytes = value.ToByteArray();
		start = 0;
	}

	public DerInteger(byte[] bytes)
		: this(bytes, clone: true)
	{
	}

	internal DerInteger(byte[] bytes, bool clone)
	{
		if (IsMalformed(bytes))
		{
			throw new ArgumentException("malformed integer", "bytes");
		}
		this.bytes = (clone ? Arrays.Clone(bytes) : bytes);
		start = SignBytesToSkip(bytes);
	}

	public bool HasValue(int x)
	{
		if (bytes.Length - start <= 4)
		{
			return IntValue(bytes, start, -1) == x;
		}
		return false;
	}

	public bool HasValue(long x)
	{
		if (bytes.Length - start <= 8)
		{
			return LongValue(bytes, start, -1) == x;
		}
		return false;
	}

	public bool HasValue(BigInteger x)
	{
		if (x != null && IntValue(bytes, start, -1) == x.IntValue)
		{
			return Value.Equals(x);
		}
		return false;
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 2, bytes);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, bytes);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 2, bytes);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, bytes);
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(bytes);
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (!(asn1Object is DerInteger derInteger))
		{
			return false;
		}
		return Arrays.AreEqual(bytes, derInteger.bytes);
	}

	public override string ToString()
	{
		return Value.ToString();
	}

	internal static DerInteger CreatePrimitive(byte[] contents)
	{
		return new DerInteger(contents, clone: false);
	}

	internal static int GetEncodingLength(BigInteger x)
	{
		return Asn1OutputStream.GetLengthOfEncodingDL(2, BigIntegers.GetByteLength(x));
	}

	internal static int IntValue(byte[] bytes, int start, int signExt)
	{
		int num = bytes.Length;
		int num2 = System.Math.Max(start, num - 4);
		int num3 = (sbyte)bytes[num2] & signExt;
		while (++num2 < num)
		{
			num3 = (num3 << 8) | bytes[num2];
		}
		return num3;
	}

	internal static long LongValue(byte[] bytes, int start, int signExt)
	{
		int num = bytes.Length;
		int num2 = System.Math.Max(start, num - 8);
		long num3 = (sbyte)bytes[num2] & signExt;
		while (++num2 < num)
		{
			num3 = (num3 << 8) | bytes[num2];
		}
		return num3;
	}

	internal static bool IsMalformed(byte[] bytes)
	{
		switch (bytes.Length)
		{
		case 0:
			return true;
		case 1:
			return false;
		default:
			if ((sbyte)bytes[0] == (sbyte)bytes[1] >> 7)
			{
				return !AllowUnsafe();
			}
			return false;
		}
	}

	internal static int SignBytesToSkip(byte[] bytes)
	{
		int i = 0;
		for (int num = bytes.Length - 1; i < num && (sbyte)bytes[i] == (sbyte)bytes[i + 1] >> 7; i++)
		{
		}
		return i;
	}
}
