using System;
using System.IO;
using System.Text;
using System.Threading;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class Asn1RelativeOid : Asn1Object
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(Asn1RelativeOid), 13)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets(), clone: false);
		}
	}

	private const int MaxContentsLength = 4096;

	private const int MaxIdentifierLength = 16383;

	private const long LongLimit = 72057594037927808L;

	private static readonly Asn1RelativeOid[] Cache = new Asn1RelativeOid[64];

	private readonly byte[] m_contents;

	private string m_identifier;

	[Obsolete("Use 'GetID' instead")]
	public string Id => GetID();

	public static Asn1RelativeOid FromContents(byte[] contents)
	{
		if (contents == null)
		{
			throw new ArgumentNullException("contents");
		}
		return CreatePrimitive(contents, clone: true);
	}

	public static Asn1RelativeOid GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Asn1RelativeOid result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is Asn1RelativeOid result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] data)
		{
			try
			{
				return (Asn1RelativeOid)Asn1Object.FromByteArray(data);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct relative OID from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static Asn1RelativeOid GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (Asn1RelativeOid)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public static bool TryFromID(string identifier, out Asn1RelativeOid oid)
	{
		if (identifier == null)
		{
			throw new ArgumentNullException("identifier");
		}
		if (identifier.Length <= 16383 && IsValidIdentifier(identifier, 0))
		{
			byte[] array = ParseIdentifier(identifier);
			if (array.Length <= 4096)
			{
				oid = new Asn1RelativeOid(array, identifier);
				return true;
			}
		}
		oid = null;
		return false;
	}

	public Asn1RelativeOid(string identifier)
	{
		CheckIdentifier(identifier);
		byte[] array = ParseIdentifier(identifier);
		CheckContentsLength(array.Length);
		m_contents = array;
		m_identifier = identifier;
	}

	private Asn1RelativeOid(byte[] contents, string identifier)
	{
		m_contents = contents;
		m_identifier = identifier;
	}

	public virtual Asn1RelativeOid Branch(string branchID)
	{
		CheckIdentifier(branchID);
		byte[] array = ParseIdentifier(branchID);
		CheckContentsLength(m_contents.Length + array.Length);
		return new Asn1RelativeOid(Arrays.Concatenate(m_contents, array), GetID() + "." + branchID);
	}

	public string GetID()
	{
		return Objects.EnsureSingletonInitialized(ref m_identifier, m_contents, ParseContents);
	}

	public override string ToString()
	{
		return GetID();
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is Asn1RelativeOid asn1RelativeOid)
		{
			return Arrays.AreEqual(m_contents, asn1RelativeOid.m_contents);
		}
		return false;
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(m_contents);
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 13, m_contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 13, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, m_contents);
	}

	internal static void CheckContentsLength(int contentsLength)
	{
		if (contentsLength > 4096)
		{
			throw new ArgumentException("exceeded relative OID contents length limit");
		}
	}

	internal static void CheckIdentifier(string identifier)
	{
		if (identifier == null)
		{
			throw new ArgumentNullException("identifier");
		}
		if (identifier.Length > 16383)
		{
			throw new ArgumentException("exceeded relative OID contents length limit");
		}
		if (!IsValidIdentifier(identifier, 0))
		{
			throw new FormatException("string " + identifier + " not a valid relative OID");
		}
	}

	internal static Asn1RelativeOid CreatePrimitive(byte[] contents, bool clone)
	{
		CheckContentsLength(contents.Length);
		uint hashCode = (uint)Arrays.GetHashCode(contents);
		hashCode ^= hashCode >> 24;
		hashCode ^= hashCode >> 12;
		hashCode ^= hashCode >> 6;
		hashCode &= 0x3F;
		Asn1RelativeOid asn1RelativeOid = Volatile.Read(ref Cache[hashCode]);
		if (asn1RelativeOid != null && Arrays.AreEqual(contents, asn1RelativeOid.m_contents))
		{
			return asn1RelativeOid;
		}
		if (!IsValidContents(contents))
		{
			throw new ArgumentException("invalid relative OID contents", "contents");
		}
		Asn1RelativeOid asn1RelativeOid2 = new Asn1RelativeOid(clone ? Arrays.Clone(contents) : contents, null);
		Asn1RelativeOid asn1RelativeOid3 = Interlocked.CompareExchange(ref Cache[hashCode], asn1RelativeOid2, asn1RelativeOid);
		if (asn1RelativeOid3 != asn1RelativeOid && asn1RelativeOid3 != null && Arrays.AreEqual(contents, asn1RelativeOid3.m_contents))
		{
			return asn1RelativeOid3;
		}
		return asn1RelativeOid2;
	}

	internal static bool IsValidContents(byte[] contents)
	{
		if (contents.Length < 1)
		{
			return false;
		}
		bool flag = true;
		for (int i = 0; i < contents.Length; i++)
		{
			if (flag && contents[i] == 128)
			{
				return false;
			}
			flag = (contents[i] & 0x80) == 0;
		}
		return flag;
	}

	internal static bool IsValidIdentifier(string identifier, int from)
	{
		int num = 0;
		int num2 = identifier.Length;
		while (--num2 >= from)
		{
			char c = identifier[num2];
			if (c == '.')
			{
				if (num == 0 || (num > 1 && identifier[num2 + 1] == '0'))
				{
					return false;
				}
				num = 0;
			}
			else
			{
				if ('0' > c || c > '9')
				{
					return false;
				}
				num++;
			}
		}
		if (num == 0 || (num > 1 && identifier[num2 + 1] == '0'))
		{
			return false;
		}
		return true;
	}

	internal static string ParseContents(byte[] contents)
	{
		StringBuilder stringBuilder = new StringBuilder();
		long num = 0L;
		BigInteger bigInteger = null;
		bool flag = true;
		for (int i = 0; i != contents.Length; i++)
		{
			int num2 = contents[i];
			if (num <= 72057594037927808L)
			{
				num += num2 & 0x7F;
				if ((num2 & 0x80) == 0)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append('.');
					}
					stringBuilder.Append(num);
					num = 0L;
				}
				else
				{
					num <<= 7;
				}
				continue;
			}
			if (bigInteger == null)
			{
				bigInteger = BigInteger.ValueOf(num);
			}
			bigInteger = bigInteger.Or(BigInteger.ValueOf(num2 & 0x7F));
			if ((num2 & 0x80) == 0)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append('.');
				}
				stringBuilder.Append(bigInteger);
				bigInteger = null;
				num = 0L;
			}
			else
			{
				bigInteger = bigInteger.ShiftLeft(7);
			}
		}
		return stringBuilder.ToString();
	}

	internal static byte[] ParseIdentifier(string identifier)
	{
		MemoryStream memoryStream = new MemoryStream();
		OidTokenizer oidTokenizer = new OidTokenizer(identifier);
		while (oidTokenizer.HasMoreTokens)
		{
			string text = oidTokenizer.NextToken();
			if (text.Length <= 18)
			{
				WriteField(memoryStream, long.Parse(text));
			}
			else
			{
				WriteField(memoryStream, new BigInteger(text));
			}
		}
		return memoryStream.ToArray();
	}

	internal static void WriteField(Stream outputStream, long fieldValue)
	{
		byte[] array = new byte[9];
		int num = 8;
		array[num] = (byte)((int)fieldValue & 0x7F);
		while (fieldValue >= 128)
		{
			fieldValue >>= 7;
			array[--num] = (byte)((int)fieldValue | 0x80);
		}
		outputStream.Write(array, num, 9 - num);
	}

	internal static void WriteField(Stream outputStream, BigInteger fieldValue)
	{
		int num = (fieldValue.BitLength + 6) / 7;
		if (num == 0)
		{
			outputStream.WriteByte(0);
			return;
		}
		BigInteger bigInteger = fieldValue;
		byte[] array = new byte[num];
		for (int num2 = num - 1; num2 >= 0; num2--)
		{
			array[num2] = (byte)(bigInteger.IntValue | 0x80);
			bigInteger = bigInteger.ShiftRight(7);
		}
		array[num - 1] &= 127;
		outputStream.Write(array, 0, array.Length);
	}
}
