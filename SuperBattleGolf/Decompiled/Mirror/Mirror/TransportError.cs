namespace Mirror;

public enum TransportError : byte
{
	DnsResolve,
	Refused,
	Timeout,
	Congestion,
	InvalidReceive,
	InvalidSend,
	ConnectionClosed,
	Unexpected
}
