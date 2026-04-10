using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[NativeHeader("Modules/Animation/ScriptBindings/Animation.bindings.h")]
[NativeHeader("Modules/Animation/AnimatorOverrideController.h")]
[HelpURL("AnimatorOverrideController")]
public class AnimatorOverrideController : RuntimeAnimatorController
{
	internal delegate void OnOverrideControllerDirtyCallback();

	internal OnOverrideControllerDirtyCallback OnOverrideControllerDirty;

	public RuntimeAnimatorController runtimeAnimatorController
	{
		[NativeMethod("GetAnimatorController")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RuntimeAnimatorController>(get_runtimeAnimatorController_Injected(intPtr));
		}
		[NativeMethod("SetAnimatorController")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_runtimeAnimatorController_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public AnimationClip this[string name]
	{
		get
		{
			return Internal_GetClipByName(name, returnEffectiveClip: true);
		}
		set
		{
			Internal_SetClipByName(name, value);
		}
	}

	public AnimationClip this[AnimationClip clip]
	{
		get
		{
			return GetClip(clip, returnEffectiveClip: true);
		}
		set
		{
			SetClip(clip, value, notify: true);
		}
	}

	public int overridesCount
	{
		[NativeMethod("GetOriginalClipsCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_overridesCount_Injected(intPtr);
		}
	}

	[Obsolete("AnimatorOverrideController.clips property is deprecated. Use AnimatorOverrideController.GetOverrides and AnimatorOverrideController.ApplyOverrides instead.")]
	public AnimationClipPair[] clips
	{
		get
		{
			int num = overridesCount;
			AnimationClipPair[] array = new AnimationClipPair[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new AnimationClipPair();
				array[i].originalClip = GetOriginalClip(i);
				array[i].overrideClip = GetOverrideClip(array[i].originalClip);
			}
			return array;
		}
		set
		{
			for (int i = 0; i < value.Length; i++)
			{
				SetClip(value[i].originalClip, value[i].overrideClip, notify: false);
			}
			SendNotification();
		}
	}

	public AnimatorOverrideController()
	{
		Internal_Create(this, null);
		OnOverrideControllerDirty = null;
	}

	public AnimatorOverrideController(RuntimeAnimatorController controller)
	{
		Internal_Create(this, controller);
		OnOverrideControllerDirty = null;
	}

	[FreeFunction("AnimationBindings::CreateAnimatorOverrideController")]
	private static void Internal_Create([Writable] AnimatorOverrideController self, RuntimeAnimatorController controller)
	{
		Internal_Create_Injected(self, MarshalledUnityObject.Marshal(controller));
	}

	[NativeMethod("GetClip")]
	private unsafe AnimationClip Internal_GetClipByName(string name, bool returnEffectiveClip)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		AnimationClip result;
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
					gcHandlePtr = Internal_GetClipByName_Injected(intPtr, ref managedSpanWrapper, returnEffectiveClip);
				}
			}
			else
			{
				gcHandlePtr = Internal_GetClipByName_Injected(intPtr, ref managedSpanWrapper, returnEffectiveClip);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<AnimationClip>(gcHandlePtr);
		}
		return result;
	}

	[NativeMethod("SetClip")]
	private unsafe void Internal_SetClipByName(string name, AnimationClip clip)
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
					Internal_SetClipByName_Injected(intPtr, ref managedSpanWrapper, MarshalledUnityObject.Marshal(clip));
					return;
				}
			}
			Internal_SetClipByName_Injected(intPtr, ref managedSpanWrapper, MarshalledUnityObject.Marshal(clip));
		}
		finally
		{
		}
	}

	private AnimationClip GetClip(AnimationClip originalClip, bool returnEffectiveClip)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<AnimationClip>(GetClip_Injected(intPtr, MarshalledUnityObject.Marshal(originalClip), returnEffectiveClip));
	}

	private void SetClip(AnimationClip originalClip, AnimationClip overrideClip, bool notify)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetClip_Injected(intPtr, MarshalledUnityObject.Marshal(originalClip), MarshalledUnityObject.Marshal(overrideClip), notify);
	}

	private void SendNotification()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SendNotification_Injected(intPtr);
	}

	private AnimationClip GetOriginalClip(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<AnimationClip>(GetOriginalClip_Injected(intPtr, index));
	}

	private AnimationClip GetOverrideClip(AnimationClip originalClip)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<AnimationClip>(GetOverrideClip_Injected(intPtr, MarshalledUnityObject.Marshal(originalClip)));
	}

	public void GetOverrides(List<KeyValuePair<AnimationClip, AnimationClip>> overrides)
	{
		if (overrides == null)
		{
			throw new ArgumentNullException("overrides");
		}
		int num = overridesCount;
		if (overrides.Capacity < num)
		{
			overrides.Capacity = num;
		}
		overrides.Clear();
		for (int i = 0; i < num; i++)
		{
			AnimationClip originalClip = GetOriginalClip(i);
			overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(originalClip, GetOverrideClip(originalClip)));
		}
	}

	public void ApplyOverrides(IList<KeyValuePair<AnimationClip, AnimationClip>> overrides)
	{
		if (overrides == null)
		{
			throw new ArgumentNullException("overrides");
		}
		for (int i = 0; i < overrides.Count; i++)
		{
			SetClip(overrides[i].Key, overrides[i].Value, notify: false);
		}
		SendNotification();
	}

	[NativeConditional("UNITY_EDITOR")]
	internal void PerformOverrideClipListCleanup()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		PerformOverrideClipListCleanup_Injected(intPtr);
	}

	[NativeConditional("UNITY_EDITOR")]
	[RequiredByNativeCode]
	internal static void OnInvalidateOverrideController(AnimatorOverrideController controller)
	{
		if (controller.OnOverrideControllerDirty != null)
		{
			controller.OnOverrideControllerDirty();
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Create_Injected([Writable] AnimatorOverrideController self, IntPtr controller);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_runtimeAnimatorController_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_runtimeAnimatorController_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_GetClipByName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, bool returnEffectiveClip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetClipByName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, IntPtr clip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetClip_Injected(IntPtr _unity_self, IntPtr originalClip, bool returnEffectiveClip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetClip_Injected(IntPtr _unity_self, IntPtr originalClip, IntPtr overrideClip, bool notify);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendNotification_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetOriginalClip_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetOverrideClip_Injected(IntPtr _unity_self, IntPtr originalClip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_overridesCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PerformOverrideClipListCleanup_Injected(IntPtr _unity_self);
}
