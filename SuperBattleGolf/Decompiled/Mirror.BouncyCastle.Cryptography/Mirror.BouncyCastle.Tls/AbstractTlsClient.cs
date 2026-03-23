using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public abstract class AbstractTlsClient : AbstractTlsPeer, TlsClient, TlsPeer
{
	protected TlsClientContext m_context;

	protected ProtocolVersion[] m_protocolVersions;

	protected int[] m_cipherSuites;

	protected IList<int> m_supportedGroups;

	protected IList<SignatureAndHashAlgorithm> m_supportedSignatureAlgorithms;

	protected IList<SignatureAndHashAlgorithm> m_supportedSignatureAlgorithmsCert;

	protected AbstractTlsClient(TlsCrypto crypto)
		: base(crypto)
	{
	}

	protected virtual bool AllowUnexpectedServerExtension(int extensionType, byte[] extensionData)
	{
		switch (extensionType)
		{
		case 10:
			TlsExtensionsUtilities.ReadSupportedGroupsExtension(extensionData);
			return true;
		case 11:
			TlsExtensionsUtilities.ReadSupportedPointFormatsExtension(extensionData);
			return true;
		default:
			return false;
		}
	}

	protected virtual IList<int> GetNamedGroupRoles()
	{
		IList<int> namedGroupRoles = TlsUtilities.GetNamedGroupRoles(GetCipherSuites());
		IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms = m_supportedSignatureAlgorithms;
		IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithmsCert = m_supportedSignatureAlgorithmsCert;
		if (supportedSignatureAlgorithms == null || TlsUtilities.ContainsAnySignatureAlgorithm(supportedSignatureAlgorithms, 3) || (supportedSignatureAlgorithmsCert != null && TlsUtilities.ContainsAnySignatureAlgorithm(supportedSignatureAlgorithmsCert, 3)))
		{
			TlsUtilities.AddToSet(namedGroupRoles, 3);
		}
		return namedGroupRoles;
	}

	protected virtual void CheckForUnexpectedServerExtension(IDictionary<int, byte[]> serverExtensions, int extensionType)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(serverExtensions, extensionType);
		if (extensionData != null && !AllowUnexpectedServerExtension(extensionType, extensionData))
		{
			throw new TlsFatalAlert(47);
		}
	}

	protected virtual byte[] GetNewConnectionID()
	{
		return null;
	}

	public virtual TlsPskIdentity GetPskIdentity()
	{
		return null;
	}

	public virtual TlsSrpIdentity GetSrpIdentity()
	{
		return null;
	}

	public virtual TlsDHGroupVerifier GetDHGroupVerifier()
	{
		return new DefaultTlsDHGroupVerifier();
	}

	public virtual TlsSrpConfigVerifier GetSrpConfigVerifier()
	{
		return new DefaultTlsSrpConfigVerifier();
	}

	protected virtual IList<X509Name> GetCertificateAuthorities()
	{
		return null;
	}

	protected virtual IList<ProtocolName> GetProtocolNames()
	{
		return null;
	}

	protected virtual CertificateStatusRequest GetCertificateStatusRequest()
	{
		return new CertificateStatusRequest(1, new OcspStatusRequest(null, null));
	}

	protected virtual IList<CertificateStatusRequestItemV2> GetMultiCertStatusRequest()
	{
		return null;
	}

	protected virtual IList<ServerName> GetSniServerNames()
	{
		return null;
	}

	protected virtual IList<int> GetSupportedGroups(IList<int> namedGroupRoles)
	{
		TlsCrypto crypto = Crypto;
		List<int> list = new List<int>();
		if (namedGroupRoles.Contains(2))
		{
			TlsUtilities.AddIfSupported(list, crypto, new int[2] { 29, 30 });
		}
		if (namedGroupRoles.Contains(2) || namedGroupRoles.Contains(3))
		{
			TlsUtilities.AddIfSupported(list, crypto, new int[2] { 23, 24 });
		}
		if (namedGroupRoles.Contains(1))
		{
			TlsUtilities.AddIfSupported(list, crypto, new int[3] { 256, 257, 258 });
		}
		return list;
	}

	protected virtual IList<SignatureAndHashAlgorithm> GetSupportedSignatureAlgorithms()
	{
		return TlsUtilities.GetDefaultSupportedSignatureAlgorithms(m_context);
	}

	protected virtual IList<SignatureAndHashAlgorithm> GetSupportedSignatureAlgorithmsCert()
	{
		return null;
	}

	protected virtual IList<TrustedAuthority> GetTrustedCAIndication()
	{
		return null;
	}

	protected virtual short[] GetAllowedClientCertificateTypes()
	{
		return null;
	}

	protected virtual short[] GetAllowedServerCertificateTypes()
	{
		return null;
	}

	public virtual void Init(TlsClientContext context)
	{
		m_context = context;
		m_protocolVersions = GetSupportedVersions();
		m_cipherSuites = GetSupportedCipherSuites();
	}

	public override ProtocolVersion[] GetProtocolVersions()
	{
		return m_protocolVersions;
	}

	public override int[] GetCipherSuites()
	{
		return m_cipherSuites;
	}

	public override void NotifyHandshakeBeginning()
	{
		base.NotifyHandshakeBeginning();
		m_supportedGroups = null;
		m_supportedSignatureAlgorithms = null;
		m_supportedSignatureAlgorithmsCert = null;
	}

	public virtual TlsSession GetSessionToResume()
	{
		return null;
	}

	public virtual IList<TlsPskExternal> GetExternalPsks()
	{
		return null;
	}

	public virtual bool IsFallback()
	{
		return false;
	}

	public virtual IDictionary<int, byte[]> GetClientExtensions()
	{
		Dictionary<int, byte[]> dictionary = new Dictionary<int, byte[]>();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		ProtocolVersion[] protocolVersions = GetProtocolVersions();
		foreach (ProtocolVersion protocolVersion in protocolVersions)
		{
			if (TlsUtilities.IsTlsV13(protocolVersion))
			{
				flag = true;
			}
			else
			{
				flag2 = true;
			}
			flag3 |= ProtocolVersion.DTLSv12.Equals(protocolVersion);
		}
		IList<ProtocolName> protocolNames = GetProtocolNames();
		if (protocolNames != null)
		{
			TlsExtensionsUtilities.AddAlpnExtensionClient(dictionary, protocolNames);
		}
		IList<ServerName> sniServerNames = GetSniServerNames();
		if (sniServerNames != null)
		{
			TlsExtensionsUtilities.AddServerNameExtensionClient(dictionary, sniServerNames);
		}
		CertificateStatusRequest certificateStatusRequest = GetCertificateStatusRequest();
		if (certificateStatusRequest != null)
		{
			TlsExtensionsUtilities.AddStatusRequestExtension(dictionary, certificateStatusRequest);
		}
		if (flag)
		{
			IList<X509Name> certificateAuthorities = GetCertificateAuthorities();
			if (certificateAuthorities != null)
			{
				TlsExtensionsUtilities.AddCertificateAuthoritiesExtension(dictionary, certificateAuthorities);
			}
		}
		if (flag2)
		{
			TlsExtensionsUtilities.AddEncryptThenMacExtension(dictionary);
			IList<CertificateStatusRequestItemV2> multiCertStatusRequest = GetMultiCertStatusRequest();
			if (multiCertStatusRequest != null)
			{
				TlsExtensionsUtilities.AddStatusRequestV2Extension(dictionary, multiCertStatusRequest);
			}
			IList<TrustedAuthority> trustedCAIndication = GetTrustedCAIndication();
			if (trustedCAIndication != null)
			{
				TlsExtensionsUtilities.AddTrustedCAKeysExtensionClient(dictionary, trustedCAIndication);
			}
		}
		if (TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(m_context.ClientVersion))
		{
			IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms = GetSupportedSignatureAlgorithms();
			if (supportedSignatureAlgorithms != null && supportedSignatureAlgorithms.Count > 0)
			{
				m_supportedSignatureAlgorithms = supportedSignatureAlgorithms;
				TlsExtensionsUtilities.AddSignatureAlgorithmsExtension(dictionary, supportedSignatureAlgorithms);
			}
			IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithmsCert = GetSupportedSignatureAlgorithmsCert();
			if (supportedSignatureAlgorithmsCert != null && supportedSignatureAlgorithmsCert.Count > 0)
			{
				m_supportedSignatureAlgorithmsCert = supportedSignatureAlgorithmsCert;
				TlsExtensionsUtilities.AddSignatureAlgorithmsCertExtension(dictionary, supportedSignatureAlgorithmsCert);
			}
		}
		IList<int> namedGroupRoles = GetNamedGroupRoles();
		IList<int> supportedGroups = GetSupportedGroups(namedGroupRoles);
		if (supportedGroups != null && supportedGroups.Count > 0)
		{
			m_supportedGroups = supportedGroups;
			TlsExtensionsUtilities.AddSupportedGroupsExtension(dictionary, supportedGroups);
		}
		if (flag2 && (namedGroupRoles.Contains(2) || namedGroupRoles.Contains(3)))
		{
			TlsExtensionsUtilities.AddSupportedPointFormatsExtension(dictionary, new short[1]);
		}
		short[] allowedClientCertificateTypes = GetAllowedClientCertificateTypes();
		if (allowedClientCertificateTypes != null && TlsUtilities.ContainsNot(allowedClientCertificateTypes, 0, allowedClientCertificateTypes.Length, 0))
		{
			TlsExtensionsUtilities.AddClientCertificateTypeExtensionClient(dictionary, allowedClientCertificateTypes);
		}
		short[] allowedServerCertificateTypes = GetAllowedServerCertificateTypes();
		if (allowedServerCertificateTypes != null && TlsUtilities.ContainsNot(allowedServerCertificateTypes, 0, allowedServerCertificateTypes.Length, 0))
		{
			TlsExtensionsUtilities.AddServerCertificateTypeExtensionClient(dictionary, allowedServerCertificateTypes);
		}
		if (flag3)
		{
			byte[] newConnectionID = GetNewConnectionID();
			if (newConnectionID != null)
			{
				TlsExtensionsUtilities.AddConnectionIDExtension(dictionary, newConnectionID);
			}
		}
		return dictionary;
	}

	public virtual IList<int> GetEarlyKeyShareGroups()
	{
		if (m_supportedGroups == null || m_supportedGroups.Count < 1)
		{
			return null;
		}
		if (m_supportedGroups.Contains(29))
		{
			return TlsUtilities.VectorOfOne(29);
		}
		if (m_supportedGroups.Contains(23))
		{
			return TlsUtilities.VectorOfOne(23);
		}
		return TlsUtilities.VectorOfOne(m_supportedGroups[0]);
	}

	public virtual bool ShouldUseCompatibilityMode()
	{
		return true;
	}

	public virtual void NotifyServerVersion(ProtocolVersion serverVersion)
	{
	}

	public virtual void NotifySessionToResume(TlsSession session)
	{
	}

	public virtual void NotifySessionID(byte[] sessionID)
	{
	}

	public virtual void NotifySelectedCipherSuite(int selectedCipherSuite)
	{
	}

	public virtual void NotifySelectedPsk(TlsPsk selectedPsk)
	{
	}

	public virtual void ProcessServerExtensions(IDictionary<int, byte[]> serverExtensions)
	{
		if (serverExtensions == null)
		{
			return;
		}
		SecurityParameters securityParameters = m_context.SecurityParameters;
		if (!TlsUtilities.IsTlsV13(securityParameters.NegotiatedVersion))
		{
			CheckForUnexpectedServerExtension(serverExtensions, 13);
			CheckForUnexpectedServerExtension(serverExtensions, 50);
			CheckForUnexpectedServerExtension(serverExtensions, 10);
			if (TlsEccUtilities.IsEccCipherSuite(securityParameters.CipherSuite))
			{
				TlsExtensionsUtilities.GetSupportedPointFormatsExtension(serverExtensions);
			}
			else
			{
				CheckForUnexpectedServerExtension(serverExtensions, 11);
			}
			CheckForUnexpectedServerExtension(serverExtensions, 21);
		}
	}

	public virtual void ProcessServerSupplementalData(IList<SupplementalDataEntry> serverSupplementalData)
	{
		if (serverSupplementalData != null)
		{
			throw new TlsFatalAlert(10);
		}
	}

	public abstract TlsAuthentication GetAuthentication();

	public virtual IList<SupplementalDataEntry> GetClientSupplementalData()
	{
		return null;
	}

	public virtual void NotifyNewSessionTicket(NewSessionTicket newSessionTicket)
	{
	}
}
