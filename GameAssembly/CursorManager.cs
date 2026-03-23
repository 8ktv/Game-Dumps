using UnityEngine;

public class CursorManager : SingletonBehaviour<CursorManager>
{
	private static bool isCursorLocked;

	private static bool isCursorForceUnlocked;

	public static bool CursorLocked => isCursorLocked;

	public static void SetCursorLock(bool locked)
	{
		isCursorLocked = locked;
		ApplyCursorLock();
	}

	public static void SetCursorForceUnlocked(bool forceUnlocked)
	{
		isCursorForceUnlocked = forceUnlocked;
		ApplyCursorLock();
	}

	private static void ApplyCursorLock()
	{
		int num;
		if (isCursorLocked)
		{
			num = ((!isCursorForceUnlocked) ? 1 : 0);
			if (num != 0)
			{
				Cursor.lockState = ((!PlayerInput.UsingGamepad) ? CursorLockMode.Locked : CursorLockMode.Confined);
				goto IL_002d;
			}
		}
		else
		{
			num = 0;
		}
		Cursor.lockState = CursorLockMode.None;
		goto IL_002d;
		IL_002d:
		Cursor.visible = num == 0;
		if (Cursor.lockState == CursorLockMode.Locked && Application.isFocused)
		{
			MouseUtility.SetMouseViewportPosition(Vector2.one / 2f);
		}
	}
}
