using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine;

[UsedByNativeCode]
[MovedFrom(true, "UnityEditor.Animations", "UnityEditor", null)]
[NativeHeader("Modules/Animation/AvatarMask.h")]
[NativeHeader("Modules/Animation/ScriptBindings/Animation.bindings.h")]
public sealed class AvatarMask : Object
{
	[Obsolete("AvatarMask.humanoidBodyPartCount is deprecated, use AvatarMaskBodyPart.LastBodyPart instead.")]
	public int humanoidBodyPartCount => 13;

	public int transformCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_transformCount_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_transformCount_Injected(intPtr, value);
		}
	}

	internal bool hasFeetIK
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasFeetIK_Injected(intPtr);
		}
	}

	public AvatarMask()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("AnimationBindings::CreateAvatarMask")]
	private static extern void Internal_Create([Writable] AvatarMask self);

	[NativeMethod("GetBodyPart")]
	public bool GetHumanoidBodyPartActive(AvatarMaskBodyPart index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetHumanoidBodyPartActive_Injected(intPtr, index);
	}

	[NativeMethod("SetBodyPart")]
	public void SetHumanoidBodyPartActive(AvatarMaskBodyPart index, bool value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetHumanoidBodyPartActive_Injected(intPtr, index, value);
	}

	public void AddTransformPath(Transform transform)
	{
		AddTransformPath(transform, recursive: true);
	}

	public void AddTransformPath([NotNull] Transform transform, [DefaultValue("true")] bool recursive)
	{
		if ((object)transform == null)
		{
			ThrowHelper.ThrowArgumentNullException(transform, "transform");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(transform);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(transform, "transform");
		}
		AddTransformPath_Injected(intPtr, intPtr2, recursive);
	}

	public void RemoveTransformPath(Transform transform)
	{
		RemoveTransformPath(transform, recursive: true);
	}

	public void RemoveTransformPath([NotNull] Transform transform, [DefaultValue("true")] bool recursive)
	{
		if ((object)transform == null)
		{
			ThrowHelper.ThrowArgumentNullException(transform, "transform");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(transform);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(transform, "transform");
		}
		RemoveTransformPath_Injected(intPtr, intPtr2, recursive);
	}

	public string GetTransformPath(int index)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetTransformPath_Injected(intPtr, index, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	public unsafe void SetTransformPath(int index, string path)
	{
		//The blocks IL_003a are reachable both inside and outside the pinned region starting at IL_0029. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(path);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetTransformPath_Injected(intPtr, index, ref managedSpanWrapper);
					return;
				}
			}
			SetTransformPath_Injected(intPtr, index, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	private float GetTransformWeight(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetTransformWeight_Injected(intPtr, index);
	}

	private void SetTransformWeight(int index, float weight)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTransformWeight_Injected(intPtr, index, weight);
	}

	public bool GetTransformActive(int index)
	{
		return GetTransformWeight(index) > 0.5f;
	}

	public void SetTransformActive(int index, bool value)
	{
		SetTransformWeight(index, value ? 1f : 0f);
	}

	internal void Copy(AvatarMask other)
	{
		for (AvatarMaskBodyPart avatarMaskBodyPart = AvatarMaskBodyPart.Root; avatarMaskBodyPart < AvatarMaskBodyPart.LastBodyPart; avatarMaskBodyPart++)
		{
			SetHumanoidBodyPartActive(avatarMaskBodyPart, other.GetHumanoidBodyPartActive(avatarMaskBodyPart));
		}
		transformCount = other.transformCount;
		for (int i = 0; i < other.transformCount; i++)
		{
			SetTransformPath(i, other.GetTransformPath(i));
			SetTransformActive(i, other.GetTransformActive(i));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetHumanoidBodyPartActive_Injected(IntPtr _unity_self, AvatarMaskBodyPart index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetHumanoidBodyPartActive_Injected(IntPtr _unity_self, AvatarMaskBodyPart index, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_transformCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_transformCount_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddTransformPath_Injected(IntPtr _unity_self, IntPtr transform, [DefaultValue("true")] bool recursive);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveTransformPath_Injected(IntPtr _unity_self, IntPtr transform, [DefaultValue("true")] bool recursive);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTransformPath_Injected(IntPtr _unity_self, int index, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTransformPath_Injected(IntPtr _unity_self, int index, ref ManagedSpanWrapper path);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetTransformWeight_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTransformWeight_Injected(IntPtr _unity_self, int index, float weight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasFeetIK_Injected(IntPtr _unity_self);
}
