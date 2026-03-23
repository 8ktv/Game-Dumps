using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Rendering.RendererUtils;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Graphics/ScriptableRenderLoop/ScriptableDrawRenderersUtility.h")]
[NativeHeader("Modules/UI/Canvas.h")]
[NativeHeader("Runtime/Export/RenderPipeline/ScriptableRenderPipeline.bindings.h")]
[NativeHeader("Runtime/Export/RenderPipeline/ScriptableRenderContext.bindings.h")]
[NativeHeader("Modules/UI/CanvasManager.h")]
[NativeType("Runtime/Graphics/ScriptableRenderLoop/ScriptableRenderContext.h")]
public struct ScriptableRenderContext : IEquatable<ScriptableRenderContext>
{
	internal enum SkyboxXRMode
	{
		Off,
		Enabled,
		LegacySinglePass
	}

	private struct CullShadowCastersContext
	{
		public IntPtr cullResults;

		public unsafe ShadowSplitData* splitBuffer;

		public int splitBufferLength;

		public unsafe LightShadowCasterCullingInfo* perLightInfos;

		public int perLightInfoCount;
	}

	private static readonly ShaderTagId kRenderTypeTag = new ShaderTagId("RenderType");

	private IntPtr m_Ptr;

	private const bool deprecateDrawXmethods = false;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptableRenderContext::BeginRenderPass")]
	private static extern void BeginRenderPass_Internal(IntPtr self, int width, int height, int volumeDepth, int samples, IntPtr colors, int colorCount, int depthAttachmentIndex, int shadingRateImageAttachmentIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptableRenderContext::BeginSubPass")]
	private static extern void BeginSubPass_Internal(IntPtr self, IntPtr colors, int colorCount, IntPtr inputs, int inputCount, bool isDepthReadOnly, bool isStencilReadOnly);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptableRenderContext::EndSubPass")]
	private static extern void EndSubPass_Internal(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptableRenderContext::EndRenderPass")]
	private static extern void EndRenderPass_Internal(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptableRenderContext::HasInvokeOnRenderObjectCallbacks")]
	private static extern bool HasInvokeOnRenderObjectCallbacks_Internal();

	[FreeFunction("ScriptableRenderPipeline_Bindings::Internal_Cull")]
	private static void Internal_Cull(ref ScriptableCullingParameters parameters, ScriptableRenderContext renderLoop, IntPtr results)
	{
		Internal_Cull_Injected(ref parameters, ref renderLoop, results);
	}

	[FreeFunction("ScriptableRenderPipeline_Bindings::Internal_CullShadowCasters")]
	private static void Internal_CullShadowCasters(ScriptableRenderContext renderLoop, IntPtr context)
	{
		Internal_CullShadowCasters_Injected(ref renderLoop, context);
	}

	[FreeFunction("InitializeSortSettings")]
	internal static void InitializeSortSettings(Camera camera, out SortingSettings sortingSettings)
	{
		InitializeSortSettings_Injected(Object.MarshalledUnityObject.Marshal(camera), out sortingSettings);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptableRenderContext::PushDisableApiRenderers")]
	public static extern void PushDisableApiRenderers();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptableRenderContext::PopDisableApiRenderers")]
	public static extern void PopDisableApiRenderers();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void Submit_Internal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool SubmitForRenderPassValidation_Internal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetCameras_Internal(Type listType, object resultList);

	private void DrawRenderers_Internal(IntPtr cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ShaderTagId tagName, bool isPassTagName, IntPtr tagValues, IntPtr stateBlocks, int stateCount)
	{
		DrawRenderers_Internal_Injected(ref this, cullResults, ref drawingSettings, ref filteringSettings, ref tagName, isPassTagName, tagValues, stateBlocks, stateCount);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void DrawShadows_Internal(IntPtr shadowDrawingSettings);

	[FreeFunction("PlayerEmitCanvasGeometryForCamera")]
	public static void EmitGeometryForCamera(Camera camera)
	{
		EmitGeometryForCamera_Injected(Object.MarshalledUnityObject.Marshal(camera));
	}

	[NativeThrows]
	private void ExecuteCommandBuffer_Internal(CommandBuffer commandBuffer)
	{
		ExecuteCommandBuffer_Internal_Injected(ref this, (commandBuffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(commandBuffer));
	}

	[NativeThrows]
	private void ExecuteCommandBufferAsync_Internal(CommandBuffer commandBuffer, ComputeQueueType queueType)
	{
		ExecuteCommandBufferAsync_Internal_Injected(ref this, (commandBuffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(commandBuffer), queueType);
	}

	private void SetupCameraProperties_Internal([NotNull] Camera camera, bool stereoSetup, int eye)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		SetupCameraProperties_Internal_Injected(ref this, intPtr, stereoSetup, eye);
	}

	private void StereoEndRender_Internal([NotNull] Camera camera, int eye, bool isFinalPass)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		StereoEndRender_Internal_Injected(ref this, intPtr, eye, isFinalPass);
	}

	private void StartMultiEye_Internal([NotNull] Camera camera, int eye)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		StartMultiEye_Internal_Injected(ref this, intPtr, eye);
	}

	private void StopMultiEye_Internal([NotNull] Camera camera)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		StopMultiEye_Internal_Injected(ref this, intPtr);
	}

	private void DrawSkybox_Internal([NotNull] Camera camera)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		DrawSkybox_Internal_Injected(ref this, intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void InvokeOnRenderObjectCallback_Internal();

	private void DrawGizmos_Internal([NotNull] Camera camera, GizmoSubset gizmoSubset)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		DrawGizmos_Internal_Injected(ref this, intPtr, gizmoSubset);
	}

	private void DrawWireOverlay_Impl([NotNull] Camera camera)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		DrawWireOverlay_Impl_Injected(ref this, intPtr);
	}

	private void DrawUIOverlay_Internal([NotNull] Camera camera)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		DrawUIOverlay_Internal_Injected(ref this, intPtr);
	}

	internal IntPtr Internal_GetPtr()
	{
		return m_Ptr;
	}

	private RendererList CreateRendererList_Internal(IntPtr cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ShaderTagId tagName, bool isPassTagName, IntPtr tagValues, IntPtr stateBlocks, int stateCount)
	{
		CreateRendererList_Internal_Injected(ref this, cullResults, ref drawingSettings, ref filteringSettings, ref tagName, isPassTagName, tagValues, stateBlocks, stateCount, out var ret);
		return ret;
	}

	private RendererList CreateShadowRendererList_Internal(IntPtr shadowDrawinSettings)
	{
		CreateShadowRendererList_Internal_Injected(ref this, shadowDrawinSettings, out var ret);
		return ret;
	}

	private RendererList CreateSkyboxRendererList_Internal([NotNull] Camera camera, int mode, Matrix4x4 proj, Matrix4x4 view, Matrix4x4 projR, Matrix4x4 viewR)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		CreateSkyboxRendererList_Internal_Injected(ref this, intPtr, mode, ref proj, ref view, ref projR, ref viewR, out var ret);
		return ret;
	}

	private RendererList CreateGizmoRendererList_Internal([NotNull] Camera camera, GizmoSubset gizmoSubset)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		CreateGizmoRendererList_Internal_Injected(ref this, intPtr, gizmoSubset, out var ret);
		return ret;
	}

	private RendererList CreateUIOverlayRendererList_Internal([NotNull] Camera camera, UISubset uiSubset)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		CreateUIOverlayRendererList_Internal_Injected(ref this, intPtr, uiSubset, out var ret);
		return ret;
	}

	private RendererList CreateWireOverlayRendererList_Internal([NotNull] Camera camera)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		CreateWireOverlayRendererList_Internal_Injected(ref this, intPtr, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void PrepareRendererListsAsync_Internal(object rendererLists);

	private RendererListStatus QueryRendererListStatus_Internal(RendererList handle)
	{
		return QueryRendererListStatus_Internal_Injected(ref this, ref handle);
	}

	internal ScriptableRenderContext(IntPtr ptr)
	{
		m_Ptr = ptr;
	}

	public unsafe void BeginRenderPass(int width, int height, int volumeDepth, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex)
	{
		BeginRenderPass_Internal(m_Ptr, width, height, volumeDepth, samples, (IntPtr)attachments.GetUnsafeReadOnlyPtr(), attachments.Length, depthAttachmentIndex, shadingRateImageAttachmentIndex);
	}

	public unsafe void BeginRenderPass(int width, int height, int volumeDepth, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex = -1)
	{
		BeginRenderPass_Internal(m_Ptr, width, height, volumeDepth, samples, (IntPtr)attachments.GetUnsafeReadOnlyPtr(), attachments.Length, depthAttachmentIndex, -1);
	}

	public unsafe void BeginRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex)
	{
		BeginRenderPass_Internal(m_Ptr, width, height, 1, samples, (IntPtr)attachments.GetUnsafeReadOnlyPtr(), attachments.Length, depthAttachmentIndex, shadingRateImageAttachmentIndex);
	}

	public unsafe void BeginRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex = -1)
	{
		BeginRenderPass_Internal(m_Ptr, width, height, 1, samples, (IntPtr)attachments.GetUnsafeReadOnlyPtr(), attachments.Length, depthAttachmentIndex, -1);
	}

	public ScopedRenderPass BeginScopedRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex)
	{
		BeginRenderPass(width, height, samples, attachments, depthAttachmentIndex, shadingRateImageAttachmentIndex);
		return new ScopedRenderPass(this);
	}

	public ScopedRenderPass BeginScopedRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex = -1)
	{
		BeginRenderPass(width, height, samples, attachments, depthAttachmentIndex, -1);
		return new ScopedRenderPass(this);
	}

	public unsafe void BeginSubPass(NativeArray<int> colors, NativeArray<int> inputs, bool isDepthReadOnly, bool isStencilReadOnly)
	{
		BeginSubPass_Internal(m_Ptr, (IntPtr)colors.GetUnsafeReadOnlyPtr(), colors.Length, (IntPtr)inputs.GetUnsafeReadOnlyPtr(), inputs.Length, isDepthReadOnly, isStencilReadOnly);
	}

	public unsafe void BeginSubPass(NativeArray<int> colors, NativeArray<int> inputs, bool isDepthStencilReadOnly = false)
	{
		BeginSubPass_Internal(m_Ptr, (IntPtr)colors.GetUnsafeReadOnlyPtr(), colors.Length, (IntPtr)inputs.GetUnsafeReadOnlyPtr(), inputs.Length, isDepthStencilReadOnly, isDepthStencilReadOnly);
	}

	public unsafe void BeginSubPass(NativeArray<int> colors, bool isDepthReadOnly, bool isStencilReadOnly)
	{
		BeginSubPass_Internal(m_Ptr, (IntPtr)colors.GetUnsafeReadOnlyPtr(), colors.Length, IntPtr.Zero, 0, isDepthReadOnly, isStencilReadOnly);
	}

	public unsafe void BeginSubPass(NativeArray<int> colors, bool isDepthStencilReadOnly = false)
	{
		BeginSubPass_Internal(m_Ptr, (IntPtr)colors.GetUnsafeReadOnlyPtr(), colors.Length, IntPtr.Zero, 0, isDepthStencilReadOnly, isDepthStencilReadOnly);
	}

	public ScopedSubPass BeginScopedSubPass(NativeArray<int> colors, NativeArray<int> inputs, bool isDepthReadOnly, bool isStencilReadOnly)
	{
		BeginSubPass(colors, inputs, isDepthReadOnly, isStencilReadOnly);
		return new ScopedSubPass(this);
	}

	public ScopedSubPass BeginScopedSubPass(NativeArray<int> colors, NativeArray<int> inputs, bool isDepthStencilReadOnly = false)
	{
		BeginSubPass(colors, inputs, isDepthStencilReadOnly);
		return new ScopedSubPass(this);
	}

	public ScopedSubPass BeginScopedSubPass(NativeArray<int> colors, bool isDepthReadOnly, bool isStencilReadOnly)
	{
		BeginSubPass(colors, isDepthReadOnly, isStencilReadOnly);
		return new ScopedSubPass(this);
	}

	public ScopedSubPass BeginScopedSubPass(NativeArray<int> colors, bool isDepthStencilReadOnly = false)
	{
		BeginSubPass(colors, isDepthStencilReadOnly);
		return new ScopedSubPass(this);
	}

	public void EndSubPass()
	{
		EndSubPass_Internal(m_Ptr);
	}

	public void EndRenderPass()
	{
		EndRenderPass_Internal(m_Ptr);
	}

	public void Submit()
	{
		Submit_Internal();
	}

	public bool SubmitForRenderPassValidation()
	{
		return SubmitForRenderPassValidation_Internal();
	}

	public bool HasInvokeOnRenderObjectCallbacks()
	{
		return HasInvokeOnRenderObjectCallbacks_Internal();
	}

	internal void GetCameras(List<Camera> results)
	{
		GetCameras_Internal(typeof(Camera), results);
	}

	[Obsolete("DrawRenderers is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
	public void DrawRenderers(CullingResults cullingResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings)
	{
		DrawRenderers_Internal(cullingResults.ptr, ref drawingSettings, ref filteringSettings, ShaderTagId.none, isPassTagName: false, IntPtr.Zero, IntPtr.Zero, 0);
	}

	[Obsolete("DrawRenderers is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
	public unsafe void DrawRenderers(CullingResults cullingResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ref RenderStateBlock stateBlock)
	{
		ShaderTagId shaderTagId = default(ShaderTagId);
		fixed (RenderStateBlock* ptr = &stateBlock)
		{
			DrawRenderers_Internal(cullingResults.ptr, ref drawingSettings, ref filteringSettings, ShaderTagId.none, isPassTagName: false, (IntPtr)(&shaderTagId), (IntPtr)ptr, 1);
		}
	}

	[Obsolete("DrawRenderers is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
	public unsafe void DrawRenderers(CullingResults cullingResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, NativeArray<ShaderTagId> renderTypes, NativeArray<RenderStateBlock> stateBlocks)
	{
		if (renderTypes.Length != stateBlocks.Length)
		{
			throw new ArgumentException(string.Format("Arrays {0} and {1} should have same length, but {2} had length {3} while {4} had length {5}.", "renderTypes", "stateBlocks", "renderTypes", renderTypes.Length, "stateBlocks", stateBlocks.Length));
		}
		DrawRenderers_Internal(cullingResults.ptr, ref drawingSettings, ref filteringSettings, kRenderTypeTag, isPassTagName: false, (IntPtr)renderTypes.GetUnsafeReadOnlyPtr(), (IntPtr)stateBlocks.GetUnsafeReadOnlyPtr(), renderTypes.Length);
	}

	[Obsolete("DrawRenderers is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
	public unsafe void DrawRenderers(CullingResults cullingResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ShaderTagId tagName, bool isPassTagName, NativeArray<ShaderTagId> tagValues, NativeArray<RenderStateBlock> stateBlocks)
	{
		if (tagValues.Length != stateBlocks.Length)
		{
			throw new ArgumentException(string.Format("Arrays {0} and {1} should have same length, but {2} had length {3} while {4} had length {5}.", "tagValues", "stateBlocks", "tagValues", tagValues.Length, "stateBlocks", stateBlocks.Length));
		}
		DrawRenderers_Internal(cullingResults.ptr, ref drawingSettings, ref filteringSettings, tagName, isPassTagName, (IntPtr)tagValues.GetUnsafeReadOnlyPtr(), (IntPtr)stateBlocks.GetUnsafeReadOnlyPtr(), tagValues.Length);
	}

	[Obsolete("DrawShadows is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateShadowRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
	public unsafe void DrawShadows(ref ShadowDrawingSettings settings)
	{
		fixed (ShadowDrawingSettings* ptr = &settings)
		{
			DrawShadows_Internal((IntPtr)ptr);
		}
	}

	public void ExecuteCommandBuffer(CommandBuffer commandBuffer)
	{
		if (commandBuffer == null)
		{
			throw new ArgumentNullException("commandBuffer");
		}
		if (commandBuffer.m_Ptr == IntPtr.Zero)
		{
			throw new ObjectDisposedException("commandBuffer");
		}
		ExecuteCommandBuffer_Internal(commandBuffer);
	}

	public void ExecuteCommandBufferAsync(CommandBuffer commandBuffer, ComputeQueueType queueType)
	{
		if (commandBuffer == null)
		{
			throw new ArgumentNullException("commandBuffer");
		}
		if (commandBuffer.m_Ptr == IntPtr.Zero)
		{
			throw new ObjectDisposedException("commandBuffer");
		}
		ExecuteCommandBufferAsync_Internal(commandBuffer, queueType);
	}

	public void SetupCameraProperties(Camera camera, bool stereoSetup = false)
	{
		SetupCameraProperties(camera, stereoSetup, 0);
	}

	public void SetupCameraProperties(Camera camera, bool stereoSetup, int eye)
	{
		SetupCameraProperties_Internal(camera, stereoSetup, eye);
	}

	public void StereoEndRender(Camera camera)
	{
		StereoEndRender(camera, 0, isFinalPass: true);
	}

	public void StereoEndRender(Camera camera, int eye)
	{
		StereoEndRender(camera, eye, isFinalPass: true);
	}

	public void StereoEndRender(Camera camera, int eye, bool isFinalPass)
	{
		StereoEndRender_Internal(camera, eye, isFinalPass);
	}

	public void StartMultiEye(Camera camera)
	{
		StartMultiEye(camera, 0);
	}

	public void StartMultiEye(Camera camera, int eye)
	{
		StartMultiEye_Internal(camera, eye);
	}

	public void StopMultiEye(Camera camera)
	{
		StopMultiEye_Internal(camera);
	}

	[Obsolete("DrawSkybox is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateSkyboxRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
	public void DrawSkybox(Camera camera)
	{
		DrawSkybox_Internal(camera);
	}

	public void InvokeOnRenderObjectCallback()
	{
		InvokeOnRenderObjectCallback_Internal();
	}

	public void DrawGizmos(Camera camera, GizmoSubset gizmoSubset)
	{
		DrawGizmos_Internal(camera, gizmoSubset);
	}

	public void DrawWireOverlay(Camera camera)
	{
		DrawWireOverlay_Impl(camera);
	}

	public void DrawUIOverlay(Camera camera)
	{
		DrawUIOverlay_Internal(camera);
	}

	public unsafe CullingResults Cull(ref ScriptableCullingParameters parameters)
	{
		CullingResults result = default(CullingResults);
		Internal_Cull(ref parameters, this, (IntPtr)(&result));
		return result;
	}

	private unsafe void ValidateCullShadowCastersParameters(in CullingResults cullingResults, in ShadowCastersCullingInfos cullingInfos)
	{
		if (false)
		{
			throw new UnityException("CullingResults is null");
		}
		if (cullingInfos.perLightInfos.Length == 0)
		{
			return;
		}
		if (cullingResults.visibleLights.Length != cullingInfos.perLightInfos.Length)
		{
			throw new UnityException($"CullingResults.visibleLights.Length ({cullingResults.visibleLights.Length}) != ShadowCastersCullingInfos.perLightInfos.Length ({cullingInfos.perLightInfos.Length}). " + "ShadowCastersCullingInfos.perLightInfos must have one entry per visible light.");
		}
		LightShadowCasterCullingInfo* unsafeReadOnlyPtr = (LightShadowCasterCullingInfo*)cullingInfos.perLightInfos.GetUnsafeReadOnlyPtr();
		for (int i = 0; i < cullingInfos.perLightInfos.Length; i++)
		{
			ref LightShadowCasterCullingInfo reference = ref unsafeReadOnlyPtr[i];
			RangeInt splitRange = reference.splitRange;
			int start = splitRange.start;
			int length = splitRange.length;
			int num = start + length;
			if (start != 0 || length != 0)
			{
				bool flag = start >= 0 && start <= cullingInfos.splitBuffer.Length;
				bool flag2 = length >= 0 && length <= 6;
				bool flag3 = num >= start && num <= cullingInfos.splitBuffer.Length;
				if (!(flag && flag2 && flag3))
				{
					throw new UnityException($"ShadowCastersCullingInfos.perLightInfos[{i}] is referring to an invalid memory location. " + $"splitRange.start ({splitRange.start}) splitRange.length ({splitRange.length}) " + $"ShadowCastersCullingInfos.splitBuffer.Length ({cullingInfos.splitBuffer.Length}).");
				}
				if (length > 0 && reference.projectionType == BatchCullingProjectionType.Unknown)
				{
					throw new UnityException($"ShadowCastersCullingInfos.perLightInfos[{i}].projectionType == {reference.projectionType}. " + $"The range however appears to be valid. splitRange.start ({splitRange.start}) splitRange.length ({splitRange.length}).");
				}
				if (reference.splitExclusionMask >> length != 0)
				{
					string arg = Convert.ToString(reference.splitExclusionMask, 2);
					throw new UnityException($"ShadowCastersCullingInfos.perLightInfos[{i}].splitExclusionMask == 0b{arg}. " + $"The highest bit set must be less than the split count. splitRange.start ({splitRange.start}) splitRange.length ({splitRange.length}).");
				}
			}
		}
	}

	public unsafe void CullShadowCasters(CullingResults cullingResults, ShadowCastersCullingInfos infos)
	{
		CullShadowCastersContext cullShadowCastersContext = new CullShadowCastersContext
		{
			cullResults = cullingResults.ptr,
			splitBuffer = (ShadowSplitData*)infos.splitBuffer.GetUnsafePtr(),
			splitBufferLength = infos.splitBuffer.Length,
			perLightInfos = (LightShadowCasterCullingInfo*)infos.perLightInfos.GetUnsafePtr(),
			perLightInfoCount = infos.perLightInfos.Length
		};
		Internal_CullShadowCasters(this, (IntPtr)(&cullShadowCastersContext));
	}

	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	internal void Validate()
	{
	}

	public bool Equals(ScriptableRenderContext other)
	{
		return m_Ptr.Equals(other.m_Ptr);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return obj is ScriptableRenderContext && Equals((ScriptableRenderContext)obj);
	}

	public override int GetHashCode()
	{
		return m_Ptr.GetHashCode();
	}

	public static bool operator ==(ScriptableRenderContext left, ScriptableRenderContext right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ScriptableRenderContext left, ScriptableRenderContext right)
	{
		return !left.Equals(right);
	}

	public RendererList CreateRendererList(RendererListDesc desc)
	{
		RendererListParams param = RendererListDesc.ConvertToParameters(in desc);
		RendererList result = CreateRendererList(ref param);
		param.Dispose();
		return result;
	}

	public RendererList CreateRendererList(ref RendererListParams param)
	{
		param.Validate();
		return CreateRendererList_Internal(param.cullingResults.ptr, ref param.drawSettings, ref param.filteringSettings, param.tagName, param.isPassTagName, param.tagsValuePtr, param.stateBlocksPtr, param.numStateBlocks);
	}

	public unsafe RendererList CreateShadowRendererList(ref ShadowDrawingSettings settings)
	{
		fixed (ShadowDrawingSettings* ptr = &settings)
		{
			return CreateShadowRendererList_Internal((IntPtr)ptr);
		}
	}

	public RendererList CreateSkyboxRendererList(Camera camera, Matrix4x4 projectionMatrixL, Matrix4x4 viewMatrixL, Matrix4x4 projectionMatrixR, Matrix4x4 viewMatrixR)
	{
		return CreateSkyboxRendererList_Internal(camera, 2, projectionMatrixL, viewMatrixL, projectionMatrixR, viewMatrixR);
	}

	public RendererList CreateSkyboxRendererList(Camera camera, Matrix4x4 projectionMatrix, Matrix4x4 viewMatrix)
	{
		return CreateSkyboxRendererList_Internal(camera, 1, projectionMatrix, viewMatrix, Matrix4x4.identity, Matrix4x4.identity);
	}

	public RendererList CreateSkyboxRendererList(Camera camera)
	{
		return CreateSkyboxRendererList_Internal(camera, 0, Matrix4x4.identity, Matrix4x4.identity, Matrix4x4.identity, Matrix4x4.identity);
	}

	public RendererList CreateGizmoRendererList(Camera camera, GizmoSubset gizmoSubset)
	{
		return CreateGizmoRendererList_Internal(camera, gizmoSubset);
	}

	public RendererList CreateUIOverlayRendererList(Camera camera)
	{
		return CreateUIOverlayRendererList_Internal(camera, UISubset.All);
	}

	public RendererList CreateUIOverlayRendererList(Camera camera, UISubset uiSubset)
	{
		return CreateUIOverlayRendererList_Internal(camera, uiSubset);
	}

	public RendererList CreateWireOverlayRendererList(Camera camera)
	{
		return CreateWireOverlayRendererList_Internal(camera);
	}

	public void PrepareRendererListsAsync(List<RendererList> rendererLists)
	{
		PrepareRendererListsAsync_Internal(rendererLists);
	}

	public RendererListStatus QueryRendererListStatus(RendererList rendererList)
	{
		return QueryRendererListStatus_Internal(rendererList);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Cull_Injected(ref ScriptableCullingParameters parameters, [In] ref ScriptableRenderContext renderLoop, IntPtr results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CullShadowCasters_Injected([In] ref ScriptableRenderContext renderLoop, IntPtr context);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InitializeSortSettings_Injected(IntPtr camera, out SortingSettings sortingSettings);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawRenderers_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, [In] ref ShaderTagId tagName, bool isPassTagName, IntPtr tagValues, IntPtr stateBlocks, int stateCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EmitGeometryForCamera_Injected(IntPtr camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExecuteCommandBuffer_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr commandBuffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExecuteCommandBufferAsync_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr commandBuffer, ComputeQueueType queueType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetupCameraProperties_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, bool stereoSetup, int eye);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StereoEndRender_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, int eye, bool isFinalPass);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StartMultiEye_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, int eye);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopMultiEye_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawSkybox_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawGizmos_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, GizmoSubset gizmoSubset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawWireOverlay_Impl_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawUIOverlay_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, [In] ref ShaderTagId tagName, bool isPassTagName, IntPtr tagValues, IntPtr stateBlocks, int stateCount, out RendererList ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateShadowRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr shadowDrawinSettings, out RendererList ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateSkyboxRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, int mode, [In] ref Matrix4x4 proj, [In] ref Matrix4x4 view, [In] ref Matrix4x4 projR, [In] ref Matrix4x4 viewR, out RendererList ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateGizmoRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, GizmoSubset gizmoSubset, out RendererList ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateUIOverlayRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, UISubset uiSubset, out RendererList ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateWireOverlayRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, out RendererList ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RendererListStatus QueryRendererListStatus_Internal_Injected(ref ScriptableRenderContext _unity_self, [In] ref RendererList handle);
}
