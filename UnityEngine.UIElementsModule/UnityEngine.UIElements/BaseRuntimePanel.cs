using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

internal abstract class BaseRuntimePanel : Panel
{
	private GameObject m_SelectableGameObject;

	private static int s_CurrentRuntimePanelCounter = 0;

	internal readonly int m_RuntimePanelCreationIndex;

	private float m_SortingPriority = 0f;

	internal int resolvedSortingIndex = 0;

	private bool m_DrawsInCameras;

	private float m_PixelsPerUnit = 100f;

	internal RenderTexture targetTexture = null;

	internal static readonly Func<Vector2, Vector3> DefaultScreenToPanelSpace = (Vector2 p) => p;

	private Func<Vector2, Vector3> m_ScreenToPanelSpace = DefaultScreenToPanelSpace;

	public GameObject selectableGameObject
	{
		get
		{
			return m_SelectableGameObject;
		}
		set
		{
			if (m_SelectableGameObject != value)
			{
				AssignPanelToComponents(null);
				m_SelectableGameObject = value;
				AssignPanelToComponents(this);
			}
		}
	}

	public float sortingPriority
	{
		get
		{
			return m_SortingPriority;
		}
		set
		{
			if (!Mathf.Approximately(m_SortingPriority, value))
			{
				m_SortingPriority = value;
				if (contextType == ContextType.Player)
				{
					UIElementsRuntimeUtility.SetPanelOrderingDirty();
				}
			}
		}
	}

	internal bool drawsInCameras
	{
		get
		{
			return m_DrawsInCameras;
		}
		set
		{
			if (m_DrawsInCameras != value)
			{
				m_DrawsInCameras = value;
				UIElementsRuntimeUtility.SetPanelsDrawInCameraDirty();
				InvokeDrawsInCamerasChanged();
			}
		}
	}

	internal float pixelsPerUnit
	{
		get
		{
			return m_PixelsPerUnit;
		}
		set
		{
			m_PixelsPerUnit = value;
		}
	}

	internal int targetDisplay { get; set; }

	internal int screenRenderingWidth => getScreenRenderingWidth(targetDisplay);

	internal int screenRenderingHeight => getScreenRenderingHeight(targetDisplay);

	public Func<Vector2, Vector3> screenToPanelSpace
	{
		get
		{
			return m_ScreenToPanelSpace;
		}
		set
		{
			m_ScreenToPanelSpace = value ?? DefaultScreenToPanelSpace;
		}
	}

	public event Action destroyed;

	internal event Action drawsInCamerasChanged;

	protected BaseRuntimePanel(ScriptableObject ownerObject, EventDispatcher dispatcher = null)
		: base(ownerObject, ContextType.Player, dispatcher)
	{
		m_RuntimePanelCreationIndex = s_CurrentRuntimePanelCounter++;
	}

	protected override void Dispose(bool disposing)
	{
		if (!base.disposed)
		{
			if (disposing)
			{
				this.destroyed?.Invoke();
			}
			base.Dispose(disposing);
		}
	}

	private void InvokeDrawsInCamerasChanged()
	{
		this.drawsInCamerasChanged?.Invoke();
	}

	internal virtual void Update()
	{
		using (m_MarkerTickScheduledActionsPreLayout.Auto())
		{
			scheduler.UpdateScheduledEvents();
			ValidateFocus();
		}
		ValidateLayout();
		using (m_MarkerTickScheduledActionsPostLayout.Auto())
		{
			UpdateAnimations();
			UpdateBindings();
		}
	}

	internal static int getScreenRenderingHeight(int display)
	{
		return (display >= 0 && display < Display.displays.Length) ? Display.displays[display].renderingHeight : Screen.height;
	}

	internal static int getScreenRenderingWidth(int display)
	{
		return (display >= 0 && display < Display.displays.Length) ? Display.displays[display].renderingWidth : Screen.width;
	}

	public override void Render()
	{
		if (drawsInCameras)
		{
			Debug.LogError("Panel.Render() must not be called on a panel that draws in cameras.");
		}
		else if (!(ownerObject == null))
		{
			if (targetTexture == null)
			{
				RenderTexture active = RenderTexture.active;
				int num = ((active != null) ? active.width : screenRenderingWidth);
				int num2 = ((active != null) ? active.height : screenRenderingHeight);
				GL.Viewport(new Rect(0f, 0f, num, num2));
				base.Render();
			}
			else
			{
				Camera current = Camera.current;
				RenderTexture active2 = RenderTexture.active;
				Camera.SetupCurrent(null);
				RenderTexture.active = targetTexture;
				GL.Viewport(new Rect(0f, 0f, targetTexture.width, targetTexture.height));
				base.Render();
				Camera.SetupCurrent(current);
				RenderTexture.active = active2;
			}
		}
	}

	internal Vector3 ScreenToPanel(Vector2 screen)
	{
		return screenToPanelSpace(screen) / base.scale;
	}

	internal bool ScreenToPanel(Vector2 screenPosition, Vector2 screenDelta, out Vector3 panelPosition, bool allowOutside = false)
	{
		panelPosition = ScreenToPanel(screenPosition);
		if (!allowOutside)
		{
			Rect layout = visualTree.layout;
			if (!layout.Contains(panelPosition))
			{
				return false;
			}
			Vector3 point = ScreenToPanel(screenPosition - screenDelta);
			if (!layout.Contains(point))
			{
				return true;
			}
		}
		return true;
	}

	private void AssignPanelToComponents(BaseRuntimePanel panel)
	{
		if (selectableGameObject == null)
		{
			return;
		}
		List<IRuntimePanelComponent> value;
		using (CollectionPool<List<IRuntimePanelComponent>, IRuntimePanelComponent>.Get(out value))
		{
			selectableGameObject.GetComponents(value);
			foreach (IRuntimePanelComponent item in value)
			{
				item.panel = panel;
			}
		}
	}

	internal void PointerLeavesPanel(int pointerId)
	{
		ClearCachedElementUnderPointer(pointerId, null);
		CommitElementUnderPointers();
		PointerDeviceState.SavePointerPosition(pointerId, BaseVisualElementPanel.s_OutsidePanelCoordinates, null, contextType);
	}

	internal void PointerEntersPanel(int pointerId, Vector3 position)
	{
		PointerDeviceState.SavePointerPosition(pointerId, position, this, contextType);
	}
}
