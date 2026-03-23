using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine;

internal class GCHandlePool
{
	private GCHandle[] m_handles;

	private int m_current;

	public GCHandlePool()
	{
		m_handles = new GCHandle[128];
	}

	public GCHandle Alloc()
	{
		if (m_current > 0)
		{
			return m_handles[--m_current];
		}
		return GCHandle.Alloc(null);
	}

	public GCHandle Alloc(object o)
	{
		if (m_current > 0)
		{
			GCHandle result = m_handles[--m_current];
			result.Target = o;
			return result;
		}
		return GCHandle.Alloc(o);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IntPtr AllocHandleIfNotNull(object o)
	{
		if (o == null)
		{
			return IntPtr.Zero;
		}
		return (IntPtr)Alloc(o);
	}

	public void Free(GCHandle h)
	{
		if (m_current == m_handles.Length)
		{
			int num = m_handles.Length * 2;
			GCHandle[] array = new GCHandle[num];
			Array.Copy(m_handles, array, m_handles.Length);
			m_handles = array;
		}
		h.Target = null;
		m_handles[m_current++] = h;
	}
}
