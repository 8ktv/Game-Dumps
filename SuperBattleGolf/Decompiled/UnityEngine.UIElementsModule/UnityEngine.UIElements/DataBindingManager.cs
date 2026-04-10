#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

internal sealed class DataBindingManager : IDisposable
{
	private readonly struct BindingRequest
	{
		public readonly BindingId bindingId;

		public readonly Binding binding;

		public readonly bool shouldProcess;

		public BindingRequest(in BindingId bindingId, Binding binding, bool shouldProcess = true)
		{
			this.bindingId = bindingId;
			this.binding = binding;
			this.shouldProcess = shouldProcess;
		}

		public BindingRequest CancelRequest()
		{
			return new BindingRequest(in bindingId, binding, shouldProcess: false);
		}
	}

	private struct BindingDataCollection : IDisposable
	{
		private Dictionary<BindingId, BindingData> m_BindingPerId;

		private List<BindingData> m_Bindings;

		public static BindingDataCollection Create()
		{
			return new BindingDataCollection
			{
				m_BindingPerId = CollectionPool<Dictionary<BindingId, BindingData>, KeyValuePair<BindingId, BindingData>>.Get(),
				m_Bindings = CollectionPool<List<BindingData>, BindingData>.Get()
			};
		}

		public void AddBindingData(BindingData bindingData)
		{
			if (m_BindingPerId.TryGetValue(bindingData.target.bindingId, out var value))
			{
				m_Bindings.Remove(value);
			}
			m_BindingPerId[bindingData.target.bindingId] = bindingData;
			m_Bindings.Add(bindingData);
		}

		public bool TryGetBindingData(in BindingId bindingId, out BindingData data)
		{
			return m_BindingPerId.TryGetValue(bindingId, out data);
		}

		public bool RemoveBindingData(BindingData bindingData)
		{
			if (!m_BindingPerId.TryGetValue(bindingData.target.bindingId, out var value))
			{
				return false;
			}
			return m_Bindings.Remove(value) && m_BindingPerId.Remove(value.target.bindingId);
		}

		public List<BindingData> GetBindings()
		{
			return m_Bindings;
		}

		public int GetBindingCount()
		{
			return m_Bindings.Count;
		}

		public void Dispose()
		{
			if (m_BindingPerId != null)
			{
				CollectionPool<Dictionary<BindingId, BindingData>, KeyValuePair<BindingId, BindingData>>.Release(m_BindingPerId);
			}
			m_BindingPerId = null;
			if (m_Bindings != null)
			{
				CollectionPool<List<BindingData>, BindingData>.Release(m_Bindings);
			}
			m_Bindings = null;
		}
	}

	internal class BindingData
	{
		public long version;

		public BindingTarget target;

		public Binding binding;

		private DataSourceContext m_LastContext;

		public BindingResult? m_SourceToUILastUpdate;

		public BindingResult? m_UIToSourceLastUpdate;

		public object localDataSource { get; set; }

		public DataSourceContext context
		{
			get
			{
				return m_LastContext;
			}
			set
			{
				if (m_LastContext.dataSource != value.dataSource || !(m_LastContext.dataSourcePath == value.dataSourcePath))
				{
					DataSourceContext previousContext = m_LastContext;
					m_LastContext = value;
					binding.OnDataSourceChanged(new DataSourceContextChanged(target.element, in target.bindingId, in previousContext, in value));
					binding.MarkDirty();
				}
			}
		}

		public void Reset()
		{
			version++;
			target = default(BindingTarget);
			binding = null;
			localDataSource = null;
			m_LastContext = default(DataSourceContext);
			m_SourceToUILastUpdate = null;
			m_UIToSourceLastUpdate = null;
		}
	}

	internal readonly struct ChangesFromUI
	{
		public readonly long version;

		public readonly Binding binding;

		public readonly BindingData bindingData;

		public bool IsValid => version == bindingData.version && binding == bindingData.binding;

		public ChangesFromUI(BindingData bindingData)
		{
			this.bindingData = bindingData;
			version = bindingData.version;
			binding = bindingData.binding;
		}
	}

	private class HierarchyBindingTracker : IDisposable
	{
		private class HierarchicalBindingsSorter : HierarchyTraversal
		{
			public HashSet<VisualElement> boundElements { get; set; }

			public List<VisualElement> results { get; set; }

			public override void TraverseRecursive(VisualElement element, int depth)
			{
				if (boundElements.Count != results.Count)
				{
					if (boundElements.Contains(element))
					{
						results.Add(element);
					}
					Recurse(element, depth);
				}
			}
		}

		private readonly BaseVisualElementPanel m_Panel;

		private readonly HierarchicalBindingsSorter m_BindingSorter;

		private readonly Dictionary<VisualElement, BindingDataCollection> m_BindingDataPerElement;

		private readonly HashSet<VisualElement> m_BoundElements;

		private readonly List<VisualElement> m_OrderedBindings;

		private bool m_IsDirty;

		private EventCallback<PropertyChangedEvent, Dictionary<VisualElement, BindingDataCollection>> m_OnPropertyChanged;

		public int GetTrackedElementsCount()
		{
			return m_BoundElements.Count;
		}

		public List<VisualElement> GetBoundElements()
		{
			if (m_IsDirty)
			{
				OrderBindings(m_Panel.visualTree);
			}
			return m_OrderedBindings;
		}

		public IEnumerable<VisualElement> GetUnorderedBoundElements()
		{
			return m_BoundElements;
		}

		public HierarchyBindingTracker(BaseVisualElementPanel panel)
		{
			m_Panel = panel;
			m_BindingSorter = new HierarchicalBindingsSorter();
			m_BindingDataPerElement = new Dictionary<VisualElement, BindingDataCollection>();
			m_BoundElements = new HashSet<VisualElement>();
			m_OrderedBindings = new List<VisualElement>();
			m_IsDirty = true;
			m_OnPropertyChanged = OnPropertyChanged;
		}

		public void SetDirty()
		{
			m_IsDirty = true;
		}

		public bool TryGetBindingCollection(VisualElement element, out BindingDataCollection collection)
		{
			return m_BindingDataPerElement.TryGetValue(element, out collection);
		}

		public bool IsTrackingElement(VisualElement element)
		{
			return m_BoundElements.Contains(element);
		}

		public void StartTrackingBinding(VisualElement element, BindingData binding)
		{
			BindingDataCollection value;
			if (m_BoundElements.Add(element))
			{
				value = BindingDataCollection.Create();
				m_BindingDataPerElement.Add(element, value);
				element.RegisterCallback(m_OnPropertyChanged, m_BindingDataPerElement);
			}
			else if (!m_BindingDataPerElement.TryGetValue(element, out value))
			{
				throw new InvalidOperationException("Trying to add a binding to an element which doesn't have a binding collection. This is an internal bug. Please report using `Help > Report a Bug...`");
			}
			binding.binding.MarkDirty();
			value.AddBindingData(binding);
			m_BindingDataPerElement[element] = value;
			SetDirty();
		}

		private void OnPropertyChanged(PropertyChangedEvent evt, Dictionary<VisualElement, BindingDataCollection> bindingCollection)
		{
			if (!(evt.target is VisualElement visualElement))
			{
				throw new InvalidOperationException("Trying to track property changes on a non 'VisualElement'. This is an internal bug. Please report using `Help > Report a Bug...`");
			}
			if (!bindingCollection.TryGetValue(visualElement, out var value))
			{
				throw new InvalidOperationException("Trying to track property changes on a 'VisualElement' that is not being tracked. This is an internal bug. Please report using `Help > Report a Bug...`");
			}
			if (value.TryGetBindingData(evt.property, out var data) && visualElement.TryGetBinding(evt.property, out var binding) && data.binding == binding && !m_Panel.dataBindingManager.m_IgnoreUIChangesData.ShouldIgnoreChange(visualElement, binding, evt.property))
			{
				m_Panel.dataBindingManager.m_DetectedChangesFromUI.Add(new ChangesFromUI(data));
			}
		}

		public void StopTrackingBinding(VisualElement element, BindingData binding)
		{
			if (m_BoundElements.Contains(element) && m_BindingDataPerElement.TryGetValue(element, out var value))
			{
				value.RemoveBindingData(binding);
				if (value.GetBindingCount() == 0)
				{
					StopTrackingElement(element);
					element.UnregisterCallback(m_OnPropertyChanged);
				}
				else
				{
					m_BindingDataPerElement[element] = value;
				}
				SetDirty();
				return;
			}
			throw new InvalidOperationException("Trying to remove a binding to an element which doesn't have a binding collection. This is an internal bug. Please report using `Help > Report a Bug...`");
		}

		public void StopTrackingElement(VisualElement element)
		{
			if (m_BindingDataPerElement.TryGetValue(element, out var value))
			{
				value.Dispose();
			}
			m_BindingDataPerElement.Remove(element);
			m_BoundElements.Remove(element);
			SetDirty();
		}

		public void Dispose()
		{
			foreach (KeyValuePair<VisualElement, BindingDataCollection> item in m_BindingDataPerElement)
			{
				item.Value.Dispose();
			}
			m_BindingDataPerElement.Clear();
			m_BoundElements.Clear();
			m_OrderedBindings.Clear();
		}

		private void OrderBindings(VisualElement root)
		{
			m_OrderedBindings.Clear();
			m_BindingSorter.boundElements = m_BoundElements;
			m_BindingSorter.results = m_OrderedBindings;
			m_BindingSorter.Traverse(root);
			m_IsDirty = false;
		}
	}

	private class HierarchyDataSourceTracker : IDisposable
	{
		private class SourceInfo
		{
			private HashSet<PropertyPath> m_DetectedChanges;

			public long lastVersion { get; set; }

			public int refCount { get; set; }

			public HashSet<PropertyPath> detectedChanges => m_DetectedChanges ?? (m_DetectedChanges = new HashSet<PropertyPath>());

			public HashSet<PropertyPath> detectedChangesNoAlloc => m_DetectedChanges;
		}

		private class InvalidateDataSourcesTraversal : HierarchyTraversal
		{
			private readonly HierarchyDataSourceTracker m_DataSourceTracker;

			private readonly HashSet<VisualElement> m_VisitedElements;

			public InvalidateDataSourcesTraversal(HierarchyDataSourceTracker dataSourceTracker)
			{
				m_DataSourceTracker = dataSourceTracker;
				m_VisitedElements = new HashSet<VisualElement>();
			}

			public void Invalidate(List<VisualElement> addedOrMovedElements, HashSet<VisualElement> removedElements)
			{
				m_VisitedElements.Clear();
				for (int i = 0; i < addedOrMovedElements.Count; i++)
				{
					VisualElement element = addedOrMovedElements[i];
					Traverse(element);
				}
				foreach (VisualElement removedElement in removedElements)
				{
					if (!m_VisitedElements.Contains(removedElement))
					{
						Traverse(removedElement);
					}
				}
			}

			public override void TraverseRecursive(VisualElement element, int depth)
			{
				if (!m_VisitedElements.Contains(element) && (depth <= 0 || element.dataSource == null))
				{
					m_VisitedElements.Add(element);
					m_DataSourceTracker.RemoveHierarchyDataSourceContextFromElement(element);
					Recurse(element, depth);
				}
			}
		}

		private class ObjectComparer : IEqualityComparer<object>
		{
			bool IEqualityComparer<object>.Equals(object x, object y)
			{
				return x == y || EqualityComparer<object>.Default.Equals(x, y);
			}

			int IEqualityComparer<object>.GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}
		}

		private readonly List<SourceInfo> m_SourceInfosPool = new List<SourceInfo>();

		private readonly DataBindingManager m_DataBindingManager;

		private readonly Dictionary<VisualElement, DataSourceContext> m_ResolvedHierarchicalDataSourceContext;

		private readonly Dictionary<Binding, int> m_BindingRefCount;

		private readonly Dictionary<object, SourceInfo> m_SourceInfos;

		private readonly HashSet<object> m_SourcesToRemove;

		private readonly InvalidateDataSourcesTraversal m_InvalidateResolvedDataSources;

		private readonly EventHandler<BindablePropertyChangedEventArgs> m_Handler;

		private readonly EventCallback<PropertyChangedEvent, VisualElement> m_VisualElementHandler;

		private SourceInfo GetPooledSourceInfo()
		{
			SourceInfo result;
			if (m_SourceInfosPool.Count > 0)
			{
				List<SourceInfo> sourceInfosPool = m_SourceInfosPool;
				result = sourceInfosPool[sourceInfosPool.Count - 1];
				m_SourceInfosPool.RemoveAt(m_SourceInfosPool.Count - 1);
			}
			else
			{
				result = new SourceInfo();
			}
			return result;
		}

		private void ReleasePooledSourceInfo(SourceInfo info)
		{
			info.lastVersion = long.MinValue;
			info.refCount = 0;
			info.detectedChangesNoAlloc?.Clear();
			m_SourceInfosPool.Add(info);
		}

		public HierarchyDataSourceTracker(DataBindingManager manager)
		{
			m_DataBindingManager = manager;
			m_ResolvedHierarchicalDataSourceContext = new Dictionary<VisualElement, DataSourceContext>();
			m_BindingRefCount = new Dictionary<Binding, int>();
			ObjectComparer comparer = new ObjectComparer();
			m_SourceInfos = new Dictionary<object, SourceInfo>(comparer);
			m_SourcesToRemove = new HashSet<object>(comparer);
			m_InvalidateResolvedDataSources = new InvalidateDataSourcesTraversal(this);
			m_Handler = TrackPropertyChanges;
			m_VisualElementHandler = OnVisualElementPropertyChanged;
		}

		internal void IncreaseBindingRefCount(ref BindingData bindingData)
		{
			Binding binding = bindingData.binding;
			if (binding != null)
			{
				if (!m_BindingRefCount.TryGetValue(binding, out var value))
				{
					value = 0;
				}
				if (binding is IDataSourceProvider dataSourceProvider)
				{
					IncreaseRefCount(dataSourceProvider.dataSource);
					bindingData.localDataSource = dataSourceProvider.dataSource;
				}
				m_BindingRefCount[binding] = value + 1;
			}
		}

		internal void DecreaseBindingRefCount(ref BindingData bindingData)
		{
			Binding binding = bindingData.binding;
			if (binding != null)
			{
				if (!m_BindingRefCount.TryGetValue(binding, out var value))
				{
					throw new InvalidOperationException("Trying to release a binding that isn't tracked. This is an internal bug. Please report using `Help > Report a Bug...`");
				}
				if (value == 1)
				{
					m_BindingRefCount.Remove(binding);
				}
				else
				{
					m_BindingRefCount[binding] = value - 1;
				}
				if (binding is IDataSourceProvider dataSourceProvider)
				{
					DecreaseRefCount(dataSourceProvider.dataSource);
				}
			}
		}

		internal void IncreaseRefCount(object dataSource)
		{
			if (dataSource == null)
			{
				return;
			}
			bool flag = m_SourcesToRemove.Remove(dataSource);
			if (!m_SourceInfos.TryGetValue(dataSource, out var value))
			{
				value = (m_SourceInfos[dataSource] = GetPooledSourceInfo());
				flag = true;
			}
			if (flag)
			{
				if (dataSource is INotifyBindablePropertyChanged notifyBindablePropertyChanged)
				{
					notifyBindablePropertyChanged.propertyChanged += m_Handler;
				}
				if (dataSource is VisualElement visualElement)
				{
					visualElement.RegisterCallback(m_VisualElementHandler, visualElement);
				}
			}
			SourceInfo sourceInfo = value;
			int refCount = sourceInfo.refCount + 1;
			sourceInfo.refCount = refCount;
		}

		private void OnVisualElementPropertyChanged(PropertyChangedEvent evt, VisualElement element)
		{
			TrackPropertyChanges(element, evt.property);
		}

		internal void DecreaseRefCount(object dataSource)
		{
			if (dataSource == null)
			{
				return;
			}
			if (!m_SourceInfos.TryGetValue(dataSource, out var value) || value.refCount == 0)
			{
				throw new InvalidOperationException("Trying to release a data source that isn't tracked. This is an internal bug. Please report using `Help > Report a Bug...`");
			}
			if (value.refCount == 1)
			{
				value.refCount = 0;
				m_SourcesToRemove.Add(dataSource);
				if (dataSource is INotifyBindablePropertyChanged notifyBindablePropertyChanged)
				{
					notifyBindablePropertyChanged.propertyChanged -= m_Handler;
				}
				if (dataSource is VisualElement visualElement)
				{
					visualElement.UnregisterCallback(m_VisualElementHandler);
				}
			}
			else
			{
				SourceInfo sourceInfo = value;
				int refCount = sourceInfo.refCount - 1;
				sourceInfo.refCount = refCount;
			}
		}

		public int GetRefCount(object dataSource)
		{
			SourceInfo value;
			return m_SourceInfos.TryGetValue(dataSource, out value) ? value.refCount : 0;
		}

		public int GetTrackedDataSourcesCount()
		{
			return m_ResolvedHierarchicalDataSourceContext.Count;
		}

		public bool IsTrackingDataSource(VisualElement element)
		{
			return m_ResolvedHierarchicalDataSourceContext.ContainsKey(element);
		}

		public HashSet<PropertyPath> GetChangesFromSource(object dataSource)
		{
			SourceInfo value;
			return m_SourceInfos.TryGetValue(dataSource, out value) ? value.detectedChangesNoAlloc : null;
		}

		public void ClearChangesFromSource(object dataSource)
		{
			if (m_SourceInfos.TryGetValue(dataSource, out var value))
			{
				value.detectedChangesNoAlloc?.Clear();
			}
		}

		public void InvalidateCachedDataSource(HashSet<VisualElement> elements, HashSet<VisualElement> removedElements)
		{
			List<VisualElement> list = CollectionPool<List<VisualElement>, VisualElement>.Get();
			try
			{
				foreach (VisualElement element in elements)
				{
					list.Add(element);
				}
				m_InvalidateResolvedDataSources.Invalidate(list, removedElements);
			}
			finally
			{
				CollectionPool<List<VisualElement>, VisualElement>.Release(list);
			}
		}

		public DataSourceContext GetResolvedDataSourceContext(VisualElement element, BindingData bindingData)
		{
			object obj = null;
			PropertyPath pathToAppend = default(PropertyPath);
			if (bindingData.binding is IDataSourceProvider dataSourceProvider)
			{
				obj = dataSourceProvider.dataSource;
				pathToAppend = dataSourceProvider.dataSourcePath;
			}
			object localDataSource = bindingData.localDataSource;
			object dataSource = obj;
			PropertyPath dataSourcePath = pathToAppend;
			try
			{
				if (obj == null)
				{
					DecreaseRefCount(localDataSource);
					DataSourceContext hierarchicalDataSourceContext = GetHierarchicalDataSourceContext(element);
					dataSource = hierarchicalDataSourceContext.dataSource;
					dataSourcePath = ((!pathToAppend.IsEmpty) ? PropertyPath.Combine(hierarchicalDataSourceContext.dataSourcePath, in pathToAppend) : hierarchicalDataSourceContext.dataSourcePath);
					return new DataSourceContext(dataSource, in dataSourcePath);
				}
				if (obj != localDataSource)
				{
					DecreaseRefCount(localDataSource);
					IncreaseRefCount(obj);
				}
			}
			finally
			{
				bindingData.localDataSource = obj;
				DataSourceContext context = new DataSourceContext(dataSource, in dataSourcePath);
				bindingData.context = context;
			}
			return new DataSourceContext(dataSource, in dataSourcePath);
		}

		private void TrackPropertyChanges(object sender, BindablePropertyChangedEventArgs args)
		{
			TrackPropertyChanges(sender, args.propertyName);
		}

		private void TrackPropertyChanges(object sender, PropertyPath propertyPath)
		{
			if (m_SourceInfos.TryGetValue(sender, out var value))
			{
				HashSet<PropertyPath> detectedChanges = value.detectedChanges;
				detectedChanges.Add(propertyPath);
			}
		}

		public bool TryGetLastVersion(object source, out long version)
		{
			if (source != null && m_SourceInfos.TryGetValue(source, out var value))
			{
				version = value.lastVersion;
				return true;
			}
			version = -1L;
			return false;
		}

		public void UpdateVersion(object source, long version)
		{
			SourceInfo sourceInfo = m_SourceInfos[source];
			sourceInfo.lastVersion = version;
			m_SourceInfos[source] = sourceInfo;
		}

		internal object GetHierarchyDataSource(VisualElement element)
		{
			return GetHierarchicalDataSourceContext(element).dataSource;
		}

		internal DataSourceContext GetHierarchicalDataSourceContext(VisualElement element)
		{
			if (m_ResolvedHierarchicalDataSourceContext.TryGetValue(element, out var value))
			{
				return value;
			}
			VisualElement visualElement = element;
			PropertyPath pathToAppend = default(PropertyPath);
			while (visualElement != null)
			{
				if (!visualElement.isDataSourcePathEmpty)
				{
					pathToAppend = PropertyPath.Combine(visualElement.dataSourcePath, in pathToAppend);
				}
				if (visualElement.dataSource != null)
				{
					object dataSource = visualElement.dataSource;
					return m_ResolvedHierarchicalDataSourceContext[element] = new DataSourceContext(dataSource, in pathToAppend);
				}
				visualElement = visualElement.hierarchy.parent;
			}
			return m_ResolvedHierarchicalDataSourceContext[element] = new DataSourceContext(null, in pathToAppend);
		}

		internal void RemoveHierarchyDataSourceContextFromElement(VisualElement element)
		{
			m_ResolvedHierarchicalDataSourceContext.Remove(element);
		}

		public void Dispose()
		{
			m_ResolvedHierarchicalDataSourceContext.Clear();
			m_BindingRefCount.Clear();
			m_SourcesToRemove.Clear();
			m_SourceInfosPool.Clear();
			m_SourceInfos.Clear();
		}

		public void ClearSourceCache()
		{
			foreach (object item in m_SourcesToRemove)
			{
				if (m_SourceInfos.TryGetValue(item, out var value))
				{
					if (value.refCount == 0)
					{
						m_SourceInfos.Remove(item);
						ReleasePooledSourceInfo(value);
						continue;
					}
					throw new InvalidOperationException("Trying to release a data source that is still being referenced. This is an internal bug. Please report using `Help > Report a Bug...`");
				}
				throw new InvalidOperationException("Trying to release a data source that isn't tracked. This is an internal bug. Please report using `Help > Report a Bug...`");
			}
			m_SourcesToRemove.Clear();
		}
	}

	private struct IgnoreUIChangesData
	{
		public VisualElement element;

		public Binding binding;

		public BindingId bindingId;

		public bool ShouldIgnoreChange(VisualElement ve, Binding b, BindingId id)
		{
			return element == ve && binding == b && bindingId == id;
		}
	}

	public struct IgnoreUIChangesScope : IDisposable
	{
		private IgnoreUIChangesData m_ScopeData;

		private DataBindingManager manager;

		internal IgnoreUIChangesScope(DataBindingManager manager, VisualElement target, BindingId bindingId, Binding binding)
		{
			this.manager = manager;
			m_ScopeData = this.manager.m_IgnoreUIChangesData;
			this.manager.m_IgnoreUIChangesData = new IgnoreUIChangesData
			{
				element = target,
				binding = binding,
				bindingId = bindingId
			};
		}

		public void Dispose()
		{
			manager.m_IgnoreUIChangesData = m_ScopeData;
		}
	}

	private readonly List<BindingData> m_BindingDataLocalPool = new List<BindingData>(64);

	private static readonly PropertyName k_RequestBindingPropertyName = "__unity-binding-request";

	private static readonly BindingId k_ClearBindingsToken = "$__BindingManager--ClearAllBindings";

	internal static BindingLogLevel globalLogLevel = BindingLogLevel.All;

	private BindingLogLevel? m_LogLevel;

	private static readonly List<BindingData> s_Empty = new List<BindingData>();

	private readonly BaseVisualElementPanel m_Panel;

	private readonly HierarchyDataSourceTracker m_DataSourceTracker;

	private readonly HierarchyBindingTracker m_BindingsTracker;

	private readonly List<ChangesFromUI> m_DetectedChangesFromUI;

	private IgnoreUIChangesData m_IgnoreUIChangesData;

	internal BindingLogLevel logLevel
	{
		get
		{
			return m_LogLevel ?? globalLogLevel;
		}
		set
		{
			m_LogLevel = value;
		}
	}

	internal void ResetLogLevel()
	{
		m_LogLevel = null;
	}

	internal DataBindingManager(BaseVisualElementPanel panel)
	{
		m_Panel = panel;
		m_DataSourceTracker = new HierarchyDataSourceTracker(this);
		m_BindingsTracker = new HierarchyBindingTracker(panel);
		m_DetectedChangesFromUI = new List<ChangesFromUI>();
	}

	internal int GetTrackedDataSourcesCount()
	{
		return m_DataSourceTracker.GetTrackedDataSourcesCount();
	}

	internal bool IsTrackingDataSource(VisualElement element)
	{
		return m_DataSourceTracker.IsTrackingDataSource(element);
	}

	internal bool TryGetLastVersion(object source, out long version)
	{
		return m_DataSourceTracker.TryGetLastVersion(source, out version);
	}

	internal void UpdateVersion(object source, long version)
	{
		m_DataSourceTracker.UpdateVersion(source, version);
	}

	internal void CacheUIBindingResult(BindingData bindingData, BindingResult result)
	{
		bindingData.m_SourceToUILastUpdate = result;
	}

	internal bool TryGetLastUIBindingResult(BindingData bindingData, out BindingResult result)
	{
		if (bindingData.m_SourceToUILastUpdate.HasValue)
		{
			result = bindingData.m_SourceToUILastUpdate.Value;
			return true;
		}
		result = default(BindingResult);
		return false;
	}

	internal void CacheSourceBindingResult(BindingData bindingData, BindingResult result)
	{
		bindingData.m_UIToSourceLastUpdate = result;
	}

	internal bool TryGetLastSourceBindingResult(BindingData bindingData, out BindingResult result)
	{
		if (bindingData.m_UIToSourceLastUpdate.HasValue)
		{
			result = bindingData.m_UIToSourceLastUpdate.Value;
			return true;
		}
		result = default(BindingResult);
		return false;
	}

	internal DataSourceContext GetResolvedDataSourceContext(VisualElement element, BindingData bindingData)
	{
		return (element.panel == m_Panel) ? m_DataSourceTracker.GetResolvedDataSourceContext(element, bindingData) : default(DataSourceContext);
	}

	internal bool TryGetSource(VisualElement element, out object dataSource)
	{
		if (element.panel == m_Panel)
		{
			dataSource = m_DataSourceTracker.GetHierarchyDataSource(element);
			return true;
		}
		dataSource = null;
		return false;
	}

	internal object TrackHierarchyDataSource(VisualElement element)
	{
		return (element.panel == m_Panel) ? m_DataSourceTracker.GetHierarchicalDataSourceContext(element).dataSource : null;
	}

	internal int GetRefCount(object dataSource)
	{
		return m_DataSourceTracker.GetRefCount(dataSource);
	}

	internal int GetBoundElementsCount()
	{
		return m_BindingsTracker.GetTrackedElementsCount();
	}

	internal IEnumerable<VisualElement> GetBoundElements()
	{
		return m_BindingsTracker.GetBoundElements();
	}

	internal IEnumerable<VisualElement> GetUnorderedBoundElements()
	{
		return m_BindingsTracker.GetUnorderedBoundElements();
	}

	public IgnoreUIChangesScope IgnoreChangesScope(VisualElement target, BindingId bindingId, Binding binding)
	{
		return new IgnoreUIChangesScope(this, target, bindingId, binding);
	}

	internal List<ChangesFromUI> GetChangedDetectedFromUI()
	{
		return m_DetectedChangesFromUI;
	}

	internal HashSet<PropertyPath> GetChangedDetectedFromSource(object dataSource)
	{
		return m_DataSourceTracker.GetChangesFromSource(dataSource);
	}

	internal void ClearChangesFromSource(object dataSource)
	{
		m_DataSourceTracker.ClearChangesFromSource(dataSource);
	}

	internal List<BindingData> GetBindingData(VisualElement element)
	{
		BindingDataCollection collection;
		return (element.panel != m_Panel) ? s_Empty : (m_BindingsTracker.TryGetBindingCollection(element, out collection) ? collection.GetBindings() : s_Empty);
	}

	internal bool TryGetBindingData(VisualElement element, in BindingId bindingId, out BindingData bindingData)
	{
		bindingData = null;
		if (element.panel == m_Panel && m_BindingsTracker.TryGetBindingCollection(element, out var collection))
		{
			return collection.TryGetBindingData(in bindingId, out bindingData);
		}
		bindingData = null;
		return false;
	}

	internal void RegisterBinding(VisualElement element, in BindingId bindingId, Binding binding)
	{
		Assert.IsFalse(binding == null);
		Assert.IsFalse(((PropertyPath)bindingId).IsEmpty, "[UI Toolkit] Could not register binding on element of type '" + element.GetType().Name + "': target property path is empty.");
		if (m_BindingsTracker.TryGetBindingCollection(element, out var collection) && collection.TryGetBindingData(in bindingId, out var data))
		{
			data.binding.OnDeactivated(new BindingActivationContext(element, in bindingId));
			DataSourceContext previousContext = m_DataSourceTracker.GetResolvedDataSourceContext(element, data);
			IDataSourceProvider dataSourceProvider = data.binding as IDataSourceProvider;
			object obj = dataSourceProvider?.dataSource;
			PropertyPath dataSourcePath = dataSourceProvider?.dataSourcePath ?? default(PropertyPath);
			if (previousContext.dataSource != obj || previousContext.dataSourcePath != dataSourcePath)
			{
				data.binding.OnDataSourceChanged(new DataSourceContextChanged(element, in bindingId, in previousContext, new DataSourceContext(obj, in dataSourcePath)));
			}
			m_DataSourceTracker.DecreaseBindingRefCount(ref data);
		}
		BindingData bindingData = GetPooledBindingData(new BindingTarget(element, in bindingId), binding);
		m_DataSourceTracker.IncreaseBindingRefCount(ref bindingData);
		m_BindingsTracker.StartTrackingBinding(element, bindingData);
		binding.OnActivated(new BindingActivationContext(element, in bindingId));
	}

	internal void UnregisterBinding(VisualElement element, in BindingId bindingId)
	{
		if (m_BindingsTracker.TryGetBindingCollection(element, out var collection) && collection.TryGetBindingData(in bindingId, out var data))
		{
			DataSourceContext previousContext = m_DataSourceTracker.GetResolvedDataSourceContext(element, data);
			IDataSourceProvider dataSourceProvider = data.binding as IDataSourceProvider;
			object obj = dataSourceProvider?.dataSource;
			PropertyPath dataSourcePath = dataSourceProvider?.dataSourcePath ?? default(PropertyPath);
			if (previousContext.dataSource != obj || previousContext.dataSourcePath != dataSourcePath)
			{
				data.binding.OnDataSourceChanged(new DataSourceContextChanged(element, in bindingId, in previousContext, new DataSourceContext(obj, in dataSourcePath)));
			}
			data.binding.OnDeactivated(new BindingActivationContext(element, in bindingId));
			m_DataSourceTracker.DecreaseBindingRefCount(ref data);
			m_BindingsTracker.StopTrackingBinding(element, data);
			ReleasePoolBindingData(data);
		}
	}

	internal void TransferBindingRequests(VisualElement element)
	{
		if (!m_BindingsTracker.IsTrackingElement(element))
		{
			return;
		}
		if (m_BindingsTracker.TryGetBindingCollection(element, out var collection))
		{
			List<BindingData> bindings = collection.GetBindings();
			while (bindings.Count > 0)
			{
				BindingData bindingData = bindings[bindings.Count - 1];
				CreateBindingRequest(element, in bindingData.target.bindingId, bindingData.binding, isTransferring: true);
				UnregisterBinding(element, in bindingData.target.bindingId);
			}
		}
		m_BindingsTracker.StopTrackingElement(element);
	}

	public void InvalidateCachedDataSource(HashSet<VisualElement> addedOrMovedElements, HashSet<VisualElement> removedElements)
	{
		m_DataSourceTracker.InvalidateCachedDataSource(addedOrMovedElements, removedElements);
	}

	public void Dispose()
	{
		m_BindingsTracker.Dispose();
		m_DataSourceTracker.Dispose();
		m_DetectedChangesFromUI.Clear();
	}

	public static void CreateBindingRequest(VisualElement target, in BindingId bindingId, Binding binding)
	{
		CreateBindingRequest(target, in bindingId, binding, isTransferring: false);
	}

	private static void CreateBindingRequest(VisualElement target, in BindingId bindingId, Binding binding, bool isTransferring)
	{
		List<BindingRequest> list = (List<BindingRequest>)target.GetProperty(k_RequestBindingPropertyName);
		if (list == null)
		{
			list = new List<BindingRequest>();
			target.SetProperty(k_RequestBindingPropertyName, list);
		}
		bool shouldProcess = true;
		for (int i = 0; i < list.Count; i++)
		{
			BindingRequest bindingRequest = list[i];
			if (bindingRequest.bindingId == bindingId)
			{
				if (isTransferring)
				{
					shouldProcess = false;
				}
				else
				{
					list[i] = bindingRequest.CancelRequest();
				}
			}
		}
		list.Add(new BindingRequest(in bindingId, binding, shouldProcess));
	}

	public static void CreateClearAllBindingsRequest(VisualElement target)
	{
		CreateBindingRequest(target, in k_ClearBindingsToken, null);
	}

	public void ProcessBindingRequests(VisualElement element)
	{
		List<BindingRequest> list = (List<BindingRequest>)element.GetProperty(k_RequestBindingPropertyName);
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			BindingRequest bindingRequest = list[i];
			if (bindingRequest.shouldProcess)
			{
				if (bindingRequest.bindingId == k_ClearBindingsToken)
				{
					ClearAllBindings(element);
				}
				else if (bindingRequest.bindingId == BindingId.Invalid)
				{
					IPanel panel = element.panel;
					string text = (panel as Panel)?.name ?? panel.visualTree.name;
					Debug.LogError("[UI Toolkit] Trying to set a binding on `" + (string.IsNullOrWhiteSpace(element.name) ? "<no name>" : element.name) + " (" + TypeUtility.GetTypeDisplayName(element.GetType()) + ")` without setting the \"property\" attribute is not supported (" + text + ").");
				}
				else if (bindingRequest.binding != null)
				{
					RegisterBinding(element, in bindingRequest.bindingId, bindingRequest.binding);
				}
				else
				{
					UnregisterBinding(element, in bindingRequest.bindingId);
				}
			}
		}
		list.Clear();
	}

	private void ClearAllBindings(VisualElement element)
	{
		List<BindingData> list = CollectionPool<List<BindingData>, BindingData>.Get();
		try
		{
			list.AddRange(GetBindingData(element));
			foreach (BindingData item in list)
			{
				UnregisterBinding(element, in item.target.bindingId);
			}
		}
		finally
		{
			CollectionPool<List<BindingData>, BindingData>.Release(list);
		}
	}

	internal static bool AnyPendingBindingRequests(VisualElement element)
	{
		List<BindingRequest> list = (List<BindingRequest>)element.GetProperty(k_RequestBindingPropertyName);
		if (list == null)
		{
			return false;
		}
		return list.Count > 0;
	}

	internal static void GetBindingRequests(VisualElement element, List<(Binding binding, BindingId bindingId)> bindingRequests)
	{
		List<BindingRequest> list = (List<BindingRequest>)element.GetProperty(k_RequestBindingPropertyName);
		if (list == null)
		{
			return;
		}
		HashSet<BindingId> hashSet = CollectionPool<HashSet<BindingId>, BindingId>.Get();
		try
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				BindingRequest bindingRequest = list[num];
				if (hashSet.Add(bindingRequest.bindingId))
				{
					bindingRequests.Add((bindingRequest.binding, bindingRequest.bindingId));
				}
			}
		}
		finally
		{
			CollectionPool<HashSet<BindingId>, BindingId>.Release(hashSet);
		}
	}

	internal static bool TryGetBindingRequest(VisualElement element, in BindingId bindingId, out Binding binding)
	{
		List<BindingRequest> list = (List<BindingRequest>)element.GetProperty(k_RequestBindingPropertyName);
		if (list == null)
		{
			binding = null;
			return false;
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			BindingRequest bindingRequest = list[num];
			if (!(bindingId != bindingRequest.bindingId) && bindingRequest.shouldProcess)
			{
				binding = bindingRequest.binding;
				return true;
			}
		}
		binding = null;
		return false;
	}

	public void DirtyBindingOrder()
	{
		m_BindingsTracker.SetDirty();
	}

	public void TrackDataSource(object previous, object current)
	{
		m_DataSourceTracker.DecreaseRefCount(previous);
		m_DataSourceTracker.IncreaseRefCount(current);
	}

	internal (int boundElementsCount, int trackedDataSourcesCount) GetTrackedInfo()
	{
		int trackedElementsCount = m_BindingsTracker.GetTrackedElementsCount();
		int trackedDataSourcesCount = m_DataSourceTracker.GetTrackedDataSourcesCount();
		return (boundElementsCount: trackedElementsCount, trackedDataSourcesCount: trackedDataSourcesCount);
	}

	public void ClearSourceCache()
	{
		m_DataSourceTracker.ClearSourceCache();
	}

	public BindingData GetPooledBindingData(BindingTarget target, Binding binding)
	{
		BindingData bindingData;
		if (m_BindingDataLocalPool.Count > 0)
		{
			List<BindingData> bindingDataLocalPool = m_BindingDataLocalPool;
			bindingData = bindingDataLocalPool[bindingDataLocalPool.Count - 1];
			m_BindingDataLocalPool.RemoveAt(m_BindingDataLocalPool.Count - 1);
		}
		else
		{
			bindingData = new BindingData();
		}
		bindingData.target = target;
		bindingData.binding = binding;
		return bindingData;
	}

	public void ReleasePoolBindingData(BindingData data)
	{
		data.Reset();
		m_BindingDataLocalPool.Add(data);
	}
}
