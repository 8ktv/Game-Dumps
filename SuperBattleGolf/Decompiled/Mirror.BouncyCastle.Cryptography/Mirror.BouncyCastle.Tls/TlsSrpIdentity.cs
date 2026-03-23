namespace Mirror.BouncyCastle.Tls;

public interface TlsSrpIdentity
{
	byte[] GetSrpIdentity();

	byte[] GetSrpPassword();
}
