namespace Mirror;

public struct NetworkPongMessage : NetworkMessage
{
	public double localTime;

	public double predictionErrorUnadjusted;

	public double predictionErrorAdjusted;

	public NetworkPongMessage(double localTime, double predictionErrorUnadjusted, double predictionErrorAdjusted)
	{
		this.localTime = localTime;
		this.predictionErrorUnadjusted = predictionErrorUnadjusted;
		this.predictionErrorAdjusted = predictionErrorAdjusted;
	}
}
