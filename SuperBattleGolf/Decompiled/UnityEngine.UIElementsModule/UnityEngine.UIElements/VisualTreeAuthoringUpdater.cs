using System.Collections.Generic;
using Unity.Profiling;

namespace UnityEngine.UIElements;

internal sealed class VisualTreeAuthoringUpdater : BaseVisualTreeUpdater
{
	internal struct StateSnapshot
	{
		public int processorsCount;

		public bool containsAccumulatedChanges;

		public bool isProcessingChanges;
	}

	private const VersionChangeType k_StyleChangedFlags = VersionChangeType.Layout | VersionChangeType.Styles;

	private const VersionChangeType k_StylingContextChangedFlags = VersionChangeType.StyleSheet;

	private const VersionChangeType k_BindingsChangedFlags = VersionChangeType.Bindings | VersionChangeType.BindingRegistration | VersionChangeType.DataSource;

	private static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("Update Authoring");

	private static readonly ProfilerMarker s_UpdateChangeProfilerMarker = new ProfilerMarker("Update Authoring - Change");

	private readonly List<IVisualElementChangeProcessor> m_RegisteredProcessors = new List<IVisualElementChangeProcessor>();

	private readonly List<IVisualElementChangeProcessor> m_ProcessorRegistrationList = new List<IVisualElementChangeProcessor>();

	private readonly List<IVisualElementChangeProcessor> m_ProcessorUnregistrationList = new List<IVisualElementChangeProcessor>();

	private BaseVisualElementPanel m_AttachedPanel;

	private readonly AuthoringChanges m_Changes1;

	private readonly AuthoringChanges m_Changes2;

	private AuthoringChanges m_Accumulator;

	private AuthoringChanges m_Notifier;

	private bool m_AccumulatingChanges;

	public override ProfilerMarker profilerMarker => s_UpdateProfilerMarker;

	private bool shouldUpdate => m_AccumulatingChanges || m_ProcessorRegistrationList.Count > 0;

	internal StateSnapshot GetState()
	{
		return new StateSnapshot
		{
			processorsCount = m_RegisteredProcessors.Count,
			containsAccumulatedChanges = m_Accumulator.ContainsChanges(),
			isProcessingChanges = m_AccumulatingChanges
		};
	}

	public VisualTreeAuthoringUpdater()
	{
		base.panelChanged += OnPanelChanged;
		m_Changes1 = new AuthoringChanges();
		m_Changes2 = new AuthoringChanges();
		m_Accumulator = m_Changes1;
		m_Notifier = m_Changes2;
	}

	public void RegisterProcessor(IVisualElementChangeProcessor processor)
	{
		if (!m_RegisteredProcessors.Contains(processor) && !m_ProcessorRegistrationList.Contains(processor))
		{
			m_ProcessorRegistrationList.Add(processor);
			m_ProcessorUnregistrationList.Remove(processor);
		}
	}

	public void UnregisterProcessor(IVisualElementChangeProcessor processor)
	{
		if (m_RegisteredProcessors.Contains(processor) && !m_ProcessorUnregistrationList.Contains(processor))
		{
			m_ProcessorUnregistrationList.Add(processor);
			m_ProcessorRegistrationList.Remove(processor);
		}
	}

	public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
	{
		if (!m_AccumulatingChanges)
		{
			return;
		}
		using (s_UpdateChangeProfilerMarker.Auto())
		{
			if ((versionChangeType & (VersionChangeType.Layout | VersionChangeType.Styles)) != 0)
			{
				m_Accumulator.styleChanged.Add(ve);
			}
			if ((versionChangeType & VersionChangeType.StyleSheet) != 0)
			{
				m_Accumulator.stylingContextChanged.Add(ve);
			}
			if ((versionChangeType & (VersionChangeType.Bindings | VersionChangeType.BindingRegistration | VersionChangeType.DataSource)) != 0)
			{
				m_Accumulator.bindingContextChanged.Add(ve);
			}
		}
	}

	public override void Update()
	{
		if (!shouldUpdate)
		{
			return;
		}
		bool? flag = null;
		SwapBuffers();
		AuthoringChanges notifier = m_Notifier;
		if (notifier.ContainsChanges())
		{
			for (int i = 0; i < m_RegisteredProcessors.Count; i++)
			{
				IVisualElementChangeProcessor visualElementChangeProcessor = m_RegisteredProcessors[i];
				visualElementChangeProcessor.ProcessChanges(base.panel, notifier);
			}
		}
		for (int j = 0; j < m_ProcessorRegistrationList.Count; j++)
		{
			IVisualElementChangeProcessor visualElementChangeProcessor2 = m_ProcessorRegistrationList[j];
			m_RegisteredProcessors.Add(visualElementChangeProcessor2);
			visualElementChangeProcessor2.BeginProcessing(base.panel);
			flag = true;
		}
		m_ProcessorRegistrationList.Clear();
		for (int k = 0; k < m_ProcessorUnregistrationList.Count; k++)
		{
			IVisualElementChangeProcessor visualElementChangeProcessor3 = m_ProcessorUnregistrationList[k];
			m_RegisteredProcessors.Remove(visualElementChangeProcessor3);
			visualElementChangeProcessor3.EndProcessing(base.panel);
		}
		m_ProcessorUnregistrationList.Clear();
		if (m_RegisteredProcessors.Count == 0)
		{
			flag = false;
		}
		if (flag.HasValue)
		{
			m_AccumulatingChanges = flag.Value;
		}
		notifier.Clear();
	}

	private void OnPanelChanged(BaseVisualElementPanel p)
	{
		if (m_AttachedPanel != p)
		{
			if (m_AttachedPanel != null)
			{
				m_AttachedPanel.hierarchyChanged -= OnHierarchyChange;
			}
			m_AttachedPanel = p;
			if (m_AttachedPanel != null)
			{
				m_AttachedPanel.hierarchyChanged += OnHierarchyChange;
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		SwapBuffers();
		AuthoringChanges notifier = m_Notifier;
		for (int i = 0; i < m_RegisteredProcessors.Count; i++)
		{
			IVisualElementChangeProcessor visualElementChangeProcessor = m_RegisteredProcessors[i];
			visualElementChangeProcessor.ProcessChanges(base.panel, notifier);
			visualElementChangeProcessor.EndProcessing(base.panel);
		}
		m_ProcessorRegistrationList.Clear();
		m_ProcessorUnregistrationList.Clear();
		m_RegisteredProcessors.Clear();
		base.panelChanged -= OnPanelChanged;
		if (m_AttachedPanel != null)
		{
			m_AttachedPanel.hierarchyChanged -= OnHierarchyChange;
		}
		notifier.Clear();
	}

	private void OnHierarchyChange(VisualElement ve, HierarchyChangeType type, IReadOnlyList<VisualElement> additionalContext = null)
	{
		if (!m_AccumulatingChanges)
		{
			return;
		}
		using (s_UpdateChangeProfilerMarker.Auto())
		{
			switch (type)
			{
			case HierarchyChangeType.RemovedFromParent:
				m_Accumulator.addedOrMovedElements.Remove(ve);
				break;
			case HierarchyChangeType.AddedToParent:
			case HierarchyChangeType.ChildrenReordered:
				m_Accumulator.addedOrMovedElements.Add(ve);
				break;
			case HierarchyChangeType.AttachedToPanel:
			{
				for (int j = 0; j < additionalContext.Count; j++)
				{
					VisualElement item2 = additionalContext[j];
					m_Accumulator.addedOrMovedElements.Add(item2);
					m_Accumulator.removedFromPanel.Remove(item2);
				}
				break;
			}
			case HierarchyChangeType.DetachedFromPanel:
			{
				for (int i = 0; i < additionalContext.Count; i++)
				{
					VisualElement item = additionalContext[i];
					m_Accumulator.addedOrMovedElements.Remove(item);
					m_Accumulator.removedFromPanel.Add(item);
				}
				break;
			}
			}
		}
	}

	private void SwapBuffers()
	{
		if (m_Accumulator == m_Changes1)
		{
			m_Accumulator = m_Changes2;
			m_Notifier = m_Changes1;
		}
		else
		{
			m_Accumulator = m_Changes1;
			m_Notifier = m_Changes2;
		}
	}
}
