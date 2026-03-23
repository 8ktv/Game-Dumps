using System;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[UsedByNativeCode]
public struct MeshLodRange
{
	[SerializeField]
	private uint m_IndexStart;

	[SerializeField]
	private uint m_IndexCount;

	public uint indexStart
	{
		get
		{
			return m_IndexStart;
		}
		set
		{
			m_IndexStart = value;
		}
	}

	public uint indexCount
	{
		get
		{
			return m_IndexCount;
		}
		set
		{
			m_IndexCount = value;
		}
	}

	public MeshLodRange(uint indexStart, uint indexCount)
	{
		m_IndexStart = indexStart;
		m_IndexCount = indexCount;
	}

	public override string ToString()
	{
		return $"MeshLodRange start:{m_IndexStart} count:{m_IndexCount})";
	}
}
