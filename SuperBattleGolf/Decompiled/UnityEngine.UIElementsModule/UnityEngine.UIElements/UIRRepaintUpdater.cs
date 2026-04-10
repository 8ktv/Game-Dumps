#define UNITY_ASSERTIONS
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

internal class UIRRepaintUpdater : BaseVisualTreeUpdater, IPanelRenderer
{
	private BaseVisualElementPanel attachedPanel;

	internal RenderTreeManager renderTreeManager;

	private static readonly string s_Description;

	private static readonly ProfilerMarker s_ProfilerMarker;

	private bool m_ForceGammaRendering;

	private uint m_VertexBudget;

	private TextureSlotCount m_TextureSlotCount = TextureSlotCount.Eight;

	public override ProfilerMarker profilerMarker => s_ProfilerMarker;

	public bool forceGammaRendering
	{
		get
		{
			return m_ForceGammaRendering;
		}
		set
		{
			if (m_ForceGammaRendering != value)
			{
				m_ForceGammaRendering = value;
				DestroyRenderChain();
			}
		}
	}

	public uint vertexBudget
	{
		get
		{
			return m_VertexBudget;
		}
		set
		{
			if (m_VertexBudget != value)
			{
				m_VertexBudget = value;
				DestroyRenderChain();
			}
		}
	}

	public TextureSlotCount textureSlotCount
	{
		get
		{
			return m_TextureSlotCount;
		}
		set
		{
			m_TextureSlotCount = value;
		}
	}

	public bool drawStats { get; set; }

	public bool breakBatches { get; set; }

	protected bool disposed { get; private set; }

	public UIRRepaintUpdater()
	{
		base.panelChanged += OnPanelChanged;
	}

	public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
	{
		if (renderTreeManager != null)
		{
			bool flag = (versionChangeType & VersionChangeType.Transform) != 0;
			bool flag2 = (versionChangeType & VersionChangeType.Size) != 0;
			bool flag3 = (versionChangeType & VersionChangeType.Overflow) != 0;
			bool flag4 = (versionChangeType & VersionChangeType.BorderRadius) != 0;
			bool flag5 = (versionChangeType & VersionChangeType.BorderWidth) != 0;
			bool flag6 = (versionChangeType & VersionChangeType.RenderHints) != 0;
			bool flag7 = (versionChangeType & VersionChangeType.DisableRendering) != 0;
			bool flag8 = (versionChangeType & VersionChangeType.Repaint) != 0;
			bool flag9 = false;
			if (ve.renderData != null)
			{
				flag9 = (ve.useRenderTexture && (ve.renderData.flags & RenderDataFlags.IsSubTreeQuad) == 0) || (!ve.useRenderTexture && (ve.renderData.flags & RenderDataFlags.IsSubTreeQuad) != 0);
			}
			if (flag6 || flag9)
			{
				renderTreeManager.UIEOnRenderHintsChanged(ve);
			}
			if (flag || flag2 || flag5)
			{
				renderTreeManager.UIEOnTransformOrSizeChanged(ve, flag, flag2 || flag5);
			}
			if (flag3 || flag4)
			{
				renderTreeManager.UIEOnClippingChanged(ve, hierarchical: false);
			}
			if ((versionChangeType & VersionChangeType.Opacity) != 0)
			{
				renderTreeManager.UIEOnOpacityChanged(ve);
			}
			if ((versionChangeType & VersionChangeType.Color) != 0)
			{
				renderTreeManager.UIEOnColorChanged(ve);
			}
			if (flag8)
			{
				renderTreeManager.UIEOnVisualsChanged(ve, hierarchical: false);
			}
			if (flag7 && !flag8)
			{
				renderTreeManager.UIEOnDisableRenderingChanged(ve);
			}
		}
	}

	public override void Update()
	{
		if (renderTreeManager == null)
		{
			InitRenderChain();
		}
		if (renderTreeManager != null && renderTreeManager.device != null)
		{
			renderTreeManager.ProcessChanges();
			renderTreeManager.drawStats = drawStats;
			renderTreeManager.device.breakBatches = breakBatches;
			renderTreeManager.textureSlotCount = textureSlotCount;
		}
	}

	public void Render()
	{
		if (renderTreeManager != null)
		{
			Debug.Assert(!renderTreeManager.drawInCameras);
			renderTreeManager.RenderRootTree();
		}
	}

	protected virtual RenderTreeManager CreateRenderChain()
	{
		return new RenderTreeManager(base.panel);
	}

	static UIRRepaintUpdater()
	{
		s_Description = "UIElements.UpdateRenderData";
		s_ProfilerMarker = new ProfilerMarker(s_Description);
		Utility.GraphicsResourcesRecreate += OnGraphicsResourcesRecreate;
	}

	private static void OnGraphicsResourcesRecreate(bool recreate)
	{
		if (!recreate)
		{
			UIRenderDevice.PrepareForGfxDeviceRecreate();
		}
		Dictionary<int, Panel>.Enumerator panelsIterator = UIElementsUtility.GetPanelsIterator();
		while (panelsIterator.MoveNext())
		{
			if (recreate)
			{
				panelsIterator.Current.Value.atlas?.Reset();
			}
			else
			{
				panelsIterator.Current.Value.panelRenderer.Reset();
			}
		}
		if (!recreate)
		{
			UIRenderDevice.FlushAllPendingDeviceDisposes();
		}
		else
		{
			UIRenderDevice.WrapUpGfxDeviceRecreate();
		}
	}

	private void OnPanelChanged(BaseVisualElementPanel obj)
	{
		DetachFromPanel();
		AttachToPanel();
	}

	private void AttachToPanel()
	{
		Debug.Assert(attachedPanel == null);
		if (base.panel != null)
		{
			attachedPanel = base.panel;
			attachedPanel.isFlatChanged += OnPanelIsFlatChanged;
			attachedPanel.atlasChanged += OnPanelAtlasChanged;
			attachedPanel.hierarchyChanged += OnPanelHierarchyChanged;
			Debug.Assert(attachedPanel.panelRenderer == null);
			attachedPanel.panelRenderer = this;
			if (base.panel is BaseRuntimePanel baseRuntimePanel)
			{
				baseRuntimePanel.drawsInCamerasChanged += OnPanelDrawsInCamerasChanged;
			}
		}
	}

	private void DetachFromPanel()
	{
		if (attachedPanel != null)
		{
			DestroyRenderChain();
			if (base.panel is BaseRuntimePanel baseRuntimePanel)
			{
				baseRuntimePanel.drawsInCamerasChanged -= OnPanelDrawsInCamerasChanged;
			}
			attachedPanel.isFlatChanged -= OnPanelIsFlatChanged;
			attachedPanel.atlasChanged -= OnPanelAtlasChanged;
			attachedPanel.hierarchyChanged -= OnPanelHierarchyChanged;
			Debug.Assert(attachedPanel.panelRenderer == this);
			attachedPanel.panelRenderer = null;
			attachedPanel = null;
		}
	}

	private void InitRenderChain()
	{
		Debug.Assert(attachedPanel != null);
		renderTreeManager = CreateRenderChain();
		renderTreeManager.UIEOnChildAdded(attachedPanel.visualTree);
	}

	public void Reset()
	{
		DestroyRenderChain();
	}

	private void DestroyRenderChain()
	{
		if (renderTreeManager != null)
		{
			renderTreeManager.Dispose();
			renderTreeManager = null;
			ResetAllElementsDataRecursive(attachedPanel.visualTree);
		}
	}

	private void OnPanelIsFlatChanged()
	{
		DestroyRenderChain();
	}

	private void OnPanelAtlasChanged()
	{
		DestroyRenderChain();
	}

	private void OnPanelDrawsInCamerasChanged()
	{
		DestroyRenderChain();
	}

	private void OnPanelHierarchyChanged(VisualElement ve, HierarchyChangeType changeType, IReadOnlyList<VisualElement> additionalContext = null)
	{
		if (renderTreeManager != null)
		{
			switch (changeType)
			{
			case HierarchyChangeType.AddedToParent:
				renderTreeManager.UIEOnChildAdded(ve);
				break;
			case HierarchyChangeType.RemovedFromParent:
				renderTreeManager.UIEOnChildRemoving(ve);
				break;
			case HierarchyChangeType.ChildrenReordered:
				renderTreeManager.UIEOnChildrenReordered(ve);
				break;
			}
		}
	}

	private void ResetAllElementsDataRecursive(VisualElement ve)
	{
		ve.renderData = null;
		ve.nestedRenderData = null;
		int num = ve.hierarchy.childCount - 1;
		while (num >= 0)
		{
			ResetAllElementsDataRecursive(ve.hierarchy[num--]);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				DetachFromPanel();
			}
			disposed = true;
		}
	}
}
