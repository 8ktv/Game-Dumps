using Unity.Properties;

namespace UnityEngine.UIElements;

public readonly struct BindingContext
{
	private readonly VisualElement m_TargetElement;

	private readonly BindingId m_BindingId;

	private readonly PropertyPath m_DataSourcePath;

	private readonly object m_DataSource;

	public VisualElement targetElement => m_TargetElement;

	public BindingId bindingId => m_BindingId;

	public PropertyPath dataSourcePath => m_DataSourcePath;

	public object dataSource => m_DataSource;

	internal BindingContext(VisualElement targetElement, in BindingId bindingId, in PropertyPath resolvedDataSourcePath, object resolvedDataSource)
	{
		m_TargetElement = targetElement;
		m_BindingId = bindingId;
		m_DataSourcePath = resolvedDataSourcePath;
		m_DataSource = resolvedDataSource;
	}
}
