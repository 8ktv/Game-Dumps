using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEditor.CoreModule", "UnityEngine.UIElementsModule" })]
internal class FontAssetFactory
{
	internal const GlyphRenderMode k_SmoothEditorBitmapGlyphRenderMode = GlyphRenderMode.SMOOTH_HINTED;

	internal const GlyphRenderMode k_RasterEditorBitmapGlyphRenderMode = GlyphRenderMode.RASTER_HINTED;

	private static readonly HashSet<FontAsset> visitedFontAssets = new HashSet<FontAsset>();

	public static FontAsset? CloneFontAssetWithBitmapRendering(FontAsset baseFontAsset, int fontSize, bool isRaster)
	{
		visitedFontAssets.Clear();
		return CloneFontAssetWithBitmapRenderingInternal(baseFontAsset, fontSize, isRaster);
	}

	private static FontAsset? CloneFontAssetWithBitmapRenderingInternal(FontAsset baseFontAsset, int fontSize, bool isRaster)
	{
		visitedFontAssets.Add(baseFontAsset);
		FontAsset fontAsset = CloneFontAssetWithBitmapSettings(baseFontAsset, fontSize, isRaster);
		if (fontAsset != null)
		{
			ProcessFontWeights(fontAsset, baseFontAsset, fontSize, isRaster);
			ProcessFallbackFonts(fontAsset, baseFontAsset, fontSize, isRaster);
		}
		return fontAsset;
	}

	private static FontAsset? CloneFontAssetWithBitmapSettings(FontAsset source, int size, bool isRaster)
	{
		bool flag = source.atlasRenderMode != GlyphRenderMode.SDFAA || !source.IsEditorFont || source.sourceFontFile == null;
		FontAsset fontAsset;
		if (source.atlasPopulationMode == AtlasPopulationMode.DynamicOS)
		{
			fontAsset = FontAsset.CreateFontAsset(source.faceInfo.familyName, source.faceInfo.styleName, size, 6, isRaster ? GlyphRenderMode.RASTER_HINTED : GlyphRenderMode.SMOOTH_HINTED);
			if (fontAsset != null)
			{
				SetupFontAssetForBitmapSettings(fontAsset);
			}
		}
		else if (flag)
		{
			fontAsset = Object.Instantiate(source);
			if (fontAsset != null)
			{
				fontAsset.fallbackFontAssetTable = new List<FontAsset>();
				fontAsset.m_IsClone = true;
				fontAsset.IsEditorFont = true;
				SetHideFlags(fontAsset);
			}
		}
		else
		{
			fontAsset = FontAsset.CreateFontAsset(source.sourceFontFile, size, 6, isRaster ? GlyphRenderMode.RASTER_HINTED : GlyphRenderMode.SMOOTH_HINTED, source.atlasWidth, source.atlasHeight);
			if (fontAsset != null)
			{
				SetupFontAssetForBitmapSettings(fontAsset);
			}
		}
		return fontAsset;
	}

	private static void ProcessFontWeights(FontAsset resultFontAsset, FontAsset baseFontAsset, int fontSize, bool isRaster)
	{
		for (int i = 0; i < baseFontAsset.fontWeightTable.Length; i++)
		{
			FontWeightPair fontWeightPair = baseFontAsset.fontWeightTable[i];
			if (fontWeightPair.regularTypeface != null)
			{
				resultFontAsset.fontWeightTable[i].regularTypeface = CloneFontAssetWithBitmapSettings(fontWeightPair.regularTypeface, fontSize, isRaster);
			}
			if (fontWeightPair.italicTypeface != null)
			{
				resultFontAsset.fontWeightTable[i].italicTypeface = CloneFontAssetWithBitmapSettings(fontWeightPair.italicTypeface, fontSize, isRaster);
			}
		}
	}

	private static void ProcessFallbackFonts(FontAsset resultFontAsset, FontAsset baseFontAsset, int fontSize, bool isRaster)
	{
		if (baseFontAsset.fallbackFontAssetTable == null)
		{
			return;
		}
		foreach (FontAsset item in baseFontAsset.fallbackFontAssetTable)
		{
			if (item != null && !visitedFontAssets.Contains(item))
			{
				visitedFontAssets.Add(item);
				if (resultFontAsset.fallbackFontAssetTable == null)
				{
					List<FontAsset> list = (resultFontAsset.fallbackFontAssetTable = new List<FontAsset>());
				}
				FontAsset fontAsset = CloneFontAssetWithBitmapRenderingInternal(item, fontSize, isRaster);
				if ((bool)fontAsset)
				{
					resultFontAsset.fallbackFontAssetTable.Add(fontAsset);
				}
			}
		}
	}

	internal static FontAsset? ConvertFontToFontAsset(Font font)
	{
		if (font == null)
		{
			return null;
		}
		FontAsset fontAsset = null;
		fontAsset = FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.DEFAULT, 1024, 1024);
		if (fontAsset != null)
		{
			SetupFontAssetSettings(fontAsset);
		}
		return fontAsset;
	}

	internal static void SetupFontAssetSettings(FontAsset fontAsset)
	{
		if ((bool)fontAsset)
		{
			SetHideFlags(fontAsset);
			fontAsset.isMultiAtlasTexturesEnabled = true;
			fontAsset.IsEditorFont = true;
		}
	}

	private static void SetupFontAssetForBitmapSettings(FontAsset fontAsset)
	{
		if ((bool)fontAsset)
		{
			SetHideFlags(fontAsset);
			fontAsset.IsEditorFont = true;
			fontAsset.isMultiAtlasTexturesEnabled = true;
			fontAsset.atlasTexture.filterMode = (TextGenerator.EnableCheckerboardPattern ? FilterMode.Bilinear : FilterMode.Point);
		}
	}

	public static void SetHideFlags(FontAsset fontAsset)
	{
		if ((bool)fontAsset)
		{
			fontAsset.hideFlags = HideFlags.DontSave;
			fontAsset.atlasTextures[0].hideFlags = HideFlags.DontSave;
			fontAsset.material.hideFlags = HideFlags.DontSave;
		}
	}
}
