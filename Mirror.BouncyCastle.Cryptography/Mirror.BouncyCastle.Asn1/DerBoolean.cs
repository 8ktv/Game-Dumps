using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerBoolean : Asn1Object
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerBoolean), 1)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	public static readonly DerBoolean False = new DerBoolean(value: false);

	public static readonly DerBoolean True = new DerBoolean(value: true);

	private readonly byte value;

	public bool IsTrue => value != 0;

	public static DerBoolean GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerBoolean result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerBoolean result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerBoolean)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct boolean from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static DerBoolean GetInstance(bool value)
	{
		if (!value)
		{
			return False;
		}
		return True;
	}

	public static DerBoolean GetInstance(int value)
	{
		if (value == 0)
		{
			return False;
		}
		return True;
	}

	public static DerBoolean GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerBoolean)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public DerBoolean(byte[] val)
	{
		if (val.Length != 1)
		{
			throw new ArgumentException("byte value should have 1 byte in it", "val");
		}
		value = val[0];
	}

	private DerBoolean(bool value)
	{
		this.value = (byte)(value ? byte.MaxValue : 0);
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 1, GetContents(encoding));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, GetContents(encoding));
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 1, GetContents(3));
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, GetContents(3));
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (!(asn1Object is DerBoolean derBoolean))
		{
			return false;
		}
		return IsTrue == derBoolean.IsTrue;
	}

	protected override int Asn1GetHashCode()
	{
		return IsTrue.GetHashCode();
	}

	public override string ToString()
	{
		if (!IsTrue)
		{
			return "FALSE";
		}
		return "TRUE";
	}

	internal static DerBoolean CreatePrimitive(byte[] contents)
	{
		if (contents.Length != 1)
		{
			throw new ArgumentException("BOOLEAN value should have 1 byte in it", "contents");
		}
		return contents[0] switch
		{
			byte.MaxValue => True, 
			0 => False, 
			_ => new DerBoolean(contents), 
		};
	}

	private byte[] GetContents(int encoding)
	{
		byte b = value;
		if (3 == encoding && IsTrue)
		{
			b = byte.MaxValue;
		}
		return new byte[1] { b };
	}
}
