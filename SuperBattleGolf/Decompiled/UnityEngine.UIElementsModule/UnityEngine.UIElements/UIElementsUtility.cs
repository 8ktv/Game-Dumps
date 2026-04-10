#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
internal class UIElementsUtility : IUIElementsUtility
{
	private static Stack<IMGUIContainer> s_ContainerStack = new Stack<IMGUIContainer>();

	private static Dictionary<int, Panel> s_UIElementsCache = new Dictionary<int, Panel>();

	private static Event s_EventInstance = new Event();

	internal static Color editorPlayModeTintColor = Color.white;

	internal static float singleLineHeight = 18f;

	public const string hiddenClassName = "unity-hidden";

	internal static bool s_EnableOSXContextualMenuEventsOnNonOSXPlatforms;

	private static UIElementsUtility s_Instance = new UIElementsUtility();

	internal static List<Panel> s_PanelsIterationList = new List<Panel>();

	internal const int kTestFrameUpdateEvent = 7777;

	private static Action testFrameUpdateCallback;

	internal static readonly string s_RepaintProfilerMarkerName = "UIElementsUtility.DoDispatch(Repaint Event)";

	internal static readonly string s_EventProfilerMarkerName = "UIElementsUtility.DoDispatch(Non Repaint Event)";

	private static readonly ProfilerMarker s_RepaintProfilerMarker = new ProfilerMarker(s_RepaintProfilerMarkerName);

	private static readonly ProfilerMarker s_EventProfilerMarker = new ProfilerMarker(s_EventProfilerMarkerName);

	internal static char[] s_Modifiers = new char[5] { '&', '%', '^', '#', '_' };

	internal static readonly HashSet<StyleSheet> s_StyleSheetsRequiringRebuilding = new HashSet<StyleSheet>();

	internal static readonly HashSet<string> s_ReimportedStyleSheetsPath = new HashSet<string>();

	internal static readonly List<StyleSheet> s_StyleSheetsRebuildList = new List<StyleSheet>();

	internal static readonly List<string> s_ReimportedStyleSheetsPathList = new List<string>();

	public static bool isOSXContextualMenuPlatform
	{
		get
		{
			RuntimePlatform platform = Application.platform;
			return platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer || s_EnableOSXContextualMenuEventsOnNonOSXPlatforms;
		}
	}

	internal static void EnableOSXContextualMenuEventsOnNonOSXPlatforms()
	{
		s_EnableOSXContextualMenuEventsOnNonOSXPlatforms = true;
	}

	internal static void ResetOSXContextualMenuEventsOnNonOSXPlatforms()
	{
		s_EnableOSXContextualMenuEventsOnNonOSXPlatforms = false;
	}

	private UIElementsUtility()
	{
		UIEventRegistration.RegisterUIElementSystem(this);
	}

	internal static IMGUIContainer GetCurrentIMGUIContainer()
	{
		if (s_ContainerStack.Count > 0)
		{
			return s_ContainerStack.Peek();
		}
		return null;
	}

	bool IUIElementsUtility.MakeCurrentIMGUIContainerDirty()
	{
		if (s_ContainerStack.Count > 0)
		{
			s_ContainerStack.Peek().MarkDirtyLayout();
			return true;
		}
		return false;
	}

	bool IUIElementsUtility.TakeCapture()
	{
		if (s_ContainerStack.Count > 0)
		{
			IMGUIContainer handler = s_ContainerStack.Peek();
			handler.CaptureMouse();
			return true;
		}
		return false;
	}

	bool IUIElementsUtility.ReleaseCapture()
	{
		return false;
	}

	bool IUIElementsUtility.ProcessEvent(int instanceID, IntPtr nativeEventPtr, ref bool eventHandled)
	{
		if (nativeEventPtr != IntPtr.Zero && s_UIElementsCache.TryGetValue(instanceID, out var value))
		{
			if (value.contextType == ContextType.Editor)
			{
				s_EventInstance.CopyFromPtr(nativeEventPtr);
				if ((EventType)7777 == s_EventInstance.type)
				{
					Action action = testFrameUpdateCallback;
					testFrameUpdateCallback = null;
					action?.Invoke();
					eventHandled = true;
					return true;
				}
				using (new IMGUIContainer.UITKScope())
				{
					eventHandled = DoDispatch(value);
				}
			}
			return true;
		}
		return false;
	}

	bool IUIElementsUtility.CleanupRoots()
	{
		s_EventInstance = null;
		s_UIElementsCache = null;
		s_ContainerStack = null;
		return false;
	}

	bool IUIElementsUtility.EndContainerGUIFromException(Exception exception)
	{
		if (s_ContainerStack.Count > 0)
		{
			GUIUtility.EndContainer();
			s_ContainerStack.Pop();
		}
		return false;
	}

	void IUIElementsUtility.UpdateSchedulers()
	{
		s_PanelsIterationList.Clear();
		GetAllPanels(s_PanelsIterationList, ContextType.Editor);
		if (LayoutManager.IsSharedManagerCreated)
		{
			LayoutManager.SharedManager.Collect();
		}
		foreach (Panel s_PanelsIteration in s_PanelsIterationList)
		{
			s_PanelsIteration.TickSchedulingUpdaters();
		}
	}

	public static Event CreateTestFrameUpdateEvent(Action callback)
	{
		testFrameUpdateCallback = callback;
		Event obj = new Event();
		obj.type = (EventType)7777;
		return obj;
	}

	void IUIElementsUtility.RequestRepaintForPanels(Action<ScriptableObject> repaintCallback)
	{
		Dictionary<int, Panel>.Enumerator panelsIterator = GetPanelsIterator();
		while (panelsIterator.MoveNext())
		{
			Panel value = panelsIterator.Current.Value;
			if (value.contextType == ContextType.Editor && value.isDirty)
			{
				repaintCallback(value.ownerObject);
			}
		}
		TextGenerationInfo.OnRepaintEnd();
	}

	public static void RegisterCachedPanel(int instanceID, Panel panel)
	{
		s_UIElementsCache.Add(instanceID, panel);
	}

	public static void RemoveCachedPanel(int instanceID)
	{
		s_UIElementsCache.Remove(instanceID);
	}

	public static bool TryGetPanel(int instanceID, out Panel panel)
	{
		return s_UIElementsCache.TryGetValue(instanceID, out panel);
	}

	internal static void BeginContainerGUI(GUILayoutUtility.LayoutCache cache, Event evt, IMGUIContainer container)
	{
		if (container.useOwnerObjectGUIState)
		{
			GUIUtility.BeginContainerFromOwner(container.elementPanel.ownerObject);
		}
		else
		{
			GUIUtility.BeginContainer(container.guiState);
		}
		s_ContainerStack.Push(container);
		GUIUtility.s_SkinMode = (int)container.contextType;
		GUIUtility.s_OriginalID = container.elementPanel.ownerObject.GetInstanceID();
		if (Event.current == null)
		{
			Event.current = evt;
		}
		else
		{
			Event.current.CopyFrom(evt);
		}
		GUI.enabled = container.enabledInHierarchy;
		GUILayoutUtility.BeginContainer(cache);
		GUIUtility.ResetGlobalState();
	}

	internal static void EndContainerGUI(Event evt, Rect layoutSize)
	{
		if (Event.current.type == EventType.Layout && s_ContainerStack.Count > 0)
		{
			GUILayoutUtility.LayoutFromContainer(layoutSize.width, layoutSize.height);
		}
		GUILayoutUtility.SelectIDList(GUIUtility.s_OriginalID, isWindow: false);
		GUIContent.ClearStaticCache();
		if (s_ContainerStack.Count > 0)
		{
		}
		evt.CopyFrom(Event.current);
		if (s_ContainerStack.Count > 0)
		{
			GUIUtility.EndContainer();
			s_ContainerStack.Pop();
		}
	}

	internal static EventBase CreateEvent(Event systemEvent)
	{
		return CreateEvent(systemEvent, systemEvent.rawType);
	}

	internal static EventBase CreateEvent(Event systemEvent, EventType eventType)
	{
		switch (eventType)
		{
		case EventType.MouseMove:
		case EventType.TouchMove:
			return PointerEventBase<PointerMoveEvent>.GetPooled(systemEvent);
		case EventType.MouseDrag:
			return PointerEventBase<PointerMoveEvent>.GetPooled(systemEvent);
		case EventType.MouseDown:
		case EventType.TouchDown:
			if (PointerDeviceState.HasAdditionalPressedButtons(PointerId.mousePointerId, systemEvent.button))
			{
				return PointerEventBase<PointerMoveEvent>.GetPooled(systemEvent);
			}
			return PointerEventBase<PointerDownEvent>.GetPooled(systemEvent);
		case EventType.MouseUp:
		case EventType.TouchUp:
			if (PointerDeviceState.HasAdditionalPressedButtons(PointerId.mousePointerId, systemEvent.button))
			{
				return PointerEventBase<PointerMoveEvent>.GetPooled(systemEvent);
			}
			return PointerEventBase<PointerUpEvent>.GetPooled(systemEvent);
		case EventType.ContextClick:
			return MouseEventBase<ContextClickEvent>.GetPooled(systemEvent);
		case EventType.MouseEnterWindow:
			return MouseEventBase<MouseEnterWindowEvent>.GetPooled(systemEvent);
		case EventType.MouseLeaveWindow:
			return MouseLeaveWindowEvent.GetPooled(systemEvent);
		case EventType.ScrollWheel:
			return WheelEvent.GetPooled(systemEvent);
		case EventType.KeyDown:
			return KeyboardEventBase<KeyDownEvent>.GetPooled(systemEvent);
		case EventType.KeyUp:
			return KeyboardEventBase<KeyUpEvent>.GetPooled(systemEvent);
		case EventType.ValidateCommand:
			return CommandEventBase<ValidateCommandEvent>.GetPooled(systemEvent);
		case EventType.ExecuteCommand:
			return CommandEventBase<ExecuteCommandEvent>.GetPooled(systemEvent);
		default:
			return IMGUIEvent.GetPooled(systemEvent);
		}
	}

	private static bool DoDispatch(BaseVisualElementPanel panel)
	{
		Debug.Assert(panel.contextType == ContextType.Editor, "panel.contextType == ContextType.Editor");
		bool result = false;
		if (s_EventInstance.type == EventType.Repaint)
		{
			Camera current = Camera.current;
			RenderTexture active = RenderTexture.active;
			Camera.SetupCurrent(null);
			RenderTexture.active = null;
			using (s_RepaintProfilerMarker.Auto())
			{
				panel.Repaint(s_EventInstance);
				panel.Render();
			}
			result = panel.IMGUIContainersCount > 0;
			Camera.SetupCurrent(current);
			RenderTexture.active = active;
		}
		else
		{
			panel.ValidateLayout();
			using EventBase eventBase = CreateEvent(s_EventInstance);
			bool flag = s_EventInstance.type == EventType.Used || s_EventInstance.type == EventType.Layout || s_EventInstance.type == EventType.ExecuteCommand || s_EventInstance.type == EventType.ValidateCommand;
			using (s_EventProfilerMarker.Auto())
			{
				panel.SendEvent(eventBase, (!flag) ? DispatchMode.Default : DispatchMode.Immediate);
			}
			if (eventBase.isPropagationStopped)
			{
				panel.visualTree.IncrementVersion(VersionChangeType.Repaint);
				result = true;
			}
		}
		return result;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal static void GetAllPanels(List<Panel> panels, ContextType contextType)
	{
		Dictionary<int, Panel>.Enumerator panelsIterator = GetPanelsIterator();
		while (panelsIterator.MoveNext())
		{
			if (panelsIterator.Current.Value.contextType == contextType)
			{
				panels.Add(panelsIterator.Current.Value);
			}
		}
	}

	internal static Dictionary<int, Panel>.Enumerator GetPanelsIterator()
	{
		return s_UIElementsCache.GetEnumerator();
	}

	internal static float PixelsPerUnitScaleForElement(VisualElement ve, Sprite sprite)
	{
		if (ve == null || ve.elementPanel == null || sprite == null)
		{
			return 1f;
		}
		float referenceSpritePixelsPerUnit = ve.elementPanel.referenceSpritePixelsPerUnit;
		float pixelsPerUnit = sprite.pixelsPerUnit;
		pixelsPerUnit = Mathf.Max(0.01f, pixelsPerUnit);
		return referenceSpritePixelsPerUnit / pixelsPerUnit;
	}

	internal static string ParseMenuName(string menuName)
	{
		if (string.IsNullOrEmpty(menuName))
		{
			return string.Empty;
		}
		string text = menuName.TrimEnd();
		int num = text.LastIndexOf(' ');
		if (num > -1)
		{
			int num2 = Array.IndexOf(s_Modifiers, text[num + 1]);
			if (text.Length > num + 1 && num2 > -1)
			{
				text = text.Substring(0, num).TrimEnd();
			}
		}
		return text;
	}

	internal static void MarkStyleSheetAsChanged(StyleSheet styleSheet)
	{
		if ((bool)styleSheet)
		{
			s_StyleSheetsRequiringRebuilding.Add(styleSheet);
		}
	}

	internal static void MarkStyleSheetAsChanged(string styleSheetPath)
	{
		if (!string.IsNullOrEmpty(styleSheetPath))
		{
			s_ReimportedStyleSheetsPath.Add(styleSheetPath);
		}
	}

	internal static void RebuildDirtyStyleSheets()
	{
		if (s_StyleSheetsRequiringRebuilding.Count == 0)
		{
			return;
		}
		try
		{
			StyleCache.ClearStyleCache();
			s_StyleSheetsRebuildList.AddRange(s_StyleSheetsRequiringRebuilding);
			s_ReimportedStyleSheetsPathList.AddRange(s_ReimportedStyleSheetsPath);
			foreach (StyleSheet s_StyleSheetsRebuild in s_StyleSheetsRebuildList)
			{
				s_StyleSheetsRebuild.RebuildIfNecessary();
			}
		}
		finally
		{
			s_StyleSheetsRebuildList.Clear();
			s_StyleSheetsRequiringRebuilding.Clear();
			s_ReimportedStyleSheetsPathList.Clear();
			s_ReimportedStyleSheetsPath.Clear();
		}
	}
}
