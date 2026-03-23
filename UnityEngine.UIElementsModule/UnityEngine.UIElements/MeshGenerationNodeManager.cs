using System;
using System.Collections.Generic;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

internal class MeshGenerationNodeManager : IDisposable
{
	private List<MeshGenerationNodeImpl> m_Nodes = new List<MeshGenerationNodeImpl>(8);

	private int m_UsedCounter;

	private EntryRecorder m_EntryRecorder;

	protected bool disposed { get; private set; }

	public MeshGenerationNodeManager(EntryRecorder entryRecorder)
	{
		m_EntryRecorder = entryRecorder;
	}

	public void CreateNode(Entry parentEntry, out MeshGenerationNode node)
	{
		MeshGenerationNodeImpl meshGenerationNodeImpl = CreateImpl(parentEntry, safe: true);
		meshGenerationNodeImpl.GetNode(out node);
	}

	public void CreateUnsafeNode(Entry parentEntry, out UnsafeMeshGenerationNode node)
	{
		MeshGenerationNodeImpl meshGenerationNodeImpl = CreateImpl(parentEntry, safe: false);
		meshGenerationNodeImpl.GetUnsafeNode(out node);
	}

	private MeshGenerationNodeImpl CreateImpl(Entry parentEntry, bool safe)
	{
		if (disposed)
		{
			DisposeHelper.NotifyDisposedUsed(this);
			return null;
		}
		if (m_Nodes.Count == m_UsedCounter)
		{
			for (int i = 0; i < 200; i++)
			{
				m_Nodes.Add(new MeshGenerationNodeImpl());
			}
		}
		MeshGenerationNodeImpl meshGenerationNodeImpl = m_Nodes[m_UsedCounter++];
		meshGenerationNodeImpl.Init(parentEntry, m_EntryRecorder, safe);
		return meshGenerationNodeImpl;
	}

	public void ResetAll()
	{
		for (int i = 0; i < m_UsedCounter; i++)
		{
			m_Nodes[i].Reset();
		}
		m_UsedCounter = 0;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			int i = 0;
			for (int count = m_Nodes.Count; i < count; i++)
			{
				m_Nodes[i].Dispose();
			}
			m_Nodes.Clear();
		}
		disposed = true;
	}
}
