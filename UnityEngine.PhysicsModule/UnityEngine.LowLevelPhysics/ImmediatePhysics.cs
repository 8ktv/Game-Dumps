using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.LowLevelPhysics;

[NativeHeader("Modules/Physics/ImmediatePhysics.h")]
public static class ImmediatePhysics
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Physics::Immediate::GenerateContacts", true)]
	private unsafe static extern int GenerateContacts_Native(void* geom1, void* geom2, void* xform1, void* xform2, int numPairs, void* contacts, int contactArrayLength, void* sizes, int sizesArrayLength, float contactDistance);

	public unsafe static int GenerateContacts(NativeArray<GeometryHolder>.ReadOnly geom1, NativeArray<GeometryHolder>.ReadOnly geom2, NativeArray<ImmediateTransform>.ReadOnly xform1, NativeArray<ImmediateTransform>.ReadOnly xform2, int pairCount, NativeArray<ImmediateContact> outContacts, NativeArray<int> outContactCounts, float contactDistance = 0.01f)
	{
		if (geom1.Length < pairCount || geom2.Length < pairCount || xform1.Length < pairCount || xform2.Length < pairCount)
		{
			throw new ArgumentException("Provided geometry or transform arrays are not large enough to fit the count of pairs.");
		}
		if (pairCount > outContactCounts.Length)
		{
			throw new ArgumentException("The output contact counts array is not big enough. The size of the array needs to match or exceed the amount of pairs.");
		}
		if (contactDistance <= 0f)
		{
			throw new ArgumentException("Contact distance must be positive and not equal to zero.");
		}
		return GenerateContacts_Native(geom1.GetUnsafeReadOnlyPtr(), geom2.GetUnsafeReadOnlyPtr(), xform1.GetUnsafeReadOnlyPtr(), xform2.GetUnsafeReadOnlyPtr(), pairCount, outContacts.GetUnsafePtr(), outContacts.Length, outContactCounts.GetUnsafePtr(), outContactCounts.Length, contactDistance);
	}
}
