using System.Collections.Generic;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public interface TlsServer : TlsPeer
{
	void Init(TlsServerContext context);

	TlsSession GetSessionToResume(byte[] sessionID);

	byte[] GetNewSessionID();

	TlsPskExternal GetExternalPsk(IList<PskIdentity> identities);

	void NotifySession(TlsSession session);

	void NotifyClientVersion(ProtocolVersion clientVersion);

	void NotifyFallback(bool isFallback);

	void NotifyOfferedCipherSuites(int[] offeredCipherSuites);

	void ProcessClientExtensions(IDictionary<int, byte[]> clientExtensions);

	ProtocolVersion GetServerVersion();

	int[] GetSupportedGroups();

	int GetSelectedCipherSuite();

	IDictionary<int, byte[]> GetServerExtensions();

	void GetServerExtensionsForConnection(IDictionary<int, byte[]> serverExtensions);

	IList<SupplementalDataEntry> GetServerSupplementalData();

	TlsCredentials GetCredentials();

	CertificateStatus GetCertificateStatus();

	CertificateRequest GetCertificateRequest();

	TlsPskIdentityManager GetPskIdentityManager();

	TlsSrpLoginParameters GetSrpLoginParameters();

	TlsDHConfig GetDHConfig();

	TlsECConfig GetECDHConfig();

	void ProcessClientSupplementalData(IList<SupplementalDataEntry> clientSupplementalData);

	void NotifyClientCertificate(Certificate clientCertificate);

	NewSessionTicket GetNewSessionTicket();
}
