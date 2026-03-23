using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.UIR;

internal class GCHandlePool : IDisposable
{
	private List<GCHandle> m_Handles;

	private int m_UsedHandlesCount;

	private readonly int k_AllocBatchSize;

	internal bool disposed { get; private set; }

	public GCHandlePool(int capacity = 256, int allocBatchSize = 64)
	{
		m_Handles = new List<GCHandle>(capacity);
		m_UsedHandlesCount = 0;
		k_AllocBatchSize = allocBatchSize;
	}

	public GCHandle Get(object target)
	{
		if (target == null)
		{
			return default(GCHandle);
		}
		if (m_UsedHandlesCount < m_Handles.Count)
		{
			GCHandle result = m_Handles[m_UsedHandlesCount++];
			result.Target = target;
			return result;
		}
		GCHandle gCHandle = GCHandle.Alloc(target);
		m_Handles.Add(gCHandle);
		m_UsedHandlesCount++;
		int i = 0;
		for (int num = k_AllocBatchSize - 1; i < num; i++)
		{
			m_Handles.Add(GCHandle.Alloc(null));
		}
		return gCHandle;
	}

	public IntPtr GetIntPtr(object target)
	{
		if (target == null)
		{
			return IntPtr.Zero;
		}
		return GCHandle.ToIntPtr(Get(target));
	}

	public void ReturnAll()
	{
		for (int i = 0; i < m_UsedHandlesCount; i++)
		{
			GCHandle value = m_Handles[i];
			value.Target = null;
			m_Handles[i] = value;
		}
		m_UsedHandlesCount = 0;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			foreach (GCHandle handle in m_Handles)
			{
				if (handle.IsAllocated)
				{
					handle.Free();
				}
			}
			m_Handles = null;
		}
		disposed = true;
	}
}
