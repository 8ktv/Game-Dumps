using System;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.Properties;

namespace UnityEngine.UIElements;

internal class VisualTreeDataBindingsUpdater : BaseVisualTreeUpdater
{
	private readonly struct VersionInfo
	{
		public readonly object source;

		public readonly long version;

		public VersionInfo(object source, long version)
		{
			this.source = source;
			this.version = version;
		}
	}

	private static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("UIElements.UpdateRuntimeBindings");

	private static readonly ProfilerMarker s_ProcessBindingRequestsProfilerMarker = new ProfilerMarker("Process Binding Requests");

	private static readonly ProfilerMarker s_ProcessDataSourcesProfilerMarker = new ProfilerMarker("Process Data Sources");

	private static readonly ProfilerMarker s_ShouldUpdateBindingProfilerMarker = new ProfilerMarker("Should Update Binding");

	private static readonly ProfilerMarker s_UpdateBindingProfilerMarker = new ProfilerMarker("Update Binding");

	private readonly BindingUpdater m_Updater = new BindingUpdater();

	private readonly List<VisualElement> m_BindingRegistrationRequests = new List<VisualElement>();

	private readonly HashSet<VisualElement> m_DataSourceChangedRequests = new HashSet<VisualElement>();

	private readonly HashSet<VisualElement> m_RemovedElements = new HashSet<VisualElement>();

	private readonly List<VisualElement> m_BoundsElement = new List<VisualElement>();

	private readonly List<VersionInfo> m_VersionChanges = new List<VersionInfo>();

	private readonly HashSet<object> m_TrackedObjects = new HashSet<object>();

	private readonly HashSet<Binding> m_RanUpdate = new HashSet<Binding>();

	private readonly HashSet<object> m_KnownSources = new HashSet<object>();

	private readonly HashSet<Binding> m_DirtyBindings = new HashSet<Binding>();

	private BaseVisualElementPanel m_AttachedPanel;

	private DataBindingManager bindingManager => base.panel.dataBindingManager;

	public override ProfilerMarker profilerMarker => s_UpdateProfilerMarker;

	public VisualTreeDataBindingsUpdater()
	{
		base.panelChanged += OnPanelChanged;
	}

	protected void OnHierarchyChange(VisualElement ve, HierarchyChangeType type, IReadOnlyList<VisualElement> additionalContext = null)
	{
		if (bindingManager.GetBoundElementsCount() == 0 && bindingManager.GetTrackedDataSourcesCount() == 0)
		{
			return;
		}
		switch (type)
		{
		case HierarchyChangeType.RemovedFromParent:
			m_DataSourceChangedRequests.Remove(ve);
			m_RemovedElements.Add(ve);
			break;
		case HierarchyChangeType.AddedToParent:
		case HierarchyChangeType.ChildrenReordered:
			m_RemovedElements.Remove(ve);
			m_DataSourceChangedRequests.Add(ve);
			break;
		case HierarchyChangeType.AttachedToPanel:
		{
			for (int j = 0; j < additionalContext.Count; j++)
			{
				VisualElement item2 = additionalContext[j];
				m_RemovedElements.Remove(item2);
				m_DataSourceChangedRequests.Add(ve);
			}
			break;
		}
		case HierarchyChangeType.DetachedFromPanel:
		{
			for (int i = 0; i < additionalContext.Count; i++)
			{
				VisualElement item = additionalContext[i];
				m_DataSourceChangedRequests.Remove(ve);
				m_RemovedElements.Add(item);
			}
			break;
		}
		}
		bindingManager.DirtyBindingOrder();
	}

	public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
	{
		if ((versionChangeType & VersionChangeType.BindingRegistration) == VersionChangeType.BindingRegistration)
		{
			m_BindingRegistrationRequests.Add(ve);
		}
		if ((versionChangeType & VersionChangeType.DataSource) == VersionChangeType.DataSource)
		{
			m_DataSourceChangedRequests.Add(ve);
		}
	}

	private void CacheAndLogBindingResult(bool appliedOnUiCache, in DataBindingManager.BindingData bindingData, in BindingResult result)
	{
		switch (bindingManager.logLevel)
		{
		case BindingLogLevel.Once:
		{
			BindingResult result2;
			if (appliedOnUiCache)
			{
				bindingManager.TryGetLastUIBindingResult(bindingData, out result2);
			}
			else
			{
				bindingManager.TryGetLastSourceBindingResult(bindingData, out result2);
			}
			if (result2.status != result.status || result2.message != result.message)
			{
				LogResult(in result);
			}
			break;
		}
		default:
			LogResult(in result);
			break;
		case BindingLogLevel.None:
			break;
		}
		if (appliedOnUiCache)
		{
			bindingManager.CacheUIBindingResult(bindingData, result);
		}
		else
		{
			bindingManager.CacheSourceBindingResult(bindingData, result);
		}
	}

	private void LogResult(in BindingResult result)
	{
		if (!string.IsNullOrWhiteSpace(result.message))
		{
			string text = (base.panel as Panel)?.name ?? base.panel.visualTree.name;
			Debug.LogWarning(result.message + " (" + text + ")");
		}
	}

	public override void Update()
	{
		ProcessAllBindingRequests();
		ProcessDataSourceChangedRequests();
		ProcessPropertyChangedEvents(m_RanUpdate);
		m_BoundsElement.AddRange(bindingManager.GetBoundElements());
		foreach (VisualElement item in m_BoundsElement)
		{
			List<DataBindingManager.BindingData> bindingData = bindingManager.GetBindingData(item);
			for (int i = 0; i < bindingData.Count; i++)
			{
				DataBindingManager.BindingData bindingData2 = bindingData[i];
				object dataSource;
				PropertyPath path;
				using (s_ShouldUpdateBindingProfilerMarker.Auto())
				{
					DataSourceContext resolvedDataSourceContext = bindingManager.GetResolvedDataSourceContext(item, bindingData2);
					dataSource = resolvedDataSourceContext.dataSource;
					path = resolvedDataSourceContext.dataSourcePath;
					var (flag, version) = GetDataSourceVersion(dataSource);
					if (bindingData2.binding == null)
					{
						continue;
					}
					if (dataSource != null && m_TrackedObjects.Add(dataSource))
					{
						m_VersionChanges.Add(new VersionInfo(dataSource, version));
					}
					if (bindingData2.binding.isDirty)
					{
						m_DirtyBindings.Add(bindingData2.binding);
					}
					if (!m_Updater.ShouldProcessBindingAtStage(bindingData2.binding, BindingUpdateStage.UpdateUI, flag, m_DirtyBindings.Contains(bindingData2.binding)))
					{
						continue;
					}
					if (dataSource != null && flag)
					{
						m_KnownSources.Add(dataSource);
					}
					if (bindingData2.binding.updateTrigger == BindingUpdateTrigger.OnSourceChanged && dataSource is INotifyBindablePropertyChanged && !bindingData2.binding.isDirty)
					{
						HashSet<PropertyPath> changedDetectedFromSource = bindingManager.GetChangedDetectedFromSource(dataSource);
						if (changedDetectedFromSource == null || changedDetectedFromSource.Count == 0)
						{
							continue;
						}
						bool flag2 = path.IsEmpty;
						foreach (PropertyPath item2 in changedDetectedFromSource)
						{
							if (IsPrefix(item2, in path))
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							continue;
						}
						goto IL_0242;
					}
					goto IL_0242;
				}
				IL_0242:
				bool isDirty = bindingData2.binding.isDirty;
				bindingData2.binding.ClearDirty();
				BindingContext context = new BindingContext(item, in bindingData2.target.bindingId, in path, dataSource);
				BindingResult result = default(BindingResult);
				long version2 = bindingData2.version;
				using (s_UpdateBindingProfilerMarker.Auto())
				{
					result = m_Updater.UpdateUI(in context, bindingData2.binding);
				}
				CacheAndLogBindingResult(appliedOnUiCache: true, in bindingData2, in result);
				if (bindingData2.version != version2)
				{
					continue;
				}
				switch (result.status)
				{
				case BindingStatus.Success:
					m_RanUpdate.Add(bindingData2.binding);
					break;
				case BindingStatus.Pending:
					if (isDirty)
					{
						bindingData2.binding.MarkDirty();
					}
					break;
				}
			}
		}
		foreach (VersionInfo versionChange in m_VersionChanges)
		{
			bindingManager.UpdateVersion(versionChange.source, versionChange.version);
		}
		ProcessPropertyChangedEvents(m_RanUpdate);
		foreach (object knownSource in m_KnownSources)
		{
			bindingManager.ClearChangesFromSource(knownSource);
		}
		m_BoundsElement.Clear();
		m_VersionChanges.Clear();
		m_TrackedObjects.Clear();
		m_RanUpdate.Clear();
		m_KnownSources.Clear();
		m_DirtyBindings.Clear();
		bindingManager.ClearSourceCache();
	}

	private (bool changed, long version) GetDataSourceVersion(object source)
	{
		if (bindingManager.TryGetLastVersion(source, out var version))
		{
			if (!(source is IDataSourceViewHashProvider dataSourceViewHashProvider))
			{
				return (changed: source != null, version: version + 1);
			}
			long viewHashCode = dataSourceViewHashProvider.GetViewHashCode();
			return (viewHashCode == version) ? (changed: false, version: version) : (changed: true, version: viewHashCode);
		}
		if (source is IDataSourceViewHashProvider dataSourceViewHashProvider2)
		{
			return (changed: true, version: dataSourceViewHashProvider2.GetViewHashCode());
		}
		return (changed: source != null, version: 0L);
	}

	private bool IsPrefix(in PropertyPath prefix, in PropertyPath path)
	{
		if (path.Length < prefix.Length)
		{
			return false;
		}
		for (int i = 0; i < prefix.Length; i++)
		{
			PropertyPathPart propertyPathPart = prefix[i];
			PropertyPathPart propertyPathPart2 = path[i];
			if (propertyPathPart.Kind != propertyPathPart2.Kind)
			{
				return false;
			}
			switch (propertyPathPart.Kind)
			{
			case PropertyPathPartKind.Name:
				if (propertyPathPart.Name != propertyPathPart2.Name)
				{
					return false;
				}
				break;
			case PropertyPathPartKind.Index:
				if (propertyPathPart.Index != propertyPathPart2.Index)
				{
					return false;
				}
				break;
			case PropertyPathPartKind.Key:
				if (propertyPathPart.Key != propertyPathPart2.Key)
				{
					return false;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return true;
	}

	private void ProcessDataSourceChangedRequests()
	{
		using (s_ProcessDataSourcesProfilerMarker.Auto())
		{
			if (m_DataSourceChangedRequests.Count != 0 || m_RemovedElements.Count != 0)
			{
				m_DataSourceChangedRequests.RemoveWhere((VisualElement e) => e.panel == null);
				bindingManager.InvalidateCachedDataSource(m_DataSourceChangedRequests, m_RemovedElements);
				m_DataSourceChangedRequests.Clear();
				m_RemovedElements.Clear();
			}
		}
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
		ProcessAllBindingRequests();
		ProcessDataSourceChangedRequests();
		base.Dispose(disposing);
		bindingManager.Dispose();
	}

	private void ProcessAllBindingRequests()
	{
		using (s_ProcessBindingRequestsProfilerMarker.Auto())
		{
			for (int i = 0; i < m_BindingRegistrationRequests.Count; i++)
			{
				VisualElement visualElement = m_BindingRegistrationRequests[i];
				if (visualElement.panel == base.panel)
				{
					ProcessBindingRequests(visualElement);
				}
			}
			m_BindingRegistrationRequests.Clear();
		}
	}

	private void ProcessBindingRequests(VisualElement element)
	{
		bindingManager.ProcessBindingRequests(element);
	}

	private void ProcessPropertyChangedEvents(HashSet<Binding> ranUpdate)
	{
		List<DataBindingManager.ChangesFromUI> changedDetectedFromUI = bindingManager.GetChangedDetectedFromUI();
		for (int i = 0; i < changedDetectedFromUI.Count; i++)
		{
			DataBindingManager.ChangesFromUI changesFromUI = changedDetectedFromUI[i];
			if (!changesFromUI.IsValid)
			{
				continue;
			}
			DataBindingManager.BindingData bindingData = changesFromUI.bindingData;
			Binding binding = bindingData.binding;
			VisualElement element = bindingData.target.element;
			if (!m_Updater.ShouldProcessBindingAtStage(binding, BindingUpdateStage.UpdateSource, versionChanged: true, dirty: false) || ranUpdate.Contains(binding))
			{
				continue;
			}
			DataSourceContext resolvedDataSourceContext = bindingManager.GetResolvedDataSourceContext(bindingData.target.element, bindingData);
			object dataSource = resolvedDataSourceContext.dataSource;
			PropertyPath resolvedDataSourcePath = resolvedDataSourceContext.dataSourcePath;
			BindingContext context = new BindingContext(element, in bindingData.target.bindingId, in resolvedDataSourcePath, dataSource);
			BindingResult result = m_Updater.UpdateSource(in context, binding);
			CacheAndLogBindingResult(appliedOnUiCache: false, in bindingData, in result);
			if (result.status != BindingStatus.Success || !changesFromUI.IsValid)
			{
				continue;
			}
			bool isDirty = bindingData.binding.isDirty;
			bindingData.binding.ClearDirty();
			BindingContext context2 = new BindingContext(element, in bindingData.target.bindingId, in resolvedDataSourcePath, dataSource);
			using (bindingManager.IgnoreChangesScope(element, context2.bindingId, binding))
			{
				result = m_Updater.UpdateUI(in context2, binding);
				CacheAndLogBindingResult(appliedOnUiCache: true, in bindingData, in result);
			}
			if (result.status == BindingStatus.Pending)
			{
				if (isDirty)
				{
					bindingData.binding.MarkDirty();
				}
				else
				{
					bindingData.binding.ClearDirty();
				}
			}
		}
		changedDetectedFromUI.Clear();
	}

	internal void PollElementsWithBindings(Action<VisualElement, IBinding> callback)
	{
		if (bindingManager.GetBoundElementsCount() <= 0)
		{
			return;
		}
		foreach (VisualElement unorderedBoundElement in bindingManager.GetUnorderedBoundElements())
		{
			if (unorderedBoundElement.elementPanel == base.panel)
			{
				callback(unorderedBoundElement, null);
			}
		}
	}
}
