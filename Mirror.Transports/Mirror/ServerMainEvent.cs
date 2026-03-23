namespace Mirror;

internal struct ServerMainEvent
{
	public ServerMainEventType type;

	public object param;

	public int? connectionId;

	public int? channelId;

	public TransportError? error;

	public ServerMainEvent(ServerMainEventType type, object param, int? connectionId, int? channelId = null, TransportError? error = null)
	{
		this.type = type;
		this.channelId = channelId;
		this.connectionId = connectionId;
		this.error = error;
		this.param = param;
	}
}
