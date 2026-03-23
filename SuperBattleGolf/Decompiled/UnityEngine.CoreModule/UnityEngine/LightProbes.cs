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
	public Vector3[] positions
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
		[NativeName("SetBakedCoefficients")]
		[FreeFunction(HasExplicitThis = true)]
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
		[NativeName("GetLightProbeCountSelf")]
		[FreeFunction(HasExplicitThis = true)]
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

	private LightProbes()
	{
	}

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

	[FreeFunction(HasExplicitThis = true)]
	[NativeName("SetLightProbePositionsSelf")]
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

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetLightProbeCount")]
	[FreeFunction]
	internal static extern int GetCount();

	[Obsolete("Use GetInterpolatedProbe instead.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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
	private static extern void GetPositionsSelf_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPositionsSelf_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positions, bool checkForDuplicatePositions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bakedProbes_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bakedProbes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_count_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_countSelf_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_cellCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_cellCountSelf_Injected(IntPtr _unity_self);
}
