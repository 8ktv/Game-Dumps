namespace UnityEngine.UIElements;

internal class TabDragPreview : VisualElement
{
	public static readonly string ussClassName = TabView.ussClassName + "__drag-preview";

	public TabDragPreview()
	{
		AddToClassList(ussClassName);
		base.pickingMode = PickingMode.Ignore;
	}
}
