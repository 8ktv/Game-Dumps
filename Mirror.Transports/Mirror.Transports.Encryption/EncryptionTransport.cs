using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Crypto;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mirror.Transports.Encryption;

[HelpURL("https://mirror-networking.gitbook.io/docs/manual/transports/encryption-transport")]
public class EncryptionTransport : Transport, PortTransport
{
	public enum ValidationMode
	{
		Off,
		List,
		Callback
	}

	[FormerlySerializedAs("inner")]
	[HideInInspector]
	public Transport Inner;

	[FormerlySerializedAs("clientValidateServerPubKey")]
	[HideInInspector]
	public ValidationMode ClientValidateServerPubKey;

	[FormerlySerializedAs("clientTrustedPubKeySignatures")]
	[HideInInspector]
	[Tooltip("List of public key fingerprints the client will accept")]
	public string[] ClientTrustedPubKeySignatures;

	public Func<PubKeyInfo, bool> OnClientValidateServerPubKey;

	[FormerlySerializedAs("serverLoadKeyPairFromFile")]
	[HideInInspector]
	public bool ServerLoadKeyPairFromFile;

	[FormerlySerializedAs("serverKeypairPath")]
	[HideInInspector]
	public string ServerKeypairPath = "./server-keys.json";

	private EncryptedConnection client;

	private readonly Dictionary<int, EncryptedConnection> serverConnections = new Dictionary<int, EncryptedConnection>();

	private readonly List<EncryptedConnection> serverPendingConnections = new List<EncryptedConnection>();

	private EncryptionCredentials credentials;

	public override bool IsEncrypted => true;

	public override string EncryptionCipher => "AES256-GCM";

	public ushort Port
	{
		get
		{
			if (Inner is PortTransport portTransport)
			{
				return portTransport.Port;
			}
			Debug.LogError($"EncryptionTransport can't get Port because {Inner} is not a PortTransport");
			return 0;
		}
		set
		{
			if (Inner is PortTransport portTransport)
			{
				portTransport.Port = value;
			}
			else
			{
				Debug.LogError($"EncryptionTransport can't set Port because {Inner} is not a PortTransport");
			}
		}
	}

	public string EncryptionPublicKeyFingerprint => credentials?.PublicKeyFingerprint;

	public byte[] EncryptionPublicKey => credentials?.PublicKeySerialized;

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
		OnServerDisconnected?.Invoke(connId);
	}

	private void HandleInnerServerError(int connId, TransportError type, string msg)
	{
		OnServerError?.Invoke(connId, type, "inner: " + msg);
	}

	private void HandleInnerServerDataReceived(int connId, ArraySegment<byte> data, int channel)
	{
		if (serverConnections.TryGetValue(connId, out var value))
		{
			value.OnReceiveRaw(data, channel);
		}
	}

	private void HandleInnerServerConnected(int connId)
	{
		HandleInnerServerConnected(connId, Inner.ServerGetClientAddress(connId));
	}

	private void HandleInnerServerConnected(int connId, string clientRemoteAddress)
	{
		Debug.Log($"[EncryptionTransport] New connection #{connId} from {clientRemoteAddress}");
		EncryptedConnection ec = null;
		ec = new EncryptedConnection(credentials, isClient: false, delegate(ArraySegment<byte> segment, int channel)
		{
			Inner.ServerSend(connId, segment, channel);
		}, delegate(ArraySegment<byte> segment, int channel)
		{
			OnServerDataReceived?.Invoke(connId, segment, channel);
		}, delegate
		{
			Debug.Log($"[EncryptionTransport] Connection #{connId} is ready");
			ServerRemoveFromPending(ec);
			OnServerConnectedWithAddress?.Invoke(connId, clientRemoteAddress);
		}, delegate(TransportError type, string msg)
		{
			OnServerError?.Invoke(connId, type, msg);
			ServerDisconnect(connId);
		});
		serverConnections.Add(connId, ec);
		serverPendingConnections.Add(ec);
	}

	private void HandleInnerClientDisconnected()
	{
		client = null;
		OnClientDisconnected?.Invoke();
	}

	private void HandleInnerClientError(TransportError arg1, string arg2)
	{
		OnClientError?.Invoke(arg1, "inner: " + arg2);
	}

	private void HandleInnerClientDataReceived(ArraySegment<byte> data, int channel)
	{
		client?.OnReceiveRaw(data, channel);
	}

	private void HandleInnerClientConnected()
	{
		client = new EncryptedConnection(credentials, isClient: true, delegate(ArraySegment<byte> segment, int channel)
		{
			Inner.ClientSend(segment, channel);
		}, delegate(ArraySegment<byte> segment, int channel)
		{
			OnClientDataReceived?.Invoke(segment, channel);
		}, delegate
		{
			OnClientConnected?.Invoke();
		}, delegate(TransportError type, string msg)
		{
			OnClientError?.Invoke(type, msg);
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

	private void Awake()
	{
		Debug.Log($"EncryptionTransport: IsHardwareAccelerated={AesUtilities.IsHardwareAccelerated}");
	}

	public override bool Available()
	{
		return Inner.Available();
	}

	public override bool ClientConnected()
	{
		if (client != null)
		{
			return client.IsReady;
		}
		return false;
	}

	public override void ClientConnect(string address)
	{
		switch (ClientValidateServerPubKey)
		{
		case ValidationMode.List:
			if (ClientTrustedPubKeySignatures == null || ClientTrustedPubKeySignatures.Length == 0)
			{
				OnClientError?.Invoke(TransportError.Unexpected, "Validate Server Public Key is set to List, but the clientTrustedPubKeySignatures list is empty.");
				return;
			}
			break;
		case ValidationMode.Callback:
			if (OnClientValidateServerPubKey == null)
			{
				OnClientError?.Invoke(TransportError.Unexpected, "Validate Server Public Key is set to Callback, but the onClientValidateServerPubKey handler is not set");
				return;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case ValidationMode.Off:
			break;
		}
		credentials = EncryptionCredentials.Generate();
		Inner.OnClientConnected = HandleInnerClientConnected;
		Inner.OnClientDataReceived = HandleInnerClientDataReceived;
		Inner.OnClientDataSent = delegate(ArraySegment<byte> bytes, int channel)
		{
			OnClientDataSent?.Invoke(bytes, channel);
		};
		Inner.OnClientError = HandleInnerClientError;
		Inner.OnClientDisconnected = HandleInnerClientDisconnected;
		Inner.ClientConnect(address);
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId = 0)
	{
		client?.Send(segment, channelId);
	}

	public override void ClientDisconnect()
	{
		Inner.ClientDisconnect();
	}

	public override Uri ServerUri()
	{
		return Inner.ServerUri();
	}

	public override bool ServerActive()
	{
		return Inner.ServerActive();
	}

	public override void ServerStart()
	{
		if (ServerLoadKeyPairFromFile)
		{
			credentials = EncryptionCredentials.LoadFromFile(ServerKeypairPath);
		}
		else
		{
			credentials = EncryptionCredentials.Generate();
		}
		Inner.OnServerConnected = HandleInnerServerConnected;
		Inner.OnServerConnectedWithAddress = HandleInnerServerConnected;
		Inner.OnServerDataReceived = HandleInnerServerDataReceived;
		Inner.OnServerDataSent = delegate(int connId, ArraySegment<byte> bytes, int channel)
		{
			OnServerDataSent?.Invoke(connId, bytes, channel);
		};
		Inner.OnServerError = HandleInnerServerError;
		Inner.OnServerDisconnected = HandleInnerServerDisconnected;
		Inner.ServerStart();
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId = 0)
	{
		if (serverConnections.TryGetValue(connectionId, out var value) && value.IsReady)
		{
			value.Send(segment, channelId);
		}
	}

	public override void ServerDisconnect(int connectionId)
	{
		Inner.ServerDisconnect(connectionId);
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		return Inner.ServerGetClientAddress(connectionId);
	}

	public override void ServerStop()
	{
		Inner.ServerStop();
	}

	public override int GetMaxPacketSize(int channelId = 0)
	{
		return Inner.GetMaxPacketSize(channelId) - 29;
	}

	public override int GetBatchThreshold(int channelId = 0)
	{
		return Inner.GetBatchThreshold(channelId) - 29;
	}

	public override void Shutdown()
	{
		Inner.Shutdown();
	}

	public override void ClientEarlyUpdate()
	{
		Inner.ClientEarlyUpdate();
	}

	public override void ClientLateUpdate()
	{
		Inner.ClientLateUpdate();
		client?.TickNonReady(NetworkTime.localTime);
	}

	public override void ServerEarlyUpdate()
	{
		Inner.ServerEarlyUpdate();
	}

	public override void ServerLateUpdate()
	{
		Inner.ServerLateUpdate();
		for (int num = serverPendingConnections.Count - 1; num >= 0; num--)
		{
			serverPendingConnections[num].TickNonReady(NetworkTime.time);
		}
	}
}
