using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class VisualTreeLayoutUpdater : BaseVisualTreeUpdater
{
	public const int kMaxValidateLayoutCount = 10;

	private static readonly string s_Description = "UIElements.UpdateLayout";

	private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(s_Description);

	private static readonly ProfilerMarker k_ComputeLayoutMarker = new ProfilerMarker("LayoutUpdater.ComputeLayout");

	private static readonly ProfilerMarker k_UpdateSubTreeMarker = new ProfilerMarker("LayoutUpdater.UpdateSubTree");

	private static readonly ProfilerMarker k_DispatchChangeEventsMarker = new ProfilerMarker("LayoutUpdater.DispatchChangeEvents");

	private List<(Rect, Rect, VisualElement)> changeEventsList = new List<(Rect, Rect, VisualElement)>();

	private List<VisualElement> missedHierarchyChangeEventsList = new List<VisualElement>();

	private TextJobSystem m_TextJobSystem = new TextJobSystem();

	public override ProfilerMarker profilerMarker => s_ProfilerMarker;

	public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
	{
		if ((versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Layout)) != 0)
		{
			LayoutNode layoutNode = ve.layoutNode;
			if (layoutNode != LayoutNode.Undefined && layoutNode.UsesMeasure)
			{
				layoutNode.MarkDirty();
			}
		}
	}

	public override void Update()
	{
		m_TextJobSystem.PrepareShapingBeforeLayout(base.panel);
		int num = 0;
		if (base.visualTree.layoutNode.IsDirty)
		{
			missedHierarchyChangeEventsList.Clear();
			while (base.visualTree.layoutNode.IsDirty)
			{
				changeEventsList.Clear();
				if (num > 0)
				{
					base.panel.ApplyStyles();
					m_TextJobSystem.PrepareShapingBeforeLayout(base.panel);
				}
				try
				{
					base.panel.duringLayoutPhase = true;
					base.visualTree.layoutNode.CalculateLayout();
				}
				finally
				{
					base.panel.duringLayoutPhase = false;
				}
				UpdateSubTree(base.visualTree, changeEventsList);
				DispatchChangeEvents(changeEventsList, num);
				if (!base.visualTree.layoutNode.IsDirty)
				{
					DispatchMissedHierarchyChangeEvents(missedHierarchyChangeEventsList, num);
					missedHierarchyChangeEventsList.Clear();
				}
				if (num++ >= 10)
				{
					Debug.LogError("Layout update is struggling to process current layout (consider simplifying to avoid recursive layout): " + base.visualTree);
					break;
				}
			}
		}
		base.visualTree.focusController.ReevaluateFocus();
	}

	private static bool UpdateHierarchyDisplayed(VisualElement ve, List<(Rect, Rect, VisualElement)> changeEvents, bool inheritedDisplayed = true)
	{
		bool flag = inheritedDisplayed & (ve.resolvedStyle.display != DisplayStyle.None);
		if (inheritedDisplayed && !flag)
		{
			ve.disableRendering = true;
		}
		else if (flag)
		{
			ve.disableRendering = false;
		}
		if (ve.areAncestorsAndSelfDisplayed == flag)
		{
			return false;
		}
		ve.areAncestorsAndSelfDisplayed = flag;
		if (!flag)
		{
			if (inheritedDisplayed)
			{
				ve.IncrementVersion(VersionChangeType.Size);
			}
			if (ve.HasSelfEventInterests(EventBase<GeometryChangedEvent>.EventCategory))
			{
				changeEvents.Add((ve.lastLayout, Rect.zero, ve));
			}
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UpdateHierarchyDisplayed(ve.hierarchy[i], changeEvents, flag);
			}
		}
		return true;
	}

	private void UpdateSubTree(VisualElement ve, List<(Rect, Rect, VisualElement)> changeEvents)
	{
		bool flag = UpdateHierarchyDisplayed(ve, changeEvents);
		if (!ve.areAncestorsAndSelfDisplayed)
		{
			return;
		}
		Rect rect = new Rect(ve.layoutNode.LayoutX, ve.layoutNode.LayoutY, ve.layoutNode.LayoutWidth, ve.layoutNode.LayoutHeight);
		Rect rect2 = new Rect(ve.layoutNode.LayoutPaddingLeft, ve.layoutNode.LayoutPaddingLeft, ve.layoutNode.LayoutPaddingRight, ve.layoutNode.LayoutPaddingBottom);
		Rect lastPseudoPadding = new Rect(rect2.x, rect2.y, rect.width - (rect2.x + rect2.width), rect.height - (rect2.y + rect2.height));
		Rect lastLayout = ve.lastLayout;
		Rect lastPseudoPadding2 = ve.lastPseudoPadding;
		VersionChangeType versionChangeType = (VersionChangeType)0;
		bool flag2 = lastLayout.size != rect.size;
		bool flag3 = lastPseudoPadding2.size != lastPseudoPadding.size;
		if (flag2 || flag3)
		{
			versionChangeType |= VersionChangeType.Size | VersionChangeType.Repaint;
		}
		bool flag4 = rect.position != lastLayout.position;
		bool flag5 = lastPseudoPadding.position != lastPseudoPadding2.position;
		if (flag4 || flag5 || flag)
		{
			versionChangeType |= VersionChangeType.Transform;
		}
		if (flag)
		{
			versionChangeType |= VersionChangeType.Size;
		}
		if ((versionChangeType & (VersionChangeType.Transform | VersionChangeType.Size)) == VersionChangeType.Size && !ve.hasDefaultRotationAndScale && (!Mathf.Approximately(ve.resolvedStyle.transformOrigin.x, 0f) || !Mathf.Approximately(ve.resolvedStyle.transformOrigin.y, 0f)))
		{
			versionChangeType |= VersionChangeType.Transform;
		}
		if (versionChangeType != 0)
		{
			ve.IncrementVersion(versionChangeType);
		}
		ve.lastLayout = rect;
		ve.lastPseudoPadding = lastPseudoPadding;
		bool hasNewLayout = ve.layoutNode.HasNewLayout;
		if (hasNewLayout)
		{
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.hierarchy[i];
				if (visualElement.layoutNode.HasNewLayout)
				{
					UpdateSubTree(visualElement, changeEvents);
				}
			}
		}
		if (ve.HasSelfEventInterests(EventBase<GeometryChangedEvent>.EventCategory))
		{
			if (flag2 || flag4 || flag)
			{
				changeEvents.Add((flag ? Rect.zero : lastLayout, rect, ve));
				if (ve.receivesHierarchyGeometryChangedEvents)
				{
					missedHierarchyChangeEventsList.Remove(ve);
				}
			}
			else if (ve.receivesHierarchyGeometryChangedEvents && ve.boundingBoxDirtiedSinceLastLayoutPass)
			{
				missedHierarchyChangeEventsList.Add(ve);
			}
		}
		ve.boundingBoxDirtiedSinceLastLayoutPass = false;
		if (hasNewLayout)
		{
			ve.layoutNode.MarkLayoutSeen();
		}
	}

	private void DispatchChangeEvents(List<(Rect, Rect, VisualElement)> changeEvents, int currentLayoutPass)
	{
		foreach (var (oldRect, newRect, target) in changeEvents)
		{
			using GeometryChangedEvent geometryChangedEvent = GeometryChangedEvent.GetPooled(oldRect, newRect);
			geometryChangedEvent.layoutPass = currentLayoutPass;
			EventDispatchUtilities.SendEventDirectlyToTarget(geometryChangedEvent, base.panel, target);
		}
	}

	private void DispatchMissedHierarchyChangeEvents(List<VisualElement> missedHierarchyChangeEvents, int currentLayoutPass)
	{
		foreach (VisualElement missedHierarchyChangeEvent in missedHierarchyChangeEvents)
		{
			using GeometryChangedEvent geometryChangedEvent = GeometryChangedEvent.GetPooled(Rect.zero, missedHierarchyChangeEvent.layout);
			geometryChangedEvent.layoutPass = currentLayoutPass;
			EventDispatchUtilities.SendEventDirectlyToTarget(geometryChangedEvent, base.panel, missedHierarchyChangeEvent);
		}
	}
}
