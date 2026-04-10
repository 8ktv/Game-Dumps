using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Runtime/Export/Graphics/Graphics.bindings.h")]
public sealed class LightProbes : Object
{
	internal struct Hash128IntPair
	{
		internal Hash128 Hash;

		internal int Value;
	}

	public unsafe Vector3[] positions
	{
		[NativeName("GetLightProbePositions")]
		[FreeFunction(HasExplicitThis = true)]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			Vector3[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_positions_Injected(intPtr, out ret);
			}
			finally
			{
				Vector3[] array = default(Vector3[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[FreeFunction(HasExplicitThis = true)]
		[NativeName("SetLightProbePositions")]
		internal set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector3> span = new Span<Vector3>(value);
			fixed (Vector3* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_positions_Injected(intPtr, ref value2);
			}
		}
	}

	public unsafe SphericalHarmonicsL2[] bakedProbes
	{
		[NativeName("GetBakedCoefficients")]
		[FreeFunction(HasExplicitThis = true)]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			SphericalHarmonicsL2[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_bakedProbes_Injected(intPtr, out ret);
			}
			finally
			{
				SphericalHarmonicsL2[] array = default(SphericalHarmonicsL2[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[FreeFunction(HasExplicitThis = true)]
		[NativeName("SetBakedCoefficients")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<SphericalHarmonicsL2> span = new Span<SphericalHarmonicsL2>(value);
			fixed (SphericalHarmonicsL2* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_bakedProbes_Injected(intPtr, ref value2);
			}
		}
	}

	public int count
	{
		[FreeFunction(HasExplicitThis = true)]
		[NativeName("GetLightProbeCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_count_Injected(intPtr);
		}
	}

	public int countSelf
	{
		[FreeFunction(HasExplicitThis = true)]
		[NativeName("GetLightProbeCountSelf")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_countSelf_Injected(intPtr);
		}
	}

	public int cellCount
	{
		[NativeName("GetTetrahedraSize")]
		[FreeFunction(HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cellCount_Injected(intPtr);
		}
	}

	public int cellCountSelf
	{
		[NativeName("GetTetrahedraSizeSelf")]
		[FreeFunction(HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cellCountSelf_Injected(intPtr);
		}
	}

	internal unsafe Hash128IntPair[] nonTetrahedralizedProbeSetIndexMap
	{
		[FreeFunction(HasExplicitThis = true)]
		[NativeName("GetNonTetrahedralizedProbeSetIndexMap")]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			Hash128IntPair[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_nonTetrahedralizedProbeSetIndexMap_Injected(intPtr, out ret);
			}
			finally
			{
				Hash128IntPair[] array = default(Hash128IntPair[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[FreeFunction(HasExplicitThis = true)]
		[NativeName("SetNonTetrahedralizedProbeSetIndexMap")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Hash128IntPair> span = new Span<Hash128IntPair>(value);
			fixed (Hash128IntPair* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_nonTetrahedralizedProbeSetIndexMap_Injected(intPtr, ref value2);
			}
		}
	}

	internal unsafe LightProbeOcclusion[] bakedLightOcclusion
	{
		[NativeName("GetBakedLightOcclusion")]
		[FreeFunction(HasExplicitThis = true)]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			LightProbeOcclusion[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_bakedLightOcclusion_Injected(intPtr, out ret);
			}
			finally
			{
				LightProbeOcclusion[] array = default(LightProbeOcclusion[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[NativeName("SetBakedLightOcclusion")]
		[FreeFunction(HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<LightProbeOcclusion> span = new Span<LightProbeOcclusion>(value);
			fixed (LightProbeOcclusion* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_bakedLightOcclusion_Injected(intPtr, ref value2);
			}
		}
	}

	internal Bounds boundingBox
	{
		[FreeFunction(HasExplicitThis = true)]
		[NativeName("GetBoundingBox")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_boundingBox_Injected(intPtr, out var ret);
			return ret;
		}
		[FreeFunction(HasExplicitThis = true)]
		[NativeName("SetBoundingBox")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_boundingBox_Injected(intPtr, ref value);
		}
	}

	internal unsafe ProbeSetIndex[] probeSets
	{
		[NativeName("GetProbeSets")]
		[FreeFunction(HasExplicitThis = true)]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			ProbeSetIndex[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_probeSets_Injected(intPtr, out ret);
			}
			finally
			{
				ProbeSetIndex[] array = default(ProbeSetIndex[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[NativeName("SetProbeSets")]
		[FreeFunction(HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<ProbeSetIndex> span = new Span<ProbeSetIndex>(value);
			fixed (ProbeSetIndex* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_probeSets_Injected(intPtr, ref value2);
			}
		}
	}

	internal unsafe Tetrahedron[] tetrahedra
	{
		[NativeName("GetTetrahedra")]
		[FreeFunction(HasExplicitThis = true)]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			Tetrahedron[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_tetrahedra_Injected(intPtr, out ret);
			}
			finally
			{
				Tetrahedron[] array = default(Tetrahedron[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[FreeFunction(HasExplicitThis = true)]
		[NativeName("SetTetrahedra")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Tetrahedron> span = new Span<Tetrahedron>(value);
			fixed (Tetrahedron* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_tetrahedra_Injected(intPtr, ref value2);
			}
		}
	}

	internal unsafe Vector3[] hullRays
	{
		[NativeName("GetHullRays")]
		[FreeFunction(HasExplicitThis = true)]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			Vector3[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_hullRays_Injected(intPtr, out ret);
			}
			finally
			{
				Vector3[] array = default(Vector3[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[FreeFunction(HasExplicitThis = true)]
		[NativeName("SetHullRays")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector3> span = new Span<Vector3>(value);
			fixed (Vector3* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_hullRays_Injected(intPtr, ref value2);
			}
		}
	}

	[Obsolete("Use bakedProbes instead.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public float[] coefficients
	{
		get
		{
			return new float[0];
		}
		set
		{
		}
	}

	public static event Action lightProbesUpdated;

	public static event Action tetrahedralizationCompleted;

	public static event Action needsRetetrahedralization;

	internal LightProbes()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Create([Writable] LightProbes self);

	[RequiredByNativeCode]
	private static void Internal_CallLightProbesUpdatedFunction()
	{
		if (LightProbes.lightProbesUpdated != null)
		{
			LightProbes.lightProbesUpdated();
		}
	}

	[RequiredByNativeCode]
	private static void Internal_CallTetrahedralizationCompletedFunction()
	{
		if (LightProbes.tetrahedralizationCompleted != null)
		{
			LightProbes.tetrahedralizationCompleted();
		}
	}

	[RequiredByNativeCode]
	private static void Internal_CallNeedsRetetrahedralizationFunction()
	{
		if (LightProbes.needsRetetrahedralization != null)
		{
			LightProbes.needsRetetrahedralization();
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void Tetrahedralize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void TetrahedralizeAsync();

	[FreeFunction]
	public static void GetInterpolatedProbe(Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe)
	{
		GetInterpolatedProbe_Injected(ref position, MarshalledUnityObject.Marshal(renderer), out probe);
	}

	[FreeFunction]
	internal static bool AreLightProbesAllowed(Renderer renderer)
	{
		return AreLightProbesAllowed_Injected(MarshalledUnityObject.Marshal(renderer));
	}

	public static void CalculateInterpolatedLightAndOcclusionProbes(Vector3[] positions, SphericalHarmonicsL2[] lightProbes, Vector4[] occlusionProbes)
	{
		if (positions == null)
		{
			throw new ArgumentNullException("positions");
		}
		if (lightProbes == null && occlusionProbes == null)
		{
			throw new ArgumentException("Argument lightProbes and occlusionProbes cannot both be null.");
		}
		if (lightProbes != null && lightProbes.Length < positions.Length)
		{
			throw new ArgumentException("lightProbes", "Argument lightProbes has less elements than positions");
		}
		if (occlusionProbes != null && occlusionProbes.Length < positions.Length)
		{
			throw new ArgumentException("occlusionProbes", "Argument occlusionProbes has less elements than positions");
		}
		CalculateInterpolatedLightAndOcclusionProbes_Internal(positions, positions.Length, lightProbes, occlusionProbes);
	}

	public static void CalculateInterpolatedLightAndOcclusionProbes(List<Vector3> positions, List<SphericalHarmonicsL2> lightProbes, List<Vector4> occlusionProbes)
	{
		if (positions == null)
		{
			throw new ArgumentNullException("positions");
		}
		if (lightProbes == null && occlusionProbes == null)
		{
			throw new ArgumentException("Argument lightProbes and occlusionProbes cannot both be null.");
		}
		if (lightProbes != null)
		{
			NoAllocHelpers.EnsureListElemCount(lightProbes, positions.Count);
		}
		if (occlusionProbes != null)
		{
			NoAllocHelpers.EnsureListElemCount(occlusionProbes, positions.Count);
		}
		CalculateInterpolatedLightAndOcclusionProbes_Internal(NoAllocHelpers.ExtractArrayFromList(positions), positions.Count, NoAllocHelpers.ExtractArrayFromList(lightProbes), NoAllocHelpers.ExtractArrayFromList(occlusionProbes));
	}

	[FreeFunction]
	[NativeName("CalculateInterpolatedLightAndOcclusionProbes")]
	internal unsafe static void CalculateInterpolatedLightAndOcclusionProbes_Internal(Vector3[] positions, int positionsCount, SphericalHarmonicsL2[] lightProbes, Vector4[] occlusionProbes)
	{
		Span<Vector3> span = new Span<Vector3>(positions);
		fixed (Vector3* begin = span)
		{
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, span.Length);
			Span<SphericalHarmonicsL2> span2 = new Span<SphericalHarmonicsL2>(lightProbes);
			fixed (SphericalHarmonicsL2* begin2 = span2)
			{
				ManagedSpanWrapper lightProbes2 = new ManagedSpanWrapper(begin2, span2.Length);
				Span<Vector4> span3 = new Span<Vector4>(occlusionProbes);
				fixed (Vector4* begin3 = span3)
				{
					ManagedSpanWrapper occlusionProbes2 = new ManagedSpanWrapper(begin3, span3.Length);
					CalculateInterpolatedLightAndOcclusionProbes_Internal_Injected(ref managedSpanWrapper, positionsCount, ref lightProbes2, ref occlusionProbes2);
				}
			}
		}
	}

	[NativeName("GetSharedLightProbesForScene")]
	[FreeFunction]
	public static LightProbes GetSharedLightProbesForScene(Scene scene)
	{
		return Unmarshal.UnmarshalUnityObject<LightProbes>(GetSharedLightProbesForScene_Injected(ref scene));
	}

	[NativeName("GetInstantiatedLightProbesForScene")]
	[FreeFunction]
	public static LightProbes GetInstantiatedLightProbesForScene(Scene scene)
	{
		return Unmarshal.UnmarshalUnityObject<LightProbes>(GetInstantiatedLightProbesForScene_Injected(ref scene));
	}

	[NativeName("GetLightProbePositionsSelf")]
	[FreeFunction(HasExplicitThis = true)]
	public Vector3[] GetPositionsSelf()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Vector3[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetPositionsSelf_Injected(intPtr, out ret);
		}
		finally
		{
			Vector3[] array = default(Vector3[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[NativeName("SetLightProbePositionsSelf")]
	[FreeFunction(HasExplicitThis = true)]
	public unsafe bool SetPositionsSelf(Vector3[] positions, bool checkForDuplicatePositions)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector3> span = new Span<Vector3>(positions);
		bool result;
		fixed (Vector3* begin = span)
		{
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, span.Length);
			result = SetPositionsSelf_Injected(intPtr, ref managedSpanWrapper, checkForDuplicatePositions);
		}
		return result;
	}

	[FreeFunction(HasExplicitThis = true)]
	internal unsafe void SetBakedCoefficients_Internal(SphericalHarmonicsL2[] coefficients)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<SphericalHarmonicsL2> span = new Span<SphericalHarmonicsL2>(coefficients);
		fixed (SphericalHarmonicsL2* begin = span)
		{
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, span.Length);
			SetBakedCoefficients_Internal_Injected(intPtr, ref managedSpanWrapper);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetLightProbeCount")]
	[FreeFunction]
	internal static extern int GetCount();

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use GetInterpolatedProbe instead.", true)]
	public void GetInterpolatedLightProbe(Vector3 position, Renderer renderer, float[] coefficients)
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetInterpolatedProbe_Injected([In] ref Vector3 position, IntPtr renderer, out SphericalHarmonicsL2 probe);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool AreLightProbesAllowed_Injected(IntPtr renderer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CalculateInterpolatedLightAndOcclusionProbes_Internal_Injected(ref ManagedSpanWrapper positions, int positionsCount, ref ManagedSpanWrapper lightProbes, ref ManagedSpanWrapper occlusionProbes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetSharedLightProbesForScene_Injected([In] ref Scene scene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetInstantiatedLightProbesForScene_Injected([In] ref Scene scene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_positions_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_positions_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPositionsSelf_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPositionsSelf_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positions, bool checkForDuplicatePositions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bakedProbes_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bakedProbes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBakedCoefficients_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper coefficients);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_count_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_countSelf_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_cellCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_cellCountSelf_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_nonTetrahedralizedProbeSetIndexMap_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_nonTetrahedralizedProbeSetIndexMap_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bakedLightOcclusion_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bakedLightOcclusion_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_boundingBox_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_boundingBox_Injected(IntPtr _unity_self, [In] ref Bounds value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_probeSets_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_probeSets_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_tetrahedra_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_tetrahedra_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_hullRays_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_hullRays_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);
}
