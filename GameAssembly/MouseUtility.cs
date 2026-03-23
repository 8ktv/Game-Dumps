using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class MouseUtility
{
	public static void SetMouseScreenPosition(Vector2 screenPosition)
	{
		SetMouseScreenPositionInternal(ClampScreenPosition(screenPosition));
	}

	public static void SetMouseWorldPosition(Vector3 worldPosition)
	{
		if (!(GameManager.Camera == null))
		{
			SetMouseScreenPositionInternal(ClampScreenPosition(GameManager.Camera.WorldToScreenPoint(worldPosition)));
		}
	}

	public static void SetMouseViewportPosition(Vector2 viewportPosition)
	{
		if (!(GameManager.Camera == null))
		{
			SetMouseScreenPositionInternal(ClampScreenPosition(GameManager.Camera.ViewportToScreenPoint(viewportPosition)));
		}
	}

	public static Vector2 ClampScreenPosition(Vector2 screenPosition)
	{
		Rect safeArea = Screen.safeArea;
		return new Vector2(BMath.Clamp(screenPosition.x, safeArea.xMin, safeArea.xMax), BMath.Clamp(screenPosition.y, safeArea.yMin, safeArea.yMax));
	}

	private static void SetMouseScreenPositionInternal(Vector2 screenPosition)
	{
		InputState.Change(Mouse.current.position, screenPosition);
		Mouse.current.WarpCursorPosition(screenPosition);
	}
}
