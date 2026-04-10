using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering;

[NativeHeader("Runtime/Graphics/GraphicsStateCollection.h")]
public sealed class GraphicsStateCollection : Object
{
	public struct GraphicsState
	{
		public VertexAttributeDescriptor[] vertexAttributes;

		public AttachmentDescriptor[] attachments;

		public SubPassDescriptor[] subPasses;

		public RenderStateBlock renderState;

		public MeshTopology topology;

		public CullMode forceCullMode;

		public ShadingRateCombiner shadingRateCombinerPrimitive;

		public ShadingRateCombiner shadingRateCombinerFragment;

		public ShadingRateFragmentSize baseShadingRate;

		public float depthBias;

		public float slopeDepthBias;

		public int depthAttachmentIndex;

		public int subPassIndex;

		public int shadingRateIndex;

		public int multiviewCount;

		public int sampleCount;

		public bool hasEyeTexture;

		public bool wireframe;

		public bool invertCulling;

		public bool negativeScale;

		public bool invertProjection;

		public void SetMeshData(Mesh mesh, int submesh, [DefaultValue("null")] Renderer renderer = null)
		{
			SetMeshData_Injected(ref this, MarshalledUnityObject.Marshal(mesh), submesh, MarshalledUnityObject.Marshal(renderer));
		}

		[NativeName("SetRenderPassData")]
		private unsafe void SetRenderPassData_Internal(int samples, ReadOnlySpan<AttachmentDescriptor> attachments, ReadOnlySpan<SubPassDescriptor> subPasses, int subPassIndex, int depthAttachmentIndex, int shadingRateIndex)
		{
			ReadOnlySpan<AttachmentDescriptor> readOnlySpan = attachments;
			fixed (AttachmentDescriptor* begin = readOnlySpan)
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
				ReadOnlySpan<SubPassDescriptor> readOnlySpan2 = subPasses;
				fixed (SubPassDescriptor* begin2 = readOnlySpan2)
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					SetRenderPassData_Internal_Injected(ref this, samples, ref managedSpanWrapper, ref managedSpanWrapper2, subPassIndex, depthAttachmentIndex, shadingRateIndex);
				}
			}
		}

		public void SetRenderPassData(int samples, NativeArray<AttachmentDescriptor> attachments, NativeArray<SubPassDescriptor> subPasses, [DefaultValue("0")] int subPassIndex = 0, [DefaultValue("-1")] int depthAttachmentIndex = -1, [DefaultValue("-1")] int shadingRateIndex = -1)
		{
			SetRenderPassData_Internal(samples, attachments, subPasses, subPassIndex, depthAttachmentIndex, shadingRateIndex);
		}

		public void SetRenderStateData(Shader shader, PassIdentifier passId)
		{
			SetRenderStateData_Injected(ref this, MarshalledUnityObject.Marshal(shader), ref passId);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMeshData_Injected(ref GraphicsState _unity_self, IntPtr mesh, int submesh, [DefaultValue("null")] IntPtr renderer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderPassData_Internal_Injected(ref GraphicsState _unity_self, int samples, ref ManagedSpanWrapper attachments, ref ManagedSpanWrapper subPasses, int subPassIndex, int depthAttachmentIndex, int shadingRateIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderStateData_Injected(ref GraphicsState _unity_self, IntPtr shader, [In] ref PassIdentifier passId);
	}

	public struct ShaderVariant
	{
		public Shader shader;

		public PassIdentifier passId;

		public LocalKeyword[] keywords;

		public ShaderVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			this.shader = shader;
			this.passId = passId;
			this.keywords = keywords;
		}

		public ShaderVariant(Material material, PassIdentifier passId)
		{
			shader = material.shader;
			this.passId = passId;
			keywords = material.enabledKeywords;
		}
	}

	public bool isTracing
	{
		[NativeName("IsTracing")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isTracing_Injected(intPtr);
		}
	}

	public int version
	{
		[NativeName("GetVersion")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_version_Injected(intPtr);
		}
		[NativeName("SetVersion")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_version_Injected(intPtr, value);
		}
	}

	public GraphicsDeviceType graphicsDeviceType
	{
		[NativeName("GetDeviceRenderer")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_graphicsDeviceType_Injected(intPtr);
		}
		[NativeName("SetDeviceRenderer")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_graphicsDeviceType_Injected(intPtr, value);
		}
	}

	public RuntimePlatform runtimePlatform
	{
		[NativeName("GetRuntimePlatform")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_runtimePlatform_Injected(intPtr);
		}
		[NativeName("SetRuntimePlatform")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_runtimePlatform_Injected(intPtr, value);
		}
	}

	public unsafe string qualityLevelName
	{
		[NativeName("GetQualityLevelName")]
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_qualityLevelName_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
		[NativeName("SetQualityLevelName")]
		set
		{
			//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(value);
					fixed (char* begin = readOnlySpan)
					{
						managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
						set_qualityLevelName_Injected(intPtr, ref managedSpanWrapper);
						return;
					}
				}
				set_qualityLevelName_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
			}
		}
	}

	public int totalGraphicsStateCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_totalGraphicsStateCount_Injected(intPtr);
		}
	}

	public int completedWarmupCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_completedWarmupCount_Injected(intPtr);
		}
	}

	public bool isWarmedUp
	{
		[NativeName("IsWarmedUp")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isWarmedUp_Injected(intPtr);
		}
	}

	public int variantCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_variantCount_Injected(intPtr);
		}
	}

	public bool BeginTrace()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return BeginTrace_Injected(intPtr);
	}

	public void EndTrace()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		EndTrace_Injected(intPtr);
	}

	public unsafe bool LoadFromFile(string filePath)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(filePath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return LoadFromFile_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return LoadFromFile_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public unsafe bool SaveToFile(string filePath)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(filePath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return SaveToFile_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return SaveToFile_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public unsafe bool SendToEditor(string fileName)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(fileName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(fileName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return SendToEditor_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return SendToEditor_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeName("Warmup")]
	public JobHandle WarmUp(JobHandle dependency = default(JobHandle))
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		WarmUp_Injected(intPtr, ref dependency, out var ret);
		return ret;
	}

	[NativeName("WarmupProgressively")]
	public JobHandle WarmUpProgressively(int count, JobHandle dependency = default(JobHandle))
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		WarmUpProgressively_Injected(intPtr, count, ref dependency, out var ret);
		return ret;
	}

	private void GetVariants([Out] ShaderVariant[] results)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetVariants_Injected(intPtr, results);
	}

	public void GetVariants(List<ShaderVariant> results)
	{
		if (results == null)
		{
			throw new ArgumentNullException("The result shader variant list cannot be null.");
		}
		results.Clear();
		NoAllocHelpers.EnsureListElemCount(results, variantCount);
		GetVariants(NoAllocHelpers.ExtractArrayFromList(results));
	}

	private void GetGraphicsStatesForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords, [Out] GraphicsState[] results)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetGraphicsStatesForVariant_Injected(intPtr, MarshalledUnityObject.Marshal(shader), ref passId, keywords, results);
	}

	public void GetGraphicsStatesForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords, List<GraphicsState> results)
	{
		if (results == null)
		{
			throw new ArgumentNullException("The result graphics state list cannot be null.");
		}
		results.Clear();
		NoAllocHelpers.EnsureListElemCount(results, GetGraphicsStateCountForVariant(shader, passId, keywords));
		GetGraphicsStatesForVariant(shader, passId, keywords, NoAllocHelpers.ExtractArrayFromList(results));
	}

	public int GetGraphicsStateCountForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetGraphicsStateCountForVariant_Injected(intPtr, MarshalledUnityObject.Marshal(shader), ref passId, keywords);
	}

	public bool AddVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
	{
		return AddVariantByShader(shader, passId, keywords);
	}

	public bool AddVariant(Material mat, PassIdentifier passId)
	{
		return AddVariantByMaterial(mat, passId);
	}

	[NativeName("AddVariant")]
	private bool AddVariantByShader(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddVariantByShader_Injected(intPtr, MarshalledUnityObject.Marshal(shader), ref passId, keywords);
	}

	[NativeName("AddVariant")]
	private bool AddVariantByMaterial(Material mat, PassIdentifier passId)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddVariantByMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(mat), ref passId);
	}

	public bool AddVariants(Material mat, [DefaultValue("-1")] int subshaderIndex = -1)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddVariants_Injected(intPtr, MarshalledUnityObject.Marshal(mat), subshaderIndex);
	}

	public bool RemoveVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
	{
		return RemoveVariantByShader(shader, passId, keywords);
	}

	public bool RemoveVariant(Material mat, PassIdentifier passId)
	{
		return RemoveVariantByMaterial(mat, passId);
	}

	[NativeName("RemoveVariant")]
	private bool RemoveVariantByShader(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemoveVariantByShader_Injected(intPtr, MarshalledUnityObject.Marshal(shader), ref passId, keywords);
	}

	[NativeName("RemoveVariant")]
	private bool RemoveVariantByMaterial(Material mat, PassIdentifier passId)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemoveVariantByMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(mat), ref passId);
	}

	public bool ContainsVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
	{
		return ContainsVariantByShader(shader, passId, keywords);
	}

	public bool ContainsVariant(Material mat, PassIdentifier passId)
	{
		return ContainsVariantByMaterial(mat, passId);
	}

	[NativeName("ContainsVariant")]
	private bool ContainsVariantByShader(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ContainsVariantByShader_Injected(intPtr, MarshalledUnityObject.Marshal(shader), ref passId, keywords);
	}

	[NativeName("ContainsVariant")]
	private bool ContainsVariantByMaterial(Material mat, PassIdentifier passId)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ContainsVariantByMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(mat), ref passId);
	}

	public void ClearVariants()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearVariants_Injected(intPtr);
	}

	public bool AddGraphicsStateForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords, GraphicsState setup)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddGraphicsStateForVariant_Injected(intPtr, MarshalledUnityObject.Marshal(shader), ref passId, keywords, ref setup);
	}

	public bool RemoveGraphicsStatesForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemoveGraphicsStatesForVariant_Injected(intPtr, MarshalledUnityObject.Marshal(shader), ref passId, keywords);
	}

	public bool CopyGraphicsStatesForVariant(Shader srcShader, PassIdentifier srcPassId, LocalKeyword[] srcKeywords, Shader dstShader, PassIdentifier dstPassId, LocalKeyword[] dstKeywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return CopyGraphicsStatesForVariant_Injected(intPtr, MarshalledUnityObject.Marshal(srcShader), ref srcPassId, srcKeywords, MarshalledUnityObject.Marshal(dstShader), ref dstPassId, dstKeywords);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("CreateFromScript")]
	private static extern void Internal_Create([Writable] GraphicsStateCollection gsc);

	public GraphicsStateCollection()
	{
		Internal_Create(this);
	}

	public GraphicsStateCollection(string filePath)
	{
		Internal_Create(this);
		LoadFromFile(filePath);
	}

	public void GetGraphicsStatesForVariant(ShaderVariant variant, List<GraphicsState> results)
	{
		GetGraphicsStatesForVariant(variant.shader, variant.passId, variant.keywords, results);
	}

	public int GetGraphicsStateCountForVariant(ShaderVariant variant)
	{
		return GetGraphicsStateCountForVariant(variant.shader, variant.passId, variant.keywords);
	}

	public bool AddGraphicsStateForVariant(ShaderVariant variant, GraphicsState setup)
	{
		return AddGraphicsStateForVariant(variant.shader, variant.passId, variant.keywords, setup);
	}

	public bool RemoveGraphicsStatesForVariant(ShaderVariant variant)
	{
		return RemoveGraphicsStatesForVariant(variant.shader, variant.passId, variant.keywords);
	}

	public bool CopyGraphicsStatesForVariant(ShaderVariant srcVariant, ShaderVariant dstVariant)
	{
		return CopyGraphicsStatesForVariant(srcVariant.shader, srcVariant.passId, srcVariant.keywords, dstVariant.shader, dstVariant.passId, dstVariant.keywords);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool BeginTrace_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EndTrace_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isTracing_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_version_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_version_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GraphicsDeviceType get_graphicsDeviceType_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_graphicsDeviceType_Injected(IntPtr _unity_self, GraphicsDeviceType value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RuntimePlatform get_runtimePlatform_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_runtimePlatform_Injected(IntPtr _unity_self, RuntimePlatform value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_qualityLevelName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_qualityLevelName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool LoadFromFile_Injected(IntPtr _unity_self, ref ManagedSpanWrapper filePath);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SaveToFile_Injected(IntPtr _unity_self, ref ManagedSpanWrapper filePath);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SendToEditor_Injected(IntPtr _unity_self, ref ManagedSpanWrapper fileName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WarmUp_Injected(IntPtr _unity_self, [In] ref JobHandle dependency, out JobHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WarmUpProgressively_Injected(IntPtr _unity_self, int count, [In] ref JobHandle dependency, out JobHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_totalGraphicsStateCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_completedWarmupCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isWarmedUp_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetVariants_Injected(IntPtr _unity_self, [Out] ShaderVariant[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGraphicsStatesForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords, [Out] GraphicsState[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_variantCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetGraphicsStateCountForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool AddVariantByShader_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool AddVariantByMaterial_Injected(IntPtr _unity_self, IntPtr mat, [In] ref PassIdentifier passId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool AddVariants_Injected(IntPtr _unity_self, IntPtr mat, [DefaultValue("-1")] int subshaderIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveVariantByShader_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveVariantByMaterial_Injected(IntPtr _unity_self, IntPtr mat, [In] ref PassIdentifier passId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ContainsVariantByShader_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ContainsVariantByMaterial_Injected(IntPtr _unity_self, IntPtr mat, [In] ref PassIdentifier passId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearVariants_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool AddGraphicsStateForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords, [In] ref GraphicsState setup);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveGraphicsStatesForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CopyGraphicsStatesForVariant_Injected(IntPtr _unity_self, IntPtr srcShader, [In] ref PassIdentifier srcPassId, LocalKeyword[] srcKeywords, IntPtr dstShader, [In] ref PassIdentifier dstPassId, LocalKeyword[] dstKeywords);
}
