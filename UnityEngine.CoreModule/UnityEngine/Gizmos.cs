using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Gizmos/Gizmos.bindings.h")]
[StaticAccessor("GizmoBindings", StaticAccessorType.DoubleColon)]
public sealed class Gizmos
{
	public static Color color
	{
		get
		{
			get_color_Injected(out var ret);
			return ret;
		}
		set
		{
			set_color_Injected(ref value);
		}
	}

	public static Matrix4x4 matrix
	{
		get
		{
			get_matrix_Injected(out var ret);
			return ret;
		}
		set
		{
			set_matrix_Injected(ref value);
		}
	}

	public static Texture exposure
	{
		get
		{
			return Unmarshal.UnmarshalUnityObject<Texture>(get_exposure_Injected());
		}
		set
		{
			set_exposure_Injected(Object.MarshalledUnityObject.Marshal(value));
		}
	}

	public static extern float probeSize
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[NativeThrows]
	public static void DrawLine(Vector3 from, Vector3 to)
	{
		DrawLine_Injected(ref from, ref to);
	}

	[NativeThrows]
	public unsafe static void DrawLineStrip(ReadOnlySpan<Vector3> points, bool looped)
	{
		ReadOnlySpan<Vector3> readOnlySpan = points;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper points2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			DrawLineStrip_Injected(ref points2, looped);
		}
	}

	[NativeMethod(Name = "DrawLineList", ThrowsException = true)]
	internal unsafe static void DrawLineListInternal(ReadOnlySpan<Vector3> points)
	{
		ReadOnlySpan<Vector3> readOnlySpan = points;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper points2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			DrawLineListInternal_Injected(ref points2);
		}
	}

	public static void DrawLineList(ReadOnlySpan<Vector3> points)
	{
		if ((points.Length & 1) != 0)
		{
			throw new UnityException("You cannot draw a line list from an odd number of points, with two points per line the number of points must be even");
		}
		DrawLineListInternal(points);
	}

	[NativeThrows]
	public static void DrawWireSphere(Vector3 center, float radius)
	{
		DrawWireSphere_Injected(ref center, radius);
	}

	[NativeThrows]
	public static void DrawSphere(Vector3 center, float radius)
	{
		DrawSphere_Injected(ref center, radius);
	}

	[NativeThrows]
	public static void DrawWireCube(Vector3 center, Vector3 size)
	{
		DrawWireCube_Injected(ref center, ref size);
	}

	[NativeThrows]
	public static void DrawCube(Vector3 center, Vector3 size)
	{
		DrawCube_Injected(ref center, ref size);
	}

	[NativeThrows]
	public static void DrawMesh(Mesh mesh, int submeshIndex, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
	{
		DrawMesh_Injected(Object.MarshalledUnityObject.Marshal(mesh), submeshIndex, ref position, ref rotation, ref scale);
	}

	[NativeThrows]
	public static void DrawWireMesh(Mesh mesh, int submeshIndex, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
	{
		DrawWireMesh_Injected(Object.MarshalledUnityObject.Marshal(mesh), submeshIndex, ref position, ref rotation, ref scale);
	}

	[NativeThrows]
	public static void DrawIcon(Vector3 center, string name, [DefaultValue("true")] bool allowScaling)
	{
		DrawIcon(center, name, allowScaling, Color.white);
	}

	[NativeThrows]
	public unsafe static void DrawIcon(Vector3 center, string name, [DefaultValue("true")] bool allowScaling, [DefaultValue("Color(255,255,255,255)")] Color tint)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					DrawIcon_Injected(ref center, ref managedSpanWrapper, allowScaling, ref tint);
					return;
				}
			}
			DrawIcon_Injected(ref center, ref managedSpanWrapper, allowScaling, ref tint);
		}
		finally
		{
		}
	}

	[NativeThrows]
	public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat)
	{
		DrawGUITexture_Injected(ref screenRect, Object.MarshalledUnityObject.Marshal(texture), leftBorder, rightBorder, topBorder, bottomBorder, Object.MarshalledUnityObject.Marshal(mat));
	}

	public static void DrawFrustum(Vector3 center, float fov, float maxRange, float minRange, float aspect)
	{
		DrawFrustum_Injected(ref center, fov, maxRange, minRange, aspect);
	}

	public static float CalculateLOD(Vector3 position, float radius)
	{
		return CalculateLOD_Injected(ref position, radius);
	}

	public static void DrawRay(Ray r)
	{
		DrawLine(r.origin, r.origin + r.direction);
	}

	public static void DrawRay(Vector3 from, Vector3 direction)
	{
		DrawLine(from, from + direction);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation)
	{
		Vector3 one = Vector3.one;
		DrawMesh(mesh, position, rotation, one);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position)
	{
		Vector3 one = Vector3.one;
		Quaternion identity = Quaternion.identity;
		DrawMesh(mesh, position, identity, one);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh)
	{
		Vector3 one = Vector3.one;
		Quaternion identity = Quaternion.identity;
		Vector3 zero = Vector3.zero;
		DrawMesh(mesh, zero, identity, one);
	}

	public static void DrawMesh(Mesh mesh, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
	{
		DrawMesh(mesh, -1, position, rotation, scale);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation)
	{
		Vector3 one = Vector3.one;
		DrawMesh(mesh, submeshIndex, position, rotation, one);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, int submeshIndex, Vector3 position)
	{
		Vector3 one = Vector3.one;
		Quaternion identity = Quaternion.identity;
		DrawMesh(mesh, submeshIndex, position, identity, one);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, int submeshIndex)
	{
		Vector3 one = Vector3.one;
		Quaternion identity = Quaternion.identity;
		Vector3 zero = Vector3.zero;
		DrawMesh(mesh, submeshIndex, zero, identity, one);
	}

	[ExcludeFromDocs]
	public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation)
	{
		Vector3 one = Vector3.one;
		DrawWireMesh(mesh, position, rotation, one);
	}

	[ExcludeFromDocs]
	public static void DrawWireMesh(Mesh mesh, Vector3 position)
	{
		Vector3 one = Vector3.one;
		Quaternion identity = Quaternion.identity;
		DrawWireMesh(mesh, position, identity, one);
	}

	[ExcludeFromDocs]
	public static void DrawWireMesh(Mesh mesh)
	{
		Vector3 one = Vector3.one;
		Quaternion identity = Quaternion.identity;
		Vector3 zero = Vector3.zero;
		DrawWireMesh(mesh, zero, identity, one);
	}

	public static void DrawWireMesh(Mesh mesh, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
	{
		DrawWireMesh(mesh, -1, position, rotation, scale);
	}

	[ExcludeFromDocs]
	public static void DrawWireMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation)
	{
		Vector3 one = Vector3.one;
		DrawWireMesh(mesh, submeshIndex, position, rotation, one);
	}

	[ExcludeFromDocs]
	public static void DrawWireMesh(Mesh mesh, int submeshIndex, Vector3 position)
	{
		Vector3 one = Vector3.one;
		Quaternion identity = Quaternion.identity;
		DrawWireMesh(mesh, submeshIndex, position, identity, one);
	}

	[ExcludeFromDocs]
	public static void DrawWireMesh(Mesh mesh, int submeshIndex)
	{
		Vector3 one = Vector3.one;
		Quaternion identity = Quaternion.identity;
		Vector3 zero = Vector3.zero;
		DrawWireMesh(mesh, submeshIndex, zero, identity, one);
	}

	[ExcludeFromDocs]
	public static void DrawIcon(Vector3 center, string name)
	{
		bool allowScaling = true;
		DrawIcon(center, name, allowScaling);
	}

	[ExcludeFromDocs]
	public static void DrawGUITexture(Rect screenRect, Texture texture)
	{
		Material mat = null;
		DrawGUITexture(screenRect, texture, mat);
	}

	public static void DrawGUITexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat)
	{
		DrawGUITexture(screenRect, texture, 0, 0, 0, 0, mat);
	}

	[ExcludeFromDocs]
	public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
	{
		Material mat = null;
		DrawGUITexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawLine_Injected([In] ref Vector3 from, [In] ref Vector3 to);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawLineStrip_Injected(ref ManagedSpanWrapper points, bool looped);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawLineListInternal_Injected(ref ManagedSpanWrapper points);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawWireSphere_Injected([In] ref Vector3 center, float radius);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawSphere_Injected([In] ref Vector3 center, float radius);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawWireCube_Injected([In] ref Vector3 center, [In] ref Vector3 size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawCube_Injected([In] ref Vector3 center, [In] ref Vector3 size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawMesh_Injected(IntPtr mesh, int submeshIndex, [In][DefaultValue("Vector3.zero")] ref Vector3 position, [In][DefaultValue("Quaternion.identity")] ref Quaternion rotation, [In][DefaultValue("Vector3.one")] ref Vector3 scale);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawWireMesh_Injected(IntPtr mesh, int submeshIndex, [In][DefaultValue("Vector3.zero")] ref Vector3 position, [In][DefaultValue("Quaternion.identity")] ref Quaternion rotation, [In][DefaultValue("Vector3.one")] ref Vector3 scale);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawIcon_Injected([In] ref Vector3 center, ref ManagedSpanWrapper name, [DefaultValue("true")] bool allowScaling, [In][DefaultValue("Color(255,255,255,255)")] ref Color tint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawGUITexture_Injected([In] ref Rect screenRect, IntPtr texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] IntPtr mat);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_color_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_color_Injected([In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_matrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_matrix_Injected([In] ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_exposure_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_exposure_Injected(IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawFrustum_Injected([In] ref Vector3 center, float fov, float maxRange, float minRange, float aspect);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float CalculateLOD_Injected([In] ref Vector3 position, float radius);
}
