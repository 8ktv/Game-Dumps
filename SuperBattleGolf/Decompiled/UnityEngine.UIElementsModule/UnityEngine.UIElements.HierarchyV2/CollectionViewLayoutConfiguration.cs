using System;

namespace UnityEngine.UIElements.HierarchyV2;

internal abstract class CollectionViewLayoutConfiguration
{
	internal CollectionView m_View;

	public Func<VisualElement> makeCell { get; set; }

	public Action<VisualElement, int> bindCell { get; set; }

	public Action<VisualElement, int> unbindCell { get; set; }

	public Action<VisualElement> destroyCell { get; set; }
}
