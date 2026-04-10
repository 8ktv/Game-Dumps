using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering;

[Serializable]
public class VrsLut
{
	[SerializeField]
	private Color[] m_Data = new Color[Vrs.shadingRateFragmentSizeCount];

	private const uint Rate1x = 0u;

	private const uint Rate2x = 1u;

	private const uint Rate4x = 2u;

	public Color this[ShadingRateFragmentSize fragmentSize]
	{
		get
		{
			return m_Data[(int)fragmentSize];
		}
		set
		{
			m_Data[(int)fragmentSize] = value;
		}
	}

	public static VrsLut CreateDefault()
	{
		return new VrsLut
		{
			[ShadingRateFragmentSize.FragmentSize1x1] = new Color(0.785f, 0.23f, 0.2f, 1f),
			[ShadingRateFragmentSize.FragmentSize1x2] = new Color(1f, 0.8f, 0.8f, 1f),
			[ShadingRateFragmentSize.FragmentSize2x1] = new Color(0.4f, 0.2f, 0.2f, 1f),
			[ShadingRateFragmentSize.FragmentSize2x2] = new Color(0.51f, 0.8f, 0.6f, 1f),
			[ShadingRateFragmentSize.FragmentSize1x4] = new Color(0.6f, 0.8f, 1f, 1f),
			[ShadingRateFragmentSize.FragmentSize4x1] = new Color(0.2f, 0.4f, 0.6f, 1f),
			[ShadingRateFragmentSize.FragmentSize2x4] = new Color(0.8f, 1f, 0.8f, 1f),
			[ShadingRateFragmentSize.FragmentSize4x2] = new Color(0.2f, 0.4f, 0.2f, 1f),
			[ShadingRateFragmentSize.FragmentSize4x4] = new Color(0.125f, 0.22f, 0.36f, 1f)
		};
	}

	public GraphicsBuffer CreateBuffer(bool forVisualization = false)
	{
		Color[] array;
		if (forVisualization)
		{
			Array values = Enum.GetValues(typeof(ShadingRateFragmentSize));
			array = new Color[MapFragmentShadingRateToBinary(ShadingRateFragmentSize.FragmentSize4x4) + 1];
			for (int num = values.Length - 1; num >= 0; num--)
			{
				ShadingRateFragmentSize shadingRateFragmentSize = (ShadingRateFragmentSize)values.GetValue(num);
				byte b = ShadingRateInfo.QueryNativeValue(shadingRateFragmentSize);
				array[b] = m_Data[(int)shadingRateFragmentSize].linear;
			}
		}
		else
		{
			array = new Color[m_Data.Length];
			for (int i = 0; i < m_Data.Length; i++)
			{
				array[i] = m_Data[i].linear;
			}
		}
		GraphicsBuffer graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, array.Length, Marshal.SizeOf(typeof(Color)));
		graphicsBuffer.SetData(array);
		return graphicsBuffer;
	}

	private uint MapFragmentShadingRateToBinary(ShadingRateFragmentSize fs)
	{
		return fs switch
		{
			ShadingRateFragmentSize.FragmentSize1x2 => EncodeShadingRate(0u, 1u), 
			ShadingRateFragmentSize.FragmentSize2x1 => EncodeShadingRate(1u, 0u), 
			ShadingRateFragmentSize.FragmentSize2x2 => EncodeShadingRate(1u, 1u), 
			ShadingRateFragmentSize.FragmentSize1x4 => EncodeShadingRate(0u, 2u), 
			ShadingRateFragmentSize.FragmentSize4x1 => EncodeShadingRate(2u, 0u), 
			ShadingRateFragmentSize.FragmentSize2x4 => EncodeShadingRate(1u, 2u), 
			ShadingRateFragmentSize.FragmentSize4x2 => EncodeShadingRate(2u, 1u), 
			ShadingRateFragmentSize.FragmentSize4x4 => EncodeShadingRate(2u, 2u), 
			_ => EncodeShadingRate(0u, 0u), 
		};
	}

	private uint EncodeShadingRate(uint x, uint y)
	{
		return (x << 2) | y;
	}
}
