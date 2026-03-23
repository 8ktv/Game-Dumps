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

internal class ATGTextJobSystem
{
	private class ManagedJobData
	{
		public TextElement textElement;

		public MeshGenerationNode node;

		public NativeTextInfo textInfo;

		public bool success;

		public void Release()
		{
			s_JobDataPool.Release(this);
		}
	}

	private struct GenerateTextJobData : IJobParallelFor
	{
		public GCHandle managedJobDataHandle;

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
			(managedJobData.textInfo, managedJobData.success) = textElement.uitkTextHandle.UpdateNative(generateNativeSettings);
		}
	}

	private GCHandle textJobDatasHandle;

	private List<ManagedJobData> textJobDatas = new List<ManagedJobData>();

	private bool hasPendingTextWork;

	private static UnityEngine.Pool.ObjectPool<ManagedJobData> s_JobDataPool = new UnityEngine.Pool.ObjectPool<ManagedJobData>(() => new ManagedJobData(), null, delegate(ManagedJobData inst)
	{
		inst.textElement = null;
	}, null, collectionCheck: false);

	internal MeshGenerationCallback m_GenerateTextJobifiedCallback;

	internal MeshGenerationCallback m_AddDrawEntriesCallback;

	private static readonly ProfilerMarker k_GenerateTextMarker = new ProfilerMarker("ATGTextJob.GenerateText");

	private static readonly ProfilerMarker k_ATGTextJobMarker = new ProfilerMarker("ATGTextJob");

	private static readonly bool k_IsMultiThreaded = true;

	private List<Texture2D> atlases = new List<Texture2D>();

	private List<float> sdfScalesArray = new List<float>();

	private List<NativeSlice<Vertex>> verticesArray = new List<NativeSlice<Vertex>>();

	private List<NativeSlice<ushort>> indicesArray = new List<NativeSlice<ushort>>();

	private List<GlyphRenderMode> renderModes = new List<GlyphRenderMode>();

	public ATGTextJobSystem()
	{
		m_GenerateTextJobifiedCallback = GenerateTextJobified;
		m_AddDrawEntriesCallback = AddDrawEntries;
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
		GenerateTextJobData jobData = new GenerateTextJobData
		{
			managedJobDataHandle = textJobDatasHandle
		};
		if (textJobDatas.Count > 0)
		{
			textJobDatas[0].textElement.uitkTextHandle.InitTextLib();
		}
		FontAsset.CreateHbFaceIfNeeded();
		for (int i = 0; i < textJobDatas.Count; i++)
		{
			ManagedJobData managedJobData = textJobDatas[i];
			TextElement textElement = managedJobData.textElement;
			FontAsset fontAsset = TextUtilities.GetFontAsset(textElement);
			TextUtilities.GetTextSettingsFrom(textElement).UpdateNativeTextSettings();
			fontAsset.EnsureNativeFontAssetIsCreated();
			if (textElement.computedStyle.unityFontDefinition.fontAsset == null)
			{
				textElement.uitkTextHandle.ConvertUssToNativeTextGenerationSettings();
			}
		}
		if (k_IsMultiThreaded)
		{
			JobHandle jobHandle = jobData.ScheduleOrRunJob(textJobDatas.Count, 1);
			mgc.AddMeshGenerationJob(jobHandle);
			mgc.AddMeshGenerationCallback(m_AddDrawEntriesCallback, null, MeshGenerationCallbackType.Work, isJobDependent: true);
			return;
		}
		for (int j = 0; j < textJobDatas.Count; j++)
		{
			jobData.Execute(j);
		}
		mgc.AddMeshGenerationCallback(m_AddDrawEntriesCallback, null, MeshGenerationCallbackType.Work, isJobDependent: false);
	}

	private void AddDrawEntries(MeshGenerationContext mgc, object _)
	{
		foreach (ManagedJobData textJobData in textJobDatas)
		{
			if (textJobData.success)
			{
				NativeTextInfo textInfo = textJobData.textInfo;
				TextElement textElement = textJobData.textElement;
				textElement.uitkTextHandle.ProcessMeshInfos(textInfo);
				textElement.uitkTextHandle.UpdateATGTextEventHandler();
				FontAsset.UpdateFontAssetsInUpdateQueue();
				mgc.GetTempMeshAllocator(out var allocator);
				ConvertMeshInfoToUIRVertex(textInfo.meshInfos, allocator, textElement, atlases, verticesArray, indicesArray, renderModes, sdfScalesArray);
				textElement.PostProcessTextVertices?.Invoke(new TextElement.GlyphsEnumerable(textElement, verticesArray, textInfo.meshInfos));
				mgc.Begin(textJobData.node.GetParentEntry(), textElement, textElement.renderData);
				mgc.meshGenerator.DrawText(verticesArray, indicesArray, atlases, renderModes, sdfScalesArray);
				textElement.OnGenerateTextOverNative(mgc);
				atlases.Clear();
				verticesArray.Clear();
				indicesArray.Clear();
				renderModes.Clear();
				sdfScalesArray.Clear();
				mgc.End();
			}
			textJobData.Release();
		}
		hasPendingTextWork = false;
		textJobDatas.Clear();
		textJobDatasHandle.Free();
	}

	private static void ConvertMeshInfoToUIRVertex(ATGMeshInfo[] meshInfos, TempMeshAllocator alloc, TextElement visualElement, List<Texture2D> atlases, List<NativeSlice<Vertex>> verticesArray, List<NativeSlice<ushort>> indicesArray, List<GlyphRenderMode> renderModes, List<float> sdfScales)
	{
		float inverseScale = 1f / visualElement.scaledPixelsPerPoint;
		for (int i = 0; i < meshInfos.Length; i++)
		{
			ATGMeshInfo aTGMeshInfo = meshInfos[i];
			int b = (int)(UIRenderDevice.maxVerticesPerPage & -4);
			bool hasMultipleColors = aTGMeshInfo.hasMultipleColors;
			if (hasMultipleColors)
			{
				visualElement.renderData.flags |= RenderDataFlags.IsIgnoringDynamicColorHint;
			}
			else
			{
				visualElement.renderData.flags &= ~RenderDataFlags.IsIgnoringDynamicColorHint;
			}
			for (int j = 0; j < aTGMeshInfo.textElementInfoIndicesByAtlas.Count; j++)
			{
				List<int> list = aTGMeshInfo.textElementInfoIndicesByAtlas[j];
				int num = list.Count * 4;
				while (num > 0)
				{
					int num2 = Mathf.Min(num, b);
					int num3 = num2 >> 2;
					int indexCount = num3 * 6;
					FontAsset fontAsset = aTGMeshInfo.fontAsset;
					atlases.Add(fontAsset.atlasTextures[j]);
					renderModes.Add(fontAsset.atlasRenderMode);
					float item = 0f;
					if (!TextGeneratorUtilities.IsBitmapRendering(renderModes[renderModes.Count - 1]))
					{
						if (atlases[atlases.Count - 1].format == TextureFormat.Alpha8)
						{
							item = fontAsset.atlasPadding + 1;
						}
					}
					sdfScales.Add(item);
					bool flag = fontAsset.atlasRenderMode != GlyphRenderMode.SMOOTH && fontAsset.atlasRenderMode != GlyphRenderMode.COLOR;
					bool isDynamicColor = !hasMultipleColors && (RenderEvents.NeedsColorID(visualElement) || (flag && RenderEvents.NeedsTextCoreSettings(visualElement)));
					alloc.AllocateTempMesh(num2, indexCount, out var vertices, out var indices);
					Vector2 min = visualElement.contentRect.min;
					int num4 = 0;
					int num5 = 0;
					int num6 = 0;
					while (num4 < num2)
					{
						bool isColorGlyph = fontAsset.atlasRenderMode == GlyphRenderMode.COLOR || fontAsset.atlasRenderMode == GlyphRenderMode.COLOR_HINTED;
						NativeTextElementInfo nativeTextElementInfo = aTGMeshInfo.textElementInfos[list[num5]];
						vertices[num4] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.bottomLeft, min, inverseScale, isDynamicColor, isColorGlyph);
						vertices[num4 + 1] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.topLeft, min, inverseScale, isDynamicColor, isColorGlyph);
						vertices[num4 + 2] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.topRight, min, inverseScale, isDynamicColor, isColorGlyph);
						vertices[num4 + 3] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.bottomRight, min, inverseScale, isDynamicColor, isColorGlyph);
						indices[num6] = (ushort)num4;
						indices[num6 + 1] = (ushort)(num4 + 1);
						indices[num6 + 2] = (ushort)(num4 + 2);
						indices[num6 + 3] = (ushort)(num4 + 2);
						indices[num6 + 4] = (ushort)(num4 + 3);
						indices[num6 + 5] = (ushort)num4;
						num4 += 4;
						num5++;
						num6 += 6;
					}
					verticesArray.Add(vertices);
					indicesArray.Add(indices);
					num -= num2;
				}
				Debug.Assert(num == 0);
			}
		}
	}
}
