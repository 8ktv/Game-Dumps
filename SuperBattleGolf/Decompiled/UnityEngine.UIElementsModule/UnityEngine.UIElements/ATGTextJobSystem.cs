#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.Pool;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

internal class ATGTextJobSystem
{
	private class ManagedJobData
	{
		public TextElement textElement;

		public MeshGenerationNode node;

		public NativeTextInfo textInfo;

		public bool success;

		public List<Texture2D> atlases = new List<Texture2D>();

		public List<float> sdfScales = new List<float>();

		public List<NativeSlice<Vertex>> vertices = new List<NativeSlice<Vertex>>();

		public List<NativeSlice<ushort>> indices = new List<NativeSlice<ushort>>();

		public List<GlyphRenderMode> renderModes = new List<GlyphRenderMode>();

		public List<List<List<int>>> textElementIndicesByMesh = new List<List<List<int>>>();

		public List<bool> hasMultipleColorsByMesh = new List<bool>();

		public Dictionary<int, HashSet<uint>> missingGlyphsPerFontAsset = new Dictionary<int, HashSet<uint>>();

		public bool hasMissingGlyphs;

		public void Clear()
		{
			textElement = null;
			node = default(MeshGenerationNode);
			textInfo = default(NativeTextInfo);
			success = false;
			hasMissingGlyphs = false;
			atlases.Clear();
			sdfScales.Clear();
			vertices.Clear();
			indices.Clear();
			renderModes.Clear();
			hasMultipleColorsByMesh.Clear();
			foreach (List<List<int>> item in textElementIndicesByMesh)
			{
				foreach (List<int> item2 in item)
				{
					item2.Clear();
				}
			}
			foreach (HashSet<uint> value in missingGlyphsPerFontAsset.Values)
			{
				value.Clear();
			}
		}
	}

	private struct PrepareShapingJob : IJobFor
	{
		public GCHandle managedJobDataHandle;

		public void Execute(int index)
		{
			List<TextElement> list = (List<TextElement>)managedJobDataHandle.Target;
			TextElement textElement = list[index];
			textElement.uitkTextHandle.ShapeText();
		}
	}

	private struct GenerateTextJobData : IJobFor
	{
		public GCHandle managedJobDataHandle;

		[ReadOnly]
		public TempMeshAllocator alloc;

		public void Execute(int index)
		{
			List<ManagedJobData> list = (List<ManagedJobData>)managedJobDataHandle.Target;
			ManagedJobData managedJobData = list[index];
			TextElement textElement = managedJobData.textElement;
			bool generateNativeSettings = textElement.computedStyle.unityFontDefinition.fontAsset != null;
			if (textElement.PostProcessTextVertices != null)
			{
				textElement.uitkTextHandle.CacheTextGenerationInfo();
			}
			ManagedJobData managedJobData2 = managedJobData;
			(NativeTextInfo, bool) tuple = textElement.uitkTextHandle.UpdateNative(generateNativeSettings);
			managedJobData.textInfo = tuple.Item1;
			managedJobData2.success = tuple.Item2;
			managedJobData.hasMissingGlyphs = managedJobData.textElement.uitkTextHandle.HasMissingGlyphs(managedJobData.textInfo, ref managedJobData.missingGlyphsPerFontAsset);
			if (!managedJobData.hasMissingGlyphs)
			{
				managedJobData.textElement.uitkTextHandle.ProcessMeshInfos(managedJobData.textInfo, ref managedJobData.textElementIndicesByMesh, ref managedJobData.hasMultipleColorsByMesh);
				ConvertMeshInfoToUIRVertex(managedJobData.textInfo.meshInfos, alloc, managedJobData.textElement, managedJobData.textElementIndicesByMesh, managedJobData.hasMultipleColorsByMesh, ref managedJobData.atlases, ref managedJobData.vertices, ref managedJobData.indices, ref managedJobData.renderModes, ref managedJobData.sdfScales);
			}
		}
	}

	private struct ConvertToUIRVertexJobData : IJobFor
	{
		public GCHandle managedJobDataHandle;

		[ReadOnly]
		public TempMeshAllocator alloc;

		public void Execute(int index)
		{
			List<ManagedJobData> list = (List<ManagedJobData>)managedJobDataHandle.Target;
			ManagedJobData managedJobData = list[index];
			TextElement textElement = managedJobData.textElement;
			if (managedJobData.hasMissingGlyphs)
			{
				managedJobData.textElement.uitkTextHandle.ProcessMeshInfos(managedJobData.textInfo, ref managedJobData.textElementIndicesByMesh, ref managedJobData.hasMultipleColorsByMesh);
				ConvertMeshInfoToUIRVertex(managedJobData.textInfo.meshInfos, alloc, managedJobData.textElement, managedJobData.textElementIndicesByMesh, managedJobData.hasMultipleColorsByMesh, ref managedJobData.atlases, ref managedJobData.vertices, ref managedJobData.indices, ref managedJobData.renderModes, ref managedJobData.sdfScales);
			}
		}
	}

	private GCHandle textJobDatasHandle;

	private List<ManagedJobData> textJobDatas = new List<ManagedJobData>();

	private bool hasPendingTextWork;

	private static readonly UnityEngine.Pool.ObjectPool<ManagedJobData> s_JobDataPool = new UnityEngine.Pool.ObjectPool<ManagedJobData>(() => new ManagedJobData(), null, delegate(ManagedJobData inst)
	{
		inst.Clear();
	}, null, collectionCheck: false);

	private static UnityEngine.Pool.ObjectPool<Dictionary<int, HashSet<uint>>> s_AggregatedMissingGlyphsPool = new UnityEngine.Pool.ObjectPool<Dictionary<int, HashSet<uint>>>(() => new Dictionary<int, HashSet<uint>>(), null, delegate(Dictionary<int, HashSet<uint>> dict)
	{
		foreach (HashSet<uint> value in dict.Values)
		{
			value.Clear();
		}
	}, null, collectionCheck: false);

	internal MeshGenerationCallback m_GenerateTextJobifiedCallback;

	internal MeshGenerationCallback m_PopulateGlyphsCallback;

	internal MeshGenerationCallback m_AddDrawEntriesCallback;

	private static readonly ProfilerMarker k_GenerateTextMarker = new ProfilerMarker("ATGTextJob.GenerateText");

	private static readonly ProfilerMarker k_ATGTextJobMarker = new ProfilerMarker("ATGTextJob");

	private static readonly ProfilerMarker k_PrepareShapingMarker = new ProfilerMarker("LayoutUpdater.PrepareShaping");

	private static readonly bool k_IsMultiThreaded = true;

	private List<TextElement> m_PrepareShapingDataList = new List<TextElement>();

	private static List<uint> s_GlyphsToAddBuffer = new List<uint>();

	public ATGTextJobSystem()
	{
		m_GenerateTextJobifiedCallback = GenerateTextJobified;
		m_PopulateGlyphsCallback = PopulateGlyphs;
		m_AddDrawEntriesCallback = AddDrawEntries;
	}

	private static bool PrepareTextElementForJobsOnMainThread(TextElement textElement)
	{
		textElement.uitkTextHandle.EnsureIsReadyForJobs();
		if (textElement.computedStyle.unityFontDefinition.fontAsset == null && !textElement.uitkTextHandle.ConvertUssToNativeTextGenerationSettings())
		{
			return false;
		}
		if (textElement.enableRichText)
		{
			TextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(textElement);
			RichTextTagParser.PreloadFontAssetsFromTags(textElement.renderedTextString, textSettingsFrom);
			RichTextTagParser.PreloadSpriteAssetsFromTags(textElement.renderedTextString, textSettingsFrom);
			RichTextTagParser.PreloadGradientAssetsFromTags(textElement.renderedTextString, textSettingsFrom);
		}
		return true;
	}

	internal void PrepareShapingBeforeLayout(BaseVisualElementPanel panel)
	{
		if (!panel.visualTree.layoutNode.IsDirty || !panel.textElementRegistry.IsValueCreated)
		{
			return;
		}
		using (k_PrepareShapingMarker.Auto())
		{
			foreach (TextElement item in panel.textElementRegistry.Value)
			{
				if (item.layoutNode.IsDirty && TextUtilities.IsAdvancedTextEnabledForElement(item) && TextElement.AnySizeAutoOrNone(item.computedStyle) && PrepareTextElementForJobsOnMainThread(item))
				{
					m_PrepareShapingDataList.Add(item);
				}
			}
			if (m_PrepareShapingDataList.Count > 0)
			{
				FontAsset.CreateHbFaceIfNeeded();
				GCHandle managedJobDataHandle = GCHandle.Alloc(m_PrepareShapingDataList);
				PrepareShapingJob jobData = new PrepareShapingJob
				{
					managedJobDataHandle = managedJobDataHandle
				};
				IJobForExtensions.ScheduleParallelByRef(ref jobData, m_PrepareShapingDataList.Count, 1, default(JobHandle)).Complete();
				managedJobDataHandle.Free();
				m_PrepareShapingDataList.Clear();
			}
		}
	}

	public void GenerateText(MeshGenerationContext mgc, TextElement textElement)
	{
		mgc.InsertMeshGenerationNode(out var node);
		ManagedJobData managedJobData = s_JobDataPool.Get();
		managedJobData.textElement = textElement;
		managedJobData.node = node;
		textJobDatas.Add(managedJobData);
		if (!hasPendingTextWork)
		{
			hasPendingTextWork = true;
			textJobDatasHandle = GCHandle.Alloc(textJobDatas);
			MeshGenerationCallbackType callbackType = ((!k_IsMultiThreaded) ? MeshGenerationCallbackType.Work : MeshGenerationCallbackType.Fork);
			mgc.AddMeshGenerationCallback(m_GenerateTextJobifiedCallback, null, callbackType, isJobDependent: false);
		}
	}

	private void GenerateTextJobified(MeshGenerationContext mgc, object _)
	{
		mgc.GetTempMeshAllocator(out var allocator);
		GenerateTextJobData jobData = new GenerateTextJobData
		{
			managedJobDataHandle = textJobDatasHandle,
			alloc = allocator
		};
		for (int num = textJobDatas.Count - 1; num >= 0; num--)
		{
			ManagedJobData managedJobData = textJobDatas[num];
			TextElement textElement = managedJobData.textElement;
			if (!PrepareTextElementForJobsOnMainThread(textElement))
			{
				textJobDatas.RemoveAt(num);
			}
		}
		FontAsset.CreateHbFaceIfNeeded();
		if (k_IsMultiThreaded)
		{
			JobHandle jobHandle = IJobForExtensions.ScheduleParallelByRef(ref jobData, textJobDatas.Count, 1, default(JobHandle));
			mgc.AddMeshGenerationJob(jobHandle);
			mgc.AddMeshGenerationCallback(m_PopulateGlyphsCallback, null, MeshGenerationCallbackType.Work, isJobDependent: true);
			return;
		}
		for (int i = 0; i < textJobDatas.Count; i++)
		{
			jobData.Execute(i);
		}
		mgc.AddMeshGenerationCallback(m_PopulateGlyphsCallback, null, MeshGenerationCallbackType.Work, isJobDependent: false);
	}

	private void PopulateGlyphs(MeshGenerationContext mgc, object _)
	{
		Dictionary<int, HashSet<uint>> dictionary = s_AggregatedMissingGlyphsPool.Get();
		bool flag = false;
		foreach (ManagedJobData textJobData in textJobDatas)
		{
			if (!textJobData.hasMissingGlyphs)
			{
				continue;
			}
			foreach (KeyValuePair<int, HashSet<uint>> item in textJobData.missingGlyphsPerFontAsset)
			{
				if (item.Value.Count != 0)
				{
					flag = true;
					if (!dictionary.TryGetValue(item.Key, out var value))
					{
						value = new HashSet<uint>();
						dictionary[item.Key] = value;
					}
					value.UnionWith(item.Value);
				}
			}
		}
		if (!flag)
		{
			s_AggregatedMissingGlyphsPool.Release(dictionary);
			AddDrawEntries(mgc, _);
			return;
		}
		foreach (KeyValuePair<int, HashSet<uint>> item2 in dictionary)
		{
			UnityEngine.TextCore.Text.TextAsset textAssetByID = UnityEngine.TextCore.Text.TextAsset.GetTextAssetByID(item2.Key);
			if (!(textAssetByID == null) && textAssetByID is FontAsset fontAsset && item2.Value.Count != 0)
			{
				s_GlyphsToAddBuffer.Clear();
				s_GlyphsToAddBuffer.AddRange(item2.Value);
				fontAsset.TryAddGlyphs(s_GlyphsToAddBuffer);
			}
		}
		s_AggregatedMissingGlyphsPool.Release(dictionary);
		FontAsset.UpdateFontAssetsInUpdateQueue();
		mgc.GetTempMeshAllocator(out var allocator);
		ConvertToUIRVertexJobData jobData = new ConvertToUIRVertexJobData
		{
			managedJobDataHandle = textJobDatasHandle,
			alloc = allocator
		};
		JobHandle jobHandle = IJobForExtensions.ScheduleParallelByRef(ref jobData, textJobDatas.Count, 1, default(JobHandle));
		mgc.AddMeshGenerationJob(jobHandle);
		mgc.AddMeshGenerationCallback(m_AddDrawEntriesCallback, null, MeshGenerationCallbackType.Work, isJobDependent: true);
	}

	private void AddDrawEntries(MeshGenerationContext mgc, object _)
	{
		foreach (ManagedJobData textJobData in textJobDatas)
		{
			if (textJobData.success)
			{
				NativeTextInfo textInfo = textJobData.textInfo;
				mgc.Begin(textJobData.node.GetParentEntry(), textJobData.textElement, textJobData.textElement.renderData);
				textJobData.textElement.PostProcessTextVertices?.Invoke(new TextElement.GlyphsEnumerable(textJobData.textElement, textJobData.vertices, textInfo.meshInfos));
				mgc.meshGenerator.DrawText(textJobData.vertices, textJobData.indices, textJobData.atlases, textJobData.renderModes, textJobData.sdfScales);
				textJobData.textElement.OnGenerateTextOverNative(mgc);
				textJobData.textElement.uitkTextHandle.UpdateATGTextEventHandler();
				mgc.End();
			}
			s_JobDataPool.Release(textJobData);
		}
		textJobDatas.Clear();
		textJobDatasHandle.Free();
		hasPendingTextWork = false;
	}

	private static void ConvertMeshInfoToUIRVertex(Span<ATGMeshInfo> meshInfos, TempMeshAllocator alloc, TextElement visualElement, List<List<List<int>>> textElementIndicesByMesh, List<bool> hasMultipleColorsByMesh, ref List<Texture2D> atlases, ref List<NativeSlice<Vertex>> verticesArray, ref List<NativeSlice<ushort>> indicesArray, ref List<GlyphRenderMode> renderModes, ref List<float> sdfScales)
	{
		float inverseScale = 1f / visualElement.scaledPixelsPerPoint;
		for (int i = 0; i < meshInfos.Length; i++)
		{
			int num = 0;
			ATGMeshInfo aTGMeshInfo = meshInfos[i];
			FontAsset fontAsset = null;
			SpriteAsset spriteAsset = null;
			UnityEngine.TextCore.Text.TextAsset textAssetByID = UnityEngine.TextCore.Text.TextAsset.GetTextAssetByID(aTGMeshInfo.textAssetId);
			if (textAssetByID == null)
			{
				continue;
			}
			bool flag = false;
			if (textAssetByID is FontAsset)
			{
				fontAsset = textAssetByID as FontAsset;
				num = fontAsset.atlasTextures.Length;
			}
			else
			{
				flag = true;
				spriteAsset = textAssetByID as SpriteAsset;
				num = 1;
			}
			int b = (int)(UIRenderDevice.maxVerticesPerPage & -4);
			bool flag2 = hasMultipleColorsByMesh[i];
			if (flag2)
			{
				visualElement.renderData.flags |= RenderDataFlags.IsIgnoringDynamicColorHint;
			}
			else
			{
				visualElement.renderData.flags &= ~RenderDataFlags.IsIgnoringDynamicColorHint;
			}
			for (int j = 0; j < num; j++)
			{
				List<int> list = textElementIndicesByMesh[i][j];
				int num2 = list.Count * 4;
				int num3 = 0;
				while (num2 > 0)
				{
					int num4 = Mathf.Min(num2, b);
					int num5 = num4 >> 2;
					int indexCount = num5 * 6;
					if (flag)
					{
						atlases.Add((Texture2D)spriteAsset.spriteSheet);
						renderModes.Add(GlyphRenderMode.COLOR);
					}
					else
					{
						atlases.Add(fontAsset.atlasTextures[j]);
						renderModes.Add(fontAsset.atlasRenderMode);
					}
					float item = 0f;
					if (!flag)
					{
						List<GlyphRenderMode> obj = renderModes;
						if (!TextGeneratorUtilities.IsBitmapRendering(obj[obj.Count - 1]))
						{
							item = fontAsset.atlasPadding + 1;
						}
					}
					sdfScales.Add(item);
					bool flag3 = !flag && fontAsset.atlasRenderMode != GlyphRenderMode.SMOOTH && fontAsset.atlasRenderMode != GlyphRenderMode.COLOR;
					bool isDynamicColor = visualElement.PostProcessTextVertices == null && !flag2 && (RenderEvents.NeedsColorID(visualElement) || (flag3 && RenderEvents.NeedsTextCoreSettings(visualElement)));
					alloc.AllocateTempMesh(num4, indexCount, out var vertices, out var indices);
					Vector2 min = visualElement.contentRect.min;
					int num6 = 0;
					int num7 = 0;
					while (num6 < num4)
					{
						bool isColorGlyph = !flag && (fontAsset.atlasRenderMode == GlyphRenderMode.COLOR || fontAsset.atlasRenderMode == GlyphRenderMode.COLOR_HINTED);
						NativeTextElementInfo nativeTextElementInfo = aTGMeshInfo.textElementInfos[list[num3]];
						vertices[num6] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.bottomLeft, min, inverseScale, isDynamicColor, isColorGlyph);
						vertices[num6 + 1] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.topLeft, min, inverseScale, isDynamicColor, isColorGlyph);
						vertices[num6 + 2] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.topRight, min, inverseScale, isDynamicColor, isColorGlyph);
						vertices[num6 + 3] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.bottomRight, min, inverseScale, isDynamicColor, isColorGlyph);
						indices[num7] = (ushort)num6;
						indices[num7 + 1] = (ushort)(num6 + 1);
						indices[num7 + 2] = (ushort)(num6 + 2);
						indices[num7 + 3] = (ushort)(num6 + 2);
						indices[num7 + 4] = (ushort)(num6 + 3);
						indices[num7 + 5] = (ushort)num6;
						num6 += 4;
						num3++;
						num7 += 6;
					}
					verticesArray.Add(vertices);
					indicesArray.Add(indices);
					num2 -= num4;
				}
				Debug.Assert(num2 == 0);
			}
		}
	}
}
