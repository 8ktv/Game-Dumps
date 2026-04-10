using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy;

public readonly struct HierarchyNodeTypeHandlerBaseEnumerable
{
	public struct Enumerator : IDisposable
	{
		private readonly IntPtr[] m_Handlers;

		private readonly int m_Count;

		private int m_Index;

		public HierarchyNodeTypeHandlerBase Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return HierarchyNodeTypeHandlerBase.FromIntPtr(m_Handlers[m_Index]);
			}
		}

		internal Enumerator(Hierarchy hierarchy)
		{
			int nodeTypeHandlersBaseCount = hierarchy.GetNodeTypeHandlersBaseCount();
			m_Handlers = ArrayPool<IntPtr>.Shared.Rent(nodeTypeHandlersBaseCount);
			m_Count = hierarchy.GetNodeTypeHandlersBaseSpan(MemoryExtensions.AsSpan(m_Handlers).Slice(0, nodeTypeHandlersBaseCount));
			m_Index = -1;
		}

		public void Dispose()
		{
			ArrayPool<IntPtr>.Shared.Return(m_Handlers);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			return ++m_Index < m_Count;
		}
	}

	private readonly Hierarchy m_Hierarchy;

	internal HierarchyNodeTypeHandlerBaseEnumerable(Hierarchy hierarchy)
	{
		m_Hierarchy = hierarchy;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(m_Hierarchy);
	}
}
