using System;
using System.Globalization;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Date;

namespace Mirror.BouncyCastle.Asn1;

public class Asn1UtcTime : Asn1Object
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(Asn1UtcTime), 23)
		{
		}

		internal override Asn1Object FromImplicitPrimitive(DerOctetString octetString)
		{
			return CreatePrimitive(octetString.GetOctets());
		}
	}

	private readonly string m_timeString;

	private readonly DateTime m_dateTime;

	private readonly bool m_dateTimeLocked;

	private readonly int m_twoDigitYearMax;

	public string TimeString => m_timeString;

	public int TwoDigitYearMax => m_twoDigitYearMax;

	public static Asn1UtcTime GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Asn1UtcTime result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is Asn1UtcTime result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (Asn1UtcTime)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct UTC time from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static Asn1UtcTime GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (Asn1UtcTime)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public Asn1UtcTime(string timeString)
	{
		m_timeString = timeString ?? throw new ArgumentNullException("timeString");
		try
		{
			m_dateTime = FromString(timeString, out m_twoDigitYearMax);
			m_dateTimeLocked = false;
		}
		catch (FormatException ex)
		{
			throw new ArgumentException("invalid date string: " + ex.Message);
		}
	}

	[Obsolete("Use `Asn1UtcTime(DateTime, int)' instead")]
	public Asn1UtcTime(DateTime dateTime)
	{
		dateTime = DateTimeUtilities.WithPrecisionSecond(dateTime.ToUniversalTime());
		m_dateTime = dateTime;
		m_dateTimeLocked = true;
		m_timeString = ToStringCanonical(dateTime, out m_twoDigitYearMax);
	}

	public Asn1UtcTime(DateTime dateTime, int twoDigitYearMax)
	{
		dateTime = DateTimeUtilities.WithPrecisionSecond(dateTime.ToUniversalTime());
		Validate(dateTime, twoDigitYearMax);
		m_dateTime = dateTime;
		m_dateTimeLocked = true;
		m_timeString = ToStringCanonical(dateTime);
		m_twoDigitYearMax = twoDigitYearMax;
	}

	internal Asn1UtcTime(byte[] contents)
		: this(Encoding.ASCII.GetString(contents))
	{
	}

	public DateTime ToDateTime()
	{
		return m_dateTime;
	}

	public DateTime ToDateTime(int twoDigitYearMax)
	{
		if (InRange(m_dateTime, twoDigitYearMax))
		{
			return m_dateTime;
		}
		if (m_dateTimeLocked)
		{
			throw new InvalidOperationException();
		}
		DateTime dateTime = m_dateTime;
		int num = dateTime.Year % 100;
		int num2 = twoDigitYearMax % 100;
		int num3 = num - num2;
		int num4 = twoDigitYearMax + num3;
		if (num3 > 0)
		{
			num4 -= 100;
		}
		dateTime = m_dateTime;
		int num5 = num4;
		DateTime dateTime2 = m_dateTime;
		return dateTime.AddYears(num5 - dateTime2.Year);
	}

	public DateTime ToDateTime(Calendar calendar)
	{
		return ToDateTime(calendar.TwoDigitYearMax);
	}

	[Obsolete("Use 'ToDateTime(2049)' instead")]
	public DateTime ToAdjustedDateTime()
	{
		return ToDateTime(2049);
	}

	internal byte[] GetContents(int encoding)
	{
		if (encoding == 3 && m_timeString.Length != 13)
		{
			string s = ToStringCanonical(m_dateTime);
			return Encoding.ASCII.GetBytes(s);
		}
		return Encoding.ASCII.GetBytes(m_timeString);
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 23, GetContents(encoding));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, GetContents(encoding));
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 23, GetContents(3));
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, GetContents(3));
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (!(asn1Object is Asn1UtcTime asn1UtcTime))
		{
			return false;
		}
		return Arrays.AreEqual(GetContents(3), asn1UtcTime.GetContents(3));
	}

	protected override int Asn1GetHashCode()
	{
		return Arrays.GetHashCode(GetContents(3));
	}

	public override string ToString()
	{
		return m_timeString;
	}

	internal static Asn1UtcTime CreatePrimitive(byte[] contents)
	{
		return new Asn1UtcTime(contents);
	}

	private static DateTime FromString(string s, out int twoDigitYearMax)
	{
		DateTimeFormatInfo invariantInfo = DateTimeFormatInfo.InvariantInfo;
		twoDigitYearMax = invariantInfo.Calendar.TwoDigitYearMax;
		return s.Length switch
		{
			11 => DateTime.ParseExact(s, "yyMMddHHmm\\Z", invariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal), 
			13 => DateTime.ParseExact(s, "yyMMddHHmmss\\Z", invariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal), 
			15 => DateTime.ParseExact(s, "yyMMddHHmmzzz", invariantInfo, DateTimeStyles.AdjustToUniversal), 
			17 => DateTime.ParseExact(s, "yyMMddHHmmsszzz", invariantInfo, DateTimeStyles.AdjustToUniversal), 
			_ => throw new FormatException(), 
		};
	}

	private static bool InRange(DateTime dateTime, int twoDigitYearMax)
	{
		return (uint)(twoDigitYearMax - dateTime.Year) < 100u;
	}

	private static string ToStringCanonical(DateTime dateTime, out int twoDigitYearMax)
	{
		DateTimeFormatInfo invariantInfo = DateTimeFormatInfo.InvariantInfo;
		twoDigitYearMax = invariantInfo.Calendar.TwoDigitYearMax;
		Validate(dateTime, twoDigitYearMax);
		return dateTime.ToString("yyMMddHHmmss\\Z", invariantInfo);
	}

	private static string ToStringCanonical(DateTime dateTime)
	{
		return dateTime.ToString("yyMMddHHmmss\\Z", DateTimeFormatInfo.InvariantInfo);
	}

	private static void Validate(DateTime dateTime, int twoDigitYearMax)
	{
		if (!InRange(dateTime, twoDigitYearMax))
		{
			throw new ArgumentOutOfRangeException("dateTime");
		}
	}
}
