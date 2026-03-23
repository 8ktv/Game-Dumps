namespace UnityEngine.UIElements.Layout;

internal readonly struct LayoutConfig
{
	private readonly LayoutDataAccess m_Access;

	private readonly LayoutHandle m_Handle;

	public static LayoutConfig Undefined => new LayoutConfig(default(LayoutDataAccess), LayoutHandle.Undefined);

	public bool IsUndefined => m_Handle.Equals(LayoutHandle.Undefined);

	public LayoutHandle Handle => m_Handle;

	public ref float PointScaleFactor => ref m_Access.GetConfigData(m_Handle).PointScaleFactor;

	public ref bool ShouldLog => ref m_Access.GetConfigData(m_Handle).ShouldLog;

	public LayoutMeasureFunction Measure
	{
		get
		{
			return m_Access.GetMeasureFunction(m_Handle);
		}
		set
		{
			m_Access.SetMeasureFunction(m_Handle, value);
		}
	}

	public LayoutBaselineFunction Baseline
	{
		get
		{
			return m_Access.GetBaselineFunction(m_Handle);
		}
		set
		{
			m_Access.SetBaselineFunction(m_Handle, value);
		}
	}

	internal LayoutConfig(LayoutDataAccess access, LayoutHandle handle)
	{
		m_Access = access;
		m_Handle = handle;
	}
}
