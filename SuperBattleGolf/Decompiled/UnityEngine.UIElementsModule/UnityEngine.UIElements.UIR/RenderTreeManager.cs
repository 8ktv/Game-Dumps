#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine.Pool;

namespace UnityEngine.UIElements.UIR;

internal class RenderTreeManager : IDisposable
{
	internal struct ElementInsertionData
	{
		public VisualElement element;

		public bool canceled;
	}

	internal class VisualChangesProcessor : IDisposable
	{
		private enum VisualsProcessingType
		{
			Head,
			Tail
		}

		private struct EntryProcessingInfo
		{
			public RenderData renderData;

			public VisualsProcessingType type;

			public Entry rootEntry;
		}

		private static readonly ProfilerMarker k_GenerateEntriesMarker = new ProfilerMarker("UIR.GenerateEntries");

		private static readonly ProfilerMarker k_ConvertEntriesToCommandsMarker = new ProfilerMarker("UIR.ConvertEntriesToCommands");

		private static readonly ProfilerMarker k_UpdateOpacityIdMarker = new ProfilerMarker("UIR.UpdateOpacityId");

		private RenderTreeManager m_RenderTreeManager;

		private MeshGenerationContext m_MeshGenerationContext;

		private BaseElementBuilder m_ElementBuilder;

		private List<EntryProcessingInfo> m_EntryProcessingList;

		private List<EntryProcessor> m_Processors;

		public BaseElementBuilder elementBuilder => m_ElementBuilder;

		public MeshGenerationContext meshGenerationContext => m_MeshGenerationContext;

		protected bool disposed { get; private set; }

		public VisualChangesProcessor(RenderTreeManager renderTreeManager)
		{
			m_RenderTreeManager = renderTreeManager;
			m_MeshGenerationContext = new MeshGenerationContext(m_RenderTreeManager.meshWriteDataPool, m_RenderTreeManager.entryRecorder, m_RenderTreeManager.tempMeshAllocator, m_RenderTreeManager.meshGenerationDeferrer, m_RenderTreeManager.meshGenerationNodeManager);
			m_ElementBuilder = new DefaultElementBuilder(m_RenderTreeManager);
			m_EntryProcessingList = new List<EntryProcessingInfo>();
			m_Processors = new List<EntryProcessor>(4);
		}

		public void ScheduleMeshGenerationJobs()
		{
			m_ElementBuilder.ScheduleMeshGenerationJobs(m_MeshGenerationContext);
		}

		public void ProcessOnVisualsChanged(RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
		{
			bool flag = renderData.pendingHierarchicalRepaint || (renderData.dirtiedValues & RenderDataDirtyTypes.VisualsHierarchy) != 0;
			if (flag)
			{
				stats.recursiveVisualUpdates++;
			}
			else
			{
				stats.nonRecursiveVisualUpdates++;
			}
			DepthFirstOnVisualsChanged(renderData, dirtyID, flag, ref stats);
		}

		private void DepthFirstOnVisualsChanged(RenderData renderData, uint dirtyID, bool hierarchical, ref ChainBuilderStats stats)
		{
			if (dirtyID == renderData.dirtyID)
			{
				return;
			}
			renderData.dirtyID = dirtyID;
			if (hierarchical)
			{
				stats.recursiveVisualUpdatesExpanded++;
			}
			if (!renderData.owner.areAncestorsAndSelfDisplayed)
			{
				if (hierarchical)
				{
					renderData.pendingHierarchicalRepaint = true;
				}
				else
				{
					renderData.pendingRepaint = true;
				}
				return;
			}
			renderData.pendingHierarchicalRepaint = false;
			renderData.pendingRepaint = false;
			if (!hierarchical && (renderData.dirtiedValues & RenderDataDirtyTypes.AllVisuals) == RenderDataDirtyTypes.VisualsOpacityId)
			{
				stats.opacityIdUpdates++;
				UpdateOpacityId(renderData, m_RenderTreeManager);
				return;
			}
			UpdateWorldFlipsWinding(renderData);
			Debug.Assert(renderData.clipMethod != ClipMethod.Undetermined);
			Debug.Assert(RenderData.AllocatesID(renderData.transformID) || renderData.parent == null || renderData.transformID.Equals(renderData.parent.transformID) || renderData.isGroupTransform);
			if (renderData.owner is TextElement)
			{
				RenderEvents.UpdateTextCoreSettings(m_RenderTreeManager, renderData.owner);
			}
			if ((renderData.owner.renderHints & RenderHints.DynamicColor) == RenderHints.DynamicColor)
			{
				RenderEvents.SetColorValues(m_RenderTreeManager, renderData.owner);
			}
			Entry entry = m_RenderTreeManager.entryPool.Get();
			entry.type = EntryType.DedicatedPlaceholder;
			m_EntryProcessingList.Add(new EntryProcessingInfo
			{
				type = VisualsProcessingType.Head,
				renderData = renderData,
				rootEntry = entry
			});
			m_MeshGenerationContext.Begin(entry, renderData.owner, renderData);
			m_ElementBuilder.Build(m_MeshGenerationContext);
			m_MeshGenerationContext.End();
			if (hierarchical)
			{
				for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
				{
					DepthFirstOnVisualsChanged(renderData2, dirtyID, hierarchical: true, ref stats);
				}
			}
			m_EntryProcessingList.Add(new EntryProcessingInfo
			{
				type = VisualsProcessingType.Tail,
				renderData = renderData,
				rootEntry = entry
			});
		}

		private static void UpdateWorldFlipsWinding(RenderData renderData)
		{
			bool localFlipsWinding = renderData.localFlipsWinding;
			bool flag = renderData.parent?.worldFlipsWinding ?? false;
			renderData.worldFlipsWinding = flag ^ localFlipsWinding;
		}

		public void ConvertEntriesToCommands(ref ChainBuilderStats stats)
		{
			int num = 0;
			for (int i = 0; i < m_EntryProcessingList.Count; i++)
			{
				EntryProcessingInfo entryProcessingInfo = m_EntryProcessingList[i];
				if (entryProcessingInfo.type == VisualsProcessingType.Head)
				{
					EntryProcessor entryProcessor;
					if (num < m_Processors.Count)
					{
						entryProcessor = m_Processors[num];
					}
					else
					{
						entryProcessor = new EntryProcessor();
						m_Processors.Add(entryProcessor);
					}
					num++;
					entryProcessor.Init(entryProcessingInfo.rootEntry, m_RenderTreeManager, entryProcessingInfo.renderData);
					entryProcessor.ProcessHead();
					CommandManipulator.ReplaceHeadCommands(m_RenderTreeManager, entryProcessingInfo.renderData, entryProcessor);
				}
				else
				{
					num--;
					EntryProcessor entryProcessor2 = m_Processors[num];
					entryProcessor2.ProcessTail();
					CommandManipulator.ReplaceTailCommands(m_RenderTreeManager, entryProcessingInfo.renderData, entryProcessor2);
				}
			}
			m_EntryProcessingList.Clear();
			for (int j = 0; j < m_Processors.Count; j++)
			{
				m_Processors[j].ClearReferences();
			}
		}

		public static void UpdateOpacityId(RenderData renderData, RenderTreeManager renderTreeManager)
		{
			if (renderData.headMesh != null)
			{
				DoUpdateOpacityId(renderData, renderTreeManager, renderData.headMesh);
			}
			if (renderData.tailMesh != null)
			{
				DoUpdateOpacityId(renderData, renderTreeManager, renderData.tailMesh);
			}
			if (renderData.hasExtraMeshes)
			{
				ExtraRenderData orAddExtraData = renderTreeManager.GetOrAddExtraData(renderData);
				for (BasicNode<MeshHandle> basicNode = orAddExtraData.extraMesh; basicNode != null; basicNode = basicNode.next)
				{
					DoUpdateOpacityId(renderData, renderTreeManager, basicNode.data);
				}
			}
		}

		private static void DoUpdateOpacityId(RenderData renderData, RenderTreeManager renderTreeManager, MeshHandle mesh)
		{
			int size = (int)mesh.allocVerts.size;
			NativeSlice<Vertex> oldVerts = mesh.allocPage.vertices.cpuData.Slice((int)mesh.allocVerts.start, size);
			renderTreeManager.device.Update(mesh, (uint)size, out var vertexData);
			Color32 opacityData = renderTreeManager.shaderInfoAllocator.OpacityAllocToVertexData(renderData.opacityID);
			renderTreeManager.opacityIdAccelerator.CreateJob(oldVerts, vertexData, opacityData, size);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					m_MeshGenerationContext.Dispose();
					m_MeshGenerationContext = null;
				}
				disposed = true;
			}
		}
	}

	private RenderTreeCompositor m_Compositor;

	private VisualChangesProcessor m_VisualChangesProcessor;

	private LinkedPool<RenderChainCommand> m_CommandPool = new LinkedPool<RenderChainCommand>(() => new RenderChainCommand(), delegate(RenderChainCommand cmd)
	{
		cmd.Reset();
	});

	private LinkedPool<ExtraRenderData> m_ExtraDataPool = new LinkedPool<ExtraRenderData>(() => new ExtraRenderData(), null);

	private BasicNodePool<MeshHandle> m_MeshHandleNodePool = new BasicNodePool<MeshHandle>();

	private BasicNodePool<GraphicEntry> m_GraphicEntryPool = new BasicNodePool<GraphicEntry>();

	private Dictionary<RenderData, ExtraRenderData> m_ExtraData = new Dictionary<RenderData, ExtraRenderData>();

	internal List<ElementInsertionData> m_InsertionList = new List<ElementInsertionData>(1024);

	private MeshGenerationDeferrer m_MeshGenerationDeferrer = new MeshGenerationDeferrer();

	private Material m_DefaultMat;

	private bool m_BlockDirtyRegistration;

	private ChainBuilderStats m_Stats;

	private uint m_StatsElementsAdded;

	private uint m_StatsElementsRemoved;

	private TextureRegistry m_TextureRegistry = TextureRegistry.instance;

	private UnityEngine.Pool.ObjectPool<RenderData> m_RenderDataPool = new UnityEngine.Pool.ObjectPool<RenderData>(() => new RenderData(), null, null, null, collectionCheck: false, 256, 1024);

	private UnityEngine.Pool.ObjectPool<RenderTree> m_RenderTreePool = new UnityEngine.Pool.ObjectPool<RenderTree>(() => new RenderTree(), null, null, null, collectionCheck: false, 8, 128);

	private static EntryPool s_SharedEntryPool = new EntryPool(10000);

	private static readonly ProfilerMarker k_MarkerProcess = new ProfilerMarker("RenderTreeManager.Process");

	private static readonly ProfilerMarker k_MarkerSerialize = new ProfilerMarker("RenderChain.Serialize");

	private RenderTree m_RootRenderTree;

	public EntryRecorder entryRecorder = new EntryRecorder(s_SharedEntryPool);

	internal UIRVEShaderInfoAllocator shaderInfoAllocator;

	internal TextureRegistry textureRegistry => m_TextureRegistry;

	internal VisualChangesProcessor visualChangesProcessor => m_VisualChangesProcessor;

	public OpacityIdAccelerator opacityIdAccelerator { get; private set; }

	private bool blockDirtyRegistration { get; set; }

	public TextureSlotCount textureSlotCount { get; set; } = TextureSlotCount.Eight;

	protected bool disposed { get; private set; }

	internal ChainBuilderStats stats => m_Stats;

	internal ref ChainBuilderStats statsByRef => ref m_Stats;

	internal RenderTree rootRenderTree
	{
		get
		{
			return m_RootRenderTree;
		}
		set
		{
			Debug.Assert(m_RootRenderTree == null);
			m_RootRenderTree = value;
		}
	}

	internal BaseVisualElementPanel panel { get; private set; }

	internal UIRenderDevice device { get; private set; }

	public BaseElementBuilder elementBuilder => m_VisualChangesProcessor.elementBuilder;

	internal AtlasBase atlas { get; private set; }

	internal VectorImageManager vectorImageManager { get; private set; }

	internal TempMeshAllocatorImpl tempMeshAllocator { get; private set; }

	internal MeshWriteDataPool meshWriteDataPool { get; } = new MeshWriteDataPool();

	internal EntryPool entryPool => s_SharedEntryPool;

	public MeshGenerationDeferrer meshGenerationDeferrer => m_MeshGenerationDeferrer;

	public MeshGenerationNodeManager meshGenerationNodeManager { get; private set; }

	internal JobManager jobManager { get; private set; }

	internal bool drawStats { get; set; }

	internal bool drawInCameras { get; }

	internal bool isFlat { get; }

	public bool forceGammaRendering { get; }

	internal RenderData GetPooledRenderData()
	{
		RenderData renderData = m_RenderDataPool.Get();
		renderData.Init();
		return renderData;
	}

	internal void ReturnPoolRenderData(RenderData data)
	{
		if (data != null)
		{
			data.Reset();
			m_RenderDataPool.Release(data);
		}
	}

	internal RenderTree GetPooledRenderTree(RenderTreeManager renderTreeManager, RenderData rootRenderData)
	{
		RenderTree renderTree = m_RenderTreePool.Get();
		renderTree.Init(renderTreeManager, rootRenderData);
		return renderTree;
	}

	internal void ReturnPoolRenderTree(RenderTree tree)
	{
		if (tree != null)
		{
			tree.Reset();
			m_RenderTreePool.Release(tree);
		}
	}

	public RenderTreeManager(BaseVisualElementPanel panel)
	{
		this.panel = panel;
		atlas = panel.atlas;
		vectorImageManager = new VectorImageManager(atlas);
		m_Compositor = new RenderTreeCompositor(this);
		tempMeshAllocator = new TempMeshAllocatorImpl();
		jobManager = new JobManager();
		opacityIdAccelerator = new OpacityIdAccelerator();
		meshGenerationNodeManager = new MeshGenerationNodeManager(entryRecorder);
		m_VisualChangesProcessor = new VisualChangesProcessor(this);
		ColorSpace activeColorSpace = QualitySettings.activeColorSpace;
		m_DefaultMat = Shaders.defaultMaterial;
		if (panel.contextType == ContextType.Player)
		{
			BaseRuntimePanel baseRuntimePanel = (BaseRuntimePanel)panel;
			drawInCameras = baseRuntimePanel.drawsInCameras;
			if (!drawInCameras && activeColorSpace == ColorSpace.Linear)
			{
				forceGammaRendering = panel.panelRenderer.forceGammaRendering;
			}
		}
		else if (activeColorSpace == ColorSpace.Linear)
		{
			forceGammaRendering = true;
		}
		isFlat = panel.isFlat;
		device = new UIRenderDevice(panel.panelRenderer.vertexBudget, 0u, isFlat, forceGammaRendering);
		Shaders.Acquire();
		shaderInfoAllocator = new UIRVEShaderInfoAllocator((!forceGammaRendering) ? activeColorSpace : ColorSpace.Gamma);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			Shaders.Release();
			ReverseDepthFirstDisposeRenderTrees(m_RootRenderTree);
			m_RootRenderTree = null;
			tempMeshAllocator.Dispose();
			tempMeshAllocator = null;
			jobManager.Dispose();
			jobManager = null;
			vectorImageManager?.Dispose();
			vectorImageManager = null;
			shaderInfoAllocator.Dispose();
			shaderInfoAllocator = null;
			device?.Dispose();
			device = null;
			opacityIdAccelerator?.Dispose();
			opacityIdAccelerator = null;
			m_VisualChangesProcessor?.Dispose();
			m_VisualChangesProcessor = null;
			m_MeshGenerationDeferrer?.Dispose();
			m_MeshGenerationDeferrer = null;
			meshGenerationNodeManager.Dispose();
			meshGenerationNodeManager = null;
			m_Compositor.Dispose();
			m_Compositor = null;
			m_RenderDataPool.Clear();
			m_RenderDataPool = null;
			foreach (ElementInsertionData insertion in m_InsertionList)
			{
				insertion.element.insertionIndex = -1;
			}
			m_InsertionList.Clear();
			atlas = null;
		}
		disposed = true;
	}

	private static void ReverseDepthFirstDisposeRenderTrees(RenderTree renderTree)
	{
		for (RenderTree renderTree2 = renderTree?.firstChild; renderTree2 != null; renderTree2 = renderTree2.nextSibling)
		{
			ReverseDepthFirstDisposeRenderTrees(renderTree2);
		}
		renderTree?.Dispose();
	}

	private void DepthFirstProcessChanges(RenderTree renderTree)
	{
		renderTree.ProcessChanges(ref m_Stats);
		for (RenderTree renderTree2 = renderTree.firstChild; renderTree2 != null; renderTree2 = renderTree2.nextSibling)
		{
			DepthFirstProcessChanges(renderTree2);
		}
	}

	public void ProcessChanges()
	{
		m_Stats = default(ChainBuilderStats);
		m_Stats.elementsAdded += m_StatsElementsAdded;
		m_Stats.elementsRemoved += m_StatsElementsRemoved;
		m_StatsElementsAdded = (m_StatsElementsRemoved = 0u);
		for (int i = 0; i < m_InsertionList.Count; i++)
		{
			ElementInsertionData elementInsertionData = m_InsertionList[i];
			if (!elementInsertionData.canceled)
			{
				elementInsertionData.element.insertionIndex = -1;
				ProcessChildAdded(elementInsertionData.element);
			}
		}
		m_InsertionList.Clear();
		m_BlockDirtyRegistration = true;
		m_Compositor.Update(m_RootRenderTree);
		device.AdvanceFrame();
		DepthFirstProcessChanges(m_RootRenderTree);
		m_BlockDirtyRegistration = false;
		meshGenerationNodeManager.ResetAll();
		tempMeshAllocator.Clear();
		meshWriteDataPool.ReturnAll();
		entryPool.ReturnAll();
		atlas?.InvokeUpdateDynamicTextures(panel);
		vectorImageManager?.Commit();
		shaderInfoAllocator.IssuePendingStorageChanges();
		device.OnFrameRenderingBegin();
		RenderNestedTrees();
		if (drawInCameras)
		{
			SerializeRootTreeCommands();
		}
	}

	private void SerializeRootTreeCommands()
	{
		Debug.Assert(drawInCameras);
		if (m_RootRenderTree?.firstCommand != null)
		{
			Exception immediateException = null;
			m_BlockDirtyRegistration = true;
			device.EvaluateChain(m_RootRenderTree.firstCommand, m_DefaultMat, vectorImageManager?.atlas, shaderInfoAllocator.atlas, null, panel.scaledPixelsPerPoint, isSerializing: true, textureSlotCount, isRenderingNestedTreeRT: false, ref immediateException);
			m_BlockDirtyRegistration = false;
			Debug.Assert(immediateException == null);
		}
	}

	public void RenderRootTree()
	{
		Debug.Assert(!drawInCameras);
		PanelClearSettings clearSettings = panel.clearSettings;
		if (clearSettings.clearColor || clearSettings.clearDepthStencil)
		{
			Color color = clearSettings.color;
			color = color.RGBMultiplied(color.a);
			GL.Clear(clearSettings.clearDepthStencil, clearSettings.clearColor, color, 0.99f);
		}
		RenderSingleTree(m_RootRenderTree, null, RectInt.zero, Rect.zero);
		if (drawStats)
		{
			DrawStats();
		}
	}

	private void RenderNestedTrees()
	{
		m_Compositor.RenderNestedPasses();
	}

	public void RenderSingleTree(RenderTree renderTree, RenderTexture nestedTreeRT, RectInt nestedTreeViewport, Rect bounds)
	{
		Debug.Assert(!drawInCameras || renderTree != m_RootRenderTree);
		if (renderTree.firstCommand == null)
		{
			return;
		}
		Exception immediateException = null;
		bool flag = false;
		RenderTexture active = null;
		float pixelsPerPoint = panel.scaledPixelsPerPoint;
		Rect value;
		if (renderTree == m_RootRenderTree)
		{
			Debug.Assert(nestedTreeRT == null);
			Rect layout = panel.visualTree.layout;
			value = new Rect(0f, 0f, layout.width, layout.height);
			bounds = layout;
		}
		else
		{
			Debug.Assert(nestedTreeRT != null);
			active = RenderTexture.active;
			Camera.SetupCurrent(null);
			RenderTexture.active = nestedTreeRT;
			flag = true;
			pixelsPerPoint = 1f;
			Rect rect = UIRUtility.CastToRect(nestedTreeViewport);
			value = rect;
			value.y = value.height - value.yMax;
			GL.Viewport(rect);
		}
		Matrix4x4 mat = ProjectionUtils.Ortho(bounds.xMin, bounds.xMax, bounds.yMax, bounds.yMin, -0.001f, 1.001f);
		GL.LoadProjectionMatrix(mat);
		GL.modelview = Matrix4x4.identity;
		m_BlockDirtyRegistration = drawInCameras;
		device.EvaluateChain(renderTree.firstCommand, m_DefaultMat, vectorImageManager?.atlas, shaderInfoAllocator.atlas, value, pixelsPerPoint, isSerializing: false, textureSlotCount, nestedTreeRT != null, ref immediateException);
		m_BlockDirtyRegistration = false;
		Utility.DisableScissor();
		if (flag)
		{
			RenderTexture.active = active;
		}
		if (immediateException == null)
		{
			return;
		}
		Debug.Assert(!drawInCameras);
		if (GUIUtility.IsExitGUIException(immediateException))
		{
			throw immediateException;
		}
		throw new ImmediateModeException(immediateException);
	}

	public void CancelInsertion(VisualElement ve)
	{
		int insertionIndex = ve.insertionIndex;
		Debug.Assert(insertionIndex >= 0 && insertionIndex < m_InsertionList.Count);
		ElementInsertionData value = m_InsertionList[insertionIndex];
		value.canceled = true;
		m_InsertionList[insertionIndex] = value;
		ve.insertionIndex = -1;
	}

	public void UIEOnChildAdded(VisualElement ve)
	{
		ve.insertionIndex = m_InsertionList.Count;
		m_InsertionList.Add(new ElementInsertionData
		{
			element = ve,
			canceled = false
		});
	}

	private void ProcessChildAdded(VisualElement ve)
	{
		VisualElement parent = ve.hierarchy.parent;
		int index = parent?.hierarchy.IndexOf(ve) ?? 0;
		if (m_BlockDirtyRegistration)
		{
			throw new InvalidOperationException("VisualElements cannot be added to an active visual tree during generateVisualContent callback execution nor during visual tree rendering");
		}
		if (parent == null || parent.renderData != null)
		{
			uint num = RenderEvents.DepthFirstOnChildAdded(this, parent, ve, index);
			Debug.Assert(ve.renderData != null);
			Debug.Assert(ve.panel == panel);
			UIEOnClippingChanged(ve, hierarchical: true);
			UIEOnOpacityChanged(ve);
			UIEOnTransformOrSizeChanged(ve, transformChanged: true, clipRectSizeChanged: true);
			UIEOnVisualsChanged(ve, hierarchical: true);
			ve.MarkRenderHintsClean();
			m_StatsElementsAdded += num;
		}
	}

	public void UIEOnChildrenReordered(VisualElement ve)
	{
		if (m_BlockDirtyRegistration)
		{
			throw new InvalidOperationException("VisualElements cannot be moved under an active visual tree during generateVisualContent callback execution nor during visual tree rendering");
		}
		int childCount = ve.hierarchy.childCount;
		for (int i = 0; i < childCount; i++)
		{
			RenderEvents.DepthFirstOnElementRemoving(this, ve.hierarchy[i]);
		}
		for (int j = 0; j < childCount; j++)
		{
			UIEOnChildAdded(ve.hierarchy[j]);
		}
		UIEOnClippingChanged(ve, hierarchical: true);
		UIEOnOpacityChanged(ve, hierarchical: true);
		UIEOnVisualsChanged(ve, hierarchical: true);
	}

	public void UIEOnChildRemoving(VisualElement ve)
	{
		if (m_BlockDirtyRegistration)
		{
			throw new InvalidOperationException("VisualElements cannot be removed from an active visual tree during generateVisualContent callback execution nor during visual tree rendering");
		}
		m_StatsElementsRemoved += RenderEvents.DepthFirstOnElementRemoving(this, ve);
		Debug.Assert(ve.renderData == null);
	}

	public void UIEOnRenderHintsChanged(VisualElement ve)
	{
		if (ve.renderData != null)
		{
			if (m_BlockDirtyRegistration)
			{
				throw new InvalidOperationException("Render Hints cannot change under an active visual tree during generateVisualContent callback execution nor during visual tree rendering");
			}
			if ((ve.renderHints & RenderHints.DirtyAll) == RenderHints.DirtyDynamicColor)
			{
				UIEOnVisualsChanged(ve, hierarchical: false);
			}
			else
			{
				UIEOnChildRemoving(ve);
				UIEOnChildAdded(ve);
			}
			ve.MarkRenderHintsClean();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void RegisterDirty(VisualElement ve, RenderDataDirtyTypes dirtyTypes, RenderDataDirtyTypeClasses dirtyClasses)
	{
		RenderData renderData = ve.renderData;
		if (renderData != null)
		{
			if (m_BlockDirtyRegistration)
			{
				throw new InvalidOperationException("VisualElements cannot change their render data under an active visual tree during generateVisualContent callback execution nor during visual tree rendering");
			}
			renderData.renderTree.dirtyTracker.RegisterDirty(renderData, dirtyTypes, dirtyClasses);
			if (ve.nestedRenderData != null)
			{
				ve.nestedRenderData.renderTree.dirtyTracker.RegisterDirty(ve.nestedRenderData, dirtyTypes, dirtyClasses);
			}
		}
	}

	public void UIEOnClippingChanged(VisualElement ve, bool hierarchical)
	{
		RegisterDirty(ve, (RenderDataDirtyTypes)(4 | (hierarchical ? 8 : 0)), RenderDataDirtyTypeClasses.Clipping);
	}

	public void UIEOnOpacityChanged(VisualElement ve, bool hierarchical = false)
	{
		RegisterDirty(ve, (RenderDataDirtyTypes)(0x80 | (hierarchical ? 256 : 0)), RenderDataDirtyTypeClasses.Opacity);
	}

	public void UIEOnColorChanged(VisualElement ve)
	{
		RegisterDirty(ve, RenderDataDirtyTypes.Color, RenderDataDirtyTypeClasses.Color);
	}

	public void UIEOnTransformOrSizeChanged(VisualElement ve, bool transformChanged, bool clipRectSizeChanged)
	{
		RenderDataDirtyTypes dirtyTypes = (RenderDataDirtyTypes)((transformChanged ? 1 : 0) | (clipRectSizeChanged ? 2 : 0));
		RegisterDirty(ve, dirtyTypes, RenderDataDirtyTypeClasses.TransformSize);
	}

	public void UIEOnVisualsChanged(VisualElement ve, bool hierarchical)
	{
		RegisterDirty(ve, (RenderDataDirtyTypes)(0x10 | (hierarchical ? 32 : 0)), RenderDataDirtyTypeClasses.Visuals);
	}

	public void UIEOnOpacityIdChanged(VisualElement ve)
	{
		RegisterDirty(ve, RenderDataDirtyTypes.VisualsOpacityId, RenderDataDirtyTypeClasses.Visuals);
	}

	public void UIEOnDisableRenderingChanged(VisualElement ve)
	{
		if (ve.renderData != null)
		{
			if (m_BlockDirtyRegistration)
			{
				throw new InvalidOperationException("VisualElements cannot change their display style during generateVisualContent callback execution nor during visual tree rendering");
			}
			CommandManipulator.DisableElementRendering(this, ve, ve.disableRendering);
		}
	}

	internal RenderChainCommand AllocCommand()
	{
		return m_CommandPool.Get();
	}

	internal void FreeCommand(RenderChainCommand cmd)
	{
		cmd.Reset();
		m_CommandPool.Return(cmd);
	}

	internal void RepaintTexturedElements()
	{
		if (m_RootRenderTree != null)
		{
			DepthFirstRepaintTextured(m_RootRenderTree);
		}
	}

	private void DepthFirstRepaintTextured(RenderTree renderTree)
	{
		RenderData rootRenderData = renderTree.rootRenderData;
		if (rootRenderData != null)
		{
			DepthFirstRepaintTextured(rootRenderData);
		}
		for (RenderTree renderTree2 = renderTree.firstChild; renderTree2 != null; renderTree2 = renderTree2.nextSibling)
		{
			DepthFirstRepaintTextured(renderTree2);
		}
	}

	private void DepthFirstRepaintTextured(RenderData renderData)
	{
		if (renderData.graphicEntries != null)
		{
			UIEOnVisualsChanged(renderData.owner, hierarchical: false);
		}
		for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
		{
			DepthFirstRepaintTextured(renderData2);
		}
	}

	public ExtraRenderData GetOrAddExtraData(RenderData renderData)
	{
		if (!m_ExtraData.TryGetValue(renderData, out var value))
		{
			value = m_ExtraDataPool.Get();
			m_ExtraData.Add(renderData, value);
			renderData.flags |= RenderDataFlags.HasExtraData;
		}
		return value;
	}

	public void FreeExtraData(RenderData renderData)
	{
		Debug.Assert(renderData.hasExtraData);
		Debug.Assert(!renderData.hasExtraMeshes);
		m_ExtraData.Remove(renderData, out var value);
		m_ExtraDataPool.Return(value);
		renderData.flags &= ~RenderDataFlags.HasExtraData;
	}

	public void InsertExtraMesh(RenderData renderData, MeshHandle mesh)
	{
		ExtraRenderData orAddExtraData = GetOrAddExtraData(renderData);
		BasicNode<MeshHandle> basicNode = m_MeshHandleNodePool.Get();
		basicNode.data = mesh;
		basicNode.InsertFirst(ref orAddExtraData.extraMesh);
		renderData.flags |= RenderDataFlags.HasExtraMeshes;
	}

	public void FreeExtraMeshes(RenderData renderData)
	{
		if (renderData.hasExtraMeshes)
		{
			ExtraRenderData extraRenderData = m_ExtraData[renderData];
			BasicNode<MeshHandle> basicNode = extraRenderData.extraMesh;
			extraRenderData.extraMesh = null;
			while (basicNode != null)
			{
				device.Free(basicNode.data);
				BasicNode<MeshHandle> next = basicNode.next;
				basicNode.data = null;
				basicNode.next = null;
				m_MeshHandleNodePool.Return(basicNode);
				basicNode = next;
			}
			renderData.flags &= ~RenderDataFlags.HasExtraMeshes;
		}
	}

	public void InsertTexture(RenderData renderData, Texture src, TextureId id, bool isAtlas)
	{
		BasicNode<GraphicEntry> basicNode = m_GraphicEntryPool.Get();
		basicNode.data.source = src;
		basicNode.data.actual = id;
		basicNode.data.replaced = isAtlas;
		basicNode.InsertFirst(ref renderData.graphicEntries);
	}

	public void InsertVectorImage(RenderData renderData, VectorImage vi)
	{
		BasicNode<GraphicEntry> basicNode = m_GraphicEntryPool.Get();
		basicNode.data.vectorImage = vi;
		basicNode.InsertFirst(ref renderData.graphicEntries);
	}

	public void ResetGraphicEntries(RenderData renderData)
	{
		AtlasBase atlasBase = atlas;
		TextureRegistry textureRegistry = m_TextureRegistry;
		BasicNodePool<GraphicEntry> graphicEntryPool = m_GraphicEntryPool;
		BasicNode<GraphicEntry> basicNode = renderData.graphicEntries;
		renderData.graphicEntries = null;
		while (basicNode != null)
		{
			BasicNode<GraphicEntry> next = basicNode.next;
			if (basicNode.data.vectorImage != null)
			{
				vectorImageManager.RemoveUser(basicNode.data.vectorImage);
				basicNode.data.vectorImage = null;
			}
			else
			{
				if (basicNode.data.replaced)
				{
					atlasBase.ReturnAtlas(renderData.owner, basicNode.data.source as Texture2D, basicNode.data.actual);
				}
				else
				{
					textureRegistry.Release(basicNode.data.actual);
				}
				basicNode.data.source = null;
			}
			graphicEntryPool.Return(basicNode);
			basicNode = next;
		}
	}

	private void DrawStats()
	{
		bool flag = device != null;
		float num = 12f;
		Rect position = new Rect(30f, 60f, 1000f, 100f);
		GUI.Box(new Rect(20f, 40f, 200f, flag ? 380 : 256), "UI Toolkit Draw Stats");
		GUI.Label(position, "Elements added\t: " + m_Stats.elementsAdded);
		position.y += num;
		GUI.Label(position, "Elements removed\t: " + m_Stats.elementsRemoved);
		position.y += num;
		GUI.Label(position, "Mesh allocs allocated\t: " + m_Stats.newMeshAllocations);
		position.y += num;
		GUI.Label(position, "Mesh allocs updated\t: " + m_Stats.updatedMeshAllocations);
		position.y += num;
		GUI.Label(position, "Clip update roots\t: " + m_Stats.recursiveClipUpdates);
		position.y += num;
		GUI.Label(position, "Clip update total\t: " + m_Stats.recursiveClipUpdatesExpanded);
		position.y += num;
		GUI.Label(position, "Opacity update roots\t: " + m_Stats.recursiveOpacityUpdates);
		position.y += num;
		GUI.Label(position, "Opacity update total\t: " + m_Stats.recursiveOpacityUpdatesExpanded);
		position.y += num;
		GUI.Label(position, "Opacity ID update\t: " + m_Stats.opacityIdUpdates);
		position.y += num;
		GUI.Label(position, "Xform update roots\t: " + m_Stats.recursiveTransformUpdates);
		position.y += num;
		GUI.Label(position, "Xform update total\t: " + m_Stats.recursiveTransformUpdatesExpanded);
		position.y += num;
		GUI.Label(position, "Xformed by bone\t: " + m_Stats.boneTransformed);
		position.y += num;
		GUI.Label(position, "Xformed by skipping\t: " + m_Stats.skipTransformed);
		position.y += num;
		GUI.Label(position, "Xformed by nudging\t: " + m_Stats.nudgeTransformed);
		position.y += num;
		GUI.Label(position, "Xformed by repaint\t: " + m_Stats.visualUpdateTransformed);
		position.y += num;
		GUI.Label(position, "Visual update roots\t: " + m_Stats.recursiveVisualUpdates);
		position.y += num;
		GUI.Label(position, "Visual update total\t: " + m_Stats.recursiveVisualUpdatesExpanded);
		position.y += num;
		GUI.Label(position, "Visual update flats\t: " + m_Stats.nonRecursiveVisualUpdates);
		position.y += num;
		GUI.Label(position, "Dirty processed\t: " + m_Stats.dirtyProcessed);
		position.y += num;
		GUI.Label(position, "Group-xform updates\t: " + m_Stats.groupTransformElementsChanged);
		position.y += num;
		if (flag)
		{
			position.y += num;
			UIRenderDevice.DrawStatistics drawStatistics = device.GatherDrawStatistics();
			GUI.Label(position, "Frame index\t: " + drawStatistics.currentFrameIndex);
			position.y += num;
			GUI.Label(position, "Command count\t: " + drawStatistics.commandCount);
			position.y += num;
			GUI.Label(position, "Skip cmd counts\t: " + drawStatistics.skippedCommandCount);
			position.y += num;
			GUI.Label(position, "Draw commands\t: " + drawStatistics.drawCommandCount);
			position.y += num;
			GUI.Label(position, "Disable commands\t: " + drawStatistics.disableCommandCount);
			position.y += num;
			GUI.Label(position, "Draw ranges\t: " + drawStatistics.drawRangeCount);
			position.y += num;
			GUI.Label(position, "Draw range calls\t: " + drawStatistics.drawRangeCallCount);
			position.y += num;
			GUI.Label(position, "Material sets\t: " + drawStatistics.materialSetCount);
			position.y += num;
			GUI.Label(position, "Stencil changes\t: " + drawStatistics.stencilRefChanges);
			position.y += num;
			GUI.Label(position, "Immediate draws\t: " + drawStatistics.immediateDraws);
			position.y += num;
			GUI.Label(position, "Total triangles\t: " + drawStatistics.totalIndices / 3);
			position.y += num;
		}
	}
}
