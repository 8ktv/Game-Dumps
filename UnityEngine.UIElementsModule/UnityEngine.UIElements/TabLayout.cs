using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal class TabLayout
{
	private TabView m_TabView;

	private List<VisualElement> m_TabHeaders;

	private bool m_IsVertical;

	public TabLayout(TabView tabView, bool isVertical)
	{
		m_TabView = tabView;
		m_TabHeaders = tabView.tabHeaders;
		m_IsVertical = isVertical;
	}

	public static float GetHeight(VisualElement t)
	{
		return t.boundingBox.height;
	}

	public static float GetWidth(VisualElement t)
	{
		return t.boundingBox.width;
	}

	public float GetTabOffset(VisualElement tab)
	{
		if (!tab.visible)
		{
			return float.NaN;
		}
		float num = 0f;
		int num2 = m_TabHeaders.IndexOf(tab);
		for (int i = 0; i < num2; i++)
		{
			VisualElement t = m_TabHeaders[i];
			float num3 = (m_IsVertical ? GetHeight(t) : GetWidth(t));
			if (!float.IsNaN(num3))
			{
				num += num3;
			}
		}
		return num;
	}

	private void InitOrderTabs()
	{
		if (m_TabHeaders == null)
		{
			m_TabHeaders = new List<VisualElement>();
		}
	}

	public void ReorderDisplay(int from, int to)
	{
		InitOrderTabs();
		m_TabView.ReorderTab(from, to);
	}
}
