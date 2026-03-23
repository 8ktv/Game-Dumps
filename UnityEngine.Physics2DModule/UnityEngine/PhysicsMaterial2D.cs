using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Physics2D/Public/PhysicsMaterial2D.h")]
public sealed class PhysicsMaterial2D : Object
{
	public float bounciness
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bounciness_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bounciness_Injected(intPtr, value);
		}
	}

	public float friction
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_friction_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_friction_Injected(intPtr, value);
		}
	}

	public PhysicsMaterialCombine2D frictionCombine
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_frictionCombine_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_frictionCombine_Injected(intPtr, value);
		}
	}

	public PhysicsMaterialCombine2D bounceCombine
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bounceCombine_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bounceCombine_Injected(intPtr, value);
		}
	}

	public PhysicsMaterial2D()
	{
		Create_Internal(this, null);
	}

	public PhysicsMaterial2D(string name)
	{
		Create_Internal(this, name);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern float GetCombinedValues(float valueA, float valueB, PhysicsMaterialCombine2D materialCombineA, PhysicsMaterialCombine2D materialCombineB);

	[NativeMethod("Create_Binding")]
	private unsafe static void Create_Internal([Writable] PhysicsMaterial2D scriptMaterial, string name)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					Create_Internal_Injected(scriptMaterial, ref managedSpanWrapper);
					return;
				}
			}
			Create_Internal_Injected(scriptMaterial, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Create_Internal_Injected([Writable] PhysicsMaterial2D scriptMaterial, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_bounciness_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bounciness_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_friction_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_friction_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsMaterialCombine2D get_frictionCombine_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_frictionCombine_Injected(IntPtr _unity_self, PhysicsMaterialCombine2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsMaterialCombine2D get_bounceCombine_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bounceCombine_Injected(IntPtr _unity_self, PhysicsMaterialCombine2D value);
}
