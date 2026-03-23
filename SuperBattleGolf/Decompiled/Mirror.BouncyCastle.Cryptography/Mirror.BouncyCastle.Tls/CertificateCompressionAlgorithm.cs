namespace Mirror.BouncyCastle.Tls;

public abstract class CertificateCompressionAlgorithm
{
	public const int zlib = 1;

	public const int brotli = 2;

	public const int zstd = 3;

	public static string GetName(int certificateCompressionAlgorithm)
	{
		return certificateCompressionAlgorithm switch
		{
			1 => "zlib", 
			2 => "brotli", 
			3 => "zstd", 
			_ => "UNKNOWN", 
		};
	}

	public static string GetText(int certificateCompressionAlgorithm)
	{
		return GetName(certificateCompressionAlgorithm) + "(" + certificateCompressionAlgorithm + ")";
	}

	public static bool IsRecognized(int certificateCompressionAlgorithm)
	{
		if ((uint)(certificateCompressionAlgorithm - 1) <= 2u)
		{
			return true;
		}
		return false;
	}
}
