using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerT61String : DerStringBase
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerT61String), 20)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	private readonly byte[] m_contents;

	public static DerT61String GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerT61String result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerT61String result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerT61String)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct T61 string from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static DerT61String GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerT61String)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public DerT61String(string str)
	{
		if (str == null)
		{
			throw new ArgumentNullException("str");
		}
		m_contents = Strings.ToByteArray(str);
	}

	public DerT61String(byte[] contents)
		: this(contents, clone: true)
	{
	}

	internal DerT61String(byte[] contents, bool clone)
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

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 20, m_contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 20, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, m_contents);
	}

	public byte[] GetOctets()
	{
		return Arrays.Clone(m_contents);
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is DerT61String derT61String)
		{
			return Arrays.AreEqual(m_contents, derT61String.m_contents);
		}
		return false;
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(m_contents);
	}

	internal static DerT61String CreatePrimitive(byte[] contents)
	{
		return new DerT61String(contents, clone: false);
	}
}
