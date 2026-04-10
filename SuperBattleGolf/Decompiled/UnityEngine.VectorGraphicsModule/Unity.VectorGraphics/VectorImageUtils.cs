using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.VectorGraphics;

[VisibleToOtherModules(new string[] { "UnityEditor.VectorGraphicsModule" })]
internal static class VectorImageUtils
{
	[VisibleToOtherModules(new string[] { "UnityEditor.VectorGraphicsModule" })]
	internal static void MakeVectorImageAsset(IEnumerable<VectorUtils.Geometry> geoms, Rect rect, uint rasterSize, out VectorImage outAsset, out Texture2D outTexAtlas)
	{
		VectorUtils.TextureAtlas textureAtlas = VectorUtils.GenerateAtlas(geoms, rasterSize, generatePOTTexture: false, encodeSettings: false, linear: false);
		if (textureAtlas != null)
		{
			VectorUtils.FillUVs(geoms, textureAtlas);
		}
		bool flag = textureAtlas != null && textureAtlas.Texture != null;
		outTexAtlas = (flag ? textureAtlas.Texture : null);
		List<VectorImageVertex> list = new List<VectorImageVertex>(100);
		List<ushort> list2 = new List<ushort>(300);
		List<GradientSettings> list3 = new List<GradientSettings>();
		Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
		foreach (VectorUtils.Geometry geom in geoms)
		{
			if (geom.Vertices.Length != 0)
			{
				Vector2[] array = new Vector2[geom.Vertices.Length];
				for (int i = 0; i < geom.Vertices.Length; i++)
				{
					Vector2 vector3 = geom.WorldTransform.MultiplyPoint(geom.Vertices[i]);
					array[i] = vector3;
				}
				Rect rect2 = VectorUtils.Bounds(array);
				vector = Vector2.Min(vector, rect2.min);
				vector2 = Vector2.Max(vector2, rect2.max);
			}
		}
		Rect rect3 = Rect.zero;
		if (vector.x != float.MaxValue)
		{
			rect3 = new Rect(vector, vector2 - vector);
		}
		HashSet<int> hashSet = new HashSet<int>();
		hashSet.Add(0);
		Dictionary<IFill, VectorUtils.PackRectItem> dictionary = new Dictionary<IFill, VectorUtils.PackRectItem>();
		if (textureAtlas != null && textureAtlas.Entries != null)
		{
			foreach (VectorUtils.PackRectItem entry in textureAtlas.Entries)
			{
				if (entry.Fill != null)
				{
					dictionary[entry.Fill] = entry;
				}
			}
		}
		if (flag && textureAtlas != null && textureAtlas.Entries != null && textureAtlas.Entries.Count > 0)
		{
			VectorUtils.PackRectItem packRectItem = textureAtlas.Entries[textureAtlas.Entries.Count - 1];
			list3.Add(new GradientSettings
			{
				gradientType = GradientType.Linear,
				addressMode = UnityEngine.UIElements.AddressMode.Wrap,
				radialFocus = Vector2.zero,
				location = new RectInt((int)packRectItem.Position.x, (int)packRectItem.Position.y, (int)packRectItem.Size.x, (int)packRectItem.Size.y)
			});
		}
		foreach (VectorUtils.Geometry geom2 in geoms)
		{
			for (int j = 0; j < geom2.Vertices.Length; j++)
			{
				Vector2 vector4 = geom2.WorldTransform.MultiplyPoint(geom2.Vertices[j]);
				vector4 -= rect3.position;
				geom2.Vertices[j] = vector4;
			}
			VectorUtils.AdjustWinding(geom2.Vertices, geom2.Indices, VectorUtils.WindingDir.CCW);
			int count = list.Count;
			for (int k = 0; k < geom2.Vertices.Length; k++)
			{
				Vector3 position = geom2.Vertices[k];
				position.z = Vertex.nearZ;
				list.Add(new VectorImageVertex
				{
					position = position,
					uv = (flag ? geom2.UVs[k] : Vector2.zero),
					tint = geom2.Color,
					settingIndex = (uint)geom2.SettingIndex
				});
			}
			ushort[] array2 = new ushort[geom2.Indices.Length];
			for (int l = 0; l < geom2.Indices.Length; l++)
			{
				array2[l] = (ushort)(geom2.Indices[l] + count);
			}
			list2.AddRange(array2);
			if (textureAtlas != null && textureAtlas.Entries != null && textureAtlas.Entries.Count > 0 && geom2.Fill != null && dictionary.TryGetValue(geom2.Fill, out var value) && !hashSet.Contains(value.SettingIndex))
			{
				hashSet.Add(value.SettingIndex);
				GradientFillType gradientType = GradientFillType.Linear;
				Vector2 radialFocus = Vector2.zero;
				AddressMode addressMode = AddressMode.Wrap;
				if (geom2.Fill is GradientFill gradientFill)
				{
					gradientType = gradientFill.Type;
					radialFocus = gradientFill.RadialFocus;
					addressMode = gradientFill.Addressing;
				}
				if (geom2.Fill is TextureFill textureFill)
				{
					addressMode = textureFill.Addressing;
				}
				list3.Add(new GradientSettings
				{
					gradientType = (GradientType)gradientType,
					addressMode = (UnityEngine.UIElements.AddressMode)addressMode,
					radialFocus = radialFocus,
					location = new RectInt((int)value.Position.x, (int)value.Position.y, (int)value.Size.x, (int)value.Size.y)
				});
			}
		}
		if (rect == Rect.zero)
		{
			rect = rect3;
		}
		else
		{
			Vector2 vector5 = rect3.position - rect.position;
			for (int m = 0; m < list.Count; m++)
			{
				VectorImageVertex value2 = list[m];
				Vector2 rhs = value2.position;
				rhs += vector5;
				rhs = Vector2.Max(rect.min, Vector2.Min(rect.max, rhs));
				value2.position = new Vector3(rhs.x, rhs.y, value2.position.z);
				list[m] = value2;
			}
		}
		outAsset = MakeVectorImageAsset(list, list2, outTexAtlas, list3, rect);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.VectorGraphicsModule" })]
	internal static Texture2D RenderVectorImageToTexture2D(VectorImage vi, int width, int height, Material mat, int antiAliasing = 1)
	{
		if (vi == null)
		{
			return null;
		}
		if (width <= 0 || height <= 0)
		{
			return null;
		}
		RenderTexture renderTexture = null;
		RenderTexture active = RenderTexture.active;
		RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0);
		renderTextureDescriptor.msaaSamples = antiAliasing;
		renderTextureDescriptor.sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
		RenderTextureDescriptor desc = renderTextureDescriptor;
		renderTexture = (RenderTexture.active = RenderTexture.GetTemporary(desc));
		PanelSettings panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
		panelSettings.clearColor = true;
		panelSettings.clearDepthStencil = true;
		panelSettings.targetTexture = renderTexture;
		GL.PushMatrix();
		BaseRuntimePanel panel = panelSettings.panel;
		VisualElement visualTree = panel.visualTree;
		visualTree.StretchToParentSize();
		visualTree.style.backgroundImage = new StyleBackground(vi);
		panel.Repaint(Event.current);
		panel.Render();
		GL.PopMatrix();
		UnityEngine.Object.DestroyImmediate(panelSettings);
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
		texture2D.hideFlags = HideFlags.HideAndDontSave;
		texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(renderTexture);
		return texture2D;
	}

	private static Texture2D BuildAtlasWithEncodedSettings(GradientSettings[] settings, Texture2D atlas)
	{
		RenderTexture active = RenderTexture.active;
		int num = atlas.width + 3;
		int num2 = Math.Max(settings.Length, atlas.height);
		RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(num, num2, RenderTextureFormat.ARGB32, 0);
		renderTextureDescriptor.sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
		RenderTextureDescriptor desc = renderTextureDescriptor;
		RenderTexture temporary = RenderTexture.GetTemporary(desc);
		GL.Clear(clearDepth: false, clearColor: true, Color.black, 1f);
		Graphics.Blit(atlas, temporary, Vector2.one, new Vector2(-3f / (float)num, 0f));
		RenderTexture.active = temporary;
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGBA32, mipChain: false);
		texture2D.hideFlags = HideFlags.HideAndDontSave;
		texture2D.ReadPixels(new Rect(0f, 0f, num, num2), 0, 0);
		VectorUtils.RawTexture dest = new VectorUtils.RawTexture
		{
			Width = 3,
			Height = settings.Length,
			Rgba = new Color32[3 * settings.Length]
		};
		for (int i = 0; i < settings.Length; i++)
		{
			GradientSettings gradientSettings = settings[i];
			int num3 = 0;
			int destY = i;
			if (gradientSettings.gradientType == GradientType.Radial)
			{
				Vector2 radialFocus = gradientSettings.radialFocus;
				radialFocus += Vector2.one;
				radialFocus /= 2f;
				radialFocus.y = 1f - radialFocus.y;
				VectorUtils.WriteRawFloat4Packed(dest, (float)gradientSettings.gradientType / 255f, (float)gradientSettings.addressMode / 255f, radialFocus.x, radialFocus.y, num3++, destY);
			}
			else
			{
				VectorUtils.WriteRawFloat4Packed(dest, 0f, (float)gradientSettings.addressMode / 255f, 0f, 0f, num3++, destY);
			}
			Vector2Int position = gradientSettings.location.position;
			Vector2Int size = gradientSettings.location.size;
			size.x--;
			size.y--;
			VectorUtils.WriteRawInt2Packed(dest, position.x + 3, position.y, num3++, destY);
			VectorUtils.WriteRawInt2Packed(dest, size.x, size.y, num3++, destY);
		}
		texture2D.SetPixels32(0, 0, 3, settings.Length, dest.Rgba, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
		return texture2D;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.VectorGraphicsModule" })]
	internal static VectorImage MakeVectorImageAsset(List<VectorImageVertex> vertices, List<ushort> indices, Texture2D atlas, List<GradientSettings> settings, Rect rect)
	{
		VectorImage vectorImage = ScriptableObject.CreateInstance<VectorImage>();
		vectorImage.vertices = vertices.ToArray();
		vectorImage.indices = indices.ToArray();
		vectorImage.atlas = atlas;
		vectorImage.settings = settings.ToArray();
		vectorImage.size = rect.size;
		return vectorImage;
	}
}
