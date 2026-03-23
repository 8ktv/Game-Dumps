using UnityEngine;

namespace Mirror;

public static class ConnectionQualityHeuristics
{
	public static Color ColorCode(this ConnectionQuality quality)
	{
		return quality switch
		{
			ConnectionQuality.POOR => Color.red, 
			ConnectionQuality.FAIR => new Color(1f, 0.647f, 0f), 
			ConnectionQuality.GOOD => Color.yellow, 
			ConnectionQuality.EXCELLENT => Color.green, 
			_ => Color.gray, 
		};
	}

	public static ConnectionQuality Simple(double rtt, double jitter)
	{
		if (rtt <= 0.1 && jitter <= 0.1)
		{
			return ConnectionQuality.EXCELLENT;
		}
		if (rtt <= 0.2 && jitter <= 0.2)
		{
			return ConnectionQuality.GOOD;
		}
		if (rtt <= 0.4 && jitter <= 0.5)
		{
			return ConnectionQuality.FAIR;
		}
		return ConnectionQuality.POOR;
	}

	public static ConnectionQuality Pragmatic(double targetBufferTime, double currentBufferTime)
	{
		double num = currentBufferTime / targetBufferTime;
		if (num <= 1.15)
		{
			return ConnectionQuality.EXCELLENT;
		}
		if (num <= 1.25)
		{
			return ConnectionQuality.GOOD;
		}
		if (num <= 1.5)
		{
			return ConnectionQuality.FAIR;
		}
		return ConnectionQuality.POOR;
	}
}
