using System;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D;

public readonly struct PhysicsDestructor
{
	public readonly struct FragmentGeometry
	{
		private readonly PhysicsTransform m_Transform;

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_Geometry;

		public FragmentGeometry(PhysicsTransform transform, ReadOnlySpan<PolygonGeometry> geometry)
		{
			m_Transform = transform;
			m_Geometry = PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(geometry);
		}
	}

	public readonly struct FragmentResult : IDisposable
	{
		private readonly PhysicsTransform m_Transform;

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_UnbrokenGeometryIslands;

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_UnbrokenGeometry;

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_BrokenGeometry;

		public PhysicsTransform transform => m_Transform;

		public NativeArray<RangeInt> unbrokenGeometryIslands => m_UnbrokenGeometryIslands.ToNativeArray<RangeInt>();

		public NativeArray<PolygonGeometry> unbrokenGeometry => m_UnbrokenGeometry.ToNativeArray<PolygonGeometry>();

		public NativeArray<PolygonGeometry> brokenGeometry => m_BrokenGeometry.ToNativeArray<PolygonGeometry>();

		public void Dispose()
		{
			unbrokenGeometryIslands.Dispose();
			unbrokenGeometry.Dispose();
			brokenGeometry.Dispose();
		}
	}

	public readonly struct SliceResult : IDisposable
	{
		private readonly PhysicsTransform m_Transform;

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_LeftGeometry;

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_RightGeometry;

		public PhysicsTransform transform => m_Transform;

		public NativeArray<PolygonGeometry> leftGeometry => m_LeftGeometry.ToNativeArray<PolygonGeometry>();

		public NativeArray<PolygonGeometry> rightGeometry => m_RightGeometry.ToNativeArray<PolygonGeometry>();

		public void Dispose()
		{
			leftGeometry.Dispose();
			rightGeometry.Dispose();
		}
	}

	public static FragmentResult Fragment(FragmentGeometry target, ReadOnlySpan<Vector2> fragmentPoints, Allocator allocator)
	{
		return PhysicsDestructorScripting2D.PhysicsDestructor_Fragment(target, fragmentPoints, allocator);
	}

	public static FragmentResult Fragment(FragmentGeometry target, FragmentGeometry mask, ReadOnlySpan<Vector2> fragmentPoints, Allocator allocator)
	{
		return PhysicsDestructorScripting2D.PhysicsDestructor_FragmentMasked(target, mask, fragmentPoints, allocator);
	}

	public static SliceResult Slice(FragmentGeometry target, Vector2 origin, Vector2 translation, Allocator allocator)
	{
		return PhysicsDestructorScripting2D.PhysicsDestructor_Slice(target, origin, translation, allocator);
	}
}
