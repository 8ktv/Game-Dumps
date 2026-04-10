#define UNITY_ASSERTIONS
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.Pool;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

internal class UITKTextJobSystem
{
	private class ManagedJobData
	{
		public TextElement visualElement;

		public MeshGenerationNode node;

		public List<Material> materials;

		public List<GlyphRenderMode> renderModes;

		public List<NativeSlice<Vertex>> vertices;

		public List<NativeSlice<ushort>> indices;

		public bool prepareSuccess;

		public void Release()
		{
			if (materials != null)
			{
				s_MaterialsPool.Release(materials);
				s_VerticesPool.Release(vertices);
				s_IndicesPool.Release(indices);
				s_RenderModesPool.Release(renderModes);
			}
			s_JobDataPool.Release(this);
		}
	}

	private struct PrepareTextJobData : IJobParallelFor
	{
		public GCHandle managedJobDataHandle;

		public void Execute(int index)
		{
			List<ManagedJobData> list = (List<ManagedJobData>)managedJobDataHandle.Target;
			ManagedJobData managedJobData = list[index];
			TextElement visualElement = managedJobData.visualElement;
			managedJobData.prepareSuccess = visualElement.uitkTextHandle.ConvertUssToTextGenerationSettings(populateScreenRect: true);
			if (managedJobData.prepareSuccess)
			{
				managedJobData.prepareSuccess = visualElement.uitkTextHandle.PrepareFontAsset();
			}
		}
	}

	private struct GenerateTextJobData : IJobParallelFor
	{
		public GCHandle managedJobDataHandle;

		[ReadOnly]
		public TempMeshAllocator alloc;

		public void Execute(int index)
		{
			List<ManagedJobData> list = (List<ManagedJobData>)managedJobDataHandle.Target;
			ManagedJobData managedJobData = list[index];
			TextElement visualElement = managedJobData.visualElement;
			if (visualElement.PostProcessTextVertices != null)
			{
				visualElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			}
			visualElement.uitkTextHandle.UpdateMesh();
			TextInfo textInfo = visualElement.uitkTextHandle.textInfo;
			MeshInfo[] meshInfo = textInfo.meshInfo;
			List<Material> materials = null;
			List<NativeSlice<Vertex>> verticesArray = null;
			List<NativeSlice<ushort>> indicesArray = null;
			List<GlyphRenderMode> renderModes = null;
			ConvertMeshInfoToUIRVertex(meshInfo, alloc, visualElement, ref materials, ref verticesArray, ref indicesArray, ref renderModes);
			managedJobData.materials = materials;
			managedJobData.vertices = verticesArray;
			managedJobData.indices = indicesArray;
			managedJobData.renderModes = renderModes;
			visualElement.uitkTextHandle.HandleATag();
			visualElement.uitkTextHandle.HandleLinkTag();
		}
	}

	private static readonly ProfilerMarker k_ExecuteMarker = new ProfilerMarker("TextJob.GenerateText");

	private static readonly ProfilerMarker k_UpdateMainThreadMarker = new ProfilerMarker("TextJob.UpdateMainThread");

	private static readonly ProfilerMarker k_PrepareMainThreadMarker = new ProfilerMarker("TextJob.PrepareMainThread");

	private static readonly ProfilerMarker k_PrepareJobifiedMarker = new ProfilerMarker("TextJob.PrepareJobified");

	private GCHandle textJobDatasHandle;

	private List<ManagedJobData> textJobDatas = new List<ManagedJobData>();

	private bool hasPendingTextWork;

	private static UnityEngine.Pool.ObjectPool<ManagedJobData> s_JobDataPool = new UnityEngine.Pool.ObjectPool<ManagedJobData>(() => new ManagedJobData(), OnGetManagedJob, delegate(ManagedJobData inst)
	{
		inst.visualElement = null;
	}, null, collectionCheck: false);

	private static UnityEngine.Pool.ObjectPool<List<Material>> s_MaterialsPool = new UnityEngine.Pool.ObjectPool<List<Material>>(() => new List<Material>(), null, delegate(List<Material> list)
	{
		list.Clear();
	}, null, collectionCheck: false);

	private static UnityEngine.Pool.ObjectPool<List<GlyphRenderMode>> s_RenderModesPool = new UnityEngine.Pool.ObjectPool<List<GlyphRenderMode>>(() => new List<GlyphRenderMode>(), null, delegate(List<GlyphRenderMode> list)
	{
		list.Clear();
	}, null, collectionCheck: false);

	private static UnityEngine.Pool.ObjectPool<List<NativeSlice<Vertex>>> s_VerticesPool = new UnityEngine.Pool.ObjectPool<List<NativeSlice<Vertex>>>(() => new List<NativeSlice<Vertex>>(), null, delegate(List<NativeSlice<Vertex>> list)
	{
		list.Clear();
	}, null, collectionCheck: false);

	private static UnityEngine.Pool.ObjectPool<List<NativeSlice<ushort>>> s_IndicesPool = new UnityEngine.Pool.ObjectPool<List<NativeSlice<ushort>>>(() => new List<NativeSlice<ushort>>(), null, delegate(List<NativeSlice<ushort>> list)
	{
		list.Clear();
	}, null, collectionCheck: false);

	internal MeshGenerationCallback m_PrepareTextJobifiedCallback;

	internal MeshGenerationCallback m_GenerateTextJobifiedCallback;

	internal MeshGenerationCallback m_AddDrawEntriesCallback;

	public UITKTextJobSystem()
	{
		m_PrepareTextJobifiedCallback = PrepareTextJobified;
		m_GenerateTextJobifiedCallback = GenerateTextJobified;
		m_AddDrawEntriesCallback = AddDrawEntries;
	}

	private static void OnGetManagedJob(ManagedJobData managedJobData)
	{
		managedJobData.vertices = null;
		managedJobData.indices = null;
		managedJobData.materials = null;
		managedJobData.renderModes = null;
		managedJobData.prepareSuccess = false;
	}

	internal void GenerateText(MeshGenerationContext mgc, TextElement textElement)
	{
		mgc.InsertMeshGenerationNode(out var node);
		ManagedJobData managedJobData = s_JobDataPool.Get();
		managedJobData.visualElement = textElement;
		managedJobData.node = node;
		textJobDatas.Add(managedJobData);
		if (!hasPendingTextWork)
		{
			hasPendingTextWork = true;
			textJobDatasHandle = GCHandle.Alloc(textJobDatas);
			mgc.AddMeshGenerationCallback(m_PrepareTextJobifiedCallback, null, MeshGenerationCallbackType.WorkThenFork, isJobDependent: false);
		}
	}

	internal void PrepareTextJobified(MeshGenerationContext mgc, object _)
	{
		TextHandle.InitThreadArrays();
		PanelTextSettings.InitializeDefaultPanelTextSettingsIfNull();
		TextHandle.UpdateCurrentFrame();
		hasPendingTextWork = false;
		PrepareTextJobData jobData = new PrepareTextJobData
		{
			managedJobDataHandle = textJobDatasHandle
		};
		UnityEngine.TextCore.Text.TextGenerator.IsExecutingJob = true;
		JobHandle jobHandle = jobData.Schedule(textJobDatas.Count, 1);
		mgc.AddMeshGenerationJob(jobHandle);
		mgc.AddMeshGenerationCallback(m_GenerateTextJobifiedCallback, null, MeshGenerationCallbackType.Work, isJobDependent: true);
	}

	private void GenerateTextJobified(MeshGenerationContext mgc, object _)
	{
		UnityEngine.TextCore.Text.TextGenerator.IsExecutingJob = false;
		foreach (ManagedJobData textJobData in textJobDatas)
		{
			TextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(textJobData.visualElement);
			textSettingsFrom?.lineBreakingRules?.LoadLineBreakingRules();
			_ = textSettingsFrom?.fallbackOSFontAssets;
			if (!textJobData.prepareSuccess)
			{
				textJobData.visualElement.uitkTextHandle.ConvertUssToTextGenerationSettings(populateScreenRect: true);
				textJobData.visualElement.uitkTextHandle.PrepareFontAsset();
			}
		}
		FontAsset.UpdateFontAssetsInUpdateQueue();
		mgc.GetTempMeshAllocator(out var allocator);
		GenerateTextJobData jobData = new GenerateTextJobData
		{
			managedJobDataHandle = textJobDatasHandle,
			alloc = allocator
		};
		TextHandle.UpdateCurrentFrame();
		UnityEngine.TextCore.Text.TextGenerator.IsExecutingJob = true;
		JobHandle jobHandle = jobData.Schedule(textJobDatas.Count, 1);
		mgc.AddMeshGenerationJob(jobHandle);
		mgc.AddMeshGenerationCallback(m_AddDrawEntriesCallback, null, MeshGenerationCallbackType.Work, isJobDependent: true);
	}

	private static void ConvertMeshInfoToUIRVertex(MeshInfo[] meshInfos, TempMeshAllocator alloc, TextElement visualElement, ref List<Material> materials, ref List<NativeSlice<Vertex>> verticesArray, ref List<NativeSlice<ushort>> indicesArray, ref List<GlyphRenderMode> renderModes)
	{
		lock (s_MaterialsPool)
		{
			materials = s_MaterialsPool.Get();
			verticesArray = s_VerticesPool.Get();
			indicesArray = s_IndicesPool.Get();
			renderModes = s_RenderModesPool.Get();
		}
		Vector2 min = visualElement.contentRect.min;
		float inverseScale = 1f / visualElement.scaledPixelsPerPoint;
		bool hasMultipleColors = visualElement.uitkTextHandle.textInfo.hasMultipleColors;
		if (hasMultipleColors)
		{
			visualElement.renderData.flags |= RenderDataFlags.IsIgnoringDynamicColorHint;
		}
		else
		{
			visualElement.renderData.flags &= ~RenderDataFlags.IsIgnoringDynamicColorHint;
		}
		for (int i = 0; i < meshInfos.Length; i++)
		{
			MeshInfo meshInfo = meshInfos[i];
			Debug.Assert((meshInfo.vertexCount & 3) == 0);
			int b = (int)(UIRenderDevice.maxVerticesPerPage & -4);
			int num = meshInfo.vertexCount;
			int num2 = 0;
			do
			{
				int num3 = Mathf.Min(num, b);
				int num4 = num3 >> 2;
				int indexCount = num4 * 6;
				materials.Add(meshInfo.material);
				renderModes.Add(meshInfo.glyphRenderMode);
				bool flag = meshInfo.glyphRenderMode != GlyphRenderMode.SMOOTH && meshInfo.glyphRenderMode != GlyphRenderMode.COLOR;
				bool isDynamicColor = meshInfo.applySDF && !hasMultipleColors && (RenderEvents.NeedsColorID(visualElement) || (flag && RenderEvents.NeedsTextCoreSettings(visualElement)));
				alloc.AllocateTempMesh(num3, indexCount, out var vertices, out var indices);
				int num5 = 0;
				int num6 = 0;
				while (num5 < num3)
				{
					vertices[num5] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num2], min, inverseScale, isDynamicColor);
					vertices[num5 + 1] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num2 + 1], min, inverseScale, isDynamicColor);
					vertices[num5 + 2] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num2 + 2], min, inverseScale, isDynamicColor);
					vertices[num5 + 3] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num2 + 3], min, inverseScale, isDynamicColor);
					indices[num6] = (ushort)num5;
					indices[num6 + 1] = (ushort)(num5 + 1);
					indices[num6 + 2] = (ushort)(num5 + 2);
					indices[num6 + 3] = (ushort)(num5 + 2);
					indices[num6 + 4] = (ushort)(num5 + 3);
					indices[num6 + 5] = (ushort)num5;
					num5 += 4;
					num2 += 4;
					num6 += 6;
				}
				verticesArray.Add(vertices);
				indicesArray.Add(indices);
				num -= num3;
			}
			while (num > 0);
			Debug.Assert(num == 0);
		}
	}

	private void AddDrawEntries(MeshGenerationContext mgc, object _)
	{
		UnityEngine.TextCore.Text.TextGenerator.IsExecutingJob = false;
		foreach (ManagedJobData textJobData in textJobDatas)
		{
			TextElement visualElement = textJobData.visualElement;
			mgc.Begin(textJobData.node.GetParentEntry(), visualElement, visualElement.nestedRenderData ?? visualElement.renderData);
			visualElement.uitkTextHandle.HandleLinkAndATagCallbacks();
			visualElement.PostProcessTextVertices?.Invoke(new TextElement.GlyphsEnumerable(visualElement, textJobData.vertices));
			mgc.meshGenerator.DrawText(textJobData.vertices, textJobData.indices, textJobData.materials, textJobData.renderModes);
			textJobData.visualElement.OnGenerateTextOver(mgc);
			mgc.End();
			textJobData.Release();
		}
		textJobDatas.Clear();
		textJobDatasHandle.Free();
	}
}
