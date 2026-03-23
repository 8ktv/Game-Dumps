using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering;

public struct BatchFilterSettings
{
	public uint renderingLayerMask;

	public int rendererPriority;

	private ulong m_sceneCullingMask;

	public byte layer;

	private byte m_batchLayer;

	private byte m_motionMode;

	private byte m_shadowMode;

	private byte m_receiveShadows;

	private byte m_staticShadowCaster;

	private byte m_allDepthSorted;

	private byte m_isSceneCullingMaskSet;

	public byte batchLayer
	{
		get
		{
			return m_batchLayer;
		}
		set
		{
			m_batchLayer = value;
		}
	}

	public MotionVectorGenerationMode motionMode
	{
		get
		{
			return (MotionVectorGenerationMode)m_motionMode;
		}
		set
		{
			m_motionMode = (byte)value;
		}
	}

	public ShadowCastingMode shadowCastingMode
	{
		get
		{
			return (ShadowCastingMode)m_shadowMode;
		}
		set
		{
			m_shadowMode = (byte)value;
		}
	}

	public bool receiveShadows
	{
		get
		{
			return m_receiveShadows != 0;
		}
		set
		{
			m_receiveShadows = (byte)(value ? 1u : 0u);
		}
	}

	public bool staticShadowCaster
	{
		get
		{
			return m_staticShadowCaster != 0;
		}
		set
		{
			m_staticShadowCaster = (byte)(value ? 1u : 0u);
		}
	}

	public bool allDepthSorted
	{
		get
		{
			return m_allDepthSorted != 0;
		}
		set
		{
			m_allDepthSorted = (byte)(value ? 1u : 0u);
		}
	}

	public ulong sceneCullingMask
	{
		get
		{
			return (m_isSceneCullingMaskSet != 0) ? m_sceneCullingMask : DefaultCullingMask();
		}
		set
		{
			m_isSceneCullingMaskSet = 1;
			m_sceneCullingMask = value;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("BatchFilterSettings::DefaultCullingMask", IsThreadSafe = true)]
	private static extern ulong DefaultCullingMask();
}
