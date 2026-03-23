using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements.Layout;

[RequiredByNativeCode]
internal readonly struct LayoutDataAccess
{
	private readonly int m_Manager;

	private readonly LayoutDataStore m_Nodes;

	private readonly LayoutDataStore m_Configs;

	public bool IsValid => m_Nodes.IsValid && m_Configs.IsValid;

	internal LayoutDataAccess(int manager, LayoutDataStore nodes, LayoutDataStore configs)
	{
		m_Manager = manager;
		m_Nodes = nodes;
		m_Configs = configs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe ref T GetTypedNodeDataRef<T>(LayoutHandle handle, LayoutNodeDataType type) where T : unmanaged
	{
		return ref *(T*)m_Nodes.GetComponentDataPtr(handle.Index, (int)type);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe ref T GetTypedConfigDataRef<T>(LayoutHandle handle, LayoutConfigDataType type) where T : unmanaged
	{
		return ref *(T*)m_Configs.GetComponentDataPtr(handle.Index, (int)type);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref LayoutNodeData GetNodeData(LayoutHandle handle)
	{
		return ref GetTypedNodeDataRef<LayoutNodeData>(handle, LayoutNodeDataType.Node);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref LayoutStyleData GetStyleData(LayoutHandle handle)
	{
		return ref GetTypedNodeDataRef<LayoutStyleData>(handle, LayoutNodeDataType.Style);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref LayoutComputedData GetComputedData(LayoutHandle handle)
	{
		return ref GetTypedNodeDataRef<LayoutComputedData>(handle, LayoutNodeDataType.Computed);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref LayoutCacheData GetCacheData(LayoutHandle handle)
	{
		return ref GetTypedNodeDataRef<LayoutCacheData>(handle, LayoutNodeDataType.Cache);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref LayoutConfigData GetConfigData(LayoutHandle handle)
	{
		return ref GetTypedConfigDataRef<LayoutConfigData>(handle, LayoutConfigDataType.Config);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LayoutMeasureFunction GetMeasureFunction(LayoutHandle handle)
	{
		return LayoutManager.GetManager(m_Manager).GetMeasureFunction(handle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMeasureFunction(LayoutHandle handle, LayoutMeasureFunction value)
	{
		LayoutManager.GetManager(m_Manager).SetMeasureFunction(handle, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public VisualElement GetOwner(LayoutHandle handle)
	{
		return LayoutManager.GetManager(m_Manager).GetOwner(handle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetOwner(LayoutHandle handle, VisualElement value)
	{
		LayoutManager.GetManager(m_Manager).SetOwner(handle, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LayoutBaselineFunction GetBaselineFunction(LayoutHandle handle)
	{
		return LayoutManager.GetManager(m_Manager).GetBaselineFunction(handle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetBaselineFunction(LayoutHandle handle, LayoutBaselineFunction value)
	{
		LayoutManager.GetManager(m_Manager).SetBaselineFunction(handle, value);
	}
}
