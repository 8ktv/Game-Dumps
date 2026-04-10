using System;
using Unity.Properties;

namespace UnityEngine.UIElements;

[Serializable]
public struct PostProcessingPass
{
	[Obsolete("This delegate will be removed. Use ApplyFilterPassSettingsDelegate instead.")]
	public delegate void PrepareMaterialPropertyBlockDelegate(MaterialPropertyBlock mpb, FilterFunction func);

	public delegate void ApplyFilterPassSettingsDelegate(MaterialPropertyBlock mpb, FilterPassContext context);

	public delegate PostProcessingMargins ComputeRequiredMarginsDelegate(FilterFunction func);

	[DontCreateProperty]
	[SerializeField]
	private Material m_Material;

	[DontCreateProperty]
	[SerializeField]
	private int m_PassIndex;

	[DontCreateProperty]
	[SerializeField]
	private ParameterBinding[] m_ParameterBindings;

	[SerializeField]
	private PostProcessingMargins m_ReadMargins;

	[SerializeField]
	[DontCreateProperty]
	private PostProcessingMargins m_WriteMargins;

	[CreateProperty]
	public Material material
	{
		get
		{
			return m_Material;
		}
		set
		{
			m_Material = value;
		}
	}

	[CreateProperty]
	public int passIndex
	{
		get
		{
			return m_PassIndex;
		}
		set
		{
			m_PassIndex = value;
		}
	}

	[CreateProperty]
	public ParameterBinding[] parameterBindings
	{
		get
		{
			return m_ParameterBindings;
		}
		set
		{
			m_ParameterBindings = value;
		}
	}

	internal PostProcessingMargins readMargins
	{
		get
		{
			return m_ReadMargins;
		}
		set
		{
			m_ReadMargins = value;
		}
	}

	[CreateProperty]
	public PostProcessingMargins writeMargins
	{
		get
		{
			return m_WriteMargins;
		}
		set
		{
			m_WriteMargins = value;
		}
	}

	[Obsolete("This property will be removed. Use applySettingsCallback instead, which provides additional contextual information.")]
	public PrepareMaterialPropertyBlockDelegate prepareMaterialPropertyBlockCallback { get; set; }

	public ApplyFilterPassSettingsDelegate applySettingsCallback { get; set; }

	public ComputeRequiredMarginsDelegate computeRequiredReadMarginsCallback { get; set; }

	public ComputeRequiredMarginsDelegate computeRequiredWriteMarginsCallback { get; set; }
}
