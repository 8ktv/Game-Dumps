using System;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy;

public readonly struct HierarchyViewModelNodesEnumerable
{
	internal delegate bool Predicate(in HierarchyNode node, HierarchyNodeFlags flags);

	public struct Enumerator
	{
		private readonly HierarchyViewModel m_HierarchyViewModel;

		private readonly Predicate m_Predicate;

		private readonly HierarchyNodeFlags m_Flags;

		private readonly ReadOnlyNativeVector<HierarchyFlattenedNode> m_FlattenedNodes;

		private readonly int m_Version;

		private int m_Index;

		public ref readonly HierarchyNode Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				ThrowIfVersionChanged();
				return ref HierarchyFlattenedNode.GetNodeByRef(in m_FlattenedNodes[m_Index]);
			}
		}

		internal Enumerator(HierarchyViewModelNodesEnumerable enumerable)
		{
			m_HierarchyViewModel = enumerable.m_HierarchyViewModel;
			m_Predicate = enumerable.m_Predicate;
			m_Flags = enumerable.m_Flags;
			m_FlattenedNodes = m_HierarchyViewModel.FlattenedNodes;
			m_Version = m_HierarchyViewModel.Version;
			m_Index = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			ThrowIfVersionChanged();
			do
			{
				if (++m_Index >= m_FlattenedNodes.Count)
				{
					return false;
				}
			}
			while (!m_Predicate(in HierarchyFlattenedNode.GetNodeByRef(in m_FlattenedNodes[m_Index]), m_Flags));
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfVersionChanged()
		{
			if (m_Version != m_HierarchyViewModel.Version)
			{
				throw new InvalidOperationException("HierarchyViewModel was modified.");
			}
		}
	}

	private readonly HierarchyViewModel m_HierarchyViewModel;

	private readonly Predicate m_Predicate;

	private readonly HierarchyNodeFlags m_Flags;

	internal HierarchyViewModelNodesEnumerable(HierarchyViewModel viewModel, HierarchyNodeFlags flags, Predicate predicate)
	{
		m_HierarchyViewModel = viewModel ?? throw new ArgumentNullException("viewModel");
		m_Predicate = predicate ?? throw new ArgumentNullException("predicate");
		m_Flags = flags;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}
}
