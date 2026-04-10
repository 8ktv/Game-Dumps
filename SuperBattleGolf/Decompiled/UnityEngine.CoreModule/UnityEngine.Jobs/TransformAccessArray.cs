using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Jobs;

[NativeType(Header = "Runtime/Transform/ScriptBindings/TransformAccess.bindings.h", CodegenOptions = CodegenOptions.Custom)]
public struct TransformAccessArray : IDisposable
{
	private IntPtr m_TransformArray;

	public bool isCreated => m_TransformArray != IntPtr.Zero;

	public Transform this[int index]
	{
		get
		{
			return GetTransform(m_TransformArray, index);
		}
		set
		{
			SetTransform(m_TransformArray, index, value);
		}
	}

	public int capacity
	{
		get
		{
			return GetCapacity(m_TransformArray);
		}
		set
		{
			SetCapacity(m_TransformArray, value);
		}
	}

	public int length => GetLength(m_TransformArray);

	public TransformAccessArray(Transform[] transforms, int desiredJobCount = -1)
	{
		Allocate(transforms.Length, desiredJobCount, out this);
		SetTransforms(m_TransformArray, transforms);
	}

	public unsafe TransformAccessArray(NativeArray<TransformHandle> transformHandles, int desiredJobCount = -1)
	{
		Allocate(transformHandles.Length, desiredJobCount, out this);
		SetTransformHandles(m_TransformArray, transformHandles.GetUnsafeReadOnlyPtr(), transformHandles.Length);
	}

	public TransformAccessArray(int capacity, int desiredJobCount = -1)
	{
		Allocate(capacity, desiredJobCount, out this);
	}

	public static void Allocate(int capacity, int desiredJobCount, out TransformAccessArray array)
	{
		array.m_TransformArray = Create(capacity, desiredJobCount);
		UnsafeUtility.LeakRecord(array.m_TransformArray, LeakCategory.TransformAccessArray, 0);
	}

	public void Dispose()
	{
		UnsafeUtility.LeakErase(m_TransformArray, LeakCategory.TransformAccessArray);
		DestroyTransformAccessArray(m_TransformArray);
		m_TransformArray = IntPtr.Zero;
	}

	internal IntPtr GetTransformAccessArrayForSchedule()
	{
		return m_TransformArray;
	}

	public TransformHandle GetTransformHandle(int index)
	{
		return GetTransformHandleInternal(m_TransformArray, index);
	}

	public void SetTransformHandle(int index, TransformHandle transformHandle)
	{
		SetTransformHandleInternal(m_TransformArray, index, transformHandle);
	}

	public void Add(Transform transform)
	{
		Add(m_TransformArray, transform);
	}

	[Obsolete("TransformAccessArray.Add(int) is obsolete. Use TransformAccessArray.Add(EntityId) instead.")]
	public void Add(int instanceId)
	{
		AddInstanceId(m_TransformArray, instanceId);
	}

	public void Add(TransformHandle transformHandle)
	{
		AddTransformHandle(m_TransformArray, transformHandle);
	}

	public void Add(EntityId entityId)
	{
		AddInstanceId(m_TransformArray, entityId);
	}

	public void RemoveAtSwapBack(int index)
	{
		RemoveAtSwapBack(m_TransformArray, index);
	}

	public void SetTransforms(Transform[] transforms)
	{
		SetTransforms(m_TransformArray, transforms);
	}

	public unsafe void SetTransformHandles(NativeArray<TransformHandle> transformHandles)
	{
		SetTransformHandles(m_TransformArray, transformHandles.GetUnsafeReadOnlyPtr(), transformHandles.Length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::Create", IsFreeFunction = true)]
	private static extern IntPtr Create(int capacity, int desiredJobCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "DestroyTransformAccessArray", IsFreeFunction = true)]
	private static extern void DestroyTransformAccessArray(IntPtr transformArray);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::SetTransforms", IsFreeFunction = true)]
	private static extern void SetTransforms(IntPtr transformArrayIntPtr, Transform[] transforms);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::SetTransformHandles", IsFreeFunction = true)]
	private unsafe static extern void SetTransformHandles(IntPtr transformArrayIntPtr, void* transformHandles, int count);

	[NativeMethod(Name = "TransformAccessArrayBindings::AddTransform", IsFreeFunction = true)]
	private static void Add(IntPtr transformArrayIntPtr, Transform transform)
	{
		Add_Injected(transformArrayIntPtr, Object.MarshalledUnityObject.Marshal(transform));
	}

	[NativeMethod(Name = "TransformAccessArrayBindings::AddTransformHandle", IsFreeFunction = true)]
	private static void AddTransformHandle(IntPtr transformArrayIntPtr, TransformHandle transformHandle)
	{
		AddTransformHandle_Injected(transformArrayIntPtr, ref transformHandle);
	}

	[NativeMethod(Name = "TransformAccessArrayBindings::AddTransformInstanceId", IsFreeFunction = true)]
	private static void AddInstanceId(IntPtr transformArrayIntPtr, EntityId instanceId)
	{
		AddInstanceId_Injected(transformArrayIntPtr, ref instanceId);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::RemoveAtSwapBack", IsFreeFunction = true, ThrowsException = true)]
	private static extern void RemoveAtSwapBack(IntPtr transformArrayIntPtr, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::GetSortedTransformAccess", IsThreadSafe = true, IsFreeFunction = true, ThrowsException = true)]
	internal static extern IntPtr GetSortedTransformAccess(IntPtr transformArrayIntPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::GetSortedToUserIndex", IsThreadSafe = true, IsFreeFunction = true, ThrowsException = true)]
	internal static extern IntPtr GetSortedToUserIndex(IntPtr transformArrayIntPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::GetLength", IsFreeFunction = true)]
	internal static extern int GetLength(IntPtr transformArrayIntPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::GetCapacity", IsFreeFunction = true)]
	internal static extern int GetCapacity(IntPtr transformArrayIntPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::SetCapacity", IsFreeFunction = true)]
	internal static extern void SetCapacity(IntPtr transformArrayIntPtr, int capacity);

	[NativeMethod(Name = "TransformAccessArrayBindings::GetTransform", IsFreeFunction = true, ThrowsException = true)]
	internal static Transform GetTransform(IntPtr transformArrayIntPtr, int index)
	{
		return Unmarshal.UnmarshalUnityObject<Transform>(GetTransform_Injected(transformArrayIntPtr, index));
	}

	[NativeMethod(Name = "TransformAccessArrayBindings::SetTransform", IsFreeFunction = true, ThrowsException = true)]
	internal static void SetTransform(IntPtr transformArrayIntPtr, int index, Transform transform)
	{
		SetTransform_Injected(transformArrayIntPtr, index, Object.MarshalledUnityObject.Marshal(transform));
	}

	[NativeMethod(Name = "TransformAccessArrayBindings::GetTransformHandle", IsFreeFunction = true, ThrowsException = true)]
	internal static TransformHandle GetTransformHandleInternal(IntPtr transformArrayIntPtr, int index)
	{
		GetTransformHandleInternal_Injected(transformArrayIntPtr, index, out var ret);
		return ret;
	}

	[NativeMethod(Name = "TransformAccessArrayBindings::SetTransformHandle", IsFreeFunction = true, ThrowsException = true)]
	internal static void SetTransformHandleInternal(IntPtr transformArrayIntPtr, int index, TransformHandle transformHandle)
	{
		SetTransformHandleInternal_Injected(transformArrayIntPtr, index, ref transformHandle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Add_Injected(IntPtr transformArrayIntPtr, IntPtr transform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddTransformHandle_Injected(IntPtr transformArrayIntPtr, [In] ref TransformHandle transformHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddInstanceId_Injected(IntPtr transformArrayIntPtr, [In] ref EntityId instanceId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetTransform_Injected(IntPtr transformArrayIntPtr, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTransform_Injected(IntPtr transformArrayIntPtr, int index, IntPtr transform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTransformHandleInternal_Injected(IntPtr transformArrayIntPtr, int index, out TransformHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTransformHandleInternal_Injected(IntPtr transformArrayIntPtr, int index, [In] ref TransformHandle transformHandle);
}
