using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[RequireComponent(typeof(AudioBehaviour))]
public sealed class AudioLowPassFilter : Behaviour
{
	public AnimationCurve customCutoffCurve
	{
		get
		{
			return GetCustomLowpassLevelCurveCopy();
		}
		set
		{
			SetCustomLowpassLevelCurveHelper(this, value);
		}
	}

	public float cutoffFrequency
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cutoffFrequency_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cutoffFrequency_Injected(intPtr, value);
		}
	}

	public float lowpassResonanceQ
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_lowpassResonanceQ_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_lowpassResonanceQ_Injected(intPtr, value);
		}
	}

	private AnimationCurve GetCustomLowpassLevelCurveCopy()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr customLowpassLevelCurveCopy_Injected = GetCustomLowpassLevelCurveCopy_Injected(intPtr);
		return (customLowpassLevelCurveCopy_Injected == (IntPtr)0) ? null : AnimationCurve.BindingsMarshaller.ConvertToManaged(customLowpassLevelCurveCopy_Injected);
	}

	[NativeMethod(Name = "AudioLowPassFilterBindings::SetCustomLowpassLevelCurveHelper", IsFreeFunction = true)]
	[NativeThrows]
	private static void SetCustomLowpassLevelCurveHelper([NotNull] AudioLowPassFilter source, AnimationCurve curve)
	{
		if ((object)source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(source);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		SetCustomLowpassLevelCurveHelper_Injected(intPtr, (curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetCustomLowpassLevelCurveCopy_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCustomLowpassLevelCurveHelper_Injected(IntPtr source, IntPtr curve);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_cutoffFrequency_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cutoffFrequency_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_lowpassResonanceQ_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lowpassResonanceQ_Injected(IntPtr _unity_self, float value);
}
