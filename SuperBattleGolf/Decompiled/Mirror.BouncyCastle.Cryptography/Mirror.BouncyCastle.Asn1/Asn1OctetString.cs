using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Asn1;

public abstract class Asn1OctetString : Asn1Object, Asn1OctetStringParser, IAsn1Convertible
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(Asn1OctetString), 4)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return octetString;
		}

		internal override Asn1Object FromImplicitConstructed(Asn1Sequence sequence)
		{
			return sequence.ToAsn1OctetString();
		}
	}

	internal static readonly byte[] EmptyOctets = new byte[0];

	internal readonly byte[] contents;

	public Asn1OctetStringParser Parser => this;

	public static Asn1OctetString GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Asn1OctetString result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is Asn1OctetString result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (Asn1OctetString)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct OCTET STRING from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static Asn1OctetString GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (Asn1OctetString)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	internal Asn1OctetString(byte[] contents)
	{
		if (contents == null)
		{
			throw new ArgumentNullException("contents");
		}
		this.contents = contents;
	}

	public Stream GetOctetStream()
	{
		return new MemoryStream(contents, writable: false);
	}

	public virtual byte[] GetOctets()
	{
		return contents;
	}

	public virtual int GetOctetsLength()
	{
		return GetOctets().Length;
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(GetOctets());
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is Asn1OctetString asn1OctetString)
		{
			return Arrays.AreEqual(GetOctets(), asn1OctetString.GetOctets());
		}
		return false;
	}

	public override string ToString()
	{
		return "#" + Hex.ToHexString(contents);
	}

	internal static Asn1OctetString CreatePrimitive(byte[] contents)
	{
		return new DerOctetString(contents);
	}
}
