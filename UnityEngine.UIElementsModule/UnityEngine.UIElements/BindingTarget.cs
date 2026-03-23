namespace UnityEngine.UIElements;

internal readonly struct BindingTarget
{
	public readonly VisualElement element;

	public readonly BindingId bindingId;

	public BindingTarget(VisualElement element, in BindingId bindingId)
	{
		this.element = element;
		this.bindingId = bindingId;
	}
}
