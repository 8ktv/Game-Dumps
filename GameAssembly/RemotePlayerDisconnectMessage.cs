using Mirror;

public struct RemotePlayerDisconnectMessage : NetworkMessage
{
	public ulong playerGuidOnServer;
}
