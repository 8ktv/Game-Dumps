using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls;

public static class TlsExtensionsUtilities
{
	public static IDictionary<int, byte[]> EnsureExtensionsInitialised(IDictionary<int, byte[]> extensions)
	{
		if (extensions != null)
		{
			return extensions;
		}
		return new Dictionary<int, byte[]>();
	}

	public static void AddAlpnExtensionClient(IDictionary<int, byte[]> extensions, IList<ProtocolName> protocolNameList)
	{
		extensions[16] = CreateAlpnExtensionClient(protocolNameList);
	}

	public static void AddAlpnExtensionServer(IDictionary<int, byte[]> extensions, ProtocolName protocolName)
	{
		extensions[16] = CreateAlpnExtensionServer(protocolName);
	}

	public static void AddCertificateAuthoritiesExtension(IDictionary<int, byte[]> extensions, IList<X509Name> authorities)
	{
		extensions[47] = CreateCertificateAuthoritiesExtension(authorities);
	}

	public static void AddClientCertificateTypeExtensionClient(IDictionary<int, byte[]> extensions, short[] certificateTypes)
	{
		extensions[19] = CreateCertificateTypeExtensionClient(certificateTypes);
	}

	public static void AddClientCertificateTypeExtensionServer(IDictionary<int, byte[]> extensions, short certificateType)
	{
		extensions[19] = CreateCertificateTypeExtensionServer(certificateType);
	}

	public static void AddClientCertificateUrlExtension(IDictionary<int, byte[]> extensions)
	{
		extensions[2] = CreateClientCertificateUrlExtension();
	}

	public static void AddCompressCertificateExtension(IDictionary<int, byte[]> extensions, int[] algorithms)
	{
		extensions[27] = CreateCompressCertificateExtension(algorithms);
	}

	public static void AddConnectionIDExtension(IDictionary<int, byte[]> extensions, byte[] connectionID)
	{
		extensions[54] = CreateConnectionIDExtension(connectionID);
	}

	public static void AddCookieExtension(IDictionary<int, byte[]> extensions, byte[] cookie)
	{
		extensions[44] = CreateCookieExtension(cookie);
	}

	public static void AddEarlyDataIndication(IDictionary<int, byte[]> extensions)
	{
		extensions[42] = CreateEarlyDataIndication();
	}

	public static void AddEarlyDataMaxSize(IDictionary<int, byte[]> extensions, long maxSize)
	{
		extensions[42] = CreateEarlyDataMaxSize(maxSize);
	}

	public static void AddEmptyExtensionData(IDictionary<int, byte[]> extensions, int extType)
	{
		extensions[extType] = CreateEmptyExtensionData();
	}

	public static void AddEncryptThenMacExtension(IDictionary<int, byte[]> extensions)
	{
		extensions[22] = CreateEncryptThenMacExtension();
	}

	public static void AddExtendedMasterSecretExtension(IDictionary<int, byte[]> extensions)
	{
		extensions[23] = CreateExtendedMasterSecretExtension();
	}

	public static void AddHeartbeatExtension(IDictionary<int, byte[]> extensions, HeartbeatExtension heartbeatExtension)
	{
		extensions[15] = CreateHeartbeatExtension(heartbeatExtension);
	}

	public static void AddKeyShareClientHello(IDictionary<int, byte[]> extensions, IList<KeyShareEntry> clientShares)
	{
		extensions[51] = CreateKeyShareClientHello(clientShares);
	}

	public static void AddKeyShareHelloRetryRequest(IDictionary<int, byte[]> extensions, int namedGroup)
	{
		extensions[51] = CreateKeyShareHelloRetryRequest(namedGroup);
	}

	public static void AddKeyShareServerHello(IDictionary<int, byte[]> extensions, KeyShareEntry serverShare)
	{
		extensions[51] = CreateKeyShareServerHello(serverShare);
	}

	public static void AddMaxFragmentLengthExtension(IDictionary<int, byte[]> extensions, short maxFragmentLength)
	{
		extensions[1] = CreateMaxFragmentLengthExtension(maxFragmentLength);
	}

	public static void AddOidFiltersExtension(IDictionary<int, byte[]> extensions, IDictionary<DerObjectIdentifier, byte[]> filters)
	{
		extensions[48] = CreateOidFiltersExtension(filters);
	}

	public static void AddPaddingExtension(IDictionary<int, byte[]> extensions, int dataLength)
	{
		extensions[21] = CreatePaddingExtension(dataLength);
	}

	public static void AddPostHandshakeAuthExtension(IDictionary<int, byte[]> extensions)
	{
		extensions[49] = CreatePostHandshakeAuthExtension();
	}

	public static void AddPreSharedKeyClientHello(IDictionary<int, byte[]> extensions, OfferedPsks offeredPsks)
	{
		extensions[41] = CreatePreSharedKeyClientHello(offeredPsks);
	}

	public static void AddPreSharedKeyServerHello(IDictionary<int, byte[]> extensions, int selectedIdentity)
	{
		extensions[41] = CreatePreSharedKeyServerHello(selectedIdentity);
	}

	public static void AddPskKeyExchangeModesExtension(IDictionary<int, byte[]> extensions, short[] modes)
	{
		extensions[45] = CreatePskKeyExchangeModesExtension(modes);
	}

	public static void AddRecordSizeLimitExtension(IDictionary<int, byte[]> extensions, int recordSizeLimit)
	{
		extensions[28] = CreateRecordSizeLimitExtension(recordSizeLimit);
	}

	public static void AddServerCertificateTypeExtensionClient(IDictionary<int, byte[]> extensions, short[] certificateTypes)
	{
		extensions[20] = CreateCertificateTypeExtensionClient(certificateTypes);
	}

	public static void AddServerCertificateTypeExtensionServer(IDictionary<int, byte[]> extensions, short certificateType)
	{
		extensions[20] = CreateCertificateTypeExtensionServer(certificateType);
	}

	public static void AddServerNameExtensionClient(IDictionary<int, byte[]> extensions, IList<ServerName> serverNameList)
	{
		extensions[0] = CreateServerNameExtensionClient(serverNameList);
	}

	public static void AddServerNameExtensionServer(IDictionary<int, byte[]> extensions)
	{
		extensions[0] = CreateServerNameExtensionServer();
	}

	public static void AddSignatureAlgorithmsExtension(IDictionary<int, byte[]> extensions, IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms)
	{
		extensions[13] = CreateSignatureAlgorithmsExtension(supportedSignatureAlgorithms);
	}

	public static void AddSignatureAlgorithmsCertExtension(IDictionary<int, byte[]> extensions, IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms)
	{
		extensions[50] = CreateSignatureAlgorithmsCertExtension(supportedSignatureAlgorithms);
	}

	public static void AddStatusRequestExtension(IDictionary<int, byte[]> extensions, CertificateStatusRequest statusRequest)
	{
		extensions[5] = CreateStatusRequestExtension(statusRequest);
	}

	public static void AddStatusRequestV2Extension(IDictionary<int, byte[]> extensions, IList<CertificateStatusRequestItemV2> statusRequestV2)
	{
		extensions[17] = CreateStatusRequestV2Extension(statusRequestV2);
	}

	public static void AddSupportedGroupsExtension(IDictionary<int, byte[]> extensions, IList<int> namedGroups)
	{
		extensions[10] = CreateSupportedGroupsExtension(namedGroups);
	}

	public static void AddSupportedPointFormatsExtension(IDictionary<int, byte[]> extensions, short[] ecPointFormats)
	{
		extensions[11] = CreateSupportedPointFormatsExtension(ecPointFormats);
	}

	public static void AddSupportedVersionsExtensionClient(IDictionary<int, byte[]> extensions, ProtocolVersion[] versions)
	{
		extensions[43] = CreateSupportedVersionsExtensionClient(versions);
	}

	public static void AddSupportedVersionsExtensionServer(IDictionary<int, byte[]> extensions, ProtocolVersion selectedVersion)
	{
		extensions[43] = CreateSupportedVersionsExtensionServer(selectedVersion);
	}

	public static void AddTruncatedHmacExtension(IDictionary<int, byte[]> extensions)
	{
		extensions[4] = CreateTruncatedHmacExtension();
	}

	public static void AddTrustedCAKeysExtensionClient(IDictionary<int, byte[]> extensions, IList<TrustedAuthority> trustedAuthoritiesList)
	{
		extensions[3] = CreateTrustedCAKeysExtensionClient(trustedAuthoritiesList);
	}

	public static void AddTrustedCAKeysExtensionServer(IDictionary<int, byte[]> extensions)
	{
		extensions[3] = CreateTrustedCAKeysExtensionServer();
	}

	public static IList<ProtocolName> GetAlpnExtensionClient(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 16);
		if (extensionData != null)
		{
			return ReadAlpnExtensionClient(extensionData);
		}
		return null;
	}

	public static ProtocolName GetAlpnExtensionServer(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 16);
		if (extensionData != null)
		{
			return ReadAlpnExtensionServer(extensionData);
		}
		return null;
	}

	public static IList<X509Name> GetCertificateAuthoritiesExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 47);
		if (extensionData != null)
		{
			return ReadCertificateAuthoritiesExtension(extensionData);
		}
		return null;
	}

	public static short[] GetClientCertificateTypeExtensionClient(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 19);
		if (extensionData != null)
		{
			return ReadCertificateTypeExtensionClient(extensionData);
		}
		return null;
	}

	public static short GetClientCertificateTypeExtensionServer(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 19);
		if (extensionData != null)
		{
			return ReadCertificateTypeExtensionServer(extensionData);
		}
		return -1;
	}

	[Obsolete("Use version without 'defaultValue' instead")]
	public static short GetClientCertificateTypeExtensionServer(IDictionary<int, byte[]> extensions, short defaultValue)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 19);
		if (extensionData != null)
		{
			return ReadCertificateTypeExtensionServer(extensionData);
		}
		return defaultValue;
	}

	public static int[] GetCompressCertificateExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 27);
		if (extensionData != null)
		{
			return ReadCompressCertificateExtension(extensionData);
		}
		return null;
	}

	public static byte[] GetConnectionIDExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 54);
		if (extensionData != null)
		{
			return ReadConnectionIDExtension(extensionData);
		}
		return null;
	}

	public static byte[] GetCookieExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 44);
		if (extensionData != null)
		{
			return ReadCookieExtension(extensionData);
		}
		return null;
	}

	public static long GetEarlyDataMaxSize(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 42);
		if (extensionData != null)
		{
			return ReadEarlyDataMaxSize(extensionData);
		}
		return -1L;
	}

	public static HeartbeatExtension GetHeartbeatExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 15);
		if (extensionData != null)
		{
			return ReadHeartbeatExtension(extensionData);
		}
		return null;
	}

	public static IList<KeyShareEntry> GetKeyShareClientHello(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 51);
		if (extensionData != null)
		{
			return ReadKeyShareClientHello(extensionData);
		}
		return null;
	}

	public static int GetKeyShareHelloRetryRequest(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 51);
		if (extensionData != null)
		{
			return ReadKeyShareHelloRetryRequest(extensionData);
		}
		return -1;
	}

	public static KeyShareEntry GetKeyShareServerHello(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 51);
		if (extensionData != null)
		{
			return ReadKeyShareServerHello(extensionData);
		}
		return null;
	}

	public static short GetMaxFragmentLengthExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 1);
		if (extensionData != null)
		{
			return ReadMaxFragmentLengthExtension(extensionData);
		}
		return -1;
	}

	public static IDictionary<DerObjectIdentifier, byte[]> GetOidFiltersExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 48);
		if (extensionData != null)
		{
			return ReadOidFiltersExtension(extensionData);
		}
		return null;
	}

	public static int GetPaddingExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 21);
		if (extensionData != null)
		{
			return ReadPaddingExtension(extensionData);
		}
		return -1;
	}

	public static OfferedPsks GetPreSharedKeyClientHello(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 41);
		if (extensionData != null)
		{
			return ReadPreSharedKeyClientHello(extensionData);
		}
		return null;
	}

	public static int GetPreSharedKeyServerHello(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 41);
		if (extensionData != null)
		{
			return ReadPreSharedKeyServerHello(extensionData);
		}
		return -1;
	}

	public static short[] GetPskKeyExchangeModesExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 45);
		if (extensionData != null)
		{
			return ReadPskKeyExchangeModesExtension(extensionData);
		}
		return null;
	}

	public static int GetRecordSizeLimitExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 28);
		if (extensionData != null)
		{
			return ReadRecordSizeLimitExtension(extensionData);
		}
		return -1;
	}

	public static short[] GetServerCertificateTypeExtensionClient(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 20);
		if (extensionData != null)
		{
			return ReadCertificateTypeExtensionClient(extensionData);
		}
		return null;
	}

	public static short GetServerCertificateTypeExtensionServer(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 20);
		if (extensionData != null)
		{
			return ReadCertificateTypeExtensionServer(extensionData);
		}
		return -1;
	}

	[Obsolete("Use version without 'defaultValue' instead")]
	public static short GetServerCertificateTypeExtensionServer(IDictionary<int, byte[]> extensions, short defaultValue)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 20);
		if (extensionData != null)
		{
			return ReadCertificateTypeExtensionServer(extensionData);
		}
		return defaultValue;
	}

	public static IList<ServerName> GetServerNameExtensionClient(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 0);
		if (extensionData != null)
		{
			return ReadServerNameExtensionClient(extensionData);
		}
		return null;
	}

	public static IList<SignatureAndHashAlgorithm> GetSignatureAlgorithmsExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 13);
		if (extensionData != null)
		{
			return ReadSignatureAlgorithmsExtension(extensionData);
		}
		return null;
	}

	public static IList<SignatureAndHashAlgorithm> GetSignatureAlgorithmsCertExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 50);
		if (extensionData != null)
		{
			return ReadSignatureAlgorithmsCertExtension(extensionData);
		}
		return null;
	}

	public static CertificateStatusRequest GetStatusRequestExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 5);
		if (extensionData != null)
		{
			return ReadStatusRequestExtension(extensionData);
		}
		return null;
	}

	public static IList<CertificateStatusRequestItemV2> GetStatusRequestV2Extension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 17);
		if (extensionData != null)
		{
			return ReadStatusRequestV2Extension(extensionData);
		}
		return null;
	}

	public static int[] GetSupportedGroupsExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 10);
		if (extensionData != null)
		{
			return ReadSupportedGroupsExtension(extensionData);
		}
		return null;
	}

	public static short[] GetSupportedPointFormatsExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 11);
		if (extensionData != null)
		{
			return ReadSupportedPointFormatsExtension(extensionData);
		}
		return null;
	}

	public static ProtocolVersion[] GetSupportedVersionsExtensionClient(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 43);
		if (extensionData != null)
		{
			return ReadSupportedVersionsExtensionClient(extensionData);
		}
		return null;
	}

	public static ProtocolVersion GetSupportedVersionsExtensionServer(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 43);
		if (extensionData != null)
		{
			return ReadSupportedVersionsExtensionServer(extensionData);
		}
		return null;
	}

	public static IList<TrustedAuthority> GetTrustedCAKeysExtensionClient(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 3);
		if (extensionData != null)
		{
			return ReadTrustedCAKeysExtensionClient(extensionData);
		}
		return null;
	}

	public static bool HasClientCertificateUrlExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 2);
		if (extensionData != null)
		{
			return ReadClientCertificateUrlExtension(extensionData);
		}
		return false;
	}

	public static bool HasEarlyDataIndication(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 42);
		if (extensionData != null)
		{
			return ReadEarlyDataIndication(extensionData);
		}
		return false;
	}

	public static bool HasEncryptThenMacExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 22);
		if (extensionData != null)
		{
			return ReadEncryptThenMacExtension(extensionData);
		}
		return false;
	}

	public static bool HasExtendedMasterSecretExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 23);
		if (extensionData != null)
		{
			return ReadExtendedMasterSecretExtension(extensionData);
		}
		return false;
	}

	public static bool HasServerNameExtensionServer(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 0);
		if (extensionData != null)
		{
			return ReadServerNameExtensionServer(extensionData);
		}
		return false;
	}

	public static bool HasPostHandshakeAuthExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 49);
		if (extensionData != null)
		{
			return ReadPostHandshakeAuthExtension(extensionData);
		}
		return false;
	}

	public static bool HasTruncatedHmacExtension(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 4);
		if (extensionData != null)
		{
			return ReadTruncatedHmacExtension(extensionData);
		}
		return false;
	}

	public static bool HasTrustedCAKeysExtensionServer(IDictionary<int, byte[]> extensions)
	{
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 3);
		if (extensionData != null)
		{
			return ReadTrustedCAKeysExtensionServer(extensionData);
		}
		return false;
	}

	public static byte[] CreateAlpnExtensionClient(IList<ProtocolName> protocolNameList)
	{
		if (protocolNameList == null || protocolNameList.Count < 1)
		{
			throw new TlsFatalAlert(80);
		}
		MemoryStream memoryStream = new MemoryStream();
		TlsUtilities.WriteUint16(0, memoryStream);
		foreach (ProtocolName protocolName in protocolNameList)
		{
			protocolName.Encode(memoryStream);
		}
		return PatchOpaque16(memoryStream);
	}

	public static byte[] CreateAlpnExtensionServer(ProtocolName protocolName)
	{
		return CreateAlpnExtensionClient(new List<ProtocolName> { protocolName });
	}

	public static byte[] CreateCertificateAuthoritiesExtension(IList<X509Name> authorities)
	{
		if (authorities == null || authorities.Count < 1)
		{
			throw new TlsFatalAlert(80);
		}
		MemoryStream memoryStream = new MemoryStream();
		TlsUtilities.WriteUint16(0, memoryStream);
		foreach (X509Name authority in authorities)
		{
			TlsUtilities.WriteOpaque16(authority.GetEncoded("DER"), memoryStream);
		}
		return PatchOpaque16(memoryStream);
	}

	public static byte[] CreateCertificateTypeExtensionClient(short[] certificateTypes)
	{
		if (TlsUtilities.IsNullOrEmpty(certificateTypes) || certificateTypes.Length > 255)
		{
			throw new TlsFatalAlert(80);
		}
		return TlsUtilities.EncodeUint8ArrayWithUint8Length(certificateTypes);
	}

	public static byte[] CreateCertificateTypeExtensionServer(short certificateType)
	{
		return TlsUtilities.EncodeUint8(certificateType);
	}

	public static byte[] CreateClientCertificateUrlExtension()
	{
		return CreateEmptyExtensionData();
	}

	public static byte[] CreateCompressCertificateExtension(int[] algorithms)
	{
		if (TlsUtilities.IsNullOrEmpty(algorithms) || algorithms.Length > 127)
		{
			throw new TlsFatalAlert(80);
		}
		return TlsUtilities.EncodeUint16ArrayWithUint8Length(algorithms);
	}

	public static byte[] CreateConnectionIDExtension(byte[] connectionID)
	{
		if (connectionID == null)
		{
			throw new TlsFatalAlert(80);
		}
		return TlsUtilities.EncodeOpaque8(connectionID);
	}

	public static byte[] CreateCookieExtension(byte[] cookie)
	{
		if (TlsUtilities.IsNullOrEmpty(cookie) || cookie.Length >= 65536)
		{
			throw new TlsFatalAlert(80);
		}
		return TlsUtilities.EncodeOpaque16(cookie);
	}

	public static byte[] CreateEarlyDataIndication()
	{
		return CreateEmptyExtensionData();
	}

	public static byte[] CreateEarlyDataMaxSize(long maxSize)
	{
		return TlsUtilities.EncodeUint32(maxSize);
	}

	public static byte[] CreateEmptyExtensionData()
	{
		return TlsUtilities.EmptyBytes;
	}

	public static byte[] CreateEncryptThenMacExtension()
	{
		return CreateEmptyExtensionData();
	}

	public static byte[] CreateExtendedMasterSecretExtension()
	{
		return CreateEmptyExtensionData();
	}

	public static byte[] CreateHeartbeatExtension(HeartbeatExtension heartbeatExtension)
	{
		if (heartbeatExtension == null)
		{
			throw new TlsFatalAlert(80);
		}
		MemoryStream memoryStream = new MemoryStream();
		heartbeatExtension.Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] CreateKeyShareClientHello(IList<KeyShareEntry> clientShares)
	{
		if (clientShares == null || clientShares.Count < 1)
		{
			return TlsUtilities.EncodeUint16(0);
		}
		MemoryStream memoryStream = new MemoryStream();
		TlsUtilities.WriteUint16(0, memoryStream);
		foreach (KeyShareEntry clientShare in clientShares)
		{
			clientShare.Encode(memoryStream);
		}
		return PatchOpaque16(memoryStream);
	}

	public static byte[] CreateKeyShareHelloRetryRequest(int namedGroup)
	{
		return TlsUtilities.EncodeUint16(namedGroup);
	}

	public static byte[] CreateKeyShareServerHello(KeyShareEntry serverShare)
	{
		if (serverShare == null)
		{
			throw new TlsFatalAlert(80);
		}
		MemoryStream memoryStream = new MemoryStream();
		serverShare.Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] CreateMaxFragmentLengthExtension(short maxFragmentLength)
	{
		return TlsUtilities.EncodeUint8(maxFragmentLength);
	}

	public static byte[] CreateOidFiltersExtension(IDictionary<DerObjectIdentifier, byte[]> filters)
	{
		MemoryStream memoryStream = new MemoryStream();
		TlsUtilities.WriteUint16(0, memoryStream);
		if (filters != null)
		{
			foreach (KeyValuePair<DerObjectIdentifier, byte[]> filter in filters)
			{
				DerObjectIdentifier key = filter.Key;
				byte[] value = filter.Value;
				if (key == null || value == null)
				{
					throw new TlsFatalAlert(80);
				}
				TlsUtilities.WriteOpaque8(key.GetEncoded("DER"), memoryStream);
				TlsUtilities.WriteOpaque16(value, memoryStream);
			}
		}
		return PatchOpaque16(memoryStream);
	}

	public static byte[] CreatePaddingExtension(int dataLength)
	{
		TlsUtilities.CheckUint16(dataLength);
		return new byte[dataLength];
	}

	public static byte[] CreatePostHandshakeAuthExtension()
	{
		return CreateEmptyExtensionData();
	}

	public static byte[] CreatePreSharedKeyClientHello(OfferedPsks offeredPsks)
	{
		if (offeredPsks == null)
		{
			throw new TlsFatalAlert(80);
		}
		MemoryStream memoryStream = new MemoryStream();
		offeredPsks.Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] CreatePreSharedKeyServerHello(int selectedIdentity)
	{
		return TlsUtilities.EncodeUint16(selectedIdentity);
	}

	public static byte[] CreatePskKeyExchangeModesExtension(short[] modes)
	{
		if (TlsUtilities.IsNullOrEmpty(modes) || modes.Length > 255)
		{
			throw new TlsFatalAlert(80);
		}
		return TlsUtilities.EncodeUint8ArrayWithUint8Length(modes);
	}

	public static byte[] CreateRecordSizeLimitExtension(int recordSizeLimit)
	{
		if (recordSizeLimit < 64)
		{
			throw new TlsFatalAlert(80);
		}
		return TlsUtilities.EncodeUint16(recordSizeLimit);
	}

	public static byte[] CreateServerNameExtensionClient(IList<ServerName> serverNameList)
	{
		if (serverNameList == null)
		{
			throw new TlsFatalAlert(80);
		}
		MemoryStream memoryStream = new MemoryStream();
		new ServerNameList(serverNameList).Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] CreateServerNameExtensionServer()
	{
		return CreateEmptyExtensionData();
	}

	public static byte[] CreateSignatureAlgorithmsExtension(IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms)
	{
		MemoryStream memoryStream = new MemoryStream();
		TlsUtilities.EncodeSupportedSignatureAlgorithms(supportedSignatureAlgorithms, memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] CreateSignatureAlgorithmsCertExtension(IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms)
	{
		return CreateSignatureAlgorithmsExtension(supportedSignatureAlgorithms);
	}

	public static byte[] CreateStatusRequestExtension(CertificateStatusRequest statusRequest)
	{
		if (statusRequest == null)
		{
			throw new TlsFatalAlert(80);
		}
		MemoryStream memoryStream = new MemoryStream();
		statusRequest.Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] CreateStatusRequestV2Extension(IList<CertificateStatusRequestItemV2> statusRequestV2)
	{
		if (statusRequestV2 == null || statusRequestV2.Count < 1)
		{
			throw new TlsFatalAlert(80);
		}
		MemoryStream memoryStream = new MemoryStream();
		TlsUtilities.WriteUint16(0, memoryStream);
		foreach (CertificateStatusRequestItemV2 item in statusRequestV2)
		{
			item.Encode(memoryStream);
		}
		return PatchOpaque16(memoryStream);
	}

	public static byte[] CreateSupportedGroupsExtension(IList<int> namedGroups)
	{
		if (namedGroups == null || namedGroups.Count < 1)
		{
			throw new TlsFatalAlert(80);
		}
		int count = namedGroups.Count;
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = namedGroups[i];
		}
		return TlsUtilities.EncodeUint16ArrayWithUint16Length(array);
	}

	public static byte[] CreateSupportedPointFormatsExtension(short[] ecPointFormats)
	{
		if (ecPointFormats == null || !Arrays.Contains(ecPointFormats, 0))
		{
			ecPointFormats = Arrays.Prepend(ecPointFormats, 0);
		}
		return TlsUtilities.EncodeUint8ArrayWithUint8Length(ecPointFormats);
	}

	public static byte[] CreateSupportedVersionsExtensionClient(ProtocolVersion[] versions)
	{
		if (TlsUtilities.IsNullOrEmpty(versions) || versions.Length > 127)
		{
			throw new TlsFatalAlert(80);
		}
		int num = versions.Length;
		byte[] array = new byte[1 + num * 2];
		TlsUtilities.WriteUint8(num * 2, array, 0);
		for (int i = 0; i < num; i++)
		{
			TlsUtilities.WriteVersion(versions[i], array, 1 + i * 2);
		}
		return array;
	}

	public static byte[] CreateSupportedVersionsExtensionServer(ProtocolVersion selectedVersion)
	{
		return TlsUtilities.EncodeVersion(selectedVersion);
	}

	public static byte[] CreateTruncatedHmacExtension()
	{
		return CreateEmptyExtensionData();
	}

	public static byte[] CreateTrustedCAKeysExtensionClient(IList<TrustedAuthority> trustedAuthoritiesList)
	{
		MemoryStream memoryStream = new MemoryStream();
		TlsUtilities.WriteUint16(0, memoryStream);
		if (trustedAuthoritiesList != null)
		{
			foreach (TrustedAuthority trustedAuthorities in trustedAuthoritiesList)
			{
				trustedAuthorities.Encode(memoryStream);
			}
		}
		return PatchOpaque16(memoryStream);
	}

	public static byte[] CreateTrustedCAKeysExtensionServer()
	{
		return CreateEmptyExtensionData();
	}

	private static bool ReadEmptyExtensionData(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		if (extensionData.Length != 0)
		{
			throw new TlsFatalAlert(47);
		}
		return true;
	}

	public static IList<ProtocolName> ReadAlpnExtensionClient(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		MemoryStream memoryStream = new MemoryStream(extensionData);
		if (TlsUtilities.ReadUint16(memoryStream) != extensionData.Length - 2)
		{
			throw new TlsFatalAlert(50);
		}
		List<ProtocolName> list = new List<ProtocolName>();
		while (memoryStream.Position < memoryStream.Length)
		{
			ProtocolName item = ProtocolName.Parse(memoryStream);
			list.Add(item);
		}
		return list;
	}

	public static ProtocolName ReadAlpnExtensionServer(byte[] extensionData)
	{
		IList<ProtocolName> list = ReadAlpnExtensionClient(extensionData);
		if (list.Count != 1)
		{
			throw new TlsFatalAlert(50);
		}
		return list[0];
	}

	public static IList<X509Name> ReadCertificateAuthoritiesExtension(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		if (extensionData.Length < 5)
		{
			throw new TlsFatalAlert(50);
		}
		MemoryStream memoryStream = new MemoryStream(extensionData);
		if (TlsUtilities.ReadUint16(memoryStream) != extensionData.Length - 2)
		{
			throw new TlsFatalAlert(50);
		}
		List<X509Name> list = new List<X509Name>();
		while (memoryStream.Position < memoryStream.Length)
		{
			byte[] encoding = TlsUtilities.ReadOpaque16(memoryStream, 1);
			X509Name instance = X509Name.GetInstance(TlsUtilities.ReadAsn1Object(encoding));
			TlsUtilities.RequireDerEncoding(instance, encoding);
			list.Add(instance);
		}
		return list;
	}

	public static short[] ReadCertificateTypeExtensionClient(byte[] extensionData)
	{
		short[] array = TlsUtilities.DecodeUint8ArrayWithUint8Length(extensionData);
		if (array.Length < 1)
		{
			throw new TlsFatalAlert(50);
		}
		return array;
	}

	public static short ReadCertificateTypeExtensionServer(byte[] extensionData)
	{
		return TlsUtilities.DecodeUint8(extensionData);
	}

	public static bool ReadClientCertificateUrlExtension(byte[] extensionData)
	{
		return ReadEmptyExtensionData(extensionData);
	}

	public static int[] ReadCompressCertificateExtension(byte[] extensionData)
	{
		int[] array = TlsUtilities.DecodeUint16ArrayWithUint8Length(extensionData);
		if (array.Length < 1)
		{
			throw new TlsFatalAlert(50);
		}
		return array;
	}

	public static byte[] ReadConnectionIDExtension(byte[] extensionData)
	{
		return TlsUtilities.DecodeOpaque8(extensionData);
	}

	public static byte[] ReadCookieExtension(byte[] extensionData)
	{
		return TlsUtilities.DecodeOpaque16(extensionData, 1);
	}

	public static bool ReadEarlyDataIndication(byte[] extensionData)
	{
		return ReadEmptyExtensionData(extensionData);
	}

	public static long ReadEarlyDataMaxSize(byte[] extensionData)
	{
		return TlsUtilities.DecodeUint32(extensionData);
	}

	public static bool ReadEncryptThenMacExtension(byte[] extensionData)
	{
		return ReadEmptyExtensionData(extensionData);
	}

	public static bool ReadExtendedMasterSecretExtension(byte[] extensionData)
	{
		return ReadEmptyExtensionData(extensionData);
	}

	public static HeartbeatExtension ReadHeartbeatExtension(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		HeartbeatExtension result = HeartbeatExtension.Parse(memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		return result;
	}

	public static IList<KeyShareEntry> ReadKeyShareClientHello(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		if (TlsUtilities.ReadUint16(memoryStream) != extensionData.Length - 2)
		{
			throw new TlsFatalAlert(50);
		}
		List<KeyShareEntry> list = new List<KeyShareEntry>();
		while (memoryStream.Position < memoryStream.Length)
		{
			KeyShareEntry item = KeyShareEntry.Parse(memoryStream);
			list.Add(item);
		}
		return list;
	}

	public static int ReadKeyShareHelloRetryRequest(byte[] extensionData)
	{
		return TlsUtilities.DecodeUint16(extensionData);
	}

	public static KeyShareEntry ReadKeyShareServerHello(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		KeyShareEntry result = KeyShareEntry.Parse(memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		return result;
	}

	public static short ReadMaxFragmentLengthExtension(byte[] extensionData)
	{
		return TlsUtilities.DecodeUint8(extensionData);
	}

	public static IDictionary<DerObjectIdentifier, byte[]> ReadOidFiltersExtension(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		if (extensionData.Length < 2)
		{
			throw new TlsFatalAlert(50);
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		if (TlsUtilities.ReadUint16(memoryStream) != extensionData.Length - 2)
		{
			throw new TlsFatalAlert(50);
		}
		Dictionary<DerObjectIdentifier, byte[]> dictionary = new Dictionary<DerObjectIdentifier, byte[]>();
		while (memoryStream.Position < memoryStream.Length)
		{
			byte[] encoding = TlsUtilities.ReadOpaque8(memoryStream, 1);
			DerObjectIdentifier instance = DerObjectIdentifier.GetInstance(TlsUtilities.ReadAsn1Object(encoding));
			TlsUtilities.RequireDerEncoding(instance, encoding);
			if (dictionary.ContainsKey(instance))
			{
				throw new TlsFatalAlert(47);
			}
			byte[] value = TlsUtilities.ReadOpaque16(memoryStream);
			dictionary[instance] = value;
		}
		return dictionary;
	}

	public static int ReadPaddingExtension(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		if (!Arrays.AreAllZeroes(extensionData, 0, extensionData.Length))
		{
			throw new TlsFatalAlert(47);
		}
		return extensionData.Length;
	}

	public static bool ReadPostHandshakeAuthExtension(byte[] extensionData)
	{
		return ReadEmptyExtensionData(extensionData);
	}

	public static OfferedPsks ReadPreSharedKeyClientHello(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		OfferedPsks result = OfferedPsks.Parse(memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		return result;
	}

	public static int ReadPreSharedKeyServerHello(byte[] extensionData)
	{
		return TlsUtilities.DecodeUint16(extensionData);
	}

	public static short[] ReadPskKeyExchangeModesExtension(byte[] extensionData)
	{
		short[] array = TlsUtilities.DecodeUint8ArrayWithUint8Length(extensionData);
		if (array.Length < 1)
		{
			throw new TlsFatalAlert(50);
		}
		return array;
	}

	public static int ReadRecordSizeLimitExtension(byte[] extensionData)
	{
		int num = TlsUtilities.DecodeUint16(extensionData);
		if (num < 64)
		{
			throw new TlsFatalAlert(47);
		}
		return num;
	}

	public static IList<ServerName> ReadServerNameExtensionClient(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		ServerNameList serverNameList = ServerNameList.Parse(memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		return serverNameList.ServerNames;
	}

	public static bool ReadServerNameExtensionServer(byte[] extensionData)
	{
		return ReadEmptyExtensionData(extensionData);
	}

	public static IList<SignatureAndHashAlgorithm> ReadSignatureAlgorithmsExtension(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		IList<SignatureAndHashAlgorithm> result = TlsUtilities.ParseSupportedSignatureAlgorithms(memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		return result;
	}

	public static IList<SignatureAndHashAlgorithm> ReadSignatureAlgorithmsCertExtension(byte[] extensionData)
	{
		return ReadSignatureAlgorithmsExtension(extensionData);
	}

	public static CertificateStatusRequest ReadStatusRequestExtension(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		CertificateStatusRequest result = CertificateStatusRequest.Parse(memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		return result;
	}

	public static IList<CertificateStatusRequestItemV2> ReadStatusRequestV2Extension(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		if (extensionData.Length < 3)
		{
			throw new TlsFatalAlert(50);
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		if (TlsUtilities.ReadUint16(memoryStream) != extensionData.Length - 2)
		{
			throw new TlsFatalAlert(50);
		}
		List<CertificateStatusRequestItemV2> list = new List<CertificateStatusRequestItemV2>();
		while (memoryStream.Position < memoryStream.Length)
		{
			CertificateStatusRequestItemV2 item = CertificateStatusRequestItemV2.Parse(memoryStream);
			list.Add(item);
		}
		return list;
	}

	public static int[] ReadSupportedGroupsExtension(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		int num = TlsUtilities.ReadUint16(memoryStream);
		if (num < 2 || (num & 1) != 0)
		{
			throw new TlsFatalAlert(50);
		}
		int[] result = TlsUtilities.ReadUint16Array(num / 2, memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		return result;
	}

	public static short[] ReadSupportedPointFormatsExtension(byte[] extensionData)
	{
		short[] array = TlsUtilities.DecodeUint8ArrayWithUint8Length(extensionData);
		if (!Arrays.Contains(array, 0))
		{
			throw new TlsFatalAlert(47);
		}
		return array;
	}

	public static ProtocolVersion[] ReadSupportedVersionsExtensionClient(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		if (extensionData.Length < 3 || extensionData.Length > 255 || (extensionData.Length & 1) == 0)
		{
			throw new TlsFatalAlert(50);
		}
		short num = TlsUtilities.ReadUint8(extensionData, 0);
		if (num != extensionData.Length - 1)
		{
			throw new TlsFatalAlert(50);
		}
		int num2 = num / 2;
		ProtocolVersion[] array = new ProtocolVersion[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = TlsUtilities.ReadVersion(extensionData, 1 + i * 2);
		}
		return array;
	}

	public static ProtocolVersion ReadSupportedVersionsExtensionServer(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		if (extensionData.Length != 2)
		{
			throw new TlsFatalAlert(50);
		}
		return TlsUtilities.ReadVersion(extensionData, 0);
	}

	public static bool ReadTruncatedHmacExtension(byte[] extensionData)
	{
		return ReadEmptyExtensionData(extensionData);
	}

	public static IList<TrustedAuthority> ReadTrustedCAKeysExtensionClient(byte[] extensionData)
	{
		if (extensionData == null)
		{
			throw new ArgumentNullException("extensionData");
		}
		if (extensionData.Length < 2)
		{
			throw new TlsFatalAlert(50);
		}
		MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
		if (TlsUtilities.ReadUint16(memoryStream) != extensionData.Length - 2)
		{
			throw new TlsFatalAlert(50);
		}
		List<TrustedAuthority> list = new List<TrustedAuthority>();
		while (memoryStream.Position < memoryStream.Length)
		{
			TrustedAuthority item = TrustedAuthority.Parse(memoryStream);
			list.Add(item);
		}
		return list;
	}

	public static bool ReadTrustedCAKeysExtensionServer(byte[] extensionData)
	{
		return ReadEmptyExtensionData(extensionData);
	}

	private static byte[] PatchOpaque16(MemoryStream buf)
	{
		int i = Convert.ToInt32(buf.Length) - 2;
		TlsUtilities.CheckUint16(i);
		byte[] array = buf.ToArray();
		TlsUtilities.WriteUint16(i, array, 0);
		return array;
	}
}
