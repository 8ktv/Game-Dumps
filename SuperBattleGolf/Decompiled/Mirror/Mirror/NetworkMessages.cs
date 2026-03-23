using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Mirror;

public static class NetworkMessages
{
	public const int IdSize = 2;

	public static readonly Dictionary<ushort, Type> Lookup = new Dictionary<ushort, Type>();

	public static void LogTypes()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("NetworkMessageIds:");
		foreach (KeyValuePair<ushort, Type> item in Lookup)
		{
			stringBuilder.AppendLine($"  Id={item.Key} = {item.Value}");
		}
		Debug.Log(stringBuilder.ToString());
	}

	public static int MaxContentSize(int channelId)
	{
		int maxPacketSize = Transport.active.GetMaxPacketSize(channelId);
		return maxPacketSize - 2 - Batcher.MaxMessageOverhead(maxPacketSize);
	}

	public static int MaxMessageSize(int channelId)
	{
		return MaxContentSize(channelId) + 2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort GetId<T>() where T : struct, NetworkMessage
	{
		return NetworkMessageId<T>.Id;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Pack<T>(T message, NetworkWriter writer) where T : struct, NetworkMessage
	{
		writer.WriteUShort(NetworkMessageId<T>.Id);
		writer.Write(message);
	}

	public static bool UnpackId(NetworkReader reader, out ushort messageId)
	{
		try
		{
			messageId = reader.ReadUShort();
			return true;
		}
		catch (EndOfStreamException)
		{
			messageId = 0;
			return false;
		}
	}

	internal static NetworkMessageDelegate WrapHandler<T, C>(Action<C, T, int> handler, bool requireAuthentication, bool exceptionsDisconnect) where T : struct, NetworkMessage where C : NetworkConnection
	{
		return delegate(NetworkConnection conn, NetworkReader reader, int channelId)
		{
			T val = default(T);
			int position = reader.Position;
			try
			{
				if (requireAuthentication && !conn.isAuthenticated)
				{
					Debug.LogWarning($"Disconnecting connection: {conn}. Received message {typeof(T)} that required authentication, but the user has not authenticated yet");
					conn.Disconnect();
					return;
				}
				val = reader.Read<T>();
			}
			catch (Exception arg)
			{
				if (exceptionsDisconnect)
				{
					Debug.LogError($"Disconnecting connection: {conn} because reading a message of type {typeof(T)} caused an Exception. This can happen if the other side accidentally (or an attacker intentionally) sent invalid data. Reason: {arg}");
					conn.Disconnect();
				}
				else
				{
					Debug.LogError($"Caught an Exception when reading a message from: {conn} of type {typeof(T)}. Reason: {arg}");
				}
				return;
			}
			finally
			{
				int position2 = reader.Position;
				NetworkDiagnostics.OnReceive(val, channelId, position2 - position);
			}
			try
			{
				handler((C)conn, val, channelId);
			}
			catch (Exception arg2)
			{
				if (exceptionsDisconnect)
				{
					Debug.LogError($"Disconnecting connection: {conn} because handling a message of type {typeof(T)} caused an Exception. This can happen if the other side accidentally (or an attacker intentionally) sent invalid data. Reason: {arg2}");
					conn.Disconnect();
				}
				else
				{
					Debug.LogError($"Caught an Exception when handling a message from: {conn} of type {typeof(T)}. Reason: {arg2}");
				}
			}
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static NetworkMessageDelegate WrapHandler<T, C>(Action<C, T> handler, bool requireAuthentication, bool exceptionsDisconnect) where T : struct, NetworkMessage where C : NetworkConnection
	{
		return WrapHandler<T, C>(Wrapped, requireAuthentication, exceptionsDisconnect);
		void Wrapped(C conn, T msg, int _)
		{
			handler(conn, msg);
		}
	}
}
