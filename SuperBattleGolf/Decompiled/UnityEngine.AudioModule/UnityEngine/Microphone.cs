using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[StaticAccessor("GetAudioManager()", StaticAccessorType.Dot)]
public sealed class Microphone
{
	public static extern string[] devices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetRecordDevices")]
		get;
	}

	internal static extern bool isAnyDeviceRecording
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("IsAnyRecordDeviceActive")]
		get;
	}

	[NativeMethod(IsThreadSafe = true)]
	private unsafe static int GetMicrophoneDeviceIDFromName(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetMicrophoneDeviceIDFromName_Injected(ref managedSpanWrapper);
				}
			}
			return GetMicrophoneDeviceIDFromName_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	private static AudioClip StartRecord(int deviceID, bool loop, float lengthSec, int frequency)
	{
		return Unmarshal.UnmarshalUnityObject<AudioClip>(StartRecord_Injected(deviceID, loop, lengthSec, frequency));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EndRecord(int deviceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsRecording(int deviceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private static extern int GetRecordPosition(int deviceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDeviceCaps(int deviceID, out int minFreq, out int maxFreq);

	public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency)
	{
		int microphoneDeviceIDFromName = GetMicrophoneDeviceIDFromName(deviceName);
		if (microphoneDeviceIDFromName == -1)
		{
			throw new ArgumentException("Couldn't acquire device ID for device name " + deviceName);
		}
		if (lengthSec <= 0)
		{
			throw new ArgumentException("Length of recording must be greater than zero seconds (was: " + lengthSec + " seconds)");
		}
		if (lengthSec > 3600)
		{
			throw new ArgumentException("Length of recording must be less than one hour (was: " + lengthSec + " seconds)");
		}
		if (frequency <= 0)
		{
			throw new ArgumentException("Frequency of recording must be greater than zero (was: " + frequency + " Hz)");
		}
		return StartRecord(microphoneDeviceIDFromName, loop, lengthSec, frequency);
	}

	public static void End(string deviceName)
	{
		int microphoneDeviceIDFromName = GetMicrophoneDeviceIDFromName(deviceName);
		if (microphoneDeviceIDFromName != -1)
		{
			EndRecord(microphoneDeviceIDFromName);
		}
	}

	public static bool IsRecording(string deviceName)
	{
		int microphoneDeviceIDFromName = GetMicrophoneDeviceIDFromName(deviceName);
		if (microphoneDeviceIDFromName == -1)
		{
			return false;
		}
		return IsRecording(microphoneDeviceIDFromName);
	}

	public static int GetPosition(string deviceName)
	{
		int microphoneDeviceIDFromName = GetMicrophoneDeviceIDFromName(deviceName);
		if (microphoneDeviceIDFromName == -1)
		{
			return 0;
		}
		return GetRecordPosition(microphoneDeviceIDFromName);
	}

	public static void GetDeviceCaps(string deviceName, out int minFreq, out int maxFreq)
	{
		minFreq = 0;
		maxFreq = 0;
		int microphoneDeviceIDFromName = GetMicrophoneDeviceIDFromName(deviceName);
		if (microphoneDeviceIDFromName != -1)
		{
			GetDeviceCaps(microphoneDeviceIDFromName, out minFreq, out maxFreq);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetMicrophoneDeviceIDFromName_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr StartRecord_Injected(int deviceID, bool loop, float lengthSec, int frequency);
}
