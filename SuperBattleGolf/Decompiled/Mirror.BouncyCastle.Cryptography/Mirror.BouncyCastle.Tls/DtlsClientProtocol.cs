using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Tls.Crypto;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls;

public class DtlsClientProtocol : DtlsProtocol
{
	protected internal class ClientHandshakeState
	{
		internal TlsClient client;

		internal TlsClientContextImpl clientContext;

		internal TlsSession tlsSession;

		internal SessionParameters sessionParameters;

		internal TlsSecret sessionMasterSecret;

		internal SessionParameters.Builder sessionParametersBuilder;

		internal int[] offeredCipherSuites;

		internal IDictionary<int, byte[]> clientExtensions;

		internal IDictionary<int, byte[]> serverExtensions;

		internal bool expectSessionTicket;

		internal IDictionary<int, TlsAgreement> clientAgreements;

		internal OfferedPsks.BindersConfig clientBinders;

		internal TlsKeyExchange keyExchange;

		internal TlsAuthentication authentication;

		internal CertificateStatus certificateStatus;

		internal CertificateRequest certificateRequest;

		internal TlsHeartbeat heartbeat;

		internal short heartbeatPolicy = 2;
	}

	public virtual DtlsTransport Connect(TlsClient client, DatagramTransport transport)
	{
		if (client == null)
		{
			throw new ArgumentNullException("client");
		}
		if (transport == null)
		{
			throw new ArgumentNullException("transport");
		}
		TlsClientContextImpl tlsClientContextImpl = new TlsClientContextImpl(client.Crypto);
		ClientHandshakeState clientHandshakeState = new ClientHandshakeState();
		clientHandshakeState.client = client;
		clientHandshakeState.clientContext = tlsClientContextImpl;
		client.Init(tlsClientContextImpl);
		tlsClientContextImpl.HandshakeBeginning(client);
		SecurityParameters securityParameters = tlsClientContextImpl.SecurityParameters;
		securityParameters.m_extendedPadding = client.ShouldUseExtendedPadding();
		DtlsRecordLayer dtlsRecordLayer = new DtlsRecordLayer(tlsClientContextImpl, client, transport);
		client.NotifyCloseHandle(dtlsRecordLayer);
		try
		{
			return ClientHandshake(clientHandshakeState, dtlsRecordLayer);
		}
		catch (TlsFatalAlert tlsFatalAlert)
		{
			AbortClientHandshake(clientHandshakeState, dtlsRecordLayer, tlsFatalAlert.AlertDescription);
			throw;
		}
		catch (IOException)
		{
			AbortClientHandshake(clientHandshakeState, dtlsRecordLayer, 80);
			throw;
		}
		catch (Exception alertCause)
		{
			AbortClientHandshake(clientHandshakeState, dtlsRecordLayer, 80);
			throw new TlsFatalAlert(80, alertCause);
		}
		finally
		{
			securityParameters.Clear();
		}
	}

	internal virtual void AbortClientHandshake(ClientHandshakeState state, DtlsRecordLayer recordLayer, short alertDescription)
	{
		recordLayer.Fail(alertDescription);
		InvalidateSession(state);
	}

	internal virtual DtlsTransport ClientHandshake(ClientHandshakeState state, DtlsRecordLayer recordLayer)
	{
		TlsClient client = state.client;
		TlsClientContextImpl clientContext = state.clientContext;
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		DtlsReliableHandshake dtlsReliableHandshake = new DtlsReliableHandshake(clientContext, recordLayer, client.GetHandshakeTimeoutMillis(), TlsUtilities.GetHandshakeResendTimeMillis(client), null);
		byte[] array = GenerateClientHello(state);
		recordLayer.SetWriteVersion(ProtocolVersion.DTLSv10);
		dtlsReliableHandshake.SendMessage(1, array);
		DtlsReliableHandshake.Message message = dtlsReliableHandshake.ReceiveMessage();
		while (message.Type == 3)
		{
			byte[] cookie = ProcessHelloVerifyRequest(state, message.Body);
			byte[] body = PatchClientHelloWithCookie(array, cookie);
			dtlsReliableHandshake.ResetAfterHelloVerifyRequestClient();
			dtlsReliableHandshake.SendMessage(1, body);
			message = dtlsReliableHandshake.ReceiveMessage();
		}
		if (message.Type == 2)
		{
			ProtocolVersion readVersion = recordLayer.ReadVersion;
			ReportServerVersion(state, readVersion);
			recordLayer.SetWriteVersion(readVersion);
			ProcessServerHello(state, message.Body);
			DtlsProtocol.ApplyMaxFragmentLengthExtension(recordLayer, securityParameters.MaxFragmentLength);
			dtlsReliableHandshake.HandshakeHash.NotifyPrfDetermined();
			if (securityParameters.IsResumedSession)
			{
				securityParameters.m_masterSecret = state.sessionMasterSecret;
				recordLayer.InitPendingEpoch(TlsUtilities.InitCipher(clientContext));
				securityParameters.m_peerVerifyData = TlsUtilities.CalculateVerifyData(clientContext, dtlsReliableHandshake.HandshakeHash, isServer: true);
				ProcessFinished(dtlsReliableHandshake.ReceiveMessageBody(20), securityParameters.PeerVerifyData);
				securityParameters.m_localVerifyData = TlsUtilities.CalculateVerifyData(clientContext, dtlsReliableHandshake.HandshakeHash, isServer: false);
				dtlsReliableHandshake.SendMessage(20, securityParameters.LocalVerifyData);
				dtlsReliableHandshake.Finish();
				if (securityParameters.IsExtendedMasterSecret)
				{
					securityParameters.m_tlsUnique = securityParameters.PeerVerifyData;
				}
				securityParameters.m_localCertificate = state.sessionParameters.LocalCertificate;
				securityParameters.m_peerCertificate = state.sessionParameters.PeerCertificate;
				securityParameters.m_pskIdentity = state.sessionParameters.PskIdentity;
				securityParameters.m_srpIdentity = state.sessionParameters.SrpIdentity;
				clientContext.HandshakeComplete(client, state.tlsSession);
				recordLayer.InitHeartbeat(state.heartbeat, 1 == state.heartbeatPolicy);
				return new DtlsTransport(recordLayer, client.IgnoreCorruptDtlsRecords);
			}
			InvalidateSession(state);
			state.tlsSession = TlsUtilities.ImportSession(securityParameters.SessionID, null);
			message = dtlsReliableHandshake.ReceiveMessage();
			if (message.Type == 23)
			{
				ProcessServerSupplementalData(state, message.Body);
				message = dtlsReliableHandshake.ReceiveMessage();
			}
			else
			{
				client.ProcessServerSupplementalData(null);
			}
			state.keyExchange = TlsUtilities.InitKeyExchangeClient(clientContext, client);
			if (message.Type == 11)
			{
				ProcessServerCertificate(state, message.Body);
				message = dtlsReliableHandshake.ReceiveMessage();
			}
			else
			{
				state.authentication = null;
			}
			if (message.Type == 22)
			{
				if (securityParameters.StatusRequestVersion < 1)
				{
					throw new TlsFatalAlert(10);
				}
				ProcessCertificateStatus(state, message.Body);
				message = dtlsReliableHandshake.ReceiveMessage();
			}
			TlsUtilities.ProcessServerCertificate(clientContext, state.certificateStatus, state.keyExchange, state.authentication, state.clientExtensions, state.serverExtensions);
			if (message.Type == 12)
			{
				ProcessServerKeyExchange(state, message.Body);
				message = dtlsReliableHandshake.ReceiveMessage();
			}
			else
			{
				state.keyExchange.SkipServerKeyExchange();
			}
			if (message.Type == 13)
			{
				ProcessCertificateRequest(state, message.Body);
				TlsUtilities.EstablishServerSigAlgs(securityParameters, state.certificateRequest);
				message = dtlsReliableHandshake.ReceiveMessage();
			}
			if (message.Type == 14)
			{
				if (message.Body.Length != 0)
				{
					throw new TlsFatalAlert(50);
				}
				TlsCredentials tlsCredentials = null;
				TlsCredentialedSigner tlsCredentialedSigner = null;
				Certificate certificate = null;
				SignatureAndHashAlgorithm signatureAndHashAlgorithm = null;
				TlsStreamSigner tlsStreamSigner = null;
				if (state.certificateRequest != null)
				{
					tlsCredentials = TlsUtilities.EstablishClientCredentials(state.authentication, state.certificateRequest);
					if (tlsCredentials != null)
					{
						certificate = tlsCredentials.Certificate;
						if (tlsCredentials is TlsCredentialedSigner)
						{
							tlsCredentialedSigner = (TlsCredentialedSigner)tlsCredentials;
							signatureAndHashAlgorithm = TlsUtilities.GetSignatureAndHashAlgorithm(securityParameters.NegotiatedVersion, tlsCredentialedSigner);
							tlsStreamSigner = tlsCredentialedSigner.GetStreamSigner();
							if (ProtocolVersion.DTLSv12.Equals(securityParameters.NegotiatedVersion))
							{
								TlsUtilities.VerifySupportedSignatureAlgorithm(securityParameters.ServerSigAlgs, signatureAndHashAlgorithm, 80);
								if (tlsStreamSigner == null)
								{
									TlsUtilities.TrackHashAlgorithmClient(dtlsReliableHandshake.HandshakeHash, signatureAndHashAlgorithm);
								}
							}
							if (tlsStreamSigner != null)
							{
								dtlsReliableHandshake.HandshakeHash.ForceBuffering();
							}
						}
					}
				}
				dtlsReliableHandshake.HandshakeHash.SealHashAlgorithms();
				if (tlsCredentials == null)
				{
					state.keyExchange.SkipClientCredentials();
				}
				else
				{
					state.keyExchange.ProcessClientCredentials(tlsCredentials);
				}
				IList<SupplementalDataEntry> clientSupplementalData = client.GetClientSupplementalData();
				if (clientSupplementalData != null)
				{
					byte[] body2 = DtlsProtocol.GenerateSupplementalData(clientSupplementalData);
					dtlsReliableHandshake.SendMessage(23, body2);
				}
				if (state.certificateRequest != null)
				{
					DtlsProtocol.SendCertificateMessage(clientContext, dtlsReliableHandshake, certificate, null);
				}
				byte[] body3 = GenerateClientKeyExchange(state);
				dtlsReliableHandshake.SendMessage(16, body3);
				securityParameters.m_sessionHash = TlsUtilities.GetCurrentPrfHash(dtlsReliableHandshake.HandshakeHash);
				TlsProtocol.EstablishMasterSecret(clientContext, state.keyExchange);
				recordLayer.InitPendingEpoch(TlsUtilities.InitCipher(clientContext));
				if (tlsCredentialedSigner != null)
				{
					DigitallySigned certificateVerify = TlsUtilities.GenerateCertificateVerifyClient(clientContext, tlsCredentialedSigner, signatureAndHashAlgorithm, tlsStreamSigner, dtlsReliableHandshake.HandshakeHash);
					byte[] body4 = GenerateCertificateVerify(state, certificateVerify);
					dtlsReliableHandshake.SendMessage(15, body4);
				}
				dtlsReliableHandshake.PrepareToFinish();
				securityParameters.m_localVerifyData = TlsUtilities.CalculateVerifyData(clientContext, dtlsReliableHandshake.HandshakeHash, isServer: false);
				dtlsReliableHandshake.SendMessage(20, securityParameters.LocalVerifyData);
				if (state.expectSessionTicket)
				{
					message = dtlsReliableHandshake.ReceiveMessage();
					if (message.Type != 4)
					{
						throw new TlsFatalAlert(10);
					}
					securityParameters.m_sessionID = TlsUtilities.EmptyBytes;
					InvalidateSession(state);
					state.tlsSession = TlsUtilities.ImportSession(securityParameters.SessionID, null);
					ProcessNewSessionTicket(state, message.Body);
				}
				securityParameters.m_peerVerifyData = TlsUtilities.CalculateVerifyData(clientContext, dtlsReliableHandshake.HandshakeHash, isServer: true);
				ProcessFinished(dtlsReliableHandshake.ReceiveMessageBody(20), securityParameters.PeerVerifyData);
				dtlsReliableHandshake.Finish();
				state.sessionMasterSecret = securityParameters.MasterSecret;
				state.sessionParameters = new SessionParameters.Builder().SetCipherSuite(securityParameters.CipherSuite).SetExtendedMasterSecret(securityParameters.IsExtendedMasterSecret).SetLocalCertificate(securityParameters.LocalCertificate)
					.SetMasterSecret(clientContext.Crypto.AdoptSecret(state.sessionMasterSecret))
					.SetNegotiatedVersion(securityParameters.NegotiatedVersion)
					.SetPeerCertificate(securityParameters.PeerCertificate)
					.SetPskIdentity(securityParameters.PskIdentity)
					.SetSrpIdentity(securityParameters.SrpIdentity)
					.SetServerExtensions(state.serverExtensions)
					.Build();
				state.tlsSession = TlsUtilities.ImportSession(securityParameters.SessionID, state.sessionParameters);
				securityParameters.m_tlsUnique = securityParameters.LocalVerifyData;
				clientContext.HandshakeComplete(client, state.tlsSession);
				recordLayer.InitHeartbeat(state.heartbeat, 1 == state.heartbeatPolicy);
				return new DtlsTransport(recordLayer, client.IgnoreCorruptDtlsRecords);
			}
			throw new TlsFatalAlert(10);
		}
		throw new TlsFatalAlert(10);
	}

	protected virtual byte[] GenerateCertificateVerify(ClientHandshakeState state, DigitallySigned certificateVerify)
	{
		MemoryStream memoryStream = new MemoryStream();
		certificateVerify.Encode(memoryStream);
		return memoryStream.ToArray();
	}

	protected virtual byte[] GenerateClientHello(ClientHandshakeState state)
	{
		TlsClient client = state.client;
		TlsClientContextImpl clientContext = state.clientContext;
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		ProtocolVersion[] protocolVersions = client.GetProtocolVersions();
		ProtocolVersion earliestDtls = ProtocolVersion.GetEarliestDtls(protocolVersions);
		ProtocolVersion latestDtls = ProtocolVersion.GetLatestDtls(protocolVersions);
		if (!ProtocolVersion.IsSupportedDtlsVersionClient(latestDtls))
		{
			throw new TlsFatalAlert(80);
		}
		clientContext.SetClientVersion(latestDtls);
		clientContext.SetClientSupportedVersions(protocolVersions);
		bool num = ProtocolVersion.DTLSv12.IsEqualOrLaterVersionOf(earliestDtls);
		bool flag = ProtocolVersion.DTLSv13.IsEqualOrEarlierVersionOf(latestDtls);
		bool useGmtUnixTime = !flag && client.ShouldUseGmtUnixTime();
		securityParameters.m_clientRandom = TlsProtocol.CreateRandomBlock(useGmtUnixTime, clientContext);
		TlsSession sessionToResume = (num ? client.GetSessionToResume() : null);
		bool num2 = client.IsFallback();
		state.offeredCipherSuites = client.GetCipherSuites();
		state.clientExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(client.GetClientExtensions());
		bool flag2 = client.ShouldUseExtendedMasterSecret();
		EstablishSession(state, sessionToResume);
		byte[] array = TlsUtilities.GetSessionID(state.tlsSession);
		if (array.Length != 0 && !Arrays.Contains(state.offeredCipherSuites, state.sessionParameters.CipherSuite))
		{
			array = TlsUtilities.EmptyBytes;
		}
		ProtocolVersion protocolVersion = null;
		if (array.Length != 0)
		{
			protocolVersion = state.sessionParameters.NegotiatedVersion;
			if (!ProtocolVersion.Contains(protocolVersions, protocolVersion))
			{
				array = TlsUtilities.EmptyBytes;
			}
		}
		if (array.Length != 0 && TlsUtilities.IsExtendedMasterSecretOptional(protocolVersion))
		{
			if (flag2)
			{
				if (!state.sessionParameters.IsExtendedMasterSecret && !client.AllowLegacyResumption())
				{
					array = TlsUtilities.EmptyBytes;
				}
			}
			else if (state.sessionParameters.IsExtendedMasterSecret)
			{
				array = TlsUtilities.EmptyBytes;
			}
		}
		if (array.Length < 1)
		{
			CancelSession(state);
		}
		client.NotifySessionToResume(state.tlsSession);
		ProtocolVersion protocolVersion2 = latestDtls;
		if (flag)
		{
			protocolVersion2 = ProtocolVersion.DTLSv12;
			TlsExtensionsUtilities.AddSupportedVersionsExtensionClient(state.clientExtensions, protocolVersions);
		}
		clientContext.SetRsaPreMasterSecretVersion(protocolVersion2);
		securityParameters.m_clientServerNames = TlsExtensionsUtilities.GetServerNameExtensionClient(state.clientExtensions);
		if (TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(latestDtls))
		{
			TlsUtilities.EstablishClientSigAlgs(securityParameters, state.clientExtensions);
		}
		securityParameters.m_clientSupportedGroups = TlsExtensionsUtilities.GetSupportedGroupsExtension(state.clientExtensions);
		state.clientBinders = null;
		state.clientAgreements = TlsUtilities.AddKeyShareToClientHello(clientContext, client, state.clientExtensions);
		if (flag2 && TlsUtilities.IsExtendedMasterSecretOptional(protocolVersions))
		{
			TlsExtensionsUtilities.AddExtendedMasterSecretExtension(state.clientExtensions);
		}
		else
		{
			state.clientExtensions.Remove(23);
		}
		bool num3 = TlsUtilities.GetExtensionData(state.clientExtensions, 65281) == null;
		bool flag3 = !Arrays.Contains(state.offeredCipherSuites, 255);
		if (num3 && flag3)
		{
			state.offeredCipherSuites = Arrays.Append(state.offeredCipherSuites, 255);
		}
		if (num2 && !Arrays.Contains(state.offeredCipherSuites, 22016))
		{
			state.offeredCipherSuites = Arrays.Append(state.offeredCipherSuites, 22016);
		}
		state.heartbeat = client.GetHeartbeat();
		state.heartbeatPolicy = client.GetHeartbeatPolicy();
		if (state.heartbeat != null || 1 == state.heartbeatPolicy)
		{
			TlsExtensionsUtilities.AddHeartbeatExtension(state.clientExtensions, new HeartbeatExtension(state.heartbeatPolicy));
		}
		int bindersSize = ((state.clientBinders != null) ? state.clientBinders.m_bindersSize : 0);
		ClientHello clientHello = new ClientHello(protocolVersion2, securityParameters.ClientRandom, array, TlsUtilities.EmptyBytes, state.offeredCipherSuites, state.clientExtensions, bindersSize);
		MemoryStream memoryStream = new MemoryStream();
		clientHello.Encode(clientContext, memoryStream);
		return memoryStream.ToArray();
	}

	protected virtual byte[] GenerateClientKeyExchange(ClientHandshakeState state)
	{
		MemoryStream memoryStream = new MemoryStream();
		state.keyExchange.GenerateClientKeyExchange(memoryStream);
		return memoryStream.ToArray();
	}

	protected virtual void CancelSession(ClientHandshakeState state)
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

	protected virtual bool EstablishSession(ClientHandshakeState state, TlsSession sessionToResume)
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
		TlsSecret sessionMasterSecret = TlsUtilities.GetSessionMasterSecret(state.clientContext.Crypto, sessionParameters.MasterSecret);
		if (sessionMasterSecret == null)
		{
			return false;
		}
		state.tlsSession = sessionToResume;
		state.sessionParameters = sessionParameters;
		state.sessionMasterSecret = sessionMasterSecret;
		return true;
	}

	protected virtual void InvalidateSession(ClientHandshakeState state)
	{
		if (state.tlsSession != null)
		{
			state.tlsSession.Invalidate();
		}
		CancelSession(state);
	}

	protected virtual void ProcessCertificateRequest(ClientHandshakeState state, byte[] body)
	{
		if (state.authentication == null)
		{
			throw new TlsFatalAlert(40);
		}
		MemoryStream memoryStream = new MemoryStream(body, writable: false);
		CertificateRequest certificateRequest = CertificateRequest.Parse(state.clientContext, memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		state.certificateRequest = TlsUtilities.ValidateCertificateRequest(certificateRequest, state.keyExchange);
	}

	protected virtual void ProcessCertificateStatus(ClientHandshakeState state, byte[] body)
	{
		MemoryStream memoryStream = new MemoryStream(body, writable: false);
		state.certificateStatus = CertificateStatus.Parse(state.clientContext, memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
	}

	protected virtual byte[] ProcessHelloVerifyRequest(ClientHandshakeState state, byte[] body)
	{
		MemoryStream memoryStream = new MemoryStream(body, writable: false);
		ProtocolVersion protocolVersion = TlsUtilities.ReadVersion(memoryStream);
		int maxLength = (ProtocolVersion.DTLSv12.IsEqualOrEarlierVersionOf(protocolVersion) ? 255 : 32);
		byte[] result = TlsUtilities.ReadOpaque8(memoryStream, 0, maxLength);
		TlsProtocol.AssertEmpty(memoryStream);
		if (!protocolVersion.IsEqualOrEarlierVersionOf(state.clientContext.ClientVersion))
		{
			throw new TlsFatalAlert(47);
		}
		return result;
	}

	protected virtual void ProcessNewSessionTicket(ClientHandshakeState state, byte[] body)
	{
		MemoryStream memoryStream = new MemoryStream(body, writable: false);
		NewSessionTicket newSessionTicket = NewSessionTicket.Parse(memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		state.client.NotifyNewSessionTicket(newSessionTicket);
	}

	protected virtual void ProcessServerCertificate(ClientHandshakeState state, byte[] body)
	{
		state.authentication = TlsUtilities.ReceiveServerCertificate(state.clientContext, state.client, new MemoryStream(body, writable: false), state.serverExtensions);
	}

	protected virtual void ProcessServerHello(ClientHandshakeState state, byte[] body)
	{
		TlsClient client = state.client;
		TlsClientContextImpl clientContext = state.clientContext;
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		ServerHello serverHello = ServerHello.Parse(new MemoryStream(body, writable: false));
		IDictionary<int, byte[]> extensions = serverHello.Extensions;
		ProtocolVersion version = serverHello.Version;
		ProtocolVersion supportedVersionsExtensionServer = TlsExtensionsUtilities.GetSupportedVersionsExtensionServer(extensions);
		ProtocolVersion protocolVersion;
		if (supportedVersionsExtensionServer == null)
		{
			protocolVersion = version;
		}
		else
		{
			if (!ProtocolVersion.DTLSv12.Equals(version) || !ProtocolVersion.DTLSv13.IsEqualOrEarlierVersionOf(supportedVersionsExtensionServer))
			{
				throw new TlsFatalAlert(47);
			}
			protocolVersion = supportedVersionsExtensionServer;
		}
		ReportServerVersion(state, protocolVersion);
		int[] offeredCipherSuites = state.offeredCipherSuites;
		securityParameters.m_serverRandom = serverHello.Random;
		if (!clientContext.ClientVersion.Equals(protocolVersion))
		{
			TlsUtilities.CheckDowngradeMarker(protocolVersion, securityParameters.ServerRandom);
		}
		byte[] array = (securityParameters.m_sessionID = serverHello.SessionID);
		client.NotifySessionID(array);
		securityParameters.m_resumedSession = array.Length != 0 && state.tlsSession != null && Arrays.AreEqual(array, state.tlsSession.SessionID);
		if (securityParameters.IsResumedSession && (serverHello.CipherSuite != state.sessionParameters.CipherSuite || !securityParameters.NegotiatedVersion.Equals(state.sessionParameters.NegotiatedVersion)))
		{
			throw new TlsFatalAlert(47, "ServerHello parameters do not match resumed session");
		}
		int num = DtlsProtocol.ValidateSelectedCipherSuite(serverHello.CipherSuite, 47);
		if (!TlsUtilities.IsValidCipherSuiteSelection(offeredCipherSuites, num) || !TlsUtilities.IsValidVersionForCipherSuite(num, securityParameters.NegotiatedVersion))
		{
			throw new TlsFatalAlert(47, "ServerHello selected invalid cipher suite");
		}
		TlsUtilities.NegotiatedCipherSuite(securityParameters, num);
		client.NotifySelectedCipherSuite(num);
		state.serverExtensions = extensions;
		if (extensions != null)
		{
			foreach (int key in extensions.Keys)
			{
				if (key != 65281)
				{
					if (TlsUtilities.GetExtensionData(state.clientExtensions, key) == null)
					{
						throw new TlsFatalAlert(110);
					}
					_ = securityParameters.IsResumedSession;
				}
			}
		}
		byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 65281);
		if (extensionData == null)
		{
			securityParameters.m_secureRenegotiation = false;
		}
		else
		{
			securityParameters.m_secureRenegotiation = true;
			if (!Arrays.FixedTimeEquals(extensionData, TlsProtocol.CreateRenegotiationInfo(TlsUtilities.EmptyBytes)))
			{
				throw new TlsFatalAlert(40);
			}
		}
		client.NotifySecureRenegotiation(securityParameters.IsSecureRenegotiation);
		bool flag = false;
		if (TlsExtensionsUtilities.HasExtendedMasterSecretExtension(state.clientExtensions))
		{
			flag = TlsExtensionsUtilities.HasExtendedMasterSecretExtension(extensions);
			if (TlsUtilities.IsExtendedMasterSecretOptional(protocolVersion))
			{
				if (!flag && client.RequiresExtendedMasterSecret())
				{
					throw new TlsFatalAlert(40, "Extended Master Secret extension is required");
				}
			}
			else if (flag)
			{
				throw new TlsFatalAlert(47, "Server sent an unexpected extended_master_secret extension negotiating " + protocolVersion);
			}
		}
		securityParameters.m_extendedMasterSecret = flag;
		if (securityParameters.IsResumedSession && securityParameters.IsExtendedMasterSecret != state.sessionParameters.IsExtendedMasterSecret)
		{
			throw new TlsFatalAlert(40, "Server resumed session with mismatched extended_master_secret negotiation");
		}
		securityParameters.m_applicationProtocol = TlsExtensionsUtilities.GetAlpnExtensionServer(extensions);
		securityParameters.m_applicationProtocolSet = true;
		if (ProtocolVersion.DTLSv12.Equals(securityParameters.NegotiatedVersion))
		{
			byte[] connectionIDExtension = TlsExtensionsUtilities.GetConnectionIDExtension(extensions);
			if (connectionIDExtension != null)
			{
				byte[] connectionIDPeer = TlsExtensionsUtilities.GetConnectionIDExtension(state.clientExtensions) ?? throw new TlsFatalAlert(80);
				securityParameters.m_connectionIDLocal = connectionIDExtension;
				securityParameters.m_connectionIDPeer = connectionIDPeer;
			}
		}
		HeartbeatExtension heartbeatExtension = TlsExtensionsUtilities.GetHeartbeatExtension(extensions);
		if (heartbeatExtension == null)
		{
			state.heartbeat = null;
			state.heartbeatPolicy = 2;
		}
		else if (1 != heartbeatExtension.Mode)
		{
			state.heartbeat = null;
		}
		IDictionary<int, byte[]> dictionary = state.clientExtensions;
		IDictionary<int, byte[]> dictionary2 = extensions;
		if (securityParameters.IsResumedSession)
		{
			dictionary = null;
			dictionary2 = state.sessionParameters.ReadServerExtensions();
		}
		if (dictionary2 != null && dictionary2.Count > 0)
		{
			bool flag2 = TlsExtensionsUtilities.HasEncryptThenMacExtension(dictionary2);
			if (flag2 && !TlsUtilities.IsBlockCipherSuite(securityParameters.CipherSuite))
			{
				throw new TlsFatalAlert(47);
			}
			securityParameters.m_encryptThenMac = flag2;
			securityParameters.m_maxFragmentLength = TlsUtilities.ProcessMaxFragmentLengthExtension(dictionary, dictionary2, 47);
			securityParameters.m_truncatedHmac = TlsExtensionsUtilities.HasTruncatedHmacExtension(dictionary2);
			if (!securityParameters.IsResumedSession)
			{
				if (TlsUtilities.HasExpectedEmptyExtensionData(dictionary2, 17, 47))
				{
					securityParameters.m_statusRequestVersion = 2;
				}
				else if (TlsUtilities.HasExpectedEmptyExtensionData(dictionary2, 5, 47))
				{
					securityParameters.m_statusRequestVersion = 1;
				}
				securityParameters.m_clientCertificateType = TlsUtilities.ProcessClientCertificateTypeExtension(dictionary, dictionary2, 47);
				securityParameters.m_serverCertificateType = TlsUtilities.ProcessServerCertificateTypeExtension(dictionary, dictionary2, 47);
				state.expectSessionTicket = TlsUtilities.HasExpectedEmptyExtensionData(dictionary2, 35, 47);
			}
		}
		if (dictionary != null)
		{
			client.ProcessServerExtensions(dictionary2);
		}
	}

	protected virtual void ProcessServerKeyExchange(ClientHandshakeState state, byte[] body)
	{
		MemoryStream memoryStream = new MemoryStream(body, writable: false);
		state.keyExchange.ProcessServerKeyExchange(memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
	}

	protected virtual void ProcessServerSupplementalData(ClientHandshakeState state, byte[] body)
	{
		IList<SupplementalDataEntry> serverSupplementalData = TlsProtocol.ReadSupplementalDataMessage(new MemoryStream(body, writable: false));
		state.client.ProcessServerSupplementalData(serverSupplementalData);
	}

	protected virtual void ReportServerVersion(ClientHandshakeState state, ProtocolVersion server_version)
	{
		TlsClientContextImpl clientContext = state.clientContext;
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		ProtocolVersion negotiatedVersion = securityParameters.NegotiatedVersion;
		if (negotiatedVersion != null)
		{
			if (!negotiatedVersion.Equals(server_version))
			{
				throw new TlsFatalAlert(47);
			}
			return;
		}
		if (!ProtocolVersion.Contains(clientContext.ClientSupportedVersions, server_version))
		{
			throw new TlsFatalAlert(70);
		}
		securityParameters.m_negotiatedVersion = server_version;
		TlsUtilities.NegotiatedVersionDtlsClient(clientContext, state.client);
	}

	protected static byte[] PatchClientHelloWithCookie(byte[] clientHelloBody, byte[] cookie)
	{
		int num = 34;
		int num2 = TlsUtilities.ReadUint8(clientHelloBody, num);
		int num3 = num + 1 + num2;
		int num4 = num3 + 1;
		byte[] array = new byte[clientHelloBody.Length + cookie.Length];
		Array.Copy(clientHelloBody, 0, array, 0, num3);
		TlsUtilities.CheckUint8(cookie.Length);
		TlsUtilities.WriteUint8(cookie.Length, array, num3);
		Array.Copy(cookie, 0, array, num4, cookie.Length);
		Array.Copy(clientHelloBody, num4, array, num4 + cookie.Length, clientHelloBody.Length - num4);
		return array;
	}
}
