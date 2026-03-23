using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerVideotexString : DerStringBase
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerVideotexString), 21)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	private readonly byte[] m_contents;

	public static DerVideotexString GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerVideotexString result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerVideotexString result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerVideotexString)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct videotex string from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static DerVideotexString GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerVideotexString)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public DerVideotexString(byte[] contents)
		: this(contents, clone: true)
	{
	}

	internal DerVideotexString(byte[] contents, bool clone)
	{
		if (contents == null)
		{
			throw new ArgumentNullException("contents");
		}
		m_contents = (clone ? Arrays.Clone(contents) : contents);
	}

	public override string GetString()
	{
		return Strings.FromByteArray(m_contents);
	}

	public byte[] GetOctets()
	{
		return Arrays.Clone(m_contents);
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 21, m_contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 21, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, m_contents);
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is DerVideotexString derVideotexString)
		{
			return Arrays.AreEqual(m_contents, derVideotexString.m_contents);
		}
		return false;
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(m_contents);
	}

	internal static DerVideotexString CreatePrimitive(byte[] contents)
	{
		return new DerVideotexString(contents, clone: false);
	}
}
