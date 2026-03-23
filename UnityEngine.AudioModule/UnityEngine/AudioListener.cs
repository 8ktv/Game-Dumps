using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[StaticAccessor("AudioListenerBindings", StaticAccessorType.DoubleColon)]
public sealed class AudioListener : AudioBehaviour
{
	public static extern float volume
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("ListenerPause")]
	public static extern bool pause
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public AudioVelocityUpdateMode velocityUpdateMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_velocityUpdateMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_velocityUpdateMode_Injected(intPtr, value);
		}
	}

	[NativeThrows]
	private unsafe static void GetOutputDataHelper([Out] float[] samples, int channel)
	{
		//The blocks IL_001b are reachable both inside and outside the pinned region starting at IL_0004. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper samples2 = default(BlittableArrayWrapper);
		try
		{
			if (samples != null)
			{
				fixed (float[] array = samples)
				{
					if (array.Length != 0)
					{
						samples2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetOutputDataHelper_Injected(out samples2, channel);
					return;
				}
			}
			GetOutputDataHelper_Injected(out samples2, channel);
		}
		finally
		{
			samples2.Unmarshal(ref array);
		}
	}

	[NativeThrows]
	private unsafe static void GetSpectrumDataHelper([Out] float[] samples, int channel, FFTWindow window)
	{
		//The blocks IL_001b are reachable both inside and outside the pinned region starting at IL_0004. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper samples2 = default(BlittableArrayWrapper);
		try
		{
			if (samples != null)
			{
				fixed (float[] array = samples)
				{
					if (array.Length != 0)
					{
						samples2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetSpectrumDataHelper_Injected(out samples2, channel, window);
					return;
				}
			}
			GetSpectrumDataHelper_Injected(out samples2, channel, window);
		}
		finally
		{
			samples2.Unmarshal(ref array);
		}
	}

	[Obsolete("GetOutputData returning a float[] is deprecated, use GetOutputData and pass a pre allocated array instead.")]
	public static float[] GetOutputData(int numSamples, int channel)
	{
		float[] array = new float[numSamples];
		GetOutputDataHelper(array, channel);
		return array;
	}

	public static void GetOutputData(float[] samples, int channel)
	{
		GetOutputDataHelper(samples, channel);
	}

	[Obsolete("GetSpectrumData returning a float[] is deprecated, use GetSpectrumData and pass a pre allocated array instead.")]
	public static float[] GetSpectrumData(int numSamples, int channel, FFTWindow window)
	{
		float[] array = new float[numSamples];
		GetSpectrumDataHelper(array, channel, window);
		return array;
	}

	public static void GetSpectrumData(float[] samples, int channel, FFTWindow window)
	{
		GetSpectrumDataHelper(samples, channel, window);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetOutputDataHelper_Injected(out BlittableArrayWrapper samples, int channel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSpectrumDataHelper_Injected(out BlittableArrayWrapper samples, int channel, FFTWindow window);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AudioVelocityUpdateMode get_velocityUpdateMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_velocityUpdateMode_Injected(IntPtr _unity_self, AudioVelocityUpdateMode value);
}
