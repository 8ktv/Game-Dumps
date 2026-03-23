using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CreditsRoll : MonoBehaviour
{
	public ScrollRect scrollRect;

	public float speed = 10f;

	public float startPosition;

	private bool aborted;

	private void OnEnable()
	{
		scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
		ResetScroll();
		aborted = false;
	}

	private void Update()
	{
		if (!aborted)
		{
			scrollRect.velocity = Vector3.up * speed;
			if (scrollRect.content.anchoredPosition.y > scrollRect.content.sizeDelta.y)
			{
				ResetScroll();
			}
			if (RectTransformUtility.RectangleContainsScreenPoint(scrollRect.GetComponent<RectTransform>(), Mouse.current.position.value) && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.scroll.value.sqrMagnitude > float.Epsilon))
			{
				aborted = true;
				scrollRect.velocity = Vector3.zero;
				scrollRect.movementType = ScrollRect.MovementType.Elastic;
			}
		}
	}

	private void ResetScroll()
	{
		Vector2 anchoredPosition = scrollRect.content.anchoredPosition;
		anchoredPosition.y = startPosition;
		scrollRect.content.anchoredPosition = anchoredPosition;
	}
}
