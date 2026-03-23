using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public abstract class Asn1Null : Asn1Object
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(Asn1Null), 5)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	public static Asn1Null GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Asn1Null result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is Asn1Null result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (Asn1Null)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct NULL from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static Asn1Null GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (Asn1Null)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	internal Asn1Null()
	{
	}

	public override string ToString()
	{
		return "NULL";
	}

	internal static Asn1Null CreatePrimitive(byte[] contents)
	{
		if (contents.Length != 0)
		{
			throw new InvalidOperationException("malformed NULL encoding encountered");
		}
		return DerNull.Instance;
	}
}
