using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy;

[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete("HierarchyViewNodesEnumerable is obsolete, it has been renamed to HierarchyViewModelNodesEnumerable.")]
public readonly struct HierarchyViewNodesEnumerable
{
	internal delegate bool PredicateCallback(in HierarchyNode node, HierarchyNodeFlags flags);

	public struct Enumerator
	{
		private readonly HierarchyViewModel m_HierarchyViewModel;

		private readonly PredicateCallback m_Predicate;

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

		internal Enumerator(HierarchyViewNodesEnumerable enumerable)
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

	private readonly PredicateCallback m_Predicate;

	private readonly HierarchyNodeFlags m_Flags;

	internal HierarchyViewNodesEnumerable(HierarchyViewModel viewModel, HierarchyNodeFlags flags, PredicateCallback predicate)
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
