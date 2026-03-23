namespace UnityEngine.UIElements;

internal class TabDragLocationPreview : VisualElement
{
	public static readonly string ussClassName = TabView.ussClassName + "__drag-location-preview";

	public static readonly string visualUssClassName = ussClassName + "__visual";

	public static readonly string verticalUssClassName = ussClassName + "__vertical";

	public static readonly string horizontalUssClassName = ussClassName + "__horizontal";

	private VisualElement m_Preview;

	internal VisualElement preview => m_Preview;

	public TabDragLocationPreview()
	{
		AddToClassList(ussClassName);
		base.pickingMode = PickingMode.Ignore;
		m_Preview = new VisualElement();
		m_Preview.AddToClassList(visualUssClassName);
		m_Preview.pickingMode = PickingMode.Ignore;
		Add(m_Preview);
	}
}
