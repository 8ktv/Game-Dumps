namespace Mirror;

internal struct QueuedMessage
{
	public int connectionId;

	public byte[] bytes;

	public double time;

	public int channelId;

	public QueuedMessage(int connectionId, byte[] bytes, double time, int channelId)
	{
		this.connectionId = connectionId;
		this.bytes = bytes;
		this.time = time;
		this.channelId = channelId;
	}
}
