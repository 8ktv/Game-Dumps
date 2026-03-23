using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/TrailRenderer.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
public sealed class TrailRenderer : Renderer
{
	[Obsolete("Use positionCount instead (UnityUpgradable) -> positionCount", false)]
	public int numPositions => positionCount;

	public float time
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_time_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_time_Injected(intPtr, value);
		}
	}

	public float startWidth
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_startWidth_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_startWidth_Injected(intPtr, value);
		}
	}

	public float endWidth
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_endWidth_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_endWidth_Injected(intPtr, value);
		}
	}

	public float widthMultiplier
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_widthMultiplier_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_widthMultiplier_Injected(intPtr, value);
		}
	}

	public bool autodestruct
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_autodestruct_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_autodestruct_Injected(intPtr, value);
		}
	}

	public bool emitting
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_emitting_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_emitting_Injected(intPtr, value);
		}
	}

	public int numCornerVertices
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_numCornerVertices_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_numCornerVertices_Injected(intPtr, value);
		}
	}

	public int numCapVertices
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_numCapVertices_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_numCapVertices_Injected(intPtr, value);
		}
	}

	public float minVertexDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_minVertexDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_minVertexDistance_Injected(intPtr, value);
		}
	}

	public Color startColor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_startColor_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_startColor_Injected(intPtr, ref value);
		}
	}

	public Color endColor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_endColor_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_endColor_Injected(intPtr, ref value);
		}
	}

	[NativeProperty("PositionsCount")]
	public int positionCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_positionCount_Injected(intPtr);
		}
	}

	public Vector2 textureScale
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_textureScale_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_textureScale_Injected(intPtr, ref value);
		}
	}

	public float shadowBias
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadowBias_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowBias_Injected(intPtr, value);
		}
	}

	public bool generateLightingData
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_generateLightingData_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_generateLightingData_Injected(intPtr, value);
		}
	}

	public bool applyActiveColorSpace
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_applyActiveColorSpace_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_applyActiveColorSpace_Injected(intPtr, value);
		}
	}

	public LineTextureMode textureMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_textureMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_textureMode_Injected(intPtr, value);
		}
	}

	public LineAlignment alignment
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_alignment_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_alignment_Injected(intPtr, value);
		}
	}

	public SpriteMaskInteraction maskInteraction
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maskInteraction_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_maskInteraction_Injected(intPtr, value);
		}
	}

	public AnimationCurve widthCurve
	{
		get
		{
			return GetWidthCurveCopy();
		}
		set
		{
			SetWidthCurve(value);
		}
	}

	public Gradient colorGradient
	{
		get
		{
			return GetColorGradientCopy();
		}
		set
		{
			SetColorGradient(value);
		}
	}

	public void SetPosition(int index, Vector3 position)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetPosition_Injected(intPtr, index, ref position);
	}

	public Vector3 GetPosition(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPosition_Injected(intPtr, index, out var ret);
		return ret;
	}

	public void Clear()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Clear_Injected(intPtr);
	}

	public void BakeMesh(Mesh mesh, bool useTransform = false)
	{
		BakeMesh(mesh, Camera.main, useTransform);
	}

	public void BakeMesh([NotNull] Mesh mesh, [NotNull] Camera camera, bool useTransform = false)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr3 = MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr3 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		BakeMesh_Injected(intPtr, intPtr2, intPtr3, useTransform);
	}

	private AnimationCurve GetWidthCurveCopy()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr widthCurveCopy_Injected = GetWidthCurveCopy_Injected(intPtr);
		return (widthCurveCopy_Injected == (IntPtr)0) ? null : AnimationCurve.BindingsMarshaller.ConvertToManaged(widthCurveCopy_Injected);
	}

	private void SetWidthCurve([NotNull] AnimationCurve curve)
	{
		if (curve == null)
		{
			ThrowHelper.ThrowArgumentNullException(curve, "curve");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = AnimationCurve.BindingsMarshaller.ConvertToNative(curve);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(curve, "curve");
		}
		SetWidthCurve_Injected(intPtr, intPtr2);
	}

	private Gradient GetColorGradientCopy()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr colorGradientCopy_Injected = GetColorGradientCopy_Injected(intPtr);
		return (colorGradientCopy_Injected == (IntPtr)0) ? null : Gradient.BindingsMarshaller.ConvertToManaged(colorGradientCopy_Injected);
	}

	private void SetColorGradient([NotNull] Gradient curve)
	{
		if (curve == null)
		{
			ThrowHelper.ThrowArgumentNullException(curve, "curve");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Gradient.BindingsMarshaller.ConvertToNative(curve);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(curve, "curve");
		}
		SetColorGradient_Injected(intPtr, intPtr2);
	}

	[FreeFunction(Name = "TrailRendererScripting::GetPositions", HasExplicitThis = true)]
	public unsafe int GetPositions([Out][NotNull] Vector3[] positions)
	{
		if (positions == null)
		{
			ThrowHelper.ThrowArgumentNullException(positions, "positions");
		}
		BlittableArrayWrapper positions2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			fixed (Vector3[] array = positions)
			{
				if (array.Length != 0)
				{
					positions2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				return GetPositions_Injected(intPtr, out positions2);
			}
		}
		finally
		{
			positions2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "TrailRendererScripting::GetVisiblePositions", HasExplicitThis = true)]
	public unsafe int GetVisiblePositions([Out][NotNull] Vector3[] positions)
	{
		if (positions == null)
		{
			ThrowHelper.ThrowArgumentNullException(positions, "positions");
		}
		BlittableArrayWrapper positions2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			fixed (Vector3[] array = positions)
			{
				if (array.Length != 0)
				{
					positions2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				return GetVisiblePositions_Injected(intPtr, out positions2);
			}
		}
		finally
		{
			positions2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "TrailRendererScripting::SetPositions", HasExplicitThis = true)]
	public unsafe void SetPositions([NotNull] Vector3[] positions)
	{
		if (positions == null)
		{
			ThrowHelper.ThrowArgumentNullException(positions, "positions");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector3> span = new Span<Vector3>(positions);
		fixed (Vector3* begin = span)
		{
			ManagedSpanWrapper positions2 = new ManagedSpanWrapper(begin, span.Length);
			SetPositions_Injected(intPtr, ref positions2);
		}
	}

	[FreeFunction(Name = "TrailRendererScripting::AddPosition", HasExplicitThis = true)]
	public void AddPosition(Vector3 position)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddPosition_Injected(intPtr, ref position);
	}

	[FreeFunction(Name = "TrailRendererScripting::AddPositions", HasExplicitThis = true)]
	public unsafe void AddPositions([NotNull] Vector3[] positions)
	{
		if (positions == null)
		{
			ThrowHelper.ThrowArgumentNullException(positions, "positions");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector3> span = new Span<Vector3>(positions);
		fixed (Vector3* begin = span)
		{
			ManagedSpanWrapper positions2 = new ManagedSpanWrapper(begin, span.Length);
			AddPositions_Injected(intPtr, ref positions2);
		}
	}

	public unsafe void SetPositions(NativeArray<Vector3> positions)
	{
		SetPositionsWithNativeContainer((IntPtr)positions.GetUnsafeReadOnlyPtr(), positions.Length);
	}

	public unsafe void SetPositions(NativeSlice<Vector3> positions)
	{
		SetPositionsWithNativeContainer((IntPtr)positions.GetUnsafeReadOnlyPtr(), positions.Length);
	}

	public unsafe int GetPositions([Out] NativeArray<Vector3> positions)
	{
		return GetPositionsWithNativeContainer((IntPtr)positions.GetUnsafePtr(), positions.Length);
	}

	public unsafe int GetPositions([Out] NativeSlice<Vector3> positions)
	{
		return GetPositionsWithNativeContainer((IntPtr)positions.GetUnsafePtr(), positions.Length);
	}

	public unsafe int GetVisiblePositions([Out] NativeArray<Vector3> positions)
	{
		return GetVisiblePositionsWithNativeContainer((IntPtr)positions.GetUnsafePtr(), positions.Length);
	}

	public unsafe int GetVisiblePositions([Out] NativeSlice<Vector3> positions)
	{
		return GetVisiblePositionsWithNativeContainer((IntPtr)positions.GetUnsafePtr(), positions.Length);
	}

	public unsafe void AddPositions([Out] NativeArray<Vector3> positions)
	{
		AddPositionsWithNativeContainer((IntPtr)positions.GetUnsafePtr(), positions.Length);
	}

	public unsafe void AddPositions([Out] NativeSlice<Vector3> positions)
	{
		AddPositionsWithNativeContainer((IntPtr)positions.GetUnsafePtr(), positions.Length);
	}

	[FreeFunction(Name = "TrailRendererScripting::SetPositionsWithNativeContainer", HasExplicitThis = true)]
	private void SetPositionsWithNativeContainer(IntPtr positions, int count)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetPositionsWithNativeContainer_Injected(intPtr, positions, count);
	}

	[FreeFunction(Name = "TrailRendererScripting::GetPositionsWithNativeContainer", HasExplicitThis = true)]
	private int GetPositionsWithNativeContainer(IntPtr positions, int length)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPositionsWithNativeContainer_Injected(intPtr, positions, length);
	}

	[FreeFunction(Name = "TrailRendererScripting::GetVisiblePositionsWithNativeContainer", HasExplicitThis = true)]
	private int GetVisiblePositionsWithNativeContainer(IntPtr positions, int length)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetVisiblePositionsWithNativeContainer_Injected(intPtr, positions, length);
	}

	[FreeFunction(Name = "TrailRendererScripting::AddPositionsWithNativeContainer", HasExplicitThis = true)]
	private void AddPositionsWithNativeContainer(IntPtr positions, int length)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddPositionsWithNativeContainer_Injected(intPtr, positions, length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_time_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_time_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_startWidth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_startWidth_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_endWidth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_endWidth_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_widthMultiplier_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_widthMultiplier_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_autodestruct_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_autodestruct_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_emitting_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_emitting_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_numCornerVertices_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_numCornerVertices_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_numCapVertices_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_numCapVertices_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_minVertexDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_minVertexDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_startColor_Injected(IntPtr _unity_self, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_startColor_Injected(IntPtr _unity_self, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_endColor_Injected(IntPtr _unity_self, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_endColor_Injected(IntPtr _unity_self, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_positionCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPosition_Injected(IntPtr _unity_self, int index, [In] ref Vector3 position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPosition_Injected(IntPtr _unity_self, int index, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_textureScale_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_textureScale_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_shadowBias_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowBias_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_generateLightingData_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_generateLightingData_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_applyActiveColorSpace_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_applyActiveColorSpace_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LineTextureMode get_textureMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_textureMode_Injected(IntPtr _unity_self, LineTextureMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LineAlignment get_alignment_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_alignment_Injected(IntPtr _unity_self, LineAlignment value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern SpriteMaskInteraction get_maskInteraction_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maskInteraction_Injected(IntPtr _unity_self, SpriteMaskInteraction value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Clear_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BakeMesh_Injected(IntPtr _unity_self, IntPtr mesh, IntPtr camera, bool useTransform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetWidthCurveCopy_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetWidthCurve_Injected(IntPtr _unity_self, IntPtr curve);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetColorGradientCopy_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetColorGradient_Injected(IntPtr _unity_self, IntPtr curve);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPositions_Injected(IntPtr _unity_self, out BlittableArrayWrapper positions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVisiblePositions_Injected(IntPtr _unity_self, out BlittableArrayWrapper positions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPositions_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddPosition_Injected(IntPtr _unity_self, [In] ref Vector3 position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddPositions_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPositionsWithNativeContainer_Injected(IntPtr _unity_self, IntPtr positions, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPositionsWithNativeContainer_Injected(IntPtr _unity_self, IntPtr positions, int length);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVisiblePositionsWithNativeContainer_Injected(IntPtr _unity_self, IntPtr positions, int length);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddPositionsWithNativeContainer_Injected(IntPtr _unity_self, IntPtr positions, int length);
}
