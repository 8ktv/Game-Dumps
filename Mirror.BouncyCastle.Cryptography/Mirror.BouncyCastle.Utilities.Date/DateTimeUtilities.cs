using System;

namespace Mirror.BouncyCastle.Utilities.Date;

public static class DateTimeUtilities
{
	public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static readonly long MaxUnixMs;

	public static readonly long MinUnixMs;

	public static long DateTimeToUnixMs(DateTime dateTime)
	{
		DateTime dateTime2 = dateTime.ToUniversalTime();
		if (dateTime2.CompareTo(UnixEpoch) < 0)
		{
			throw new ArgumentOutOfRangeException("dateTime", "DateTime value may not be before the epoch");
		}
		long ticks = dateTime2.Ticks;
		DateTime unixEpoch = UnixEpoch;
		return (ticks - unixEpoch.Ticks) / 10000;
	}

	public static DateTime UnixMsToDateTime(long unixMs)
	{
		if (unixMs < MinUnixMs || unixMs > MaxUnixMs)
		{
			throw new ArgumentOutOfRangeException("unixMs");
		}
		long num = unixMs * 10000;
		DateTime unixEpoch = UnixEpoch;
		return new DateTime(num + unixEpoch.Ticks, DateTimeKind.Utc);
	}

	public static long CurrentUnixMs()
	{
		return DateTimeToUnixMs(DateTime.UtcNow);
	}

	public static DateTime WithPrecisionCentisecond(DateTime dateTime)
	{
		return dateTime.AddTicks(-(dateTime.Ticks % 100000));
	}

	public static DateTime WithPrecisionDecisecond(DateTime dateTime)
	{
		return dateTime.AddTicks(-(dateTime.Ticks % 1000000));
	}

	public static DateTime WithPrecisionMillisecond(DateTime dateTime)
	{
		return dateTime.AddTicks(-(dateTime.Ticks % 10000));
	}

	public static DateTime WithPrecisionSecond(DateTime dateTime)
	{
		return dateTime.AddTicks(-(dateTime.Ticks % 10000000));
	}

	static DateTimeUtilities()
	{
		DateTime maxValue = DateTime.MaxValue;
		MaxUnixMs = (maxValue.Ticks - UnixEpoch.Ticks) / 10000;
		MinUnixMs = 0L;
	}
}
