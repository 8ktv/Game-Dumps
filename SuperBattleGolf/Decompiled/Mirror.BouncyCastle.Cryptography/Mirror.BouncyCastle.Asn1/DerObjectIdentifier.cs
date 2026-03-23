using System;
using System.IO;
using System.Text;
using System.Threading;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerObjectIdentifier : Asn1Object
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerObjectIdentifier), 6)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets(), clone: false);
		}
	}

	private const int MaxContentsLength = 4096;

	private const int MaxIdentifierLength = 16385;

	private const long LongLimit = 72057594037927808L;

	private static readonly DerObjectIdentifier[] Cache = new DerObjectIdentifier[1024];

	private readonly byte[] m_contents;

	private string m_identifier;

	public string Id => GetID();

	public static DerObjectIdentifier FromContents(byte[] contents)
	{
		if (contents == null)
		{
			throw new ArgumentNullException("contents");
		}
		return CreatePrimitive(contents, clone: true);
	}

	public static DerObjectIdentifier GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerObjectIdentifier result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerObjectIdentifier result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerObjectIdentifier)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct object identifier from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static DerObjectIdentifier GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		if (!declaredExplicit && !taggedObject.IsParsed() && taggedObject.HasContextTag())
		{
			Asn1Object asn1Object = taggedObject.GetBaseObject().ToAsn1Object();
			if (!(asn1Object is DerObjectIdentifier))
			{
				return FromContents(Asn1OctetString.GetInstance(asn1Object).GetOctets());
			}
		}
		return (DerObjectIdentifier)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public static bool TryFromID(string identifier, out DerObjectIdentifier oid)
	{
		if (identifier == null)
		{
			throw new ArgumentNullException("identifier");
		}
		if (identifier.Length <= 16385 && IsValidIdentifier(identifier))
		{
			byte[] array = ParseIdentifier(identifier);
			if (array.Length <= 4096)
			{
				oid = new DerObjectIdentifier(array, identifier);
				return true;
			}
		}
		oid = null;
		return false;
	}

	public DerObjectIdentifier(string identifier)
	{
		CheckIdentifier(identifier);
		byte[] array = ParseIdentifier(identifier);
		CheckContentsLength(array.Length);
		m_contents = array;
		m_identifier = identifier;
	}

	private DerObjectIdentifier(byte[] contents, string identifier)
	{
		m_contents = contents;
		m_identifier = identifier;
	}

	public virtual DerObjectIdentifier Branch(string branchID)
	{
		Asn1RelativeOid.CheckIdentifier(branchID);
		byte[] array = Asn1RelativeOid.ParseIdentifier(branchID);
		CheckContentsLength(m_contents.Length + array.Length);
		return new DerObjectIdentifier(Arrays.Concatenate(m_contents, array), GetID() + "." + branchID);
	}

	public string GetID()
	{
		return Objects.EnsureSingletonInitialized(ref m_identifier, m_contents, ParseContents);
	}

	public virtual bool On(DerObjectIdentifier stem)
	{
		byte[] contents = m_contents;
		byte[] contents2 = stem.m_contents;
		int num = contents2.Length;
		if (contents.Length > num)
		{
			return Arrays.AreEqual(contents, 0, num, contents2, 0, num);
		}
		return false;
	}

	public override string ToString()
	{
		return GetID();
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is DerObjectIdentifier derObjectIdentifier)
		{
			return Arrays.AreEqual(m_contents, derObjectIdentifier.m_contents);
		}
		return false;
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(m_contents);
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 6, m_contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 6, m_contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, m_contents);
	}

	internal static void CheckContentsLength(int contentsLength)
	{
		if (contentsLength > 4096)
		{
			throw new ArgumentException("exceeded OID contents length limit");
		}
	}

	internal static void CheckIdentifier(string identifier)
	{
		if (identifier == null)
		{
			throw new ArgumentNullException("identifier");
		}
		if (identifier.Length > 16385)
		{
			throw new ArgumentException("exceeded OID contents length limit");
		}
		if (!IsValidIdentifier(identifier))
		{
			throw new FormatException("string " + identifier + " not a valid OID");
		}
	}

	internal static DerObjectIdentifier CreatePrimitive(byte[] contents, bool clone)
	{
		CheckContentsLength(contents.Length);
		uint hashCode = (uint)Arrays.GetHashCode(contents);
		hashCode ^= hashCode >> 20;
		hashCode ^= hashCode >> 10;
		hashCode &= 0x3FF;
		DerObjectIdentifier derObjectIdentifier = Volatile.Read(ref Cache[hashCode]);
		if (derObjectIdentifier != null && Arrays.AreEqual(contents, derObjectIdentifier.m_contents))
		{
			return derObjectIdentifier;
		}
		if (!Asn1RelativeOid.IsValidContents(contents))
		{
			throw new ArgumentException("invalid OID contents", "contents");
		}
		DerObjectIdentifier derObjectIdentifier2 = new DerObjectIdentifier(clone ? Arrays.Clone(contents) : contents, null);
		DerObjectIdentifier derObjectIdentifier3 = Interlocked.CompareExchange(ref Cache[hashCode], derObjectIdentifier2, derObjectIdentifier);
		if (derObjectIdentifier3 != derObjectIdentifier && derObjectIdentifier3 != null && Arrays.AreEqual(contents, derObjectIdentifier3.m_contents))
		{
			return derObjectIdentifier3;
		}
		return derObjectIdentifier2;
	}

	private static bool IsValidIdentifier(string identifier)
	{
		if (identifier.Length < 3 || identifier[1] != '.')
		{
			return false;
		}
		char c = identifier[0];
		if (c < '0' || c > '2')
		{
			return false;
		}
		if (!Asn1RelativeOid.IsValidIdentifier(identifier, 2))
		{
			return false;
		}
		if (c == '2')
		{
			return true;
		}
		if (identifier.Length == 3 || identifier[3] == '.')
		{
			return true;
		}
		if (identifier.Length == 4 || identifier[4] == '.')
		{
			return identifier[2] < '4';
		}
		return false;
	}

	private static string ParseContents(byte[] contents)
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
						if (num < 40)
						{
							stringBuilder.Append('0');
						}
						else if (num < 80)
						{
							stringBuilder.Append('1');
							num -= 40;
						}
						else
						{
							stringBuilder.Append('2');
							num -= 80;
						}
						flag = false;
					}
					stringBuilder.Append('.');
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
					stringBuilder.Append('2');
					bigInteger = bigInteger.Subtract(BigInteger.ValueOf(80));
					flag = false;
				}
				stringBuilder.Append('.');
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

	private static byte[] ParseIdentifier(string identifier)
	{
		MemoryStream memoryStream = new MemoryStream();
		OidTokenizer oidTokenizer = new OidTokenizer(identifier);
		string s = oidTokenizer.NextToken();
		int num = int.Parse(s) * 40;
		s = oidTokenizer.NextToken();
		if (s.Length <= 18)
		{
			Asn1RelativeOid.WriteField(memoryStream, num + long.Parse(s));
		}
		else
		{
			Asn1RelativeOid.WriteField(memoryStream, new BigInteger(s).Add(BigInteger.ValueOf(num)));
		}
		while (oidTokenizer.HasMoreTokens)
		{
			s = oidTokenizer.NextToken();
			if (s.Length <= 18)
			{
				Asn1RelativeOid.WriteField(memoryStream, long.Parse(s));
			}
			else
			{
				Asn1RelativeOid.WriteField(memoryStream, new BigInteger(s));
			}
		}
		return memoryStream.ToArray();
	}
}
