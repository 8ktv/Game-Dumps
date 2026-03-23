namespace Mirror;

internal struct ThreadEvent
{
	public ThreadEventType type;

	public object param;

	public int? connectionId;

	public int? channelId;

	public ThreadEvent(ThreadEventType type, object param, int? connectionId = null, int? channelId = null)
	{
		this.type = type;
		this.connectionId = connectionId;
		this.channelId = channelId;
		this.param = param;
	}
}
