using System;
using System.Globalization;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.X509;

public class Time : Asn1Encodable, IAsn1Choice
{
	private readonly Asn1Object m_timeObject;

	public static Time GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Time result)
		{
			return result;
		}
		if (obj is Asn1UtcTime utcTime)
		{
			return new Time(utcTime);
		}
		if (obj is Asn1GeneralizedTime generalizedTime)
		{
			return new Time(generalizedTime);
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	public static Time GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	public Time(Asn1GeneralizedTime generalizedTime)
	{
		m_timeObject = generalizedTime ?? throw new ArgumentNullException("generalizedTime");
	}

	public Time(Asn1UtcTime utcTime)
	{
		if (utcTime == null)
		{
			throw new ArgumentNullException("utcTime");
		}
		utcTime.ToDateTime(2049);
		m_timeObject = utcTime;
	}

	public Time(DateTime date)
	{
		DateTime dateTime = date.ToUniversalTime();
		if (dateTime.Year < 1950 || dateTime.Year > 2049)
		{
			m_timeObject = Rfc5280Asn1Utilities.CreateGeneralizedTime(dateTime);
		}
		else
		{
			m_timeObject = Rfc5280Asn1Utilities.CreateUtcTime(dateTime);
		}
	}

	public DateTime ToDateTime()
	{
		try
		{
			if (m_timeObject is Asn1UtcTime asn1UtcTime)
			{
				return asn1UtcTime.ToDateTime(2049);
			}
			return ((Asn1GeneralizedTime)m_timeObject).ToDateTime();
		}
		catch (FormatException ex)
		{
			throw new InvalidOperationException("invalid date string: " + ex.Message);
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_timeObject;
	}

	public override string ToString()
	{
		if (m_timeObject is Asn1UtcTime asn1UtcTime)
		{
			return asn1UtcTime.ToDateTime(2049).ToString("yyyyMMddHHmmssK", DateTimeFormatInfo.InvariantInfo);
		}
		if (m_timeObject is Asn1GeneralizedTime asn1GeneralizedTime)
		{
			return asn1GeneralizedTime.ToDateTime().ToString("yyyyMMddHHmmss.FFFFFFFK", DateTimeFormatInfo.InvariantInfo);
		}
		throw new InvalidOperationException();
	}
}
