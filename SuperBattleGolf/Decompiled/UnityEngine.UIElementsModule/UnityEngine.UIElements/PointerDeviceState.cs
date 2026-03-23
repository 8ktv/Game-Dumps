#define UNITY_ASSERTIONS
using System;

namespace UnityEngine.UIElements;

internal static class PointerDeviceState
{
	[Flags]
	internal enum LocationFlag
	{
		None = 0,
		OutsidePanel = 1
	}

	private struct PointerLocation
	{
		internal Vector3 Position { get; private set; }

		internal IPanel Panel { get; private set; }

		internal LocationFlag Flags { get; private set; }

		internal void SetLocation(Vector3 position, IPanel panel)
		{
			Position = position;
			Panel = panel;
			Flags = LocationFlag.None;
			if (panel != null)
			{
				BaseVisualElementPanel obj = panel as BaseVisualElementPanel;
				if (obj == null || !obj.isFlat || panel.visualTree.layout.Contains(position))
				{
					return;
				}
			}
			Flags |= LocationFlag.OutsidePanel;
		}
	}

	public class RuntimePointerState
	{
		public struct RaycastHit
		{
			public float distance;

			public Collider collider;

			public UIDocument document;

			public VisualElement element;
		}

		public RaycastHit hit;

		public int updateFrameCount = 0;

		public virtual void Reset()
		{
			hit = default(RaycastHit);
			updateFrameCount = 0;
		}
	}

	public class ScreenPointerState : RuntimePointerState
	{
		public Vector2 mousePosition;

		public int? targetDisplay;

		public override void Reset()
		{
			base.Reset();
			mousePosition = Vector2.zero;
			targetDisplay = null;
		}
	}

	public class TrackedPointerState : RuntimePointerState
	{
		public Vector3 worldPosition = Vector3.zero;

		public Quaternion worldOrientation = Quaternion.identity;

		public float maxDistance = float.PositiveInfinity;

		public Ray worldRay => new Ray(worldPosition, worldOrientation * Vector3.forward);

		public override void Reset()
		{
			base.Reset();
			worldPosition = Vector3.zero;
			worldOrientation = Quaternion.identity;
			maxDistance = float.PositiveInfinity;
		}
	}

	private static RuntimePointerState[] s_RuntimePointerStates = new RuntimePointerState[PointerId.maxPointers];

	private static PointerLocation[] s_PlayerPointerLocations = new PointerLocation[PointerId.maxPointers];

	private static int[] s_PressedButtons = new int[PointerId.maxPointers];

	private static readonly RuntimePanel[] s_PlayerPanelWithSoftPointerCapture = new RuntimePanel[PointerId.maxPointers];

	private static readonly UIDocument[] s_WorldSpaceDocumentWithSoftPointerCapture = new UIDocument[PointerId.maxPointers];

	private static readonly Camera[] s_CameraWithSoftPointerCapture = new Camera[PointerId.maxPointers];

	internal static void Reset()
	{
		for (int i = 0; i < PointerId.maxPointers; i++)
		{
			s_PlayerPointerLocations[i].SetLocation(Vector2.zero, null);
			s_PressedButtons[i] = 0;
			s_PlayerPanelWithSoftPointerCapture[i] = null;
			if (s_WorldSpaceDocumentWithSoftPointerCapture[i] != null)
			{
				s_WorldSpaceDocumentWithSoftPointerCapture[i].softPointerCaptures = 0;
				s_WorldSpaceDocumentWithSoftPointerCapture[i] = null;
			}
		}
		for (int j = 0; j < PointerId.maxPointers; j++)
		{
			s_RuntimePointerStates[j]?.Reset();
		}
	}

	internal static void RemovePanelData(IPanel panel)
	{
		for (int i = 0; i < PointerId.maxPointers; i++)
		{
			if (s_PlayerPointerLocations[i].Panel == panel)
			{
				s_PlayerPointerLocations[i].SetLocation(Vector2.zero, null);
			}
			if (s_PlayerPanelWithSoftPointerCapture[i] == panel)
			{
				s_PlayerPanelWithSoftPointerCapture[i] = null;
				if (s_WorldSpaceDocumentWithSoftPointerCapture[i] != null)
				{
					s_WorldSpaceDocumentWithSoftPointerCapture[i].softPointerCaptures = 0;
					s_WorldSpaceDocumentWithSoftPointerCapture[i] = null;
				}
			}
		}
	}

	internal static void RemoveDocumentData(UIDocument document)
	{
		if (document.softPointerCaptures == 0)
		{
			return;
		}
		for (int i = 0; i < PointerId.maxPointers; i++)
		{
			if (s_WorldSpaceDocumentWithSoftPointerCapture[i] == document)
			{
				s_WorldSpaceDocumentWithSoftPointerCapture[i].softPointerCaptures = 0;
				s_WorldSpaceDocumentWithSoftPointerCapture[i] = null;
			}
		}
	}

	public static void SavePointerPosition(int pointerId, Vector3 position, IPanel panel, ContextType contextType)
	{
		if ((uint)contextType > 1u)
		{
		}
		s_PlayerPointerLocations[pointerId].SetLocation(position, panel);
	}

	public static void PressButton(int pointerId, int buttonId)
	{
		Debug.Assert(buttonId >= 0, "PressButton expects buttonId >= 0");
		Debug.Assert(buttonId < 32, "PressButton expects buttonId < 32");
		s_PressedButtons[pointerId] |= 1 << buttonId;
	}

	public static void ReleaseButton(int pointerId, int buttonId)
	{
		Debug.Assert(buttonId >= 0, "ReleaseButton expects buttonId >= 0");
		Debug.Assert(buttonId < 32, "ReleaseButton expects buttonId < 32");
		s_PressedButtons[pointerId] &= ~(1 << buttonId);
	}

	public static void ReleaseAllButtons(int pointerId)
	{
		s_PressedButtons[pointerId] = 0;
	}

	public static Vector3 GetPointerPosition(int pointerId, ContextType contextType)
	{
		if ((uint)contextType > 1u)
		{
		}
		return s_PlayerPointerLocations[pointerId].Position;
	}

	public static Vector3 GetPointerDeltaPosition(int pointerId, ContextType contextType, Vector3 newPosition)
	{
		if ((uint)contextType > 1u)
		{
		}
		if (s_PlayerPointerLocations[pointerId].Panel == null)
		{
			return Vector3.zero;
		}
		return newPosition - s_PlayerPointerLocations[pointerId].Position;
	}

	public static IPanel GetPanel(int pointerId, ContextType contextType)
	{
		if ((uint)contextType > 1u)
		{
		}
		return s_PlayerPointerLocations[pointerId].Panel;
	}

	private static bool HasFlagFast(LocationFlag flagSet, LocationFlag flag)
	{
		return (flagSet & flag) == flag;
	}

	public static bool HasLocationFlag(int pointerId, ContextType contextType, LocationFlag flag)
	{
		if ((uint)contextType > 1u)
		{
		}
		return HasFlagFast(s_PlayerPointerLocations[pointerId].Flags, flag);
	}

	public static int GetPressedButtons(int pointerId)
	{
		return s_PressedButtons[pointerId];
	}

	internal static bool HasAdditionalPressedButtons(int pointerId, int exceptButtonId)
	{
		return (s_PressedButtons[pointerId] & ~(1 << exceptButtonId)) != 0;
	}

	internal static RuntimePanel GetPlayerPanelWithSoftPointerCapture(int pointerId)
	{
		return s_PlayerPanelWithSoftPointerCapture[pointerId];
	}

	internal static UIDocument GetWorldSpaceDocumentWithSoftPointerCapture(int pointerId)
	{
		return s_WorldSpaceDocumentWithSoftPointerCapture[pointerId];
	}

	internal static Camera GetCameraWithSoftPointerCapture(int pointerId)
	{
		return s_CameraWithSoftPointerCapture[pointerId];
	}

	internal static void SetElementWithSoftPointerCapture(int pointerId, VisualElement element, Camera camera)
	{
		RuntimePanel runtimePanel = element?.elementPanel as RuntimePanel;
		s_PlayerPanelWithSoftPointerCapture[pointerId] = runtimePanel;
		s_CameraWithSoftPointerCapture[pointerId] = camera;
		ref UIDocument reference = ref s_WorldSpaceDocumentWithSoftPointerCapture[pointerId];
		if (reference != null)
		{
			reference.softPointerCaptures &= ~(1 << pointerId);
		}
		reference = ((runtimePanel != null && runtimePanel.drawsInCameras) ? UIDocument.FindRootUIDocument(element) : null);
		if (reference != null)
		{
			reference.softPointerCaptures |= 1 << pointerId;
		}
	}

	internal static TrackedPointerState GetTrackedState(int pointerId, bool createIfNull = false)
	{
		int num = pointerId - PointerId.trackedPointerIdBase;
		if (num < 0 || num >= PointerId.trackedPointerCount)
		{
			return null;
		}
		if (createIfNull)
		{
			RuntimePointerState[] array = s_RuntimePointerStates;
			if (array[pointerId] == null)
			{
				array[pointerId] = new TrackedPointerState();
			}
		}
		return (TrackedPointerState)s_RuntimePointerStates[pointerId];
	}

	internal static void RemoveTrackedState(int pointerId)
	{
		int num = pointerId - PointerId.trackedPointerIdBase;
		if (num >= 0 && num < PointerId.trackedPointerCount)
		{
			s_RuntimePointerStates[pointerId] = null;
		}
	}

	internal static ScreenPointerState GetScreenPointerState(int pointerId, bool createIfNull = false)
	{
		int num = pointerId - PointerId.trackedPointerIdBase;
		if (num >= 0 && num < PointerId.trackedPointerCount)
		{
			return null;
		}
		if (createIfNull)
		{
			RuntimePointerState[] array = s_RuntimePointerStates;
			if (array[pointerId] == null)
			{
				array[pointerId] = new ScreenPointerState();
			}
		}
		return (ScreenPointerState)s_RuntimePointerStates[pointerId];
	}
}
