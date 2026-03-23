namespace Mirror;

internal struct ClientMainEvent
{
	public ClientMainEventType type;

	public object param;

	public int? channelId;

	public TransportError? error;

	public ClientMainEvent(ClientMainEventType type, object param, int? channelId = null, TransportError? error = null)
	{
		this.type = type;
		this.channelId = channelId;
		this.error = error;
		this.param = param;
	}
}
