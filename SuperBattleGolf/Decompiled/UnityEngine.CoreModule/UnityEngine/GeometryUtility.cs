using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[StaticAccessor("GeometryUtilityScripting", StaticAccessorType.DoubleColon)]
public sealed class GeometryUtility
{
	public static Plane[] CalculateFrustumPlanes(Camera camera)
	{
		Plane[] array = new Plane[6];
		CalculateFrustumPlanes(camera, MemoryExtensions.AsSpan(array));
		return array;
	}

	public static Plane[] CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix)
	{
		Plane[] array = new Plane[6];
		CalculateFrustumPlanes(in worldToProjectionMatrix, MemoryExtensions.AsSpan(array));
		return array;
	}

	public static Plane[] CalculateFrustumPlanes(in Matrix4x4 worldToProjectionMatrix)
	{
		Plane[] array = new Plane[6];
		CalculateFrustumPlanes(in worldToProjectionMatrix, MemoryExtensions.AsSpan(array));
		return array;
	}

	public static void CalculateFrustumPlanes(Camera camera, Span<Plane> planes)
	{
		GeometryUtility.CalculateFrustumPlanes(in ILSpyHelper_AsRefReadOnly(camera.projectionMatrix * camera.worldToCameraMatrix), planes);
		static ref readonly T ILSpyHelper_AsRefReadOnly<T>(in T temp)
		{
			//ILSpy generated this function to help ensure overload resolution can pick the overload using 'in'
			return ref temp;
		}
	}

	public static void CalculateFrustumPlanes(Camera camera, Plane[] planes)
	{
		GeometryUtility.CalculateFrustumPlanes(in ILSpyHelper_AsRefReadOnly(camera.projectionMatrix * camera.worldToCameraMatrix), MemoryExtensions.AsSpan(planes));
		static ref readonly T ILSpyHelper_AsRefReadOnly<T>(in T temp)
		{
			//ILSpy generated this function to help ensure overload resolution can pick the overload using 'in'
			return ref temp;
		}
	}

	public static void CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix, Span<Plane> planes)
	{
		if (planes == null)
		{
			throw new ArgumentNullException("planes");
		}
		if (planes.Length != 6)
		{
			throw new ArgumentException("Planes array must be of length 6.", "planes");
		}
		Internal_ExtractPlanes(planes, in worldToProjectionMatrix);
	}

	public static void CalculateFrustumPlanes(in Matrix4x4 worldToProjectionMatrix, Span<Plane> planes)
	{
		if (planes == null)
		{
			throw new ArgumentNullException("planes");
		}
		if (planes.Length != 6)
		{
			throw new ArgumentException("Planes array must be of length 6.", "planes");
		}
		Internal_ExtractPlanes(planes, in worldToProjectionMatrix);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix, Plane[] planes)
	{
		CalculateFrustumPlanes(in worldToProjectionMatrix, MemoryExtensions.AsSpan(planes));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CalculateFrustumPlanes(in Matrix4x4 worldToProjectionMatrix, Plane[] planes)
	{
		CalculateFrustumPlanes(in worldToProjectionMatrix, MemoryExtensions.AsSpan(planes));
	}

	public static Bounds CalculateBounds(Vector3[] positions, Matrix4x4 transform)
	{
		if (positions == null)
		{
			throw new ArgumentNullException("positions");
		}
		if (positions.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.", "positions");
		}
		return Internal_CalculateBounds(positions, in transform);
	}

	public static Bounds CalculateBounds(Vector3[] positions, in Matrix4x4 transform)
	{
		if (positions == null)
		{
			throw new ArgumentNullException("positions");
		}
		if (positions.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.", "positions");
		}
		return Internal_CalculateBounds(positions, in transform);
	}

	public static bool TryCreatePlaneFromPolygon(Vector3[] vertices, out Plane plane)
	{
		if (vertices == null || vertices.Length < 3)
		{
			plane = new Plane(Vector3.up, 0f);
			return false;
		}
		if (vertices.Length == 3)
		{
			Vector3 a = vertices[0];
			Vector3 b = vertices[1];
			Vector3 c = vertices[2];
			plane = new Plane(in a, in b, in c);
			return plane.normal.sqrMagnitude > 0f;
		}
		Vector3 lhs = Vector3.zero;
		int num = vertices.Length - 1;
		Vector3 vector = vertices[num];
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector2 = vertices[i];
			lhs.x += (vector.y - vector2.y) * (vector.z + vector2.z);
			lhs.y += (vector.z - vector2.z) * (vector.x + vector2.x);
			lhs.z += (vector.x - vector2.x) * (vector.y + vector2.y);
			vector = vector2;
		}
		lhs.Normalize();
		float num2 = 0f;
		for (int j = 0; j < vertices.Length; j++)
		{
			Vector3 rhs = vertices[j];
			num2 -= Vector3.Dot(in lhs, in rhs);
		}
		num2 /= (float)vertices.Length;
		plane = new Plane(in lhs, num2);
		return plane.normal.sqrMagnitude > 0f;
	}

	[NativeName("TestPlanesAABB")]
	private unsafe static bool Internal_TestPlanesAABB(ReadOnlySpan<Plane> planes, in Bounds bounds)
	{
		ReadOnlySpan<Plane> readOnlySpan = planes;
		bool result;
		fixed (Plane* begin = readOnlySpan)
		{
			ManagedSpanWrapper planes2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			result = Internal_TestPlanesAABB_Injected(ref planes2, in bounds);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TestPlanesAABB(Plane[] planes, Bounds bounds)
	{
		return Internal_TestPlanesAABB(MemoryExtensions.AsSpan(planes), in bounds);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TestPlanesAABB(Plane[] planes, in Bounds bounds)
	{
		return Internal_TestPlanesAABB(MemoryExtensions.AsSpan(planes), in bounds);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TestPlanesAABB(ReadOnlySpan<Plane> planes, in Bounds bounds)
	{
		return Internal_TestPlanesAABB(planes, in bounds);
	}

	[NativeName("ExtractPlanes")]
	private unsafe static void Internal_ExtractPlanes(Span<Plane> planes, in Matrix4x4 worldToProjectionMatrix)
	{
		Span<Plane> span = planes;
		fixed (Plane* begin = span)
		{
			ManagedSpanWrapper planes2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_ExtractPlanes_Injected(ref planes2, in worldToProjectionMatrix);
		}
	}

	[NativeName("CalculateBounds")]
	private unsafe static Bounds Internal_CalculateBounds(Vector3[] positions, in Matrix4x4 transform)
	{
		Span<Vector3> span = new Span<Vector3>(positions);
		Bounds ret;
		fixed (Vector3* begin = span)
		{
			ManagedSpanWrapper positions2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_CalculateBounds_Injected(ref positions2, in transform, out ret);
		}
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_TestPlanesAABB_Injected(ref ManagedSpanWrapper planes, in Bounds bounds);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_ExtractPlanes_Injected(ref ManagedSpanWrapper planes, in Matrix4x4 worldToProjectionMatrix);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CalculateBounds_Injected(ref ManagedSpanWrapper positions, in Matrix4x4 transform, out Bounds ret);
}
