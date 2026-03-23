using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Hierarchy;

namespace UnityEngine.UIElements;

internal class VisualNodePropertyRegistry
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct TypeIndex<T>
	{
		public static int Index;
	}

	private class HierarchyPropertyBinding<TProperty> where TProperty : unmanaged
	{
		public readonly HierarchyPropertyUnmanaged<TProperty> Property;

		public HierarchyPropertyBinding(HierarchyPropertyUnmanaged<TProperty> property)
		{
			Property = property;
		}
	}

	private static int s_InternalPropertyCount;

	private static int s_HierarchyPropertyCount;

	private readonly VisualManager m_Manager;

	private unsafe readonly VisualNodePropertyData*[] m_InternalPropertyData;

	private readonly ChunkAllocatingArray<object> m_Bindings = new ChunkAllocatingArray<object>();

	public static void RegisterInternalProperty<TProperty>()
	{
		if (TypeIndex<TProperty>.Index != 0)
		{
			throw new InvalidOperationException("TProperty has already been registered");
		}
		TypeIndex<TProperty>.Index = -(++s_InternalPropertyCount);
	}

	public static void RegisterHierarchyProperty<TProperty>()
	{
		if (TypeIndex<TProperty>.Index != 0)
		{
			throw new InvalidOperationException("TProperty has already been registered");
		}
		TypeIndex<TProperty>.Index = ++s_HierarchyPropertyCount;
	}

	public unsafe VisualNodePropertyRegistry(VisualManager manager)
	{
		m_Manager = manager ?? throw new ArgumentNullException("manager");
		m_InternalPropertyData = new VisualNodePropertyData*[s_InternalPropertyCount];
		for (int i = 0; i < s_InternalPropertyCount; i++)
		{
			m_InternalPropertyData[i] = (VisualNodePropertyData*)(void*)m_Manager.GetPropertyPtr(i);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsInternalProperty(int typeIndex)
	{
		return typeIndex < 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe VisualNodeProperty<T> GetInternalProperty<T>(int typeIndex) where T : unmanaged
	{
		return new VisualNodeProperty<T>(m_InternalPropertyData[-typeIndex - 1]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private HierarchyPropertyUnmanaged<T> GetHierarchyProperty<T>(int typeIndex) where T : unmanaged
	{
		throw new NotImplementedException();
	}

	public T GetProperty<T>(VisualNodeHandle handle) where T : unmanaged
	{
		int index = TypeIndex<T>.Index;
		if (index == 0)
		{
			throw new InvalidOperationException("The property type has not been registered");
		}
		if (IsInternalProperty(index))
		{
			return GetInternalProperty<T>(index)[handle];
		}
		return GetHierarchyProperty<T>(index).GetValue(in UnsafeUtility.As<VisualNodeHandle, HierarchyNode>(ref handle));
	}

	public void SetProperty<T>(VisualNodeHandle handle, in T value) where T : unmanaged
	{
		int index = TypeIndex<T>.Index;
		if (index == 0)
		{
			throw new InvalidOperationException("The property type has not been registered");
		}
		if (IsInternalProperty(index))
		{
			GetInternalProperty<T>(index)[handle] = value;
		}
		else
		{
			GetHierarchyProperty<T>(index).SetValue(in UnsafeUtility.As<VisualNodeHandle, HierarchyNode>(ref handle), value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref T GetPropertyRef<T>(VisualNodeHandle handle) where T : unmanaged
	{
		int index = TypeIndex<T>.Index;
		if (index == 0)
		{
			throw new InvalidOperationException("The property type has not been registered");
		}
		if (index > 0)
		{
			throw new InvalidOperationException("The property type is not an internal property");
		}
		return ref GetInternalProperty<T>(index)[handle];
	}
}
