namespace Unity.Hierarchy;

public ref struct HierarchyViewModelFlagsChangeScope
{
	private readonly HierarchyViewModel m_HierarchyViewModel;

	private readonly bool m_Notify;

	public HierarchyViewModelFlagsChangeScope(HierarchyViewModel hierarchyViewModel)
	{
		m_HierarchyViewModel = hierarchyViewModel;
		m_Notify = true;
		m_HierarchyViewModel.BeginFlagsChange();
	}

	public HierarchyViewModelFlagsChangeScope(HierarchyViewModel hierarchyViewModel, bool notify)
	{
		m_HierarchyViewModel = hierarchyViewModel;
		m_Notify = notify;
		m_HierarchyViewModel.BeginFlagsChange();
	}

	public void Dispose()
	{
		if (m_HierarchyViewModel != null && m_HierarchyViewModel.IsCreated)
		{
			if (m_Notify)
			{
				m_HierarchyViewModel.EndFlagsChange();
			}
			else
			{
				m_HierarchyViewModel.EndFlagsChangeWithoutNotify();
			}
		}
	}
}
