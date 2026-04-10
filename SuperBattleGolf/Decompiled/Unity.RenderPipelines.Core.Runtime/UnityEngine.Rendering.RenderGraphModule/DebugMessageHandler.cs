using System;
using System.IO;
using UnityEngine.Networking.PlayerConnection;

namespace UnityEngine.Rendering.RenderGraphModule;

internal class DebugMessageHandler : ScriptableObject
{
	public enum MessageType : byte
	{
		Activate,
		DebugData,
		AnalyticsData
	}

	public abstract class IPayload
	{
		public int version;

		public bool isCompatible => version == 1;
	}

	public class DebugDataPayload : IPayload
	{
		public string graphName;

		public EntityId executionId;

		public RenderGraph.DebugData debugData;
	}

	public class AnalyticsPayload : IPayload
	{
		public GraphicsDeviceType graphicsDeviceType;

		public DeviceType deviceType;

		public string deviceModel;

		public string gpuVendor;

		public string gpuName;

		public AnalyticsPayload()
		{
			deviceModel = SystemInfo.deviceModel;
			deviceType = SystemInfo.deviceType;
			graphicsDeviceType = SystemInfo.graphicsDeviceType;
			gpuVendor = SystemInfo.graphicsDeviceVendor;
			gpuName = SystemInfo.graphicsDeviceName;
		}
	}

	internal const int k_Version = 1;

	private static readonly Guid s_EditorToPlayerGuid = new Guid("df519969-f421-4397-b2a1-1740abc989a0");

	private static readonly Guid s_PlayerToEditorGuid = new Guid("98d0787d-3917-4c48-8393-e313498046e6");

	private Action<MessageType, IPayload> m_UserCallback;

	private void InternalCallback(MessageEventArgs msg)
	{
		var (arg, arg2) = DeserializeMessage(msg.data);
		m_UserCallback(arg, arg2);
	}

	public void Register(Action<MessageType, IPayload> callback)
	{
		m_UserCallback = callback;
		PlayerConnection.instance.Register(s_EditorToPlayerGuid, InternalCallback);
	}

	public void UnregisterAll()
	{
		PlayerConnection.instance.Unregister(s_EditorToPlayerGuid, InternalCallback);
	}

	public void Send(MessageType messageType, IPayload payload = null)
	{
		PlayerConnection.instance.Send(s_PlayerToEditorGuid, SerializeMessage(messageType, payload));
	}

	internal static byte[] SerializeMessage(MessageType type, IPayload payload = null)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write((byte)type);
		switch (type)
		{
		case MessageType.DebugData:
			binaryWriter.Write(1);
			if (!(payload is DebugDataPayload debugDataPayload))
			{
				throw new InvalidOperationException("No valid payload provided");
			}
			binaryWriter.Write(debugDataPayload.graphName);
			binaryWriter.Write(debugDataPayload.executionId);
			binaryWriter.Write(RenderGraph.DebugDataSerialization.ToJson(debugDataPayload.debugData));
			break;
		case MessageType.AnalyticsData:
			binaryWriter.Write(1);
			if (!(payload is AnalyticsPayload analyticsPayload))
			{
				throw new InvalidOperationException("No valid payload provided");
			}
			binaryWriter.Write((int)analyticsPayload.graphicsDeviceType);
			binaryWriter.Write((int)analyticsPayload.deviceType);
			binaryWriter.Write(analyticsPayload.deviceModel);
			binaryWriter.Write(analyticsPayload.gpuVendor);
			binaryWriter.Write(analyticsPayload.gpuName);
			break;
		}
		return memoryStream.ToArray();
	}

	internal static (MessageType, IPayload) DeserializeMessage(byte[] data)
	{
		using MemoryStream input = new MemoryStream(data);
		using BinaryReader binaryReader = new BinaryReader(input);
		MessageType messageType = (MessageType)binaryReader.ReadByte();
		switch (messageType)
		{
		case MessageType.DebugData:
		{
			DebugDataPayload debugDataPayload = new DebugDataPayload();
			debugDataPayload.version = binaryReader.ReadInt32();
			if (!debugDataPayload.isCompatible)
			{
				Debug.LogWarning($"Render Graph Viewer message version mismatch (expected {1}, received {debugDataPayload.version})");
				return (messageType, debugDataPayload);
			}
			debugDataPayload.graphName = binaryReader.ReadString();
			debugDataPayload.executionId = binaryReader.ReadInt32();
			debugDataPayload.debugData = RenderGraph.DebugDataSerialization.FromJson(binaryReader.ReadString());
			return (messageType, debugDataPayload);
		}
		case MessageType.AnalyticsData:
		{
			AnalyticsPayload analyticsPayload = new AnalyticsPayload();
			analyticsPayload.version = binaryReader.ReadInt32();
			if (!analyticsPayload.isCompatible)
			{
				return (messageType, analyticsPayload);
			}
			analyticsPayload.graphicsDeviceType = (GraphicsDeviceType)binaryReader.ReadInt32();
			analyticsPayload.deviceType = (DeviceType)binaryReader.ReadInt32();
			analyticsPayload.deviceModel = binaryReader.ReadString();
			analyticsPayload.gpuVendor = binaryReader.ReadString();
			analyticsPayload.gpuName = binaryReader.ReadString();
			return (messageType, analyticsPayload);
		}
		default:
			return (messageType, null);
		}
	}
}
