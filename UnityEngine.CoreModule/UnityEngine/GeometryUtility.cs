using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[StaticAccessor("GeometryUtilityScripting", StaticAccessorType.DoubleColon)]
public sealed class GeometryUtility
{
	public static Plane[] CalculateFrustumPlanes(Camera camera)
	{
		Plane[] array = new Plane[6];
		CalculateFrustumPlanes(camera, array);
		return array;
	}

	public static Plane[] CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix)
	{
		Plane[] array = new Plane[6];
		CalculateFrustumPlanes(worldToProjectionMatrix, array);
		return array;
	}

	public static void CalculateFrustumPlanes(Camera camera, Plane[] planes)
	{
		CalculateFrustumPlanes(camera.projectionMatrix * camera.worldToCameraMatrix, planes);
	}

	public static void CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix, Plane[] planes)
	{
		if (planes == null)
		{
			throw new ArgumentNullException("planes");
		}
		if (planes.Length != 6)
		{
			throw new ArgumentException("Planes array must be of length 6.", "planes");
		}
		Internal_ExtractPlanes(planes, worldToProjectionMatrix);
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
		return Internal_CalculateBounds(positions, transform);
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
			plane = new Plane(a, b, c);
			return plane.normal.sqrMagnitude > 0f;
		}
		Vector3 zero = Vector3.zero;
		int num = vertices.Length - 1;
		Vector3 vector = vertices[num];
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector2 = vertices[i];
			zero.x += (vector.y - vector2.y) * (vector.z + vector2.z);
			zero.y += (vector.z - vector2.z) * (vector.x + vector2.x);
			zero.z += (vector.x - vector2.x) * (vector.y + vector2.y);
			vector = vector2;
		}
		zero.Normalize();
		float num2 = 0f;
		foreach (Vector3 rhs in vertices)
		{
			num2 -= Vector3.Dot(zero, rhs);
		}
		num2 /= (float)vertices.Length;
		plane = new Plane(zero, num2);
		return plane.normal.sqrMagnitude > 0f;
	}

	public unsafe static bool TestPlanesAABB(Plane[] planes, Bounds bounds)
	{
		Span<Plane> span = new Span<Plane>(planes);
		bool result;
		fixed (Plane* begin = span)
		{
			ManagedSpanWrapper planes2 = new ManagedSpanWrapper(begin, span.Length);
			result = TestPlanesAABB_Injected(ref planes2, ref bounds);
		}
		return result;
	}

	[NativeName("ExtractPlanes")]
	private unsafe static void Internal_ExtractPlanes([Out] Plane[] planes, Matrix4x4 worldToProjectionMatrix)
	{
		//The blocks IL_001b are reachable both inside and outside the pinned region starting at IL_0004. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper planes2 = default(BlittableArrayWrapper);
		try
		{
			if (planes != null)
			{
				fixed (Plane[] array = planes)
				{
					if (array.Length != 0)
					{
						planes2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					Internal_ExtractPlanes_Injected(out planes2, ref worldToProjectionMatrix);
					return;
				}
			}
			Internal_ExtractPlanes_Injected(out planes2, ref worldToProjectionMatrix);
		}
		finally
		{
			planes2.Unmarshal(ref array);
		}
	}

	[NativeName("CalculateBounds")]
	private unsafe static Bounds Internal_CalculateBounds(Vector3[] positions, Matrix4x4 transform)
	{
		Span<Vector3> span = new Span<Vector3>(positions);
		Bounds ret;
		fixed (Vector3* begin = span)
		{
			ManagedSpanWrapper positions2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_CalculateBounds_Injected(ref positions2, ref transform, out ret);
		}
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool TestPlanesAABB_Injected(ref ManagedSpanWrapper planes, [In] ref Bounds bounds);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_ExtractPlanes_Injected(out BlittableArrayWrapper planes, [In] ref Matrix4x4 worldToProjectionMatrix);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CalculateBounds_Injected(ref ManagedSpanWrapper positions, [In] ref Matrix4x4 transform, out Bounds ret);
}
