using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Audio;

[ExcludeFromPreset]
[ExcludeFromObjectFactory]
[NativeHeader("Modules/Audio/Public/AudioMixer.h")]
[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioMixer.bindings.h")]
public class AudioMixer : Object
{
	[NativeProperty]
	public AudioMixerGroup outputAudioMixerGroup
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<AudioMixerGroup>(get_outputAudioMixerGroup_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_outputAudioMixerGroup_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	[NativeProperty]
	public AudioMixerUpdateMode updateMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_updateMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_updateMode_Injected(intPtr, value);
		}
	}

	internal AudioMixer()
	{
	}

	[NativeMethod("FindSnapshotFromName")]
	public unsafe AudioMixerSnapshot FindSnapshot(string name)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		AudioMixerSnapshot result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = FindSnapshot_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			else
			{
				gcHandlePtr = FindSnapshot_Injected(intPtr, ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<AudioMixerSnapshot>(gcHandlePtr);
		}
		return result;
	}

	[NativeMethod("AudioMixerBindings::FindMatchingGroups", IsFreeFunction = true, HasExplicitThis = true)]
	public unsafe AudioMixerGroup[] FindMatchingGroups(string subPath)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(subPath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(subPath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return FindMatchingGroups_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return FindMatchingGroups_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	internal void TransitionToSnapshot(AudioMixerSnapshot snapshot, float timeToReach)
	{
		if (snapshot == null)
		{
			throw new ArgumentException("null Snapshot passed to AudioMixer.TransitionToSnapshot of AudioMixer '" + base.name + "'");
		}
		if (snapshot.audioMixer != this)
		{
			throw new ArgumentException("Snapshot '" + snapshot.name + "' passed to AudioMixer.TransitionToSnapshot is not a snapshot from AudioMixer '" + base.name + "'");
		}
		TransitionToSnapshotInternal(snapshot, timeToReach);
	}

	[NativeMethod("TransitionToSnapshot")]
	private void TransitionToSnapshotInternal(AudioMixerSnapshot snapshot, float timeToReach)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		TransitionToSnapshotInternal_Injected(intPtr, MarshalledUnityObject.Marshal(snapshot), timeToReach);
	}

	[NativeMethod("AudioMixerBindings::TransitionToSnapshots", IsFreeFunction = true, HasExplicitThis = true, ThrowsException = true)]
	public unsafe void TransitionToSnapshots(AudioMixerSnapshot[] snapshots, float[] weights, float timeToReach)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<float> span = new Span<float>(weights);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper weights2 = new ManagedSpanWrapper(begin, span.Length);
			TransitionToSnapshots_Injected(intPtr, snapshots, ref weights2, timeToReach);
		}
	}

	[NativeMethod]
	public unsafe bool SetFloat(string name, float value)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return SetFloat_Injected(intPtr, ref managedSpanWrapper, value);
				}
			}
			return SetFloat_Injected(intPtr, ref managedSpanWrapper, value);
		}
		finally
		{
		}
	}

	[NativeMethod]
	public unsafe bool ClearFloat(string name)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return ClearFloat_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return ClearFloat_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeMethod]
	public unsafe bool GetFloat(string name, out float value)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetFloat_Injected(intPtr, ref managedSpanWrapper, out value);
				}
			}
			return GetFloat_Injected(intPtr, ref managedSpanWrapper, out value);
		}
		finally
		{
		}
	}

	[NativeMethod("AudioMixerBindings::GetAbsoluteAudibilityFromGroup", HasExplicitThis = true, IsFreeFunction = true)]
	internal float GetAbsoluteAudibilityFromGroup(AudioMixerGroup group)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAbsoluteAudibilityFromGroup_Injected(intPtr, MarshalledUnityObject.Marshal(group));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_outputAudioMixerGroup_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_outputAudioMixerGroup_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr FindSnapshot_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AudioMixerGroup[] FindMatchingGroups_Injected(IntPtr _unity_self, ref ManagedSpanWrapper subPath);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TransitionToSnapshotInternal_Injected(IntPtr _unity_self, IntPtr snapshot, float timeToReach);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TransitionToSnapshots_Injected(IntPtr _unity_self, AudioMixerSnapshot[] snapshots, ref ManagedSpanWrapper weights, float timeToReach);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AudioMixerUpdateMode get_updateMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_updateMode_Injected(IntPtr _unity_self, AudioMixerUpdateMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetFloat_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ClearFloat_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetFloat_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, out float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetAbsoluteAudibilityFromGroup_Injected(IntPtr _unity_self, IntPtr group);
}
