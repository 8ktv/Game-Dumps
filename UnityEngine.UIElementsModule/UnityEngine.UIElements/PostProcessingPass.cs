using System;

namespace UnityEngine.UIElements;

[Serializable]
internal struct PostProcessingPass
{
	public delegate void PrepareMaterialPropertyBlockDelegate(MaterialPropertyBlock mpb, FilterFunction func);

	public delegate PostProcessingMargins ComputeRequiredMarginsDelegate(FilterFunction func);

	[SerializeField]
	private Material m_Material;

	[SerializeField]
	private int m_PassIndex;

	[SerializeField]
	private ParameterBinding[] m_ParameterBindings;

	[SerializeField]
	private PostProcessingMargins m_ReadMargins;

	[SerializeField]
	private PostProcessingMargins m_WriteMargins;

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

	public PrepareMaterialPropertyBlockDelegate prepareMaterialPropertyBlockCallback { get; set; }

	public ComputeRequiredMarginsDelegate computeRequiredReadMarginsCallback { get; set; }

	public ComputeRequiredMarginsDelegate computeRequiredWriteMarginsCallback { get; set; }
}
