namespace Mirror;

public struct NetworkPingMessage : NetworkMessage
{
	public double localTime;

	public double predictedTimeAdjusted;

	public NetworkPingMessage(double localTime, double predictedTimeAdjusted)
	{
		this.localTime = localTime;
		this.predictedTimeAdjusted = predictedTimeAdjusted;
	}
}
