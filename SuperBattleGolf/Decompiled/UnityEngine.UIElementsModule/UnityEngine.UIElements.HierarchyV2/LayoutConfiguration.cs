using System;

namespace UnityEngine.UIElements.HierarchyV2;

internal class LayoutConfiguration : CollectionViewLayoutConfiguration
{
	public LayoutConfiguration()
	{
		base.makeCell = (Func<VisualElement>)Delegate.Combine(base.makeCell, new Func<VisualElement>(MakeCell));
		base.bindCell = (Action<VisualElement, int>)Delegate.Combine(base.bindCell, new Action<VisualElement, int>(BindCell));
		base.unbindCell = (Action<VisualElement, int>)Delegate.Combine(base.unbindCell, new Action<VisualElement, int>(UnbindCell));
		base.destroyCell = (Action<VisualElement>)Delegate.Combine(base.destroyCell, new Action<VisualElement>(DestroyCell));
	}

	private VisualElement MakeCell()
	{
		return new Label();
	}

	private void BindCell(VisualElement element, int index)
	{
	}

	private void UnbindCell(VisualElement element, int index)
	{
	}

	private void DestroyCell(VisualElement element)
	{
	}
}
