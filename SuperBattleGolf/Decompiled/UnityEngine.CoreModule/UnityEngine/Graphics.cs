using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;

namespace UnityEngine;

[NativeHeader("Runtime/Shaders/ComputeShader.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Graphics/CopyTexture.h")]
[NativeHeader("Runtime/Graphics/ColorGamut.h")]
[NativeHeader("Runtime/Camera/LightProbeProxyVolume.h")]
[NativeHeader("Runtime/Misc/PlayerSettings.h")]
public class Graphics
{
	internal static readonly int kMaxDrawMeshInstanceCount = Internal_GetMaxDrawMeshInstanceCount();

	internal static Dictionary<int, RenderInstancedDataLayout> s_RenderInstancedDataLayouts = new Dictionary<int, RenderInstancedDataLayout>();

	public static ColorGamut activeColorGamut => GetActiveColorGamut();

	[StaticAccessor("GetGfxDevice()", StaticAccessorType.Dot)]
	public static extern GraphicsTier activeTier
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static bool preserveFramebufferAlpha => GetPreserveFramebufferAlpha();

	public static OpenGLESVersion minOpenGLESVersion => GetMinOpenGLESVersion();

	public static RenderBuffer activeColorBuffer => GetActiveColorBuffer();

	public static RenderBuffer activeDepthBuffer => GetActiveDepthBuffer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsScripting::GetMaxDrawMeshInstanceCount", IsThreadSafe = true)]
	private static extern int Internal_GetMaxDrawMeshInstanceCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern ColorGamut GetActiveColorGamut();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
	[NativeMethod(Name = "GetPreserveFramebufferAlpha")]
	internal static extern bool GetPreserveFramebufferAlpha();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetMinOpenGLESVersion")]
	[StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
	internal static extern OpenGLESVersion GetMinOpenGLESVersion();

	[FreeFunction("GraphicsScripting::GetActiveColorBuffer")]
	private static RenderBuffer GetActiveColorBuffer()
	{
		GetActiveColorBuffer_Injected(out var ret);
		return ret;
	}

	[FreeFunction("GraphicsScripting::GetActiveDepthBuffer")]
	private static RenderBuffer GetActiveDepthBuffer()
	{
		GetActiveDepthBuffer_Injected(out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsScripting::SetNullRT")]
	private static extern void Internal_SetNullRT();

	[NativeMethod(Name = "GraphicsScripting::SetGfxRT", IsFreeFunction = true, ThrowsException = true)]
	private static void Internal_SetGfxRT(GraphicsTexture gfxTex, int mip, CubemapFace face, int depthSlice)
	{
		Internal_SetGfxRT_Injected((gfxTex == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(gfxTex), mip, face, depthSlice);
	}

	[NativeMethod(Name = "GraphicsScripting::SetRTSimple", IsFreeFunction = true, ThrowsException = true)]
	private static void Internal_SetRTSimple(RenderBuffer color, RenderBuffer depth, int mip, CubemapFace face, int depthSlice)
	{
		Internal_SetRTSimple_Injected(ref color, ref depth, mip, face, depthSlice);
	}

	[NativeMethod(Name = "GraphicsScripting::SetMRTSimple", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static void Internal_SetMRTSimple([NotNull] RenderBuffer[] color, RenderBuffer depth, int mip, CubemapFace face, int depthSlice)
	{
		if (color == null)
		{
			ThrowHelper.ThrowArgumentNullException(color, "color");
		}
		Span<RenderBuffer> span = new Span<RenderBuffer>(color);
		fixed (RenderBuffer* begin = span)
		{
			ManagedSpanWrapper color2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_SetMRTSimple_Injected(ref color2, ref depth, mip, face, depthSlice);
		}
	}

	[NativeMethod(Name = "GraphicsScripting::SetMRTFull", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static void Internal_SetMRTFullSetup([NotNull] RenderBuffer[] color, RenderBuffer depth, int mip, CubemapFace face, int depthSlice, [NotNull] RenderBufferLoadAction[] colorLA, [NotNull] RenderBufferStoreAction[] colorSA, RenderBufferLoadAction depthLA, RenderBufferStoreAction depthSA)
	{
		if (color == null)
		{
			ThrowHelper.ThrowArgumentNullException(color, "color");
		}
		if (colorLA == null)
		{
			ThrowHelper.ThrowArgumentNullException(colorLA, "colorLA");
		}
		if (colorSA == null)
		{
			ThrowHelper.ThrowArgumentNullException(colorSA, "colorSA");
		}
		Span<RenderBuffer> span = new Span<RenderBuffer>(color);
		fixed (RenderBuffer* begin = span)
		{
			ManagedSpanWrapper color2 = new ManagedSpanWrapper(begin, span.Length);
			Span<RenderBufferLoadAction> span2 = new Span<RenderBufferLoadAction>(colorLA);
			fixed (RenderBufferLoadAction* begin2 = span2)
			{
				ManagedSpanWrapper colorLA2 = new ManagedSpanWrapper(begin2, span2.Length);
				Span<RenderBufferStoreAction> span3 = new Span<RenderBufferStoreAction>(colorSA);
				fixed (RenderBufferStoreAction* begin3 = span3)
				{
					ManagedSpanWrapper colorSA2 = new ManagedSpanWrapper(begin3, span3.Length);
					Internal_SetMRTFullSetup_Injected(ref color2, ref depth, mip, face, depthSlice, ref colorLA2, ref colorSA2, depthLA, depthSA);
				}
			}
		}
	}

	[NativeMethod(Name = "GraphicsScripting::SetRandomWriteTargetRT", IsFreeFunction = true, ThrowsException = true)]
	private static void Internal_SetRandomWriteTargetRT(int index, RenderTexture uav)
	{
		Internal_SetRandomWriteTargetRT_Injected(index, Object.MarshalledUnityObject.Marshal(uav));
	}

	[FreeFunction("GraphicsScripting::SetRandomWriteTargetBuffer")]
	private static void Internal_SetRandomWriteTargetBuffer(int index, ComputeBuffer uav, bool preserveCounterValue)
	{
		Internal_SetRandomWriteTargetBuffer_Injected(index, (uav == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(uav), preserveCounterValue);
	}

	[FreeFunction("GraphicsScripting::SetRandomWriteTargetBuffer")]
	private static void Internal_SetRandomWriteTargetGraphicsBuffer(int index, GraphicsBuffer uav, bool preserveCounterValue)
	{
		Internal_SetRandomWriteTargetGraphicsBuffer_Injected(index, (uav == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(uav), preserveCounterValue);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetGfxDevice()", StaticAccessorType.Dot)]
	public static extern void ClearRandomWriteTargets();

	[FreeFunction("CopyTexture")]
	private static void CopyTexture_Full(Texture src, Texture dst)
	{
		CopyTexture_Full_Injected(Object.MarshalledUnityObject.Marshal(src), Object.MarshalledUnityObject.Marshal(dst));
	}

	[FreeFunction("CopyTexture")]
	private static void CopyTexture_Slice_AllMips(Texture src, int srcElement, Texture dst, int dstElement)
	{
		CopyTexture_Slice_AllMips_Injected(Object.MarshalledUnityObject.Marshal(src), srcElement, Object.MarshalledUnityObject.Marshal(dst), dstElement);
	}

	[FreeFunction("CopyTexture")]
	private static void CopyTexture_Slice(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip)
	{
		CopyTexture_Slice_Injected(Object.MarshalledUnityObject.Marshal(src), srcElement, srcMip, Object.MarshalledUnityObject.Marshal(dst), dstElement, dstMip);
	}

	[FreeFunction("CopyTextureRegion")]
	private static void CopyTexture_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY)
	{
		CopyTexture_Region_Injected(Object.MarshalledUnityObject.Marshal(src), srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, Object.MarshalledUnityObject.Marshal(dst), dstElement, dstMip, dstX, dstY);
	}

	[FreeFunction("CopyTexture")]
	private static void CopyTexture_Full_Gfx(GraphicsTexture src, GraphicsTexture dst)
	{
		CopyTexture_Full_Gfx_Injected((src == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dst));
	}

	[FreeFunction("CopyTexture")]
	private static void CopyTexture_Slice_AllMips_Gfx(GraphicsTexture src, int srcElement, GraphicsTexture dst, int dstElement)
	{
		CopyTexture_Slice_AllMips_Gfx_Injected((src == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(src), srcElement, (dst == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dst), dstElement);
	}

	[FreeFunction("CopyTexture")]
	private static void CopyTexture_Slice_Gfx(GraphicsTexture src, int srcElement, int srcMip, GraphicsTexture dst, int dstElement, int dstMip)
	{
		CopyTexture_Slice_Gfx_Injected((src == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(src), srcElement, srcMip, (dst == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dst), dstElement, dstMip);
	}

	[FreeFunction("CopyTextureRegion")]
	private static void CopyTexture_Region_Gfx(GraphicsTexture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsTexture dst, int dstElement, int dstMip, int dstX, int dstY)
	{
		CopyTexture_Region_Gfx_Injected((src == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(src), srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, (dst == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dst), dstElement, dstMip, dstX, dstY);
	}

	[FreeFunction("ConvertTexture")]
	private static bool ConvertTexture_Full(Texture src, Texture dst)
	{
		return ConvertTexture_Full_Injected(Object.MarshalledUnityObject.Marshal(src), Object.MarshalledUnityObject.Marshal(dst));
	}

	[FreeFunction("ConvertTexture")]
	private static bool ConvertTexture_Slice(Texture src, int srcElement, Texture dst, int dstElement)
	{
		return ConvertTexture_Slice_Injected(Object.MarshalledUnityObject.Marshal(src), srcElement, Object.MarshalledUnityObject.Marshal(dst), dstElement);
	}

	[FreeFunction("ConvertTexture")]
	private static bool ConvertTexture_Full_Gfx(GraphicsTexture src, GraphicsTexture dst)
	{
		return ConvertTexture_Full_Gfx_Injected((src == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dst));
	}

	[FreeFunction("ConvertTexture")]
	private static bool ConvertTexture_Slice_Gfx(GraphicsTexture src, int srcElement, GraphicsTexture dst, int dstElement)
	{
		return ConvertTexture_Slice_Gfx_Injected((src == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(src), srcElement, (dst == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dst), dstElement);
	}

	[FreeFunction("GraphicsScripting::CopyBuffer", ThrowsException = true)]
	private static void CopyBufferImpl([NotNull] GraphicsBuffer source, [NotNull] GraphicsBuffer dest)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		if (dest == null)
		{
			ThrowHelper.ThrowArgumentNullException(dest, "dest");
		}
		IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(source);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(dest);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(dest, "dest");
		}
		CopyBufferImpl_Injected(intPtr, intPtr2);
	}

	[FreeFunction("GraphicsScripting::DrawMeshNow")]
	private static void Internal_DrawMeshNow1([NotNull] Mesh mesh, int subsetIndex, Vector3 position, Quaternion rotation)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		Internal_DrawMeshNow1_Injected(intPtr, subsetIndex, ref position, ref rotation);
	}

	[FreeFunction("GraphicsScripting::DrawMeshNow")]
	private static void Internal_DrawMeshNow2([NotNull] Mesh mesh, int subsetIndex, Matrix4x4 matrix)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		Internal_DrawMeshNow2_Injected(intPtr, subsetIndex, ref matrix);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsScripting::DrawTexture")]
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
	internal static extern void Internal_DrawTexture(ref Internal_DrawTextureArguments args);

	[FreeFunction("GraphicsScripting::RenderMesh")]
	private unsafe static void Internal_RenderMesh(RenderParams rparams, [NotNull] Mesh mesh, int submeshIndex, Matrix4x4 objectToWorld, Matrix4x4* prevObjectToWorld)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		Internal_RenderMesh_Injected(ref rparams, intPtr, submeshIndex, ref objectToWorld, prevObjectToWorld);
	}

	[FreeFunction("GraphicsScripting::RenderMeshInstanced")]
	private static void Internal_RenderMeshInstanced(RenderParams rparams, [NotNull] Mesh mesh, int submeshIndex, IntPtr instanceData, RenderInstancedDataLayout layout, uint instanceCount)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		Internal_RenderMeshInstanced_Injected(ref rparams, intPtr, submeshIndex, instanceData, ref layout, instanceCount);
	}

	[FreeFunction("GraphicsScripting::RenderMeshIndirect")]
	private static void Internal_RenderMeshIndirect(RenderParams rparams, [NotNull] Mesh mesh, [NotNull] GraphicsBuffer argsBuffer, int commandCount, int startCommand)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		if (argsBuffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
		}
		Internal_RenderMeshIndirect_Injected(ref rparams, intPtr, intPtr2, commandCount, startCommand);
	}

	[FreeFunction("GraphicsScripting::RenderMeshPrimitives")]
	private static void Internal_RenderMeshPrimitives(RenderParams rparams, [NotNull] Mesh mesh, int submeshIndex, int instanceCount)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		Internal_RenderMeshPrimitives_Injected(ref rparams, intPtr, submeshIndex, instanceCount);
	}

	[FreeFunction("GraphicsScripting::RenderPrimitives")]
	private static void Internal_RenderPrimitives(RenderParams rparams, MeshTopology topology, int vertexCount, int instanceCount)
	{
		Internal_RenderPrimitives_Injected(ref rparams, topology, vertexCount, instanceCount);
	}

	[FreeFunction("GraphicsScripting::RenderPrimitivesIndexed")]
	private static void Internal_RenderPrimitivesIndexed(RenderParams rparams, MeshTopology topology, [NotNull] GraphicsBuffer indexBuffer, int indexCount, int startIndex, int instanceCount)
	{
		if (indexBuffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(indexBuffer, "indexBuffer");
		}
		IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(indexBuffer, "indexBuffer");
		}
		Internal_RenderPrimitivesIndexed_Injected(ref rparams, topology, intPtr, indexCount, startIndex, instanceCount);
	}

	[FreeFunction("GraphicsScripting::RenderPrimitivesIndirect")]
	private static void Internal_RenderPrimitivesIndirect(RenderParams rparams, MeshTopology topology, [NotNull] GraphicsBuffer argsBuffer, int commandCount, int startCommand)
	{
		if (argsBuffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
		}
		IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
		}
		Internal_RenderPrimitivesIndirect_Injected(ref rparams, topology, intPtr, commandCount, startCommand);
	}

	[FreeFunction("GraphicsScripting::RenderPrimitivesIndexedIndirect")]
	private static void Internal_RenderPrimitivesIndexedIndirect(RenderParams rparams, MeshTopology topology, [NotNull] GraphicsBuffer indexBuffer, [NotNull] GraphicsBuffer commandBuffer, int commandCount, int startCommand)
	{
		if (indexBuffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(indexBuffer, "indexBuffer");
		}
		if (commandBuffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(commandBuffer, "commandBuffer");
		}
		IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(indexBuffer, "indexBuffer");
		}
		IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(commandBuffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(commandBuffer, "commandBuffer");
		}
		Internal_RenderPrimitivesIndexedIndirect_Injected(ref rparams, topology, intPtr, intPtr2, commandCount, startCommand);
	}

	[FreeFunction("GraphicsScripting::DrawMesh")]
	private static void Internal_DrawMesh(Mesh mesh, int submeshIndex, Matrix4x4 matrix, Material material, int layer, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume)
	{
		Internal_DrawMesh_Injected(Object.MarshalledUnityObject.Marshal(mesh), submeshIndex, ref matrix, Object.MarshalledUnityObject.Marshal(material), layer, Object.MarshalledUnityObject.Marshal(camera), (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, Object.MarshalledUnityObject.Marshal(probeAnchor), lightProbeUsage, Object.MarshalledUnityObject.Marshal(lightProbeProxyVolume));
	}

	[FreeFunction("GraphicsScripting::DrawMeshInstanced")]
	private unsafe static void Internal_DrawMeshInstanced([NotNull] Mesh mesh, int submeshIndex, [NotNull] Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		if ((object)material == null)
		{
			ThrowHelper.ThrowArgumentNullException(material, "material");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(material);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(material, "material");
		}
		Span<Matrix4x4> span = new Span<Matrix4x4>(matrices);
		fixed (Matrix4x4* begin = span)
		{
			ManagedSpanWrapper matrices2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_DrawMeshInstanced_Injected(intPtr, submeshIndex, intPtr2, ref matrices2, count, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, layer, Object.MarshalledUnityObject.Marshal(camera), lightProbeUsage, Object.MarshalledUnityObject.Marshal(lightProbeProxyVolume));
		}
	}

	[FreeFunction("GraphicsScripting::DrawMeshInstancedProcedural")]
	private static void Internal_DrawMeshInstancedProcedural([NotNull] Mesh mesh, int submeshIndex, [NotNull] Material material, Bounds bounds, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		if ((object)material == null)
		{
			ThrowHelper.ThrowArgumentNullException(material, "material");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(material);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(material, "material");
		}
		Internal_DrawMeshInstancedProcedural_Injected(intPtr, submeshIndex, intPtr2, ref bounds, count, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, layer, Object.MarshalledUnityObject.Marshal(camera), lightProbeUsage, Object.MarshalledUnityObject.Marshal(lightProbeProxyVolume));
	}

	[FreeFunction("GraphicsScripting::DrawMeshInstancedIndirect")]
	private static void Internal_DrawMeshInstancedIndirect([NotNull] Mesh mesh, int submeshIndex, [NotNull] Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		if ((object)material == null)
		{
			ThrowHelper.ThrowArgumentNullException(material, "material");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(material);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(material, "material");
		}
		Internal_DrawMeshInstancedIndirect_Injected(intPtr, submeshIndex, intPtr2, ref bounds, (bufferWithArgs == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, layer, Object.MarshalledUnityObject.Marshal(camera), lightProbeUsage, Object.MarshalledUnityObject.Marshal(lightProbeProxyVolume));
	}

	[FreeFunction("GraphicsScripting::DrawMeshInstancedIndirect")]
	private static void Internal_DrawMeshInstancedIndirectGraphicsBuffer([NotNull] Mesh mesh, int submeshIndex, [NotNull] Material material, Bounds bounds, GraphicsBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		if ((object)material == null)
		{
			ThrowHelper.ThrowArgumentNullException(material, "material");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(material);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(material, "material");
		}
		Internal_DrawMeshInstancedIndirectGraphicsBuffer_Injected(intPtr, submeshIndex, intPtr2, ref bounds, (bufferWithArgs == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, layer, Object.MarshalledUnityObject.Marshal(camera), lightProbeUsage, Object.MarshalledUnityObject.Marshal(lightProbeProxyVolume));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsScripting::DrawProceduralNow")]
	private static extern void Internal_DrawProceduralNow(MeshTopology topology, int vertexCount, int instanceCount);

	[FreeFunction("GraphicsScripting::DrawProceduralIndexedNow")]
	private static void Internal_DrawProceduralIndexedNow(MeshTopology topology, GraphicsBuffer indexBuffer, int indexCount, int instanceCount)
	{
		Internal_DrawProceduralIndexedNow_Injected(topology, (indexBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer), indexCount, instanceCount);
	}

	[FreeFunction("GraphicsScripting::DrawProceduralIndirectNow")]
	private static void Internal_DrawProceduralIndirectNow(MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset)
	{
		Internal_DrawProceduralIndirectNow_Injected(topology, (bufferWithArgs == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset);
	}

	[FreeFunction("GraphicsScripting::DrawProceduralIndexedIndirectNow")]
	private static void Internal_DrawProceduralIndexedIndirectNow(MeshTopology topology, GraphicsBuffer indexBuffer, ComputeBuffer bufferWithArgs, int argsOffset)
	{
		Internal_DrawProceduralIndexedIndirectNow_Injected(topology, (indexBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer), (bufferWithArgs == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset);
	}

	[FreeFunction("GraphicsScripting::DrawProceduralIndirectNow")]
	private static void Internal_DrawProceduralIndirectNowGraphicsBuffer(MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset)
	{
		Internal_DrawProceduralIndirectNowGraphicsBuffer_Injected(topology, (bufferWithArgs == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset);
	}

	[FreeFunction("GraphicsScripting::DrawProceduralIndexedIndirectNow")]
	private static void Internal_DrawProceduralIndexedIndirectNowGraphicsBuffer(MeshTopology topology, GraphicsBuffer indexBuffer, GraphicsBuffer bufferWithArgs, int argsOffset)
	{
		Internal_DrawProceduralIndexedIndirectNowGraphicsBuffer_Injected(topology, (indexBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer), (bufferWithArgs == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset);
	}

	[FreeFunction("GraphicsScripting::DrawProcedural")]
	private static void Internal_DrawProcedural(Material material, Bounds bounds, MeshTopology topology, int vertexCount, int instanceCount, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		Internal_DrawProcedural_Injected(Object.MarshalledUnityObject.Marshal(material), ref bounds, topology, vertexCount, instanceCount, Object.MarshalledUnityObject.Marshal(camera), (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, layer);
	}

	[FreeFunction("GraphicsScripting::DrawProceduralIndexed")]
	private static void Internal_DrawProceduralIndexed(Material material, Bounds bounds, MeshTopology topology, GraphicsBuffer indexBuffer, int indexCount, int instanceCount, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		Internal_DrawProceduralIndexed_Injected(Object.MarshalledUnityObject.Marshal(material), ref bounds, topology, (indexBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer), indexCount, instanceCount, Object.MarshalledUnityObject.Marshal(camera), (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, layer);
	}

	[FreeFunction("GraphicsScripting::DrawProceduralIndirect")]
	private static void Internal_DrawProceduralIndirect(Material material, Bounds bounds, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		Internal_DrawProceduralIndirect_Injected(Object.MarshalledUnityObject.Marshal(material), ref bounds, topology, (bufferWithArgs == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, Object.MarshalledUnityObject.Marshal(camera), (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, layer);
	}

	[FreeFunction("GraphicsScripting::DrawProceduralIndirect")]
	private static void Internal_DrawProceduralIndirectGraphicsBuffer(Material material, Bounds bounds, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		Internal_DrawProceduralIndirectGraphicsBuffer_Injected(Object.MarshalledUnityObject.Marshal(material), ref bounds, topology, (bufferWithArgs == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, Object.MarshalledUnityObject.Marshal(camera), (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, layer);
	}

	[FreeFunction("GraphicsScripting::DrawProceduralIndexedIndirect")]
	private static void Internal_DrawProceduralIndexedIndirect(Material material, Bounds bounds, MeshTopology topology, GraphicsBuffer indexBuffer, ComputeBuffer bufferWithArgs, int argsOffset, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		Internal_DrawProceduralIndexedIndirect_Injected(Object.MarshalledUnityObject.Marshal(material), ref bounds, topology, (indexBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer), (bufferWithArgs == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, Object.MarshalledUnityObject.Marshal(camera), (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, layer);
	}

	[FreeFunction("GraphicsScripting::DrawProceduralIndexedIndirect")]
	private static void Internal_DrawProceduralIndexedIndirectGraphicsBuffer(Material material, Bounds bounds, MeshTopology topology, GraphicsBuffer indexBuffer, GraphicsBuffer bufferWithArgs, int argsOffset, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		Internal_DrawProceduralIndexedIndirectGraphicsBuffer_Injected(Object.MarshalledUnityObject.Marshal(material), ref bounds, topology, (indexBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer), (bufferWithArgs == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, Object.MarshalledUnityObject.Marshal(camera), (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), castShadows, receiveShadows, layer);
	}

	[FreeFunction("GraphicsScripting::BlitMaterial")]
	private static void Internal_BlitMaterial5(Texture source, RenderTexture dest, [NotNull] Material mat, int pass, bool setRT)
	{
		if ((object)mat == null)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		IntPtr source2 = Object.MarshalledUnityObject.Marshal(source);
		IntPtr dest2 = Object.MarshalledUnityObject.Marshal(dest);
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mat);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		Internal_BlitMaterial5_Injected(source2, dest2, intPtr, pass, setRT);
	}

	[FreeFunction("GraphicsScripting::BlitMaterial")]
	private static void Internal_BlitMaterial6(Texture source, RenderTexture dest, [NotNull] Material mat, int pass, bool setRT, int destDepthSlice)
	{
		if ((object)mat == null)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		IntPtr source2 = Object.MarshalledUnityObject.Marshal(source);
		IntPtr dest2 = Object.MarshalledUnityObject.Marshal(dest);
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mat);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		Internal_BlitMaterial6_Injected(source2, dest2, intPtr, pass, setRT, destDepthSlice);
	}

	[FreeFunction("GraphicsScripting::BlitMultitap")]
	private unsafe static void Internal_BlitMultiTap4(Texture source, RenderTexture dest, [NotNull] Material mat, [NotNull] Vector2[] offsets)
	{
		if ((object)mat == null)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		if (offsets == null)
		{
			ThrowHelper.ThrowArgumentNullException(offsets, "offsets");
		}
		IntPtr source2 = Object.MarshalledUnityObject.Marshal(source);
		IntPtr dest2 = Object.MarshalledUnityObject.Marshal(dest);
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mat);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		Span<Vector2> span = new Span<Vector2>(offsets);
		fixed (Vector2* begin = span)
		{
			ManagedSpanWrapper offsets2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_BlitMultiTap4_Injected(source2, dest2, intPtr, ref offsets2);
		}
	}

	[FreeFunction("GraphicsScripting::BlitMultitap")]
	private unsafe static void Internal_BlitMultiTap5(Texture source, RenderTexture dest, [NotNull] Material mat, [NotNull] Vector2[] offsets, int destDepthSlice)
	{
		if ((object)mat == null)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		if (offsets == null)
		{
			ThrowHelper.ThrowArgumentNullException(offsets, "offsets");
		}
		IntPtr source2 = Object.MarshalledUnityObject.Marshal(source);
		IntPtr dest2 = Object.MarshalledUnityObject.Marshal(dest);
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mat);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		Span<Vector2> span = new Span<Vector2>(offsets);
		fixed (Vector2* begin = span)
		{
			ManagedSpanWrapper offsets2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_BlitMultiTap5_Injected(source2, dest2, intPtr, ref offsets2, destDepthSlice);
		}
	}

	[FreeFunction("GraphicsScripting::Blit")]
	private static void Blit2(Texture source, RenderTexture dest)
	{
		Blit2_Injected(Object.MarshalledUnityObject.Marshal(source), Object.MarshalledUnityObject.Marshal(dest));
	}

	[FreeFunction("GraphicsScripting::Blit")]
	private static void Blit3(Texture source, RenderTexture dest, int sourceDepthSlice, int destDepthSlice)
	{
		Blit3_Injected(Object.MarshalledUnityObject.Marshal(source), Object.MarshalledUnityObject.Marshal(dest), sourceDepthSlice, destDepthSlice);
	}

	[FreeFunction("GraphicsScripting::Blit")]
	private static void Blit4(Texture source, RenderTexture dest, Vector2 scale, Vector2 offset)
	{
		Blit4_Injected(Object.MarshalledUnityObject.Marshal(source), Object.MarshalledUnityObject.Marshal(dest), ref scale, ref offset);
	}

	[FreeFunction("GraphicsScripting::Blit")]
	private static void Blit5(Texture source, RenderTexture dest, Vector2 scale, Vector2 offset, int sourceDepthSlice, int destDepthSlice)
	{
		Blit5_Injected(Object.MarshalledUnityObject.Marshal(source), Object.MarshalledUnityObject.Marshal(dest), ref scale, ref offset, sourceDepthSlice, destDepthSlice);
	}

	[FreeFunction("GraphicsScripting::BlitMaterial")]
	private static void Internal_BlitMaterialGfx5(Texture source, GraphicsTexture dest, [NotNull] Material mat, int pass, bool setRT)
	{
		if ((object)mat == null)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		IntPtr source2 = Object.MarshalledUnityObject.Marshal(source);
		IntPtr dest2 = ((dest == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dest));
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mat);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		Internal_BlitMaterialGfx5_Injected(source2, dest2, intPtr, pass, setRT);
	}

	[FreeFunction("GraphicsScripting::BlitMaterial")]
	private static void Internal_BlitMaterialGfx6(Texture source, GraphicsTexture dest, [NotNull] Material mat, int pass, bool setRT, int destDepthSlice)
	{
		if ((object)mat == null)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		IntPtr source2 = Object.MarshalledUnityObject.Marshal(source);
		IntPtr dest2 = ((dest == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dest));
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mat);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		Internal_BlitMaterialGfx6_Injected(source2, dest2, intPtr, pass, setRT, destDepthSlice);
	}

	[FreeFunction("GraphicsScripting::BlitMultitap")]
	private unsafe static void Internal_BlitMultiTapGfx4(Texture source, GraphicsTexture dest, [NotNull] Material mat, [NotNull] Vector2[] offsets)
	{
		if ((object)mat == null)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		if (offsets == null)
		{
			ThrowHelper.ThrowArgumentNullException(offsets, "offsets");
		}
		IntPtr source2 = Object.MarshalledUnityObject.Marshal(source);
		IntPtr dest2 = ((dest == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dest));
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mat);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		Span<Vector2> span = new Span<Vector2>(offsets);
		fixed (Vector2* begin = span)
		{
			ManagedSpanWrapper offsets2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_BlitMultiTapGfx4_Injected(source2, dest2, intPtr, ref offsets2);
		}
	}

	[FreeFunction("GraphicsScripting::BlitMultitap")]
	private unsafe static void Internal_BlitMultiTapGfx5(Texture source, GraphicsTexture dest, [NotNull] Material mat, [NotNull] Vector2[] offsets, int destDepthSlice)
	{
		if ((object)mat == null)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		if (offsets == null)
		{
			ThrowHelper.ThrowArgumentNullException(offsets, "offsets");
		}
		IntPtr source2 = Object.MarshalledUnityObject.Marshal(source);
		IntPtr dest2 = ((dest == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dest));
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(mat);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mat, "mat");
		}
		Span<Vector2> span = new Span<Vector2>(offsets);
		fixed (Vector2* begin = span)
		{
			ManagedSpanWrapper offsets2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_BlitMultiTapGfx5_Injected(source2, dest2, intPtr, ref offsets2, destDepthSlice);
		}
	}

	[FreeFunction("GraphicsScripting::Blit")]
	private static void BlitGfx2(Texture source, GraphicsTexture dest)
	{
		BlitGfx2_Injected(Object.MarshalledUnityObject.Marshal(source), (dest == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dest));
	}

	[FreeFunction("GraphicsScripting::Blit")]
	private static void BlitGfx3(Texture source, GraphicsTexture dest, int sourceDepthSlice, int destDepthSlice)
	{
		BlitGfx3_Injected(Object.MarshalledUnityObject.Marshal(source), (dest == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dest), sourceDepthSlice, destDepthSlice);
	}

	[FreeFunction("GraphicsScripting::Blit")]
	private static void BlitGfx4(Texture source, GraphicsTexture dest, Vector2 scale, Vector2 offset)
	{
		BlitGfx4_Injected(Object.MarshalledUnityObject.Marshal(source), (dest == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dest), ref scale, ref offset);
	}

	[FreeFunction("GraphicsScripting::Blit")]
	private static void BlitGfx5(Texture source, GraphicsTexture dest, Vector2 scale, Vector2 offset, int sourceDepthSlice, int destDepthSlice)
	{
		BlitGfx5_Injected(Object.MarshalledUnityObject.Marshal(source), (dest == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(dest), ref scale, ref offset, sourceDepthSlice, destDepthSlice);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GraphicsScripting::CreateGPUFence", IsFreeFunction = true, ThrowsException = true)]
	private static extern IntPtr CreateGPUFenceImpl(GraphicsFenceType fenceType, SynchronisationStageFlags stage);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GraphicsScripting::WaitOnGPUFence", IsFreeFunction = true, ThrowsException = true)]
	private static extern void WaitOnGPUFenceImpl(IntPtr fencePtr, SynchronisationStageFlags stage);

	[NativeMethod(Name = "GraphicsScripting::ExecuteCommandBuffer", IsFreeFunction = true, ThrowsException = true)]
	public static void ExecuteCommandBuffer([NotNull] CommandBuffer buffer)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		ExecuteCommandBuffer_Injected(intPtr);
	}

	[NativeMethod(Name = "GraphicsScripting::ExecuteCommandBufferAsync", IsFreeFunction = true, ThrowsException = true)]
	public static void ExecuteCommandBufferAsync([NotNull] CommandBuffer buffer, ComputeQueueType queueType)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		ExecuteCommandBufferAsync_Injected(intPtr, queueType);
	}

	internal static void CheckLoadActionValid(RenderBufferLoadAction load, string bufferType)
	{
		if (load != RenderBufferLoadAction.Load && load != RenderBufferLoadAction.DontCare)
		{
			throw new ArgumentException($"Bad {bufferType} LoadAction provided.");
		}
	}

	internal static void CheckStoreActionValid(RenderBufferStoreAction store, string bufferType)
	{
		if (store != RenderBufferStoreAction.Store && store != RenderBufferStoreAction.DontCare)
		{
			throw new ArgumentException($"Bad {bufferType} StoreAction provided.");
		}
	}

	internal static void SetRenderTargetImpl(RenderTargetSetup setup)
	{
		if (setup.color.Length == 0)
		{
			throw new ArgumentException("Invalid color buffer count for SetRenderTarget");
		}
		if (setup.color.Length != setup.colorLoad.Length)
		{
			throw new ArgumentException("Color LoadAction and Buffer arrays have different sizes");
		}
		if (setup.color.Length != setup.colorStore.Length)
		{
			throw new ArgumentException("Color StoreAction and Buffer arrays have different sizes");
		}
		RenderBufferLoadAction[] colorLoad = setup.colorLoad;
		foreach (RenderBufferLoadAction load in colorLoad)
		{
			CheckLoadActionValid(load, "Color");
		}
		RenderBufferStoreAction[] colorStore = setup.colorStore;
		foreach (RenderBufferStoreAction store in colorStore)
		{
			CheckStoreActionValid(store, "Color");
		}
		CheckLoadActionValid(setup.depthLoad, "Depth");
		CheckStoreActionValid(setup.depthStore, "Depth");
		if (setup.cubemapFace < CubemapFace.Unknown || setup.cubemapFace > CubemapFace.NegativeZ)
		{
			throw new ArgumentException("Bad CubemapFace provided");
		}
		Internal_SetMRTFullSetup(setup.color, setup.depth, setup.mipLevel, setup.cubemapFace, setup.depthSlice, setup.colorLoad, setup.colorStore, setup.depthLoad, setup.depthStore);
	}

	internal static void SetRenderTargetImpl(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
	{
		Internal_SetRTSimple(colorBuffer, depthBuffer, mipLevel, face, depthSlice);
	}

	internal static void SetRenderTargetImpl(RenderTexture rt, int mipLevel, CubemapFace face, int depthSlice)
	{
		if ((bool)rt)
		{
			SetRenderTargetImpl(rt.colorBuffer, rt.depthBuffer, mipLevel, face, depthSlice);
		}
		else
		{
			Internal_SetNullRT();
		}
	}

	internal static void SetRenderTargetImpl(GraphicsTexture rt, int mipLevel, CubemapFace face, int depthSlice)
	{
		if (rt != null)
		{
			Internal_SetGfxRT(rt, mipLevel, face, depthSlice);
		}
		else
		{
			Internal_SetNullRT();
		}
	}

	internal static void SetRenderTargetImpl(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
	{
		Internal_SetMRTSimple(colorBuffers, depthBuffer, mipLevel, face, depthSlice);
	}

	public static void SetRenderTarget(RenderTexture rt, [DefaultValue("0")] int mipLevel, [DefaultValue("CubemapFace.Unknown")] CubemapFace face, [DefaultValue("0")] int depthSlice)
	{
		SetRenderTargetImpl(rt, mipLevel, face, depthSlice);
	}

	public static void SetRenderTarget(GraphicsTexture rt, [DefaultValue("0")] int mipLevel, [DefaultValue("CubemapFace.Unknown")] CubemapFace face, [DefaultValue("0")] int depthSlice)
	{
		SetRenderTargetImpl(rt, mipLevel, face, depthSlice);
	}

	public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, [DefaultValue("0")] int mipLevel, [DefaultValue("CubemapFace.Unknown")] CubemapFace face, [DefaultValue("0")] int depthSlice)
	{
		SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face, depthSlice);
	}

	public static void SetRenderTarget(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer)
	{
		SetRenderTargetImpl(colorBuffers, depthBuffer, 0, CubemapFace.Unknown, 0);
	}

	public static void SetRenderTarget(RenderTargetSetup setup)
	{
		SetRenderTargetImpl(setup);
	}

	public static void SetRandomWriteTarget(int index, RenderTexture uav)
	{
		if (index < 0 || index >= SystemInfo.supportedRandomWriteTargetCount)
		{
			throw new ArgumentOutOfRangeException("index", $"must be non-negative less than {SystemInfo.supportedRandomWriteTargetCount}.");
		}
		Internal_SetRandomWriteTargetRT(index, uav);
	}

	public static void SetRandomWriteTarget(int index, ComputeBuffer uav, [DefaultValue("false")] bool preserveCounterValue)
	{
		if (uav == null)
		{
			throw new ArgumentNullException("uav");
		}
		if (uav.m_Ptr == IntPtr.Zero)
		{
			throw new ObjectDisposedException("uav");
		}
		if (index < 0 || index >= SystemInfo.supportedRandomWriteTargetCount)
		{
			throw new ArgumentOutOfRangeException("index", $"must be non-negative less than {SystemInfo.supportedRandomWriteTargetCount}.");
		}
		Internal_SetRandomWriteTargetBuffer(index, uav, preserveCounterValue);
	}

	public static void SetRandomWriteTarget(int index, GraphicsBuffer uav, [DefaultValue("false")] bool preserveCounterValue)
	{
		if (uav == null)
		{
			throw new ArgumentNullException("uav");
		}
		if (uav.m_Ptr == IntPtr.Zero)
		{
			throw new ObjectDisposedException("uav");
		}
		if (index < 0 || index >= SystemInfo.supportedRandomWriteTargetCount)
		{
			throw new ArgumentOutOfRangeException("index", $"must be non-negative less than {SystemInfo.supportedRandomWriteTargetCount}.");
		}
		Internal_SetRandomWriteTargetGraphicsBuffer(index, uav, preserveCounterValue);
	}

	public static void CopyTexture(Texture src, Texture dst)
	{
		CopyTexture_Full(src, dst);
	}

	public static void CopyTexture(Texture src, int srcElement, Texture dst, int dstElement)
	{
		CopyTexture_Slice_AllMips(src, srcElement, dst, dstElement);
	}

	public static void CopyTexture(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip)
	{
		CopyTexture_Slice(src, srcElement, srcMip, dst, dstElement, dstMip);
	}

	public static void CopyTexture(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY)
	{
		CopyTexture_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dst, dstElement, dstMip, dstX, dstY);
	}

	public static void CopyTexture(GraphicsTexture src, GraphicsTexture dst)
	{
		CopyTexture_Full_Gfx(src, dst);
	}

	public static void CopyTexture(GraphicsTexture src, int srcElement, GraphicsTexture dst, int dstElement)
	{
		CopyTexture_Slice_AllMips_Gfx(src, srcElement, dst, dstElement);
	}

	public static void CopyTexture(GraphicsTexture src, int srcElement, int srcMip, GraphicsTexture dst, int dstElement, int dstMip)
	{
		CopyTexture_Slice_Gfx(src, srcElement, srcMip, dst, dstElement, dstMip);
	}

	public static void CopyTexture(GraphicsTexture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsTexture dst, int dstElement, int dstMip, int dstX, int dstY)
	{
		CopyTexture_Region_Gfx(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dst, dstElement, dstMip, dstX, dstY);
	}

	public static bool ConvertTexture(Texture src, Texture dst)
	{
		return ConvertTexture_Full(src, dst);
	}

	public static bool ConvertTexture(Texture src, int srcElement, Texture dst, int dstElement)
	{
		return ConvertTexture_Slice(src, srcElement, dst, dstElement);
	}

	public static bool ConvertTexture(GraphicsTexture src, GraphicsTexture dst)
	{
		return ConvertTexture_Full_Gfx(src, dst);
	}

	public static bool ConvertTexture(GraphicsTexture src, int srcElement, GraphicsTexture dst, int dstElement)
	{
		return ConvertTexture_Slice_Gfx(src, srcElement, dst, dstElement);
	}

	public static GraphicsFence CreateAsyncGraphicsFence([DefaultValue("SynchronisationStage.PixelProcessing")] SynchronisationStage stage)
	{
		return CreateGraphicsFence(GraphicsFenceType.AsyncQueueSynchronisation, GraphicsFence.TranslateSynchronizationStageToFlags(stage));
	}

	public static GraphicsFence CreateAsyncGraphicsFence()
	{
		return CreateGraphicsFence(GraphicsFenceType.AsyncQueueSynchronisation, SynchronisationStageFlags.PixelProcessing);
	}

	public static GraphicsFence CreateGraphicsFence(GraphicsFenceType fenceType, [DefaultValue("SynchronisationStage.PixelProcessing")] SynchronisationStageFlags stage)
	{
		GraphicsFence result = default(GraphicsFence);
		result.m_FenceType = fenceType;
		result.m_Ptr = CreateGPUFenceImpl(fenceType, stage);
		result.InitPostAllocation();
		result.Validate();
		return result;
	}

	public static void WaitOnAsyncGraphicsFence(GraphicsFence fence)
	{
		WaitOnAsyncGraphicsFence(fence, SynchronisationStage.PixelProcessing);
	}

	public static void WaitOnAsyncGraphicsFence(GraphicsFence fence, [DefaultValue("SynchronisationStage.PixelProcessing")] SynchronisationStage stage)
	{
		if (fence.m_FenceType != GraphicsFenceType.AsyncQueueSynchronisation)
		{
			throw new ArgumentException("Graphics.WaitOnGraphicsFence can only be called with fences created with GraphicsFenceType.AsyncQueueSynchronization.");
		}
		fence.Validate();
		if (fence.IsFencePending())
		{
			WaitOnGPUFenceImpl(fence.m_Ptr, GraphicsFence.TranslateSynchronizationStageToFlags(stage));
		}
	}

	internal static void ValidateCopyBuffer(GraphicsBuffer source, GraphicsBuffer dest)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (dest == null)
		{
			throw new ArgumentNullException("dest");
		}
		long num = (long)source.count * (long)source.stride;
		long num2 = (long)dest.count * (long)dest.stride;
		if (num != num2)
		{
			throw new ArgumentException($"CopyBuffer source and destination buffers must be the same size, source was {num} bytes, dest was {num2} bytes");
		}
		if ((source.target & GraphicsBuffer.Target.CopySource) == 0)
		{
			throw new ArgumentException("CopyBuffer source must have CopySource target", "source");
		}
		if ((dest.target & GraphicsBuffer.Target.CopyDestination) == 0)
		{
			throw new ArgumentException("CopyBuffer destination must have CopyDestination target", "dest");
		}
	}

	public static void CopyBuffer(GraphicsBuffer source, GraphicsBuffer dest)
	{
		ValidateCopyBuffer(source, dest);
		CopyBufferImpl(source, dest);
	}

	private static void DrawTextureImpl(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat, int pass)
	{
		Internal_DrawTextureArguments args = new Internal_DrawTextureArguments
		{
			screenRect = screenRect,
			sourceRect = sourceRect,
			leftBorder = leftBorder,
			rightBorder = rightBorder,
			topBorder = topBorder,
			bottomBorder = bottomBorder,
			color = color,
			leftBorderColor = Color.black,
			topBorderColor = Color.black,
			rightBorderColor = Color.black,
			bottomBorderColor = Color.black,
			pass = pass,
			texture = texture,
			smoothCorners = true,
			mat = mat
		};
		Internal_DrawTexture(ref args);
	}

	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
	{
		DrawTextureImpl(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
	}

	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
	{
		Color32 color = new Color32(128, 128, 128, 128);
		DrawTextureImpl(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
	}

	public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
	{
		DrawTexture(screenRect, texture, new Rect(0f, 0f, 1f, 1f), leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
	}

	public static void DrawTexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
	{
		DrawTexture(screenRect, texture, 0, 0, 0, 0, mat, pass);
	}

	public unsafe static void RenderMesh(in RenderParams rparams, Mesh mesh, int submeshIndex, Matrix4x4 objectToWorld, [DefaultValue("null")] Matrix4x4? prevObjectToWorld = null)
	{
		if (prevObjectToWorld.HasValue)
		{
			Matrix4x4 value = prevObjectToWorld.Value;
			Internal_RenderMesh(rparams, mesh, submeshIndex, objectToWorld, &value);
		}
		else
		{
			Internal_RenderMesh(rparams, mesh, submeshIndex, objectToWorld, null);
		}
	}

	internal static RenderInstancedDataLayout GetCachedRenderInstancedDataLayout(Type type)
	{
		int hashCode = type.GetHashCode();
		if (!s_RenderInstancedDataLayouts.TryGetValue(hashCode, out var value))
		{
			value = new RenderInstancedDataLayout(type);
			s_RenderInstancedDataLayouts.Add(hashCode, value);
		}
		return value;
	}

	public unsafe static void RenderMeshInstanced<T>(in RenderParams rparams, Mesh mesh, int submeshIndex, T[] instanceData, [DefaultValue("-1")] int instanceCount = -1, [DefaultValue("0")] int startInstance = 0) where T : unmanaged
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (!rparams.material.enableInstancing)
		{
			throw new InvalidOperationException("Material needs to enable instancing for use with RenderMeshInstanced.");
		}
		if (instanceData == null)
		{
			throw new ArgumentNullException("instanceData");
		}
		RenderInstancedDataLayout cachedRenderInstancedDataLayout = GetCachedRenderInstancedDataLayout(typeof(T));
		uint instanceCount2 = Math.Min((uint)instanceCount, (uint)Math.Max(0, instanceData.Length - startInstance));
		fixed (T* ptr = instanceData)
		{
			Internal_RenderMeshInstanced(rparams, mesh, submeshIndex, (IntPtr)(ptr + startInstance), cachedRenderInstancedDataLayout, instanceCount2);
		}
	}

	public unsafe static void RenderMeshInstanced<T>(in RenderParams rparams, Mesh mesh, int submeshIndex, List<T> instanceData, [DefaultValue("-1")] int instanceCount = -1, [DefaultValue("0")] int startInstance = 0) where T : unmanaged
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (!rparams.material.enableInstancing)
		{
			throw new InvalidOperationException("Material needs to enable instancing for use with RenderMeshInstanced.");
		}
		if (instanceData == null)
		{
			throw new ArgumentNullException("instanceData");
		}
		RenderInstancedDataLayout cachedRenderInstancedDataLayout = GetCachedRenderInstancedDataLayout(typeof(T));
		uint instanceCount2 = Math.Min((uint)instanceCount, (uint)Math.Max(0, instanceData.Count - startInstance));
		fixed (T* ptr = NoAllocHelpers.ExtractArrayFromList(instanceData))
		{
			Internal_RenderMeshInstanced(rparams, mesh, submeshIndex, (IntPtr)(ptr + startInstance), cachedRenderInstancedDataLayout, instanceCount2);
		}
	}

	public unsafe static void RenderMeshInstanced<T>(RenderParams rparams, Mesh mesh, int submeshIndex, NativeArray<T> instanceData, [DefaultValue("-1")] int instanceCount = -1, [DefaultValue("0")] int startInstance = 0) where T : unmanaged
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (!rparams.material.enableInstancing)
		{
			throw new InvalidOperationException("Material needs to enable instancing for use with RenderMeshInstanced.");
		}
		bool flag = false;
		RenderInstancedDataLayout cachedRenderInstancedDataLayout = GetCachedRenderInstancedDataLayout(typeof(T));
		uint instanceCount2 = Math.Min((uint)instanceCount, (uint)Math.Max(0, instanceData.Length - startInstance));
		Internal_RenderMeshInstanced(rparams, mesh, submeshIndex, (IntPtr)((byte*)instanceData.GetUnsafePtr() + (nint)startInstance * (nint)sizeof(T)), cachedRenderInstancedDataLayout, instanceCount2);
	}

	public static void RenderMeshIndirect(in RenderParams rparams, Mesh mesh, GraphicsBuffer argsBuffer, [DefaultValue("1")] int commandCount = 1, [DefaultValue("0")] int startCommand = 0)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		Internal_RenderMeshIndirect(rparams, mesh, argsBuffer, commandCount, startCommand);
	}

	public static void RenderMeshPrimitives(in RenderParams rparams, Mesh mesh, int submeshIndex, [DefaultValue("1")] int instanceCount = 1)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		Internal_RenderMeshPrimitives(rparams, mesh, submeshIndex, instanceCount);
	}

	public static void RenderPrimitives(in RenderParams rparams, MeshTopology topology, int vertexCount, [DefaultValue("1")] int instanceCount = 1)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		Internal_RenderPrimitives(rparams, topology, vertexCount, instanceCount);
	}

	public static void RenderPrimitivesIndexed(in RenderParams rparams, MeshTopology topology, GraphicsBuffer indexBuffer, int indexCount, [DefaultValue("0")] int startIndex = 0, [DefaultValue("1")] int instanceCount = 1)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		Internal_RenderPrimitivesIndexed(rparams, topology, indexBuffer, indexCount, startIndex, instanceCount);
	}

	public static void RenderPrimitivesIndirect(in RenderParams rparams, MeshTopology topology, GraphicsBuffer argsBuffer, [DefaultValue("1")] int commandCount = 1, [DefaultValue("0")] int startCommand = 0)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		Internal_RenderPrimitivesIndirect(rparams, topology, argsBuffer, commandCount, startCommand);
	}

	public static void RenderPrimitivesIndexedIndirect(in RenderParams rparams, MeshTopology topology, GraphicsBuffer indexBuffer, GraphicsBuffer commandBuffer, [DefaultValue("1")] int commandCount = 1, [DefaultValue("0")] int startCommand = 0)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		Internal_RenderPrimitivesIndexedIndirect(rparams, topology, indexBuffer, commandBuffer, commandCount, startCommand);
	}

	public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
	{
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		Internal_DrawMeshNow1(mesh, materialIndex, position, rotation);
	}

	public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix, int materialIndex)
	{
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		Internal_DrawMeshNow2(mesh, materialIndex, matrix);
	}

	public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation)
	{
		DrawMeshNow(mesh, position, rotation, -1);
	}

	public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix)
	{
		DrawMeshNow(mesh, matrix, -1);
	}

	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("true")] bool castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("true")] bool useLightProbes)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows, null, useLightProbes ? LightProbeUsage.BlendProbes : LightProbeUsage.Off, null);
	}

	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("null")] Transform probeAnchor, [DefaultValue("true")] bool useLightProbes)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes ? LightProbeUsage.BlendProbes : LightProbeUsage.Off, null);
	}

	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("true")] bool castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("true")] bool useLightProbes)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows, null, useLightProbes ? LightProbeUsage.BlendProbes : LightProbeUsage.Off, null);
	}

	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
	{
		if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
		{
			throw new ArgumentException("Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.", "lightProbeProxyVolume");
		}
		Internal_DrawMesh(mesh, submeshIndex, matrix, material, layer, camera, properties, castShadows, receiveShadows, probeAnchor, lightProbeUsage, lightProbeProxyVolume);
	}

	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, [DefaultValue("matrices.Length")] int count, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera, [DefaultValue("LightProbeUsage.BlendProbes")] LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
		}
		if (material == null)
		{
			throw new ArgumentNullException("material");
		}
		if (!material.enableInstancing)
		{
			throw new InvalidOperationException("Material needs to enable instancing for use with DrawMeshInstanced.");
		}
		if (matrices == null)
		{
			throw new ArgumentNullException("matrices");
		}
		if (count < 0 || count > Mathf.Min(kMaxDrawMeshInstanceCount, matrices.Length))
		{
			throw new ArgumentOutOfRangeException("count", $"Count must be in the range of 0 to {Mathf.Min(kMaxDrawMeshInstanceCount, matrices.Length)}.");
		}
		if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
		{
			throw new ArgumentException("Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.", "lightProbeProxyVolume");
		}
		if (count > 0)
		{
			Internal_DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
		}
	}

	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera, [DefaultValue("LightProbeUsage.BlendProbes")] LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
	{
		if (matrices == null)
		{
			throw new ArgumentNullException("matrices");
		}
		DrawMeshInstanced(mesh, submeshIndex, material, NoAllocHelpers.ExtractArrayFromList(matrices), matrices.Count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
	}

	public static void DrawMeshInstancedProcedural(Mesh mesh, int submeshIndex, Material material, Bounds bounds, int count, MaterialPropertyBlock properties = null, ShadowCastingMode castShadows = ShadowCastingMode.On, bool receiveShadows = true, int layer = 0, Camera camera = null, LightProbeUsage lightProbeUsage = LightProbeUsage.BlendProbes, LightProbeProxyVolume lightProbeProxyVolume = null)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
		}
		if (material == null)
		{
			throw new ArgumentNullException("material");
		}
		if (count <= 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
		{
			throw new ArgumentException("Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.", "lightProbeProxyVolume");
		}
		if (count > 0)
		{
			Internal_DrawMeshInstancedProcedural(mesh, submeshIndex, material, bounds, count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
		}
	}

	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera, [DefaultValue("LightProbeUsage.BlendProbes")] LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
		}
		if (material == null)
		{
			throw new ArgumentNullException("material");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
		{
			throw new ArgumentException("Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.", "lightProbeProxyVolume");
		}
		Internal_DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
	}

	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, GraphicsBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera, [DefaultValue("LightProbeUsage.BlendProbes")] LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
		}
		if (material == null)
		{
			throw new ArgumentNullException("material");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
		{
			throw new ArgumentException("Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.", "lightProbeProxyVolume");
		}
		Internal_DrawMeshInstancedIndirectGraphicsBuffer(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
	}

	public static void DrawProceduralNow(MeshTopology topology, int vertexCount, int instanceCount = 1)
	{
		Internal_DrawProceduralNow(topology, vertexCount, instanceCount);
	}

	public static void DrawProceduralNow(MeshTopology topology, GraphicsBuffer indexBuffer, int indexCount, int instanceCount = 1)
	{
		if (indexBuffer == null)
		{
			throw new ArgumentNullException("indexBuffer");
		}
		Internal_DrawProceduralIndexedNow(topology, indexBuffer, indexCount, instanceCount);
	}

	public static void DrawProceduralIndirectNow(MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset = 0)
	{
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		Internal_DrawProceduralIndirectNow(topology, bufferWithArgs, argsOffset);
	}

	public static void DrawProceduralIndirectNow(MeshTopology topology, GraphicsBuffer indexBuffer, ComputeBuffer bufferWithArgs, int argsOffset = 0)
	{
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		if (indexBuffer == null)
		{
			throw new ArgumentNullException("indexBuffer");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		Internal_DrawProceduralIndexedIndirectNow(topology, indexBuffer, bufferWithArgs, argsOffset);
	}

	public static void DrawProceduralIndirectNow(MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset = 0)
	{
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		Internal_DrawProceduralIndirectNowGraphicsBuffer(topology, bufferWithArgs, argsOffset);
	}

	public static void DrawProceduralIndirectNow(MeshTopology topology, GraphicsBuffer indexBuffer, GraphicsBuffer bufferWithArgs, int argsOffset = 0)
	{
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		if (indexBuffer == null)
		{
			throw new ArgumentNullException("indexBuffer");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		Internal_DrawProceduralIndexedIndirectNowGraphicsBuffer(topology, indexBuffer, bufferWithArgs, argsOffset);
	}

	public static void DrawProcedural(Material material, Bounds bounds, MeshTopology topology, int vertexCount, int instanceCount = 1, Camera camera = null, MaterialPropertyBlock properties = null, ShadowCastingMode castShadows = ShadowCastingMode.On, bool receiveShadows = true, int layer = 0)
	{
		Internal_DrawProcedural(material, bounds, topology, vertexCount, instanceCount, camera, properties, castShadows, receiveShadows, layer);
	}

	public static void DrawProcedural(Material material, Bounds bounds, MeshTopology topology, GraphicsBuffer indexBuffer, int indexCount, int instanceCount = 1, Camera camera = null, MaterialPropertyBlock properties = null, ShadowCastingMode castShadows = ShadowCastingMode.On, bool receiveShadows = true, int layer = 0)
	{
		if (indexBuffer == null)
		{
			throw new ArgumentNullException("indexBuffer");
		}
		Internal_DrawProceduralIndexed(material, bounds, topology, indexBuffer, indexCount, instanceCount, camera, properties, castShadows, receiveShadows, layer);
	}

	public static void DrawProceduralIndirect(Material material, Bounds bounds, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset = 0, Camera camera = null, MaterialPropertyBlock properties = null, ShadowCastingMode castShadows = ShadowCastingMode.On, bool receiveShadows = true, int layer = 0)
	{
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		Internal_DrawProceduralIndirect(material, bounds, topology, bufferWithArgs, argsOffset, camera, properties, castShadows, receiveShadows, layer);
	}

	public static void DrawProceduralIndirect(Material material, Bounds bounds, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset = 0, Camera camera = null, MaterialPropertyBlock properties = null, ShadowCastingMode castShadows = ShadowCastingMode.On, bool receiveShadows = true, int layer = 0)
	{
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		Internal_DrawProceduralIndirectGraphicsBuffer(material, bounds, topology, bufferWithArgs, argsOffset, camera, properties, castShadows, receiveShadows, layer);
	}

	public static void DrawProceduralIndirect(Material material, Bounds bounds, MeshTopology topology, GraphicsBuffer indexBuffer, ComputeBuffer bufferWithArgs, int argsOffset = 0, Camera camera = null, MaterialPropertyBlock properties = null, ShadowCastingMode castShadows = ShadowCastingMode.On, bool receiveShadows = true, int layer = 0)
	{
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		if (indexBuffer == null)
		{
			throw new ArgumentNullException("indexBuffer");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		Internal_DrawProceduralIndexedIndirect(material, bounds, topology, indexBuffer, bufferWithArgs, argsOffset, camera, properties, castShadows, receiveShadows, layer);
	}

	public static void DrawProceduralIndirect(Material material, Bounds bounds, MeshTopology topology, GraphicsBuffer indexBuffer, GraphicsBuffer bufferWithArgs, int argsOffset = 0, Camera camera = null, MaterialPropertyBlock properties = null, ShadowCastingMode castShadows = ShadowCastingMode.On, bool receiveShadows = true, int layer = 0)
	{
		if (!SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		if (indexBuffer == null)
		{
			throw new ArgumentNullException("indexBuffer");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		Internal_DrawProceduralIndexedIndirectGraphicsBuffer(material, bounds, topology, indexBuffer, bufferWithArgs, argsOffset, camera, properties, castShadows, receiveShadows, layer);
	}

	public static void Blit(Texture source, RenderTexture dest)
	{
		Blit2(source, dest);
	}

	public static void Blit(Texture source, RenderTexture dest, int sourceDepthSlice, int destDepthSlice)
	{
		Blit3(source, dest, sourceDepthSlice, destDepthSlice);
	}

	public static void Blit(Texture source, RenderTexture dest, Vector2 scale, Vector2 offset)
	{
		Blit4(source, dest, scale, offset);
	}

	public static void Blit(Texture source, RenderTexture dest, Vector2 scale, Vector2 offset, int sourceDepthSlice, int destDepthSlice)
	{
		Blit5(source, dest, scale, offset, sourceDepthSlice, destDepthSlice);
	}

	public static void Blit(Texture source, RenderTexture dest, Material mat, [DefaultValue("-1")] int pass)
	{
		Internal_BlitMaterial5(source, dest, mat, pass, setRT: true);
	}

	public static void Blit(Texture source, RenderTexture dest, Material mat, int pass, int destDepthSlice)
	{
		Internal_BlitMaterial6(source, dest, mat, pass, setRT: true, destDepthSlice);
	}

	public static void Blit(Texture source, RenderTexture dest, Material mat)
	{
		Blit(source, dest, mat, -1);
	}

	public static void Blit(Texture source, Material mat, [DefaultValue("-1")] int pass)
	{
		Internal_BlitMaterial5(source, null, mat, pass, setRT: false);
	}

	public static void Blit(Texture source, Material mat, int pass, int destDepthSlice)
	{
		Internal_BlitMaterial6(source, null, mat, pass, setRT: false, destDepthSlice);
	}

	public static void Blit(Texture source, Material mat)
	{
		Blit(source, mat, -1);
	}

	public static void BlitMultiTap(Texture source, RenderTexture dest, Material mat, params Vector2[] offsets)
	{
		if (offsets.Length == 0)
		{
			throw new ArgumentException("empty offsets list passed.", "offsets");
		}
		Internal_BlitMultiTap4(source, dest, mat, offsets);
	}

	public static void BlitMultiTap(Texture source, RenderTexture dest, Material mat, int destDepthSlice, params Vector2[] offsets)
	{
		if (offsets.Length == 0)
		{
			throw new ArgumentException("empty offsets list passed.", "offsets");
		}
		Internal_BlitMultiTap5(source, dest, mat, offsets, destDepthSlice);
	}

	public static void Blit(Texture source, GraphicsTexture dest)
	{
		BlitGfx2(source, dest);
	}

	public static void Blit(Texture source, GraphicsTexture dest, int sourceDepthSlice, int destDepthSlice)
	{
		BlitGfx3(source, dest, sourceDepthSlice, destDepthSlice);
	}

	public static void Blit(Texture source, GraphicsTexture dest, Vector2 scale, Vector2 offset)
	{
		BlitGfx4(source, dest, scale, offset);
	}

	public static void Blit(Texture source, GraphicsTexture dest, Vector2 scale, Vector2 offset, int sourceDepthSlice, int destDepthSlice)
	{
		BlitGfx5(source, dest, scale, offset, sourceDepthSlice, destDepthSlice);
	}

	public static void Blit(Texture source, GraphicsTexture dest, Material mat, [DefaultValue("-1")] int pass)
	{
		Internal_BlitMaterialGfx5(source, dest, mat, pass, setRT: true);
	}

	public static void Blit(Texture source, GraphicsTexture dest, Material mat, int pass, int destDepthSlice)
	{
		Internal_BlitMaterialGfx6(source, dest, mat, pass, setRT: true, destDepthSlice);
	}

	public static void Blit(Texture source, GraphicsTexture dest, Material mat)
	{
		Blit(source, dest, mat, -1);
	}

	public static void BlitMultiTap(Texture source, GraphicsTexture dest, Material mat, params Vector2[] offsets)
	{
		if (offsets.Length == 0)
		{
			throw new ArgumentException("empty offsets list passed.", "offsets");
		}
		Internal_BlitMultiTapGfx4(source, dest, mat, offsets);
	}

	public static void BlitMultiTap(Texture source, GraphicsTexture dest, Material mat, int destDepthSlice, params Vector2[] offsets)
	{
		if (offsets.Length == 0)
		{
			throw new ArgumentException("empty offsets list passed.", "offsets");
		}
		Internal_BlitMultiTapGfx5(source, dest, mat, offsets, destDepthSlice);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, null, 0, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, 0, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer)
	{
		DrawMesh(mesh, matrix, material, layer, null, 0, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera)
	{
		DrawMesh(mesh, matrix, material, layer, camera, 0, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, LightProbeUsage.BlendProbes, null);
	}

	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("null")] Transform probeAnchor, [DefaultValue("true")] bool useLightProbes)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes ? LightProbeUsage.BlendProbes : LightProbeUsage.Off, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage)
	{
		Internal_DrawMesh(mesh, submeshIndex, matrix, material, layer, camera, properties, castShadows, receiveShadows, probeAnchor, lightProbeUsage, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, matrices.Length, null, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, null, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, null, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset = 0, MaterialPropertyBlock properties = null, ShadowCastingMode castShadows = ShadowCastingMode.On, bool receiveShadows = true, int layer = 0, Camera camera = null, LightProbeUsage lightProbeUsage = LightProbeUsage.BlendProbes)
	{
		DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, GraphicsBuffer bufferWithArgs, int argsOffset = 0, MaterialPropertyBlock properties = null, ShadowCastingMode castShadows = ShadowCastingMode.On, bool receiveShadows = true, int layer = 0, Camera camera = null, LightProbeUsage lightProbeUsage = LightProbeUsage.BlendProbes)
	{
		DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, null);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat)
	{
		DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, -1);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color)
	{
		DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, null, -1);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
	{
		DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, mat, -1);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
	{
		DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, null, -1);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
	{
		DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat, -1);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
	{
		DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, null, -1);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, Material mat)
	{
		DrawTexture(screenRect, texture, mat, -1);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture)
	{
		DrawTexture(screenRect, texture, null, -1);
	}

	[ExcludeFromDocs]
	public static void SetRenderTarget(RenderTexture rt)
	{
		SetRenderTarget(rt, 0, CubemapFace.Unknown, 0);
	}

	[ExcludeFromDocs]
	public static void SetRenderTarget(RenderTexture rt, int mipLevel)
	{
		SetRenderTarget(rt, mipLevel, CubemapFace.Unknown, 0);
	}

	[ExcludeFromDocs]
	public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face)
	{
		SetRenderTarget(rt, mipLevel, face, 0);
	}

	[ExcludeFromDocs]
	public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
	{
		SetRenderTarget(colorBuffer, depthBuffer, 0, CubemapFace.Unknown, 0);
	}

	[ExcludeFromDocs]
	public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel)
	{
		SetRenderTarget(colorBuffer, depthBuffer, mipLevel, CubemapFace.Unknown, 0);
	}

	[ExcludeFromDocs]
	public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face)
	{
		SetRenderTarget(colorBuffer, depthBuffer, mipLevel, face, 0);
	}

	[ExcludeFromDocs]
	public static void SetRandomWriteTarget(int index, ComputeBuffer uav)
	{
		SetRandomWriteTarget(index, uav, preserveCounterValue: false);
	}

	[ExcludeFromDocs]
	public static void SetRandomWriteTarget(int index, GraphicsBuffer uav)
	{
		SetRandomWriteTarget(index, uav, preserveCounterValue: false);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetActiveColorBuffer_Injected(out RenderBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetActiveDepthBuffer_Injected(out RenderBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetGfxRT_Injected(IntPtr gfxTex, int mip, CubemapFace face, int depthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetRTSimple_Injected([In] ref RenderBuffer color, [In] ref RenderBuffer depth, int mip, CubemapFace face, int depthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetMRTSimple_Injected(ref ManagedSpanWrapper color, [In] ref RenderBuffer depth, int mip, CubemapFace face, int depthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetMRTFullSetup_Injected(ref ManagedSpanWrapper color, [In] ref RenderBuffer depth, int mip, CubemapFace face, int depthSlice, ref ManagedSpanWrapper colorLA, ref ManagedSpanWrapper colorSA, RenderBufferLoadAction depthLA, RenderBufferStoreAction depthSA);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetRandomWriteTargetRT_Injected(int index, IntPtr uav);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetRandomWriteTargetBuffer_Injected(int index, IntPtr uav, bool preserveCounterValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetRandomWriteTargetGraphicsBuffer_Injected(int index, IntPtr uav, bool preserveCounterValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyTexture_Full_Injected(IntPtr src, IntPtr dst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyTexture_Slice_AllMips_Injected(IntPtr src, int srcElement, IntPtr dst, int dstElement);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyTexture_Slice_Injected(IntPtr src, int srcElement, int srcMip, IntPtr dst, int dstElement, int dstMip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyTexture_Region_Injected(IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, IntPtr dst, int dstElement, int dstMip, int dstX, int dstY);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyTexture_Full_Gfx_Injected(IntPtr src, IntPtr dst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyTexture_Slice_AllMips_Gfx_Injected(IntPtr src, int srcElement, IntPtr dst, int dstElement);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyTexture_Slice_Gfx_Injected(IntPtr src, int srcElement, int srcMip, IntPtr dst, int dstElement, int dstMip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyTexture_Region_Gfx_Injected(IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, IntPtr dst, int dstElement, int dstMip, int dstX, int dstY);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ConvertTexture_Full_Injected(IntPtr src, IntPtr dst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ConvertTexture_Slice_Injected(IntPtr src, int srcElement, IntPtr dst, int dstElement);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ConvertTexture_Full_Gfx_Injected(IntPtr src, IntPtr dst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ConvertTexture_Slice_Gfx_Injected(IntPtr src, int srcElement, IntPtr dst, int dstElement);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyBufferImpl_Injected(IntPtr source, IntPtr dest);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawMeshNow1_Injected(IntPtr mesh, int subsetIndex, [In] ref Vector3 position, [In] ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawMeshNow2_Injected(IntPtr mesh, int subsetIndex, [In] ref Matrix4x4 matrix);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void Internal_RenderMesh_Injected([In] ref RenderParams rparams, IntPtr mesh, int submeshIndex, [In] ref Matrix4x4 objectToWorld, Matrix4x4* prevObjectToWorld);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RenderMeshInstanced_Injected([In] ref RenderParams rparams, IntPtr mesh, int submeshIndex, IntPtr instanceData, [In] ref RenderInstancedDataLayout layout, uint instanceCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RenderMeshIndirect_Injected([In] ref RenderParams rparams, IntPtr mesh, IntPtr argsBuffer, int commandCount, int startCommand);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RenderMeshPrimitives_Injected([In] ref RenderParams rparams, IntPtr mesh, int submeshIndex, int instanceCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RenderPrimitives_Injected([In] ref RenderParams rparams, MeshTopology topology, int vertexCount, int instanceCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RenderPrimitivesIndexed_Injected([In] ref RenderParams rparams, MeshTopology topology, IntPtr indexBuffer, int indexCount, int startIndex, int instanceCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RenderPrimitivesIndirect_Injected([In] ref RenderParams rparams, MeshTopology topology, IntPtr argsBuffer, int commandCount, int startCommand);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RenderPrimitivesIndexedIndirect_Injected([In] ref RenderParams rparams, MeshTopology topology, IntPtr indexBuffer, IntPtr commandBuffer, int commandCount, int startCommand);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawMesh_Injected(IntPtr mesh, int submeshIndex, [In] ref Matrix4x4 matrix, IntPtr material, int layer, IntPtr camera, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, IntPtr probeAnchor, LightProbeUsage lightProbeUsage, IntPtr lightProbeProxyVolume);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawMeshInstanced_Injected(IntPtr mesh, int submeshIndex, IntPtr material, ref ManagedSpanWrapper matrices, int count, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, IntPtr camera, LightProbeUsage lightProbeUsage, IntPtr lightProbeProxyVolume);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawMeshInstancedProcedural_Injected(IntPtr mesh, int submeshIndex, IntPtr material, [In] ref Bounds bounds, int count, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, IntPtr camera, LightProbeUsage lightProbeUsage, IntPtr lightProbeProxyVolume);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawMeshInstancedIndirect_Injected(IntPtr mesh, int submeshIndex, IntPtr material, [In] ref Bounds bounds, IntPtr bufferWithArgs, int argsOffset, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, IntPtr camera, LightProbeUsage lightProbeUsage, IntPtr lightProbeProxyVolume);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawMeshInstancedIndirectGraphicsBuffer_Injected(IntPtr mesh, int submeshIndex, IntPtr material, [In] ref Bounds bounds, IntPtr bufferWithArgs, int argsOffset, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, IntPtr camera, LightProbeUsage lightProbeUsage, IntPtr lightProbeProxyVolume);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProceduralIndexedNow_Injected(MeshTopology topology, IntPtr indexBuffer, int indexCount, int instanceCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProceduralIndirectNow_Injected(MeshTopology topology, IntPtr bufferWithArgs, int argsOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProceduralIndexedIndirectNow_Injected(MeshTopology topology, IntPtr indexBuffer, IntPtr bufferWithArgs, int argsOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProceduralIndirectNowGraphicsBuffer_Injected(MeshTopology topology, IntPtr bufferWithArgs, int argsOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProceduralIndexedIndirectNowGraphicsBuffer_Injected(MeshTopology topology, IntPtr indexBuffer, IntPtr bufferWithArgs, int argsOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProcedural_Injected(IntPtr material, [In] ref Bounds bounds, MeshTopology topology, int vertexCount, int instanceCount, IntPtr camera, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProceduralIndexed_Injected(IntPtr material, [In] ref Bounds bounds, MeshTopology topology, IntPtr indexBuffer, int indexCount, int instanceCount, IntPtr camera, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProceduralIndirect_Injected(IntPtr material, [In] ref Bounds bounds, MeshTopology topology, IntPtr bufferWithArgs, int argsOffset, IntPtr camera, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProceduralIndirectGraphicsBuffer_Injected(IntPtr material, [In] ref Bounds bounds, MeshTopology topology, IntPtr bufferWithArgs, int argsOffset, IntPtr camera, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProceduralIndexedIndirect_Injected(IntPtr material, [In] ref Bounds bounds, MeshTopology topology, IntPtr indexBuffer, IntPtr bufferWithArgs, int argsOffset, IntPtr camera, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawProceduralIndexedIndirectGraphicsBuffer_Injected(IntPtr material, [In] ref Bounds bounds, MeshTopology topology, IntPtr indexBuffer, IntPtr bufferWithArgs, int argsOffset, IntPtr camera, IntPtr properties, ShadowCastingMode castShadows, bool receiveShadows, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_BlitMaterial5_Injected(IntPtr source, IntPtr dest, IntPtr mat, int pass, bool setRT);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_BlitMaterial6_Injected(IntPtr source, IntPtr dest, IntPtr mat, int pass, bool setRT, int destDepthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_BlitMultiTap4_Injected(IntPtr source, IntPtr dest, IntPtr mat, ref ManagedSpanWrapper offsets);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_BlitMultiTap5_Injected(IntPtr source, IntPtr dest, IntPtr mat, ref ManagedSpanWrapper offsets, int destDepthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Blit2_Injected(IntPtr source, IntPtr dest);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Blit3_Injected(IntPtr source, IntPtr dest, int sourceDepthSlice, int destDepthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Blit4_Injected(IntPtr source, IntPtr dest, [In] ref Vector2 scale, [In] ref Vector2 offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Blit5_Injected(IntPtr source, IntPtr dest, [In] ref Vector2 scale, [In] ref Vector2 offset, int sourceDepthSlice, int destDepthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_BlitMaterialGfx5_Injected(IntPtr source, IntPtr dest, IntPtr mat, int pass, bool setRT);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_BlitMaterialGfx6_Injected(IntPtr source, IntPtr dest, IntPtr mat, int pass, bool setRT, int destDepthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_BlitMultiTapGfx4_Injected(IntPtr source, IntPtr dest, IntPtr mat, ref ManagedSpanWrapper offsets);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_BlitMultiTapGfx5_Injected(IntPtr source, IntPtr dest, IntPtr mat, ref ManagedSpanWrapper offsets, int destDepthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BlitGfx2_Injected(IntPtr source, IntPtr dest);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BlitGfx3_Injected(IntPtr source, IntPtr dest, int sourceDepthSlice, int destDepthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BlitGfx4_Injected(IntPtr source, IntPtr dest, [In] ref Vector2 scale, [In] ref Vector2 offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BlitGfx5_Injected(IntPtr source, IntPtr dest, [In] ref Vector2 scale, [In] ref Vector2 offset, int sourceDepthSlice, int destDepthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExecuteCommandBuffer_Injected(IntPtr buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExecuteCommandBufferAsync_Injected(IntPtr buffer, ComputeQueueType queueType);
}
