namespace Mirror;

internal enum ThreadEventType
{
	DoServerStart,
	DoServerSend,
	DoServerDisconnect,
	DoServerStop,
	DoClientConnect,
	DoClientSend,
	DoClientDisconnect,
	Sleep,
	Wake,
	DoShutdown
}
