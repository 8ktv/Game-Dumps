namespace Mirror.BouncyCastle.Tls;

public interface TlsHeartbeat
{
	int IdleMillis { get; }

	int TimeoutMillis { get; }

	byte[] GeneratePayload();
}
