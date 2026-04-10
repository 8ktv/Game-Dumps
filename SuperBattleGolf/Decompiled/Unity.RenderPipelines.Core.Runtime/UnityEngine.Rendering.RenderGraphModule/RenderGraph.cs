using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule;

[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
public class RenderGraph
{
	internal class DebugExecutionItem
	{
		public EntityId id { get; }

		public string name { get; }

		public DebugExecutionItem(EntityId id, string name)
		{
			this.id = id;
			this.name = name;
		}
	}

	[Serializable]
	internal class DebugData
	{
		[Serializable]
		public class ResourceLists<T>
		{
			[SerializeField]
			private List<T> m_Textures = new List<T>();

			[SerializeField]
			private List<T> m_Buffers = new List<T>();

			[SerializeField]
			private List<T> m_AccelerationStructures = new List<T>();

			public List<T> this[int index]
			{
				get
				{
					return index switch
					{
						0 => m_Textures, 
						1 => m_Buffers, 
						2 => m_AccelerationStructures, 
						_ => throw new ArgumentOutOfRangeException("index"), 
					};
				}
				set
				{
					switch (index)
					{
					case 0:
						m_Textures = value ?? new List<T>();
						break;
					case 1:
						m_Buffers = value ?? new List<T>();
						break;
					case 2:
						m_AccelerationStructures = value ?? new List<T>();
						break;
					default:
						throw new ArgumentOutOfRangeException("index");
					}
				}
			}

			public void Clear()
			{
				for (int i = 0; i < 3; i++)
				{
					if (this[i] == null)
					{
						this[i] = new List<T>();
					}
					else
					{
						this[i].Clear();
					}
				}
			}
		}

		[Serializable]
		public class ResourceDataLists : ResourceLists<ResourceData>
		{
		}

		[Serializable]
		public class SerializableNativePassAttachment
		{
			public RenderBufferLoadAction loadAction;

			public RenderBufferStoreAction storeAction;

			public bool memoryless;

			public int mipLevel;

			public int depthSlice;

			public SerializableNativePassAttachment(NativePassAttachment att)
			{
				loadAction = att.loadAction;
				storeAction = att.storeAction;
				memoryless = att.memoryless;
				mipLevel = att.mipLevel;
				depthSlice = att.depthSlice;
			}
		}

		[Serializable]
		[DebuggerDisplay("PassDebug: {name}")]
		public struct PassData
		{
			[Serializable]
			public class ResourceIdLists : ResourceLists<int>
			{
			}

			[Serializable]
			public class NRPInfo
			{
				[Serializable]
				public class NativeRenderPassInfo
				{
					[Serializable]
					public class AttachmentInfo
					{
						public string resourceName;

						public string loadReason;

						public string storeReason;

						public string storeMsaaReason;

						public int attachmentIndex;

						public SerializableNativePassAttachment attachment;
					}

					[Serializable]
					public struct PassCompatibilityInfo
					{
						public string message;

						public bool isCompatible;
					}

					public string passBreakReasoning;

					public List<AttachmentInfo> attachmentInfos;

					public SerializedDictionary<int, PassCompatibilityInfo> passCompatibility;

					public List<int> mergedPassIds;
				}

				[SerializeReference]
				public NativeRenderPassInfo nativePassInfo;

				public List<int> textureFBFetchList = new List<int>();

				public List<int> setGlobals = new List<int>();

				public int width;

				public int height;

				public int volumeDepth;

				public int samples;

				public bool hasDepth;
			}

			public string name;

			public RenderGraphPassType type;

			public ResourceIdLists resourceReadLists;

			public ResourceIdLists resourceWriteLists;

			public bool culled;

			public bool async;

			public int nativeSubPassIndex;

			public int syncToPassIndex;

			public int syncFromPassIndex;

			public bool generateDebugData;

			[SerializeReference]
			public NRPInfo nrpInfo;

			public PassScriptInfo scriptInfo;
		}

		[Serializable]
		public class BufferResourceData
		{
			public int count;

			public int stride;

			public GraphicsBuffer.Target target;

			public GraphicsBuffer.UsageFlags usage;
		}

		[Serializable]
		public class TextureResourceData
		{
			public int width;

			public int height;

			public int depth;

			public bool bindMS;

			public int samples;

			public GraphicsFormat format;

			public bool clearBuffer;
		}

		[Serializable]
		[DebuggerDisplay("ResourceDebug: {name} [{creationPassIndex}:{releasePassIndex}]")]
		public struct ResourceData
		{
			public string name;

			public bool imported;

			public int creationPassIndex;

			public int releasePassIndex;

			public List<int> consumerList;

			public List<int> producerList;

			public bool memoryless;

			[SerializeReference]
			public TextureResourceData textureData;

			[SerializeReference]
			public BufferResourceData bufferData;
		}

		[Serializable]
		public struct PassScriptInfo
		{
			public string filePath;

			public int line;
		}

		public string executionName;

		public bool valid;

		public int graphHash;

		public bool isNRPCompiler;

		public List<PassData> passList = new List<PassData>();

		public ResourceDataLists resourceLists = new ResourceDataLists();

		public DebugData(string executionName)
		{
			this.executionName = executionName;
		}

		public void Clear()
		{
			passList.Clear();
			resourceLists.Clear();
			valid = false;
		}
	}

	internal static class DebugDataSerialization
	{
		public static string ToJson(DebugData debugData)
		{
			if (debugData == null)
			{
				return string.Empty;
			}
			return JsonUtility.ToJson(debugData, prettyPrint: false);
		}

		public static DebugData FromJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return null;
			}
			return JsonUtility.FromJson<DebugData>(json);
		}
	}

	internal struct CompiledResourceInfo
	{
		public List<int> producers;

		public List<int> consumers;

		public int refCount;

		public bool imported;

		public void Reset()
		{
			if (producers == null)
			{
				producers = new List<int>();
			}
			if (consumers == null)
			{
				consumers = new List<int>();
			}
			producers.Clear();
			consumers.Clear();
			refCount = 0;
			imported = false;
		}
	}

	[DebuggerDisplay("RenderPass: {name} (Index:{index} Async:{enableAsyncCompute})")]
	internal struct CompiledPassInfo
	{
		public string name;

		public int index;

		public List<int>[] resourceCreateList;

		public List<int>[] resourceReleaseList;

		public GraphicsFence fence;

		public int refCount;

		public int syncToPassIndex;

		public int syncFromPassIndex;

		public bool enableAsyncCompute;

		public bool allowPassCulling;

		public bool needGraphicsFence;

		public bool culled;

		public bool culledByRendererList;

		public bool hasSideEffect;

		public bool enableFoveatedRasterization;

		public ExtendedFeatureFlags extendedFeatureFlags;

		public bool hasShadingRateImage;

		public bool hasShadingRateStates;

		public void Reset(RenderGraphPass pass, int index)
		{
			name = pass.name;
			this.index = index;
			enableAsyncCompute = pass.enableAsyncCompute;
			allowPassCulling = pass.allowPassCulling;
			enableFoveatedRasterization = pass.enableFoveatedRasterization;
			extendedFeatureFlags = ExtendedFeatureFlags.None;
			hasShadingRateImage = pass.hasShadingRateImage && !pass.enableFoveatedRasterization;
			hasShadingRateStates = pass.hasShadingRateStates && !pass.enableFoveatedRasterization;
			if (resourceCreateList == null)
			{
				resourceCreateList = new List<int>[3];
				resourceReleaseList = new List<int>[3];
				for (int i = 0; i < 3; i++)
				{
					resourceCreateList[i] = new List<int>();
					resourceReleaseList[i] = new List<int>();
				}
			}
			for (int j = 0; j < 3; j++)
			{
				resourceCreateList[j].Clear();
				resourceReleaseList[j].Clear();
			}
			refCount = 0;
			culled = false;
			culledByRendererList = false;
			hasSideEffect = false;
			syncToPassIndex = -1;
			syncFromPassIndex = -1;
			needGraphicsFence = false;
		}
	}

	internal interface ICompiledGraph
	{
		void Clear();
	}

	internal class CompiledGraph : ICompiledGraph
	{
		public DynamicArray<CompiledResourceInfo>[] compiledResourcesInfos = new DynamicArray<CompiledResourceInfo>[3];

		public DynamicArray<CompiledPassInfo> compiledPassInfos = new DynamicArray<CompiledPassInfo>();

		public int lastExecutionFrame;

		public CompiledGraph()
		{
			for (int i = 0; i < 3; i++)
			{
				compiledResourcesInfos[i] = new DynamicArray<CompiledResourceInfo>();
			}
		}

		public void Clear()
		{
			for (int i = 0; i < 3; i++)
			{
				compiledResourcesInfos[i].Clear();
			}
			compiledPassInfos.Clear();
		}

		private void InitResourceInfosData(DynamicArray<CompiledResourceInfo> resourceInfos, int count)
		{
			resourceInfos.Resize(count);
			for (int i = 0; i < resourceInfos.size; i++)
			{
				resourceInfos[i].Reset();
			}
		}

		public void InitializeCompilationData(List<RenderGraphPass> passes, RenderGraphResourceRegistry resources)
		{
			InitResourceInfosData(compiledResourcesInfos[0], resources.GetTextureResourceCount());
			InitResourceInfosData(compiledResourcesInfos[1], resources.GetBufferResourceCount());
			InitResourceInfosData(compiledResourcesInfos[2], resources.GetRayTracingAccelerationStructureResourceCount());
			compiledPassInfos.Resize(passes.Count);
			for (int i = 0; i < compiledPassInfos.size; i++)
			{
				compiledPassInfos[i].Reset(passes[i], i);
			}
		}
	}

	private class ProfilingScopePassData
	{
		public ProfilingSampler sampler;
	}

	internal delegate void OnGraphRegisteredDelegate(string graphName);

	internal delegate void OnExecutionRegisteredDelegate(string graphName, EntityId executionId, string executionName);

	internal static class RenderGraphExceptionMessages
	{
		internal static bool enableCaller = true;

		internal const string k_RenderGraphExecutionError = "Render Graph Execution error";

		private static readonly Dictionary<RenderGraphState, string> m_RenderGraphStateMessages = new Dictionary<RenderGraphState, string>
		{
			{
				RenderGraphState.RecordingPass,
				"This API cannot be called when Render Graph records a pass. Call the API within SetRenderFunc() or outside of AddUnsafePass()/AddComputePass()/AddRasterRenderPass()."
			},
			{
				RenderGraphState.RecordingGraph,
				"This API cannot be called during the Render Graph high-level recording step. Call the API within AddUnsafePass()/AddComputePass()/AddRasterRenderPass() or outside of RecordRenderGraph()."
			},
			{
				RenderGraphState.RecordingPass | RenderGraphState.Executing,
				"This API cannot be called when Render Graph records a pass or executes it. Call the API outside of AddUnsafePass()/AddComputePass()/AddRasterRenderPass()."
			},
			{
				RenderGraphState.Executing,
				"This API cannot be called during the Render Graph execution. Call the API outside of SetRenderFunc()."
			},
			{
				RenderGraphState.Active,
				"This API cannot be called when Render Graph is active. Call the API outside of RecordRenderGraph()."
			}
		};

		private const string k_ErrorDefaultMessage = "Invalid render graph state, impossible to log the exception.";

		internal const string k_NonTextureAsAttachmentError = "Only textures can be used as a fragment attachment.";

		internal const string k_OneResourceTwoVersionsError = "A pass is using SetAttachment or UseTexture on two versions of the same resource. Make sure you only access the latest version.";

		internal const string k_UseTextureRandWriteTwoVersionsError = "A pass is using UseTextureRandomWrite on two versions of the same resource.  Make sure you only access the latest version.";

		internal const string k_InvalidGetRenderTargetInfoResultsError = "GetRenderTargetInfo returned invalid results. Check that the width, height, and number of MSAA samples is not 0.";

		internal const string k_CannotDetermineSubPassFlagNoDepth = "SubPassFlag for merging cannot be determined if native pass doesn't have a depth attachment. Make sure your pass has a depth attachment.";

		internal const string k_AddingOlderAttachmentVersion = "The pass adds an older version while a higher version is already registered with the pass. Make sure you only access the latest version.";

		internal const string k_NonIncrementalCreationCall = "Something went wrong when compiling the graph. The Creation lists must be set-up incrementally for all passes, but AddFirstUse is called in an arbitrary non-incremental way.";

		internal const string k_NonIncrementalDestructionCall = "Something went wrong when compiling the graph. The Destruction lists must be set-up incrementally for all passes, AddLastUse is called in an arbitrary non-incremental way.";

		internal const string k_UndisposedBuilderPreviousPass = "Finish building the previous pass first by disposing of the pass builder object before adding a new pass. You can manually dispose of the builder with 'builder.Dispose()'.";

		internal const string k_WriteToVersionedResource = "The pass writes to a versioned resource handle. You can only write to unversioned resource handles to avoid branches in the resource history.";

		internal const string k_WriteToResourceTwice = "The pass writes to a resource twice. You can only write the same resource once within a pass.";

		internal const string k_TextureAlreadyBeingUsedThroughSetAttachment = "UseTexture is called on a texture that is already used through SetRenderAttachment. Check your code and make sure the texture is only used once.";

		internal const string k_SetRenderAttachmentTextureAlreadyUsed = "SetRenderAttachment is called on a texture that is already used through UseTexture/SetRenderAttachment. Check your code and make sure the texture is only used once.";

		internal const string k_SetRenderAttachmentOnDepthTexture = "SetRenderAttachment is called on a texture that has a depth format. Use a texture with a color format instead, or call SetRenderDepthAttachment.";

		internal const string k_SetRenderAttachmentOnGlobalTexture = "SetRenderAttachment is called on a texture that is currently bound to a global texture slot. Shaders might be using the texture using samplers. Make sure textures are not set as globals when using them as fragment attachments.";

		internal const string k_InvalidResource = "Using an invalid resource. Invalid resources can be resources leftover from a previous execution.";

		internal const string k_ReadWriteTransient = "This pass is reading or writing a transient resource. Transient resources are always assumed to be both read and written using 'AccessFlags.ReadWrite'.";

		internal const string k_MoreThanOneResourceForMRTIndex = "You can only bind a single texture to a single index in a multiple render texture (MRT). Verify your indexes are correct.";

		internal const string k_MoreThanOneTextureForFragInputIndex = "You can only bind a single texture to a fragment input index. Verify your indexes are correct.";

		internal const string k_MoreThanOneTextureRandomWriteInputIndex = "You can only bind a single texture to a random write input index. Verify your indexes are correct.";

		internal const string k_MultipleDepthTextures = "You can only set a single depth texture per pass.";

		internal const string k_LoadingMemorylessResource = "This pass is loading a resource marked as memoryless.";

		internal const string k_ResolvignMemorylessResource = "This pass is storing or resolving a resource marked as memoryless";

		internal const string k_RenderPassIsEmpty = "Empty render pass";

		internal const string k_RenderPassHasInvalidProperties = "Invalid render pass properties. One or more properties are zero.";

		internal const string k_ShadingRateImageAttachmentDoesNotMatch = "Low level rendergraph error: Shading rate image attachment in renderpass does not match.";

		internal const string k_AttachmentsDoNotMatch = "Low level rendergraph error: Attachments in renderpass do not match.";

		internal const string k_MultisampledShaderResolveInvalidAttachmentSetup = "Low level rendergraph error: last subpass with shader resolve must have one color attachment.";

		internal const string k_MultisampledShaderResolveInputAttachmentNotMemoryless = "Low level rendergraph error: last subpass with shader resolve must have all input attachments as memoryless attachments.";

		internal const string k_InvalidMRTSetup = "Multiple render texture (MRT) setup is invalid. Some indices are not used.";

		internal const string k_NoDepthBufferMRT = "Setting multiple render textures (MRTs) without a depth buffer is not supported.";

		internal const string k_InvalidDepthAndColorTargets = "Neither depth nor color render targets are correctly set up.";

		internal const string k_InvalidResourceType = "Invalid resource type, expected texture or buffer";

		internal const string k_NoRenderFunction = "RenderPass was not provided with an execute function.";

		internal const string k_BeginNoActivePass = "Compiler error: Pass is marked as beginning a native sub pass but no pass is currently active.";

		internal const string k_NoActivePassForSubpass = "Compiler error: Generated a subpass pass but no pass is currently active.";

		internal static string MismatchInDimensions(string name, int fragWidth, int fragHeight, int fragVolumeDepth, ResourceUnversionedData resInfo)
		{
			return $"Mismatch in fragment dimensions when using resource '{name}'. Expected {fragWidth} x {fragHeight} x {fragVolumeDepth} " + $"but got {resInfo.width} x {resInfo.height} x {resInfo.volumeDepth} instead.";
		}

		internal static string MismatchInMSAASamlpes(string name, int expectedSamples, int actualSamples)
		{
			string text = ((expectedSamples == 1) ? "None" : expectedSamples.ToString());
			string text2 = ((actualSamples == 1) ? "None" : actualSamples.ToString());
			return "Mismatch in number of MSAA samples when using resource '" + name + "'. Expected " + text + " but got " + text2 + " instead.";
		}

		internal static string NoGlobalTextureAtPropertyID(int propertyId)
		{
			return $"This pass is trying to read the global texture property {propertyId} but no previous pass in the graph bound a value to this global.";
		}

		internal static string UseDepthWithColorFormat(GraphicsFormat colorFormat)
		{
			return $"SetRenderAttachmentDepth is called on a texture that has a color format {colorFormat}. Use a texture with a depth format instead, or call SetRenderAttachment.";
		}

		internal static string UseTransientTextureInWrongPass(int transientIndex)
		{
			return $"This pass is using a transient resource from a different pass (pass index {transientIndex}). A transient resource should only be used in a single pass.";
		}

		internal static string IncompatibleTextureUVOrigin(TextureUVOriginSelection origin, string attachmentType, string attachmentName, RenderGraphResourceType attachmentResourceType, int attachmentResourceIndex, TextureUVOriginSelection attachmentOrigin)
		{
			return $"TextureUVOrigin `{origin}` is not compatible with existing {attachmentType} attachment `{attachmentType}` of type `{attachmentResourceType}` at index `{attachmentResourceIndex}` with TextureUVOrigin `{attachmentOrigin}`";
		}

		internal static string IncompatibleTextureUVOriginUseTexture(TextureUVOriginSelection origin)
		{
			return $"UseTexture() of a resource with `{origin}` is not compatible with Unity's standard UV origin for texture reading {TextureUVOrigin.BottomLeft}. Are you trying to UseTexture() on a backbuffer?";
		}

		internal static string UsingLegacyRenderGraph(string passName)
		{
			return "Pass '" + passName + "' is using the legacy rendergraph API. You cannot use legacy passes with the Native Render Pass Compiler. The APIs that are compatible with the Native Render Pass Compiler are AddUnsafePass, AddComputePass and AddRasterRenderPass.";
		}

		internal static string IncompatibleTextureUVOriginStore(string firstAttachmentName, TextureUVOriginSelection firstAttachmentOrigin, string secondAttachmentName, TextureUVOriginSelection secondAttachmentOrigin)
		{
			return $"Texture attachment {firstAttachmentName} with uv origin {firstAttachmentOrigin} does not match with texture attachment {secondAttachmentName} with uv origin {secondAttachmentOrigin}. Storing both would result in contents being flipped.";
		}

		internal static string GetExceptionMessage(RenderGraphState state)
		{
			string higherCaller = GetHigherCaller();
			if (!m_RenderGraphStateMessages.TryGetValue(state, out var value))
			{
				if (!enableCaller)
				{
					return "Invalid render graph state, impossible to log the exception.";
				}
				return "[" + higherCaller + "] Invalid render graph state, impossible to log the exception.";
			}
			if (!enableCaller)
			{
				return value;
			}
			return "[" + higherCaller + "] " + value;
		}

		private static string GetHigherCaller()
		{
			StackTrace stackTrace = new StackTrace(3, fNeedFileInfo: false);
			if (stackTrace.FrameCount > 0)
			{
				return stackTrace.GetFrame(0)?.GetMethod()?.Name ?? "UnknownCaller";
			}
			return "UnknownCaller";
		}
	}

	private NativePassCompiler nativeCompiler;

	public static readonly int kMaxMRTCount = 8;

	internal RenderGraphResourceRegistry m_Resources;

	private RenderGraphObjectPool m_RenderGraphPool = new RenderGraphObjectPool();

	private RenderGraphBuilders m_builderInstance = new RenderGraphBuilders();

	internal List<RenderGraphPass> m_RenderPasses = new List<RenderGraphPass>(64);

	private List<RendererListHandle> m_RendererLists = new List<RendererListHandle>(32);

	private RenderGraphDebugParams m_DebugParameters = new RenderGraphDebugParams();

	private RenderGraphLogger m_FrameInformationLogger = new RenderGraphLogger();

	private RenderGraphDefaultResources m_DefaultResources = new RenderGraphDefaultResources();

	private Dictionary<int, ProfilingSampler> m_DefaultProfilingSamplers = new Dictionary<int, ProfilingSampler>();

	private InternalRenderGraphContext m_RenderGraphContext = new InternalRenderGraphContext();

	private CommandBuffer m_PreviousCommandBuffer;

	private List<int>[] m_ImmediateModeResourceList = new List<int>[3];

	private RenderGraphCompilationCache m_CompilationCache;

	private RenderTargetIdentifier[][] m_TempMRTArrays;

	private Stack<int> m_CullingStack = new Stack<int>();

	private EntityId m_CurrentExecutionId;

	private bool m_CurrentExecutionCanGenerateDebugData;

	private int m_ExecutionCount;

	private int m_CurrentFrameIndex;

	private int m_CurrentImmediatePassIndex;

	private bool m_ExecutionExceptionWasRaised;

	private bool m_RendererListCulling;

	private bool m_EnableCompilationCaching;

	internal static bool? s_EnableCompilationCachingForTests;

	private CompiledGraph m_DefaultCompiledGraph = new CompiledGraph();

	private CompiledGraph m_CurrentCompiledGraph;

	private RenderGraphState m_RenderGraphState;

	private RenderTextureUVOriginStrategy m_renderTextureUVOriginStrategy;

	private static Dictionary<RenderGraph, List<DebugExecutionItem>> s_RegisteredExecutions = new Dictionary<RenderGraph, List<DebugExecutionItem>>();

	private const string k_BeginProfilingSamplerPassName = "BeginProfile";

	private const string k_EndProfilingSamplerPassName = "EndProfile";

	private static bool s_DebugSessionWasActive;

	private Dictionary<int, TextureHandle> registeredGlobals = new Dictionary<int, TextureHandle>();

	public bool nativeRenderPassesEnabled { get; set; } = true;

	internal static bool hasAnyRenderGraphWithNativeRenderPassesEnabled
	{
		get
		{
			foreach (KeyValuePair<RenderGraph, List<DebugExecutionItem>> s_RegisteredExecution in s_RegisteredExecutions)
			{
				s_RegisteredExecution.Deconstruct(out var key, out var _);
				if (key.nativeRenderPassesEnabled)
				{
					return true;
				}
			}
			return false;
		}
	}

	public string name { get; private set; } = "RenderGraph";

	internal RenderGraphState RenderGraphState
	{
		get
		{
			return m_RenderGraphState;
		}
		set
		{
			m_RenderGraphState = value;
		}
	}

	public RenderTextureUVOriginStrategy renderTextureUVOriginStrategy
	{
		get
		{
			return m_renderTextureUVOriginStrategy;
		}
		internal set
		{
			m_renderTextureUVOriginStrategy = value;
		}
	}

	public static bool isRenderGraphViewerActive => RenderGraphDebugSession.hasActiveDebugSession;

	internal static bool enableValidityChecks { get; private set; }

	public RenderGraphDefaultResources defaultResources => m_DefaultResources;

	internal RenderGraphDebugParams debugParams => m_DebugParameters;

	internal bool areAnySettingsActive => m_DebugParameters.AreAnySettingsActive;

	internal static event OnGraphRegisteredDelegate onGraphRegistered;

	internal static event OnGraphRegisteredDelegate onGraphUnregistered;

	internal static event OnExecutionRegisteredDelegate onExecutionRegistered;

	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	private void AddPassDebugMetadata(RenderGraphPass renderPass, string file, int line)
	{
	}

	internal NativePassCompiler CompileNativeRenderGraph(int graphHash)
	{
		using (new ProfilingScope(m_RenderGraphContext.cmd, ProfilingSampler.Get(RenderGraphProfileId.CompileRenderGraph)))
		{
			if (nativeCompiler == null)
			{
				nativeCompiler = new NativePassCompiler(m_CompilationCache);
			}
			if (!nativeCompiler.Initialize(m_Resources, m_RenderPasses, m_DebugParameters, name, m_EnableCompilationCaching, graphHash, m_ExecutionCount, m_renderTextureUVOriginStrategy))
			{
				nativeCompiler.Compile(m_Resources);
			}
			ref NativeList<PassData> passData = ref nativeCompiler.contextData.passData;
			int length = passData.Length;
			for (int i = 0; i < length; i++)
			{
				if (!passData.ElementAt(i).culled)
				{
					m_RendererLists.AddRange(m_RenderPasses[i].usedRendererListList);
				}
			}
			m_Resources.CreateRendererLists(m_RendererLists, m_RenderGraphContext.renderContext, m_RendererListCulling);
			return nativeCompiler;
		}
	}

	private void ExecuteNativeRenderGraph()
	{
		using (new ProfilingScope(m_RenderGraphContext.cmd, ProfilingSampler.Get(RenderGraphProfileId.ExecuteRenderGraph)))
		{
			nativeCompiler.ExecuteGraph(m_RenderGraphContext, m_Resources, in m_RenderPasses);
		}
	}

	public RenderGraph(string name = "RenderGraph")
	{
		this.name = name;
		if (GraphicsSettings.TryGetRenderPipelineSettings<RenderGraphGlobalSettings>(out var settings))
		{
			m_EnableCompilationCaching = settings.enableCompilationCaching;
			enableValidityChecks = settings.enableValidityChecks;
		}
		else
		{
			enableValidityChecks = true;
		}
		if (s_EnableCompilationCachingForTests.HasValue)
		{
			m_EnableCompilationCaching = s_EnableCompilationCachingForTests.Value;
		}
		if (m_EnableCompilationCaching)
		{
			m_CompilationCache = new RenderGraphCompilationCache();
		}
		m_TempMRTArrays = new RenderTargetIdentifier[kMaxMRTCount][];
		for (int i = 0; i < kMaxMRTCount; i++)
		{
			m_TempMRTArrays[i] = new RenderTargetIdentifier[i + 1];
		}
		m_Resources = new RenderGraphResourceRegistry(m_DebugParameters, m_FrameInformationLogger);
		RegisterGraph();
		m_RenderGraphState = RenderGraphState.Idle;
		RenderGraphExceptionMessages.enableCaller = true;
	}

	internal void CleanupResourcesAndGraph()
	{
		ClearCurrentCompiledGraph();
		m_Resources.Cleanup();
		m_DefaultResources.Cleanup();
		m_RenderGraphPool.Cleanup();
		nativeCompiler?.Cleanup();
		m_CompilationCache?.Clear();
		DelegateHashCodeUtils.ClearCache();
	}

	public void Cleanup()
	{
		m_CompilationCache?.Cleanup();
		CleanupResourcesAndGraph();
		UnregisterGraph();
	}

	internal List<DebugUI.Widget> GetWidgetList()
	{
		return m_DebugParameters.GetWidgetList(name);
	}

	public void RegisterDebug(DebugUI.Panel panel = null)
	{
		m_DebugParameters.RegisterDebug(name, panel);
	}

	public void UnRegisterDebug()
	{
		m_DebugParameters.UnRegisterDebug(name);
	}

	public static List<RenderGraph> GetRegisteredRenderGraphs()
	{
		return new List<RenderGraph>(s_RegisteredExecutions.Keys);
	}

	internal static Dictionary<RenderGraph, List<DebugExecutionItem>> GetRegisteredExecutions()
	{
		return s_RegisteredExecutions;
	}

	public void EndFrame()
	{
		m_Resources.PurgeUnusedGraphicsResources();
		if (m_DebugParameters.logFrameInformation)
		{
			m_FrameInformationLogger.FlushLogs();
		}
		if (m_DebugParameters.logResources)
		{
			m_Resources.FlushLogs();
		}
		m_DebugParameters.ResetLogging();
	}

	public TextureHandle ImportTexture(RTHandle rt)
	{
		return m_Resources.ImportTexture(in rt);
	}

	public TextureHandle ImportShadingRateImageTexture(RTHandle rt)
	{
		if (ShadingRateInfo.supportsPerImageTile)
		{
			return m_Resources.ImportTexture(in rt);
		}
		return TextureHandle.nullHandle;
	}

	public TextureHandle ImportTexture(RTHandle rt, ImportResourceParams importParams)
	{
		return m_Resources.ImportTexture(in rt, in importParams);
	}

	public TextureHandle ImportTexture(RTHandle rt, RenderTargetInfo info, ImportResourceParams importParams = default(ImportResourceParams))
	{
		return m_Resources.ImportTexture(in rt, info, in importParams);
	}

	internal TextureHandle ImportTexture(RTHandle rt, bool isBuiltin)
	{
		return m_Resources.ImportTexture(in rt, isBuiltin);
	}

	public TextureHandle ImportBackbuffer(RenderTargetIdentifier rt, RenderTargetInfo info, ImportResourceParams importParams = default(ImportResourceParams))
	{
		return m_Resources.ImportBackbuffer(rt, in info, in importParams);
	}

	public TextureHandle ImportBackbuffer(RenderTargetIdentifier rt)
	{
		RenderTargetInfo info = default(RenderTargetInfo);
		info.width = (info.height = (info.volumeDepth = (info.msaaSamples = 1)));
		info.format = GraphicsFormat.R8G8B8A8_SRGB;
		return m_Resources.ImportBackbuffer(rt, in info, default(ImportResourceParams));
	}

	public TextureHandle CreateTexture(in TextureDesc desc)
	{
		return m_Resources.CreateTexture(in desc);
	}

	[Obsolete("CreateSharedTexture() and shared texture workflow are deprecated, use ImportTexture() workflow instead.")]
	public TextureHandle CreateSharedTexture(in TextureDesc desc, bool explicitRelease = false)
	{
		return m_Resources.CreateSharedTexture(in desc, explicitRelease);
	}

	[Obsolete("RefreshSharedTextureDesc() and shared texture workflow are deprecated, use ImportTexture() workflow instead.")]
	public void RefreshSharedTextureDesc(TextureHandle handle, in TextureDesc desc)
	{
		m_Resources.RefreshSharedTextureDesc(in handle, in desc);
	}

	[Obsolete("ReleaseSharedTexture() and shared texture workflow are deprecated, use ImportTexture() workflow instead.")]
	public void ReleaseSharedTexture(TextureHandle texture)
	{
		m_Resources.ReleaseSharedTexture(in texture);
	}

	public TextureHandle CreateTexture(TextureHandle texture)
	{
		return m_Resources.CreateTexture(in m_Resources.GetTextureResourceDesc(in texture.handle));
	}

	public TextureHandle CreateTexture(TextureHandle texture, string name, bool clear = false)
	{
		TextureDesc desc = GetTextureDesc(in texture);
		desc.name = name;
		desc.clearBuffer = clear;
		return m_Resources.CreateTexture(in desc);
	}

	public void CreateTextureIfInvalid(in TextureDesc desc, ref TextureHandle texture)
	{
		if (!texture.IsValid())
		{
			texture = m_Resources.CreateTexture(in desc);
		}
	}

	public TextureDesc GetTextureDesc(in TextureHandle texture)
	{
		return m_Resources.GetTextureResourceDesc(in texture.handle);
	}

	public RenderTargetInfo GetRenderTargetInfo(TextureHandle texture)
	{
		m_Resources.GetRenderTargetInfo(in texture.handle, out var outInfo);
		return outInfo;
	}

	public RendererListHandle CreateRendererList(in RendererListDesc desc)
	{
		return m_Resources.CreateRendererList(in desc);
	}

	public RendererListHandle CreateRendererList(in RendererListParams desc)
	{
		return m_Resources.CreateRendererList(in desc);
	}

	public RendererListHandle CreateShadowRendererList(ref ShadowDrawingSettings shadowDrawingSettings)
	{
		return m_Resources.CreateShadowRendererList(m_RenderGraphContext.renderContext, ref shadowDrawingSettings);
	}

	public RendererListHandle CreateGizmoRendererList(in Camera camera, in GizmoSubset gizmoSubset)
	{
		return m_Resources.CreateGizmoRendererList(m_RenderGraphContext.renderContext, in camera, in gizmoSubset);
	}

	public RendererListHandle CreateUIOverlayRendererList(in Camera camera)
	{
		return m_Resources.CreateUIOverlayRendererList(m_RenderGraphContext.renderContext, in camera, UISubset.All);
	}

	public RendererListHandle CreateUIOverlayRendererList(in Camera camera, in UISubset uiSubset)
	{
		return m_Resources.CreateUIOverlayRendererList(m_RenderGraphContext.renderContext, in camera, in uiSubset);
	}

	public RendererListHandle CreateWireOverlayRendererList(in Camera camera)
	{
		return m_Resources.CreateWireOverlayRendererList(m_RenderGraphContext.renderContext, in camera);
	}

	public RendererListHandle CreateSkyboxRendererList(in Camera camera)
	{
		return m_Resources.CreateSkyboxRendererList(m_RenderGraphContext.renderContext, in camera);
	}

	public RendererListHandle CreateSkyboxRendererList(in Camera camera, Matrix4x4 projectionMatrix, Matrix4x4 viewMatrix)
	{
		return m_Resources.CreateSkyboxRendererList(m_RenderGraphContext.renderContext, in camera, projectionMatrix, viewMatrix);
	}

	public RendererListHandle CreateSkyboxRendererList(in Camera camera, Matrix4x4 projectionMatrixL, Matrix4x4 viewMatrixL, Matrix4x4 projectionMatrixR, Matrix4x4 viewMatrixR)
	{
		return m_Resources.CreateSkyboxRendererList(m_RenderGraphContext.renderContext, in camera, projectionMatrixL, viewMatrixL, projectionMatrixR, viewMatrixR);
	}

	[Obsolete("ImportBuffer with forceRelease parameter is deprecated. Use ImportBuffer without it instead. #from(6000.3)")]
	public BufferHandle ImportBuffer(GraphicsBuffer graphicsBuffer, bool forceRelease = false)
	{
		return ImportBuffer(graphicsBuffer);
	}

	public BufferHandle ImportBuffer(GraphicsBuffer graphicsBuffer)
	{
		return m_Resources.ImportBuffer(graphicsBuffer);
	}

	public BufferHandle CreateBuffer(in BufferDesc desc)
	{
		return m_Resources.CreateBuffer(in desc);
	}

	public BufferHandle CreateBuffer(in BufferHandle graphicsBuffer)
	{
		return m_Resources.CreateBuffer(in m_Resources.GetBufferResourceDesc(in graphicsBuffer.handle));
	}

	public BufferDesc GetBufferDesc(in BufferHandle graphicsBuffer)
	{
		return m_Resources.GetBufferResourceDesc(in graphicsBuffer.handle);
	}

	public RayTracingAccelerationStructureHandle ImportRayTracingAccelerationStructure(in RayTracingAccelerationStructure accelStruct, string name = null)
	{
		return m_Resources.ImportRayTracingAccelerationStructure(in accelStruct, name);
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckNotUsedWhenExecuting()
	{
		if (enableValidityChecks && m_RenderGraphState == RenderGraphState.Executing)
		{
			throw new InvalidOperationException(RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.Executing));
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckNotUsedWhenRecordingGraph()
	{
		if (enableValidityChecks && m_RenderGraphState == RenderGraphState.RecordingGraph)
		{
			throw new InvalidOperationException(RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.RecordingGraph));
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckNotUsedWhenRecordPassOrExecute()
	{
		if (enableValidityChecks && (m_RenderGraphState == RenderGraphState.RecordingPass || m_RenderGraphState == RenderGraphState.Executing))
		{
			throw new InvalidOperationException(RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.RecordingPass | RenderGraphState.Executing));
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckNotUsedWhenRecordingPass()
	{
		if (enableValidityChecks && m_RenderGraphState == RenderGraphState.RecordingPass)
		{
			throw new InvalidOperationException(RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.RecordingPass));
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckNotUsingNativeRenderPassCompiler()
	{
		if (enableValidityChecks && nativeRenderPassesEnabled)
		{
			throw new InvalidOperationException("`AddRenderPass` is not compatible with the Native Render Pass Compiler. It is meant to be used with the HDRP Compiler. The APIs that are compatible with the Native Render Pass Compiler are AddUnsafePass, AddComputePass and AddRasterRenderPass.");
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckUsingNativeRenderPassCompiler()
	{
		if (enableValidityChecks && (!nativeRenderPassesEnabled || nativeCompiler == null))
		{
			throw new InvalidOperationException("Only compatible with the Native Render Pass Compiler.");
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckNotUsedWhenActive()
	{
		if (enableValidityChecks && (m_RenderGraphState & RenderGraphState.Active) != RenderGraphState.Idle)
		{
			throw new InvalidOperationException(RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.Active));
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckNotUsedWhenIdle()
	{
		if (enableValidityChecks && m_RenderGraphState == RenderGraphState.Idle)
		{
			throw new InvalidOperationException(RenderGraphExceptionMessages.GetExceptionMessage(RenderGraphState.Active));
		}
	}

	public IRasterRenderGraphBuilder AddRasterRenderPass<PassData>(string passName, out PassData passData, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
	{
		return AddRasterRenderPass<PassData>(passName, out passData, GetDefaultProfilingSampler(passName), file, line);
	}

	public IRasterRenderGraphBuilder AddRasterRenderPass<PassData>(string passName, out PassData passData, ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
	{
		m_RenderGraphState = RenderGraphState.RecordingPass;
		RasterRenderGraphPass<PassData> rasterRenderGraphPass = m_RenderGraphPool.Get<RasterRenderGraphPass<PassData>>();
		rasterRenderGraphPass.Initialize(m_RenderPasses.Count, m_RenderGraphPool.Get<PassData>(), passName, RenderGraphPassType.Raster, sampler);
		passData = rasterRenderGraphPass.data;
		m_RenderPasses.Add(rasterRenderGraphPass);
		m_builderInstance.Setup(rasterRenderGraphPass, m_Resources, this);
		return m_builderInstance;
	}

	public IComputeRenderGraphBuilder AddComputePass<PassData>(string passName, out PassData passData, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
	{
		return AddComputePass<PassData>(passName, out passData, GetDefaultProfilingSampler(passName), file, line);
	}

	public IComputeRenderGraphBuilder AddComputePass<PassData>(string passName, out PassData passData, ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
	{
		m_RenderGraphState = RenderGraphState.RecordingPass;
		ComputeRenderGraphPass<PassData> computeRenderGraphPass = m_RenderGraphPool.Get<ComputeRenderGraphPass<PassData>>();
		computeRenderGraphPass.Initialize(m_RenderPasses.Count, m_RenderGraphPool.Get<PassData>(), passName, RenderGraphPassType.Compute, sampler);
		passData = computeRenderGraphPass.data;
		m_RenderPasses.Add(computeRenderGraphPass);
		m_builderInstance.Setup(computeRenderGraphPass, m_Resources, this);
		return m_builderInstance;
	}

	public IUnsafeRenderGraphBuilder AddUnsafePass<PassData>(string passName, out PassData passData, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
	{
		return AddUnsafePass<PassData>(passName, out passData, GetDefaultProfilingSampler(passName), file, line);
	}

	public IUnsafeRenderGraphBuilder AddUnsafePass<PassData>(string passName, out PassData passData, ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
	{
		m_RenderGraphState = RenderGraphState.RecordingPass;
		UnsafeRenderGraphPass<PassData> unsafeRenderGraphPass = m_RenderGraphPool.Get<UnsafeRenderGraphPass<PassData>>();
		unsafeRenderGraphPass.Initialize(m_RenderPasses.Count, m_RenderGraphPool.Get<PassData>(), passName, RenderGraphPassType.Unsafe, sampler);
		unsafeRenderGraphPass.AllowGlobalState(value: true);
		passData = unsafeRenderGraphPass.data;
		m_RenderPasses.Add(unsafeRenderGraphPass);
		m_builderInstance.Setup(unsafeRenderGraphPass, m_Resources, this);
		return m_builderInstance;
	}

	[Obsolete("AddRenderPass() is deprecated, use AddRasterRenderPass/AddComputePass/AddUnsafePass() instead.")]
	public RenderGraphBuilder AddRenderPass<PassData>(string passName, out PassData passData, ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
	{
		m_RenderGraphState = RenderGraphState.RecordingPass;
		RenderGraphPass<PassData> renderGraphPass = m_RenderGraphPool.Get<RenderGraphPass<PassData>>();
		renderGraphPass.Initialize(m_RenderPasses.Count, m_RenderGraphPool.Get<PassData>(), passName, RenderGraphPassType.Legacy, sampler);
		renderGraphPass.AllowGlobalState(value: true);
		passData = renderGraphPass.data;
		m_RenderPasses.Add(renderGraphPass);
		return new RenderGraphBuilder(renderGraphPass, m_Resources, this);
	}

	[Obsolete("AddRenderPass() is deprecated, use AddRasterRenderPass/AddComputePass/AddUnsafePass() instead.")]
	public RenderGraphBuilder AddRenderPass<PassData>(string passName, out PassData passData, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where PassData : class, new()
	{
		return AddRenderPass<PassData>(passName, out passData, GetDefaultProfilingSampler(passName), file, line);
	}

	public void BeginRecording(in RenderGraphParameters parameters)
	{
		m_ExecutionExceptionWasRaised = false;
		m_RenderGraphState = RenderGraphState.RecordingGraph;
		m_CurrentFrameIndex = parameters.currentFrameIndex;
		m_CurrentExecutionId = parameters.executionId;
		m_CurrentExecutionCanGenerateDebugData = parameters.generateDebugData && parameters.executionId != EntityId.None;
		m_RendererListCulling = parameters.rendererListCulling && !m_EnableCompilationCaching;
		m_renderTextureUVOriginStrategy = parameters.renderTextureUVOriginStrategy;
		m_Resources.BeginRenderGraph(m_ExecutionCount++);
		if (m_DebugParameters.enableLogging)
		{
			m_FrameInformationLogger.Initialize(GetExecutionNameAllocates(m_CurrentExecutionId));
		}
		m_DefaultResources.InitializeForRendering(this);
		m_RenderGraphContext.cmd = parameters.commandBuffer;
		m_RenderGraphContext.renderContext = parameters.scriptableRenderContext;
		m_RenderGraphContext.contextlessTesting = parameters.invalidContextForTesting;
		m_RenderGraphContext.renderGraphPool = m_RenderGraphPool;
		m_RenderGraphContext.defaultResources = m_DefaultResources;
		m_RenderGraphContext.forceResourceCreation = false;
		if (!m_DebugParameters.immediateMode)
		{
			return;
		}
		UpdateCurrentCompiledGraph(-1, forceNoCaching: true);
		LogFrameInformation();
		m_CurrentCompiledGraph.compiledPassInfos.Resize(m_CurrentCompiledGraph.compiledPassInfos.capacity);
		m_CurrentImmediatePassIndex = 0;
		for (int i = 0; i < 3; i++)
		{
			if (m_ImmediateModeResourceList[i] == null)
			{
				m_ImmediateModeResourceList[i] = new List<int>();
			}
			m_ImmediateModeResourceList[i].Clear();
		}
		m_Resources.BeginExecute(m_CurrentFrameIndex);
	}

	public void EndRecordingAndExecute()
	{
		Execute();
		if (m_DebugParameters.immediateMode)
		{
			ReleaseImmediateModeResources();
		}
		ClearCompiledGraph(m_CurrentCompiledGraph, m_EnableCompilationCaching);
		m_Resources.EndExecute();
		InvalidateContext();
		m_RenderGraphState = RenderGraphState.Idle;
	}

	public bool ResetGraphAndLogException(Exception e)
	{
		m_RenderGraphState = RenderGraphState.Idle;
		if (!m_RenderGraphContext.contextlessTesting)
		{
			Debug.LogError("Render Graph Execution error");
			if (!m_ExecutionExceptionWasRaised)
			{
				Debug.LogException(e);
			}
			m_ExecutionExceptionWasRaised = true;
		}
		CommandBuffer.ThrowOnSetRenderTarget = false;
		CleanupResourcesAndGraph();
		m_RenderGraphContext.cmd.Clear();
		return m_RenderGraphContext.contextlessTesting;
	}

	internal void Execute()
	{
		m_ExecutionExceptionWasRaised = false;
		m_RenderGraphState = RenderGraphState.Executing;
		if (!m_DebugParameters.immediateMode)
		{
			LogFrameInformation();
			int graphHash = 0;
			if (m_EnableCompilationCaching)
			{
				graphHash = ComputeGraphHash();
			}
			if (nativeRenderPassesEnabled)
			{
				CompileNativeRenderGraph(graphHash);
			}
			else
			{
				CompileRenderGraph(graphHash);
			}
			m_RenderGraphContext.compilerContext = ((!nativeRenderPassesEnabled) ? null : nativeCompiler?.contextData);
			m_Resources.BeginExecute(m_CurrentFrameIndex);
			if (nativeRenderPassesEnabled)
			{
				ExecuteNativeRenderGraph();
			}
			else
			{
				ExecuteRenderGraph();
			}
			ClearGlobalBindings();
		}
	}

	public void BeginProfilingSampler(ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (sampler == null)
		{
			return;
		}
		ProfilingScopePassData passData;
		using IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = AddUnsafePass<ProfilingScopePassData>("BeginProfile", out passData, null, file, line);
		passData.sampler = sampler;
		unsafeRenderGraphBuilder.AllowPassCulling(value: false);
		unsafeRenderGraphBuilder.GenerateDebugData(value: false);
		unsafeRenderGraphBuilder.SetRenderFunc(delegate(ProfilingScopePassData data, UnsafeGraphContext ctx)
		{
			data.sampler.Begin(CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd));
		});
	}

	public void EndProfilingSampler(ProfilingSampler sampler, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (sampler == null)
		{
			return;
		}
		ProfilingScopePassData passData;
		using IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = AddUnsafePass<ProfilingScopePassData>("EndProfile", out passData, null, file, line);
		passData.sampler = sampler;
		unsafeRenderGraphBuilder.AllowPassCulling(value: false);
		unsafeRenderGraphBuilder.GenerateDebugData(value: false);
		unsafeRenderGraphBuilder.SetRenderFunc(delegate(ProfilingScopePassData data, UnsafeGraphContext ctx)
		{
			data.sampler.End(CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd));
		});
	}

	internal DynamicArray<CompiledPassInfo> GetCompiledPassInfos()
	{
		return m_CurrentCompiledGraph.compiledPassInfos;
	}

	internal void ClearCurrentCompiledGraph()
	{
		ClearCompiledGraph(m_CurrentCompiledGraph, useCompilationCaching: false);
	}

	private void ClearCompiledGraph(CompiledGraph compiledGraph, bool useCompilationCaching)
	{
		ClearRenderPasses();
		m_Resources.Clear(m_ExecutionExceptionWasRaised);
		m_RendererLists.Clear();
		registeredGlobals.Clear();
		if (!useCompilationCaching && !nativeRenderPassesEnabled)
		{
			compiledGraph?.Clear();
		}
	}

	private void InvalidateContext()
	{
		m_RenderGraphContext.cmd = null;
		m_RenderGraphContext.renderGraphPool = null;
		m_RenderGraphContext.defaultResources = null;
		m_RenderGraphContext.compilerContext = null;
	}

	internal void OnPassAdded(RenderGraphPass pass)
	{
		if (m_DebugParameters.immediateMode)
		{
			ExecutePassImmediately(pass);
		}
	}

	internal int ComputeGraphHash()
	{
		using (new ProfilingScope(ProfilingSampler.Get(RenderGraphProfileId.ComputeHashRenderGraph)))
		{
			HashFNV1A32 generator = HashFNV1A32.Create();
			for (int i = 0; i < m_RenderPasses.Count; i++)
			{
				m_RenderPasses[i].ComputeHash(ref generator, m_Resources);
			}
			return generator.value;
		}
	}

	private void CountReferences()
	{
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		DynamicArray<CompiledResourceInfo>[] compiledResourcesInfos = m_CurrentCompiledGraph.compiledResourcesInfos;
		for (int i = 0; i < compiledPassInfos.size; i++)
		{
			RenderGraphPass renderGraphPass = m_RenderPasses[i];
			ref CompiledPassInfo reference = ref compiledPassInfos[i];
			for (int j = 0; j < 3; j++)
			{
				foreach (ResourceHandle item in renderGraphPass.resourceReadLists[j])
				{
					ResourceHandle res = item;
					ref CompiledResourceInfo reference2 = ref compiledResourcesInfos[j][res.index];
					reference2.imported = m_Resources.IsRenderGraphResourceImported(in res);
					reference2.consumers.Add(i);
					reference2.refCount++;
				}
				foreach (ResourceHandle item2 in renderGraphPass.resourceWriteLists[j])
				{
					ResourceHandle res2 = item2;
					ref CompiledResourceInfo reference3 = ref compiledResourcesInfos[j][res2.index];
					reference3.imported = m_Resources.IsRenderGraphResourceImported(in res2);
					reference3.producers.Add(i);
					reference.hasSideEffect = reference3.imported;
					reference.refCount++;
				}
				foreach (ResourceHandle item3 in renderGraphPass.transientResourceList[j])
				{
					ref CompiledResourceInfo reference4 = ref compiledResourcesInfos[j][item3.index];
					reference4.refCount++;
					reference4.consumers.Add(i);
					reference4.producers.Add(i);
				}
			}
		}
	}

	private void CullUnusedPasses()
	{
		if (m_DebugParameters.disablePassCulling)
		{
			if (m_DebugParameters.enableLogging)
			{
				m_FrameInformationLogger.LogLine("- Pass Culling Disabled -\n");
			}
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			DynamicArray<CompiledResourceInfo> dynamicArray = m_CurrentCompiledGraph.compiledResourcesInfos[i];
			m_CullingStack.Clear();
			for (int j = 1; j < dynamicArray.size; j++)
			{
				if (dynamicArray[j].refCount == 0)
				{
					m_CullingStack.Push(j);
				}
			}
			while (m_CullingStack.Count != 0)
			{
				foreach (int producer in dynamicArray[m_CullingStack.Pop()].producers)
				{
					ref CompiledPassInfo reference = ref m_CurrentCompiledGraph.compiledPassInfos[producer];
					RenderGraphPass renderGraphPass = m_RenderPasses[producer];
					reference.refCount--;
					if (reference.refCount != 0 || reference.hasSideEffect || !reference.allowPassCulling)
					{
						continue;
					}
					reference.culled = true;
					foreach (ResourceHandle item in renderGraphPass.resourceReadLists[i])
					{
						ref CompiledResourceInfo reference2 = ref dynamicArray[item.index];
						reference2.refCount--;
						if (reference2.refCount == 0)
						{
							m_CullingStack.Push(item.index);
						}
					}
				}
			}
		}
		LogCulledPasses();
	}

	private void UpdatePassSynchronization(ref CompiledPassInfo currentPassInfo, ref CompiledPassInfo producerPassInfo, int currentPassIndex, int lastProducer, ref int intLastSyncIndex)
	{
		currentPassInfo.syncToPassIndex = lastProducer;
		intLastSyncIndex = lastProducer;
		producerPassInfo.needGraphicsFence = true;
		if (producerPassInfo.syncFromPassIndex == -1)
		{
			producerPassInfo.syncFromPassIndex = currentPassIndex;
		}
	}

	private void UpdateResourceSynchronization(ref int lastGraphicsPipeSync, ref int lastComputePipeSync, int currentPassIndex, in CompiledResourceInfo resource)
	{
		int latestProducerIndex = GetLatestProducerIndex(currentPassIndex, in resource);
		if (latestProducerIndex == -1)
		{
			return;
		}
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		ref CompiledPassInfo reference = ref compiledPassInfos[currentPassIndex];
		if (m_CurrentCompiledGraph.compiledPassInfos[latestProducerIndex].enableAsyncCompute == reference.enableAsyncCompute)
		{
			return;
		}
		if (reference.enableAsyncCompute)
		{
			if (latestProducerIndex > lastGraphicsPipeSync)
			{
				UpdatePassSynchronization(ref reference, ref compiledPassInfos[latestProducerIndex], currentPassIndex, latestProducerIndex, ref lastGraphicsPipeSync);
			}
		}
		else if (latestProducerIndex > lastComputePipeSync)
		{
			UpdatePassSynchronization(ref reference, ref compiledPassInfos[latestProducerIndex], currentPassIndex, latestProducerIndex, ref lastComputePipeSync);
		}
	}

	private int GetFirstValidConsumerIndex(int passIndex, in CompiledResourceInfo info)
	{
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		foreach (int consumer in info.consumers)
		{
			if (consumer > passIndex && !compiledPassInfos[consumer].culled)
			{
				return consumer;
			}
		}
		return -1;
	}

	private int FindTextureProducer(int consumerPass, in CompiledResourceInfo info, out int index)
	{
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		int result = 0;
		for (index = 0; index < info.producers.Count; index++)
		{
			int num = info.producers[index];
			if (!compiledPassInfos[num].culled)
			{
				return num;
			}
			if (num >= consumerPass)
			{
				return result;
			}
			result = num;
		}
		return result;
	}

	private int GetLatestProducerIndex(int passIndex, in CompiledResourceInfo info)
	{
		int result = -1;
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		foreach (int producer in info.producers)
		{
			CompiledPassInfo compiledPassInfo = compiledPassInfos[producer];
			if (producer < passIndex && !compiledPassInfo.culled && !compiledPassInfo.culledByRendererList)
			{
				result = producer;
				continue;
			}
			return result;
		}
		return result;
	}

	private int GetLatestValidReadIndex(in CompiledResourceInfo info)
	{
		if (info.consumers.Count == 0)
		{
			return -1;
		}
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		List<int> consumers = info.consumers;
		for (int num = consumers.Count - 1; num >= 0; num--)
		{
			if (!compiledPassInfos[consumers[num]].culled)
			{
				return consumers[num];
			}
		}
		return -1;
	}

	private int GetFirstValidWriteIndex(in CompiledResourceInfo info)
	{
		if (info.producers.Count == 0)
		{
			return -1;
		}
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		List<int> producers = info.producers;
		for (int i = 0; i < producers.Count; i++)
		{
			if (!compiledPassInfos[producers[i]].culled)
			{
				return producers[i];
			}
		}
		return -1;
	}

	private int GetLatestValidWriteIndex(in CompiledResourceInfo info)
	{
		if (info.producers.Count == 0)
		{
			return -1;
		}
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		List<int> producers = info.producers;
		for (int num = producers.Count - 1; num >= 0; num--)
		{
			if (!compiledPassInfos[producers[num]].culled)
			{
				return producers[num];
			}
		}
		return -1;
	}

	private void CreateRendererLists()
	{
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		for (int i = 0; i < compiledPassInfos.size; i++)
		{
			ref CompiledPassInfo reference = ref compiledPassInfos[i];
			if (!reference.culled)
			{
				m_RendererLists.AddRange(m_RenderPasses[reference.index].usedRendererListList);
			}
		}
		m_Resources.CreateRendererLists(m_RendererLists, m_RenderGraphContext.renderContext, m_RendererListCulling);
	}

	internal bool GetImportedFallback(TextureDesc desc, out TextureHandle fallback)
	{
		fallback = TextureHandle.nullHandle;
		if (!desc.bindTextureMS)
		{
			if (desc.depthBufferBits != DepthBits.None)
			{
				fallback = defaultResources.whiteTexture;
			}
			else if (desc.clearColor == Color.black || desc.clearColor == default(Color))
			{
				if (desc.dimension == TextureXR.dimension)
				{
					fallback = defaultResources.blackTextureXR;
				}
				else if (desc.dimension == TextureDimension.Tex3D)
				{
					fallback = defaultResources.blackTexture3DXR;
				}
				else if (desc.dimension == TextureDimension.Tex2D)
				{
					fallback = defaultResources.blackTexture;
				}
			}
			else if (desc.clearColor == Color.white)
			{
				if (desc.dimension == TextureXR.dimension)
				{
					fallback = defaultResources.whiteTextureXR;
				}
				else if (desc.dimension == TextureDimension.Tex2D)
				{
					fallback = defaultResources.whiteTexture;
				}
			}
		}
		return fallback.IsValid();
	}

	private void AllocateCulledPassResources(ref CompiledPassInfo passInfo)
	{
		int index = passInfo.index;
		RenderGraphPass renderGraphPass = m_RenderPasses[index];
		for (int i = 0; i < 3; i++)
		{
			DynamicArray<CompiledResourceInfo> dynamicArray = m_CurrentCompiledGraph.compiledResourcesInfos[i];
			foreach (ResourceHandle item in renderGraphPass.resourceWriteLists[i])
			{
				ResourceHandle handle = item;
				ref CompiledResourceInfo reference = ref dynamicArray[handle.index];
				int firstValidConsumerIndex = GetFirstValidConsumerIndex(index, in reference);
				int index2;
				int num = FindTextureProducer(firstValidConsumerIndex, in reference, out index2);
				if (firstValidConsumerIndex == -1 || index != num)
				{
					continue;
				}
				if (i == 0)
				{
					TextureResource textureResource = m_Resources.GetTextureResource(in handle);
					if (!textureResource.desc.disableFallBackToImportedTexture && GetImportedFallback(textureResource.desc, out var fallback))
					{
						reference.imported = true;
						textureResource.imported = true;
						textureResource.graphicsResource = m_Resources.GetTexture(in fallback);
						continue;
					}
					textureResource.desc.sizeMode = TextureSizeMode.Explicit;
					textureResource.desc.width = 1;
					textureResource.desc.height = 1;
					textureResource.desc.clearBuffer = true;
				}
				reference.producers[index2 - 1] = firstValidConsumerIndex;
			}
		}
	}

	private void UpdateResourceAllocationAndSynchronization()
	{
		int lastGraphicsPipeSync = -1;
		int lastComputePipeSync = -1;
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		DynamicArray<CompiledResourceInfo>[] compiledResourcesInfos = m_CurrentCompiledGraph.compiledResourcesInfos;
		for (int i = 0; i < compiledPassInfos.size; i++)
		{
			ref CompiledPassInfo reference = ref compiledPassInfos[i];
			if (reference.culledByRendererList)
			{
				AllocateCulledPassResources(ref reference);
			}
			if (reference.culled)
			{
				continue;
			}
			RenderGraphPass renderGraphPass = m_RenderPasses[reference.index];
			for (int j = 0; j < 3; j++)
			{
				DynamicArray<CompiledResourceInfo> dynamicArray = compiledResourcesInfos[j];
				foreach (ResourceHandle item in renderGraphPass.resourceReadLists[j])
				{
					UpdateResourceSynchronization(ref lastGraphicsPipeSync, ref lastComputePipeSync, i, in dynamicArray[item.index]);
				}
				foreach (ResourceHandle item2 in renderGraphPass.resourceWriteLists[j])
				{
					UpdateResourceSynchronization(ref lastGraphicsPipeSync, ref lastComputePipeSync, i, in dynamicArray[item2.index]);
				}
			}
		}
		for (int k = 0; k < 3; k++)
		{
			DynamicArray<CompiledResourceInfo> dynamicArray2 = compiledResourcesInfos[k];
			for (int l = 1; l < dynamicArray2.size; l++)
			{
				CompiledResourceInfo info = dynamicArray2[l];
				bool flag = m_Resources.IsRenderGraphResourceShared((RenderGraphResourceType)k, l);
				if (info.imported && !flag)
				{
					continue;
				}
				int firstValidWriteIndex = GetFirstValidWriteIndex(in info);
				if (firstValidWriteIndex != -1)
				{
					compiledPassInfos[firstValidWriteIndex].resourceCreateList[k].Add(l);
				}
				int latestValidReadIndex = GetLatestValidReadIndex(in info);
				int latestValidWriteIndex = GetLatestValidWriteIndex(in info);
				int num = ((firstValidWriteIndex != -1 || info.imported) ? Math.Max(latestValidWriteIndex, latestValidReadIndex) : (-1));
				if (num != -1)
				{
					if (compiledPassInfos[num].enableAsyncCompute)
					{
						int num2 = num;
						int num3 = compiledPassInfos[num2].syncFromPassIndex;
						while (num3 == -1 && num2++ < compiledPassInfos.size - 1)
						{
							if (compiledPassInfos[num2].enableAsyncCompute)
							{
								num3 = compiledPassInfos[num2].syncFromPassIndex;
							}
						}
						if (num2 == compiledPassInfos.size)
						{
							if (!compiledPassInfos[num].hasSideEffect)
							{
								RenderGraphPass renderGraphPass2 = m_RenderPasses[num];
								string arg = "<unknown>";
								throw new InvalidOperationException($"{(RenderGraphResourceType)k} resource '{arg}' in asynchronous pass '{renderGraphPass2.name}' is missing synchronization on the graphics pipeline.");
							}
							num3 = num2;
						}
						int num4 = Math.Max(0, num3 - 1);
						while (compiledPassInfos[num4].culled)
						{
							num4 = Math.Max(0, num4 - 1);
						}
						compiledPassInfos[num4].resourceReleaseList[k].Add(l);
					}
					else
					{
						compiledPassInfos[num].resourceReleaseList[k].Add(l);
					}
				}
				if (flag && (firstValidWriteIndex != -1 || num != -1))
				{
					m_Resources.UpdateSharedResourceLastFrameIndex(k, l);
				}
			}
		}
	}

	private void UpdateAllSharedResourceLastFrameIndex()
	{
		for (int i = 0; i < 3; i++)
		{
			DynamicArray<CompiledResourceInfo> dynamicArray = m_CurrentCompiledGraph.compiledResourcesInfos[i];
			int sharedResourceCount = m_Resources.GetSharedResourceCount((RenderGraphResourceType)i);
			for (int j = 1; j <= sharedResourceCount; j++)
			{
				CompiledResourceInfo info = dynamicArray[j];
				int latestValidReadIndex = GetLatestValidReadIndex(in info);
				if (GetFirstValidWriteIndex(in info) != -1 || latestValidReadIndex != -1)
				{
					m_Resources.UpdateSharedResourceLastFrameIndex(i, j);
				}
			}
		}
	}

	private bool AreRendererListsEmpty(List<RendererListHandle> rendererLists)
	{
		foreach (RendererListHandle rendererList2 in rendererLists)
		{
			RendererListHandle handle = rendererList2;
			RendererList rendererList = m_Resources.GetRendererList(in handle);
			if (m_RenderGraphContext.renderContext.QueryRendererListStatus(rendererList) == RendererListStatus.kRendererListPopulated)
			{
				return false;
			}
		}
		if (rendererLists.Count <= 0)
		{
			return false;
		}
		return true;
	}

	private void TryCullPassAtIndex(int passIndex)
	{
		ref CompiledPassInfo reference = ref m_CurrentCompiledGraph.compiledPassInfos[passIndex];
		RenderGraphPass renderGraphPass = m_RenderPasses[passIndex];
		if (!reference.culled && renderGraphPass.allowPassCulling && renderGraphPass.allowRendererListCulling && !reference.hasSideEffect && AreRendererListsEmpty(renderGraphPass.usedRendererListList))
		{
			reference.culled = (reference.culledByRendererList = true);
		}
	}

	private void CullRendererLists()
	{
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		for (int i = 0; i < compiledPassInfos.size; i++)
		{
			CompiledPassInfo compiledPassInfo = compiledPassInfos[i];
			if (!compiledPassInfo.culled && !compiledPassInfo.hasSideEffect && m_RenderPasses[i].usedRendererListList.Count > 0)
			{
				TryCullPassAtIndex(i);
			}
		}
	}

	private bool UpdateCurrentCompiledGraph(int graphHash, bool forceNoCaching = false)
	{
		bool result = false;
		if (m_EnableCompilationCaching && !forceNoCaching)
		{
			result = m_CompilationCache.GetCompilationCache(graphHash, m_ExecutionCount, out m_CurrentCompiledGraph);
		}
		else
		{
			m_CurrentCompiledGraph = m_DefaultCompiledGraph;
		}
		return result;
	}

	internal void CompileRenderGraph(int graphHash)
	{
		using (new ProfilingScope(m_RenderGraphContext.cmd, ProfilingSampler.Get(RenderGraphProfileId.CompileRenderGraph)))
		{
			bool num = UpdateCurrentCompiledGraph(graphHash);
			if (!num)
			{
				m_CurrentCompiledGraph.Clear();
				m_CurrentCompiledGraph.InitializeCompilationData(m_RenderPasses, m_Resources);
				CountReferences();
				CullUnusedPasses();
			}
			CreateRendererLists();
			if (!num)
			{
				if (m_RendererListCulling)
				{
					CullRendererLists();
				}
				UpdateResourceAllocationAndSynchronization();
			}
			else
			{
				UpdateAllSharedResourceLastFrameIndex();
			}
			LogRendererListsCreation();
		}
	}

	private ref CompiledPassInfo CompilePassImmediatly(RenderGraphPass pass)
	{
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		if (m_CurrentImmediatePassIndex >= compiledPassInfos.size)
		{
			compiledPassInfos.Resize(compiledPassInfos.size * 2);
		}
		ref CompiledPassInfo reference = ref compiledPassInfos[m_CurrentImmediatePassIndex++];
		reference.Reset(pass, m_CurrentImmediatePassIndex - 1);
		reference.enableAsyncCompute = false;
		for (int i = 0; i < 3; i++)
		{
			foreach (ResourceHandle item in pass.transientResourceList[i])
			{
				reference.resourceCreateList[i].Add(item.index);
				reference.resourceReleaseList[i].Add(item.index);
			}
			foreach (ResourceHandle item2 in pass.resourceWriteLists[i])
			{
				ResourceHandle res = item2;
				if (!pass.transientResourceList[i].Contains(res) && !m_Resources.IsGraphicsResourceCreated(in res))
				{
					reference.resourceCreateList[i].Add(res.index);
					m_ImmediateModeResourceList[i].Add(res.index);
				}
			}
			foreach (ResourceHandle item3 in pass.resourceReadLists[i])
			{
				_ = item3;
			}
		}
		foreach (RendererListHandle usedRendererList in pass.usedRendererListList)
		{
			RendererListHandle res2 = usedRendererList;
			if (!m_Resources.IsRendererListCreated(in res2))
			{
				m_RendererLists.Add(res2);
			}
		}
		m_Resources.CreateRendererLists(m_RendererLists, m_RenderGraphContext.renderContext);
		m_RendererLists.Clear();
		return ref reference;
	}

	private void ExecutePassImmediately(RenderGraphPass pass)
	{
		ExecuteCompiledPass(ref CompilePassImmediatly(pass));
	}

	private void ExecuteCompiledPass(ref CompiledPassInfo passInfo)
	{
		if (passInfo.culled)
		{
			return;
		}
		RenderGraphPass renderGraphPass = m_RenderPasses[passInfo.index];
		if (!renderGraphPass.HasRenderFunc())
		{
			throw new InvalidOperationException("RenderPass " + renderGraphPass.name + " was not provided with an execute function.");
		}
		try
		{
			using (new ProfilingScope(m_RenderGraphContext.cmd, renderGraphPass.customSampler))
			{
				LogRenderPassBegin(in passInfo);
				using (new RenderGraphLogIndent(m_FrameInformationLogger))
				{
					m_RenderGraphContext.executingPass = renderGraphPass;
					PreRenderPassExecute(in passInfo, renderGraphPass, m_RenderGraphContext);
					renderGraphPass.Execute(m_RenderGraphContext);
					PostRenderPassExecute(ref passInfo, renderGraphPass, m_RenderGraphContext);
				}
			}
		}
		catch (Exception exception)
		{
			if (!m_RenderGraphContext.contextlessTesting)
			{
				m_ExecutionExceptionWasRaised = true;
				Debug.LogError($"Render Graph execution error at pass '{renderGraphPass.name}' ({passInfo.index})");
				Debug.LogException(exception);
			}
			throw;
		}
	}

	private void ExecuteRenderGraph()
	{
		using (new ProfilingScope(m_RenderGraphContext.cmd, ProfilingSampler.Get(RenderGraphProfileId.ExecuteRenderGraph)))
		{
			DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
			for (int i = 0; i < compiledPassInfos.size; i++)
			{
				ExecuteCompiledPass(ref compiledPassInfos[i]);
			}
		}
	}

	private void PreRenderPassSetRenderTargets(in CompiledPassInfo passInfo, RenderGraphPass pass, InternalRenderGraphContext rgContext)
	{
		if (passInfo.hasShadingRateImage)
		{
			CommandBuffer cmd = rgContext.cmd;
			RenderGraphResourceRegistry resources = m_Resources;
			TextureAccess shadingRateAccess = pass.shadingRateAccess;
			CoreUtils.SetShadingRateImage(cmd, (RenderTargetIdentifier)resources.GetTexture(in shadingRateAccess.textureHandle));
		}
		bool flag = pass.depthAccess.textureHandle.IsValid();
		if (!flag && pass.colorBufferMaxIndex == -1)
		{
			return;
		}
		TextureAccess[] colorBufferAccess = pass.colorBufferAccess;
		if (pass.colorBufferMaxIndex > 0)
		{
			RenderTargetIdentifier[] array = m_TempMRTArrays[pass.colorBufferMaxIndex];
			for (int i = 0; i <= pass.colorBufferMaxIndex; i++)
			{
				array[i] = m_Resources.GetTexture(in colorBufferAccess[i].textureHandle);
			}
			if (!flag)
			{
				throw new InvalidOperationException("Setting MRTs without a depth buffer is not supported.");
			}
			CommandBuffer cmd2 = rgContext.cmd;
			RenderGraphResourceRegistry resources2 = m_Resources;
			TextureAccess shadingRateAccess = pass.depthAccess;
			CoreUtils.SetRenderTarget(cmd2, array, resources2.GetTexture(in shadingRateAccess.textureHandle));
		}
		else if (flag)
		{
			if (pass.colorBufferMaxIndex > -1)
			{
				CommandBuffer cmd3 = rgContext.cmd;
				RTHandle texture = m_Resources.GetTexture(in pass.colorBufferAccess[0].textureHandle);
				RenderGraphResourceRegistry resources3 = m_Resources;
				TextureAccess shadingRateAccess = pass.depthAccess;
				CoreUtils.SetRenderTarget(cmd3, texture, resources3.GetTexture(in shadingRateAccess.textureHandle));
			}
			else
			{
				CommandBuffer cmd4 = rgContext.cmd;
				RenderGraphResourceRegistry resources4 = m_Resources;
				TextureAccess shadingRateAccess = pass.depthAccess;
				CoreUtils.SetRenderTarget(cmd4, resources4.GetTexture(in shadingRateAccess.textureHandle));
			}
		}
		else
		{
			if (!pass.colorBufferAccess[0].textureHandle.IsValid())
			{
				throw new InvalidOperationException("Neither Depth nor color render targets are correctly setup at pass " + pass.name + ".");
			}
			CoreUtils.SetRenderTarget(rgContext.cmd, m_Resources.GetTexture(in pass.colorBufferAccess[0].textureHandle));
		}
	}

	private void PreRenderPassExecute(in CompiledPassInfo passInfo, RenderGraphPass pass, InternalRenderGraphContext rgContext)
	{
		m_PreviousCommandBuffer = rgContext.cmd;
		bool flag = false;
		for (int i = 0; i < 3; i++)
		{
			foreach (int item in passInfo.resourceCreateList[i])
			{
				flag |= m_Resources.CreatePooledResource(rgContext, i, item);
			}
		}
		if (passInfo.enableFoveatedRasterization)
		{
			rgContext.cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Enabled);
		}
		if (passInfo.hasShadingRateStates)
		{
			rgContext.cmd.SetShadingRateFragmentSize(pass.shadingRateFragmentSize);
			rgContext.cmd.SetShadingRateCombiner(ShadingRateCombinerStage.Primitive, pass.primitiveShadingRateCombiner);
			rgContext.cmd.SetShadingRateCombiner(ShadingRateCombinerStage.Fragment, pass.fragmentShadingRateCombiner);
		}
		PreRenderPassSetRenderTargets(in passInfo, pass, rgContext);
		if (passInfo.enableAsyncCompute)
		{
			GraphicsFence fence = default(GraphicsFence);
			if (flag)
			{
				fence = rgContext.cmd.CreateGraphicsFence(GraphicsFenceType.AsyncQueueSynchronisation, SynchronisationStageFlags.AllGPUOperations);
			}
			if (!rgContext.contextlessTesting)
			{
				rgContext.renderContext.ExecuteCommandBuffer(rgContext.cmd);
			}
			rgContext.cmd.Clear();
			CommandBuffer commandBuffer = CommandBufferPool.Get(pass.name);
			commandBuffer.SetExecutionFlags(CommandBufferExecutionFlags.AsyncCompute);
			rgContext.cmd = commandBuffer;
			if (flag)
			{
				rgContext.cmd.WaitOnAsyncGraphicsFence(fence, SynchronisationStageFlags.PixelProcessing);
			}
		}
		if (passInfo.syncToPassIndex != -1)
		{
			rgContext.cmd.WaitOnAsyncGraphicsFence(m_CurrentCompiledGraph.compiledPassInfos[passInfo.syncToPassIndex].fence, SynchronisationStageFlags.PixelProcessing);
		}
	}

	private void PostRenderPassExecute(ref CompiledPassInfo passInfo, RenderGraphPass pass, InternalRenderGraphContext rgContext)
	{
		if (passInfo.hasShadingRateStates || passInfo.hasShadingRateImage)
		{
			rgContext.cmd.ResetShadingRate();
		}
		foreach (var setGlobals in pass.setGlobalsList)
		{
			rgContext.cmd.SetGlobalTexture(setGlobals.Item2, setGlobals.Item1);
		}
		if (passInfo.needGraphicsFence)
		{
			passInfo.fence = rgContext.cmd.CreateAsyncGraphicsFence();
		}
		if (passInfo.enableAsyncCompute)
		{
			rgContext.renderContext.ExecuteCommandBufferAsync(rgContext.cmd, ComputeQueueType.Background);
			CommandBufferPool.Release(rgContext.cmd);
			rgContext.cmd = m_PreviousCommandBuffer;
		}
		if (passInfo.enableFoveatedRasterization)
		{
			rgContext.cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
		}
		m_RenderGraphPool.ReleaseAllTempAlloc();
		for (int i = 0; i < 3; i++)
		{
			foreach (int item in passInfo.resourceReleaseList[i])
			{
				m_Resources.ReleasePooledResource(rgContext, i, item);
			}
		}
	}

	private void ClearRenderPasses()
	{
		foreach (RenderGraphPass renderPass in m_RenderPasses)
		{
			renderPass.Release(m_RenderGraphPool);
		}
		m_RenderPasses.Clear();
	}

	private void ReleaseImmediateModeResources()
	{
		for (int i = 0; i < 3; i++)
		{
			foreach (int item in m_ImmediateModeResourceList[i])
			{
				m_Resources.ReleasePooledResource(m_RenderGraphContext, i, item);
			}
		}
	}

	private void LogFrameInformation()
	{
		if (m_DebugParameters.enableLogging)
		{
			m_FrameInformationLogger.LogLine("==== Render Graph Frame Information Log " + GetExecutionNameAllocates(m_CurrentExecutionId) + " ====");
			if (!m_DebugParameters.immediateMode)
			{
				m_FrameInformationLogger.LogLine("Number of passes declared: {0}\n", m_RenderPasses.Count);
			}
		}
	}

	private void LogRendererListsCreation()
	{
		if (m_DebugParameters.enableLogging)
		{
			m_FrameInformationLogger.LogLine("Number of renderer lists created: {0}\n", m_RendererLists.Count);
		}
	}

	private void LogRenderPassBegin(in CompiledPassInfo passInfo)
	{
		if (!m_DebugParameters.enableLogging)
		{
			return;
		}
		RenderGraphPass renderGraphPass = m_RenderPasses[passInfo.index];
		m_FrameInformationLogger.LogLine("[{0}][{1}] \"{2}\"", renderGraphPass.index, renderGraphPass.enableAsyncCompute ? "Compute" : "Graphics", renderGraphPass.name);
		using (new RenderGraphLogIndent(m_FrameInformationLogger))
		{
			if (passInfo.syncToPassIndex != -1)
			{
				m_FrameInformationLogger.LogLine("Synchronize with [{0}]", passInfo.syncToPassIndex);
			}
		}
	}

	private void LogCulledPasses()
	{
		if (!m_DebugParameters.enableLogging)
		{
			return;
		}
		m_FrameInformationLogger.LogLine("Pass Culling Report:");
		using (new RenderGraphLogIndent(m_FrameInformationLogger))
		{
			DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
			for (int i = 0; i < compiledPassInfos.size; i++)
			{
				if (compiledPassInfos[i].culled)
				{
					RenderGraphPass renderGraphPass = m_RenderPasses[i];
					m_FrameInformationLogger.LogLine("[{0}] {1}", renderGraphPass.index, renderGraphPass.name);
				}
			}
			m_FrameInformationLogger.LogLine("\n");
		}
	}

	private ProfilingSampler GetDefaultProfilingSampler(string name)
	{
		return null;
	}

	private void UpdateImportedResourceLifeTime(ref DebugData.ResourceData data, List<int> passList)
	{
		foreach (int pass in passList)
		{
			if (data.creationPassIndex == -1)
			{
				data.creationPassIndex = pass;
			}
			else
			{
				data.creationPassIndex = Math.Min(data.creationPassIndex, pass);
			}
			if (data.releasePassIndex == -1)
			{
				data.releasePassIndex = pass;
			}
			else
			{
				data.releasePassIndex = Math.Max(data.releasePassIndex, pass);
			}
		}
	}

	private void RegisterGraph()
	{
		s_RegisteredExecutions.Add(this, new List<DebugExecutionItem>());
		RenderGraph.onGraphRegistered?.Invoke(name);
	}

	private void UnregisterGraph()
	{
		s_RegisteredExecutions.Remove(this);
		RenderGraph.onGraphUnregistered?.Invoke(name);
	}

	private static string GetExecutionNameAllocates(EntityId entityId)
	{
		Object obj = Resources.EntityIdToObject(entityId);
		if (!(obj != null))
		{
			return $"RenderGraphExecution ({entityId})";
		}
		return obj.name;
	}

	private void ClearCacheIfNewActiveDebugSession()
	{
		if (RenderGraphDebugSession.hasActiveDebugSession && !s_DebugSessionWasActive)
		{
			m_CompilationCache?.Clear();
		}
		s_DebugSessionWasActive = RenderGraphDebugSession.hasActiveDebugSession;
	}

	private void GenerateDebugData(int graphHash)
	{
		if (!RenderGraphDebugSession.hasActiveDebugSession || !m_CurrentExecutionCanGenerateDebugData || m_ExecutionExceptionWasRaised)
		{
			return;
		}
		List<DebugExecutionItem> list = s_RegisteredExecutions[this];
		bool flag = false;
		List<EntityId> value;
		using (ListPool<EntityId>.Get(out value))
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				DebugExecutionItem debugExecutionItem = list[num];
				if (Resources.EntityIdToObject(debugExecutionItem.id) == null)
				{
					list.RemoveAt(num);
					value.Add(debugExecutionItem.id);
				}
				else if (debugExecutionItem.id == m_CurrentExecutionId)
				{
					flag = true;
				}
			}
			string executionNameAllocates = GetExecutionNameAllocates(m_CurrentExecutionId);
			if (!flag)
			{
				list.Add(new DebugExecutionItem(m_CurrentExecutionId, executionNameAllocates));
			}
			if (value.Count > 0)
			{
				RenderGraphDebugSession.DeleteExecutionIds(name, value);
			}
			if (!flag)
			{
				RenderGraph.onExecutionRegistered?.Invoke(name, m_CurrentExecutionId, executionNameAllocates);
			}
			if (!m_EnableCompilationCaching)
			{
				graphHash = ComputeGraphHash();
			}
			DebugData debugData = RenderGraphDebugSession.GetDebugData(name, m_CurrentExecutionId);
			bool flag2 = debugData.executionName != executionNameAllocates;
			if (!debugData.valid || debugData.graphHash != graphHash || flag2)
			{
				debugData.Clear();
				debugData.executionName = executionNameAllocates;
				debugData.graphHash = graphHash;
				if (nativeRenderPassesEnabled)
				{
					nativeCompiler.GenerateNativeCompilerDebugData(ref debugData);
				}
				else
				{
					GenerateCompilerDebugData(ref debugData);
				}
				debugData.valid = true;
				RenderGraphDebugSession.SetDebugData(name, m_CurrentExecutionId, debugData);
			}
		}
	}

	private void GenerateCompilerDebugData(ref DebugData debugData)
	{
		DynamicArray<CompiledPassInfo> compiledPassInfos = m_CurrentCompiledGraph.compiledPassInfos;
		DynamicArray<CompiledResourceInfo>[] compiledResourcesInfos = m_CurrentCompiledGraph.compiledResourcesInfos;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < compiledResourcesInfos[i].size; j++)
			{
				ref CompiledResourceInfo reference = ref compiledResourcesInfos[i][j];
				DebugData.ResourceData data = default(DebugData.ResourceData);
				if (j != 0)
				{
					string renderGraphResourceName = m_Resources.GetRenderGraphResourceName((RenderGraphResourceType)i, j);
					data.name = ((!string.IsNullOrEmpty(renderGraphResourceName)) ? renderGraphResourceName : "(unnamed)");
					data.imported = m_Resources.IsRenderGraphResourceImported((RenderGraphResourceType)i, j);
				}
				else
				{
					data.name = "<null>";
					data.imported = true;
				}
				data.creationPassIndex = -1;
				data.releasePassIndex = -1;
				RenderGraphResourceType renderGraphResourceType = (RenderGraphResourceType)i;
				ResourceHandle res = new ResourceHandle(j, renderGraphResourceType, shared: false);
				if (j != 0 && res.IsValid())
				{
					switch (renderGraphResourceType)
					{
					case RenderGraphResourceType.Texture:
					{
						m_Resources.GetRenderTargetInfo(in res, out var outInfo);
						DebugData.TextureResourceData textureResourceData = new DebugData.TextureResourceData();
						textureResourceData.width = outInfo.width;
						textureResourceData.height = outInfo.height;
						textureResourceData.depth = outInfo.volumeDepth;
						textureResourceData.samples = outInfo.msaaSamples;
						textureResourceData.format = outInfo.format;
						textureResourceData.bindMS = outInfo.bindMS;
						data.textureData = textureResourceData;
						break;
					}
					case RenderGraphResourceType.Buffer:
					{
						ref readonly BufferDesc bufferResourceDesc = ref m_Resources.GetBufferResourceDesc(in res, noThrowOnInvalidDesc: true);
						DebugData.BufferResourceData bufferResourceData = new DebugData.BufferResourceData();
						bufferResourceData.count = bufferResourceDesc.count;
						bufferResourceData.stride = bufferResourceDesc.stride;
						bufferResourceData.target = bufferResourceDesc.target;
						bufferResourceData.usage = bufferResourceDesc.usageFlags;
						data.bufferData = bufferResourceData;
						break;
					}
					}
				}
				data.consumerList = new List<int>(reference.consumers);
				data.producerList = new List<int>(reference.producers);
				if (data.imported)
				{
					UpdateImportedResourceLifeTime(ref data, data.consumerList);
					UpdateImportedResourceLifeTime(ref data, data.producerList);
				}
				debugData.resourceLists[i].Add(data);
			}
		}
		for (int k = 0; k < compiledPassInfos.size; k++)
		{
			ref CompiledPassInfo reference2 = ref compiledPassInfos[k];
			RenderGraphPass renderGraphPass = m_RenderPasses[reference2.index];
			DebugData.PassData item = new DebugData.PassData
			{
				name = renderGraphPass.name,
				type = renderGraphPass.type,
				culled = reference2.culled,
				async = reference2.enableAsyncCompute,
				generateDebugData = renderGraphPass.generateDebugData,
				resourceReadLists = new DebugData.PassData.ResourceIdLists(),
				resourceWriteLists = new DebugData.PassData.ResourceIdLists(),
				syncFromPassIndex = reference2.syncFromPassIndex,
				syncToPassIndex = reference2.syncToPassIndex
			};
			for (int l = 0; l < 3; l++)
			{
				item.resourceReadLists[l] = new List<int>();
				item.resourceWriteLists[l] = new List<int>();
				foreach (ResourceHandle item2 in renderGraphPass.resourceReadLists[l])
				{
					item.resourceReadLists[l].Add(item2.index);
				}
				foreach (ResourceHandle item3 in renderGraphPass.resourceWriteLists[l])
				{
					item.resourceWriteLists[l].Add(item3.index);
				}
				foreach (int item4 in reference2.resourceCreateList[l])
				{
					DebugData.ResourceData value = debugData.resourceLists[l][item4];
					if (!value.imported)
					{
						value.creationPassIndex = k;
						debugData.resourceLists[l][item4] = value;
					}
				}
				foreach (int item5 in reference2.resourceReleaseList[l])
				{
					DebugData.ResourceData value2 = debugData.resourceLists[l][item5];
					if (!value2.imported)
					{
						value2.releasePassIndex = k;
						debugData.resourceLists[l][item5] = value2;
					}
				}
			}
			debugData.passList.Add(item);
		}
	}

	internal void SetGlobal(in TextureHandle h, int globalPropertyId)
	{
		if (!h.IsValid())
		{
			throw new ArgumentException("Attempting to register an invalid texture handle as a global");
		}
		registeredGlobals[globalPropertyId] = h;
	}

	internal bool IsGlobal(int globalPropertyId)
	{
		return registeredGlobals.ContainsKey(globalPropertyId);
	}

	internal Dictionary<int, TextureHandle>.ValueCollection AllGlobals()
	{
		return registeredGlobals.Values;
	}

	internal TextureHandle GetGlobal(int globalPropertyId)
	{
		registeredGlobals.TryGetValue(globalPropertyId, out var value);
		return value;
	}

	internal void ClearGlobalBindings()
	{
		foreach (KeyValuePair<int, TextureHandle> registeredGlobal in registeredGlobals)
		{
			m_RenderGraphContext.cmd.SetGlobalTexture(registeredGlobal.Key, defaultResources.blackTexture);
		}
	}
}
