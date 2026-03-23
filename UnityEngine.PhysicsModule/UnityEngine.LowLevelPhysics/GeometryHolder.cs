using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.LowLevelPhysics;

public struct GeometryHolder
{
	private int m_Type;

	private uint m_DataStart;

	private IntPtr m_FakePointer0;

	private IntPtr m_FakePointer1;

	private unsafe fixed uint m_Blob[6];

	public GeometryType Type => (GeometryType)m_Type;

	private unsafe void SetGeometry<T>(T geometry) where T : struct, IGeometry
	{
		m_Type = (int)geometry.GeometryType;
		UnsafeUtility.CopyStructureToPtr(ref geometry, UnsafeUtility.AddressOf(ref m_DataStart));
	}

	public unsafe T As<T>() where T : struct, IGeometry
	{
		T output = default(T);
		if (output.GeometryType != (GeometryType)m_Type)
		{
			throw new InvalidOperationException($"Unable to get geometry of type {output.GeometryType} from a geometry holder that stores {m_Type}.");
		}
		UnsafeUtility.CopyPtrToStructure<T>(UnsafeUtility.AddressOf(ref m_DataStart), out output);
		return output;
	}

	public static GeometryHolder Create<T>(T geometry) where T : struct, IGeometry
	{
		GeometryHolder result = new GeometryHolder
		{
			m_DataStart = 0u,
			m_Type = -1,
			m_FakePointer0 = new IntPtr(3735928559L),
			m_FakePointer1 = new IntPtr(3735928559L)
		};
		result.SetGeometry(geometry);
		return result;
	}
}
