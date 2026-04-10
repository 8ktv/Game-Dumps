using System;
using Unity.Collections;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering;

public interface IUnsafeCommandBuffer : IBaseCommandBuffer, IRasterCommandBuffer, IComputeCommandBuffer
{
	void RequestAsyncReadback(ComputeBuffer src, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadback(GraphicsBuffer src, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadback(ComputeBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadback(GraphicsBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadback(Texture src, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadback(Texture src, int mipIndex, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadback(Texture src, int mipIndex, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadback(Texture src, int mipIndex, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadback(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadback(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadback(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback);

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, ComputeBuffer src, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, ComputeBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, GraphicsBuffer src, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, GraphicsBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct;

	void Clear();

	void ClearRandomWriteTargets();

	void SetRenderTarget(RenderTargetIdentifier rt);

	void SetRenderTarget(RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction);

	void SetRenderTarget(RenderTargetIdentifier rt, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction);

	void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel);

	void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace);

	void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace, int depthSlice);

	void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth);

	void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel);

	void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace);

	void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice);

	void SetRenderTarget(RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction);

	void SetRenderTarget(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth);

	void SetRenderTarget(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice);

	void SetRenderTarget(RenderTargetBinding binding, int mipLevel, CubemapFace cubemapFace, int depthSlice);

	void SetRenderTarget(RenderTargetBinding binding);

	void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, RenderTargetIdentifier rt);

	void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, RenderTargetIdentifier rt);

	void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, RenderTargetIdentifier rt, int mipLevel);

	void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, RenderTargetIdentifier rt, int mipLevel);

	void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, RenderTargetIdentifier rt, int mipLevel, RenderTextureSubElement element);

	void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, RenderTargetIdentifier rt, int mipLevel, RenderTextureSubElement element);

	void SetRayTracingTextureParam(RayTracingShader rayTracingShader, string name, RenderTargetIdentifier rt);

	void SetRayTracingTextureParam(RayTracingShader rayTracingShader, int nameID, RenderTargetIdentifier rt);

	void GenerateMips(RenderTargetIdentifier rt);

	void GenerateMips(RenderTexture rt);

	void SetRandomWriteTarget(int index, RenderTargetIdentifier rt);

	void SetRandomWriteTarget(int index, ComputeBuffer buffer, bool preserveCounterValue);

	void SetRandomWriteTarget(int index, ComputeBuffer buffer);

	void SetRandomWriteTarget(int index, GraphicsBuffer buffer, bool preserveCounterValue);

	void SetRandomWriteTarget(int index, GraphicsBuffer buffer);

	void CopyTexture(RenderTargetIdentifier src, RenderTargetIdentifier dst);

	void CopyTexture(RenderTargetIdentifier src, int srcElement, RenderTargetIdentifier dst, int dstElement);

	void CopyTexture(RenderTargetIdentifier src, int srcElement, int srcMip, RenderTargetIdentifier dst, int dstElement, int dstMip);

	void CopyTexture(RenderTargetIdentifier src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, RenderTargetIdentifier dst, int dstElement, int dstMip, int dstX, int dstY);

	void SetGlobalTexture(string name, RenderTargetIdentifier value);

	void SetGlobalTexture(int nameID, RenderTargetIdentifier value);

	void SetGlobalTexture(string name, RenderTargetIdentifier value, RenderTextureSubElement element);

	void SetGlobalTexture(int nameID, RenderTargetIdentifier value, RenderTextureSubElement element);
}
