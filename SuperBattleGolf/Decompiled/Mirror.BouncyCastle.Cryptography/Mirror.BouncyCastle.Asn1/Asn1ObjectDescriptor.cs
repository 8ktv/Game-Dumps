using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public sealed class Asn1ObjectDescriptor : Asn1Object
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(Asn1ObjectDescriptor), 7)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return new Asn1ObjectDescriptor((DerGraphicString)DerGraphicString.Meta.Instance.FromImplicitPrimitive(octetString));
		}

		internal override Asn1Object FromImplicitConstructed(Asn1Sequence sequence)
		{
			return new Asn1ObjectDescriptor((DerGraphicString)DerGraphicString.Meta.Instance.FromImplicitConstructed(sequence));
		}
	}

	private readonly DerGraphicString m_baseGraphicString;

	public DerGraphicString BaseGraphicString => m_baseGraphicString;

	public static Asn1ObjectDescriptor GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Asn1ObjectDescriptor result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is Asn1ObjectDescriptor result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (Asn1ObjectDescriptor)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct object descriptor from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static Asn1ObjectDescriptor GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (Asn1ObjectDescriptor)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public Asn1ObjectDescriptor(DerGraphicString baseGraphicString)
	{
		if (baseGraphicString == null)
		{
			throw new ArgumentNullException("baseGraphicString");
		}
		m_baseGraphicString = baseGraphicString;
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return m_baseGraphicString.GetEncodingImplicit(encoding, 0, 7);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return m_baseGraphicString.GetEncodingImplicit(encoding, tagClass, tagNo);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return m_baseGraphicString.GetEncodingDerImplicit(0, 7);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return m_baseGraphicString.GetEncodingDerImplicit(tagClass, tagNo);
	}

	protected override int Asn1GetHashCode()
	{
		return ~m_baseGraphicString.CallAsn1GetHashCode();
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is Asn1ObjectDescriptor asn1ObjectDescriptor)
		{
			return m_baseGraphicString.Equals(asn1ObjectDescriptor.m_baseGraphicString);
		}
		return false;
	}

	internal static Asn1ObjectDescriptor CreatePrimitive(byte[] contents)
	{
		return new Asn1ObjectDescriptor(DerGraphicString.CreatePrimitive(contents));
	}
}
