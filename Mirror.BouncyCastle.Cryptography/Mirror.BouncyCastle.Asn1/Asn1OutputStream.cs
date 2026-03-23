using System;
using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Asn1;

public class Asn1OutputStream : FilterStream
{
	internal const int EncodingBer = 1;

	internal const int EncodingDL = 2;

	internal const int EncodingDer = 3;

	private readonly bool m_leaveOpen;

	internal virtual int Encoding => 1;

	public static Asn1OutputStream Create(Stream output)
	{
		return new Asn1OutputStream(output, leaveOpen: false);
	}

	public static Asn1OutputStream Create(Stream output, string encoding)
	{
		return Create(output, encoding, leaveOpen: false);
	}

	public static Asn1OutputStream Create(Stream output, string encoding, bool leaveOpen)
	{
		if ("DER".Equals(encoding))
		{
			return new DerOutputStream(output, leaveOpen);
		}
		if ("DL".Equals(encoding))
		{
			return new DLOutputStream(output, leaveOpen);
		}
		return new Asn1OutputStream(output, leaveOpen);
	}

	internal static int GetEncodingType(string encoding)
	{
		if ("DER".Equals(encoding))
		{
			return 3;
		}
		if ("DL".Equals(encoding))
		{
			return 2;
		}
		return 1;
	}

	protected internal Asn1OutputStream(Stream output, bool leaveOpen)
		: base(output)
	{
		if (!output.CanWrite)
		{
			throw new ArgumentException("Expected stream to be writable", "output");
		}
		m_leaveOpen = leaveOpen;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			FlushInternal();
		}
		if (m_leaveOpen)
		{
			Detach(disposing);
		}
		else
		{
			base.Dispose(disposing);
		}
	}

	public virtual void WriteObject(Asn1Encodable asn1Encodable)
	{
		if (asn1Encodable == null)
		{
			throw new ArgumentNullException("asn1Encodable");
		}
		asn1Encodable.ToAsn1Object().GetEncoding(Encoding).Encode(this);
		FlushInternal();
	}

	public virtual void WriteObject(Asn1Object asn1Object)
	{
		if (asn1Object == null)
		{
			throw new ArgumentNullException("asn1Object");
		}
		asn1Object.GetEncoding(Encoding).Encode(this);
		FlushInternal();
	}

	internal void EncodeContents(IAsn1Encoding[] contentsEncodings)
	{
		int i = 0;
		for (int num = contentsEncodings.Length; i < num; i++)
		{
			contentsEncodings[i].Encode(this);
		}
	}

	private void FlushInternal()
	{
	}

	internal void WriteDL(int dl)
	{
		if (dl < 128)
		{
			WriteByte((byte)dl);
			return;
		}
		byte[] array = new byte[5];
		int num = array.Length;
		do
		{
			array[--num] = (byte)dl;
			dl >>= 8;
		}
		while (dl > 0);
		int num2 = array.Length - num;
		array[--num] = (byte)(0x80 | num2);
		Write(array, num, num2 + 1);
	}

	internal void WriteIdentifier(int flags, int tagNo)
	{
		if (tagNo < 31)
		{
			WriteByte((byte)(flags | tagNo));
			return;
		}
		byte[] array = new byte[6];
		int num = array.Length;
		array[--num] = (byte)(tagNo & 0x7F);
		while (tagNo > 127)
		{
			tagNo >>= 7;
			array[--num] = (byte)((tagNo & 0x7F) | 0x80);
		}
		array[--num] = (byte)(flags | 0x1F);
		Write(array, num, array.Length - num);
	}

	internal static IAsn1Encoding[] GetContentsEncodings(int encoding, Asn1Encodable[] elements)
	{
		int num = elements.Length;
		IAsn1Encoding[] array = new IAsn1Encoding[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = elements[i].ToAsn1Object().GetEncoding(encoding);
		}
		return array;
	}

	internal static DerEncoding[] GetContentsEncodingsDer(Asn1Encodable[] elements)
	{
		int num = elements.Length;
		DerEncoding[] array = new DerEncoding[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = elements[i].ToAsn1Object().GetEncodingDer();
		}
		return array;
	}

	internal static int GetLengthOfContents(IAsn1Encoding[] contentsEncodings)
	{
		int num = 0;
		int i = 0;
		for (int num2 = contentsEncodings.Length; i < num2; i++)
		{
			num += contentsEncodings[i].GetLength();
		}
		return num;
	}

	internal static int GetLengthOfDL(int dl)
	{
		if (dl < 128)
		{
			return 1;
		}
		int num = 2;
		while ((dl >>= 8) > 0)
		{
			num++;
		}
		return num;
	}

	internal static int GetLengthOfEncodingDL(int tagNo, int contentsLength)
	{
		return GetLengthOfIdentifier(tagNo) + GetLengthOfDL(contentsLength) + contentsLength;
	}

	internal static int GetLengthOfEncodingIL(int tagNo, IAsn1Encoding contentsEncoding)
	{
		return GetLengthOfIdentifier(tagNo) + 3 + contentsEncoding.GetLength();
	}

	internal static int GetLengthOfEncodingIL(int tagNo, IAsn1Encoding[] contentsEncodings)
	{
		return GetLengthOfIdentifier(tagNo) + 3 + GetLengthOfContents(contentsEncodings);
	}

	internal static int GetLengthOfIdentifier(int tagNo)
	{
		if (tagNo < 31)
		{
			return 1;
		}
		int num = 2;
		while ((tagNo >>= 7) > 0)
		{
			num++;
		}
		return num;
	}
}
