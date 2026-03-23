#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR;

internal class RenderTree
{
	[Flags]
	internal enum AllowedClasses
	{
		Clipping = 1,
		Opacity = 2,
		Color = 4,
		TransformSize = 8,
		Visuals = 0x10,
		All = 0x1F
	}

	private RenderTreeManager m_RenderTreeManager;

	private DepthOrderedDirtyTracking m_DirtyTracker;

	private RenderChainCommand m_FirstCommand;

	private RenderData m_RootRenderData;

	public TextureId quadTextureId;

	public RectInt quadRect;

	internal RenderTree parent;

	internal RenderTree firstChild;

	internal RenderTree nextSibling;

	private static readonly ProfilerMarker k_MarkerClipProcessing = new ProfilerMarker("RenderTree.UpdateClips");

	private static readonly ProfilerMarker k_MarkerOpacityProcessing = new ProfilerMarker("RenderTree.UpdateOpacity");

	private static readonly ProfilerMarker k_MarkerColorsProcessing = new ProfilerMarker("RenderTree.UpdateColors");

	private static readonly ProfilerMarker k_MarkerTransformProcessing = new ProfilerMarker("RenderTree.UpdateTransforms");

	private static readonly ProfilerMarker k_MarkerVisualsProcessing = new ProfilerMarker("RenderTree.UpdateVisuals");

	private AllowedClasses m_AllowedDirtyClasses = AllowedClasses.All;

	internal RenderTreeManager renderTreeManager => m_RenderTreeManager;

	internal RenderData rootRenderData => m_RootRenderData;

	internal ref DepthOrderedDirtyTracking dirtyTracker => ref m_DirtyTracker;

	internal RenderChainCommand firstCommand => m_FirstCommand;

	internal bool isRootRenderTree
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return rootRenderData.owner.parent == null && !rootRenderData.isNestedRenderTreeRoot;
		}
	}

	public void Init(RenderTreeManager renderTreeManager, RenderData rootRenderData)
	{
		m_RenderTreeManager = renderTreeManager;
		m_RootRenderData = rootRenderData;
		m_DirtyTracker.owner = this;
		quadTextureId = TextureId.invalid;
		parent = null;
		firstChild = null;
		nextSibling = null;
		m_DirtyTracker.heads = new List<RenderData>(8);
		m_DirtyTracker.tails = new List<RenderData>(8);
		m_DirtyTracker.minDepths = new int[5];
		m_DirtyTracker.maxDepths = new int[5];
		m_DirtyTracker.Reset();
	}

	public void Reset()
	{
		m_RenderTreeManager = null;
		m_RootRenderData = null;
		parent = null;
		firstChild = null;
		nextSibling = null;
	}

	public void Dispose()
	{
		if (m_RootRenderData != null)
		{
			DepthFirstResetTextures(m_RootRenderData);
		}
	}

	private void DepthFirstResetTextures(RenderData renderData)
	{
		m_RenderTreeManager.ResetTextures(renderData);
		for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
		{
			DepthFirstResetTextures(renderData2);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void OnRenderDataClippingChanged(RenderData renderData, bool hierarchical)
	{
		Debug.Assert((m_AllowedDirtyClasses & AllowedClasses.Clipping) != 0);
		m_DirtyTracker.RegisterDirty(renderData, (RenderDataDirtyTypes)(4 | (hierarchical ? 8 : 0)), RenderDataDirtyTypeClasses.Clipping);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void OnRenderDataOpacityChanged(RenderData renderData, bool hierarchical = false)
	{
		Debug.Assert((m_AllowedDirtyClasses & AllowedClasses.Opacity) != 0);
		m_DirtyTracker.RegisterDirty(renderData, (RenderDataDirtyTypes)(0x80 | (hierarchical ? 256 : 0)), RenderDataDirtyTypeClasses.Opacity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void OnRenderDataColorChanged(RenderData renderData)
	{
		Debug.Assert((m_AllowedDirtyClasses & AllowedClasses.Color) != 0);
		m_DirtyTracker.RegisterDirty(renderData, RenderDataDirtyTypes.Color, RenderDataDirtyTypeClasses.Color);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void OnRenderDataTransformOrSizeChanged(RenderData renderData, bool transformChanged, bool clipRectSizeChanged)
	{
		Debug.Assert((m_AllowedDirtyClasses & AllowedClasses.TransformSize) != 0);
		RenderDataDirtyTypes dirtyTypes = (RenderDataDirtyTypes)((transformChanged ? 1 : 0) | (clipRectSizeChanged ? 2 : 0));
		m_DirtyTracker.RegisterDirty(renderData, dirtyTypes, RenderDataDirtyTypeClasses.TransformSize);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void OnRenderDataOpacityIdChanged(RenderData renderData)
	{
		Debug.Assert((m_AllowedDirtyClasses & AllowedClasses.Visuals) != 0);
		m_DirtyTracker.RegisterDirty(renderData, RenderDataDirtyTypes.VisualsOpacityId, RenderDataDirtyTypeClasses.Visuals);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void OnRenderDataVisualsChanged(RenderData renderData, bool hierarchical)
	{
		Debug.Assert((m_AllowedDirtyClasses & AllowedClasses.Visuals) != 0);
		m_DirtyTracker.RegisterDirty(renderData, (RenderDataDirtyTypes)(0x10 | (hierarchical ? 32 : 0)), RenderDataDirtyTypeClasses.Visuals);
	}

	public void ProcessChanges(ref ChainBuilderStats stats)
	{
		m_DirtyTracker.dirtyID++;
		int num = 0;
		RenderDataDirtyTypes renderDataDirtyTypes = RenderDataDirtyTypes.Clipping | RenderDataDirtyTypes.ClippingHierarchy;
		RenderDataDirtyTypes dirtyTypesInverse = ~renderDataDirtyTypes;
		m_AllowedDirtyClasses &= ~AllowedClasses.Clipping;
		for (int i = m_DirtyTracker.minDepths[num]; i <= m_DirtyTracker.maxDepths[num]; i++)
		{
			RenderData renderData = m_DirtyTracker.heads[i];
			while (renderData != null)
			{
				RenderData nextDirty = renderData.nextDirty;
				if ((renderData.dirtiedValues & renderDataDirtyTypes) != RenderDataDirtyTypes.None)
				{
					if (renderData.dirtyID != m_DirtyTracker.dirtyID)
					{
						RenderEvents.ProcessOnClippingChanged(m_RenderTreeManager, renderData, m_DirtyTracker.dirtyID, ref stats);
					}
					m_DirtyTracker.ClearDirty(renderData, dirtyTypesInverse);
				}
				renderData = nextDirty;
				stats.dirtyProcessed++;
			}
		}
		m_DirtyTracker.dirtyID++;
		num = 1;
		renderDataDirtyTypes = RenderDataDirtyTypes.Opacity | RenderDataDirtyTypes.OpacityHierarchy;
		dirtyTypesInverse = ~renderDataDirtyTypes;
		m_AllowedDirtyClasses &= ~AllowedClasses.Opacity;
		for (int j = m_DirtyTracker.minDepths[num]; j <= m_DirtyTracker.maxDepths[num]; j++)
		{
			RenderData renderData2 = m_DirtyTracker.heads[j];
			while (renderData2 != null)
			{
				RenderData nextDirty2 = renderData2.nextDirty;
				if ((renderData2.dirtiedValues & renderDataDirtyTypes) != RenderDataDirtyTypes.None)
				{
					if (renderData2.dirtyID != m_DirtyTracker.dirtyID)
					{
						RenderEvents.ProcessOnOpacityChanged(m_RenderTreeManager, renderData2, m_DirtyTracker.dirtyID, ref stats);
					}
					m_DirtyTracker.ClearDirty(renderData2, dirtyTypesInverse);
				}
				renderData2 = nextDirty2;
				stats.dirtyProcessed++;
			}
		}
		m_DirtyTracker.dirtyID++;
		num = 2;
		renderDataDirtyTypes = RenderDataDirtyTypes.Color;
		dirtyTypesInverse = ~renderDataDirtyTypes;
		m_AllowedDirtyClasses &= ~AllowedClasses.Color;
		for (int k = m_DirtyTracker.minDepths[num]; k <= m_DirtyTracker.maxDepths[num]; k++)
		{
			RenderData renderData3 = m_DirtyTracker.heads[k];
			while (renderData3 != null)
			{
				RenderData nextDirty3 = renderData3.nextDirty;
				if ((renderData3.dirtiedValues & renderDataDirtyTypes) != RenderDataDirtyTypes.None)
				{
					if (renderData3 != null && renderData3.dirtyID != m_DirtyTracker.dirtyID)
					{
						RenderEvents.ProcessOnColorChanged(m_RenderTreeManager, renderData3, m_DirtyTracker.dirtyID, ref stats);
					}
					m_DirtyTracker.ClearDirty(renderData3, dirtyTypesInverse);
				}
				renderData3 = nextDirty3;
				stats.dirtyProcessed++;
			}
		}
		m_DirtyTracker.dirtyID++;
		num = 3;
		renderDataDirtyTypes = RenderDataDirtyTypes.Transform | RenderDataDirtyTypes.ClipRectSize;
		dirtyTypesInverse = ~renderDataDirtyTypes;
		m_AllowedDirtyClasses &= ~AllowedClasses.TransformSize;
		for (int l = m_DirtyTracker.minDepths[num]; l <= m_DirtyTracker.maxDepths[num]; l++)
		{
			RenderData renderData4 = m_DirtyTracker.heads[l];
			while (renderData4 != null)
			{
				RenderData nextDirty4 = renderData4.nextDirty;
				if ((renderData4.dirtiedValues & renderDataDirtyTypes) != RenderDataDirtyTypes.None)
				{
					if (renderData4.dirtyID != m_DirtyTracker.dirtyID)
					{
						RenderEvents.ProcessOnTransformOrSizeChanged(m_RenderTreeManager, renderData4, m_DirtyTracker.dirtyID, ref stats);
					}
					m_DirtyTracker.ClearDirty(renderData4, dirtyTypesInverse);
				}
				renderData4 = nextDirty4;
				stats.dirtyProcessed++;
			}
		}
		m_RenderTreeManager.jobManager.CompleteNudgeJobs();
		m_DirtyTracker.dirtyID++;
		num = 4;
		renderDataDirtyTypes = RenderDataDirtyTypes.AllVisuals;
		dirtyTypesInverse = ~renderDataDirtyTypes;
		m_AllowedDirtyClasses &= ~AllowedClasses.Visuals;
		for (int m = m_DirtyTracker.minDepths[num]; m <= m_DirtyTracker.maxDepths[num]; m++)
		{
			RenderData renderData5 = m_DirtyTracker.heads[m];
			while (renderData5 != null)
			{
				RenderData nextDirty5 = renderData5.nextDirty;
				if ((renderData5.dirtiedValues & renderDataDirtyTypes) != RenderDataDirtyTypes.None)
				{
					if (renderData5.dirtyID != m_DirtyTracker.dirtyID)
					{
						m_RenderTreeManager.visualChangesProcessor.ProcessOnVisualsChanged(renderData5, m_DirtyTracker.dirtyID, ref stats);
					}
					m_DirtyTracker.ClearDirty(renderData5, dirtyTypesInverse);
				}
				renderData5 = nextDirty5;
				stats.dirtyProcessed++;
			}
		}
		m_RenderTreeManager.meshGenerationDeferrer.ProcessDeferredWork(m_RenderTreeManager.visualChangesProcessor.meshGenerationContext);
		m_RenderTreeManager.visualChangesProcessor.ScheduleMeshGenerationJobs();
		m_RenderTreeManager.meshGenerationDeferrer.ProcessDeferredWork(m_RenderTreeManager.visualChangesProcessor.meshGenerationContext);
		m_RenderTreeManager.visualChangesProcessor.ConvertEntriesToCommands(ref stats);
		m_RenderTreeManager.jobManager.CompleteConvertMeshJobs();
		m_RenderTreeManager.jobManager.CompleteCopyMeshJobs();
		m_RenderTreeManager.opacityIdAccelerator.CompleteJobs();
		m_DirtyTracker.Reset();
		m_AllowedDirtyClasses = AllowedClasses.All;
	}

	internal void OnRenderCommandAdded(RenderChainCommand command)
	{
		if (command.prev == null)
		{
			m_FirstCommand = command;
		}
	}

	internal void OnRenderCommandsRemoved(RenderChainCommand firstCommand, RenderChainCommand lastCommand)
	{
		if (firstCommand.prev == null)
		{
			m_FirstCommand = lastCommand.next;
		}
	}

	internal void ChildWillBeRemoved(RenderData renderData)
	{
		if (renderData.dirtiedValues != RenderDataDirtyTypes.None)
		{
			m_DirtyTracker.ClearDirty(renderData, ~renderData.dirtiedValues);
		}
		Debug.Assert(renderData.dirtiedValues == RenderDataDirtyTypes.None);
		Debug.Assert(renderData.prevDirty == null);
		Debug.Assert(renderData.nextDirty == null);
	}
}
