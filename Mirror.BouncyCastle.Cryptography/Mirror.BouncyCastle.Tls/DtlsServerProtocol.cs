using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Tls.Crypto;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls;

public class DtlsServerProtocol : DtlsProtocol
{
	protected internal class ServerHandshakeState
	{
		internal TlsServer server;

		internal TlsServerContextImpl serverContext;

		internal TlsSession tlsSession;

		internal SessionParameters sessionParameters;

		internal TlsSecret sessionMasterSecret;

		internal SessionParameters.Builder sessionParametersBuilder;

		internal ClientHello clientHello;

		internal IDictionary<int, byte[]> serverExtensions;

		internal bool expectSessionTicket;

		internal TlsKeyExchange keyExchange;

		internal TlsCredentials serverCredentials;

		internal CertificateRequest certificateRequest;

		internal TlsHeartbeat heartbeat;

		internal short heartbeatPolicy = 2;
	}

	protected bool m_verifyRequests = true;

	public virtual bool VerifyRequests
	{
		get
		{
			return m_verifyRequests;
		}
		set
		{
			m_verifyRequests = value;
		}
	}

	public virtual DtlsTransport Accept(TlsServer server, DatagramTransport transport)
	{
		return Accept(server, transport, null);
	}

	public virtual DtlsTransport Accept(TlsServer server, DatagramTransport transport, DtlsRequest request)
	{
		if (server == null)
		{
			throw new ArgumentNullException("server");
		}
		if (transport == null)
		{
			throw new ArgumentNullException("transport");
		}
		TlsServerContextImpl tlsServerContextImpl = new TlsServerContextImpl(server.Crypto);
		ServerHandshakeState serverHandshakeState = new ServerHandshakeState();
		serverHandshakeState.server = server;
		serverHandshakeState.serverContext = tlsServerContextImpl;
		server.Init(tlsServerContextImpl);
		tlsServerContextImpl.HandshakeBeginning(server);
		SecurityParameters securityParameters = tlsServerContextImpl.SecurityParameters;
		securityParameters.m_extendedPadding = server.ShouldUseExtendedPadding();
		DtlsRecordLayer dtlsRecordLayer = new DtlsRecordLayer(tlsServerContextImpl, server, transport);
		server.NotifyCloseHandle(dtlsRecordLayer);
		try
		{
			return ServerHandshake(serverHandshakeState, dtlsRecordLayer, request);
		}
		catch (TlsFatalAlert tlsFatalAlert)
		{
			AbortServerHandshake(serverHandshakeState, dtlsRecordLayer, tlsFatalAlert.AlertDescription);
			throw;
		}
		catch (IOException)
		{
			AbortServerHandshake(serverHandshakeState, dtlsRecordLayer, 80);
			throw;
		}
		catch (Exception alertCause)
		{
			AbortServerHandshake(serverHandshakeState, dtlsRecordLayer, 80);
			throw new TlsFatalAlert(80, alertCause);
		}
		finally
		{
			securityParameters.Clear();
		}
	}

	internal virtual void AbortServerHandshake(ServerHandshakeState state, DtlsRecordLayer recordLayer, short alertDescription)
	{
		recordLayer.Fail(alertDescription);
		InvalidateSession(state);
	}

	internal virtual DtlsTransport ServerHandshake(ServerHandshakeState state, DtlsRecordLayer recordLayer, DtlsRequest request)
	{
		TlsServer server = state.server;
		TlsServerContextImpl serverContext = state.serverContext;
		SecurityParameters securityParameters = serverContext.SecurityParameters;
		DtlsReliableHandshake dtlsReliableHandshake = new DtlsReliableHandshake(serverContext, recordLayer, server.GetHandshakeTimeoutMillis(), TlsUtilities.GetHandshakeResendTimeMillis(server), request);
		DtlsReliableHandshake.Message message = null;
		if (request == null)
		{
			message = dtlsReliableHandshake.ReceiveMessage();
			if (message.Type != 1)
			{
				throw new TlsFatalAlert(10);
			}
			ProcessClientHello(state, message.Body);
			message = null;
		}
		else
		{
			ProcessClientHello(state, request.ClientHello);
			request = null;
		}
		byte[] body = GenerateServerHello(state, recordLayer);
		ProtocolVersion writeVersion = (recordLayer.ReadVersion = serverContext.ServerVersion);
		recordLayer.SetWriteVersion(writeVersion);
		dtlsReliableHandshake.SendMessage(2, body);
		dtlsReliableHandshake.HandshakeHash.NotifyPrfDetermined();
		if (securityParameters.IsResumedSession)
		{
			securityParameters.m_masterSecret = state.sessionMasterSecret;
			recordLayer.InitPendingEpoch(TlsUtilities.InitCipher(serverContext));
			securityParameters.m_localVerifyData = TlsUtilities.CalculateVerifyData(serverContext, dtlsReliableHandshake.HandshakeHash, isServer: true);
			dtlsReliableHandshake.SendMessage(20, securityParameters.LocalVerifyData);
			securityParameters.m_peerVerifyData = TlsUtilities.CalculateVerifyData(serverContext, dtlsReliableHandshake.HandshakeHash, isServer: false);
			ProcessFinished(dtlsReliableHandshake.ReceiveMessageBody(20), securityParameters.PeerVerifyData);
			dtlsReliableHandshake.Finish();
			if (securityParameters.IsExtendedMasterSecret)
			{
				securityParameters.m_tlsUnique = securityParameters.LocalVerifyData;
			}
			securityParameters.m_localCertificate = state.sessionParameters.LocalCertificate;
			securityParameters.m_peerCertificate = state.sessionParameters.PeerCertificate;
			securityParameters.m_pskIdentity = state.sessionParameters.PskIdentity;
			securityParameters.m_srpIdentity = state.sessionParameters.SrpIdentity;
			serverContext.HandshakeComplete(server, state.tlsSession);
			recordLayer.InitHeartbeat(state.heartbeat, 1 == state.heartbeatPolicy);
			return new DtlsTransport(recordLayer, server.IgnoreCorruptDtlsRecords);
		}
		IList<SupplementalDataEntry> serverSupplementalData = server.GetServerSupplementalData();
		if (serverSupplementalData != null)
		{
			byte[] body2 = DtlsProtocol.GenerateSupplementalData(serverSupplementalData);
			dtlsReliableHandshake.SendMessage(23, body2);
		}
		state.keyExchange = TlsUtilities.InitKeyExchangeServer(serverContext, server);
		state.serverCredentials = null;
		if (!KeyExchangeAlgorithm.IsAnonymous(securityParameters.KeyExchangeAlgorithm))
		{
			state.serverCredentials = TlsUtilities.EstablishServerCredentials(server);
		}
		Certificate certificate = null;
		MemoryStream memoryStream = new MemoryStream();
		if (state.serverCredentials == null)
		{
			state.keyExchange.SkipServerCredentials();
		}
		else
		{
			state.keyExchange.ProcessServerCredentials(state.serverCredentials);
			certificate = state.serverCredentials.Certificate;
			DtlsProtocol.SendCertificateMessage(serverContext, dtlsReliableHandshake, certificate, memoryStream);
		}
		securityParameters.m_tlsServerEndPoint = memoryStream.ToArray();
		if (certificate == null || certificate.IsEmpty)
		{
			securityParameters.m_statusRequestVersion = 0;
		}
		if (securityParameters.StatusRequestVersion > 0)
		{
			CertificateStatus certificateStatus = server.GetCertificateStatus();
			if (certificateStatus != null)
			{
				byte[] body3 = GenerateCertificateStatus(state, certificateStatus);
				dtlsReliableHandshake.SendMessage(22, body3);
			}
		}
		byte[] array = state.keyExchange.GenerateServerKeyExchange();
		if (array != null)
		{
			dtlsReliableHandshake.SendMessage(12, array);
		}
		if (state.serverCredentials != null)
		{
			state.certificateRequest = server.GetCertificateRequest();
			if (state.certificateRequest == null)
			{
				if (!state.keyExchange.RequiresCertificateVerify)
				{
					throw new TlsFatalAlert(80);
				}
			}
			else
			{
				if (TlsUtilities.IsTlsV12(serverContext) != (state.certificateRequest.SupportedSignatureAlgorithms != null))
				{
					throw new TlsFatalAlert(80);
				}
				state.certificateRequest = TlsUtilities.ValidateCertificateRequest(state.certificateRequest, state.keyExchange);
				TlsUtilities.EstablishServerSigAlgs(securityParameters, state.certificateRequest);
				if (ProtocolVersion.DTLSv12.Equals(securityParameters.NegotiatedVersion))
				{
					TlsUtilities.TrackHashAlgorithms(dtlsReliableHandshake.HandshakeHash, securityParameters.ServerSigAlgs);
					if (serverContext.Crypto.HasAnyStreamVerifiers(securityParameters.ServerSigAlgs))
					{
						dtlsReliableHandshake.HandshakeHash.ForceBuffering();
					}
				}
				else if (serverContext.Crypto.HasAnyStreamVerifiersLegacy(state.certificateRequest.CertificateTypes))
				{
					dtlsReliableHandshake.HandshakeHash.ForceBuffering();
				}
			}
		}
		dtlsReliableHandshake.HandshakeHash.SealHashAlgorithms();
		if (state.certificateRequest != null)
		{
			byte[] body4 = GenerateCertificateRequest(state, state.certificateRequest);
			dtlsReliableHandshake.SendMessage(13, body4);
		}
		dtlsReliableHandshake.SendMessage(14, TlsUtilities.EmptyBytes);
		message = dtlsReliableHandshake.ReceiveMessage();
		if (message.Type == 23)
		{
			ProcessClientSupplementalData(state, message.Body);
			message = dtlsReliableHandshake.ReceiveMessage();
		}
		else
		{
			server.ProcessClientSupplementalData(null);
		}
		if (state.certificateRequest == null)
		{
			state.keyExchange.SkipClientCredentials();
		}
		else if (message.Type == 11)
		{
			ProcessClientCertificate(state, message.Body);
			message = dtlsReliableHandshake.ReceiveMessage();
		}
		else
		{
			if (TlsUtilities.IsTlsV12(serverContext))
			{
				throw new TlsFatalAlert(10);
			}
			NotifyClientCertificate(state, Certificate.EmptyChain);
		}
		if (message.Type == 16)
		{
			ProcessClientKeyExchange(state, message.Body);
			securityParameters.m_sessionHash = TlsUtilities.GetCurrentPrfHash(dtlsReliableHandshake.HandshakeHash);
			TlsProtocol.EstablishMasterSecret(serverContext, state.keyExchange);
			recordLayer.InitPendingEpoch(TlsUtilities.InitCipher(serverContext));
			if (ExpectCertificateVerifyMessage(state))
			{
				message = dtlsReliableHandshake.ReceiveMessageDelayedDigest(15);
				byte[] body5 = message.Body;
				ProcessCertificateVerify(state, body5, dtlsReliableHandshake.HandshakeHash);
				dtlsReliableHandshake.PrepareToFinish();
				dtlsReliableHandshake.UpdateHandshakeMessagesDigest(message);
			}
			else
			{
				dtlsReliableHandshake.PrepareToFinish();
			}
			message = null;
			securityParameters.m_peerVerifyData = TlsUtilities.CalculateVerifyData(serverContext, dtlsReliableHandshake.HandshakeHash, isServer: false);
			ProcessFinished(dtlsReliableHandshake.ReceiveMessageBody(20), securityParameters.PeerVerifyData);
			if (state.expectSessionTicket)
			{
				NewSessionTicket newSessionTicket = server.GetNewSessionTicket();
				byte[] body6 = GenerateNewSessionTicket(state, newSessionTicket);
				dtlsReliableHandshake.SendMessage(4, body6);
			}
			securityParameters.m_localVerifyData = TlsUtilities.CalculateVerifyData(serverContext, dtlsReliableHandshake.HandshakeHash, isServer: true);
			dtlsReliableHandshake.SendMessage(20, securityParameters.LocalVerifyData);
			dtlsReliableHandshake.Finish();
			state.sessionMasterSecret = securityParameters.MasterSecret;
			state.sessionParameters = new SessionParameters.Builder().SetCipherSuite(securityParameters.CipherSuite).SetExtendedMasterSecret(securityParameters.IsExtendedMasterSecret).SetLocalCertificate(securityParameters.LocalCertificate)
				.SetMasterSecret(serverContext.Crypto.AdoptSecret(state.sessionMasterSecret))
				.SetNegotiatedVersion(securityParameters.NegotiatedVersion)
				.SetPeerCertificate(securityParameters.PeerCertificate)
				.SetPskIdentity(securityParameters.PskIdentity)
				.SetSrpIdentity(securityParameters.SrpIdentity)
				.SetServerExtensions(state.serverExtensions)
				.Build();
			state.tlsSession = TlsUtilities.ImportSession(securityParameters.SessionID, state.sessionParameters);
			securityParameters.m_tlsUnique = securityParameters.PeerVerifyData;
			serverContext.HandshakeComplete(server, state.tlsSession);
			recordLayer.InitHeartbeat(state.heartbeat, 1 == state.heartbeatPolicy);
			return new DtlsTransport(recordLayer, server.IgnoreCorruptDtlsRecords);
		}
		throw new TlsFatalAlert(10);
	}

	protected virtual byte[] GenerateCertificateRequest(ServerHandshakeState state, CertificateRequest certificateRequest)
	{
		MemoryStream memoryStream = new MemoryStream();
		certificateRequest.Encode(state.serverContext, memoryStream);
		return memoryStream.ToArray();
	}

	protected virtual byte[] GenerateCertificateStatus(ServerHandshakeState state, CertificateStatus certificateStatus)
	{
		MemoryStream memoryStream = new MemoryStream();
		certificateStatus.Encode(memoryStream);
		return memoryStream.ToArray();
	}

	protected virtual byte[] GenerateNewSessionTicket(ServerHandshakeState state, NewSessionTicket newSessionTicket)
	{
		MemoryStream memoryStream = new MemoryStream();
		newSessionTicket.Encode(memoryStream);
		return memoryStream.ToArray();
	}

	internal virtual byte[] GenerateServerHello(ServerHandshakeState state, DtlsRecordLayer recordLayer)
	{
		TlsServer server = state.server;
		TlsServerContextImpl serverContext = state.serverContext;
		SecurityParameters securityParameters = serverContext.SecurityParameters;
		ProtocolVersion serverVersion = server.GetServerVersion();
		if (!ProtocolVersion.Contains(serverContext.ClientSupportedVersions, serverVersion))
		{
			throw new TlsFatalAlert(80);
		}
		securityParameters.m_negotiatedVersion = serverVersion;
		bool useGmtUnixTime = server.ShouldUseGmtUnixTime();
		securityParameters.m_serverRandom = TlsProtocol.CreateRandomBlock(useGmtUnixTime, serverContext);
		if (!serverVersion.Equals(ProtocolVersion.GetLatestDtls(server.GetProtocolVersions())))
		{
			TlsUtilities.WriteDowngradeMarker(serverVersion, securityParameters.ServerRandom);
		}
		IDictionary<int, byte[]> extensions = state.clientHello.Extensions;
		TlsSession sessionToResume = server.GetSessionToResume(state.clientHello.SessionID);
		bool flag = EstablishSession(state, sessionToResume);
		if (flag && !serverVersion.Equals(state.sessionParameters.NegotiatedVersion))
		{
			flag = false;
		}
		bool flag2 = false;
		if (TlsUtilities.IsExtendedMasterSecretOptional(serverVersion) && server.ShouldUseExtendedMasterSecret())
		{
			if (TlsExtensionsUtilities.HasExtendedMasterSecretExtension(extensions))
			{
				flag2 = true;
			}
			else
			{
				if (server.RequiresExtendedMasterSecret())
				{
					throw new TlsFatalAlert(40, "Extended Master Secret extension is required");
				}
				if (flag)
				{
					if (state.sessionParameters.IsExtendedMasterSecret)
					{
						throw new TlsFatalAlert(40, "Extended Master Secret extension is required for EMS session resumption");
					}
					if (!server.AllowLegacyResumption())
					{
						throw new TlsFatalAlert(40, "Extended Master Secret extension is required for legacy session resumption");
					}
				}
			}
		}
		if (flag && flag2 != state.sessionParameters.IsExtendedMasterSecret)
		{
			flag = false;
		}
		securityParameters.m_extendedMasterSecret = flag2;
		if (!flag)
		{
			CancelSession(state);
			byte[] array = server.GetNewSessionID();
			if (array == null)
			{
				array = TlsUtilities.EmptyBytes;
			}
			state.tlsSession = TlsUtilities.ImportSession(array, null);
		}
		securityParameters.m_resumedSession = flag;
		securityParameters.m_sessionID = state.tlsSession.SessionID;
		server.NotifySession(state.tlsSession);
		TlsUtilities.NegotiatedVersionDtlsServer(serverContext);
		int cipherSuite = DtlsProtocol.ValidateSelectedCipherSuite(server.GetSelectedCipherSuite(), 80);
		if (!TlsUtilities.IsValidCipherSuiteSelection(state.clientHello.CipherSuites, cipherSuite) || !TlsUtilities.IsValidVersionForCipherSuite(cipherSuite, securityParameters.NegotiatedVersion))
		{
			throw new TlsFatalAlert(80);
		}
		TlsUtilities.NegotiatedCipherSuite(securityParameters, cipherSuite);
		IDictionary<int, byte[]> extensions2 = (flag ? state.sessionParameters.ReadServerExtensions() : server.GetServerExtensions());
		state.serverExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(extensions2);
		server.GetServerExtensionsForConnection(state.serverExtensions);
		if (securityParameters.IsSecureRenegotiation)
		{
			byte[] extensionData = TlsUtilities.GetExtensionData(state.serverExtensions, 65281);
			if (extensionData == null)
			{
				state.serverExtensions[65281] = TlsProtocol.CreateRenegotiationInfo(TlsUtilities.EmptyBytes);
			}
		}
		if (securityParameters.IsExtendedMasterSecret)
		{
			TlsExtensionsUtilities.AddExtendedMasterSecretExtension(state.serverExtensions);
		}
		else
		{
			state.serverExtensions.Remove(23);
		}
		if (state.heartbeat != null || 1 == state.heartbeatPolicy)
		{
			TlsExtensionsUtilities.AddHeartbeatExtension(state.serverExtensions, new HeartbeatExtension(state.heartbeatPolicy));
		}
		securityParameters.m_applicationProtocol = TlsExtensionsUtilities.GetAlpnExtensionServer(state.serverExtensions);
		securityParameters.m_applicationProtocolSet = true;
		if (ProtocolVersion.DTLSv12.Equals(securityParameters.NegotiatedVersion))
		{
			byte[] connectionIDExtension = TlsExtensionsUtilities.GetConnectionIDExtension(state.serverExtensions);
			if (connectionIDExtension != null)
			{
				byte[] connectionIDLocal = TlsExtensionsUtilities.GetConnectionIDExtension(extensions) ?? throw new TlsFatalAlert(80);
				securityParameters.m_connectionIDLocal = connectionIDLocal;
				securityParameters.m_connectionIDPeer = connectionIDExtension;
			}
		}
		if (state.serverExtensions.Count > 0)
		{
			securityParameters.m_encryptThenMac = TlsExtensionsUtilities.HasEncryptThenMacExtension(state.serverExtensions);
			securityParameters.m_maxFragmentLength = TlsUtilities.ProcessMaxFragmentLengthExtension(flag ? null : extensions, state.serverExtensions, 80);
			securityParameters.m_truncatedHmac = TlsExtensionsUtilities.HasTruncatedHmacExtension(state.serverExtensions);
			if (!flag)
			{
				if (TlsUtilities.HasExpectedEmptyExtensionData(state.serverExtensions, 17, 80))
				{
					securityParameters.m_statusRequestVersion = 2;
				}
				else if (TlsUtilities.HasExpectedEmptyExtensionData(state.serverExtensions, 5, 80))
				{
					securityParameters.m_statusRequestVersion = 1;
				}
				securityParameters.m_clientCertificateType = TlsUtilities.ProcessClientCertificateTypeExtension(extensions, state.serverExtensions, 80);
				securityParameters.m_serverCertificateType = TlsUtilities.ProcessServerCertificateTypeExtension(extensions, state.serverExtensions, 80);
				state.expectSessionTicket = TlsUtilities.HasExpectedEmptyExtensionData(state.serverExtensions, 35, 80);
			}
		}
		ServerHello serverHello = new ServerHello(serverVersion, securityParameters.ServerRandom, securityParameters.SessionID, securityParameters.CipherSuite, state.serverExtensions);
		state.clientHello = null;
		DtlsProtocol.ApplyMaxFragmentLengthExtension(recordLayer, securityParameters.MaxFragmentLength);
		MemoryStream memoryStream = new MemoryStream();
		serverHello.Encode(serverContext, memoryStream);
		return memoryStream.ToArray();
	}

	protected virtual void CancelSession(ServerHandshakeState state)
	{
		if (state.sessionMasterSecret != null)
		{
			state.sessionMasterSecret.Destroy();
			state.sessionMasterSecret = null;
		}
		if (state.sessionParameters != null)
		{
			state.sessionParameters.Clear();
			state.sessionParameters = null;
		}
		state.tlsSession = null;
	}

	protected virtual bool EstablishSession(ServerHandshakeState state, TlsSession sessionToResume)
	{
		state.tlsSession = null;
		state.sessionParameters = null;
		state.sessionMasterSecret = null;
		if (sessionToResume == null || !sessionToResume.IsResumable)
		{
			return false;
		}
		SessionParameters sessionParameters = sessionToResume.ExportSessionParameters();
		if (sessionParameters == null)
		{
			return false;
		}
		ProtocolVersion negotiatedVersion = sessionParameters.NegotiatedVersion;
		if (negotiatedVersion == null || !negotiatedVersion.IsDtls)
		{
			return false;
		}
		if (!sessionParameters.IsExtendedMasterSecret && !TlsUtilities.IsExtendedMasterSecretOptional(negotiatedVersion))
		{
			return false;
		}
		TlsSecret sessionMasterSecret = TlsUtilities.GetSessionMasterSecret(state.serverContext.Crypto, sessionParameters.MasterSecret);
		if (sessionMasterSecret == null)
		{
			return false;
		}
		state.tlsSession = sessionToResume;
		state.sessionParameters = sessionParameters;
		state.sessionMasterSecret = sessionMasterSecret;
		return true;
	}

	protected virtual void InvalidateSession(ServerHandshakeState state)
	{
		if (state.tlsSession != null)
		{
			state.tlsSession.Invalidate();
		}
		CancelSession(state);
	}

	protected virtual void NotifyClientCertificate(ServerHandshakeState state, Certificate clientCertificate)
	{
		if (state.certificateRequest == null)
		{
			throw new TlsFatalAlert(80);
		}
		TlsUtilities.ProcessClientCertificate(state.serverContext, clientCertificate, state.keyExchange, state.server);
	}

	protected virtual void ProcessClientCertificate(ServerHandshakeState state, byte[] body)
	{
		MemoryStream memoryStream = new MemoryStream(body, writable: false);
		Certificate clientCertificate = Certificate.Parse(new Certificate.ParseOptions
		{
			CertificateType = state.serverContext.SecurityParameters.ClientCertificateType,
			MaxChainLength = state.server.GetMaxCertificateChainLength()
		}, state.serverContext, memoryStream, null);
		TlsProtocol.AssertEmpty(memoryStream);
		NotifyClientCertificate(state, clientCertificate);
	}

	protected virtual void ProcessCertificateVerify(ServerHandshakeState state, byte[] body, TlsHandshakeHash handshakeHash)
	{
		if (state.certificateRequest == null)
		{
			throw new InvalidOperationException();
		}
		MemoryStream memoryStream = new MemoryStream(body, writable: false);
		TlsServerContextImpl serverContext = state.serverContext;
		DigitallySigned certificateVerify = DigitallySigned.Parse(serverContext, memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		TlsUtilities.VerifyCertificateVerifyClient(serverContext, state.certificateRequest, certificateVerify, handshakeHash);
	}

	protected virtual void ProcessClientHello(ServerHandshakeState state, byte[] body)
	{
		ClientHello clientHello = ClientHello.Parse(new MemoryStream(body, writable: false), Stream.Null);
		ProcessClientHello(state, clientHello);
	}

	protected virtual void ProcessClientHello(ServerHandshakeState state, ClientHello clientHello)
	{
		state.clientHello = clientHello;
		ProtocolVersion version = clientHello.Version;
		int[] cipherSuites = clientHello.CipherSuites;
		IDictionary<int, byte[]> extensions = clientHello.Extensions;
		TlsServer server = state.server;
		TlsServerContextImpl serverContext = state.serverContext;
		SecurityParameters securityParameters = serverContext.SecurityParameters;
		if (!version.IsDtls)
		{
			throw new TlsFatalAlert(47);
		}
		serverContext.SetRsaPreMasterSecretVersion(version);
		serverContext.SetClientSupportedVersions(TlsExtensionsUtilities.GetSupportedVersionsExtensionClient(extensions));
		ProtocolVersion protocolVersion = version;
		if (serverContext.ClientSupportedVersions == null)
		{
			if (protocolVersion.IsLaterVersionOf(ProtocolVersion.DTLSv12))
			{
				protocolVersion = ProtocolVersion.DTLSv12;
			}
			serverContext.SetClientSupportedVersions(protocolVersion.DownTo(ProtocolVersion.DTLSv10));
		}
		else
		{
			protocolVersion = ProtocolVersion.GetLatestDtls(serverContext.ClientSupportedVersions);
		}
		if (!ProtocolVersion.SERVER_EARLIEST_SUPPORTED_DTLS.IsEqualOrEarlierVersionOf(protocolVersion))
		{
			throw new TlsFatalAlert(70);
		}
		serverContext.SetClientVersion(protocolVersion);
		server.NotifyClientVersion(serverContext.ClientVersion);
		securityParameters.m_clientRandom = clientHello.Random;
		server.NotifyFallback(Arrays.Contains(cipherSuites, 22016));
		server.NotifyOfferedCipherSuites(cipherSuites);
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 65281);
		if (Arrays.Contains(cipherSuites, 255))
		{
			securityParameters.m_secureRenegotiation = true;
		}
		if (extensionData != null)
		{
			securityParameters.m_secureRenegotiation = true;
			if (!Arrays.FixedTimeEquals(extensionData, TlsProtocol.CreateRenegotiationInfo(TlsUtilities.EmptyBytes)))
			{
				throw new TlsFatalAlert(40);
			}
		}
		server.NotifySecureRenegotiation(securityParameters.IsSecureRenegotiation);
		if (extensions == null)
		{
			return;
		}
		TlsExtensionsUtilities.GetPaddingExtension(extensions);
		securityParameters.m_clientServerNames = TlsExtensionsUtilities.GetServerNameExtensionClient(extensions);
		if (TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(protocolVersion))
		{
			TlsUtilities.EstablishClientSigAlgs(securityParameters, extensions);
		}
		securityParameters.m_clientSupportedGroups = TlsExtensionsUtilities.GetSupportedGroupsExtension(extensions);
		HeartbeatExtension heartbeatExtension = TlsExtensionsUtilities.GetHeartbeatExtension(extensions);
		if (heartbeatExtension != null)
		{
			if (1 == heartbeatExtension.Mode)
			{
				state.heartbeat = server.GetHeartbeat();
			}
			state.heartbeatPolicy = server.GetHeartbeatPolicy();
		}
		server.ProcessClientExtensions(extensions);
	}

	protected virtual void ProcessClientKeyExchange(ServerHandshakeState state, byte[] body)
	{
		MemoryStream memoryStream = new MemoryStream(body, writable: false);
		state.keyExchange.ProcessClientKeyExchange(memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
	}

	protected virtual void ProcessClientSupplementalData(ServerHandshakeState state, byte[] body)
	{
		IList<SupplementalDataEntry> clientSupplementalData = TlsProtocol.ReadSupplementalDataMessage(new MemoryStream(body, writable: false));
		state.server.ProcessClientSupplementalData(clientSupplementalData);
	}

	protected virtual bool ExpectCertificateVerifyMessage(ServerHandshakeState state)
	{
		if (state.certificateRequest == null)
		{
			return false;
		}
		Certificate peerCertificate = state.serverContext.SecurityParameters.PeerCertificate;
		if (peerCertificate != null && !peerCertificate.IsEmpty)
		{
			if (state.keyExchange != null)
			{
				return state.keyExchange.RequiresCertificateVerify;
			}
			return true;
		}
		return false;
	}
}
