using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RectMaxWidth : UIBehaviour, ILayoutSelfController, ILayoutController
{
	public float maxWidth;

	private DrivenRectTransformTracker m_Tracker;

	protected override void OnEnable()
	{
		base.OnEnable();
		SetDirty();
	}

	protected override void OnDisable()
	{
		m_Tracker.Clear();
		LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
		base.OnDisable();
	}

	protected void SetDirty()
	{
		if (IsActive())
		{
			LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
		}
	}

	public void SetLayoutHorizontal()
	{
		RectTransform component = GetComponent<RectTransform>();
		if (component.rect.width > maxWidth)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
			m_Tracker.Add(this, component, DrivenTransformProperties.SizeDeltaX);
		}
	}

	public void SetLayoutVertical()
	{
	}
}
