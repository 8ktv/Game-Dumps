#define UNITY_ASSERTIONS
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Profiling;

namespace UnityEngine.UIElements.Layout;

internal class LayoutManager : IDisposable
{
	private enum SharedManagerState
	{
		Uninitialized,
		Initialized,
		Shutdown
	}

	private static SharedManagerState s_Initialized;

	private static bool s_AppDomainUnloadRegistered;

	private static LayoutManager s_SharedInstance;

	private static readonly List<LayoutManager> s_Managers = new List<LayoutManager>();

	public const int k_CapacityBig = 65536;

	public const int k_CapacitySmall = 16;

	private const int k_InitialConfigCapacity = 32;

	private readonly int m_Index;

	private LayoutDataStore m_Nodes;

	private LayoutDataStore m_Configs;

	private readonly ConcurrentQueue<LayoutHandle> m_NodesToFree = new ConcurrentQueue<LayoutHandle>();

	private readonly LayoutHandle m_DefaultConfig;

	private readonly ManagedObjectStore<LayoutMeasureFunction> m_ManagedMeasureFunctions = new ManagedObjectStore<LayoutMeasureFunction>(16);

	private readonly ManagedObjectStore<LayoutBaselineFunction> m_ManagedBaselineFunctions = new ManagedObjectStore<LayoutBaselineFunction>(16);

	private readonly ManagedObjectStore<GCHandle> m_ManagedOwners = new ManagedObjectStore<GCHandle>();

	private readonly ProfilerMarker m_CollectMarker = new ProfilerMarker("UIElements.CollectLayoutNodes");

	private int m_HighMark = -1;

	public static bool IsSharedManagerCreated => s_Initialized == SharedManagerState.Initialized;

	public static LayoutManager SharedManager
	{
		get
		{
			Initialize();
			return s_SharedInstance;
		}
	}

	private static int DefaultCapacity => 16;

	public int NodeCapacity => m_Nodes.Capacity;

	private static void Initialize()
	{
		if (s_Initialized != SharedManagerState.Uninitialized)
		{
			return;
		}
		s_Initialized = SharedManagerState.Initialized;
		if (!s_AppDomainUnloadRegistered)
		{
			AppDomain.CurrentDomain.DomainUnload += delegate
			{
				Shutdown();
			};
			s_AppDomainUnloadRegistered = true;
		}
		s_SharedInstance = new LayoutManager(Allocator.Persistent);
	}

	private static void Shutdown()
	{
		if (s_Initialized == SharedManagerState.Initialized)
		{
			s_Initialized = SharedManagerState.Shutdown;
			s_SharedInstance.Dispose();
		}
	}

	internal static LayoutManager GetManager(int index)
	{
		return ((uint)index < s_Managers.Count) ? s_Managers[index] : null;
	}

	public LayoutManager(Allocator allocator)
		: this(allocator, DefaultCapacity)
	{
	}

	public LayoutManager(Allocator allocator, int initialNodeCapacity)
	{
		m_Index = s_Managers.Count;
		s_Managers.Add(this);
		ComponentType[] components = new ComponentType[4]
		{
			ComponentType.Create<LayoutNodeData>(),
			ComponentType.Create<LayoutStyleData>(),
			ComponentType.Create<LayoutComputedData>(),
			ComponentType.Create<LayoutCacheData>()
		};
		Span<MemoryLabel> span = stackalloc MemoryLabel[4]
		{
			new MemoryLabel("UIElements", "Layout.ComponentData<LayoutNodeData>"),
			new MemoryLabel("UIElements", "Layout.ComponentData<LayoutStyleData>"),
			new MemoryLabel("UIElements", "Layout.ComponentData<LayoutComputedData>"),
			new MemoryLabel("UIElements", "Layout.ComponentData<LayoutCacheData>")
		};
		ReadOnlySpan<MemoryLabel> labels = span;
		ComponentType[] components2 = new ComponentType[1] { ComponentType.Create<LayoutConfigData>() };
		span = stackalloc MemoryLabel[1]
		{
			new MemoryLabel("UIElements", "Layout.ComponentData<LayoutConfigData>")
		};
		ReadOnlySpan<MemoryLabel> labels2 = span;
		m_Nodes = new LayoutDataStore(components, labels, initialNodeCapacity, allocator);
		m_Configs = new LayoutDataStore(components2, labels2, 32, allocator);
		m_DefaultConfig = CreateConfig().Handle;
	}

	public unsafe void Dispose()
	{
		s_Managers[m_Index] = null;
		for (int i = 0; i <= m_HighMark; i++)
		{
			LayoutCacheData* componentDataPtr = (LayoutCacheData*)m_Nodes.GetComponentDataPtr(i, 3);
			componentDataPtr->ClearCachedMeasurements();
			LayoutNodeData* componentDataPtr2 = (LayoutNodeData*)m_Nodes.GetComponentDataPtr(i, 0);
			if (componentDataPtr2->Children.IsCreated)
			{
				componentDataPtr2->Children.Dispose();
				componentDataPtr2->Children = new LayoutList<LayoutHandle>();
				GCHandle value = m_ManagedOwners.GetValue(componentDataPtr2->ManagedOwnerIndex);
				if (value.IsAllocated)
				{
					value.Free();
				}
			}
		}
		m_Nodes.Dispose();
		m_Configs.Dispose();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private LayoutDataAccess GetAccess()
	{
		return new LayoutDataAccess(m_Index, m_Nodes, m_Configs);
	}

	public LayoutConfig GetDefaultConfig()
	{
		return new LayoutConfig(GetAccess(), m_DefaultConfig);
	}

	public LayoutConfig CreateConfig()
	{
		return new LayoutConfig(GetAccess(), m_Configs.Allocate<LayoutConfigData>(LayoutConfigData.Default));
	}

	public void DestroyConfig(ref LayoutConfig config)
	{
		m_Configs.Free(config.Handle);
		config = LayoutConfig.Undefined;
	}

	public LayoutNode CreateNode()
	{
		return CreateNodeInternal(m_DefaultConfig);
	}

	public LayoutNode CreateNode(LayoutConfig config)
	{
		return CreateNodeInternal(config.Handle);
	}

	public LayoutNode CreateNode(LayoutNode source)
	{
		LayoutNode result = CreateNodeInternal(source.Config.Handle);
		result.CopyStyle(source);
		return result;
	}

	private LayoutNode CreateNodeInternal(LayoutHandle configHandle)
	{
		TryRecycleSingleNode();
		ref LayoutDataStore nodes = ref m_Nodes;
		LayoutNodeData component = new LayoutNodeData
		{
			Config = configHandle,
			Children = new LayoutList<LayoutHandle>()
		};
		LayoutHandle handle = nodes.Allocate<LayoutNodeData, LayoutStyleData, LayoutComputedData, LayoutCacheData>(in component, in LayoutStyleData.Default, LayoutComputedData.Default, in LayoutCacheData.Default);
		if (handle.Index > m_HighMark)
		{
			m_HighMark = handle.Index;
		}
		LayoutNode result = new LayoutNode(GetAccess(), handle);
		Debug.Assert(!GetAccess().GetNodeData(handle).Children.IsCreated, "memory is not initialized");
		return result;
	}

	private void TryRecycleSingleNode()
	{
		if (m_NodesToFree.TryDequeue(out var result))
		{
			FreeNode(result);
		}
	}

	private void TryRecycleNodes()
	{
		for (int i = 0; i < 100; i++)
		{
			if (!m_NodesToFree.TryDequeue(out var result))
			{
				break;
			}
			FreeNode(result);
		}
	}

	public void EnqueueNodeForRecycling(ref LayoutNode node)
	{
		if (!node.IsUndefined)
		{
			m_NodesToFree.Enqueue(node.Handle);
			node = LayoutNode.Undefined;
		}
	}

	private void FreeNode(LayoutHandle handle)
	{
		LayoutDataAccess access = GetAccess();
		ref LayoutNodeData nodeData = ref access.GetNodeData(handle);
		if (nodeData.Children.IsCreated)
		{
			nodeData.Children.Dispose();
			nodeData.Children = new LayoutList<LayoutHandle>();
		}
		access = GetAccess();
		access.GetCacheData(handle).ClearCachedMeasurements();
		nodeData.UsesMeasure = false;
		nodeData.UsesBaseline = false;
		GCHandle value = m_ManagedOwners.GetValue(nodeData.ManagedOwnerIndex);
		if (value.IsAllocated)
		{
			value.Free();
		}
		m_ManagedOwners.UpdateValue(ref nodeData.ManagedOwnerIndex, default(GCHandle));
		m_Nodes.Free(in handle);
	}

	public void Collect()
	{
		using (m_CollectMarker.Auto())
		{
			TryRecycleNodes();
		}
	}

	public VisualElement GetOwner(LayoutHandle handle)
	{
		if (GetAccess().GetNodeData(handle).ManagedOwnerIndex == 0)
		{
			return null;
		}
		return m_ManagedOwners.GetValue(GetAccess().GetNodeData(handle).ManagedOwnerIndex).Target as VisualElement;
	}

	public void SetOwner(LayoutHandle handle, VisualElement value)
	{
		ref int managedOwnerIndex = ref GetAccess().GetNodeData(handle).ManagedOwnerIndex;
		GCHandle value2 = m_ManagedOwners.GetValue(managedOwnerIndex);
		if (value2.IsAllocated)
		{
			value2.Free();
		}
		value2 = ((value != null) ? GCHandle.Alloc(value, GCHandleType.Weak) : default(GCHandle));
		m_ManagedOwners.UpdateValue(ref managedOwnerIndex, value2);
	}

	public LayoutMeasureFunction GetMeasureFunction(LayoutHandle handle)
	{
		int managedMeasureFunctionIndex = GetAccess().GetConfigData(handle).ManagedMeasureFunctionIndex;
		return m_ManagedMeasureFunctions.GetValue(managedMeasureFunctionIndex);
	}

	public void SetMeasureFunction(LayoutHandle handle, LayoutMeasureFunction value)
	{
		ref int managedMeasureFunctionIndex = ref GetAccess().GetConfigData(handle).ManagedMeasureFunctionIndex;
		m_ManagedMeasureFunctions.UpdateValue(ref managedMeasureFunctionIndex, value);
	}

	public LayoutBaselineFunction GetBaselineFunction(LayoutHandle handle)
	{
		int managedBaselineFunctionIndex = GetAccess().GetConfigData(handle).ManagedBaselineFunctionIndex;
		return m_ManagedBaselineFunctions.GetValue(managedBaselineFunctionIndex);
	}

	public void SetBaselineFunction(LayoutHandle handle, LayoutBaselineFunction value)
	{
		ref int managedBaselineFunctionIndex = ref GetAccess().GetConfigData(handle).ManagedBaselineFunctionIndex;
		m_ManagedBaselineFunctions.UpdateValue(ref managedBaselineFunctionIndex, value);
	}
}
