using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.Scripting;

namespace Unity.Hierarchy;

[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode(Optional = true)]
[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
internal abstract class HierarchyNodeTypeHandler : HierarchyNodeTypeHandlerBase
{
	private readonly Lazy<ObjectPool<HierarchyViewItem>> m_ViewItemPool;

	internal ObjectPool<HierarchyViewItem> ViewItemPool => m_ViewItemPool.Value;

	protected HierarchyNodeTypeHandler()
	{
		m_ViewItemPool = new Lazy<ObjectPool<HierarchyViewItem>>(() => new ObjectPool<HierarchyViewItem>(() => new HierarchyViewItem(), null, null, null, collectionCheck: true, 0));
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal HierarchyNodeTypeHandler(IntPtr nativePtr, Hierarchy hierarchy, HierarchyCommandList cmdList)
		: base(nativePtr, hierarchy, cmdList)
	{
		m_ViewItemPool = new Lazy<ObjectPool<HierarchyViewItem>>(() => new ObjectPool<HierarchyViewItem>(() => new HierarchyViewItem(), null, null, null, collectionCheck: true, 0));
	}

	protected virtual void OnBindView(HierarchyView view)
	{
	}

	protected virtual void OnUnbindView(HierarchyView view)
	{
	}

	protected virtual void OnBindItem(HierarchyViewItem item)
	{
	}

	protected virtual void OnUnbindItem(HierarchyViewItem item)
	{
	}

	internal void Internal_BindView(HierarchyView view)
	{
		OnBindView(view);
	}

	internal void Internal_UnbindView(HierarchyView view)
	{
		OnUnbindView(view);
	}

	internal void Internal_BindItem(HierarchyViewItem item)
	{
		OnBindItem(item);
	}

	internal void Internal_UnbindItem(HierarchyViewItem item)
	{
		OnUnbindItem(item);
	}
}
