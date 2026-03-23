using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
public class MultiplexTransport : Transport, PortTransport
{
	public Transport[] transports;

	private Transport available;

	private readonly Dictionary<KeyValuePair<int, int>, int> originalToMultiplexedId = new Dictionary<KeyValuePair<int, int>, int>(100);

	private readonly Dictionary<int, KeyValuePair<int, int>> multiplexedToOriginalId = new Dictionary<int, KeyValuePair<int, int>>(100);

	private int nextMultiplexedId = 1;

	private bool alreadyWarned;

	public ushort Port
	{
		get
		{
			Transport[] array = transports;
			foreach (Transport transport in array)
			{
				if (transport.Available() && transport is PortTransport portTransport)
				{
					return portTransport.Port;
				}
			}
			return 0;
		}
		set
		{
			if (Utils.IsHeadless() && !alreadyWarned)
			{
				alreadyWarned = true;
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("[Multiplexer] Server cannot set the same listen port for all transports! Set them directly instead.");
				Console.ResetColor();
				return;
			}
			Transport[] array = transports;
			foreach (Transport transport in array)
			{
				if (transport.Available() && transport is PortTransport portTransport)
				{
					portTransport.Port = value;
					break;
				}
			}
		}
	}

	public int AddToLookup(int originalConnectionId, int transportIndex)
	{
		KeyValuePair<int, int> keyValuePair = new KeyValuePair<int, int>(originalConnectionId, transportIndex);
		int num = nextMultiplexedId++;
		originalToMultiplexedId[keyValuePair] = num;
		multiplexedToOriginalId[num] = keyValuePair;
		return num;
	}

	public void RemoveFromLookup(int originalConnectionId, int transportIndex)
	{
		KeyValuePair<int, int> key = new KeyValuePair<int, int>(originalConnectionId, transportIndex);
		if (originalToMultiplexedId.TryGetValue(key, out var value))
		{
			originalToMultiplexedId.Remove(key);
			multiplexedToOriginalId.Remove(value);
		}
	}

	public bool OriginalId(int multiplexId, out int originalConnectionId, out int transportIndex)
	{
		if (!multiplexedToOriginalId.ContainsKey(multiplexId))
		{
			originalConnectionId = 0;
			transportIndex = 0;
			return false;
		}
		KeyValuePair<int, int> keyValuePair = multiplexedToOriginalId[multiplexId];
		originalConnectionId = keyValuePair.Key;
		transportIndex = keyValuePair.Value;
		return true;
	}

	public int MultiplexId(int originalConnectionId, int transportIndex)
	{
		KeyValuePair<int, int> key = new KeyValuePair<int, int>(originalConnectionId, transportIndex);
		if (originalToMultiplexedId.TryGetValue(key, out var value))
		{
			return value;
		}
		return 0;
	}

	public void Awake()
	{
		if (transports == null || transports.Length == 0)
		{
			Debug.LogError("[Multiplexer] Multiplex transport requires at least 1 underlying transport");
		}
	}

	public override void ClientEarlyUpdate()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ClientEarlyUpdate();
		}
	}

	public override void ServerEarlyUpdate()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ServerEarlyUpdate();
		}
	}

	public override void ClientLateUpdate()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ClientLateUpdate();
		}
	}

	public override void ServerLateUpdate()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ServerLateUpdate();
		}
	}

	private void OnEnable()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
	}

	private void OnDisable()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	public override bool Available()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Available())
			{
				return true;
			}
		}
		return false;
	}

	public override void ClientConnect(string address)
	{
		Transport[] array = transports;
		foreach (Transport transport in array)
		{
			if (transport.Available())
			{
				available = transport;
				transport.OnClientConnected = OnClientConnected;
				transport.OnClientDataReceived = OnClientDataReceived;
				transport.OnClientError = OnClientError;
				transport.OnClientTransportException = OnClientTransportException;
				transport.OnClientDisconnected = OnClientDisconnected;
				transport.ClientConnect(address);
				return;
			}
		}
		throw new ArgumentException("[Multiplexer] No transport suitable for this platform");
	}

	public override void ClientConnect(Uri uri)
	{
		Transport[] array = transports;
		foreach (Transport transport in array)
		{
			if (transport.Available())
			{
				try
				{
					available = transport;
					transport.OnClientConnected = OnClientConnected;
					transport.OnClientDataReceived = OnClientDataReceived;
					transport.OnClientError = OnClientError;
					transport.OnClientTransportException = OnClientTransportException;
					transport.OnClientDisconnected = OnClientDisconnected;
					transport.ClientConnect(uri);
					return;
				}
				catch (ArgumentException)
				{
				}
			}
		}
		throw new ArgumentException("[Multiplexer] No transport suitable for this platform");
	}

	public override bool ClientConnected()
	{
		if ((object)available != null)
		{
			return available.ClientConnected();
		}
		return false;
	}

	public override void ClientDisconnect()
	{
		if ((object)available != null)
		{
			available.ClientDisconnect();
		}
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId)
	{
		available.ClientSend(segment, channelId);
	}

	private void AddServerCallbacks()
	{
		for (int i = 0; i < transports.Length; i++)
		{
			int transportIndex = i;
			Transport obj = transports[i];
			obj.OnServerConnected = delegate(int originalConnectionId)
			{
				int obj2 = AddToLookup(originalConnectionId, transportIndex);
				OnServerConnected(obj2);
			};
			obj.OnServerConnectedWithAddress = delegate(int originalConnectionId, string address)
			{
				int arg = AddToLookup(originalConnectionId, transportIndex);
				OnServerConnectedWithAddress(arg, address);
			};
			obj.OnServerDataReceived = delegate(int originalConnectionId, ArraySegment<byte> data, int channel)
			{
				int num = MultiplexId(originalConnectionId, transportIndex);
				if (num == 0)
				{
					if (Utils.IsHeadless())
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine($"[Multiplexer] Received data for unknown connectionId={originalConnectionId} on transport={transportIndex}");
						Console.ResetColor();
					}
					else
					{
						Debug.LogWarning($"[Multiplexer] Received data for unknown connectionId={originalConnectionId} on transport={transportIndex}");
					}
				}
				else
				{
					OnServerDataReceived(num, data, channel);
				}
			};
			obj.OnServerError = delegate(int originalConnectionId, TransportError error, string reason)
			{
				int num = MultiplexId(originalConnectionId, transportIndex);
				if (num == 0)
				{
					if (Utils.IsHeadless())
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine($"[Multiplexer] Received error for unknown connectionId={originalConnectionId} on transport={transportIndex}");
						Console.ResetColor();
					}
					else
					{
						Debug.LogError($"[Multiplexer] Received error for unknown connectionId={originalConnectionId} on transport={transportIndex}");
					}
				}
				else
				{
					OnServerError(num, error, reason);
				}
			};
			obj.OnServerTransportException = delegate(int originalConnectionId, Exception exception)
			{
				int arg = MultiplexId(originalConnectionId, transportIndex);
				OnServerTransportException(arg, exception);
			};
			obj.OnServerDisconnected = delegate(int originalConnectionId)
			{
				int num = MultiplexId(originalConnectionId, transportIndex);
				if (num == 0)
				{
					if (Utils.IsHeadless())
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine($"[Multiplexer] Received disconnect for unknown connectionId={originalConnectionId} on transport={transportIndex}");
						Console.ResetColor();
					}
					else
					{
						Debug.LogWarning($"[Multiplexer] Received disconnect for unknown connectionId={originalConnectionId} on transport={transportIndex}");
					}
				}
				else
				{
					OnServerDisconnected(num);
					RemoveFromLookup(originalConnectionId, transportIndex);
				}
			};
		}
	}

	public override Uri ServerUri()
	{
		return transports[0].ServerUri();
	}

	public override bool ServerActive()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].ServerActive())
			{
				return false;
			}
		}
		return true;
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		if (OriginalId(connectionId, out var originalConnectionId, out var transportIndex))
		{
			return transports[transportIndex].ServerGetClientAddress(originalConnectionId);
		}
		return "";
	}

	public override void ServerDisconnect(int connectionId)
	{
		if (OriginalId(connectionId, out var originalConnectionId, out var transportIndex))
		{
			transports[transportIndex].ServerDisconnect(originalConnectionId);
		}
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		if (OriginalId(connectionId, out var originalConnectionId, out var transportIndex))
		{
			transports[transportIndex].ServerSend(originalConnectionId, segment, channelId);
		}
	}

	public override void ServerStart()
	{
		AddServerCallbacks();
		Transport[] array = transports;
		foreach (Transport transport in array)
		{
			transport.ServerStart();
			if (transport is PortTransport portTransport)
			{
				if (Utils.IsHeadless())
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"[Multiplexer]: Server listening on port {portTransport.Port} with {transport}");
					Console.ResetColor();
				}
				else
				{
					Debug.Log($"[Multiplexer]: Server listening on port {portTransport.Port} with {transport}");
				}
			}
		}
	}

	public override void ServerStop()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ServerStop();
		}
	}

	public override int GetMaxPacketSize(int channelId = 0)
	{
		int num = int.MaxValue;
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			num = Mathf.Min(array[i].GetMaxPacketSize(channelId), num);
		}
		return num;
	}

	public override void Shutdown()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Shutdown();
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Multiplexer:");
		Transport[] array = transports;
		foreach (Transport arg in array)
		{
			stringBuilder.Append($" {arg}");
		}
		return stringBuilder.ToString().Trim();
	}
}
