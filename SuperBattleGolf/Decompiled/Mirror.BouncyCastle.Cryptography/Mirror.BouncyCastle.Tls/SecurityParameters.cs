using System.Collections.Generic;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public sealed class SecurityParameters
{
	internal int m_entity = -1;

	internal bool m_resumedSession;

	internal bool m_secureRenegotiation;

	internal int m_cipherSuite;

	internal short m_maxFragmentLength = -1;

	internal int m_prfAlgorithm = -1;

	internal int m_prfCryptoHashAlgorithm = -1;

	internal int m_prfHashLength = -1;

	internal int m_verifyDataLength = -1;

	internal TlsSecret m_baseKeyClient;

	internal TlsSecret m_baseKeyServer;

	internal TlsSecret m_earlyExporterMasterSecret;

	internal TlsSecret m_earlySecret;

	internal TlsSecret m_exporterMasterSecret;

	internal TlsSecret m_handshakeSecret;

	internal TlsSecret m_masterSecret;

	internal TlsSecret m_trafficSecretClient;

	internal TlsSecret m_trafficSecretServer;

	internal byte[] m_clientRandom;

	internal byte[] m_serverRandom;

	internal byte[] m_sessionHash;

	internal byte[] m_sessionID;

	internal byte[] m_pskIdentity;

	internal byte[] m_srpIdentity;

	internal byte[] m_tlsServerEndPoint;

	internal byte[] m_tlsUnique;

	internal bool m_encryptThenMac;

	internal bool m_extendedMasterSecret;

	internal bool m_extendedPadding;

	internal bool m_truncatedHmac;

	internal ProtocolName m_applicationProtocol;

	internal bool m_applicationProtocolSet;

	internal short[] m_clientCertTypes;

	internal IList<ServerName> m_clientServerNames;

	internal IList<SignatureAndHashAlgorithm> m_clientSigAlgs;

	internal IList<SignatureAndHashAlgorithm> m_clientSigAlgsCert;

	internal int[] m_clientSupportedGroups;

	internal IList<SignatureAndHashAlgorithm> m_serverSigAlgs;

	internal IList<SignatureAndHashAlgorithm> m_serverSigAlgsCert;

	internal int[] m_serverSupportedGroups;

	internal int m_keyExchangeAlgorithm = -1;

	internal Certificate m_localCertificate;

	internal Certificate m_peerCertificate;

	internal ProtocolVersion m_negotiatedVersion;

	internal int m_statusRequestVersion;

	internal short m_clientCertificateType;

	internal short m_serverCertificateType;

	internal byte[] m_localVerifyData;

	internal byte[] m_peerVerifyData;

	internal byte[] m_connectionIDLocal;

	internal byte[] m_connectionIDPeer;

	public ProtocolName ApplicationProtocol => m_applicationProtocol;

	public TlsSecret BaseKeyClient => m_baseKeyClient;

	public TlsSecret BaseKeyServer => m_baseKeyServer;

	public int CipherSuite => m_cipherSuite;

	public short ClientCertificateType => m_clientCertificateType;

	public short[] ClientCertTypes => m_clientCertTypes;

	public byte[] ClientRandom => m_clientRandom;

	public IList<ServerName> ClientServerNames => m_clientServerNames;

	public IList<SignatureAndHashAlgorithm> ClientSigAlgs => m_clientSigAlgs;

	public IList<SignatureAndHashAlgorithm> ClientSigAlgsCert => m_clientSigAlgsCert;

	public int[] ClientSupportedGroups => m_clientSupportedGroups;

	public byte[] ConnectionIDLocal => m_connectionIDLocal;

	public byte[] ConnectionIDPeer => m_connectionIDPeer;

	public TlsSecret EarlyExporterMasterSecret => m_earlyExporterMasterSecret;

	public TlsSecret EarlySecret => m_earlySecret;

	public TlsSecret ExporterMasterSecret => m_exporterMasterSecret;

	public int Entity => m_entity;

	public TlsSecret HandshakeSecret => m_handshakeSecret;

	public bool IsApplicationProtocolSet => m_applicationProtocolSet;

	public bool IsEncryptThenMac => m_encryptThenMac;

	public bool IsExtendedMasterSecret => m_extendedMasterSecret;

	public bool IsExtendedPadding => m_extendedPadding;

	public bool IsResumedSession => m_resumedSession;

	public bool IsSecureRenegotiation => m_secureRenegotiation;

	public bool IsTruncatedHmac => m_truncatedHmac;

	public int KeyExchangeAlgorithm => m_keyExchangeAlgorithm;

	public Certificate LocalCertificate => m_localCertificate;

	public byte[] LocalVerifyData => m_localVerifyData;

	public TlsSecret MasterSecret => m_masterSecret;

	public short MaxFragmentLength => m_maxFragmentLength;

	public ProtocolVersion NegotiatedVersion => m_negotiatedVersion;

	public Certificate PeerCertificate => m_peerCertificate;

	public byte[] PeerVerifyData => m_peerVerifyData;

	public int PrfAlgorithm => m_prfAlgorithm;

	public int PrfCryptoHashAlgorithm => m_prfCryptoHashAlgorithm;

	public int PrfHashLength => m_prfHashLength;

	public byte[] PskIdentity => m_pskIdentity;

	public short ServerCertificateType => m_serverCertificateType;

	public byte[] ServerRandom => m_serverRandom;

	public IList<SignatureAndHashAlgorithm> ServerSigAlgs => m_serverSigAlgs;

	public IList<SignatureAndHashAlgorithm> ServerSigAlgsCert => m_serverSigAlgsCert;

	public int[] ServerSupportedGroups => m_serverSupportedGroups;

	public byte[] SessionHash => m_sessionHash;

	public byte[] SessionID => m_sessionID;

	public byte[] SrpIdentity => m_srpIdentity;

	public int StatusRequestVersion => m_statusRequestVersion;

	public byte[] TlsServerEndPoint => m_tlsServerEndPoint;

	public byte[] TlsUnique => m_tlsUnique;

	public TlsSecret TrafficSecretClient => m_trafficSecretClient;

	public TlsSecret TrafficSecretServer => m_trafficSecretServer;

	public int VerifyDataLength => m_verifyDataLength;

	internal void Clear()
	{
		m_sessionHash = null;
		m_sessionID = null;
		m_clientCertTypes = null;
		m_clientServerNames = null;
		m_clientSigAlgs = null;
		m_clientSigAlgsCert = null;
		m_clientSupportedGroups = null;
		m_serverSigAlgs = null;
		m_serverSigAlgsCert = null;
		m_serverSupportedGroups = null;
		m_statusRequestVersion = 0;
		m_baseKeyClient = ClearSecret(m_baseKeyClient);
		m_baseKeyServer = ClearSecret(m_baseKeyServer);
		m_earlyExporterMasterSecret = ClearSecret(m_earlyExporterMasterSecret);
		m_earlySecret = ClearSecret(m_earlySecret);
		m_exporterMasterSecret = ClearSecret(m_exporterMasterSecret);
		m_handshakeSecret = ClearSecret(m_handshakeSecret);
		m_masterSecret = ClearSecret(m_masterSecret);
	}

	private static TlsSecret ClearSecret(TlsSecret secret)
	{
		secret?.Destroy();
		return null;
	}
}
