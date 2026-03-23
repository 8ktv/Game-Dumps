using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerNumericString : DerStringBase
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerNumericString), 18)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	private readonly byte[] m_contents;

	public static DerNumericString GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerNumericString result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerNumericString result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerNumericString)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct numeric string from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static DerNumericString GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerNumericString)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public DerNumericString(string str)
		: this(str, validate: false)
	{
	}

	public DerNumericString(string str, bool validate)
	{
		if (str == null)
		{
			throw new ArgumentNullException("str");
		}
		if (validate && !IsNumericString(str))
		{
			throw new ArgumentException("string contains illegal characters", "str");
		}
		m_contents = Strings.ToAsciiByteArray(str);
	}

	public DerNumericString(byte[] contents)
		: this(contents, clone: true)
	{
	}

	internal DerNumericString(byte[] contents, bool clone)
	{
		if (contents == null)
		{
			throw new ArgumentNullException("contents");
		}
		m_contents = (clone ? Arrays.Clone(contents) : contents);
	}

	public override string GetString()
	{
		return Strings.FromAsciiByteArray(m_contents);
	}

	public byte[] GetOctets()
	{
		return Arrays.Clone(m_contents);
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 18, m_contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 18, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, m_contents);
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is DerNumericString derNumericString)
		{
			return Arrays.AreEqual(m_contents, derNumericString.m_contents);
		}
		return false;
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(m_contents);
	}

	public static bool IsNumericString(string str)
	{
		foreach (char c in str)
		{
			if (c > '\u007f' || (c != ' ' && !char.IsDigit(c)))
			{
				return false;
			}
		}
		return true;
	}

	internal static bool IsNumericString(byte[] contents)
	{
		foreach (byte b in contents)
		{
			if (b != 32 && (uint)(b - 48) > 9u)
			{
				return false;
			}
		}
		return true;
	}

	internal static DerNumericString CreatePrimitive(byte[] contents)
	{
		return new DerNumericString(contents, clone: false);
	}
}
