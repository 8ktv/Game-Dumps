namespace UnityEngine.Rendering.UnifiedRayTracing;

public class RayTracingResources
{
	internal ComputeShader geometryPoolKernels { get; set; }

	internal ComputeShader copyBuffer { get; set; }

	internal ComputeShader copyPositions { get; set; }

	internal ComputeShader bitHistogram { get; set; }

	internal ComputeShader blockReducePart { get; set; }

	internal ComputeShader blockScan { get; set; }

	internal ComputeShader buildHlbvh { get; set; }

	internal ComputeShader restructureBvh { get; set; }

	internal ComputeShader scatter { get; set; }

	public void LoadFromAssetBundle(AssetBundle assetBundle)
	{
		geometryPoolKernels = assetBundle.LoadAsset<ComputeShader>("Packages/com.unity.render-pipelines.core/Runtime/UnifiedRayTracing/Common/GeometryPool/GeometryPoolKernels.compute");
		copyBuffer = assetBundle.LoadAsset<ComputeShader>("Packages/com.unity.render-pipelines.core/Runtime/UnifiedRayTracing/Common/Utilities/CopyBuffer.compute");
		copyPositions = assetBundle.LoadAsset<ComputeShader>("Packages/com.unity.render-pipelines.core/Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/copyPositions.compute");
		bitHistogram = assetBundle.LoadAsset<ComputeShader>("Packages/com.unity.render-pipelines.core/Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/bit_histogram.compute");
		blockReducePart = assetBundle.LoadAsset<ComputeShader>("Packages/com.unity.render-pipelines.core/Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/block_reduce_part.compute");
		blockScan = assetBundle.LoadAsset<ComputeShader>("Packages/com.unity.render-pipelines.core/Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/block_scan.compute");
		buildHlbvh = assetBundle.LoadAsset<ComputeShader>("Packages/com.unity.render-pipelines.core/Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/build_hlbvh.compute");
		restructureBvh = assetBundle.LoadAsset<ComputeShader>("Packages/com.unity.render-pipelines.core/Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/restructure_bvh.compute");
		scatter = assetBundle.LoadAsset<ComputeShader>("Packages/com.unity.render-pipelines.core/Runtime/UnifiedRayTracing/Compute/RadeonRays/kernels/scatter.compute");
	}

	public bool LoadFromRenderPipelineResources()
	{
		if (!GraphicsSettings.TryGetRenderPipelineSettings<RayTracingRenderPipelineResources>(out var settings))
		{
			return false;
		}
		geometryPoolKernels = settings.GeometryPoolKernels;
		copyBuffer = settings.CopyBuffer;
		copyPositions = settings.CopyPositions;
		bitHistogram = settings.BitHistogram;
		blockReducePart = settings.BlockReducePart;
		blockScan = settings.BlockScan;
		buildHlbvh = settings.BuildHlbvh;
		restructureBvh = settings.RestructureBvh;
		scatter = settings.Scatter;
		return true;
	}
}
