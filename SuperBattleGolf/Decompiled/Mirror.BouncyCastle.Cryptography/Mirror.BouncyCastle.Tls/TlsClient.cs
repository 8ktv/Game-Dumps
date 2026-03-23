using System.Collections.Generic;

namespace Mirror.BouncyCastle.Tls;

public interface TlsClient : TlsPeer
{
	void Init(TlsClientContext context);

	TlsSession GetSessionToResume();

	IList<TlsPskExternal> GetExternalPsks();

	bool IsFallback();

	IDictionary<int, byte[]> GetClientExtensions();

	IList<int> GetEarlyKeyShareGroups();

	void NotifyServerVersion(ProtocolVersion selectedVersion);

	void NotifySessionToResume(TlsSession session);

	void NotifySessionID(byte[] sessionID);

	void NotifySelectedCipherSuite(int selectedCipherSuite);

	void NotifySelectedPsk(TlsPsk selectedPsk);

	void ProcessServerExtensions(IDictionary<int, byte[]> serverExtensions);

	void ProcessServerSupplementalData(IList<SupplementalDataEntry> serverSupplementalData);

	TlsPskIdentity GetPskIdentity();

	TlsSrpIdentity GetSrpIdentity();

	TlsDHGroupVerifier GetDHGroupVerifier();

	TlsSrpConfigVerifier GetSrpConfigVerifier();

	TlsAuthentication GetAuthentication();

	IList<SupplementalDataEntry> GetClientSupplementalData();

	void NotifyNewSessionTicket(NewSessionTicket newSessionTicket);
}
