using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements;

internal readonly struct VisualPanel
{
	private readonly VisualManager m_Manager;

	private readonly VisualPanelHandle m_Handle;

	public static VisualPanel Null => new VisualPanel(null, VisualPanelHandle.Null);

	public VisualPanelHandle Handle => m_Handle;

	public bool IsCreated => !m_Handle.Equals(VisualPanelHandle.Null) && m_Manager.ContainsPanel(in m_Handle);

	internal unsafe ref VisualPanelData Data => ref UnsafeUtility.AsRef<VisualPanelData>(m_Manager.GetPanelDataPtr(in m_Handle));

	public ref VisualNodeHandle RootContainer => ref Data.RootContainer;

	public ref bool DuringLayoutPhase => ref Data.DuringLayoutPhase;

	public VisualNode GetRootContainer()
	{
		return new VisualNode(m_Manager, Data.RootContainer);
	}

	public void SetRootContainer(VisualNode node)
	{
		Data.RootContainer = node.Handle;
	}

	internal VisualPanel(VisualManager manager, VisualPanelHandle handle)
	{
		m_Manager = manager;
		m_Handle = handle;
	}

	public void Destroy()
	{
		m_Manager.RemovePanel(in m_Handle);
	}

	public BaseVisualElementPanel GetOwner()
	{
		return m_Manager.GetOwner(in m_Handle);
	}

	public void SetOwner(BaseVisualElementPanel owner)
	{
		m_Manager.SetOwner(in m_Handle, owner);
	}
}
