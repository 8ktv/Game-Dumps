#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine.UIElements.Layout;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

internal static class UIElementsRuntimeUtility
{
	public delegate BaseRuntimePanel CreateRuntimePanelDelegate(ScriptableObject ownerObject);

	internal static Func<bool> IsEditingPrefab;

	private static bool s_RegisteredPlayerloopCallback;

	private static readonly List<BaseRuntimePanel> s_SortedScreenOverlayPanels;

	private static readonly List<BaseRuntimePanel> s_CachedWorldSpacePanels;

	private static readonly List<BaseRuntimePanel> s_SortedPlayerPanels;

	private static bool s_PanelOrderingOrDrawInCameraDirty;

	internal static int s_ResolvedSortingIndexMax;

	private static int currentOverlayIndex;

	private static DefaultEventSystem s_DefaultEventSystem;

	private static List<PanelSettings> s_PotentiallyEmptyPanelSettings;

	internal static Object activeEventSystem { get; private set; }

	internal static bool useDefaultEventSystem => overrideUseDefaultEventSystem ?? (activeEventSystem == null);

	internal static bool? overrideUseDefaultEventSystem { get; set; }

	internal static bool autoUpdateEventSystem { get; set; }

	internal static DefaultEventSystem defaultEventSystem => s_DefaultEventSystem ?? (s_DefaultEventSystem = new DefaultEventSystem());

	public static event Action<BaseRuntimePanel> onCreatePanel;

	static UIElementsRuntimeUtility()
	{
		IsEditingPrefab = null;
		s_RegisteredPlayerloopCallback = false;
		s_SortedScreenOverlayPanels = new List<BaseRuntimePanel>();
		s_CachedWorldSpacePanels = new List<BaseRuntimePanel>();
		s_SortedPlayerPanels = new List<BaseRuntimePanel>();
		s_PanelOrderingOrDrawInCameraDirty = true;
		s_ResolvedSortingIndexMax = 0;
		currentOverlayIndex = -1;
		autoUpdateEventSystem = true;
		s_PotentiallyEmptyPanelSettings = new List<PanelSettings>();
		Canvas.externBeginRenderOverlays = BeginRenderOverlays;
		Canvas.externRenderOverlaysBefore = delegate(int displayIndex, int sortOrder)
		{
			RenderOverlaysBeforePriority(displayIndex, sortOrder);
		};
		Canvas.externEndRenderOverlays = EndRenderOverlays;
		UIElementsRuntimeUtilityNative.SetUpdateCallback(UpdatePanels);
	}

	public static EventBase CreateEvent(Event systemEvent)
	{
		return UIElementsUtility.CreateEvent(systemEvent, systemEvent.rawType);
	}

	public static BaseRuntimePanel FindOrCreateRuntimePanel(ScriptableObject ownerObject, CreateRuntimePanelDelegate createDelegate)
	{
		if (UIElementsUtility.TryGetPanel(ownerObject.GetInstanceID(), out var panel))
		{
			if (panel is BaseRuntimePanel result)
			{
				return result;
			}
			RemoveCachedPanelInternal(ownerObject.GetInstanceID());
		}
		BaseRuntimePanel baseRuntimePanel = createDelegate(ownerObject);
		baseRuntimePanel.IMGUIEventInterests = new EventInterests
		{
			wantsMouseMove = true,
			wantsMouseEnterLeaveWindow = true
		};
		RegisterCachedPanelInternal(ownerObject.GetInstanceID(), baseRuntimePanel);
		UIElementsRuntimeUtility.onCreatePanel?.Invoke(baseRuntimePanel);
		return baseRuntimePanel;
	}

	public static void DisposeRuntimePanel(ScriptableObject ownerObject)
	{
		if (UIElementsUtility.TryGetPanel(ownerObject.GetInstanceID(), out var panel))
		{
			panel.Dispose();
			RemoveCachedPanelInternal(ownerObject.GetInstanceID());
		}
	}

	private static void GetPlayerPanelsByRenderMode(List<BaseRuntimePanel> outScreenSpaceOverlayPanels, List<BaseRuntimePanel> outWorldSpacePanels)
	{
		List<Panel> value;
		using (CollectionPool<List<Panel>, Panel>.Get(out value))
		{
			UIElementsUtility.GetAllPanels(value, ContextType.Player);
			foreach (Panel item in value)
			{
				if (item is BaseRuntimePanel baseRuntimePanel)
				{
					if (baseRuntimePanel.drawsInCameras)
					{
						outWorldSpacePanels.Add(baseRuntimePanel);
					}
					else
					{
						outScreenSpaceOverlayPanels.Add(baseRuntimePanel);
					}
				}
			}
		}
	}

	private static void RegisterCachedPanelInternal(int instanceID, IPanel panel)
	{
		UIElementsUtility.RegisterCachedPanel(instanceID, panel as Panel);
		s_PanelOrderingOrDrawInCameraDirty = true;
		if (!s_RegisteredPlayerloopCallback)
		{
			s_RegisteredPlayerloopCallback = true;
			EnableRenderingAndInputCallbacks();
			Canvas.SetExternalCanvasEnabled(enabled: true);
		}
	}

	private static void RemoveCachedPanelInternal(int instanceID)
	{
		UIElementsUtility.RemoveCachedPanel(instanceID);
		s_PanelOrderingOrDrawInCameraDirty = true;
		List<Panel> value;
		using (CollectionPool<List<Panel>, Panel>.Get(out value))
		{
			UIElementsUtility.GetAllPanels(value, ContextType.Player);
			if (value.Count == 0)
			{
				SortPanels();
				s_RegisteredPlayerloopCallback = false;
				DisableRenderingAndInputCallbacks();
				Canvas.SetExternalCanvasEnabled(enabled: false);
			}
		}
	}

	public static void RenderOffscreenPanels()
	{
		Camera current = Camera.current;
		RenderTexture active = RenderTexture.active;
		foreach (BaseRuntimePanel sortedScreenOverlayPlayerPanel in GetSortedScreenOverlayPlayerPanels())
		{
			if (sortedScreenOverlayPlayerPanel.targetTexture != null)
			{
				RenderPanel(sortedScreenOverlayPlayerPanel, restoreState: false);
			}
		}
		Camera.SetupCurrent(current);
		RenderTexture.active = active;
	}

	public static void RepaintPanel(BaseRuntimePanel panel)
	{
		Camera current = Camera.current;
		RenderTexture active = RenderTexture.active;
		panel.Repaint(Event.current);
		Camera.SetupCurrent(current);
		RenderTexture.active = active;
	}

	public static void RenderPanel(BaseRuntimePanel panel, bool restoreState = true)
	{
		Debug.Assert(!panel.drawsInCameras);
		Camera current = Camera.current;
		RenderTexture active = RenderTexture.active;
		panel.Render();
		if (!panel.drawsInCameras && restoreState)
		{
			Camera.SetupCurrent(current);
			RenderTexture.active = active;
		}
	}

	internal static void BeginRenderOverlays(int displayIndex)
	{
		currentOverlayIndex = 0;
	}

	internal static void RenderOverlaysBeforePriority(int displayIndex, float maxPriority)
	{
		if (currentOverlayIndex < 0)
		{
			return;
		}
		List<BaseRuntimePanel> sortedScreenOverlayPlayerPanels = GetSortedScreenOverlayPlayerPanels();
		while (currentOverlayIndex < sortedScreenOverlayPlayerPanels.Count)
		{
			BaseRuntimePanel baseRuntimePanel = sortedScreenOverlayPlayerPanels[currentOverlayIndex];
			if (baseRuntimePanel.sortingPriority >= maxPriority)
			{
				break;
			}
			if (baseRuntimePanel.targetDisplay == displayIndex && baseRuntimePanel.targetTexture == null)
			{
				RenderPanel(baseRuntimePanel);
			}
			currentOverlayIndex++;
		}
	}

	internal static void EndRenderOverlays(int displayIndex)
	{
		RenderOverlaysBeforePriority(displayIndex, float.MaxValue);
		currentOverlayIndex = -1;
	}

	public static void RepaintPanels(bool onlyOffscreen)
	{
		foreach (BaseRuntimePanel sortedPlayerPanel in GetSortedPlayerPanels())
		{
			if (!onlyOffscreen || sortedPlayerPanel.targetTexture != null)
			{
				RepaintPanel(sortedPlayerPanel);
			}
		}
	}

	public static void RegisterEventSystem(Object eventSystem)
	{
		if (activeEventSystem != null && activeEventSystem != eventSystem && eventSystem.GetType().Name == "EventSystem")
		{
			Debug.LogWarning("There can be only one active Event System.");
		}
		activeEventSystem = eventSystem;
	}

	public static void UnregisterEventSystem(Object eventSystem)
	{
		if (activeEventSystem == eventSystem)
		{
			activeEventSystem = null;
		}
	}

	public static void UpdatePanels()
	{
		RemoveUnusedPanels();
		UIRenderDevice.ProcessDeviceFreeQueue();
		if (LayoutManager.IsSharedManagerCreated)
		{
			LayoutManager.SharedManager.Collect();
		}
		List<BaseRuntimePanel> sortedPlayerPanels = GetSortedPlayerPanels();
		if (sortedPlayerPanels.Count == 0)
		{
			return;
		}
		foreach (BaseRuntimePanel item in sortedPlayerPanels)
		{
			item.Update();
		}
		UpdateEventSystem();
	}

	internal static void UpdateEventSystem()
	{
		if (useDefaultEventSystem)
		{
			defaultEventSystem.isInputReady = true;
			if (autoUpdateEventSystem)
			{
				defaultEventSystem.Update(DefaultEventSystem.UpdateMode.IgnoreIfAppNotFocused);
			}
		}
		else if (s_DefaultEventSystem != null)
		{
			s_DefaultEventSystem.isInputReady = false;
		}
	}

	internal static void MarkPotentiallyEmpty(PanelSettings settings)
	{
		if (!s_PotentiallyEmptyPanelSettings.Contains(settings))
		{
			s_PotentiallyEmptyPanelSettings.Add(settings);
		}
	}

	internal static void RemoveUnusedPanels()
	{
		foreach (PanelSettings s_PotentiallyEmptyPanelSetting in s_PotentiallyEmptyPanelSettings)
		{
			UIDocumentList attachedUIDocumentsList = s_PotentiallyEmptyPanelSetting.m_AttachedUIDocumentsList;
			if (attachedUIDocumentsList == null || attachedUIDocumentsList.m_AttachedUIDocuments.Count == 0)
			{
				s_PotentiallyEmptyPanelSetting.DisposePanel();
			}
		}
		s_PotentiallyEmptyPanelSettings.Clear();
	}

	public static void EnableRenderingAndInputCallbacks()
	{
		UIElementsRuntimeUtilityNative.SetRenderingCallbacks(RepaintPanels, RenderOffscreenPanels);
	}

	public static void DisableRenderingAndInputCallbacks()
	{
		UIElementsRuntimeUtilityNative.UnsetRenderingCallbacks();
		if (s_DefaultEventSystem != null)
		{
			s_DefaultEventSystem.isInputReady = false;
		}
	}

	internal static void SetPanelOrderingDirty()
	{
		s_PanelOrderingOrDrawInCameraDirty = true;
	}

	internal static void SetPanelsDrawInCameraDirty()
	{
		s_PanelOrderingOrDrawInCameraDirty = true;
	}

	internal static List<BaseRuntimePanel> GetWorldSpacePlayerPanels()
	{
		if (s_PanelOrderingOrDrawInCameraDirty)
		{
			SortPanels();
		}
		return s_CachedWorldSpacePanels;
	}

	public static List<BaseRuntimePanel> GetSortedScreenOverlayPlayerPanels()
	{
		if (s_PanelOrderingOrDrawInCameraDirty)
		{
			SortPanels();
		}
		return s_SortedScreenOverlayPanels;
	}

	public static List<BaseRuntimePanel> GetSortedPlayerPanels()
	{
		if (s_PanelOrderingOrDrawInCameraDirty)
		{
			SortPanels();
		}
		return s_SortedPlayerPanels;
	}

	internal static List<IPanel> GetSortedPlayerPanelsInternal()
	{
		List<IPanel> list = new List<IPanel>();
		foreach (BaseRuntimePanel sortedPlayerPanel in GetSortedPlayerPanels())
		{
			list.Add(sortedPlayerPanel);
		}
		return list;
	}

	private static void SortPanels()
	{
		s_SortedScreenOverlayPanels.Clear();
		s_CachedWorldSpacePanels.Clear();
		GetPlayerPanelsByRenderMode(s_SortedScreenOverlayPanels, s_CachedWorldSpacePanels);
		s_SortedScreenOverlayPanels.Sort(delegate(BaseRuntimePanel runtimePanelA, BaseRuntimePanel runtimePanelB)
		{
			if (runtimePanelA == null || runtimePanelB == null)
			{
				return 0;
			}
			float num2 = runtimePanelA.sortingPriority - runtimePanelB.sortingPriority;
			return Mathf.Approximately(0f, num2) ? runtimePanelA.m_RuntimePanelCreationIndex.CompareTo(runtimePanelB.m_RuntimePanelCreationIndex) : ((!(num2 < 0f)) ? 1 : (-1));
		});
		for (int num = 0; num < s_SortedScreenOverlayPanels.Count; num++)
		{
			BaseRuntimePanel baseRuntimePanel = s_SortedScreenOverlayPanels[num];
			baseRuntimePanel.resolvedSortingIndex = num;
		}
		s_ResolvedSortingIndexMax = s_SortedScreenOverlayPanels.Count - 1;
		s_SortedPlayerPanels.Clear();
		foreach (BaseRuntimePanel s_CachedWorldSpacePanel in s_CachedWorldSpacePanels)
		{
			s_SortedPlayerPanels.Add(s_CachedWorldSpacePanel);
		}
		foreach (BaseRuntimePanel s_SortedScreenOverlayPanel in s_SortedScreenOverlayPanels)
		{
			s_SortedPlayerPanels.Add(s_SortedScreenOverlayPanel);
		}
		s_PanelOrderingOrDrawInCameraDirty = false;
	}

	internal static Vector2 MultiDisplayBottomLeftToPanelPosition(Vector2 position, out int? targetDisplay)
	{
		Vector2 position2 = MultiDisplayToLocalScreenPosition(position, out targetDisplay);
		return ScreenBottomLeftToPanelPosition(position2, targetDisplay.GetValueOrDefault());
	}

	internal static Vector2 MultiDisplayToLocalScreenPosition(Vector2 position, out int? targetDisplay)
	{
		Vector3 vector = Display.RelativeMouseAt(position);
		if (vector != Vector3.zero)
		{
			targetDisplay = (int)vector.z;
			return vector;
		}
		targetDisplay = null;
		return position;
	}

	internal static Vector2 ScreenBottomLeftToPanelPosition(Vector2 position, int targetDisplay)
	{
		return FlipY(position, GetRuntimeDisplayHeight(targetDisplay));
	}

	internal static Vector2 ScreenBottomLeftToPanelDelta(Vector2 delta)
	{
		return FlipDeltaY(delta);
	}

	internal static Vector2 PanelToScreenBottomLeftPosition(Vector2 panelPosition, int targetDisplay)
	{
		return FlipY(panelPosition, GetRuntimeDisplayHeight(targetDisplay));
	}

	internal static Vector2 FlipY(Vector2 p, float displayHeight)
	{
		p.y = displayHeight - p.y;
		return p;
	}

	private static Vector2 FlipDeltaY(Vector2 delta)
	{
		delta.y = 0f - delta.y;
		return delta;
	}

	private static float GetRuntimeDisplayHeight(int targetDisplay)
	{
		if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
		{
			return Display.displays[targetDisplay].systemHeight;
		}
		return Screen.height;
	}

	internal static float GetEditorDisplayHeight(int targetDisplay)
	{
		return GetRuntimeDisplayHeight(targetDisplay);
	}
}
