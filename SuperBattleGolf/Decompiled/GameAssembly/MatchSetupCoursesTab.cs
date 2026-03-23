using UnityEngine;
using UnityEngine.UI;

public class MatchSetupCoursesTab : MonoBehaviour
{
	public ScrollRect activeHolesScrollRect;

	public ScrollRect inactiveHolesScrollRect;

	private void OnEnable()
	{
		ResetHoleScrollRects();
	}

	public void ResetHoleScrollRects()
	{
		activeHolesScrollRect.horizontalNormalizedPosition = 1f;
		inactiveHolesScrollRect.verticalNormalizedPosition = 1f;
	}
}
