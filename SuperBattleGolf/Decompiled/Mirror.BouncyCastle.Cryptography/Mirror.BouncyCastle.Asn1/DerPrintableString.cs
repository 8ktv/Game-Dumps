using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerPrintableString : DerStringBase
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerPrintableString), 19)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	private readonly byte[] m_contents;

	public static DerPrintableString GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerPrintableString result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerPrintableString result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerPrintableString)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct printable string from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static DerPrintableString GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerPrintableString)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public DerPrintableString(string str)
		: this(str, validate: false)
	{
	}

	public DerPrintableString(string str, bool validate)
	{
		if (str == null)
		{
			throw new ArgumentNullException("str");
		}
		if (validate && !IsPrintableString(str))
		{
			throw new ArgumentException("string contains illegal characters", "str");
		}
		m_contents = Strings.ToAsciiByteArray(str);
	}

	public DerPrintableString(byte[] contents)
		: this(contents, clone: true)
	{
	}

	internal DerPrintableString(byte[] contents, bool clone)
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
		return new PrimitiveEncoding(0, 19, m_contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 19, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, m_contents);
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is DerPrintableString derPrintableString)
		{
			return Arrays.AreEqual(m_contents, derPrintableString.m_contents);
		}
		return false;
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(m_contents);
	}

	public static bool IsPrintableString(string str)
	{
		foreach (char c in str)
		{
			if (c > '\u007f')
			{
				return false;
			}
			if (!char.IsLetterOrDigit(c))
			{
				switch (c)
				{
				case ' ':
				case '\'':
				case '(':
				case ')':
				case '+':
				case ',':
				case '-':
				case '.':
				case '/':
				case ':':
				case '=':
				case '?':
					continue;
				}
				return false;
			}
		}
		return true;
	}

	internal static DerPrintableString CreatePrimitive(byte[] contents)
	{
		return new DerPrintableString(contents, clone: false);
	}
}
