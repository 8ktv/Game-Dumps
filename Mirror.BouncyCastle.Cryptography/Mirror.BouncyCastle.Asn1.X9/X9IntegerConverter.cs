using System;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;

namespace Mirror.BouncyCastle.Asn1.X9;

public abstract class X9IntegerConverter
{
	public static int GetByteLength(ECFieldElement fe)
	{
		return fe.GetEncodedLength();
	}

	public static int GetByteLength(ECCurve c)
	{
		return c.FieldElementEncodingLength;
	}

	public static byte[] IntegerToBytes(BigInteger s, int qLength)
	{
		byte[] array = s.ToByteArrayUnsigned();
		if (qLength < array.Length)
		{
			byte[] array2 = new byte[qLength];
			Array.Copy(array, array.Length - array2.Length, array2, 0, array2.Length);
			return array2;
		}
		if (qLength > array.Length)
		{
			byte[] array3 = new byte[qLength];
			Array.Copy(array, 0, array3, array3.Length - array.Length, array.Length);
			return array3;
		}
		return array;
	}
}
