#define DEBUG_DRAW
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenSpacePlanarReflections : MonoBehaviour
{
	private static readonly List<SsprPlane> activePlanes = new List<SsprPlane>();

	private static bool planesOrdered;

	public ComputeShader ssprShader;

	public float planeInterval = 0.5f;

	public int downscale = 4;

	public int maxPlaneCount = 32;

	private Camera camera;

	private CommandBuffer commandBuffer;

	private RenderTexture resultRenderTexture;

	private ComputeBuffer dispatchBuffer;

	private float[] planeHeights;

	private readonly float[] planeLookup = new float[256];

	private readonly Plane[] frustumPlaneCache = new Plane[6];

	private readonly int[] dispatchCountCache = new int[4];

	private bool initialized;

	private GlobalKeyword shaderEnable;

	[CVar("ssprDebug", "", "", false, true)]
	private static bool debug;

	private void Awake()
	{
		camera = GetComponent<Camera>();
		camera.depthTextureMode |= DepthTextureMode.Depth;
		shaderEnable = GlobalKeyword.Create("SSPR_ENABLE");
	}

	private void Start()
	{
		planeHeights = new float[maxPlaneCount];
		dispatchBuffer = new ComputeBuffer(4, 16, ComputeBufferType.DrawIndirect);
		UpdateRtIfNeeded();
		RebuildCommandBuffer();
		Shader.SetGlobalFloatArray("_SSPR_PLANES", planeHeights);
		GameSettings.GraphicsSettings.OnGraphicsQualityApply += UpdateQuality;
		UpdateQuality();
		initialized = true;
	}

	private void OnEnable()
	{
		Shader.SetKeyword(in shaderEnable, value: true);
	}

	private void OnDisable()
	{
		Shader.SetKeyword(in shaderEnable, value: false);
		resultRenderTexture.Release();
		resultRenderTexture = null;
		camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer);
		commandBuffer.Clear();
		commandBuffer = null;
	}

	private void OnDestroy()
	{
		ClearPlanes();
		if (initialized)
		{
			dispatchBuffer.Release();
			dispatchBuffer.Dispose();
			GameSettings.GraphicsSettings.OnGraphicsQualityApply -= UpdateQuality;
		}
	}

	private void UpdateQuality()
	{
		base.enabled = GameSettings.All.Graphics.ScreenSpaceReflectionsQuality != GameSettings.GraphicsSettings.Quality.Off;
		if (base.enabled)
		{
			downscale = BMath.Max(1, (int)GameSettings.All.Graphics.ScreenSpaceReflectionsQuality * 2);
		}
	}

	private void Update()
	{
		if (UpdateRtIfNeeded())
		{
			RebuildCommandBuffer();
		}
	}

	public static void ClearPlanes()
	{
		activePlanes.Clear();
	}

	public static SsprPlane AddPlane(float height, Bounds bounds)
	{
		Vector3 size = bounds.size;
		size.y = BMath.Max(size.y, 1f);
		bounds.size = size;
		SsprPlane ssprPlane = new SsprPlane
		{
			height = height,
			bounds = bounds
		};
		activePlanes.Add(ssprPlane);
		planesOrdered = false;
		return ssprPlane;
	}

	private void RebuildCommandBuffer()
	{
		CameraEvent evt = CameraEvent.BeforeImageEffectsOpaque;
		if (commandBuffer == null)
		{
			commandBuffer = new CommandBuffer();
			commandBuffer.name = "Screen Space Planar Reflections";
		}
		else
		{
			commandBuffer.Clear();
			camera.RemoveCommandBuffer(evt, commandBuffer);
		}
		int num = Shader.PropertyToID("_SSPRTempRt1");
		int num2 = Shader.PropertyToID("_SSPRGrab");
		RenderTargetIdentifier rt = new RenderTargetIdentifier(resultRenderTexture);
		int kernelIndex = ssprShader.FindKernel("Clear");
		int kernelIndex2 = ssprShader.FindKernel("ProjectHash");
		int kernelIndex3 = ssprShader.FindKernel("Reconstruct");
		int width = resultRenderTexture.width;
		int height = resultRenderTexture.height;
		RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height, RenderTextureFormat.RInt, 0, 0);
		desc.enableRandomWrite = true;
		desc.dimension = TextureDimension.Tex2DArray;
		desc.volumeDepth = maxPlaneCount;
		desc.msaaSamples = 1;
		commandBuffer.GetTemporaryRT(num, desc);
		commandBuffer.GetTemporaryRT(num2, width, height, 0, FilterMode.Point, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1, enableRandomWrite: false);
		commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, num2);
		Vector3Int dispatchCount = GetDispatchCount(1);
		commandBuffer.SetComputeTextureParam(ssprShader, kernelIndex, "HashOut", num);
		commandBuffer.SetComputeTextureParam(ssprShader, kernelIndex, "ColorOut", rt);
		commandBuffer.DispatchCompute(ssprShader, kernelIndex, dispatchCount.x, dispatchCount.y, 1);
		Vector4 val = new Vector4(width, height, 1f / (float)width, 1f / (float)height);
		commandBuffer.SetComputeVectorParam(ssprShader, "ViewDir", base.transform.forward);
		commandBuffer.SetComputeVectorParam(ssprShader, "TexSize", val);
		commandBuffer.SetComputeTextureParam(ssprShader, kernelIndex2, "HashOut", num);
		commandBuffer.DispatchCompute(ssprShader, kernelIndex2, dispatchBuffer, 0u);
		commandBuffer.SetComputeTextureParam(ssprShader, kernelIndex3, "_ColorSrc", num2);
		commandBuffer.SetComputeTextureParam(ssprShader, kernelIndex3, "ColorOut", rt);
		commandBuffer.SetComputeTextureParam(ssprShader, kernelIndex3, "HashIn", num);
		commandBuffer.DispatchCompute(ssprShader, kernelIndex3, dispatchBuffer, 0u);
		commandBuffer.ReleaseTemporaryRT(num);
		commandBuffer.ReleaseTemporaryRT(num2);
		camera.AddCommandBuffer(evt, commandBuffer);
	}

	private void OnPreCull()
	{
		if (GameManager.Camera == null)
		{
			return;
		}
		float y = camera.transform.position.y;
		float num = 0f;
		float num2 = float.MaxValue;
		int num3 = 0;
		if (!planesOrdered)
		{
			activePlanes.Sort(CompareReflectionPlanes);
			planesOrdered = true;
		}
		GeometryUtility.CalculateFrustumPlanes(GameManager.Camera, frustumPlaneCache);
		foreach (SsprPlane activePlane in activePlanes)
		{
			if (activePlane.height >= y || activePlane.height >= num2)
			{
				continue;
			}
			if (debug)
			{
				BDebug.DrawWireCube(activePlane.bounds.center, activePlane.bounds.size, Quaternion.identity, Color.blue);
			}
			if (!GeometryUtility.TestPlanesAABB(frustumPlaneCache, activePlane.bounds))
			{
				activePlane.index = -1;
				continue;
			}
			num = BMath.Max(num, activePlane.height);
			num2 = activePlane.height;
			if (num3 < maxPlaneCount)
			{
				activePlane.index = num3;
				planeHeights[num3] = activePlane.height;
				num3++;
			}
		}
		float num4 = BMath.Ceil(num / planeInterval);
		for (int i = 0; i < num3; i++)
		{
			int num5 = (int)num4 - BMath.FloorToInt(planeHeights[i] / planeInterval);
			planeLookup[num5] = i;
		}
		Vector3Int dispatchCount = GetDispatchCount(num3);
		dispatchCountCache[0] = dispatchCount.x;
		dispatchCountCache[1] = dispatchCount.y;
		dispatchCountCache[2] = dispatchCount.z;
		dispatchBuffer.SetData(dispatchCountCache);
		Shader.SetGlobalVector("_SSPR_PARAMS", new Vector4(num4, num4 * planeInterval, planeInterval, 1f / planeInterval));
		Shader.SetGlobalMatrix("_SSPR_I_V", camera.cameraToWorldMatrix);
		Shader.SetGlobalMatrix("_SSPR_I_P", camera.projectionMatrix.inverse);
		Shader.SetGlobalFloatArray("_SSPR_PLANES", planeHeights);
		Shader.SetGlobalFloatArray("_SSPR_PLANELOOKUP", planeLookup);
	}

	private Vector3Int GetDispatchCount(int planeCount)
	{
		return new Vector3Int((resultRenderTexture.width + 7) / 8, (resultRenderTexture.height + 7) / 8, planeCount);
	}

	private bool UpdateRtIfNeeded(bool verbose = false)
	{
		int num = (Screen.width + downscale - 1) / downscale;
		int num2 = (Screen.height + downscale - 1) / downscale;
		if (resultRenderTexture == null || resultRenderTexture.width != num || resultRenderTexture.height != num2)
		{
			if (resultRenderTexture != null)
			{
				resultRenderTexture.Release();
			}
			resultRenderTexture = new RenderTexture(num, num2, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
			resultRenderTexture.name = "Screenspace planar reflections";
			resultRenderTexture.autoGenerateMips = false;
			resultRenderTexture.useMipMap = false;
			resultRenderTexture.enableRandomWrite = true;
			resultRenderTexture.dimension = TextureDimension.Tex2DArray;
			resultRenderTexture.volumeDepth = maxPlaneCount;
			resultRenderTexture.antiAliasing = 1;
			resultRenderTexture.Create();
			Shader.SetGlobalTexture("_SSPRTexResult", resultRenderTexture);
			if (verbose)
			{
				Debug.Log("SSPR texture updated!");
			}
			return true;
		}
		return false;
	}

	private static int CompareReflectionPlanes(SsprPlane a, SsprPlane b)
	{
		if (a.height == b.height)
		{
			return 0;
		}
		if (a.height > b.height)
		{
			return -1;
		}
		return 1;
	}
}
