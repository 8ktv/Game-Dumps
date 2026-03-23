namespace UnityEngine.UIElements;

public readonly struct DataSourceContextChanged
{
	private readonly VisualElement m_TargetElement;

	private readonly BindingId m_BindingId;

	private readonly DataSourceContext m_PreviousContext;

	private readonly DataSourceContext m_NewContext;

	public VisualElement targetElement => m_TargetElement;

	public BindingId bindingId => m_BindingId;

	public DataSourceContext previousContext => m_PreviousContext;

	public DataSourceContext newContext => m_NewContext;

	internal DataSourceContextChanged(VisualElement element, in BindingId bindingId, in DataSourceContext previousContext, in DataSourceContext newContext)
	{
		m_TargetElement = element;
		m_BindingId = bindingId;
		m_PreviousContext = previousContext;
		m_NewContext = newContext;
	}
}
