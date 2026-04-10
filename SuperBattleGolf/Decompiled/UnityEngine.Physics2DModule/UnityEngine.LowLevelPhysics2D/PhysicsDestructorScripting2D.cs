using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.LowLevelPhysics2D;

[StaticAccessor("PhysicsDestructor2D", StaticAccessorType.DoubleColon)]
[NativeHeader("Modules/Physics2D/LowLevel/PhysicsDestructor2D.h")]
internal static class PhysicsDestructorScripting2D
{
	[NativeMethod(Name = "Fragment", IsThreadSafe = true)]
	internal unsafe static PhysicsDestructor.FragmentResult PhysicsDestructor_Fragment(PhysicsDestructor.FragmentGeometry target, ReadOnlySpan<Vector2> fragmentPoints, Allocator allocator)
	{
		ReadOnlySpan<Vector2> readOnlySpan = fragmentPoints;
		PhysicsDestructor.FragmentResult ret;
		fixed (Vector2* begin = readOnlySpan)
		{
			ManagedSpanWrapper fragmentPoints2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsDestructor_Fragment_Injected(ref target, ref fragmentPoints2, allocator, out ret);
		}
		return ret;
	}

	[NativeMethod(Name = "FragmentMasked", IsThreadSafe = true)]
	internal unsafe static PhysicsDestructor.FragmentResult PhysicsDestructor_FragmentMasked(PhysicsDestructor.FragmentGeometry target, PhysicsDestructor.FragmentGeometry mask, ReadOnlySpan<Vector2> fragmentPoints, Allocator allocator)
	{
		ReadOnlySpan<Vector2> readOnlySpan = fragmentPoints;
		PhysicsDestructor.FragmentResult ret;
		fixed (Vector2* begin = readOnlySpan)
		{
			ManagedSpanWrapper fragmentPoints2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsDestructor_FragmentMasked_Injected(ref target, ref mask, ref fragmentPoints2, allocator, out ret);
		}
		return ret;
	}

	[NativeMethod(Name = "Slice", IsThreadSafe = true)]
	internal static PhysicsDestructor.SliceResult PhysicsDestructor_Slice(PhysicsDestructor.FragmentGeometry target, Vector2 origin, Vector2 translation, Allocator allocator)
	{
		PhysicsDestructor_Slice_Injected(ref target, ref origin, ref translation, allocator, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsDestructor_Fragment_Injected([In] ref PhysicsDestructor.FragmentGeometry target, ref ManagedSpanWrapper fragmentPoints, Allocator allocator, out PhysicsDestructor.FragmentResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsDestructor_FragmentMasked_Injected([In] ref PhysicsDestructor.FragmentGeometry target, [In] ref PhysicsDestructor.FragmentGeometry mask, ref ManagedSpanWrapper fragmentPoints, Allocator allocator, out PhysicsDestructor.FragmentResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsDestructor_Slice_Injected([In] ref PhysicsDestructor.FragmentGeometry target, [In] ref Vector2 origin, [In] ref Vector2 translation, Allocator allocator, out PhysicsDestructor.SliceResult ret);
}
