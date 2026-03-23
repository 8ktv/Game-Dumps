namespace Edgegap;

public enum ConnectionState : byte
{
	Disconnected,
	Checking,
	Valid,
	Invalid,
	SessionTimeout,
	Error
}
