using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Mirror.BouncyCastle.Crypto;
using UnityEngine;
using UnityEngine.Serialization;
using kcp2k;

namespace Mirror.Transports.Encryption;

[HelpURL("https://mirror-networking.gitbook.io/docs/manual/transports/encryption-transport")]
public class ThreadedEncryptionKcpTransport : ThreadedKcpTransport
{
	public enum ValidationMode
	{
		Off,
		List,
		Callback
	}

	[HideInInspector]
	public ValidationMode ClientValidateServerPubKey;

	[Tooltip("List of public key fingerprints the client will accept")]
	[HideInInspector]
	public string[] ClientTrustedPubKeySignatures;

	public Func<PubKeyInfo, bool> OnClientValidateServerPubKey;

	[HideInInspector]
	[FormerlySerializedAs("serverLoadKeyPairFromFile")]
	public bool ServerLoadKeyPairFromFile;

	[HideInInspector]
	[FormerlySerializedAs("serverKeypairPath")]
	public string ServerKeypairPath = "./server-keys.json";

	private EncryptedConnection encryptedClient;

	private readonly Dictionary<int, EncryptedConnection> serverConnections = new Dictionary<int, EncryptedConnection>();

	private readonly List<EncryptedConnection> serverPendingConnections = new List<EncryptedConnection>();

	private EncryptionCredentials credentials;

	private Stopwatch stopwatch = Stopwatch.StartNew();

	public override bool IsEncrypted => true;

	public override string EncryptionCipher => "AES256-GCM";

	public string EncryptionPublicKeyFingerprint => credentials?.PublicKeyFingerprint;

	public byte[] EncryptionPublicKey => credentials?.PublicKeySerialized;

	public override string ToString()
	{
		return "Encrypted " + base.ToString();
	}

	private void ServerRemoveFromPending(EncryptedConnection con)
	{
		for (int i = 0; i < serverPendingConnections.Count; i++)
		{
			if (serverPendingConnections[i] == con)
			{
				int index = serverPendingConnections.Count - 1;
				serverPendingConnections[i] = serverPendingConnections[index];
				serverPendingConnections.RemoveAt(index);
				break;
			}
		}
	}

	private void HandleInnerServerDisconnected(int connId)
	{
		if (serverConnections.TryGetValue(connId, out var value))
		{
			ServerRemoveFromPending(value);
			serverConnections.Remove(connId);
		}
		OnThreadedServerDisconnected(connId);
	}

	private void HandleInnerServerDataReceived(int connId, ArraySegment<byte> data, int channel)
	{
		if (serverConnections.TryGetValue(connId, out var value))
		{
			value.OnReceiveRaw(data, channel);
		}
	}

	private void HandleInnerServerConnected(int connId, IPEndPoint clientRemoteAddress)
	{
		UnityEngine.Debug.Log($"[ThreadedEncryptionKcpTransport] New connection #{connId} from {clientRemoteAddress}");
		EncryptedConnection ec = null;
		ec = new EncryptedConnection(credentials, isClient: false, delegate(ArraySegment<byte> segment, int channel)
		{
			server.Send(connId, segment, KcpTransport.ToKcpChannel(channel));
			OnThreadedServerSend(connId, segment, channel);
		}, delegate(ArraySegment<byte> segment, int channel)
		{
			OnThreadedServerReceive(connId, segment, channel);
		}, delegate
		{
			UnityEngine.Debug.Log($"[ThreadedEncryptionKcpTransport] Connection #{connId} is ready");
			ServerRemoveFromPending(ec);
			OnThreadedServerConnected(connId, clientRemoteAddress);
		}, delegate(TransportError type, string msg)
		{
			OnThreadedServerError(connId, type, msg);
			ServerDisconnect(connId);
		});
		serverConnections.Add(connId, ec);
		serverPendingConnections.Add(ec);
	}

	private void HandleInnerClientDisconnected()
	{
		encryptedClient = null;
		OnThreadedClientDisconnected();
	}

	private void HandleInnerClientDataReceived(ArraySegment<byte> data, int channel)
	{
		encryptedClient?.OnReceiveRaw(data, channel);
	}

	private void HandleInnerClientConnected()
	{
		encryptedClient = new EncryptedConnection(credentials, isClient: true, delegate(ArraySegment<byte> segment, int channel)
		{
			client.Send(segment, KcpTransport.ToKcpChannel(channel));
			OnThreadedClientSend(segment, channel);
		}, delegate(ArraySegment<byte> segment, int channel)
		{
			OnThreadedClientReceive(segment, channel);
		}, delegate
		{
			OnThreadedClientConnected();
		}, delegate(TransportError type, string msg)
		{
			OnThreadedClientError(type, msg);
			ClientDisconnect();
		}, HandleClientValidateServerPubKey);
	}

	private bool HandleClientValidateServerPubKey(PubKeyInfo pubKeyInfo)
	{
		return ClientValidateServerPubKey switch
		{
			ValidationMode.Off => true, 
			ValidationMode.List => Array.IndexOf(ClientTrustedPubKeySignatures, pubKeyInfo.Fingerprint) >= 0, 
			ValidationMode.Callback => OnClientValidateServerPubKey(pubKeyInfo), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	protected override void Awake()
	{
		base.Awake();
		client = new KcpClient(HandleInnerClientConnected, delegate(ArraySegment<byte> message, KcpChannel channel)
		{
			HandleInnerClientDataReceived(message, KcpTransport.FromKcpChannel(channel));
		}, HandleInnerClientDisconnected, delegate(ErrorCode error, string reason)
		{
			OnThreadedClientError(KcpTransport.ToTransportError(error), reason);
		}, config);
		server = new KcpServer(HandleInnerServerConnected, delegate(int connectionId, ArraySegment<byte> message, KcpChannel channel)
		{
			HandleInnerServerDataReceived(connectionId, message, KcpTransport.FromKcpChannel(channel));
		}, HandleInnerServerDisconnected, delegate(int connectionId, ErrorCode error, string reason)
		{
			OnThreadedServerError(connectionId, KcpTransport.ToTransportError(error), reason);
		}, config);
		UnityEngine.Debug.Log($"ThreadedEncryptionKcpTransport: IsHardwareAccelerated={AesUtilities.IsHardwareAccelerated}");
	}

	protected override void ThreadedClientConnect(string address)
	{
		if (SetupEncryptionForClient())
		{
			base.ThreadedClientConnect(address);
		}
	}

	private bool SetupEncryptionForClient()
	{
		switch (ClientValidateServerPubKey)
		{
		case ValidationMode.List:
			if (ClientTrustedPubKeySignatures == null || ClientTrustedPubKeySignatures.Length == 0)
			{
				OnThreadedClientError(TransportError.Unexpected, "Validate Server Public Key is set to List, but the clientTrustedPubKeySignatures list is empty.");
				return false;
			}
			break;
		case ValidationMode.Callback:
			if (OnClientValidateServerPubKey == null)
			{
				OnThreadedClientError(TransportError.Unexpected, "Validate Server Public Key is set to Callback, but the onClientValidateServerPubKey handler is not set");
				return false;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case ValidationMode.Off:
			break;
		}
		credentials = EncryptionCredentials.Generate();
		return true;
	}

	protected override void ThreadedClientConnect(Uri address)
	{
		if (SetupEncryptionForClient())
		{
			base.ThreadedClientConnect(address);
		}
	}

	protected override void ThreadedClientSend(ArraySegment<byte> segment, int channelId)
	{
		encryptedClient?.Send(segment, channelId);
	}

	protected override void ThreadedServerStart()
	{
		if (ServerLoadKeyPairFromFile)
		{
			credentials = EncryptionCredentials.LoadFromFile(ServerKeypairPath);
		}
		else
		{
			credentials = EncryptionCredentials.Generate();
		}
		base.ThreadedServerStart();
	}

	protected override void ThreadedServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		if (serverConnections.TryGetValue(connectionId, out var value) && value.IsReady)
		{
			value.Send(segment, channelId);
		}
	}

	public override int GetMaxPacketSize(int channelId = 0)
	{
		return base.GetMaxPacketSize(channelId) - 29;
	}

	public override int GetBatchThreshold(int channelId)
	{
		return base.GetBatchThreshold(channelId) - 29;
	}

	protected override void ThreadedClientLateUpdate()
	{
		base.ThreadedClientLateUpdate();
		encryptedClient?.TickNonReady(stopwatch.Elapsed.TotalSeconds);
	}

	protected override void ThreadedServerLateUpdate()
	{
		base.ThreadedServerLateUpdate();
		for (int num = serverPendingConnections.Count - 1; num >= 0; num--)
		{
			serverPendingConnections[num].TickNonReady(stopwatch.Elapsed.TotalSeconds);
		}
	}
}
