using System;
using System.Globalization;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class Asn1GeneralizedTime : Asn1Object
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(Asn1GeneralizedTime), 24)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	private readonly string m_timeString;

	private readonly bool m_timeStringCanonical;

	private readonly DateTime m_dateTime;

	public string TimeString => m_timeString;

	public static Asn1GeneralizedTime GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Asn1GeneralizedTime result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is Asn1GeneralizedTime result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (Asn1GeneralizedTime)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct generalized time from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static Asn1GeneralizedTime GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (Asn1GeneralizedTime)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public Asn1GeneralizedTime(string timeString)
	{
		m_timeString = timeString ?? throw new ArgumentNullException("timeString");
		m_timeStringCanonical = false;
		try
		{
			m_dateTime = FromString(timeString);
		}
		catch (FormatException ex)
		{
			throw new ArgumentException("invalid date string: " + ex.Message);
		}
	}

	public Asn1GeneralizedTime(DateTime dateTime)
	{
		dateTime = dateTime.ToUniversalTime();
		m_dateTime = dateTime;
		m_timeString = ToStringCanonical(dateTime);
		m_timeStringCanonical = true;
	}

	internal Asn1GeneralizedTime(byte[] contents)
		: this(Encoding.ASCII.GetString(contents))
	{
	}

	public DateTime ToDateTime()
	{
		return m_dateTime;
	}

	internal byte[] GetContents(int encoding)
	{
		if (encoding == 3 && !m_timeStringCanonical)
		{
			return Encoding.ASCII.GetBytes(ToStringCanonical(m_dateTime));
		}
		return Encoding.ASCII.GetBytes(m_timeString);
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 24, GetContents(encoding));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, GetContents(encoding));
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 24, GetContents(3));
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, GetContents(3));
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (!(asn1Object is Asn1GeneralizedTime asn1GeneralizedTime))
		{
			return false;
		}
		return Arrays.AreEqual(GetContents(3), asn1GeneralizedTime.GetContents(3));
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(GetContents(3));
	}

	internal static Asn1GeneralizedTime CreatePrimitive(byte[] contents)
	{
		return new Asn1GeneralizedTime(contents);
	}

	private static DateTime FromString(string s)
	{
		if (s.Length < 10)
		{
			throw new FormatException();
		}
		s = s.Replace(',', '.');
		if (Platform.EndsWith(s, "Z"))
		{
			return s.Length switch
			{
				11 => ParseUtc(s, "yyyyMMddHH\\Z"), 
				13 => ParseUtc(s, "yyyyMMddHHmm\\Z"), 
				15 => ParseUtc(s, "yyyyMMddHHmmss\\Z"), 
				17 => ParseUtc(s, "yyyyMMddHHmmss.f\\Z"), 
				18 => ParseUtc(s, "yyyyMMddHHmmss.ff\\Z"), 
				19 => ParseUtc(s, "yyyyMMddHHmmss.fff\\Z"), 
				20 => ParseUtc(s, "yyyyMMddHHmmss.ffff\\Z"), 
				21 => ParseUtc(s, "yyyyMMddHHmmss.fffff\\Z"), 
				22 => ParseUtc(s, "yyyyMMddHHmmss.ffffff\\Z"), 
				23 => ParseUtc(s, "yyyyMMddHHmmss.fffffff\\Z"), 
				_ => throw new FormatException(), 
			};
		}
		int num = IndexOfSign(s, System.Math.Max(10, s.Length - 5));
		if (num < 0)
		{
			return s.Length switch
			{
				10 => ParseLocal(s, "yyyyMMddHH"), 
				12 => ParseLocal(s, "yyyyMMddHHmm"), 
				14 => ParseLocal(s, "yyyyMMddHHmmss"), 
				16 => ParseLocal(s, "yyyyMMddHHmmss.f"), 
				17 => ParseLocal(s, "yyyyMMddHHmmss.ff"), 
				18 => ParseLocal(s, "yyyyMMddHHmmss.fff"), 
				19 => ParseLocal(s, "yyyyMMddHHmmss.ffff"), 
				20 => ParseLocal(s, "yyyyMMddHHmmss.fffff"), 
				21 => ParseLocal(s, "yyyyMMddHHmmss.ffffff"), 
				22 => ParseLocal(s, "yyyyMMddHHmmss.fffffff"), 
				_ => throw new FormatException(), 
			};
		}
		if (num == s.Length - 5)
		{
			return s.Length switch
			{
				15 => ParseTimeZone(s, "yyyyMMddHHzzz"), 
				17 => ParseTimeZone(s, "yyyyMMddHHmmzzz"), 
				19 => ParseTimeZone(s, "yyyyMMddHHmmsszzz"), 
				21 => ParseTimeZone(s, "yyyyMMddHHmmss.fzzz"), 
				22 => ParseTimeZone(s, "yyyyMMddHHmmss.ffzzz"), 
				23 => ParseTimeZone(s, "yyyyMMddHHmmss.fffzzz"), 
				24 => ParseTimeZone(s, "yyyyMMddHHmmss.ffffzzz"), 
				25 => ParseTimeZone(s, "yyyyMMddHHmmss.fffffzzz"), 
				26 => ParseTimeZone(s, "yyyyMMddHHmmss.ffffffzzz"), 
				27 => ParseTimeZone(s, "yyyyMMddHHmmss.fffffffzzz"), 
				_ => throw new FormatException(), 
			};
		}
		if (num == s.Length - 3)
		{
			return s.Length switch
			{
				13 => ParseTimeZone(s, "yyyyMMddHHzz"), 
				15 => ParseTimeZone(s, "yyyyMMddHHmmzz"), 
				17 => ParseTimeZone(s, "yyyyMMddHHmmsszz"), 
				19 => ParseTimeZone(s, "yyyyMMddHHmmss.fzz"), 
				20 => ParseTimeZone(s, "yyyyMMddHHmmss.ffzz"), 
				21 => ParseTimeZone(s, "yyyyMMddHHmmss.fffzz"), 
				22 => ParseTimeZone(s, "yyyyMMddHHmmss.ffffzz"), 
				23 => ParseTimeZone(s, "yyyyMMddHHmmss.fffffzz"), 
				24 => ParseTimeZone(s, "yyyyMMddHHmmss.ffffffzz"), 
				25 => ParseTimeZone(s, "yyyyMMddHHmmss.fffffffzz"), 
				_ => throw new FormatException(), 
			};
		}
		throw new FormatException();
	}

	private static int IndexOfSign(string s, int startIndex)
	{
		int num = Platform.IndexOf(s, '+', startIndex);
		if (num < 0)
		{
			num = Platform.IndexOf(s, '-', startIndex);
		}
		return num;
	}

	private static DateTime ParseLocal(string s, string format)
	{
		return DateTime.ParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeLocal);
	}

	private static DateTime ParseTimeZone(string s, string format)
	{
		return DateTime.ParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal);
	}

	private static DateTime ParseUtc(string s, string format)
	{
		return DateTime.ParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
	}

	private static string ToStringCanonical(DateTime dateTime)
	{
		return dateTime.ToUniversalTime().ToString("yyyyMMddHHmmss.FFFFFFFK", DateTimeFormatInfo.InvariantInfo);
	}
}
