namespace Mirror.BouncyCastle.Tls;

public abstract class CachedInformationType
{
	public const short cert = 1;

	public const short cert_req = 2;

	public static string GetName(short cachedInformationType)
	{
		return cachedInformationType switch
		{
			1 => "cert", 
			2 => "cert_req", 
			_ => "UNKNOWN", 
		};
	}

	public static string GetText(short cachedInformationType)
	{
		return GetName(cachedInformationType) + "(" + cachedInformationType + ")";
	}
}
