using System;
using System.Runtime.InteropServices;

namespace UnityEngine;

[StructLayout(LayoutKind.Explicit)]
public struct IntegrationInfo
{
	[Flags]
	internal enum SupportedUnityFeatures
	{
		None = 0,
		DynamicsSupport = 2,
		SDKVisualDebuggerSupport = 4,
		ArticulationSupport = 8,
		ImmediateModeSupport = 0x10,
		VehicleSupport = 0x20,
		CharacterControllerSupport = 0x40
	}

	internal const uint k_InvalidID = 0u;

	internal const uint k_FallbackIntegrationId = 3737844653u;

	[FieldOffset(0)]
	private readonly uint m_Id;

	[FieldOffset(4)]
	private unsafe fixed ushort m_IntegrationVersion[3];

	[FieldOffset(10)]
	private unsafe fixed ushort m_SdkVersion[3];

	[FieldOffset(16)]
	private readonly SupportedUnityFeatures m_Features;

	[FieldOffset(20)]
	private unsafe fixed byte m_Name[16];

	[FieldOffset(36)]
	private unsafe fixed byte m_Desc[220];

	public readonly uint id => m_Id;

	public unsafe string name
	{
		get
		{
			fixed (byte* value = m_Name)
			{
				return Marshal.PtrToStringAnsi(new IntPtr(value));
			}
		}
	}

	public unsafe string description
	{
		get
		{
			fixed (byte* desc = m_Desc)
			{
				return Marshal.PtrToStringAnsi(new IntPtr(desc));
			}
		}
	}

	public bool isFallback => id == 3737844653u;
}
