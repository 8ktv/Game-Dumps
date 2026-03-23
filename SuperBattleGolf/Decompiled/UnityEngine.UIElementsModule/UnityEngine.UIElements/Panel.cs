#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class Panel : BaseVisualElementPanel
{
	internal class UIFrameState
	{
		internal virtual long[] updatersFrameCount { get; }

		internal virtual long schedulerFrameCount { get; }

		internal virtual bool isPanelDirty { get; }

		internal virtual ContextType panelContextType { get; }

		internal UIFrameState()
		{
		}

		internal UIFrameState(Panel panel)
		{
			isPanelDirty = panel.isDirty;
			panelContextType = panel.contextType;
			schedulerFrameCount = panel.scheduler.FrameCount;
			updatersFrameCount = panel.visualTreeUpdater.GetUpdatersFrameCount();
		}

		public static bool operator >(UIFrameState leftOperand, UIFrameState rightOperand)
		{
			return leftOperand.HasFullUIFrameOccurredSince(rightOperand);
		}

		public static bool operator <(UIFrameState leftOperand, UIFrameState rightOperand)
		{
			return rightOperand.HasFullUIFrameOccurredSince(leftOperand);
		}

		private bool HasFullUIFrameOccurredSince(UIFrameState reference)
		{
			if (panelContextType != reference.panelContextType)
			{
				throw new NotSupportedException("Comparison is only valid for frames with the same ContextType.");
			}
			if (schedulerFrameCount <= reference.schedulerFrameCount)
			{
				return false;
			}
			if (isPanelDirty || panelContextType != ContextType.Editor)
			{
				for (int i = 0; i < updatersFrameCount.Length; i++)
				{
					if (updatersFrameCount[i] <= reference.updatersFrameCount[i])
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	internal const int k_DefaultPixelsPerUnit = 100;

	private VisualElement m_RootContainer;

	private VisualTreeUpdater m_VisualTreeUpdater;

	private IStylePropertyAnimationSystem m_StylePropertyAnimationSystem;

	private string m_PanelName;

	private uint m_Version = 0u;

	private uint m_RepaintVersion = 0u;

	private uint m_HierarchyVersion = 0u;

	private uint m_LastTickedHierarchyVersion = 0u;

	private ProfilerMarker m_MarkerPrepareRepaint;

	private ProfilerMarker m_MarkerRender;

	private ProfilerMarker m_MarkerValidateLayout;

	private ProfilerMarker m_MarkerTickScheduledActions;

	protected ProfilerMarker m_MarkerTickScheduledActionsPreLayout;

	protected ProfilerMarker m_MarkerTickScheduledActionsPostLayout;

	private ProfilerMarker m_MarkerPanelChangeReceiver;

	private static ProfilerMarker s_MarkerPickAll = new ProfilerMarker("UIElements.PickAll");

	private TimerEventScheduler m_Scheduler;

	private bool m_JustReceivedFocus;

	private IDebugPanelChangeReceiver m_PanelChangeReceiver;

	private AtlasBase m_Atlas;

	private bool m_ValidatingLayout = false;

	public sealed override VisualElement visualTree => m_RootContainer;

	public sealed override EventDispatcher dispatcher { get; set; }

	public TimerEventScheduler timerEventScheduler => m_Scheduler ?? (m_Scheduler = new TimerEventScheduler());

	internal override IScheduler scheduler => timerEventScheduler;

	internal VisualTreeUpdater visualTreeUpdater => m_VisualTreeUpdater;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal override IStylePropertyAnimationSystem styleAnimationSystem
	{
		get
		{
			return m_StylePropertyAnimationSystem;
		}
		set
		{
			if (m_StylePropertyAnimationSystem == value)
			{
				return;
			}
			try
			{
				m_StylePropertyAnimationSystem?.CancelAllAnimations();
			}
			finally
			{
				m_StylePropertyAnimationSystem = value;
			}
		}
	}

	public override ScriptableObject ownerObject { get; protected set; }

	public override ContextType contextType { get; }

	public override SavePersistentViewData saveViewData { get; set; }

	public override GetViewDataDictionary getViewDataDictionary { get; set; }

	public sealed override FocusController focusController { get; set; }

	public override EventInterests IMGUIEventInterests { get; set; }

	internal static LoadResourceFunction loadResourceFunc { private get; set; }

	internal string name
	{
		get
		{
			return m_PanelName;
		}
		set
		{
			m_PanelName = value;
			CreateMarkers();
		}
	}

	public IDebugPanelChangeReceiver panelChangeReceiver
	{
		get
		{
			return m_PanelChangeReceiver;
		}
		set
		{
			m_PanelChangeReceiver = value;
			if (value != null)
			{
				Debug.LogWarning("IPanelChangeReceiver suscribed to panel '" + name + "' and may affect performance. The callback should be used only in debugging scenario and won't work outside development builds");
			}
		}
	}

	internal static TimeMsFunction TimeSinceStartup { get; set; }

	public override int IMGUIContainersCount { get; set; }

	public override IMGUIContainer rootIMGUIContainer { get; set; }

	internal override uint version => m_Version;

	internal override uint repaintVersion => m_RepaintVersion;

	internal override uint hierarchyVersion => m_HierarchyVersion;

	public override AtlasBase atlas
	{
		get
		{
			return m_Atlas;
		}
		set
		{
			if (m_Atlas != value)
			{
				m_Atlas?.InvokeRemovedFromPanel(this);
				m_Atlas = value;
				InvokeAtlasChanged();
				m_Atlas?.InvokeAssignedToPanel(this);
			}
		}
	}

	internal virtual Color HyperlinkColor => Color.blue;

	internal static event Action<Panel> beforeAnyRepaint;

	internal UIFrameState GetFrameState()
	{
		return new UIFrameState(this);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static Object LoadResource(string pathName, Type type, float dpiScaling)
	{
		Object obj = null;
		if (loadResourceFunc != null)
		{
			return loadResourceFunc(pathName, type, dpiScaling);
		}
		return Resources.Load(pathName, type);
	}

	internal void Focus()
	{
		m_JustReceivedFocus = true;
	}

	internal void Blur()
	{
		focusController?.BlurLastFocusedElement();
	}

	public void ValidateFocus()
	{
		if (m_JustReceivedFocus)
		{
			m_JustReceivedFocus = false;
			focusController?.SetFocusToLastFocusedElement();
		}
	}

	private void CreateMarkers()
	{
		string text = (string.IsNullOrEmpty(m_PanelName) ? "Panel" : m_PanelName);
		m_MarkerPrepareRepaint = new ProfilerMarker(text + ".PrepareRepaint");
		m_MarkerRender = new ProfilerMarker(text + ".Render");
		m_MarkerValidateLayout = new ProfilerMarker(text + ".ValidateLayout");
		m_MarkerTickScheduledActions = new ProfilerMarker(text + ".TickScheduledActions");
		m_MarkerTickScheduledActionsPreLayout = new ProfilerMarker(text + ".TickScheduledActionsPreLayout");
		m_MarkerTickScheduledActionsPostLayout = new ProfilerMarker(text + ".TickScheduledActionsPostLayout");
		m_MarkerPanelChangeReceiver = new ProfilerMarker(text + ".ExecutePanelChangeReceiverCallback");
	}

	public Panel(ScriptableObject ownerObject, ContextType contextType, EventDispatcher dispatcher)
	{
		Debug.Assert(contextType == ContextType.Player, "In a player, panel context type must be set to Player.");
		contextType = ContextType.Player;
		this.ownerObject = ownerObject;
		this.contextType = contextType;
		this.dispatcher = dispatcher;
		repaintData = new RepaintData();
		cursorManager = new CursorManager();
		base.contextualMenuManager = null;
		dataBindingManager = new DataBindingManager(this);
		m_VisualTreeUpdater = new VisualTreeUpdater(this);
		SetSpecializedHierarchyFlagsUpdater();
		m_RootContainer = ((contextType == ContextType.Editor) ? new EditorPanelRootElement() : new PanelRootElement());
		visualTree.SetPanel(this);
		focusController = new FocusController(new VisualElementFocusRing(visualTree));
		styleAnimationSystem = new StylePropertyAnimationSystem();
		CreateMarkers();
		InvokeHierarchyChanged(visualTree, HierarchyChangeType.AddedToParent);
		atlas = new DynamicAtlas();
	}

	protected override void Dispose(bool disposing)
	{
		if (!base.disposed)
		{
			if (disposing)
			{
				atlas = null;
				visualTree.Clear();
				m_VisualTreeUpdater.Dispose();
			}
			base.Dispose(disposing);
		}
	}

	public static long TimeSinceStartupMs()
	{
		return TimeSinceStartup?.Invoke() ?? DefaultTimeSinceStartupMs();
	}

	internal static long DefaultTimeSinceStartupMs()
	{
		return (long)(Time.realtimeSinceStartup * 1000f);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "Assembly-CSharp-testable" })]
	internal static VisualElement PickAllWithoutValidatingLayout(VisualElement root, Vector2 point)
	{
		return PickAll(root, point);
	}

	internal static VisualElement PickAll(VisualElement root, Vector2 point, List<VisualElement> picked = null, bool includeIgnoredElement = false)
	{
		return PerformPick(root, point, picked, includeIgnoredElement);
	}

	private static VisualElement PerformPick(VisualElement root, Vector2 point, List<VisualElement> picked = null, bool includeIgnoredElement = false)
	{
		if (root.resolvedStyle.display == DisplayStyle.None)
		{
			return null;
		}
		if (root.pickingMode == PickingMode.Ignore && root.hierarchy.childCount == 0 && !includeIgnoredElement)
		{
			return null;
		}
		if (!root.worldBoundingBox.Contains(point))
		{
			return null;
		}
		Vector2 localPoint = root.WorldToLocal(point);
		bool flag = root.ContainsPoint(localPoint);
		if (!flag && root.ShouldClip())
		{
			return null;
		}
		VisualElement visualElement = null;
		int childCount = root.hierarchy.childCount;
		for (int num = childCount - 1; num >= 0; num--)
		{
			VisualElement root2 = root.hierarchy[num];
			VisualElement visualElement2 = PerformPick(root2, point, picked, includeIgnoredElement);
			if (visualElement == null && visualElement2 != null)
			{
				if (picked == null)
				{
					return visualElement2;
				}
				visualElement = visualElement2;
			}
		}
		if (root.visible && (root.pickingMode == PickingMode.Position || includeIgnoredElement) && flag)
		{
			picked?.Add(root);
			if (visualElement == null)
			{
				visualElement = root;
			}
		}
		return visualElement;
	}

	public override VisualElement PickAll(Vector2 point, List<VisualElement> picked)
	{
		ValidateLayout();
		picked?.Clear();
		return PickAll(visualTree, point, picked);
	}

	public override VisualElement Pick(Vector2 point, int pointerId)
	{
		ValidateLayout();
		Vector2 pickPosition;
		bool isTemporary;
		VisualElement topElementUnderPointer = m_TopElementUnderPointers.GetTopElementUnderPointer(pointerId, out pickPosition, out isTemporary);
		if (!isTemporary && PixelOf(pickPosition) == PixelOf(point))
		{
			return topElementUnderPointer;
		}
		return PickAll(visualTree, point);
		static Vector2Int PixelOf(Vector2 p)
		{
			return Vector2Int.FloorToInt(p);
		}
	}

	public override void ValidateLayout()
	{
		using (new IMGUIContainer.UITKScope())
		{
			if (!m_ValidatingLayout)
			{
				m_ValidatingLayout = true;
				m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
				m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Layout);
				m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.TransformClip);
				m_ValidatingLayout = false;
			}
		}
	}

	public override void UpdateAnimations()
	{
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Animation);
	}

	public override void UpdateBindings()
	{
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Bindings);
	}

	public override void UpdateDataBinding()
	{
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.DataBinding);
	}

	public override void TickSchedulingUpdaters()
	{
		using (new IMGUIContainer.UITKScope())
		{
			using (m_MarkerTickScheduledActions.Auto())
			{
				timerEventScheduler.UpdateScheduledEvents();
				ValidateFocus();
				UpdateBindings();
				UpdateDataBinding();
				UpdateAnimations();
				m_LastTickedHierarchyVersion = m_HierarchyVersion;
			}
		}
	}

	public override void ApplyStyles()
	{
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
	}

	public override void UpdateForRepaint()
	{
		if (m_LastTickedHierarchyVersion != m_HierarchyVersion)
		{
			TickSchedulingUpdaters();
		}
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.DataBinding);
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Layout);
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.TransformClip);
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Repaint);
	}

	internal void UpdateWithoutRepaint()
	{
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Bindings);
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.DataBinding);
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
		m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Layout);
	}

	public override void Repaint(Event e)
	{
		using (new IMGUIContainer.UITKScope())
		{
			m_RepaintVersion = version;
			repaintData.repaintEvent = e;
			InvokeBeforeUpdate();
			Panel.beforeAnyRepaint?.Invoke(this);
			using (m_MarkerPrepareRepaint.Auto())
			{
				UpdateForRepaint();
			}
		}
	}

	public override void Render()
	{
		using (new IMGUIContainer.UITKScope())
		{
			base.Render();
		}
	}

	internal override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
	{
		m_Version++;
		m_VisualTreeUpdater.OnVersionChanged(ve, versionChangeType);
		if (panelChangeReceiver != null)
		{
			using (m_MarkerPanelChangeReceiver.Auto())
			{
				panelChangeReceiver.OnVisualElementChange(ve, versionChangeType);
			}
		}
		if ((versionChangeType & VersionChangeType.Hierarchy) == VersionChangeType.Hierarchy)
		{
			m_HierarchyVersion++;
		}
	}

	internal override void SetUpdater(IVisualTreeUpdater updater, VisualTreeUpdatePhase phase)
	{
		m_VisualTreeUpdater.SetUpdater(updater, phase);
	}

	internal override IVisualTreeUpdater GetUpdater(VisualTreeUpdatePhase phase)
	{
		return m_VisualTreeUpdater.GetUpdater(phase);
	}
}
