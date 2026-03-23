namespace Mirror.BouncyCastle.Tls;

public interface TlsSrpIdentityManager
{
	TlsSrpLoginParameters GetLoginParameters(byte[] identity);
}
