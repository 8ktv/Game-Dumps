using System;
using UnityEngine;
using UnityEngine.Rendering;

public class DrivingRangeStaticCamera : MonoBehaviour
{
	[SerializeField]
	private LayerMask dynamicLayers;

	[SerializeField]
	private Transform cameraParent;

	[SerializeField]
	private Rotator[] rotators;

	[SerializeField]
	private Camera thisCamera;

	[SerializeField]
	private Shader depthBlitShader;

	private bool callbacksActive;

	private bool renderFrame = true;

	private float lastFrameRender;

	private RenderTexture staticRenderTexture;

	private RenderTexture staticDepthTexture;

	private bool hasTextures;

	private bool activeInternal;

	private bool isRendering;

	private static Material depthBlitMaterial;

	private static readonly int _DepthTexture = Shader.PropertyToID("_DepthTexture");

	private static readonly int _CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");

	public bool IsActive => activeInternal;

	public bool IsRendering => isRendering;

	private void Awake()
	{
		SetCameraActive(isActive: false, force: true);
		SetCameraRendering(render: false, force: true);
	}

	private void OnDestroy()
	{
		SetCameraCallbacks(enabled: false);
		CleanUpRenderTextures();
	}

	public void SetCameraActive(bool isActive, bool force = false)
	{
		if (force || activeInternal != isActive)
		{
			for (int i = 0; i < rotators.Length; i++)
			{
				rotators[i].enabled = isActive;
			}
			cameraParent.gameObject.SetActive(isActive);
			SetCameraCallbacks(isActive);
			activeInternal = isActive;
		}
	}

	public void SetCameraRendering(bool render, bool force = false)
	{
		if (force || render != isRendering)
		{
			thisCamera.enabled = render;
			isRendering = render;
		}
	}

	private void CleanUpRenderTextures()
	{
		if (staticRenderTexture != null)
		{
			UnityEngine.Object.Destroy(staticRenderTexture);
		}
		if (staticDepthTexture != null)
		{
			UnityEngine.Object.Destroy(staticDepthTexture);
		}
	}

	private void SetCameraCallbacks(bool enabled)
	{
		if (callbacksActive != enabled)
		{
			if (enabled)
			{
				Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(PreRender));
				Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(PostRender));
			}
			else
			{
				Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(PreRender));
				Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(PostRender));
			}
			callbacksActive = enabled;
		}
	}

	public void RenderStaticTexture()
	{
		RenderTexture targetTexture = thisCamera.targetTexture;
		staticRenderTexture = new RenderTexture(targetTexture.descriptor);
		staticRenderTexture.antiAliasing = 4;
		staticRenderTexture.Create();
		staticDepthTexture = new RenderTexture(targetTexture.width, targetTexture.height, 24, RenderTextureFormat.RFloat);
		staticDepthTexture.Create();
		QualitySettings.shadows = (GameSettings.All.Graphics.ShadowsEnabled ? ShadowQuality.All : ShadowQuality.Disable);
		thisCamera.clearFlags = CameraClearFlags.Skybox;
		thisCamera.cullingMask = ~(int)dynamicLayers;
		thisCamera.targetTexture = staticRenderTexture;
		thisCamera.allowMSAA = true;
		thisCamera.depthTextureMode = DepthTextureMode.Depth;
		CommandBuffer commandBuffer = new CommandBuffer();
		commandBuffer.Blit(BuiltinRenderTextureType.Depth, staticDepthTexture);
		thisCamera.AddCommandBuffer(CameraEvent.AfterDepthTexture, commandBuffer);
		thisCamera.Render();
		thisCamera.RemoveAllCommandBuffers();
		if (depthBlitMaterial == null)
		{
			depthBlitMaterial = new Material(depthBlitShader);
		}
		commandBuffer.Clear();
		commandBuffer.Blit(staticRenderTexture, BuiltinRenderTextureType.CameraTarget, depthBlitMaterial);
		thisCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer);
		thisCamera.allowMSAA = false;
		thisCamera.targetTexture = targetTexture;
		thisCamera.cullingMask = dynamicLayers;
		thisCamera.depthTextureMode = DepthTextureMode.None;
		thisCamera.clearFlags = CameraClearFlags.Nothing;
		hasTextures = true;
	}

	private void PreRender(Camera camera)
	{
		if (!(camera != thisCamera) && !(staticDepthTexture == null) && !(depthBlitMaterial == null))
		{
			QualitySettings.shadows = ShadowQuality.Disable;
			depthBlitMaterial.SetTexture(_DepthTexture, staticDepthTexture);
			Shader.SetGlobalTexture(_CameraDepthTexture, staticDepthTexture);
		}
	}

	private void PostRender(Camera camera)
	{
		if (!(camera != thisCamera))
		{
			QualitySettings.shadows = (GameSettings.All.Graphics.ShadowsEnabled ? ShadowQuality.All : ShadowQuality.Disable);
		}
	}
}
