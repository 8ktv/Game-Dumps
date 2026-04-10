#define UNITY_ASSERTIONS
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

public class MeshGenerationContext
{
	[Flags]
	internal enum MeshFlags
	{
		None = 0,
		SkipDynamicAtlas = 2,
		IsUsingVectorImageGradients = 4,
		SliceTiled = 8
	}

	private Painter2D m_Painter2D;

	private MeshWriteDataPool m_MeshWriteDataPool;

	private TempMeshAllocatorImpl m_Allocator;

	private MeshGenerationDeferrer m_MeshGenerationDeferrer;

	private MeshGenerationNodeManager m_MeshGenerationNodeManager;

	private static readonly ProfilerMarker k_AllocateMarker = new ProfilerMarker("UIR.MeshGenerationContext.Allocate");

	private static readonly ProfilerMarker k_DrawVectorImageMarker = new ProfilerMarker("UIR.MeshGenerationContext.DrawVectorImage");

	public VisualElement visualElement { get; private set; }

	internal RenderData renderData { get; private set; }

	public Painter2D painter2D
	{
		get
		{
			if (disposed)
			{
				Debug.LogError("Accessing painter2D on disposed MeshGenerationContext");
				return null;
			}
			if (m_Painter2D == null)
			{
				m_Painter2D = new Painter2D(this);
			}
			return m_Painter2D;
		}
	}

	internal bool hasPainter2D => m_Painter2D != null;

	internal IMeshGenerator meshGenerator { get; set; }

	internal EntryRecorder entryRecorder { get; private set; }

	internal Entry parentEntry { get; private set; }

	internal bool disposed { get; private set; }

	internal MeshGenerationContext(MeshWriteDataPool meshWriteDataPool, EntryRecorder entryRecorder, TempMeshAllocatorImpl allocator, MeshGenerationDeferrer meshGenerationDeferrer, MeshGenerationNodeManager meshGenerationNodeManager)
	{
		m_MeshWriteDataPool = meshWriteDataPool;
		m_Allocator = allocator;
		m_MeshGenerationDeferrer = meshGenerationDeferrer;
		m_MeshGenerationNodeManager = meshGenerationNodeManager;
		this.entryRecorder = entryRecorder;
		meshGenerator = new MeshGenerator(this);
	}

	public void AllocateTempMesh(int vertexCount, int indexCount, out NativeSlice<Vertex> vertices, out NativeSlice<ushort> indices)
	{
		m_Allocator.AllocateTempMesh(vertexCount, indexCount, out vertices, out indices);
	}

	public MeshWriteData Allocate(int vertexCount, int indexCount, Texture texture = null)
	{
		using (k_AllocateMarker.Auto())
		{
			MeshWriteData meshWriteData = m_MeshWriteDataPool.Get();
			if (vertexCount == 0 || indexCount == 0)
			{
				meshWriteData.Reset(default(NativeSlice<Vertex>), default(NativeSlice<ushort>));
				return meshWriteData;
			}
			if (vertexCount > UIRenderDevice.maxVerticesPerPage)
			{
				throw new ArgumentOutOfRangeException("vertexCount", $"Attempting to allocate {vertexCount} vertices which exceeds the limit of {UIRenderDevice.maxVerticesPerPage}.");
			}
			m_Allocator.AllocateTempMesh(vertexCount, indexCount, out var vertices, out var indices);
			Debug.Assert(vertices.Length == vertexCount);
			Debug.Assert(indices.Length == indexCount);
			meshWriteData.Reset(vertices, indices);
			entryRecorder.DrawMesh(parentEntry, meshWriteData.m_Vertices, meshWriteData.m_Indices, texture);
			return meshWriteData;
		}
	}

	public void DrawMesh(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture = null)
	{
		if (vertices.Length != 0 && indices.Length != 0)
		{
			entryRecorder.DrawMesh(parentEntry, vertices, indices, texture);
		}
	}

	public void DrawVectorImage(VectorImage vectorImage, Vector2 offset, Angle rotationAngle, Vector2 scale)
	{
		using (k_DrawVectorImageMarker.Auto())
		{
			meshGenerator.DrawVectorImage(vectorImage, offset, rotationAngle, scale);
		}
	}

	public void DrawText(string text, Vector2 pos, float fontSize, Color color, FontAsset font = null)
	{
		if (font == null)
		{
			font = TextUtilities.GetFontAsset(visualElement);
		}
		meshGenerator.DrawText(text, pos, fontSize, color, font);
	}

	public void GetTempMeshAllocator(out TempMeshAllocator allocator)
	{
		m_Allocator.CreateNativeHandle(out allocator);
	}

	public void InsertMeshGenerationNode(out MeshGenerationNode node)
	{
		Entry entry = entryRecorder.InsertPlaceholder(parentEntry);
		m_MeshGenerationNodeManager.CreateNode(entry, out node);
	}

	internal void InsertUnsafeMeshGenerationNode(out UnsafeMeshGenerationNode node)
	{
		Entry entry = entryRecorder.InsertPlaceholder(parentEntry);
		m_MeshGenerationNodeManager.CreateUnsafeNode(entry, out node);
	}

	public void AddMeshGenerationJob(JobHandle jobHandle)
	{
		m_MeshGenerationDeferrer.AddMeshGenerationJob(jobHandle);
	}

	internal void AddMeshGenerationCallback(MeshGenerationCallback callback, object userData, MeshGenerationCallbackType callbackType, bool isJobDependent)
	{
		m_MeshGenerationDeferrer.AddMeshGenerationCallback(callback, userData, callbackType, isJobDependent);
	}

	internal void Begin(Entry parentEntry, VisualElement ve, RenderData renderData)
	{
		if (visualElement != null)
		{
			throw new InvalidOperationException("Begin can only be called when there is no target set. Did you forget to call End?");
		}
		if (parentEntry == null)
		{
			throw new ArgumentException("The state of the provided MeshGenerationNode is invalid (entry is null).");
		}
		if (ve == null)
		{
			throw new ArgumentException("ve");
		}
		this.parentEntry = parentEntry;
		visualElement = ve;
		this.renderData = renderData;
		meshGenerator.currentElement = ve;
	}

	internal void End()
	{
		if (visualElement == null)
		{
			throw new InvalidOperationException("End can only be called after a successful call to Begin.");
		}
		meshGenerator.currentElement = null;
		visualElement = null;
		parentEntry = null;
		m_Painter2D?.Reset();
	}

	internal void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				m_Painter2D?.Dispose();
				m_Painter2D = null;
				m_MeshWriteDataPool = null;
				entryRecorder = null;
				(meshGenerator as MeshGenerator)?.Dispose();
				meshGenerator = null;
				m_Allocator = null;
				m_MeshGenerationDeferrer = null;
				m_MeshGenerationNodeManager = null;
			}
			disposed = true;
		}
	}
}
