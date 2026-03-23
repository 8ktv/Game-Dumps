using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerBmpString : DerStringBase
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerBmpString), 30)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	private readonly string m_str;

	public static DerBmpString GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerBmpString result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerBmpString result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerBmpString)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct BMP string from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
	}

	public static DerBmpString GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerBmpString)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	internal DerBmpString(byte[] contents)
	{
		if (contents == null)
		{
			throw new ArgumentNullException("contents");
		}
		int num = contents.Length;
		if ((num & 1) != 0)
		{
			throw new ArgumentException("malformed BMPString encoding encountered", "contents");
		}
		int num2 = num / 2;
		char[] array = new char[num2];
		for (int i = 0; i != num2; i++)
		{
			array[i] = (char)((contents[2 * i] << 8) | (contents[2 * i + 1] & 0xFF));
		}
		m_str = new string(array);
	}

	internal DerBmpString(char[] str)
	{
		if (str == null)
		{
			throw new ArgumentNullException("str");
		}
		m_str = new string(str);
	}

	public DerBmpString(string str)
	{
		if (str == null)
		{
			throw new ArgumentNullException("str");
		}
		m_str = str;
	}

	public override string GetString()
	{
		return m_str;
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is DerBmpString derBmpString)
		{
			return m_str.Equals(derBmpString.m_str);
		}
		return false;
	}

	protected override int Asn1GetHashCode()
	{
		return m_str.GetHashCode();
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 30, GetContents());
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, GetContents());
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 30, GetContents());
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, GetContents());
	}

	private byte[] GetContents()
	{
		char[] array = m_str.ToCharArray();
		byte[] array2 = new byte[array.Length * 2];
		for (int i = 0; i != array.Length; i++)
		{
			array2[2 * i] = (byte)((int)array[i] >> 8);
			array2[2 * i + 1] = (byte)array[i];
		}
		return array2;
	}

	internal static DerBmpString CreatePrimitive(byte[] contents)
	{
		return new DerBmpString(contents);
	}

	internal static DerBmpString CreatePrimitive(char[] str)
	{
		return new DerBmpString(str);
	}
}
