using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeType("Modules/Animation/AnimationClip.h")]
[NativeHeader("Modules/Animation/ScriptBindings/AnimationClip.bindings.h")]
public sealed class AnimationClip : Motion
{
	[NativeProperty("Length", false, TargetType.Function)]
	public float length
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_length_Injected(intPtr);
		}
	}

	[NativeProperty("StartTime", false, TargetType.Function)]
	internal float startTime
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_startTime_Injected(intPtr);
		}
	}

	[NativeProperty("StopTime", false, TargetType.Function)]
	internal float stopTime
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stopTime_Injected(intPtr);
		}
	}

	[NativeProperty("SampleRate", false, TargetType.Function)]
	public float frameRate
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_frameRate_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_frameRate_Injected(intPtr, value);
		}
	}

	[NativeProperty("WrapMode", false, TargetType.Function)]
	public WrapMode wrapMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wrapMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wrapMode_Injected(intPtr, value);
		}
	}

	[NativeProperty("Bounds", false, TargetType.Function)]
	public Bounds localBounds
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localBounds_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_localBounds_Injected(intPtr, ref value);
		}
	}

	public new bool legacy
	{
		[NativeMethod("IsLegacy")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_legacy_Injected(intPtr);
		}
		[NativeMethod("SetLegacy")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_legacy_Injected(intPtr, value);
		}
	}

	public bool humanMotion
	{
		[NativeMethod("IsHumanMotion")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_humanMotion_Injected(intPtr);
		}
	}

	public bool empty
	{
		[NativeMethod("IsEmpty")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_empty_Injected(intPtr);
		}
	}

	public bool hasGenericRootTransform
	{
		[NativeMethod("HasGenericRootTransform")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasGenericRootTransform_Injected(intPtr);
		}
	}

	public bool hasMotionFloatCurves
	{
		[NativeMethod("HasMotionFloatCurves")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasMotionFloatCurves_Injected(intPtr);
		}
	}

	public bool hasMotionCurves
	{
		[NativeMethod("HasMotionCurves")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasMotionCurves_Injected(intPtr);
		}
	}

	public bool hasRootCurves
	{
		[NativeMethod("HasRootCurves")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasRootCurves_Injected(intPtr);
		}
	}

	internal bool hasRootMotion
	{
		[FreeFunction(Name = "AnimationClipBindings::Internal_GetHasRootMotion", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasRootMotion_Injected(intPtr);
		}
	}

	public unsafe AnimationEvent[] events
	{
		get
		{
			GetEventsInternal(out var values, out var size);
			AnimationEvent[] result = AnimationEventBlittable.PointerToAnimationEvents(values, size);
			AnimationEventBlittable.DisposeEvents(values, size);
			return result;
		}
		set
		{
			using NativeArray<AnimationEventBlittable> nativeArray = new NativeArray<AnimationEventBlittable>(value.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			AnimationEventBlittable* ptr = (AnimationEventBlittable*)nativeArray.GetUnsafePtr();
			AnimationEventBlittable.FromAnimationEvents(value, ptr);
			SetEventsInternal(ptr, nativeArray.Length);
			for (int i = 0; i < value.Length; i++)
			{
				ptr->Dispose();
				ptr++;
			}
		}
	}

	public AnimationClip()
	{
		Internal_CreateAnimationClip(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("AnimationClipBindings::Internal_CreateAnimationClip")]
	private static extern void Internal_CreateAnimationClip([Writable] AnimationClip self);

	public void SampleAnimation(GameObject go, float time)
	{
		SampleAnimation(go, this, time, wrapMode);
	}

	[FreeFunction]
	[NativeHeader("Modules/Animation/AnimationUtility.h")]
	internal static void SampleAnimation([NotNull] GameObject go, [NotNull] AnimationClip clip, float inTime, WrapMode wrapMode)
	{
		if ((object)go == null)
		{
			ThrowHelper.ThrowArgumentNullException(go, "go");
		}
		if ((object)clip == null)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(go);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(go, "go");
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(clip);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		SampleAnimation_Injected(intPtr, intPtr2, inTime, wrapMode);
	}

	[FreeFunction("AnimationClipBindings::Internal_SetCurve", HasExplicitThis = true)]
	public unsafe void SetCurve([NotNull] string relativePath, [NotNull] Type type, [NotNull] string propertyName, AnimationCurve curve)
	{
		//The blocks IL_0066, IL_0074, IL_0082, IL_0090, IL_0095, IL_009c, IL_00a5, IL_00a8 are reachable both inside and outside the pinned region starting at IL_0055. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0095, IL_009c, IL_00a5, IL_00a8 are reachable both inside and outside the pinned region starting at IL_0082. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0095, IL_009c, IL_00a5, IL_00a8 are reachable both inside and outside the pinned region starting at IL_0082. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if (relativePath == null)
		{
			ThrowHelper.ThrowArgumentNullException(relativePath, "relativePath");
		}
		if ((object)type == null)
		{
			ThrowHelper.ThrowArgumentNullException(type, "type");
		}
		if (propertyName == null)
		{
			ThrowHelper.ThrowArgumentNullException(propertyName, "propertyName");
		}
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper relativePath2;
			Type type2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			ref ManagedSpanWrapper propertyName2;
			IntPtr curve2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(relativePath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(relativePath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					relativePath2 = ref managedSpanWrapper;
					type2 = type;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyName, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(propertyName);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							propertyName2 = ref managedSpanWrapper2;
							curve2 = ((curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
							SetCurve_Injected(intPtr, ref relativePath2, type2, ref propertyName2, curve2);
							return;
						}
					}
					propertyName2 = ref managedSpanWrapper2;
					curve2 = ((curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
					SetCurve_Injected(intPtr, ref relativePath2, type2, ref propertyName2, curve2);
					return;
				}
			}
			relativePath2 = ref managedSpanWrapper;
			type2 = type;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyName, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(propertyName);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					propertyName2 = ref managedSpanWrapper2;
					curve2 = ((curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
					SetCurve_Injected(intPtr, ref relativePath2, type2, ref propertyName2, curve2);
					return;
				}
			}
			propertyName2 = ref managedSpanWrapper2;
			curve2 = ((curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
			SetCurve_Injected(intPtr, ref relativePath2, type2, ref propertyName2, curve2);
		}
		finally
		{
		}
	}

	public void EnsureQuaternionContinuity()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		EnsureQuaternionContinuity_Injected(intPtr);
	}

	public void ClearCurves()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearCurves_Injected(intPtr);
	}

	public void AddEvent(AnimationEvent evt)
	{
		if (evt == null)
		{
			throw new ArgumentNullException("evt");
		}
		AnimationEventBlittable animationEventBlittable = AnimationEventBlittable.FromAnimationEvent(evt);
		AddEventInternal(animationEventBlittable);
		animationEventBlittable.Dispose();
	}

	[FreeFunction(Name = "AnimationClipBindings::AddEventInternal", HasExplicitThis = true)]
	private void AddEventInternal(object evt)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddEventInternal_Injected(intPtr, evt);
	}

	[FreeFunction(Name = "AnimationClipBindings::SetEventsInternal", HasExplicitThis = true)]
	private unsafe void SetEventsInternal(void* data, int length)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetEventsInternal_Injected(intPtr, data, length);
	}

	[FreeFunction(Name = "AnimationClipBindings::GetEventsInternal", HasExplicitThis = true)]
	private void GetEventsInternal(out IntPtr values, out int size)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetEventsInternal_Injected(intPtr, out values, out size);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SampleAnimation_Injected(IntPtr go, IntPtr clip, float inTime, WrapMode wrapMode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_length_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_startTime_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_stopTime_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_frameRate_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_frameRate_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCurve_Injected(IntPtr _unity_self, ref ManagedSpanWrapper relativePath, Type type, ref ManagedSpanWrapper propertyName, IntPtr curve);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnsureQuaternionContinuity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearCurves_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern WrapMode get_wrapMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wrapMode_Injected(IntPtr _unity_self, WrapMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localBounds_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_localBounds_Injected(IntPtr _unity_self, [In] ref Bounds value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_legacy_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_legacy_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_humanMotion_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_empty_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasGenericRootTransform_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasMotionFloatCurves_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasMotionCurves_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasRootCurves_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasRootMotion_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddEventInternal_Injected(IntPtr _unity_self, object evt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void SetEventsInternal_Injected(IntPtr _unity_self, void* data, int length);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetEventsInternal_Injected(IntPtr _unity_self, out IntPtr values, out int size);
}
