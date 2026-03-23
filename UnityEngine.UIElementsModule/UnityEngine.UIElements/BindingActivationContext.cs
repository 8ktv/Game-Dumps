namespace UnityEngine.UIElements;

public readonly struct BindingActivationContext
{
	private readonly VisualElement m_TargetElement;

	private readonly BindingId m_BindingId;

	public VisualElement targetElement => m_TargetElement;

	public BindingId bindingId => m_BindingId;

	internal BindingActivationContext(VisualElement element, in BindingId property)
	{
		m_TargetElement = element;
		m_BindingId = property;
	}
}
