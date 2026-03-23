using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerIA5String : DerStringBase
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerIA5String), 22)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	private readonly byte[] m_contents;

	public static DerIA5String GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerIA5String result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerIA5String result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerIA5String)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct IA5 string from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static DerIA5String GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerIA5String)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public DerIA5String(string str)
		: this(str, validate: false)
	{
	}

	public DerIA5String(string str, bool validate)
	{
		if (str == null)
		{
			throw new ArgumentNullException("str");
		}
		if (validate && !IsIA5String(str))
		{
			throw new ArgumentException("string contains illegal characters", "str");
		}
		m_contents = Strings.ToAsciiByteArray(str);
	}

	public DerIA5String(byte[] contents)
		: this(contents, clone: true)
	{
	}

	internal DerIA5String(byte[] contents, bool clone)
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
		return new PrimitiveEncoding(0, 22, m_contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 22, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, m_contents);
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is DerIA5String derIA5String)
		{
			return Arrays.AreEqual(m_contents, derIA5String.m_contents);
		}
		return false;
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(m_contents);
	}

	public static bool IsIA5String(string str)
	{
		for (int i = 0; i < str.Length; i++)
		{
			if (str[i] > '\u007f')
			{
				return false;
			}
		}
		return true;
	}

	internal static DerIA5String CreatePrimitive(byte[] contents)
	{
		return new DerIA5String(contents, clone: false);
	}
}
