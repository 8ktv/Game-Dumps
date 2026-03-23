#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR;

internal class TempMeshAllocatorImpl : IDisposable
{
	private struct ThreadData
	{
		public List<IntPtr> allocations;
	}

	private GCHandle m_GCHandle;

	private ThreadData[] m_ThreadData;

	private TempAllocator<Vertex> m_VertexPool = new TempAllocator<Vertex>(8192, 2048, 65536);

	private TempAllocator<ushort> m_IndexPool = new TempAllocator<ushort>(16384, 4096, 131072);

	protected bool disposed { get; private set; }

	public TempMeshAllocatorImpl()
	{
		m_GCHandle = GCHandle.Alloc(this);
		m_ThreadData = new ThreadData[JobsUtility.ThreadIndexCount];
		for (int i = 0; i < JobsUtility.ThreadIndexCount; i++)
		{
			m_ThreadData[i].allocations = new List<IntPtr>();
		}
	}

	public void CreateNativeHandle(out TempMeshAllocator allocator)
	{
		TempMeshAllocator.Create(m_GCHandle, out allocator);
	}

	private unsafe NativeSlice<T> Allocate<T>(int count, int alignment) where T : struct
	{
		ref ThreadData reference = ref m_ThreadData[UIRUtility.GetThreadIndex()];
		Debug.Assert(count > 0);
		long size = UnsafeUtility.SizeOf<T>() * count;
		void* ptr = UnsafeUtility.Malloc(size, UnsafeUtility.AlignOf<T>(), Allocator.TempJob);
		reference.allocations.Add((IntPtr)ptr);
		NativeArray<T> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, count, Allocator.Invalid);
		return nativeArray;
	}

	public void AllocateTempMesh(int vertexCount, int indexCount, out NativeSlice<Vertex> vertices, out NativeSlice<ushort> indices)
	{
		if (vertexCount > UIRenderDevice.maxVerticesPerPage)
		{
			throw new ArgumentOutOfRangeException("vertexCount", $"Attempting to allocate {vertexCount} vertices which exceeds the limit of {UIRenderDevice.maxVerticesPerPage}.");
		}
		if (!JobsUtility.IsExecutingJob)
		{
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
				vertices = default(NativeSlice<Vertex>);
				indices = default(NativeSlice<ushort>);
			}
			else
			{
				vertices = ((vertexCount > 0) ? m_VertexPool.Alloc(vertexCount) : default(NativeSlice<Vertex>));
				indices = ((indexCount > 0) ? m_IndexPool.Alloc(indexCount) : default(NativeSlice<ushort>));
			}
		}
		else
		{
			vertices = ((vertexCount > 0) ? Allocate<Vertex>(vertexCount, 4) : default(NativeSlice<Vertex>));
			indices = ((indexCount > 0) ? Allocate<ushort>(indexCount, 2) : default(NativeSlice<ushort>));
		}
	}

	public unsafe void Clear()
	{
		for (int i = 0; i < m_ThreadData.Length; i++)
		{
			foreach (IntPtr allocation in m_ThreadData[i].allocations)
			{
				UnsafeUtility.Free(allocation.ToPointer(), Allocator.TempJob);
			}
			m_ThreadData[i].allocations.Clear();
		}
		m_VertexPool.Reset();
		m_IndexPool.Reset();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				Clear();
				m_GCHandle.Free();
				m_VertexPool.Dispose();
				m_IndexPool.Dispose();
			}
			disposed = true;
		}
	}
}
