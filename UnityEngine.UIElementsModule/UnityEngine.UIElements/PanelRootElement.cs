namespace UnityEngine.UIElements;

internal class PanelRootElement : VisualElement
{
	public PanelRootElement()
	{
		base.name = VisualElementUtils.GetUniqueName("unity-panel-container");
		base.viewDataKey = "PanelContainer";
		base.pickingMode = PickingMode.Ignore;
		SetAsNextParentWithEventInterests();
	}
}
