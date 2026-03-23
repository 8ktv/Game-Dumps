using UnityEngine;
using UnityEngine.UI;

public static class ScrollRectExtensions
{
	public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 focusPoint)
	{
		Vector2 size = scrollView.content.rect.size;
		Vector2 size2 = ((RectTransform)scrollView.content.parent).rect.size;
		Vector2 scale = scrollView.content.localScale;
		size.Scale(scale);
		focusPoint.Scale(scale);
		Vector2 normalizedPosition = scrollView.normalizedPosition;
		if (scrollView.horizontal && size.x > size2.x)
		{
			normalizedPosition.x = Mathf.Clamp01((focusPoint.x - size2.x * 0.5f) / (size.x - size2.x));
		}
		if (scrollView.vertical && size.y > size2.y)
		{
			normalizedPosition.y = Mathf.Clamp01((focusPoint.y - size2.y * 0.5f) / (size.y - size2.y));
		}
		return normalizedPosition;
	}

	public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, RectTransform item)
	{
		Vector2 vector = scrollView.content.InverseTransformPoint(item.transform.TransformPoint(item.rect.center));
		Vector2 size = scrollView.content.rect.size;
		size.Scale(scrollView.content.pivot);
		return scrollView.CalculateFocusedScrollPosition(vector + size);
	}

	public static void FocusAtPoint(this ScrollRect scrollView, Vector2 focusPoint)
	{
		scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(focusPoint);
	}

	public static void FocusOnItem(this ScrollRect scrollView, RectTransform item)
	{
		scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(item);
	}

	public static void EnsureVisibility(this ScrollRect scrollRect, RectTransform item, Vector2 padding)
	{
		if (!(item == null))
		{
			Vector2 min = scrollRect.viewport.rect.min;
			Vector2 max = scrollRect.viewport.rect.max;
			Vector2 vector = scrollRect.viewport.InverseTransformPoint(item.TransformPoint(item.rect.min));
			Vector2 vector2 = scrollRect.viewport.InverseTransformPoint(item.TransformPoint(item.rect.max));
			vector -= padding;
			vector2 += padding;
			Vector2 zero = Vector2.zero;
			if (vector2.y > max.y)
			{
				zero.y = vector2.y - max.y;
			}
			if (vector.x < min.x)
			{
				zero.x = vector.x - min.x;
			}
			if (vector2.x > max.x)
			{
				zero.x = vector2.x - max.x;
			}
			if (vector.y < min.y)
			{
				zero.y = vector.y - min.y;
			}
			Vector3 direction = scrollRect.viewport.TransformDirection(zero);
			Vector3 vector3 = scrollRect.content.InverseTransformDirection(direction);
			if (!scrollRect.horizontal)
			{
				vector3.x = 0f;
			}
			if (!scrollRect.vertical)
			{
				vector3.y = 0f;
			}
			scrollRect.content.localPosition -= vector3;
		}
	}
}
