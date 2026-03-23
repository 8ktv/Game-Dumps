using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Graphics/AsyncGPUReadbackManaged.h")]
[NativeHeader("Runtime/Shaders/ComputeShader.h")]
[NativeHeader("Runtime/Graphics/Texture.h")]
[UsedByNativeCode]
public struct AsyncGPUReadbackRequest
{
	internal IntPtr m_Ptr;

	internal int m_Version;

	public bool done => IsDone();

	public bool hasError => HasError();

	public int layerCount => GetLayerCount();

	public int layerDataSize => GetLayerDataSize();

	public int width => GetWidth();

	public int height => GetHeight();

	public int depth => GetDepth();

	public bool forcePlayerLoopUpdate
	{
		get
		{
			return GetForcePlayerLoopUpdate();
		}
		set
		{
			SetForcePlayerLoopUpdate(value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void Update();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void WaitForCompletion();

	public unsafe NativeArray<T> GetData<T>(int layer = 0) where T : struct
	{
		if (!done || hasError)
		{
			throw new InvalidOperationException("Cannot access the data as it is not available");
		}
		if (layer < 0 || layer >= layerCount)
		{
			throw new ArgumentException($"Layer index is out of range {layer} / {layerCount}");
		}
		int num = UnsafeUtility.SizeOf<T>();
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)GetDataRaw(layer), layerDataSize / num, Allocator.None);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool IsDone();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool HasError();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetLayerCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetLayerDataSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetWidth();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetHeight();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetDepth();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool GetForcePlayerLoopUpdate();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetForcePlayerLoopUpdate(bool b);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void SetScriptingCallback(Action<AsyncGPUReadbackRequest> callback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern IntPtr GetDataRaw(int layer);

	[RequiredByNativeCode]
	private static void InvokeCallback(Action<AsyncGPUReadbackRequest> callback, AsyncGPUReadbackRequest obj)
	{
		callback(obj);
	}
}
