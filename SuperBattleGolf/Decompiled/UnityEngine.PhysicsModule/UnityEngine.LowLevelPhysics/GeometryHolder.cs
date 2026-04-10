using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.LowLevelPhysics;

public struct GeometryHolder
{
	internal unsafe fixed int m_Data[12];

	public unsafe GeometryType Type => (GeometryType)m_Data[0];

	public unsafe T As<T>() where T : struct, IGeometry
	{
		T output = default(T);
		if (output.GeometryType != Type)
		{
			throw new InvalidOperationException($"Unable to get geometry of type {output.GeometryType} from a geometry holder that stores {Type}.");
		}
		UnsafeUtility.CopyPtrToStructure<T>(UnsafeUtility.AddressOf(ref this), out output);
		return output;
	}

	public unsafe static GeometryHolder Create<T>(T geometry) where T : struct, IGeometry
	{
		GeometryHolder output = default(GeometryHolder);
		UnsafeUtility.CopyStructureToPtr(ref geometry, UnsafeUtility.AddressOf(ref output));
		ref int data = ref output.m_Data[0];
		data = (int)geometry.GeometryType;
		return output;
	}
}
