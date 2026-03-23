using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public static class NetworkTime
{
	private const float DefaultPingInterval = 0.1f;

	public static float PingInterval = 0.1f;

	public const int PingWindowSize = 50;

	private static double lastPingTime;

	private static ExponentialMovingAverage _rtt = new ExponentialMovingAverage(50);

	private static int PredictionErrorWindowSize = 20;

	private static ExponentialMovingAverage _predictionErrorUnadjusted = new ExponentialMovingAverage(PredictionErrorWindowSize);

	public static double ClientLastPongsTime { get; private set; }

	public static double localTime
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Time.unscaledTimeAsDouble;
		}
	}

	public static double time
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (!NetworkServer.active)
			{
				return NetworkClient.localTimeline;
			}
			return localTime;
		}
	}

	public static double predictionErrorUnadjusted => _predictionErrorUnadjusted.Value;

	public static double predictionErrorAdjusted { get; private set; }

	public static double predictedTime
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (!NetworkServer.active)
			{
				return localTime + predictionErrorUnadjusted;
			}
			return localTime;
		}
	}

	public static double offset => localTime - time;

	public static double rtt => _rtt.Value;

	public static double rttVariance => _rtt.Variance;

	[RuntimeInitializeOnLoadMethod]
	public static void ResetStatics()
	{
		PingInterval = 0.1f;
		lastPingTime = 0.0;
		ClientLastPongsTime = Time.timeAsDouble;
		_rtt = new ExponentialMovingAverage(50);
	}

	internal static void UpdateClient()
	{
		if (localTime >= lastPingTime + (double)PingInterval)
		{
			SendPing();
		}
	}

	internal static void SendPing()
	{
		NetworkClient.Send(new NetworkPingMessage(localTime, predictedTime), 1);
		lastPingTime = localTime;
	}

	internal static void OnServerPing(NetworkConnectionToClient conn, NetworkPingMessage message)
	{
		double num = localTime - message.localTime;
		double num2 = localTime - message.predictedTimeAdjusted;
		NetworkPongMessage message2 = new NetworkPongMessage(message.localTime, num, num2);
		conn.Send(message2, 1);
	}

	internal static void OnClientPong(NetworkPongMessage message)
	{
		if (!(message.localTime > localTime))
		{
			double newValue = localTime - message.localTime;
			_rtt.Add(newValue);
			_predictionErrorUnadjusted.Add(message.predictionErrorUnadjusted);
			predictionErrorAdjusted = message.predictionErrorAdjusted;
			ClientLastPongsTime = Time.timeAsDouble;
		}
	}

	internal static void OnClientPing(NetworkPingMessage message)
	{
		NetworkClient.Send(new NetworkPongMessage(message.localTime, 0.0, 0.0), 1);
	}

	internal static void OnServerPong(NetworkConnectionToClient conn, NetworkPongMessage message)
	{
		if (!(message.localTime > localTime))
		{
			double newValue = localTime - message.localTime;
			conn._rtt.Add(newValue);
		}
	}

	internal static void EarlyUpdate()
	{
	}
}
