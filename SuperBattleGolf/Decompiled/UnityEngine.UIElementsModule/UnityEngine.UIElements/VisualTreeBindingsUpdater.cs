using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;

namespace UnityEngine.UIElements;

internal class VisualTreeBindingsUpdater : BaseVisualTreeHierarchyTrackerUpdater
{
	private class RequestObjectListPool : ObjectListPool<IBindingRequest>
	{
	}

	private static readonly PropertyName s_BindingRequestObjectVEPropertyName = "__unity-binding-request-object";

	private static readonly PropertyName s_AdditionalBindingObjectVEPropertyName = "__unity-additional-binding-object";

	private static readonly string s_Description = "UIElements.UpdateEditorBindings";

	private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(s_Description);

	private static readonly ProfilerMarker s_ProfilerBindingRequestsMarker = new ProfilerMarker("Bindings.Requests");

	private static ProfilerMarker s_MarkerUpdate = new ProfilerMarker("Bindings.Update");

	private static ProfilerMarker s_MarkerPoll = new ProfilerMarker("Bindings.PollElementsWithBindings");

	private readonly HashSet<VisualElement> m_ElementsWithBindings = new HashSet<VisualElement>();

	private readonly HashSet<VisualElement> m_ElementsToAdd = new HashSet<VisualElement>();

	private readonly HashSet<VisualElement> m_ElementsToRemove = new HashSet<VisualElement>();

	internal const int k_MinUpdateDelayMs = 100;

	private const int k_MaxBindingTimeMs = 100;

	private long m_LastUpdateTime = 0L;

	private HashSet<VisualElement> m_ElementsToBind = new HashSet<VisualElement>();

	private List<IBinding> updatedBindings = new List<IBinding>();

	public override ProfilerMarker profilerMarker => s_ProfilerMarker;

	public static bool disableBindingsThrottling { get; set; } = false;

	public Dictionary<object, object> temporaryObjectCache { get; private set; } = new Dictionary<object, object>();

	private IBinding GetBindingObjectFromElement(VisualElement ve)
	{
		if (!(ve is IBindable { binding: not null, binding: var binding }))
		{
			return GetAdditionalBinding(ve);
		}
		return binding;
	}

	private void StartTracking(VisualElement ve)
	{
		m_ElementsToAdd.Add(ve);
		m_ElementsToRemove.Remove(ve);
	}

	private void StopTracking(VisualElement ve)
	{
		m_ElementsToRemove.Add(ve);
		m_ElementsToAdd.Remove(ve);
	}

	public static void SetAdditionalBinding(VisualElement ve, IBinding b)
	{
		GetAdditionalBinding(ve)?.Release();
		ve.SetProperty(s_AdditionalBindingObjectVEPropertyName, b);
		ve.IncrementVersion(VersionChangeType.Bindings);
	}

	public static void ClearAdditionalBinding(VisualElement ve)
	{
		SetAdditionalBinding(ve, null);
	}

	public static IBinding GetAdditionalBinding(VisualElement ve)
	{
		return ve.GetProperty(s_AdditionalBindingObjectVEPropertyName) as IBinding;
	}

	public static void AddBindingRequest(VisualElement ve, IBindingRequest req)
	{
		List<IBindingRequest> list = ve.GetProperty(s_BindingRequestObjectVEPropertyName) as List<IBindingRequest>;
		if (list == null)
		{
			list = ObjectListPool<IBindingRequest>.Get();
			ve.SetProperty(s_BindingRequestObjectVEPropertyName, list);
		}
		list.Add(req);
		ve.IncrementVersion(VersionChangeType.Bindings);
	}

	public static void RemoveBindingRequest(VisualElement ve, IBindingRequest req)
	{
		if (ve.GetProperty(s_BindingRequestObjectVEPropertyName) is List<IBindingRequest> list)
		{
			req.Release();
			list.Remove(req);
			if (list.Count == 0)
			{
				ObjectListPool<IBindingRequest>.Release(list);
				ve.SetProperty(s_BindingRequestObjectVEPropertyName, null);
			}
		}
	}

	public static void ClearBindingRequests(VisualElement ve)
	{
		if (!(ve.GetProperty(s_BindingRequestObjectVEPropertyName) is List<IBindingRequest> list))
		{
			return;
		}
		foreach (IBindingRequest item in list)
		{
			item.Release();
		}
		ObjectListPool<IBindingRequest>.Release(list);
		ve.SetProperty(s_BindingRequestObjectVEPropertyName, null);
	}

	private void StartTrackingRecursive(VisualElement ve)
	{
		IBinding bindingObjectFromElement = GetBindingObjectFromElement(ve);
		if (bindingObjectFromElement != null)
		{
			StartTracking(ve);
		}
		object property = ve.GetProperty(s_BindingRequestObjectVEPropertyName);
		if (property != null)
		{
			m_ElementsToBind.Add(ve);
		}
		int childCount = ve.hierarchy.childCount;
		for (int i = 0; i < childCount; i++)
		{
			VisualElement ve2 = ve.hierarchy[i];
			StartTrackingRecursive(ve2);
		}
	}

	private void StopTrackingRecursive(VisualElement ve)
	{
		StopTracking(ve);
		object property = ve.GetProperty(s_BindingRequestObjectVEPropertyName);
		if (property != null)
		{
			m_ElementsToBind.Remove(ve);
		}
		int childCount = ve.hierarchy.childCount;
		for (int i = 0; i < childCount; i++)
		{
			VisualElement ve2 = ve.hierarchy[i];
			StopTrackingRecursive(ve2);
		}
	}

	public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
	{
		base.OnVersionChanged(ve, versionChangeType);
		if ((versionChangeType & VersionChangeType.Bindings) == VersionChangeType.Bindings)
		{
			if (GetBindingObjectFromElement(ve) != null)
			{
				StartTracking(ve);
			}
			else
			{
				StopTracking(ve);
			}
			object property = ve.GetProperty(s_BindingRequestObjectVEPropertyName);
			if (property != null)
			{
				m_ElementsToBind.Add(ve);
			}
		}
	}

	protected override void OnHierarchyChange(VisualElement ve, HierarchyChangeType type)
	{
		switch (type)
		{
		case HierarchyChangeType.AddedToParent:
			StartTrackingRecursive(ve);
			break;
		case HierarchyChangeType.RemovedFromParent:
			StopTrackingRecursive(ve);
			break;
		}
	}

	private long CurrentTime()
	{
		return base.panel.TimeSinceStartupMs();
	}

	public bool ShouldProcessBindings(long startTime)
	{
		return disableBindingsThrottling || CurrentTime() - startTime < 100;
	}

	public void PerformTrackingOperations()
	{
		foreach (VisualElement item in m_ElementsToAdd)
		{
			IBinding bindingObjectFromElement = GetBindingObjectFromElement(item);
			if (bindingObjectFromElement != null)
			{
				m_ElementsWithBindings.Add(item);
			}
		}
		m_ElementsToAdd.Clear();
		foreach (VisualElement item2 in m_ElementsToRemove)
		{
			m_ElementsWithBindings.Remove(item2);
		}
		m_ElementsToRemove.Clear();
	}

	public override void Update()
	{
		base.Update();
		if (m_ElementsToBind.Count > 0)
		{
			using (s_ProfilerBindingRequestsMarker.Auto())
			{
				long startTime = CurrentTime();
				while (m_ElementsToBind.Count > 0 && ShouldProcessBindings(startTime))
				{
					VisualElement visualElement = m_ElementsToBind.FirstOrDefault();
					if (visualElement == null)
					{
						break;
					}
					m_ElementsToBind.Remove(visualElement);
					if (!(visualElement.GetProperty(s_BindingRequestObjectVEPropertyName) is List<IBindingRequest> list))
					{
						continue;
					}
					visualElement.SetProperty(s_BindingRequestObjectVEPropertyName, null);
					foreach (IBindingRequest item in list)
					{
						item.Bind(visualElement);
						item.Release();
					}
					ObjectListPool<IBindingRequest>.Release(list);
				}
			}
		}
		PerformTrackingOperations();
		if (m_ElementsWithBindings.Count > 0)
		{
			long num = CurrentTime();
			if (disableBindingsThrottling || m_LastUpdateTime + 100 < num)
			{
				UpdateBindings();
				m_LastUpdateTime = num;
			}
		}
		if (m_ElementsToBind.Count == 0)
		{
			temporaryObjectCache.Clear();
		}
	}

	private void UpdateBindings()
	{
		foreach (VisualElement elementsWithBinding in m_ElementsWithBindings)
		{
			IBinding bindingObjectFromElement = GetBindingObjectFromElement(elementsWithBinding);
			if (bindingObjectFromElement == null || elementsWithBinding.elementPanel != base.panel)
			{
				bindingObjectFromElement?.Release();
				StopTracking(elementsWithBinding);
			}
			else
			{
				updatedBindings.Add(bindingObjectFromElement);
			}
		}
		foreach (IBinding updatedBinding in updatedBindings)
		{
			updatedBinding.PreUpdate();
		}
		foreach (IBinding updatedBinding2 in updatedBindings)
		{
			updatedBinding2.Update();
		}
		updatedBindings.Clear();
	}

	internal override void PollElementsWithBindings(Action<VisualElement, IBinding> callback)
	{
		PerformTrackingOperations();
		if (m_ElementsWithBindings.Count <= 0)
		{
			return;
		}
		foreach (VisualElement elementsWithBinding in m_ElementsWithBindings)
		{
			IBinding bindingObjectFromElement = GetBindingObjectFromElement(elementsWithBinding);
			if (bindingObjectFromElement == null || elementsWithBinding.elementPanel != base.panel)
			{
				bindingObjectFromElement?.Release();
				StopTracking(elementsWithBinding);
			}
			else
			{
				callback(elementsWithBinding, bindingObjectFromElement);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		PerformTrackingOperations();
	}
}
