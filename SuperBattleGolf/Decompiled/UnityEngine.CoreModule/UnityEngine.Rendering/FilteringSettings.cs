using System;
using UnityEngine.Internal;

namespace UnityEngine.Rendering;

public struct FilteringSettings : IEquatable<FilteringSettings>
{
	private RenderQueueRange m_RenderQueueRange;

	private int m_LayerMask;

	private uint m_RenderingLayerMask;

	private uint m_BatchLayerMask;

	private int m_ExcludeMotionVectorObjects;

	private int m_ForceAllMotionVectorObjects;

	private SortingLayerRange m_SortingLayerRange;

	public static FilteringSettings defaultValue => new FilteringSettings(RenderQueueRange.all);

	public RenderQueueRange renderQueueRange
	{
		get
		{
			return m_RenderQueueRange;
		}
		set
		{
			m_RenderQueueRange = value;
		}
	}

	public int layerMask
	{
		get
		{
			return m_LayerMask;
		}
		set
		{
			m_LayerMask = value;
		}
	}

	public uint renderingLayerMask
	{
		get
		{
			return m_RenderingLayerMask;
		}
		set
		{
			m_RenderingLayerMask = value;
		}
	}

	public uint batchLayerMask
	{
		get
		{
			return m_BatchLayerMask;
		}
		set
		{
			m_BatchLayerMask = value;
		}
	}

	public bool excludeMotionVectorObjects
	{
		get
		{
			return m_ExcludeMotionVectorObjects != 0;
		}
		set
		{
			m_ExcludeMotionVectorObjects = (value ? 1 : 0);
		}
	}

	public bool forceAllMotionVectorObjects
	{
		get
		{
			return m_ForceAllMotionVectorObjects != 0;
		}
		set
		{
			m_ForceAllMotionVectorObjects = (value ? 1 : 0);
		}
	}

	public SortingLayerRange sortingLayerRange
	{
		get
		{
			return m_SortingLayerRange;
		}
		set
		{
			m_SortingLayerRange = value;
		}
	}

	public FilteringSettings([DefaultValue("RenderQueueRange.all")] RenderQueueRange? renderQueueRange = null, int layerMask = -1, uint renderingLayerMask = uint.MaxValue, int excludeMotionVectorObjects = 0)
	{
		this = default(FilteringSettings);
		m_RenderQueueRange = renderQueueRange ?? RenderQueueRange.all;
		m_LayerMask = layerMask;
		m_RenderingLayerMask = renderingLayerMask;
		m_BatchLayerMask = uint.MaxValue;
		m_ExcludeMotionVectorObjects = excludeMotionVectorObjects;
		m_ForceAllMotionVectorObjects = 0;
		m_SortingLayerRange = SortingLayerRange.all;
	}

	public bool Equals(FilteringSettings other)
	{
		return m_RenderQueueRange.Equals(other.m_RenderQueueRange) && m_LayerMask == other.m_LayerMask && m_RenderingLayerMask == other.m_RenderingLayerMask && m_BatchLayerMask == other.m_BatchLayerMask && m_ExcludeMotionVectorObjects == other.m_ExcludeMotionVectorObjects && m_ForceAllMotionVectorObjects == other.m_ForceAllMotionVectorObjects;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return obj is FilteringSettings && Equals((FilteringSettings)obj);
	}

	public override int GetHashCode()
	{
		int hashCode = m_RenderQueueRange.GetHashCode();
		hashCode = (hashCode * 397) ^ m_LayerMask;
		hashCode = (hashCode * 397) ^ (int)m_RenderingLayerMask;
		hashCode = (hashCode * 397) ^ (int)m_BatchLayerMask;
		hashCode = (hashCode * 397) ^ m_ExcludeMotionVectorObjects;
		return (hashCode * 397) ^ m_ForceAllMotionVectorObjects;
	}

	public static bool operator ==(FilteringSettings left, FilteringSettings right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(FilteringSettings left, FilteringSettings right)
	{
		return !left.Equals(right);
	}
}
