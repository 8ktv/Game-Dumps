namespace Mirror.BouncyCastle.Tls;

public abstract class CertificateType
{
	public const short X509 = 0;

	public const short OpenPGP = 1;

	public const short RawPublicKey = 2;

	public static bool IsValid(short certificateType)
	{
		if (certificateType >= 0)
		{
			return certificateType <= 2;
		}
		return false;
	}
}
