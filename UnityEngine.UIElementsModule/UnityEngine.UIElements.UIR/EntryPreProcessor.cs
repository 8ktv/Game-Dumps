#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.UIR;

internal class EntryPreProcessor
{
	public struct AllocSize
	{
		public int vertexCount;

		public int indexCount;
	}

	private int m_ChildrenIndex;

	private List<AllocSize> m_Allocs;

	private List<AllocSize> m_HeadAllocs = new List<AllocSize>(1);

	private List<AllocSize> m_TailAllocs = new List<AllocSize>(1);

	private List<Entry> m_FlattenedEntries = new List<Entry>(8);

	private AllocSize m_Pending;

	private Stack<AllocSize> m_Mask = new Stack<AllocSize>(1);

	private bool m_IsPushingMask;

	public int childrenIndex => m_ChildrenIndex;

	public List<AllocSize> headAllocs => m_HeadAllocs;

	public List<AllocSize> tailAllocs => m_TailAllocs;

	public List<Entry> flattenedEntries => m_FlattenedEntries;

	public void PreProcess(Entry root)
	{
		m_ChildrenIndex = -1;
		m_FlattenedEntries.Clear();
		m_HeadAllocs.Clear();
		m_TailAllocs.Clear();
		m_Allocs = m_HeadAllocs;
		DoEvaluate(root);
		Flush();
		Debug.Assert(!m_IsPushingMask);
		Debug.Assert(m_Mask.Count == 0);
		Debug.Assert(m_ChildrenIndex >= 0);
	}

	public void ClearReferences()
	{
		m_FlattenedEntries.Clear();
	}

	private void DoEvaluate(Entry entry)
	{
		while (entry != null)
		{
			if (entry.type != EntryType.DedicatedPlaceholder)
			{
				m_FlattenedEntries.Add(entry);
			}
			switch (entry.type)
			{
			case EntryType.DrawSolidMesh:
			case EntryType.DrawTexturedMesh:
			case EntryType.DrawTexturedMeshSkipAtlas:
			case EntryType.DrawDynamicTexturedMesh:
			case EntryType.DrawTextMesh:
			case EntryType.DrawGradients:
				Debug.Assert(entry.vertices.Length <= UIRenderDevice.maxVerticesPerPage);
				Add(entry.vertices.Length, entry.indices.Length);
				break;
			case EntryType.DrawChildren:
				Debug.Assert(!m_IsPushingMask);
				Debug.Assert(m_ChildrenIndex == -1);
				Flush();
				m_ChildrenIndex = m_FlattenedEntries.Count - 1;
				m_Allocs = tailAllocs;
				break;
			case EntryType.BeginStencilMask:
				Debug.Assert(!m_IsPushingMask);
				m_IsPushingMask = true;
				break;
			case EntryType.EndStencilMask:
				Debug.Assert(m_IsPushingMask);
				m_IsPushingMask = false;
				break;
			case EntryType.PopStencilMask:
			{
				AllocSize result;
				while (m_Mask.TryPop(out result))
				{
					Add(result.vertexCount, result.indexCount);
				}
				break;
			}
			default:
				throw new NotImplementedException();
			case EntryType.DrawImmediate:
			case EntryType.DrawImmediateCull:
			case EntryType.PushClippingRect:
			case EntryType.PopClippingRect:
			case EntryType.PushScissors:
			case EntryType.PopScissors:
			case EntryType.PushGroupMatrix:
			case EntryType.PopGroupMatrix:
			case EntryType.PushDefaultMaterial:
			case EntryType.PopDefaultMaterial:
			case EntryType.CutRenderChain:
			case EntryType.DedicatedPlaceholder:
				break;
			}
			if (entry.firstChild != null)
			{
				DoEvaluate(entry.firstChild);
			}
			entry = entry.nextSibling;
		}
	}

	private void Add(int vertexCount, int indexCount)
	{
		if (vertexCount != 0 && indexCount != 0)
		{
			int num = m_Pending.vertexCount + vertexCount;
			if (num <= UIRenderDevice.maxVerticesPerPage)
			{
				m_Pending.vertexCount = num;
				m_Pending.indexCount += indexCount;
			}
			else
			{
				Flush();
				m_Pending.vertexCount = vertexCount;
				m_Pending.indexCount = indexCount;
			}
			if (m_IsPushingMask)
			{
				m_Mask.Push(new AllocSize
				{
					vertexCount = vertexCount,
					indexCount = indexCount
				});
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Flush()
	{
		if (m_Pending.vertexCount > 0)
		{
			m_Allocs.Add(m_Pending);
			m_Pending = default(AllocSize);
		}
	}
}
