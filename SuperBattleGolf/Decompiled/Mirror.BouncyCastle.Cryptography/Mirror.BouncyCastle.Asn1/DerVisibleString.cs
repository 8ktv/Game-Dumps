using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerVisibleString : DerStringBase
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerVisibleString), 26)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	private readonly byte[] m_contents;

	public static DerVisibleString GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerVisibleString result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerVisibleString result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerVisibleString)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct visible string from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static DerVisibleString GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerVisibleString)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public DerVisibleString(string str)
	{
		if (str == null)
		{
			throw new ArgumentNullException("str");
		}
		m_contents = Strings.ToAsciiByteArray(str);
	}

	public DerVisibleString(byte[] contents)
		: this(contents, clone: true)
	{
	}

	internal DerVisibleString(byte[] contents, bool clone)
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
		return new PrimitiveEncoding(0, 26, m_contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 26, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, m_contents);
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is DerVisibleString derVisibleString)
		{
			return Arrays.AreEqual(m_contents, derVisibleString.m_contents);
		}
		return false;
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(m_contents);
	}

	internal static DerVisibleString CreatePrimitive(byte[] contents)
	{
		return new DerVisibleString(contents, clone: false);
	}
}
