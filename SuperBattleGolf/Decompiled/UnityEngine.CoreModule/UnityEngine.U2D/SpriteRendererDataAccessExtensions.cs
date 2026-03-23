using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.U2D;

[NativeHeader("Runtime/Graphics/Mesh/SpriteRenderer.h")]
[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
public static class SpriteRendererDataAccessExtensions
{
	internal unsafe static void SetDeformableBuffer(this SpriteRenderer spriteRenderer, NativeArray<byte> src)
	{
		if (spriteRenderer.sprite == null)
		{
			throw new ArgumentException($"spriteRenderer does not have a valid sprite set.");
		}
		if (src.Length != SpriteDataAccessExtensions.GetPrimaryVertexStreamSize(spriteRenderer.sprite))
		{
			throw new InvalidOperationException($"custom sprite vertex data size must match sprite asset's vertex data size {src.Length} {SpriteDataAccessExtensions.GetPrimaryVertexStreamSize(spriteRenderer.sprite)}");
		}
		SetDeformableBuffer(spriteRenderer, src.GetUnsafeReadOnlyPtr(), src.Length);
	}

	internal unsafe static void SetDeformableBuffer(this SpriteRenderer spriteRenderer, NativeArray<Vector3> src)
	{
		if (spriteRenderer.sprite == null)
		{
			throw new InvalidOperationException("spriteRenderer does not have a valid sprite set.");
		}
		if (src.Length != spriteRenderer.sprite.GetVertexCount())
		{
			throw new InvalidOperationException($"The src length {src.Length} must match the vertex count of source Sprite {spriteRenderer.sprite.GetVertexCount()}.");
		}
		SetDeformableBuffer(spriteRenderer, src.GetUnsafeReadOnlyPtr(), src.Length);
	}

	internal unsafe static void SetBatchDeformableBufferAndLocalAABBArray(SpriteRenderer[] spriteRenderers, NativeArray<IntPtr> buffers, NativeArray<int> bufferSizes, NativeArray<Bounds> bounds)
	{
		int num = spriteRenderers.Length;
		if (num != buffers.Length || num != bufferSizes.Length || num != bounds.Length)
		{
			throw new ArgumentException("Input array sizes are not the same.");
		}
		SetBatchDeformableBufferAndLocalAABBArray(spriteRenderers, buffers.GetUnsafeReadOnlyPtr(), bufferSizes.GetUnsafeReadOnlyPtr(), bounds.GetUnsafeReadOnlyPtr(), num);
	}

	internal unsafe static void SetBoneTransformsArray(SpriteRenderer[] spriteRenderers, NativeArray<IntPtr> buffers, NativeArray<int> bufferSizes, NativeArray<Bounds> bounds)
	{
		int num = spriteRenderers.Length;
		if (num != buffers.Length || num != bufferSizes.Length || num != bounds.Length)
		{
			throw new ArgumentException("Input array sizes are not the same.");
		}
		SetBoneTransformsArray(spriteRenderers, buffers.GetUnsafeReadOnlyPtr(), bufferSizes.GetUnsafeReadOnlyPtr(), bounds.GetUnsafeReadOnlyPtr(), num);
	}

	internal unsafe static bool IsUsingDeformableBuffer(this SpriteRenderer spriteRenderer, IntPtr buffer)
	{
		return IsUsingDeformableBuffer(spriteRenderer, (void*)buffer);
	}

	internal unsafe static void SetBoneTransforms(this SpriteRenderer spriteRenderer, NativeArray<Matrix4x4> src)
	{
		SetBoneTransforms(spriteRenderer, src.GetUnsafeReadOnlyPtr(), src.Length);
	}

	public static void DeactivateDeformableBuffer([NotNull] this SpriteRenderer renderer)
	{
		if ((object)renderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(renderer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		DeactivateDeformableBuffer_Injected(intPtr);
	}

	internal static void SetLocalAABB([NotNull] this SpriteRenderer renderer, Bounds aabb)
	{
		if ((object)renderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(renderer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		SetLocalAABB_Injected(intPtr, ref aabb);
	}

	private unsafe static void SetDeformableBuffer([NotNull] SpriteRenderer spriteRenderer, void* src, int count)
	{
		if ((object)spriteRenderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(spriteRenderer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
		}
		SetDeformableBuffer_Injected(intPtr, src, count);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void SetBatchDeformableBufferAndLocalAABBArray(SpriteRenderer[] spriteRenderers, void* buffers, void* bufferSizes, void* bounds, int count);

	private unsafe static bool IsUsingDeformableBuffer([NotNull] SpriteRenderer spriteRenderer, void* buffer)
	{
		if ((object)spriteRenderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(spriteRenderer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
		}
		return IsUsingDeformableBuffer_Injected(intPtr, buffer);
	}

	private unsafe static void SetBoneTransforms([NotNull] SpriteRenderer spriteRenderer, void* src, int count)
	{
		if ((object)spriteRenderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(spriteRenderer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
		}
		SetBoneTransforms_Injected(intPtr, src, count);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void SetBoneTransformsArray(SpriteRenderer[] spriteRenderers, void* buffers, void* bufferSizes, void* bounds, int count);

	internal static void SetupMaterialProperties([NotNull] SpriteRenderer spriteRenderer)
	{
		if ((object)spriteRenderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(spriteRenderer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
		}
		SetupMaterialProperties_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool IsGPUSkinningEnabled();

	internal static bool IsSRPBatchingEnabled([NotNull] this SpriteRenderer spriteRenderer)
	{
		if ((object)spriteRenderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(spriteRenderer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
		}
		return IsSRPBatchingEnabled_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DeactivateDeformableBuffer_Injected(IntPtr renderer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLocalAABB_Injected(IntPtr renderer, [In] ref Bounds aabb);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void SetDeformableBuffer_Injected(IntPtr spriteRenderer, void* src, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern bool IsUsingDeformableBuffer_Injected(IntPtr spriteRenderer, void* buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void SetBoneTransforms_Injected(IntPtr spriteRenderer, void* src, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetupMaterialProperties_Injected(IntPtr spriteRenderer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsSRPBatchingEnabled_Injected(IntPtr spriteRenderer);
}
