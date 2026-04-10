using System;
using UnityEngine.Categorization;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.UnifiedRayTracing;

[Serializable]
[MovedFrom(true, "UnityEngine.Rendering.UnifiedRayTracing", "Unity.Rendering.LightTransport.Runtime", null)]
[SupportedOnRenderPipeline(new Type[] { })]
[CategoryInfo(Name = "R: Unified Ray Tracing", Order = 1000)]
[HideInInspector]
internal class RayTracingRenderPipelineResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
{
	[SerializeField]
	[HideInInspector]
	private int m_Version = 1;

	[SerializeField]
	[ResourcePath("Runtime/UnifiedRayTracing/Common/GeometryPool/GeometryPoolKernels.compute", SearchType.ProjectPath)]
	private ComputeShader m_GeometryPoolKernels;

	[SerializeField]
	[ResourcePath("Runtime/UnifiedRayTracing/Common/Utilities/CopyBuffer.compute", SearchType.ProjectPath)]
	private ComputeShader m_CopyBuffer;

	[SerializeField]
	[ResourcePath("Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/copyPositions.compute", SearchType.ProjectPath)]
	private ComputeShader m_CopyPositions;

	[SerializeField]
	[ResourcePath("Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/bit_histogram.compute", SearchType.ProjectPath)]
	private ComputeShader m_BitHistogram;

	[SerializeField]
	[ResourcePath("Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/block_reduce_part.compute", SearchType.ProjectPath)]
	private ComputeShader m_BlockReducePart;

	[SerializeField]
	[ResourcePath("Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/block_scan.compute", SearchType.ProjectPath)]
	private ComputeShader m_BlockScan;

	[SerializeField]
	[ResourcePath("Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/build_hlbvh.compute", SearchType.ProjectPath)]
	private ComputeShader m_BuildHlbvh;

	[SerializeField]
	[ResourcePath("Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/restructure_bvh.compute", SearchType.ProjectPath)]
	private ComputeShader m_RestructureBvh;

	[SerializeField]
	[ResourcePath("Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/scatter.compute", SearchType.ProjectPath)]
	private ComputeShader m_Scatter;

	public int version => m_Version;

	public ComputeShader GeometryPoolKernels
	{
		get
		{
			return m_GeometryPoolKernels;
		}
		set
		{
			this.SetValueAndNotify(ref m_GeometryPoolKernels, value, "m_GeometryPoolKernels");
		}
	}

	public ComputeShader CopyBuffer
	{
		get
		{
			return m_CopyBuffer;
		}
		set
		{
			this.SetValueAndNotify(ref m_CopyBuffer, value, "m_CopyBuffer");
		}
	}

	public ComputeShader CopyPositions
	{
		get
		{
			return m_CopyPositions;
		}
		set
		{
			this.SetValueAndNotify(ref m_CopyPositions, value, "m_CopyPositions");
		}
	}

	public ComputeShader BitHistogram
	{
		get
		{
			return m_BitHistogram;
		}
		set
		{
			this.SetValueAndNotify(ref m_BitHistogram, value, "m_BitHistogram");
		}
	}

	public ComputeShader BlockReducePart
	{
		get
		{
			return m_BlockReducePart;
		}
		set
		{
			this.SetValueAndNotify(ref m_BlockReducePart, value, "m_BlockReducePart");
		}
	}

	public ComputeShader BlockScan
	{
		get
		{
			return m_BlockScan;
		}
		set
		{
			this.SetValueAndNotify(ref m_BlockScan, value, "m_BlockScan");
		}
	}

	public ComputeShader BuildHlbvh
	{
		get
		{
			return m_BuildHlbvh;
		}
		set
		{
			this.SetValueAndNotify(ref m_BuildHlbvh, value, "m_BuildHlbvh");
		}
	}

	public ComputeShader RestructureBvh
	{
		get
		{
			return m_RestructureBvh;
		}
		set
		{
			this.SetValueAndNotify(ref m_RestructureBvh, value, "m_RestructureBvh");
		}
	}

	public ComputeShader Scatter
	{
		get
		{
			return m_Scatter;
		}
		set
		{
			this.SetValueAndNotify(ref m_Scatter, value, "m_Scatter");
		}
	}
}
