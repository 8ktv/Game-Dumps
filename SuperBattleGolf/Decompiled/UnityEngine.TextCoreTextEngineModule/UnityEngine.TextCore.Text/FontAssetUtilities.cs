using System.Collections.Generic;

namespace UnityEngine.TextCore.Text;

internal static class FontAssetUtilities
{
	private static HashSet<int> k_SearchedAssets;

	internal static Character GetCharacterFromFontAsset(uint unicode, FontAsset sourceFontAsset, bool includeFallbacks, FontStyles fontStyle, TextFontWeight fontWeight, out bool isAlternativeTypeface, bool populateLigatures)
	{
		if (includeFallbacks)
		{
			if (k_SearchedAssets == null)
			{
				k_SearchedAssets = new HashSet<int>();
			}
			else
			{
				k_SearchedAssets.Clear();
			}
		}
		return GetCharacterFromFontAsset_Internal(unicode, sourceFontAsset, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface, populateLigatures);
	}

	private static Character GetCharacterFromFontAsset_Internal(uint unicode, FontAsset sourceFontAsset, bool includeFallbacks, FontStyles fontStyle, TextFontWeight fontWeight, out bool isAlternativeTypeface, bool populateLigatures)
	{
		bool flag = !TextGenerator.IsExecutingJob;
		isAlternativeTypeface = false;
		Character character = null;
		bool flag2 = (fontStyle & FontStyles.Italic) == FontStyles.Italic;
		if (flag2 || fontWeight != TextFontWeight.Regular)
		{
			if (!flag && sourceFontAsset.m_CharacterLookupDictionary == null)
			{
				return null;
			}
			if (sourceFontAsset.GetCharacterInLookupCache(unicode, fontStyle, fontWeight, out character))
			{
				if (character.textAsset != null)
				{
					return character;
				}
				if (!flag)
				{
					return null;
				}
				sourceFontAsset.RemoveCharacterInLookupCache(unicode, fontStyle, fontWeight);
			}
			FontWeightPair[] fontWeightTable = sourceFontAsset.fontWeightTable;
			int textFontWeightIndex = TextUtilities.GetTextFontWeightIndex(fontWeight);
			FontAsset fontAsset = (flag2 ? fontWeightTable[textFontWeightIndex].italicTypeface : fontWeightTable[textFontWeightIndex].regularTypeface);
			if (fontAsset != null)
			{
				if (!flag && fontAsset.m_CharacterLookupDictionary == null)
				{
					return null;
				}
				if (fontAsset.GetCharacterInLookupCache(unicode, fontStyle, fontWeight, out character))
				{
					if (character.textAsset != null)
					{
						isAlternativeTypeface = true;
						return character;
					}
					if (!flag)
					{
						return null;
					}
					fontAsset.RemoveCharacterInLookupCache(unicode, fontStyle, fontWeight);
				}
				if (fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic || fontAsset.atlasPopulationMode == AtlasPopulationMode.DynamicOS)
				{
					if (!flag)
					{
						if (!fontAsset.m_MissingUnicodesFromFontFile.Contains(unicode))
						{
							return null;
						}
					}
					else if (fontAsset.TryAddCharacterInternal(unicode, fontStyle, fontWeight, out character, populateLigatures))
					{
						isAlternativeTypeface = true;
						return character;
					}
				}
				else if (fontAsset.GetCharacterInLookupCache(unicode, FontStyles.Normal, TextFontWeight.Regular, out character))
				{
					if (character.textAsset != null)
					{
						isAlternativeTypeface = true;
						return character;
					}
					if (!flag)
					{
						return null;
					}
					fontAsset.RemoveCharacterInLookupCache(unicode, fontStyle, fontWeight);
				}
			}
		}
		if (!flag && sourceFontAsset.m_CharacterLookupDictionary == null)
		{
			return null;
		}
		if (sourceFontAsset.GetCharacterInLookupCache(unicode, FontStyles.Normal, TextFontWeight.Regular, out character))
		{
			if (character.textAsset != null)
			{
				return character;
			}
			if (!flag)
			{
				return null;
			}
			sourceFontAsset.RemoveCharacterInLookupCache(unicode, FontStyles.Normal, TextFontWeight.Regular);
		}
		if (sourceFontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic || sourceFontAsset.atlasPopulationMode == AtlasPopulationMode.DynamicOS)
		{
			if (!flag)
			{
				return null;
			}
			if (sourceFontAsset.TryAddCharacterInternal(unicode, FontStyles.Normal, TextFontWeight.Regular, out character, populateLigatures))
			{
				return character;
			}
		}
		if (character == null && !flag)
		{
			return null;
		}
		if (character == null && includeFallbacks && sourceFontAsset.fallbackFontAssetTable != null)
		{
			List<FontAsset> fallbackFontAssetTable = sourceFontAsset.fallbackFontAssetTable;
			int count = fallbackFontAssetTable.Count;
			if (count == 0)
			{
				return null;
			}
			for (int i = 0; i < count; i++)
			{
				FontAsset fontAsset2 = fallbackFontAssetTable[i];
				if (fontAsset2 == null)
				{
					continue;
				}
				int hashCode = fontAsset2.GetHashCode();
				if (k_SearchedAssets.Add(hashCode))
				{
					character = GetCharacterFromFontAsset_Internal(unicode, fontAsset2, includeFallbacks: true, fontStyle, fontWeight, out isAlternativeTypeface, populateLigatures);
					if (character != null)
					{
						return character;
					}
				}
			}
		}
		return null;
	}

	public static Character GetCharacterFromFontAssets(uint unicode, FontAsset sourceFontAsset, List<FontAsset> fontAssets, List<FontAsset> OSFallbackList, bool includeFallbacks, FontStyles fontStyle, TextFontWeight fontWeight, out bool isAlternativeTypeface)
	{
		return GetCharacterFromFontAssetsInternal(unicode, sourceFontAsset, fontAssets, OSFallbackList, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface);
	}

	internal static Character GetCharacterFromFontAssetsInternal(uint unicode, FontAsset sourceFontAsset, List<FontAsset> fontAssets, List<FontAsset> OSFallbackList, bool includeFallbacks, FontStyles fontStyle, TextFontWeight fontWeight, out bool isAlternativeTypeface, bool populateLigatures = true)
	{
		isAlternativeTypeface = false;
		if (includeFallbacks)
		{
			if (k_SearchedAssets == null)
			{
				k_SearchedAssets = new HashSet<int>();
			}
			else
			{
				k_SearchedAssets.Clear();
			}
		}
		Character characterFromFontAssetsInternal = GetCharacterFromFontAssetsInternal(unicode, fontAssets, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface, populateLigatures);
		if (characterFromFontAssetsInternal != null)
		{
			return characterFromFontAssetsInternal;
		}
		return GetCharacterFromFontAssetsInternal(unicode, OSFallbackList, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface, populateLigatures);
	}

	private static Character GetCharacterFromFontAssetsInternal(uint unicode, List<FontAsset> fontAssets, bool includeFallbacks, FontStyles fontStyle, TextFontWeight fontWeight, out bool isAlternativeTypeface, bool populateLigatures = true)
	{
		isAlternativeTypeface = false;
		if (fontAssets == null || fontAssets.Count == 0)
		{
			return null;
		}
		if (includeFallbacks)
		{
			if (k_SearchedAssets == null)
			{
				k_SearchedAssets = new HashSet<int>();
			}
			else
			{
				k_SearchedAssets.Clear();
			}
		}
		int count = fontAssets.Count;
		for (int i = 0; i < count; i++)
		{
			FontAsset fontAsset = fontAssets[i];
			if (!(fontAsset == null))
			{
				Character characterFromFontAsset_Internal = GetCharacterFromFontAsset_Internal(unicode, fontAsset, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface, populateLigatures);
				if (characterFromFontAsset_Internal != null)
				{
					return characterFromFontAsset_Internal;
				}
			}
		}
		return null;
	}

	internal static TextElement GetTextElementFromTextAssets(uint unicode, FontAsset sourceFontAsset, List<TextAsset> textAssets, bool includeFallbacks, FontStyles fontStyle, TextFontWeight fontWeight, out bool isAlternativeTypeface, bool populateLigatures)
	{
		isAlternativeTypeface = false;
		if (textAssets == null || textAssets.Count == 0)
		{
			return null;
		}
		if (includeFallbacks)
		{
			if (k_SearchedAssets == null)
			{
				k_SearchedAssets = new HashSet<int>();
			}
			else
			{
				k_SearchedAssets.Clear();
			}
		}
		int count = textAssets.Count;
		for (int i = 0; i < count; i++)
		{
			TextAsset textAsset = textAssets[i];
			if (textAsset == null)
			{
				continue;
			}
			if (textAsset.GetType() == typeof(FontAsset))
			{
				FontAsset sourceFontAsset2 = textAsset as FontAsset;
				Character characterFromFontAsset_Internal = GetCharacterFromFontAsset_Internal(unicode, sourceFontAsset2, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface, populateLigatures);
				if (characterFromFontAsset_Internal != null)
				{
					return characterFromFontAsset_Internal;
				}
			}
			else
			{
				SpriteAsset spriteAsset = textAsset as SpriteAsset;
				SpriteCharacter spriteCharacterFromSpriteAsset_Internal = GetSpriteCharacterFromSpriteAsset_Internal(unicode, spriteAsset, includeFallbacks: true);
				if (spriteCharacterFromSpriteAsset_Internal != null)
				{
					return spriteCharacterFromSpriteAsset_Internal;
				}
			}
		}
		return null;
	}

	public static SpriteCharacter GetSpriteCharacterFromSpriteAsset(uint unicode, SpriteAsset spriteAsset, bool includeFallbacks)
	{
		bool flag = !TextGenerator.IsExecutingJob;
		if (spriteAsset == null)
		{
			return null;
		}
		if (spriteAsset.spriteCharacterLookupTable.TryGetValue(unicode, out var value))
		{
			return value;
		}
		if (includeFallbacks)
		{
			if (k_SearchedAssets == null)
			{
				k_SearchedAssets = new HashSet<int>();
			}
			else
			{
				k_SearchedAssets.Clear();
			}
			k_SearchedAssets.Add(spriteAsset.GetHashCode());
			List<SpriteAsset> fallbackSpriteAssets = spriteAsset.fallbackSpriteAssets;
			if (fallbackSpriteAssets != null && fallbackSpriteAssets.Count > 0)
			{
				int count = fallbackSpriteAssets.Count;
				for (int i = 0; i < count; i++)
				{
					SpriteAsset spriteAsset2 = fallbackSpriteAssets[i];
					if (spriteAsset2 == null)
					{
						continue;
					}
					int hashCode = spriteAsset2.GetHashCode();
					if (k_SearchedAssets.Add(hashCode))
					{
						value = GetSpriteCharacterFromSpriteAsset_Internal(unicode, spriteAsset2, includeFallbacks: true);
						if (value != null)
						{
							return value;
						}
					}
				}
			}
		}
		return null;
	}

	private static SpriteCharacter GetSpriteCharacterFromSpriteAsset_Internal(uint unicode, SpriteAsset spriteAsset, bool includeFallbacks)
	{
		if (spriteAsset.spriteCharacterLookupTable.TryGetValue(unicode, out var value))
		{
			return value;
		}
		if (includeFallbacks)
		{
			List<SpriteAsset> fallbackSpriteAssets = spriteAsset.fallbackSpriteAssets;
			if (fallbackSpriteAssets != null && fallbackSpriteAssets.Count > 0)
			{
				int count = fallbackSpriteAssets.Count;
				for (int i = 0; i < count; i++)
				{
					SpriteAsset spriteAsset2 = fallbackSpriteAssets[i];
					if (spriteAsset2 == null)
					{
						continue;
					}
					int hashCode = spriteAsset2.GetHashCode();
					if (k_SearchedAssets.Add(hashCode))
					{
						value = GetSpriteCharacterFromSpriteAsset_Internal(unicode, spriteAsset2, includeFallbacks: true);
						if (value != null)
						{
							return value;
						}
					}
				}
			}
		}
		return null;
	}

	public static uint GetCodePoint(string text, ref int index)
	{
		char c = text[index];
		if (char.IsHighSurrogate(c) && index + 1 < text.Length && char.IsLowSurrogate(text[index + 1]))
		{
			uint result = (uint)char.ConvertToUtf32(c, text[index + 1]);
			index++;
			return result;
		}
		return c;
	}

	public static uint GetCodePoint(uint[] codesPoints, ref int index)
	{
		char c = (char)codesPoints[index];
		if (char.IsHighSurrogate(c) && index + 1 < codesPoints.Length && char.IsLowSurrogate((char)codesPoints[index + 1]))
		{
			uint result = (uint)char.ConvertToUtf32(c, (char)codesPoints[index + 1]);
			index++;
			return result;
		}
		return c;
	}
}
