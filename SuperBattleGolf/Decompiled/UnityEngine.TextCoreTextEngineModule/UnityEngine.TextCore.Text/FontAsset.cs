using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.Serialization;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/TextCoreTextEngine/Native/FontAsset.h")]
[ExcludeFromPreset]
public class FontAsset : TextAsset
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(FontAsset fontAsset)
		{
			return fontAsset.m_NativeFontAsset;
		}
	}

	[SerializeField]
	internal string m_SourceFontFileGUID;

	[SerializeField]
	internal FontAssetCreationEditorSettings m_fontAssetCreationEditorSettings;

	[SerializeField]
	private Font m_SourceFontFile;

	[SerializeField]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal string m_SourceFontFilePath;

	[SerializeField]
	private AtlasPopulationMode m_AtlasPopulationMode;

	[SerializeField]
	internal bool InternalDynamicOS;

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	[SerializeField]
	internal bool IsEditorFont = false;

	[SerializeField]
	internal FaceInfo m_FaceInfo;

	private int m_FamilyNameHashCode;

	private int m_StyleNameHashCode;

	[SerializeField]
	internal List<Glyph> m_GlyphTable = new List<Glyph>();

	internal Dictionary<uint, Glyph> m_GlyphLookupDictionary;

	[SerializeField]
	internal List<Character> m_CharacterTable = new List<Character>();

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal Dictionary<uint, Character> m_CharacterLookupDictionary;

	internal Texture2D m_AtlasTexture;

	[SerializeField]
	internal Texture2D[] m_AtlasTextures;

	[SerializeField]
	internal int m_AtlasTextureIndex;

	[SerializeField]
	private bool m_IsMultiAtlasTexturesEnabled = true;

	[SerializeField]
	private bool m_GetFontFeatures = true;

	[SerializeField]
	private bool m_ClearDynamicDataOnBuild = true;

	[SerializeField]
	internal int m_AtlasWidth;

	[SerializeField]
	internal int m_AtlasHeight;

	[SerializeField]
	internal int m_AtlasPadding;

	[SerializeField]
	internal GlyphRenderMode m_AtlasRenderMode;

	[SerializeField]
	private List<GlyphRect> m_UsedGlyphRects;

	[SerializeField]
	private List<GlyphRect> m_FreeGlyphRects;

	[SerializeField]
	internal FontFeatureTable m_FontFeatureTable = new FontFeatureTable();

	[SerializeField]
	internal bool m_ShouldReimportFontFeatures;

	[SerializeField]
	internal List<FontAsset> m_FallbackFontAssetTable;

	[SerializeField]
	private FontWeightPair[] m_FontWeightTable = new FontWeightPair[10];

	[SerializeField]
	[FormerlySerializedAs("normalStyle")]
	internal float m_RegularStyleWeight = 0f;

	[SerializeField]
	[FormerlySerializedAs("normalSpacingOffset")]
	internal float m_RegularStyleSpacing = 0f;

	[FormerlySerializedAs("boldStyle")]
	[SerializeField]
	internal float m_BoldStyleWeight = 0.75f;

	[FormerlySerializedAs("boldSpacing")]
	[SerializeField]
	internal float m_BoldStyleSpacing = 7f;

	[FormerlySerializedAs("italicStyle")]
	[SerializeField]
	internal byte m_ItalicStyleSlant = 35;

	[SerializeField]
	[FormerlySerializedAs("tabSize")]
	internal byte m_TabMultiple = 10;

	internal bool IsFontAssetLookupTablesDirty;

	private IntPtr m_NativeFontAsset = IntPtr.Zero;

	private List<Glyph> m_GlyphsToRender = new List<Glyph>();

	private List<Glyph> m_GlyphsRendered = new List<Glyph>();

	private List<uint> m_GlyphIndexList = new List<uint>();

	private List<uint> m_GlyphIndexListNewlyAdded = new List<uint>();

	internal List<uint> m_GlyphsToAdd = new List<uint>();

	internal HashSet<uint> m_GlyphsToAddLookup = new HashSet<uint>();

	internal List<Character> m_CharactersToAdd = new List<Character>();

	internal HashSet<uint> m_CharactersToAddLookup = new HashSet<uint>();

	internal List<uint> s_MissingCharacterList = new List<uint>();

	internal HashSet<uint> m_MissingUnicodesFromFontFile = new HashSet<uint>();

	internal Dictionary<(uint, uint), uint> m_VariantGlyphIndexes = new Dictionary<(uint, uint), uint>();

	internal bool m_IsClone;

	private static readonly List<WeakReference<FontAsset>> s_CallbackInstances = new List<WeakReference<FontAsset>>();

	private static ProfilerMarker k_ReadFontAssetDefinitionMarker = new ProfilerMarker("FontAsset.ReadFontAssetDefinition");

	private static ProfilerMarker k_AddSynthesizedCharactersMarker = new ProfilerMarker("FontAsset.AddSynthesizedCharacters");

	private static ProfilerMarker k_TryAddGlyphMarker = new ProfilerMarker("FontAsset.TryAddGlyph");

	private static ProfilerMarker k_TryAddCharacterMarker = new ProfilerMarker("FontAsset.TryAddCharacter");

	private static ProfilerMarker k_TryAddCharactersMarker = new ProfilerMarker("FontAsset.TryAddCharacters");

	private static ProfilerMarker k_UpdateLigatureSubstitutionRecordsMarker = new ProfilerMarker("FontAsset.UpdateLigatureSubstitutionRecords");

	private static ProfilerMarker k_UpdateGlyphAdjustmentRecordsMarker = new ProfilerMarker("FontAsset.UpdateGlyphAdjustmentRecords");

	private static ProfilerMarker k_UpdateDiacriticalMarkAdjustmentRecordsMarker = new ProfilerMarker("FontAsset.UpdateDiacriticalAdjustmentRecords");

	private static ProfilerMarker k_ClearFontAssetDataMarker = new ProfilerMarker("FontAsset.ClearFontAssetData");

	private static ProfilerMarker k_UpdateFontAssetDataMarker = new ProfilerMarker("FontAsset.UpdateFontAssetData");

	private static string s_DefaultMaterialSuffix = " Atlas Material";

	private static HashSet<int> k_SearchedFontAssetLookup;

	private static List<FontAsset> k_FontAssets_FontFeaturesUpdateQueue = new List<FontAsset>();

	private static HashSet<int> k_FontAssets_FontFeaturesUpdateQueueLookup = new HashSet<int>();

	private static List<FontAsset> k_FontAssets_KerningUpdateQueue = new List<FontAsset>();

	private static HashSet<int> k_FontAssets_KerningUpdateQueueLookup = new HashSet<int>();

	private static List<Texture2D> k_FontAssets_AtlasTexturesUpdateQueue = new List<Texture2D>();

	private static HashSet<int> k_FontAssets_AtlasTexturesUpdateQueueLookup = new HashSet<int>();

	internal static uint[] k_GlyphIndexArray;

	private static HashSet<int> visitedFontAssets = new HashSet<int>();

	public FontAssetCreationEditorSettings fontAssetCreationEditorSettings
	{
		get
		{
			return m_fontAssetCreationEditorSettings;
		}
		set
		{
			m_fontAssetCreationEditorSettings = value;
		}
	}

	public Font sourceFontFile
	{
		get
		{
			return m_SourceFontFile;
		}
		internal set
		{
			m_SourceFontFile = value;
		}
	}

	public AtlasPopulationMode atlasPopulationMode
	{
		get
		{
			return m_AtlasPopulationMode;
		}
		set
		{
			m_AtlasPopulationMode = value;
		}
	}

	public FaceInfo faceInfo
	{
		get
		{
			return m_FaceInfo;
		}
		set
		{
			m_FaceInfo = value;
			if (m_NativeFontAsset != IntPtr.Zero)
			{
				UpdateFaceInfo();
			}
		}
	}

	internal int familyNameHashCode
	{
		get
		{
			if (m_FamilyNameHashCode == 0)
			{
				m_FamilyNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(m_FaceInfo.familyName);
			}
			return m_FamilyNameHashCode;
		}
		set
		{
			m_FamilyNameHashCode = value;
		}
	}

	internal int styleNameHashCode
	{
		get
		{
			if (m_StyleNameHashCode == 0)
			{
				m_StyleNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(m_FaceInfo.styleName);
			}
			return m_StyleNameHashCode;
		}
		set
		{
			m_StyleNameHashCode = value;
		}
	}

	public List<Glyph> glyphTable
	{
		get
		{
			return m_GlyphTable;
		}
		internal set
		{
			m_GlyphTable = value;
		}
	}

	public Dictionary<uint, Glyph> glyphLookupTable
	{
		get
		{
			if (m_GlyphLookupDictionary == null)
			{
				ReadFontAssetDefinition();
			}
			return m_GlyphLookupDictionary;
		}
	}

	public List<Character> characterTable
	{
		get
		{
			return m_CharacterTable;
		}
		internal set
		{
			m_CharacterTable = value;
		}
	}

	public Dictionary<uint, Character> characterLookupTable
	{
		get
		{
			if (m_CharacterLookupDictionary == null)
			{
				ReadFontAssetDefinition();
			}
			return m_CharacterLookupDictionary;
		}
	}

	public Texture2D atlasTexture
	{
		get
		{
			if (m_AtlasTexture == null)
			{
				m_AtlasTexture = atlasTextures[0];
			}
			return m_AtlasTexture;
		}
	}

	public Texture2D[] atlasTextures
	{
		get
		{
			return m_AtlasTextures;
		}
		set
		{
			m_AtlasTextures = value;
		}
	}

	public int atlasTextureCount => m_AtlasTextureIndex + 1;

	public bool isMultiAtlasTexturesEnabled
	{
		get
		{
			return m_IsMultiAtlasTexturesEnabled;
		}
		set
		{
			m_IsMultiAtlasTexturesEnabled = value;
		}
	}

	public bool getFontFeatures
	{
		get
		{
			return m_GetFontFeatures;
		}
		set
		{
			m_GetFontFeatures = value;
		}
	}

	internal bool clearDynamicDataOnBuild
	{
		get
		{
			return m_ClearDynamicDataOnBuild;
		}
		set
		{
			m_ClearDynamicDataOnBuild = value;
		}
	}

	public int atlasWidth
	{
		get
		{
			return m_AtlasWidth;
		}
		internal set
		{
			m_AtlasWidth = value;
		}
	}

	public int atlasHeight
	{
		get
		{
			return m_AtlasHeight;
		}
		internal set
		{
			m_AtlasHeight = value;
		}
	}

	public int atlasPadding
	{
		get
		{
			return m_AtlasPadding;
		}
		internal set
		{
			m_AtlasPadding = value;
		}
	}

	public GlyphRenderMode atlasRenderMode
	{
		get
		{
			return m_AtlasRenderMode;
		}
		internal set
		{
			m_AtlasRenderMode = value;
		}
	}

	internal List<GlyphRect> usedGlyphRects
	{
		get
		{
			return m_UsedGlyphRects;
		}
		set
		{
			m_UsedGlyphRects = value;
		}
	}

	internal List<GlyphRect> freeGlyphRects
	{
		get
		{
			return m_FreeGlyphRects;
		}
		set
		{
			m_FreeGlyphRects = value;
		}
	}

	public FontFeatureTable fontFeatureTable
	{
		get
		{
			return m_FontFeatureTable;
		}
		internal set
		{
			m_FontFeatureTable = value;
		}
	}

	public List<FontAsset> fallbackFontAssetTable
	{
		get
		{
			return m_FallbackFontAssetTable;
		}
		set
		{
			m_FallbackFontAssetTable = value;
		}
	}

	public FontWeightPair[] fontWeightTable
	{
		get
		{
			return m_FontWeightTable;
		}
		internal set
		{
			m_FontWeightTable = value;
		}
	}

	public float regularStyleWeight
	{
		get
		{
			return m_RegularStyleWeight;
		}
		set
		{
			m_RegularStyleWeight = value;
		}
	}

	public float regularStyleSpacing
	{
		get
		{
			return m_RegularStyleSpacing;
		}
		set
		{
			m_RegularStyleSpacing = value;
		}
	}

	public float boldStyleWeight
	{
		get
		{
			return m_BoldStyleWeight;
		}
		set
		{
			m_BoldStyleWeight = value;
		}
	}

	public float boldStyleSpacing
	{
		get
		{
			return m_BoldStyleSpacing;
		}
		set
		{
			m_BoldStyleSpacing = value;
		}
	}

	public byte italicStyleSlant
	{
		get
		{
			return m_ItalicStyleSlant;
		}
		set
		{
			m_ItalicStyleSlant = value;
		}
	}

	public byte tabMultiple
	{
		get
		{
			return m_TabMultiple;
		}
		set
		{
			m_TabMultiple = value;
		}
	}

	internal IntPtr nativeFontAsset
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		get
		{
			EnsureNativeFontAssetIsCreated();
			return m_NativeFontAsset;
		}
	}

	private static void EnsureAdditionalCapacity<T>(List<T> container, int additionalCapacity)
	{
		int num = container.Count + additionalCapacity;
		if (container.Capacity < num)
		{
			container.Capacity = num;
		}
	}

	private static void EnsureAdditionalCapacity<TKey, TValue>(Dictionary<TKey, TValue> container, int additionalCapacity)
	{
		int capacity = container.Count + additionalCapacity;
		container.EnsureCapacity(capacity);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal bool IsBitmap()
	{
		return ((GlyphRasterModes)m_AtlasRenderMode).HasFlag(GlyphRasterModes.RASTER_MODE_BITMAP) && !((GlyphRasterModes)m_AtlasRenderMode).HasFlag(GlyphRasterModes.RASTER_MODE_COLOR);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal bool IsRaster()
	{
		return m_AtlasRenderMode == GlyphRenderMode.RASTER_HINTED;
	}

	internal bool IsColor()
	{
		return ((GlyphRasterModes)m_AtlasRenderMode).HasFlag(GlyphRasterModes.RASTER_MODE_COLOR);
	}

	public static FontAsset CreateFontAsset(string familyName, string styleName, int pointSize = 90)
	{
		FontAsset fontAsset = CreateFontAssetInternal(familyName, styleName, pointSize);
		if (fontAsset == null)
		{
			Debug.Log("Unable to find a font file with the specified Family Name [" + familyName + "] and Style [" + styleName + "].");
			return null;
		}
		return fontAsset;
	}

	internal static FontAsset? CreateFontAssetInternal(string familyName, string styleName, int pointSize = 90)
	{
		if (FontEngine.TryGetSystemFontReference(familyName, styleName, out var fontRef))
		{
			return CreateFontAsset(fontRef.filePath, fontRef.faceIndex, pointSize, 9, GlyphRenderMode.DEFAULT, 1024, 1024, AtlasPopulationMode.DynamicOS);
		}
		return null;
	}

	public static FontAsset? CreateFontAsset(string familyName, string styleName, int pointSize, int padding, GlyphRenderMode renderMode)
	{
		if (FontEngine.TryGetSystemFontReference(familyName, styleName, out var fontRef))
		{
			return CreateFontAsset(fontRef.filePath, fontRef.faceIndex, pointSize, padding, renderMode, 1024, 1024, AtlasPopulationMode.DynamicOS);
		}
		return null;
	}

	internal static List<FontAsset> CreateFontAssetOSFallbackList(string[] fallbacksFamilyNames, int pointSize = 90)
	{
		List<FontAsset> list = new List<FontAsset>();
		foreach (string familyName in fallbacksFamilyNames)
		{
			FontAsset fontAsset = CreateFontAssetFromFamilyName(familyName, pointSize);
			if (!(fontAsset == null))
			{
				list.Add(fontAsset);
			}
		}
		return list;
	}

	internal static FontAsset CreateFontAssetWithOSFallbackList(string[] fallbacksFamilyNames, int pointSize = 90)
	{
		FontAsset fontAsset = null;
		foreach (string familyName in fallbacksFamilyNames)
		{
			FontAsset fontAsset2 = CreateFontAssetFromFamilyName(familyName, pointSize);
			if (!(fontAsset2 == null))
			{
				if (fontAsset == null)
				{
					fontAsset = fontAsset2;
				}
				if (fontAsset.fallbackFontAssetTable == null)
				{
					fontAsset.fallbackFontAssetTable = new List<FontAsset>();
				}
				fontAsset.fallbackFontAssetTable.Add(fontAsset2);
			}
		}
		return fontAsset;
	}

	private static FontAsset CreateFontAssetFromFamilyName(string familyName, int pointSize = 90)
	{
		FontAsset fontAsset = null;
		if (FontEngine.TryGetSystemFontReference(familyName, null, out var fontRef))
		{
			fontAsset = CreateFontAsset(fontRef.filePath, fontRef.faceIndex, pointSize, 9, GlyphRenderMode.DEFAULT, 1024, 1024, AtlasPopulationMode.DynamicOS);
		}
		if (fontAsset == null)
		{
			return null;
		}
		FontAssetFactory.SetHideFlags(fontAsset);
		fontAsset.isMultiAtlasTexturesEnabled = true;
		fontAsset.InternalDynamicOS = true;
		return fontAsset;
	}

	public static FontAsset CreateFontAsset(string fontFilePath, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight)
	{
		return CreateFontAsset(fontFilePath, faceIndex, samplingPointSize, atlasPadding, renderMode, atlasWidth, atlasHeight, AtlasPopulationMode.Dynamic);
	}

	private static FontAsset CreateFontAsset(string fontFilePath, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode, bool enableMultiAtlasSupport = true)
	{
		if (FontEngine.LoadFontFace(fontFilePath, samplingPointSize, faceIndex) != FontEngineError.Success)
		{
			Debug.Log("Unable to load font face from [" + fontFilePath + "].");
			return null;
		}
		FontAsset fontAsset = CreateFontAssetInstance(null, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
		if ((bool)fontAsset)
		{
			fontAsset.m_SourceFontFilePath = fontFilePath;
		}
		return fontAsset;
	}

	public static FontAsset CreateFontAsset(Font font)
	{
		return CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024);
	}

	public static FontAsset CreateFontAsset(Font font, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.Dynamic, bool enableMultiAtlasSupport = true)
	{
		return CreateFontAsset(font, 0, samplingPointSize, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
	}

	private static FontAsset CreateFontAsset(Font font, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.Dynamic, bool enableMultiAtlasSupport = true)
	{
		if (font.name == "LegacyRuntime")
		{
			string[] oSFallbacks = Font.GetOSFallbacks();
			if (FontEngine.LoadFontFace(font, samplingPointSize, faceIndex) == FontEngineError.Success)
			{
				FontAsset fontAsset = CreateFontAssetInstance(font, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
				List<FontAsset> list = CreateFontAssetOSFallbackList(oSFallbacks, samplingPointSize);
				fontAsset.fallbackFontAssetTable = list;
				return fontAsset;
			}
			FontAsset fontAsset2 = CreateFontAssetWithOSFallbackList(oSFallbacks, samplingPointSize);
			if (fontAsset2 != null)
			{
				return fontAsset2;
			}
		}
		if (FontEngine.LoadFontFace(font, samplingPointSize, faceIndex) != FontEngineError.Success)
		{
			FontAsset fontAsset3 = CreateFontAsset(font.name, "Regular");
			if (fontAsset3 != null)
			{
				return fontAsset3;
			}
			Debug.LogWarning("Unable to load font face for [" + font.name + "]. Make sure \"Include Font Data\" is enabled in the Font Import Settings.", font);
			return null;
		}
		return CreateFontAssetInstance(font, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
	}

	private static FontAsset CreateFontAssetInstance(Font font, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode, bool enableMultiAtlasSupport)
	{
		FontAsset fontAsset = ScriptableObject.CreateInstance<FontAsset>();
		fontAsset.m_Version = "1.1.0";
		fontAsset.faceInfo = FontEngine.GetFaceInfo();
		if (renderMode == GlyphRenderMode.DEFAULT)
		{
			renderMode = (FontEngine.IsColorFontFace() ? GlyphRenderMode.COLOR : GlyphRenderMode.SDFAA);
		}
		if (atlasPopulationMode == AtlasPopulationMode.Dynamic && font != null)
		{
			fontAsset.sourceFontFile = font;
		}
		fontAsset.atlasPopulationMode = atlasPopulationMode;
		fontAsset.atlasWidth = atlasWidth;
		fontAsset.atlasHeight = atlasHeight;
		fontAsset.atlasPadding = atlasPadding;
		fontAsset.atlasRenderMode = renderMode;
		fontAsset.atlasTextures = new Texture2D[1];
		TextureFormat textureFormat = (((renderMode & (GlyphRenderMode)65536) != (GlyphRenderMode)65536) ? TextureFormat.Alpha8 : TextureFormat.RGBA32);
		Texture2D texture2D = new Texture2D(1, 1, textureFormat, mipChain: false);
		fontAsset.atlasTextures[0] = texture2D;
		fontAsset.isMultiAtlasTexturesEnabled = enableMultiAtlasSupport;
		int num;
		if ((renderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16)
		{
			num = 0;
			Material material;
			if (textureFormat == TextureFormat.Alpha8)
			{
				if (!TextShaderUtilities.ShaderRef_MobileBitmap)
				{
					return null;
				}
				material = new Material(TextShaderUtilities.ShaderRef_MobileBitmap);
			}
			else
			{
				if (!TextShaderUtilities.ShaderRef_Sprite)
				{
					return null;
				}
				material = new Material(TextShaderUtilities.ShaderRef_Sprite);
			}
			material.SetTexture(TextShaderUtilities.ID_MainTex, texture2D);
			material.SetFloat(TextShaderUtilities.ID_TextureWidth, atlasWidth);
			material.SetFloat(TextShaderUtilities.ID_TextureHeight, atlasHeight);
			fontAsset.material = material;
		}
		else
		{
			num = 1;
			Material material2 = new Material(TextShaderUtilities.ShaderRef_MobileSDF);
			material2.SetTexture(TextShaderUtilities.ID_MainTex, texture2D);
			material2.SetFloat(TextShaderUtilities.ID_TextureWidth, atlasWidth);
			material2.SetFloat(TextShaderUtilities.ID_TextureHeight, atlasHeight);
			material2.SetFloat(TextShaderUtilities.ID_GradientScale, atlasPadding + num);
			material2.SetFloat(TextShaderUtilities.ID_WeightNormal, fontAsset.regularStyleWeight);
			material2.SetFloat(TextShaderUtilities.ID_WeightBold, fontAsset.boldStyleWeight);
			fontAsset.material = material2;
		}
		fontAsset.freeGlyphRects = new List<GlyphRect>(8)
		{
			new GlyphRect(0, 0, atlasWidth - num, atlasHeight - num)
		};
		fontAsset.usedGlyphRects = new List<GlyphRect>(8);
		fontAsset.ReadFontAssetDefinition();
		return fontAsset;
	}

	private void RegisterCallbackInstance(FontAsset instance)
	{
		for (int i = 0; i < s_CallbackInstances.Count; i++)
		{
			if (s_CallbackInstances[i].TryGetTarget(out var target) && target == instance)
			{
				return;
			}
		}
		for (int j = 0; j < s_CallbackInstances.Count; j++)
		{
			if (!s_CallbackInstances[j].TryGetTarget(out var _))
			{
				s_CallbackInstances[j] = new WeakReference<FontAsset>(instance);
				return;
			}
		}
		s_CallbackInstances.Add(new WeakReference<FontAsset>(this));
	}

	internal override void OnDestroy()
	{
		base.OnDestroy();
		if (!m_IsClone)
		{
			DestroyAtlasTextures();
			if ((bool)m_Material)
			{
				Object.Destroy(m_Material);
			}
			m_Material = null;
		}
		if (m_NativeFontAsset != IntPtr.Zero)
		{
			Destroy(m_NativeFontAsset, MarshalledUnityObject.MarshalNotNull(this));
			m_NativeFontAsset = IntPtr.Zero;
		}
	}

	public void ReadFontAssetDefinition()
	{
		InitializeDictionaryLookupTables();
		AddSynthesizedCharactersAndFaceMetrics();
		if (m_FaceInfo.capLine == 0f && m_CharacterLookupDictionary.TryGetValue(88u, out var value))
		{
			uint glyphIndex = value.glyphIndex;
			m_FaceInfo.capLine = m_GlyphLookupDictionary[glyphIndex].metrics.horizontalBearingY;
		}
		if (m_FaceInfo.meanLine == 0f && m_CharacterLookupDictionary.TryGetValue(88u, out value))
		{
			uint glyphIndex2 = value.glyphIndex;
			m_FaceInfo.meanLine = m_GlyphLookupDictionary[glyphIndex2].metrics.horizontalBearingY;
		}
		if (m_FaceInfo.scale == 0f)
		{
			m_FaceInfo.scale = 1f;
		}
		if (m_FaceInfo.strikethroughOffset == 0f)
		{
			m_FaceInfo.strikethroughOffset = m_FaceInfo.capLine / 2.5f;
		}
		if (m_AtlasPadding == 0 && base.material.HasProperty(TextShaderUtilities.ID_GradientScale))
		{
			m_AtlasPadding = (int)base.material.GetFloat(TextShaderUtilities.ID_GradientScale) - 1;
		}
		if (m_FaceInfo.unitsPerEM == 0 && atlasPopulationMode != AtlasPopulationMode.Static)
		{
			if (!JobsUtility.IsExecutingJob)
			{
				m_FaceInfo.unitsPerEM = FontEngine.GetFaceInfo().unitsPerEM;
				Debug.Log("Font Asset [" + base.name + "] Units Per EM set to " + m_FaceInfo.unitsPerEM + ". Please commit the newly serialized value.", this);
			}
			else
			{
				Debug.LogError("Font Asset [" + base.name + "] is missing Units Per EM. Please select the 'Reset FaceInfo' menu item on Font Asset [" + base.name + "] to ensure proper serialization.", this);
			}
		}
		base.hashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name);
		familyNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(m_FaceInfo.familyName);
		styleNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(m_FaceInfo.styleName);
		base.materialHashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name + s_DefaultMaterialSuffix);
		TextResourceManager.AddFontAsset(this);
		IsFontAssetLookupTablesDirty = false;
		RegisterCallbackInstance(this);
	}

	internal void InitializeDictionaryLookupTables()
	{
		InitializeGlyphLookupDictionary();
		InitializeCharacterLookupDictionary();
		if ((m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS) && m_ShouldReimportFontFeatures)
		{
			ImportFontFeatures();
		}
		InitializeLigatureSubstitutionLookupDictionary();
		InitializeGlyphPairAdjustmentRecordsLookupDictionary();
		InitializeMarkToBaseAdjustmentRecordsLookupDictionary();
		InitializeMarkToMarkAdjustmentRecordsLookupDictionary();
	}

	private static void InitializeLookup<T>(ICollection source, ref Dictionary<uint, T> lookup, int defaultCapacity = 16)
	{
		int capacity = source?.Count ?? defaultCapacity;
		if (lookup == null)
		{
			lookup = new Dictionary<uint, T>(capacity);
			return;
		}
		lookup.Clear();
		lookup.EnsureCapacity(capacity);
	}

	private static void InitializeList<T>(ICollection source, ref List<T> list, int defaultCapacity = 16)
	{
		int capacity = source?.Count ?? defaultCapacity;
		if (list == null)
		{
			list = new List<T>(capacity);
			return;
		}
		list.Clear();
		list.Capacity = capacity;
	}

	internal void InitializeGlyphLookupDictionary()
	{
		InitializeLookup(m_GlyphTable, ref m_GlyphLookupDictionary);
		InitializeList(m_GlyphTable, ref m_GlyphIndexList);
		InitializeList(null, ref m_GlyphIndexListNewlyAdded);
		foreach (Glyph item in m_GlyphTable)
		{
			uint index = item.index;
			if (m_GlyphLookupDictionary.TryAdd(index, item))
			{
				m_GlyphIndexList.Add(index);
			}
		}
	}

	internal void InitializeCharacterLookupDictionary()
	{
		InitializeLookup(m_CharacterTable, ref m_CharacterLookupDictionary);
		foreach (Character item in m_CharacterTable)
		{
			uint unicode = item.unicode;
			uint glyphIndex = item.glyphIndex;
			if (m_CharacterLookupDictionary.TryAdd(unicode, item))
			{
				item.textAsset = this;
				item.glyph = m_GlyphLookupDictionary[glyphIndex];
			}
		}
		m_MissingUnicodesFromFontFile?.Clear();
	}

	internal void ClearFallbackCharacterTable()
	{
		List<uint> list = new List<uint>();
		foreach (KeyValuePair<uint, Character> item in m_CharacterLookupDictionary)
		{
			Character value = item.Value;
			if (value.textAsset != this)
			{
				list.Add(item.Key);
			}
		}
		foreach (uint item2 in list)
		{
			m_CharacterLookupDictionary.Remove(item2);
		}
	}

	internal void InitializeLigatureSubstitutionLookupDictionary()
	{
		List<LigatureSubstitutionRecord> ligatureSubstitutionRecords = m_FontFeatureTable.m_LigatureSubstitutionRecords;
		InitializeLookup(ligatureSubstitutionRecords, ref m_FontFeatureTable.m_LigatureSubstitutionRecordLookup);
		if (ligatureSubstitutionRecords == null)
		{
			return;
		}
		foreach (LigatureSubstitutionRecord item in ligatureSubstitutionRecords)
		{
			if (item.componentGlyphIDs != null && item.componentGlyphIDs.Length != 0)
			{
				uint key = item.componentGlyphIDs[0];
				if (m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.TryGetValue(key, out var value))
				{
					value.Add(item);
					continue;
				}
				m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.Add(key, new List<LigatureSubstitutionRecord> { item });
			}
		}
	}

	internal void InitializeGlyphPairAdjustmentRecordsLookupDictionary()
	{
		List<GlyphPairAdjustmentRecord> glyphPairAdjustmentRecords = m_FontFeatureTable.glyphPairAdjustmentRecords;
		InitializeLookup(glyphPairAdjustmentRecords, ref m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup);
		if (glyphPairAdjustmentRecords == null)
		{
			return;
		}
		foreach (GlyphPairAdjustmentRecord item in glyphPairAdjustmentRecords)
		{
			uint key = (item.secondAdjustmentRecord.glyphIndex << 16) | item.firstAdjustmentRecord.glyphIndex;
			m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryAdd(key, item);
		}
	}

	internal void InitializeMarkToBaseAdjustmentRecordsLookupDictionary()
	{
		List<MarkToBaseAdjustmentRecord> markToBaseAdjustmentRecords = m_FontFeatureTable.m_MarkToBaseAdjustmentRecords;
		InitializeLookup(markToBaseAdjustmentRecords, ref m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup);
		if (markToBaseAdjustmentRecords == null)
		{
			return;
		}
		foreach (MarkToBaseAdjustmentRecord item in markToBaseAdjustmentRecords)
		{
			uint key = (item.markGlyphID << 16) | item.baseGlyphID;
			m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryAdd(key, item);
		}
	}

	internal void InitializeMarkToMarkAdjustmentRecordsLookupDictionary()
	{
		List<MarkToMarkAdjustmentRecord> markToMarkAdjustmentRecords = m_FontFeatureTable.m_MarkToMarkAdjustmentRecords;
		InitializeLookup(markToMarkAdjustmentRecords, ref m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup);
		if (markToMarkAdjustmentRecords == null)
		{
			return;
		}
		foreach (MarkToMarkAdjustmentRecord item in markToMarkAdjustmentRecords)
		{
			uint key = (item.combiningMarkGlyphID << 16) | item.baseMarkGlyphID;
			m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryAdd(key, item);
		}
	}

	internal void AddSynthesizedCharactersAndFaceMetrics()
	{
		bool flag = false;
		if (m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS)
		{
			flag = LoadFontFace() == FontEngineError.Success;
			if (!flag && !InternalDynamicOS)
			{
				Debug.LogWarning("Unable to load font face for [" + base.name + "] font asset.", this);
			}
		}
		AddSynthesizedCharacter(3u, flag, addImmediately: true);
		AddSynthesizedCharacter(9u, flag, addImmediately: true);
		AddSynthesizedCharacter(10u, flag);
		AddSynthesizedCharacter(11u, flag);
		AddSynthesizedCharacter(13u, flag);
		AddSynthesizedCharacter(1564u, flag);
		AddSynthesizedCharacter(8203u, flag);
		AddSynthesizedCharacter(8206u, flag);
		AddSynthesizedCharacter(8207u, flag);
		AddSynthesizedCharacter(8232u, flag);
		AddSynthesizedCharacter(8233u, flag);
		AddSynthesizedCharacter(8288u, flag);
	}

	private void AddSynthesizedCharacter(uint unicode, bool isFontFaceLoaded, bool addImmediately = false)
	{
		if (m_CharacterLookupDictionary.ContainsKey(unicode))
		{
			return;
		}
		Glyph glyph;
		Character value;
		if (isFontFaceLoaded && FontEngine.GetGlyphIndex(unicode) != 0)
		{
			if (!addImmediately)
			{
				return;
			}
			GlyphLoadFlags flags = (((m_AtlasRenderMode & (GlyphRenderMode)4) == (GlyphRenderMode)4) ? (GlyphLoadFlags.LOAD_NO_HINTING | GlyphLoadFlags.LOAD_NO_BITMAP) : GlyphLoadFlags.LOAD_NO_BITMAP);
			if (!FontEngine.TryGetGlyphWithUnicodeValue(unicode, flags, out glyph))
			{
				return;
			}
			value = new Character(unicode, this, glyph);
			{
				foreach (TextFontWeight value2 in Enum.GetValues(typeof(TextFontWeight)))
				{
					m_CharacterLookupDictionary.Add(CreateCompositeKey(unicode, FontStyles.Normal, value2), value);
					m_CharacterLookupDictionary.Add(CreateCompositeKey(unicode, FontStyles.Italic, value2), value);
				}
				return;
			}
		}
		glyph = new Glyph(0u, new GlyphMetrics(0f, 0f, 0f, 0f, 0f), GlyphRect.zero, 1f, 0);
		value = new Character(unicode, this, glyph);
		foreach (TextFontWeight value3 in Enum.GetValues(typeof(TextFontWeight)))
		{
			m_CharacterLookupDictionary.Add(CreateCompositeKey(unicode, FontStyles.Normal, value3), value);
			m_CharacterLookupDictionary.Add(CreateCompositeKey(unicode, FontStyles.Italic, value3), value);
		}
	}

	internal void AddCharacterToLookupCache(uint unicode, Character character)
	{
		AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
	}

	internal void AddCharacterToLookupCache(uint unicode, Character character, FontStyles fontStyle, TextFontWeight fontWeight)
	{
		if (m_CharacterLookupDictionary == null)
		{
			ReadFontAssetDefinition();
		}
		m_CharacterLookupDictionary.TryAdd(CreateCompositeKey(unicode, fontStyle, fontWeight), character);
	}

	internal bool GetCharacterInLookupCache(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight, out Character character)
	{
		if (m_CharacterLookupDictionary == null)
		{
			ReadFontAssetDefinition();
		}
		return m_CharacterLookupDictionary.TryGetValue(CreateCompositeKey(unicode, fontStyle, fontWeight), out character);
	}

	internal void RemoveCharacterInLookupCache(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight)
	{
		if (m_CharacterLookupDictionary == null)
		{
			ReadFontAssetDefinition();
		}
		m_CharacterLookupDictionary.Remove(CreateCompositeKey(unicode, fontStyle, fontWeight));
	}

	internal bool ContainsCharacterInLookupCache(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight)
	{
		if (m_CharacterLookupDictionary == null)
		{
			ReadFontAssetDefinition();
		}
		return m_CharacterLookupDictionary.ContainsKey(CreateCompositeKey(unicode, fontStyle, fontWeight));
	}

	private uint CreateCompositeKey(uint unicode, FontStyles fontStyle = FontStyles.Normal, TextFontWeight fontWeight = TextFontWeight.Regular)
	{
		if (fontStyle == FontStyles.Normal && fontWeight == TextFontWeight.Regular)
		{
			return unicode;
		}
		bool flag = (fontStyle & FontStyles.Italic) == FontStyles.Italic;
		int num = 0;
		if (fontWeight != TextFontWeight.Regular)
		{
			num = TextUtilities.GetTextFontWeightIndex(fontWeight);
		}
		uint num2 = unicode & 0x1FFFFF;
		uint num3 = (uint)((num & 0xF) << 21);
		uint num4 = (flag ? 33554432u : 0u);
		return num2 | num3 | num4;
	}

	internal FontEngineError LoadFontFace()
	{
		if (m_AtlasPopulationMode == AtlasPopulationMode.Dynamic)
		{
			if (FontEngine.LoadFontFace(m_SourceFontFile, m_FaceInfo.pointSize, m_FaceInfo.faceIndex) == FontEngineError.Success)
			{
				return FontEngineError.Success;
			}
			if (!string.IsNullOrEmpty(m_SourceFontFilePath))
			{
				return FontEngine.LoadFontFace(m_SourceFontFilePath, m_FaceInfo.pointSize, m_FaceInfo.faceIndex);
			}
			return FontEngineError.Invalid_Face;
		}
		return FontEngine.LoadFontFace(m_FaceInfo.familyName, m_FaceInfo.styleName, m_FaceInfo.pointSize);
	}

	internal void SortCharacterTable()
	{
		if (m_CharacterTable != null && m_CharacterTable.Count > 0)
		{
			m_CharacterTable = m_CharacterTable.OrderBy((Character c) => c.unicode).ToList();
		}
	}

	internal void SortGlyphTable()
	{
		if (m_GlyphTable != null && m_GlyphTable.Count > 0)
		{
			m_GlyphTable = m_GlyphTable.OrderBy((Glyph c) => c.index).ToList();
		}
	}

	internal void SortFontFeatureTable()
	{
		m_FontFeatureTable.SortGlyphPairAdjustmentRecords();
		m_FontFeatureTable.SortMarkToBaseAdjustmentRecords();
		m_FontFeatureTable.SortMarkToMarkAdjustmentRecords();
	}

	internal void SortAllTables()
	{
		SortGlyphTable();
		SortCharacterTable();
		SortFontFeatureTable();
	}

	public bool HasCharacter(int character)
	{
		if (characterLookupTable == null)
		{
			return false;
		}
		return m_CharacterLookupDictionary.ContainsKey((uint)character);
	}

	public bool HasCharacter(char character, bool searchFallbacks = false, bool tryAddCharacter = false)
	{
		return HasCharacter((uint)character, searchFallbacks, tryAddCharacter);
	}

	public bool HasCharacter(uint character, bool searchFallbacks = false, bool tryAddCharacter = false)
	{
		if (characterLookupTable == null)
		{
			return false;
		}
		if (m_CharacterLookupDictionary.ContainsKey(character))
		{
			return true;
		}
		if (tryAddCharacter && (m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS) && TryAddCharacterInternal(character, FontStyles.Normal, TextFontWeight.Regular, out var _))
		{
			return true;
		}
		if (searchFallbacks)
		{
			if (k_SearchedFontAssetLookup == null)
			{
				k_SearchedFontAssetLookup = new HashSet<int>();
			}
			else
			{
				k_SearchedFontAssetLookup.Clear();
			}
			k_SearchedFontAssetLookup.Add(GetInstanceID());
			if (fallbackFontAssetTable != null && fallbackFontAssetTable.Count > 0)
			{
				for (int i = 0; i < fallbackFontAssetTable.Count && fallbackFontAssetTable[i] != null; i++)
				{
					FontAsset fontAsset = fallbackFontAssetTable[i];
					int item = fontAsset.GetInstanceID();
					if (k_SearchedFontAssetLookup.Add(item) && fontAsset.HasCharacter_Internal(character, FontStyles.Normal, TextFontWeight.Regular, searchFallbacks: true, tryAddCharacter))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool HasCharacterWithStyle_Internal(uint character, FontStyles fontStyle, TextFontWeight fontWeight, bool searchFallbacks = false, bool tryAddCharacter = false)
	{
		return HasCharacter_Internal(character, fontStyle, fontWeight, searchFallbacks, tryAddCharacter);
	}

	private bool HasCharacter_Internal(uint character, FontStyles fontStyle = FontStyles.Normal, TextFontWeight fontWeight = TextFontWeight.Regular, bool searchFallbacks = false, bool tryAddCharacter = false)
	{
		if (m_CharacterLookupDictionary == null)
		{
			ReadFontAssetDefinition();
			if (m_CharacterLookupDictionary == null)
			{
				return false;
			}
		}
		if (ContainsCharacterInLookupCache(character, fontStyle, fontWeight))
		{
			return true;
		}
		if (tryAddCharacter && (atlasPopulationMode == AtlasPopulationMode.Dynamic || m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS) && TryAddCharacterInternal(character, fontStyle, fontWeight, out var _))
		{
			return true;
		}
		if (searchFallbacks)
		{
			if (fallbackFontAssetTable == null || fallbackFontAssetTable.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < fallbackFontAssetTable.Count && fallbackFontAssetTable[i] != null; i++)
			{
				FontAsset fontAsset = fallbackFontAssetTable[i];
				int item = fontAsset.GetInstanceID();
				if (k_SearchedFontAssetLookup.Add(item) && fontAsset.HasCharacter_Internal(character, fontStyle, fontWeight, searchFallbacks: true, tryAddCharacter))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool HasCharacters(string text, out List<char> missingCharacters)
	{
		if (characterLookupTable == null)
		{
			missingCharacters = null;
			return false;
		}
		missingCharacters = new List<char>();
		for (int i = 0; i < text.Length; i++)
		{
			uint codePoint = FontAssetUtilities.GetCodePoint(text, ref i);
			if (!m_CharacterLookupDictionary.ContainsKey(codePoint))
			{
				missingCharacters.Add((char)codePoint);
			}
		}
		if (missingCharacters.Count == 0)
		{
			return true;
		}
		return false;
	}

	public bool HasCharacters(string text, out uint[] missingCharacters, bool searchFallbacks = false, bool tryAddCharacter = false)
	{
		missingCharacters = null;
		if (characterLookupTable == null)
		{
			return false;
		}
		s_MissingCharacterList.Clear();
		for (int i = 0; i < text.Length; i++)
		{
			bool flag = true;
			uint codePoint = FontAssetUtilities.GetCodePoint(text, ref i);
			if (m_CharacterLookupDictionary.ContainsKey(codePoint) || (tryAddCharacter && (atlasPopulationMode == AtlasPopulationMode.Dynamic || m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS) && TryAddCharacterInternal(codePoint, FontStyles.Normal, TextFontWeight.Regular, out var _)))
			{
				continue;
			}
			if (searchFallbacks)
			{
				if (k_SearchedFontAssetLookup == null)
				{
					k_SearchedFontAssetLookup = new HashSet<int>();
				}
				else
				{
					k_SearchedFontAssetLookup.Clear();
				}
				k_SearchedFontAssetLookup.Add(GetInstanceID());
				if (fallbackFontAssetTable != null && fallbackFontAssetTable.Count > 0)
				{
					for (int j = 0; j < fallbackFontAssetTable.Count && fallbackFontAssetTable[j] != null; j++)
					{
						FontAsset fontAsset = fallbackFontAssetTable[j];
						int item = fontAsset.GetInstanceID();
						if (k_SearchedFontAssetLookup.Add(item) && fontAsset.HasCharacter_Internal(codePoint, FontStyles.Normal, TextFontWeight.Regular, searchFallbacks: true, tryAddCharacter))
						{
							flag = false;
							break;
						}
					}
				}
			}
			if (flag)
			{
				s_MissingCharacterList.Add(codePoint);
			}
		}
		if (s_MissingCharacterList.Count > 0)
		{
			missingCharacters = s_MissingCharacterList.ToArray();
			return false;
		}
		return true;
	}

	public bool HasCharacters(string text)
	{
		if (characterLookupTable == null)
		{
			return false;
		}
		for (int i = 0; i < text.Length; i++)
		{
			uint codePoint = FontAssetUtilities.GetCodePoint(text, ref i);
			if (!m_CharacterLookupDictionary.ContainsKey(codePoint))
			{
				return false;
			}
		}
		return true;
	}

	public static string GetCharacters(FontAsset fontAsset)
	{
		string text = string.Empty;
		for (int i = 0; i < fontAsset.characterTable.Count; i++)
		{
			text += (char)fontAsset.characterTable[i].unicode;
		}
		return text;
	}

	public static int[] GetCharactersArray(FontAsset fontAsset)
	{
		int[] array = new int[fontAsset.characterTable.Count];
		for (int i = 0; i < fontAsset.characterTable.Count; i++)
		{
			array[i] = (int)fontAsset.characterTable[i].unicode;
		}
		return array;
	}

	internal uint GetGlyphIndex(uint unicode)
	{
		bool success;
		return GetGlyphIndex(unicode, out success);
	}

	internal Glyph GetGlyphInCache(uint glyphID)
	{
		if (m_GlyphLookupDictionary == null)
		{
			return null;
		}
		if (!glyphLookupTable.TryGetValue(glyphID, out var value))
		{
			return null;
		}
		return value;
	}

	internal uint GetGlyphIndex(uint unicode, out bool success)
	{
		success = true;
		if (characterLookupTable.TryGetValue(unicode, out var value))
		{
			return value.glyphIndex;
		}
		if (TextGenerator.IsExecutingJob)
		{
			success = false;
			return 0u;
		}
		return (LoadFontFace() == FontEngineError.Success) ? FontEngine.GetGlyphIndex(unicode) : 0u;
	}

	internal uint GetGlyphVariantIndex(uint unicode, uint variantSelectorUnicode)
	{
		return (LoadFontFace() == FontEngineError.Success) ? FontEngine.GetVariantGlyphIndex(unicode, variantSelectorUnicode) : 0u;
	}

	internal void UpdateFontAssetData()
	{
		uint[] array = new uint[m_CharacterTable.Count];
		for (int i = 0; i < m_CharacterTable.Count; i++)
		{
			array[i] = m_CharacterTable[i].unicode;
		}
		ClearCharacterAndGlyphTables();
		ClearFontFeaturesTables();
		ClearAtlasTextures(setAtlasSizeToZero: true);
		ReadFontAssetDefinition();
		if (array.Length != 0)
		{
			TryAddCharacters(array, m_GetFontFeatures);
		}
	}

	public void ClearFontAssetData(bool setAtlasSizeToZero = false)
	{
		using (k_ClearFontAssetDataMarker.Auto())
		{
			ClearCharacterAndGlyphTables();
			ClearFontFeaturesTables();
			ClearAtlasTextures(setAtlasSizeToZero);
			ReadFontAssetDefinition();
			for (int i = 0; i < s_CallbackInstances.Count; i++)
			{
				if (s_CallbackInstances[i].TryGetTarget(out var target) && target != this)
				{
					target.ClearFallbackCharacterTable();
				}
			}
			TextEventManager.ON_FONT_PROPERTY_CHANGED(isChanged: true, this);
		}
	}

	internal void ClearCharacterAndGlyphTablesInternal()
	{
		ClearCharacterAndGlyphTables();
		ClearAtlasTextures(setAtlasSizeToZero: true);
		ReadFontAssetDefinition();
	}

	private void ClearCharacterAndGlyphTables()
	{
		if (m_GlyphTable != null)
		{
			m_GlyphTable.Clear();
		}
		if (m_CharacterTable != null)
		{
			m_CharacterTable.Clear();
		}
		if (m_UsedGlyphRects != null)
		{
			m_UsedGlyphRects.Clear();
		}
		if (m_FreeGlyphRects != null)
		{
			int num = (((m_AtlasRenderMode & (GlyphRenderMode)16) != (GlyphRenderMode)16) ? 1 : 0);
			m_FreeGlyphRects.Clear();
			m_FreeGlyphRects.Add(new GlyphRect(0, 0, m_AtlasWidth - num, m_AtlasHeight - num));
		}
		if (m_GlyphsToRender != null)
		{
			m_GlyphsToRender.Clear();
		}
		if (m_GlyphsRendered != null)
		{
			m_GlyphsRendered.Clear();
		}
	}

	private void ClearFontFeaturesTables()
	{
		if (m_FontFeatureTable != null && m_FontFeatureTable.m_LigatureSubstitutionRecords != null)
		{
			m_FontFeatureTable.m_LigatureSubstitutionRecords.Clear();
		}
		if (m_FontFeatureTable != null && m_FontFeatureTable.glyphPairAdjustmentRecords != null)
		{
			m_FontFeatureTable.glyphPairAdjustmentRecords.Clear();
		}
		if (m_FontFeatureTable != null && m_FontFeatureTable.m_MarkToBaseAdjustmentRecords != null)
		{
			m_FontFeatureTable.m_MarkToBaseAdjustmentRecords.Clear();
		}
		if (m_FontFeatureTable != null && m_FontFeatureTable.m_MarkToMarkAdjustmentRecords != null)
		{
			m_FontFeatureTable.m_MarkToMarkAdjustmentRecords.Clear();
		}
	}

	internal void ClearAtlasTextures(bool setAtlasSizeToZero = false)
	{
		m_AtlasTextureIndex = 0;
		if (m_AtlasTextures == null)
		{
			return;
		}
		Texture2D texture2D = null;
		for (int i = 1; i < m_AtlasTextures.Length; i++)
		{
			texture2D = m_AtlasTextures[i];
			if ((bool)texture2D)
			{
				Object.Destroy(texture2D);
			}
		}
		Array.Resize(ref m_AtlasTextures, 1);
		texture2D = (m_AtlasTexture = m_AtlasTextures[0]);
		if (!texture2D.isReadable)
		{
		}
		TextureFormat format = (((m_AtlasRenderMode & (GlyphRenderMode)65536) != (GlyphRenderMode)65536) ? TextureFormat.Alpha8 : TextureFormat.RGBA32);
		if (setAtlasSizeToZero)
		{
			texture2D.Reinitialize(1, 1, format, hasMipMap: false);
		}
		else if (texture2D.width != m_AtlasWidth || texture2D.height != m_AtlasHeight)
		{
			texture2D.Reinitialize(m_AtlasWidth, m_AtlasHeight, format, hasMipMap: false);
		}
		FontEngine.ResetAtlasTexture(texture2D);
		texture2D.Apply();
	}

	private void DestroyAtlasTextures()
	{
		m_AtlasTexture = null;
		m_AtlasTextureIndex = -1;
		if (m_AtlasTextures == null)
		{
			return;
		}
		Texture2D[] array = m_AtlasTextures;
		foreach (Texture2D texture2D in array)
		{
			if (texture2D != null)
			{
				Object.Destroy(texture2D);
			}
		}
		m_AtlasTextures = null;
	}

	internal static void RegisterFontAssetForFontFeatureUpdate(FontAsset fontAsset)
	{
		int item = fontAsset.instanceID;
		if (k_FontAssets_FontFeaturesUpdateQueueLookup.Add(item))
		{
			k_FontAssets_FontFeaturesUpdateQueue.Add(fontAsset);
		}
	}

	internal static void RegisterFontAssetForKerningUpdate(FontAsset fontAsset)
	{
		int item = fontAsset.instanceID;
		if (k_FontAssets_KerningUpdateQueueLookup.Add(item))
		{
			k_FontAssets_KerningUpdateQueue.Add(fontAsset);
		}
	}

	internal static void UpdateFontFeaturesForFontAssetsInQueue()
	{
		int count = k_FontAssets_FontFeaturesUpdateQueue.Count;
		for (int i = 0; i < count; i++)
		{
			k_FontAssets_FontFeaturesUpdateQueue[i].UpdateGPOSFontFeaturesForNewlyAddedGlyphs();
		}
		if (count > 0)
		{
			k_FontAssets_FontFeaturesUpdateQueue.Clear();
			k_FontAssets_FontFeaturesUpdateQueueLookup.Clear();
		}
		count = k_FontAssets_KerningUpdateQueue.Count;
		for (int j = 0; j < count; j++)
		{
			k_FontAssets_KerningUpdateQueue[j].UpdateGlyphAdjustmentRecordsForNewGlyphs();
		}
		if (count > 0)
		{
			k_FontAssets_KerningUpdateQueue.Clear();
			k_FontAssets_KerningUpdateQueueLookup.Clear();
		}
	}

	internal static void RegisterAtlasTextureForApply(Texture2D texture)
	{
		int item = texture.GetInstanceID();
		if (k_FontAssets_AtlasTexturesUpdateQueueLookup.Add(item))
		{
			k_FontAssets_AtlasTexturesUpdateQueue.Add(texture);
		}
	}

	internal static void UpdateAtlasTexturesInQueue()
	{
		int count = k_FontAssets_AtlasTexturesUpdateQueueLookup.Count;
		for (int i = 0; i < count; i++)
		{
			k_FontAssets_AtlasTexturesUpdateQueue[i].Apply(updateMipmaps: false, makeNoLongerReadable: false);
		}
		if (count > 0)
		{
			k_FontAssets_AtlasTexturesUpdateQueue.Clear();
			k_FontAssets_AtlasTexturesUpdateQueueLookup.Clear();
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal static void UpdateFontAssetsInUpdateQueue()
	{
		UpdateAtlasTexturesInQueue();
		UpdateFontFeaturesForFontAssetsInQueue();
	}

	public bool TryAddCharacters(uint[] unicodes, bool includeFontFeatures = false)
	{
		uint[] missingUnicodes;
		return TryAddCharacters(unicodes, out missingUnicodes, includeFontFeatures);
	}

	public bool TryAddCharacters(uint[] unicodes, out uint[] missingUnicodes, bool includeFontFeatures = false)
	{
		using (k_TryAddCharactersMarker.Auto())
		{
			if (unicodes == null || unicodes.Length == 0 || m_AtlasPopulationMode == AtlasPopulationMode.Static)
			{
				if (m_AtlasPopulationMode == AtlasPopulationMode.Static)
				{
					Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because its AtlasPopulationMode is set to Static.", this);
				}
				else
				{
					Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because the provided Unicode list is Null or Empty.", this);
				}
				missingUnicodes = null;
				return false;
			}
			if (LoadFontFace() != FontEngineError.Success)
			{
				missingUnicodes = new uint[unicodes.Length];
				int num = 0;
				foreach (uint num2 in unicodes)
				{
					missingUnicodes[num++] = num2;
				}
				return false;
			}
			if (m_CharacterLookupDictionary == null || m_GlyphLookupDictionary == null)
			{
				ReadFontAssetDefinition();
			}
			Dictionary<uint, Character> characterLookupDictionary = m_CharacterLookupDictionary;
			Dictionary<uint, Glyph> glyphLookupDictionary = m_GlyphLookupDictionary;
			m_GlyphsToAdd.Clear();
			m_GlyphsToAddLookup.Clear();
			m_CharactersToAdd.Clear();
			m_CharactersToAddLookup.Clear();
			s_MissingCharacterList.Clear();
			bool flag = false;
			int num3 = unicodes.Length;
			for (int j = 0; j < num3; j++)
			{
				uint codePoint = FontAssetUtilities.GetCodePoint(unicodes, ref j);
				if (characterLookupDictionary.ContainsKey(codePoint))
				{
					continue;
				}
				uint glyphIndex = FontEngine.GetGlyphIndex(codePoint);
				if (glyphIndex == 0)
				{
					switch (codePoint)
					{
					case 160u:
						glyphIndex = FontEngine.GetGlyphIndex(32u);
						break;
					case 173u:
					case 8209u:
						glyphIndex = FontEngine.GetGlyphIndex(45u);
						break;
					}
					if (glyphIndex == 0)
					{
						s_MissingCharacterList.Add(codePoint);
						flag = true;
						continue;
					}
				}
				Character character = new Character(codePoint, glyphIndex);
				if (glyphLookupDictionary.TryGetValue(glyphIndex, out var value))
				{
					character.glyph = value;
					character.textAsset = this;
					m_CharacterTable.Add(character);
					characterLookupDictionary.Add(codePoint, character);
					continue;
				}
				if (m_GlyphsToAddLookup.Add(glyphIndex))
				{
					m_GlyphsToAdd.Add(glyphIndex);
				}
				if (m_CharactersToAddLookup.Add(codePoint))
				{
					m_CharactersToAdd.Add(character);
				}
			}
			if (m_GlyphsToAdd.Count == 0)
			{
				missingUnicodes = unicodes;
				return !flag;
			}
			if (m_AtlasTextures[m_AtlasTextureIndex].width <= 1 || m_AtlasTextures[m_AtlasTextureIndex].height <= 1)
			{
				m_AtlasTextures[m_AtlasTextureIndex].Reinitialize(m_AtlasWidth, m_AtlasHeight);
				FontEngine.ResetAtlasTexture(m_AtlasTextures[m_AtlasTextureIndex]);
			}
			Glyph[] glyphs;
			bool flag2 = FontEngine.TryAddGlyphsToTexture(m_GlyphsToAdd, m_AtlasPadding, GlyphPackingMode.BestShortSideFit, m_FreeGlyphRects, m_UsedGlyphRects, m_AtlasRenderMode, m_AtlasTextures[m_AtlasTextureIndex], out glyphs);
			int additionalCapacity = glyphs.Length;
			EnsureAdditionalCapacity(m_GlyphTable, additionalCapacity);
			EnsureAdditionalCapacity(glyphLookupDictionary, additionalCapacity);
			EnsureAdditionalCapacity(m_GlyphIndexListNewlyAdded, additionalCapacity);
			EnsureAdditionalCapacity(m_GlyphIndexList, additionalCapacity);
			for (int k = 0; k < glyphs.Length && glyphs[k] != null; k++)
			{
				Glyph glyph = glyphs[k];
				uint index = glyph.index;
				glyph.atlasIndex = m_AtlasTextureIndex;
				m_GlyphTable.Add(glyph);
				glyphLookupDictionary.Add(index, glyph);
				m_GlyphIndexListNewlyAdded.Add(index);
				m_GlyphIndexList.Add(index);
			}
			m_GlyphsToAdd.Clear();
			int count = m_CharactersToAdd.Count;
			EnsureAdditionalCapacity(m_GlyphsToAdd, count);
			EnsureAdditionalCapacity(m_CharacterTable, count);
			EnsureAdditionalCapacity(characterLookupDictionary, count);
			for (int l = 0; l < m_CharactersToAdd.Count; l++)
			{
				Character character2 = m_CharactersToAdd[l];
				if (!glyphLookupDictionary.TryGetValue(character2.glyphIndex, out var value2))
				{
					m_GlyphsToAdd.Add(character2.glyphIndex);
					continue;
				}
				character2.glyph = value2;
				character2.textAsset = this;
				m_CharacterTable.Add(character2);
				characterLookupDictionary.Add(character2.unicode, character2);
				m_CharactersToAdd.RemoveAt(l);
				l--;
			}
			if (m_IsMultiAtlasTexturesEnabled && !flag2)
			{
				while (!flag2)
				{
					flag2 = TryAddGlyphsToNewAtlasTexture();
				}
			}
			else if (!flag2)
			{
				Debug.Log("Atlas is full, consider enabling multi-atlas textures in the Font Asset: " + base.name);
			}
			if (includeFontFeatures)
			{
				UpdateFontFeaturesForNewlyAddedGlyphs();
			}
			foreach (Character item in m_CharactersToAdd)
			{
				s_MissingCharacterList.Add(item.unicode);
			}
			missingUnicodes = null;
			if (s_MissingCharacterList.Count > 0)
			{
				missingUnicodes = s_MissingCharacterList.ToArray();
			}
			return flag2 && !flag;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal bool TryAddGlyphs(List<uint> glyphsToAdd)
	{
		if (LoadFontFace() != FontEngineError.Success)
		{
			return false;
		}
		if (m_CharacterLookupDictionary == null || m_GlyphLookupDictionary == null)
		{
			ReadFontAssetDefinition();
			glyphsToAdd.RemoveAll((uint glyphId) => m_GlyphLookupDictionary.ContainsKey(glyphId));
			if (glyphsToAdd.Count == 0)
			{
				return true;
			}
		}
		if (m_AtlasTextures[m_AtlasTextureIndex].width <= 1 || m_AtlasTextures[m_AtlasTextureIndex].height <= 1)
		{
			m_AtlasTextures[m_AtlasTextureIndex].Reinitialize(m_AtlasWidth, m_AtlasHeight);
			FontEngine.ResetAtlasTexture(m_AtlasTextures[m_AtlasTextureIndex]);
		}
		bool flag = false;
		while (!flag)
		{
			flag = FontEngine.TryAddGlyphsToTexture(glyphsToAdd, m_AtlasPadding, GlyphPackingMode.BestShortSideFit, m_FreeGlyphRects, m_UsedGlyphRects, m_AtlasRenderMode, m_AtlasTextures[m_AtlasTextureIndex], out var glyphs);
			int additionalCapacity = glyphs.Length;
			EnsureAdditionalCapacity(m_GlyphTable, additionalCapacity);
			EnsureAdditionalCapacity(m_GlyphLookupDictionary, additionalCapacity);
			EnsureAdditionalCapacity(m_GlyphIndexListNewlyAdded, additionalCapacity);
			EnsureAdditionalCapacity(m_GlyphIndexList, additionalCapacity);
			HashSet<uint> successfullyAddedGlyphIndices = new HashSet<uint>();
			for (int num = 0; num < glyphs.Length && glyphs[num] != null; num++)
			{
				Glyph glyph = glyphs[num];
				uint index = glyph.index;
				glyph.atlasIndex = m_AtlasTextureIndex;
				m_GlyphTable.Add(glyph);
				m_GlyphLookupDictionary.Add(index, glyph);
				m_GlyphIndexListNewlyAdded.Add(index);
				m_GlyphIndexList.Add(index);
				successfullyAddedGlyphIndices.Add(index);
			}
			if (successfullyAddedGlyphIndices.Count > 0)
			{
				glyphsToAdd.RemoveAll((uint id) => successfullyAddedGlyphIndices.Contains(id));
			}
			RegisterAtlasTextureForApply(m_AtlasTextures[m_AtlasTextureIndex]);
			if (!m_IsMultiAtlasTexturesEnabled && !flag)
			{
				Debug.Log("Atlas is full, consider enabling multi-atlas textures in the Font Asset: " + base.name);
				break;
			}
			if (!flag)
			{
				SetupNewAtlasTexture();
			}
		}
		if (m_GetFontFeatures && m_GlyphIndexListNewlyAdded.Count > 0)
		{
			RegisterFontAssetForKerningUpdate(this);
		}
		FontEngine.SetTextureUploadMode(shouldUploadImmediately: true);
		return flag;
	}

	public bool TryAddCharacters(string characters, bool includeFontFeatures = false)
	{
		string missingCharacters;
		return TryAddCharacters(characters, out missingCharacters, includeFontFeatures);
	}

	public bool TryAddCharacters(string characters, out string missingCharacters, bool includeFontFeatures = false)
	{
		uint[] array = new uint[characters.Length];
		for (int i = 0; i < characters.Length; i++)
		{
			array[i] = characters[i];
		}
		uint[] missingUnicodes;
		bool result = TryAddCharacters(array, out missingUnicodes, includeFontFeatures);
		if (missingUnicodes == null || missingUnicodes.Length == 0)
		{
			missingCharacters = null;
			return result;
		}
		StringBuilder stringBuilder = new StringBuilder(missingUnicodes.Length);
		uint[] array2 = missingUnicodes;
		foreach (uint num in array2)
		{
			stringBuilder.Append((char)num);
		}
		missingCharacters = stringBuilder.ToString();
		return result;
	}

	internal bool TryAddGlyphVariantIndexInternal(uint unicode, uint nextCharacter, uint variantGlyphIndex)
	{
		return m_VariantGlyphIndexes.TryAdd((unicode, nextCharacter), variantGlyphIndex);
	}

	internal bool TryGetGlyphVariantIndexInternal(uint unicode, uint nextCharacter, out uint variantGlyphIndex)
	{
		return m_VariantGlyphIndexes.TryGetValue((unicode, nextCharacter), out variantGlyphIndex);
	}

	internal bool TryAddGlyphInternal(uint glyphIndex, out Glyph glyph, bool populateLigatures = true)
	{
		using (k_TryAddGlyphMarker.Auto())
		{
			glyph = null;
			if (glyphLookupTable.TryGetValue(glyphIndex, out glyph))
			{
				return true;
			}
			if (LoadFontFace() != FontEngineError.Success)
			{
				return false;
			}
			return TryAddGlyphToAtlas(glyphIndex, out glyph, populateLigatures);
		}
	}

	internal bool TryAddCharacterInternal(uint unicode, out Character character)
	{
		return TryAddCharacterInternal(unicode, FontStyles.Normal, TextFontWeight.Regular, out character);
	}

	internal bool TryAddCharacterInternal(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight, out Character character, bool populateLigatures = true)
	{
		using (k_TryAddCharacterMarker.Auto())
		{
			character = null;
			if (m_MissingUnicodesFromFontFile.Contains(unicode))
			{
				return false;
			}
			if (LoadFontFace() != FontEngineError.Success)
			{
				return false;
			}
			uint glyphIndex = FontEngine.GetGlyphIndex(unicode);
			if (glyphIndex == 0)
			{
				switch (unicode)
				{
				case 160u:
					glyphIndex = FontEngine.GetGlyphIndex(32u);
					break;
				case 173u:
				case 8209u:
					glyphIndex = FontEngine.GetGlyphIndex(45u);
					break;
				}
				if (glyphIndex == 0)
				{
					m_MissingUnicodesFromFontFile.Add(unicode);
					return false;
				}
			}
			if (glyphLookupTable.ContainsKey(glyphIndex))
			{
				character = CreateCharacterAndAddToCache(unicode, m_GlyphLookupDictionary[glyphIndex], fontStyle, fontWeight);
				return true;
			}
			Glyph glyph = null;
			if (TryAddGlyphToAtlas(glyphIndex, out glyph, populateLigatures))
			{
				character = CreateCharacterAndAddToCache(unicode, glyph, fontStyle, fontWeight);
				return true;
			}
			return false;
		}
	}

	private bool TryAddGlyphToAtlas(uint glyphIndex, out Glyph glyph, bool populateLigatures = true)
	{
		glyph = null;
		if (!m_AtlasTextures[m_AtlasTextureIndex].isReadable)
		{
			Debug.LogWarning("Unable to add the requested glyph to font asset [" + base.name + "]'s atlas texture. Please make the texture [" + m_AtlasTextures[m_AtlasTextureIndex].name + "] readable.", m_AtlasTextures[m_AtlasTextureIndex]);
			return false;
		}
		if (m_AtlasTextures[m_AtlasTextureIndex].width <= 1 || m_AtlasTextures[m_AtlasTextureIndex].height <= 1)
		{
			m_AtlasTextures[m_AtlasTextureIndex].Reinitialize(m_AtlasWidth, m_AtlasHeight);
			FontEngine.ResetAtlasTexture(m_AtlasTextures[m_AtlasTextureIndex]);
		}
		FontEngine.SetTextureUploadMode(shouldUploadImmediately: false);
		if (TryAddGlyphToTexture(glyphIndex, out glyph, populateLigatures))
		{
			return true;
		}
		if (m_IsMultiAtlasTexturesEnabled && m_UsedGlyphRects.Count > 0)
		{
			SetupNewAtlasTexture();
			FontEngine.SetTextureUploadMode(shouldUploadImmediately: false);
			if (TryAddGlyphToTexture(glyphIndex, out glyph, populateLigatures))
			{
				return true;
			}
		}
		else if (m_UsedGlyphRects.Count > 0)
		{
			Debug.Log("Atlas is full, consider enabling multi-atlas textures in the Font Asset: " + base.name);
		}
		return false;
	}

	private bool TryAddGlyphToTexture(uint glyphIndex, out Glyph glyph, bool populateLigatures = true)
	{
		if (FontEngine.TryAddGlyphToTexture(glyphIndex, m_AtlasPadding, GlyphPackingMode.BestShortSideFit, m_FreeGlyphRects, m_UsedGlyphRects, m_AtlasRenderMode, m_AtlasTextures[m_AtlasTextureIndex], out glyph))
		{
			glyph.atlasIndex = m_AtlasTextureIndex;
			m_GlyphTable.Add(glyph);
			m_GlyphLookupDictionary.Add(glyphIndex, glyph);
			m_GlyphIndexList.Add(glyphIndex);
			m_GlyphIndexListNewlyAdded.Add(glyphIndex);
			if (m_GetFontFeatures)
			{
				if (populateLigatures)
				{
					UpdateGSUBFontFeaturesForNewGlyphIndex(glyphIndex);
					RegisterFontAssetForFontFeatureUpdate(this);
				}
				else
				{
					RegisterFontAssetForKerningUpdate(this);
				}
			}
			RegisterAtlasTextureForApply(m_AtlasTextures[m_AtlasTextureIndex]);
			FontEngine.SetTextureUploadMode(shouldUploadImmediately: true);
			return true;
		}
		return false;
	}

	private bool TryAddGlyphsToNewAtlasTexture()
	{
		SetupNewAtlasTexture();
		Glyph[] glyphs;
		bool result = FontEngine.TryAddGlyphsToTexture(m_GlyphsToAdd, m_AtlasPadding, GlyphPackingMode.BestShortSideFit, m_FreeGlyphRects, m_UsedGlyphRects, m_AtlasRenderMode, m_AtlasTextures[m_AtlasTextureIndex], out glyphs);
		for (int i = 0; i < glyphs.Length && glyphs[i] != null; i++)
		{
			Glyph glyph = glyphs[i];
			uint index = glyph.index;
			glyph.atlasIndex = m_AtlasTextureIndex;
			m_GlyphTable.Add(glyph);
			m_GlyphLookupDictionary.Add(index, glyph);
			m_GlyphIndexListNewlyAdded.Add(index);
			m_GlyphIndexList.Add(index);
		}
		m_GlyphsToAdd.Clear();
		for (int j = 0; j < m_CharactersToAdd.Count; j++)
		{
			Character character = m_CharactersToAdd[j];
			if (!m_GlyphLookupDictionary.TryGetValue(character.glyphIndex, out var value))
			{
				m_GlyphsToAdd.Add(character.glyphIndex);
				continue;
			}
			character.glyph = value;
			character.textAsset = this;
			m_CharacterTable.Add(character);
			m_CharacterLookupDictionary.Add(character.unicode, character);
			m_CharactersToAdd.RemoveAt(j);
			j--;
		}
		return result;
	}

	private void SetupNewAtlasTexture()
	{
		m_AtlasTextureIndex++;
		if (m_AtlasTextures.Length == m_AtlasTextureIndex)
		{
			Array.Resize(ref m_AtlasTextures, m_AtlasTextures.Length * 2);
		}
		TextureFormat textureFormat = (((m_AtlasRenderMode & (GlyphRenderMode)65536) != (GlyphRenderMode)65536) ? TextureFormat.Alpha8 : TextureFormat.RGBA32);
		m_AtlasTextures[m_AtlasTextureIndex] = new Texture2D(m_AtlasWidth, m_AtlasHeight, textureFormat, mipChain: false);
		m_AtlasTextures[m_AtlasTextureIndex].hideFlags = m_AtlasTextures[0].hideFlags;
		FontEngine.ResetAtlasTexture(m_AtlasTextures[m_AtlasTextureIndex]);
		int num = (((m_AtlasRenderMode & (GlyphRenderMode)16) != (GlyphRenderMode)16) ? 1 : 0);
		m_FreeGlyphRects.Clear();
		m_FreeGlyphRects.Add(new GlyphRect(0, 0, m_AtlasWidth - num, m_AtlasHeight - num));
		m_UsedGlyphRects.Clear();
	}

	private Character CreateCharacterAndAddToCache(uint unicode, Glyph glyph, FontStyles fontStyle, TextFontWeight fontWeight)
	{
		if (!m_CharacterLookupDictionary.TryGetValue(unicode, out var value))
		{
			value = new Character(unicode, this, glyph);
			m_CharacterTable.Add(value);
			AddCharacterToLookupCache(unicode, value, FontStyles.Normal, TextFontWeight.Regular);
		}
		if (fontStyle != FontStyles.Normal || fontWeight != TextFontWeight.Regular)
		{
			AddCharacterToLookupCache(unicode, value, fontStyle, fontWeight);
		}
		return value;
	}

	private void UpdateFontFeaturesForNewlyAddedGlyphs()
	{
		UpdateLigatureSubstitutionRecords();
		UpdateGlyphAdjustmentRecords();
		UpdateDiacriticalMarkAdjustmentRecords();
		m_GlyphIndexListNewlyAdded.Clear();
	}

	private void UpdateGlyphAdjustmentRecordsForNewGlyphs()
	{
		UpdateGlyphAdjustmentRecords();
		m_GlyphIndexListNewlyAdded.Clear();
	}

	private void UpdateGPOSFontFeaturesForNewlyAddedGlyphs()
	{
		UpdateGlyphAdjustmentRecords();
		UpdateDiacriticalMarkAdjustmentRecords();
		m_GlyphIndexListNewlyAdded.Clear();
	}

	internal void ImportFontFeatures()
	{
		if (LoadFontFace() == FontEngineError.Success)
		{
			GlyphPairAdjustmentRecord[] allPairAdjustmentRecords = FontEngine.GetAllPairAdjustmentRecords();
			if (allPairAdjustmentRecords != null)
			{
				AddPairAdjustmentRecords(allPairAdjustmentRecords);
			}
			MarkToBaseAdjustmentRecord[] allMarkToBaseAdjustmentRecords = FontEngine.GetAllMarkToBaseAdjustmentRecords();
			if (allMarkToBaseAdjustmentRecords != null)
			{
				AddMarkToBaseAdjustmentRecords(allMarkToBaseAdjustmentRecords);
			}
			MarkToMarkAdjustmentRecord[] allMarkToMarkAdjustmentRecords = FontEngine.GetAllMarkToMarkAdjustmentRecords();
			if (allMarkToMarkAdjustmentRecords != null)
			{
				AddMarkToMarkAdjustmentRecords(allMarkToMarkAdjustmentRecords);
			}
			LigatureSubstitutionRecord[] allLigatureSubstitutionRecords = FontEngine.GetAllLigatureSubstitutionRecords();
			if (allLigatureSubstitutionRecords != null)
			{
				AddLigatureSubstitutionRecords(allLigatureSubstitutionRecords);
			}
			m_ShouldReimportFontFeatures = false;
		}
	}

	private void UpdateGSUBFontFeaturesForNewGlyphIndex(uint glyphIndex)
	{
		LigatureSubstitutionRecord[] ligatureSubstitutionRecords = FontEngine.GetLigatureSubstitutionRecords(glyphIndex);
		if (ligatureSubstitutionRecords != null)
		{
			AddLigatureSubstitutionRecords(ligatureSubstitutionRecords);
		}
	}

	internal void UpdateLigatureSubstitutionRecords()
	{
		LigatureSubstitutionRecord[] ligatureSubstitutionRecords = FontEngine.GetLigatureSubstitutionRecords(m_GlyphIndexListNewlyAdded);
		if (ligatureSubstitutionRecords != null)
		{
			AddLigatureSubstitutionRecords(ligatureSubstitutionRecords);
		}
	}

	private void AddLigatureSubstitutionRecords(LigatureSubstitutionRecord[] records)
	{
		Dictionary<uint, List<LigatureSubstitutionRecord>> ligatureSubstitutionRecordLookup = m_FontFeatureTable.m_LigatureSubstitutionRecordLookup;
		List<LigatureSubstitutionRecord> ligatureSubstitutionRecords = m_FontFeatureTable.m_LigatureSubstitutionRecords;
		EnsureAdditionalCapacity(ligatureSubstitutionRecordLookup, records.Length);
		EnsureAdditionalCapacity(ligatureSubstitutionRecords, records.Length);
		for (int i = 0; i < records.Length; i++)
		{
			LigatureSubstitutionRecord ligatureSubstitutionRecord = records[i];
			if (ligatureSubstitutionRecord.componentGlyphIDs == null || ligatureSubstitutionRecord.ligatureGlyphID == 0)
			{
				break;
			}
			uint key = ligatureSubstitutionRecord.componentGlyphIDs[0];
			LigatureSubstitutionRecord ligatureSubstitutionRecord2 = new LigatureSubstitutionRecord
			{
				componentGlyphIDs = ligatureSubstitutionRecord.componentGlyphIDs,
				ligatureGlyphID = ligatureSubstitutionRecord.ligatureGlyphID
			};
			if (ligatureSubstitutionRecordLookup.TryGetValue(key, out var value))
			{
				foreach (LigatureSubstitutionRecord item in value)
				{
					if (ligatureSubstitutionRecord2 == item)
					{
						return;
					}
				}
				ligatureSubstitutionRecordLookup[key].Add(ligatureSubstitutionRecord2);
			}
			else
			{
				ligatureSubstitutionRecordLookup.Add(key, new List<LigatureSubstitutionRecord> { ligatureSubstitutionRecord2 });
			}
			ligatureSubstitutionRecords.Add(ligatureSubstitutionRecord2);
		}
	}

	internal void UpdateGlyphAdjustmentRecords()
	{
		LoadFontFace();
		GlyphPairAdjustmentRecord[] pairAdjustmentRecords = FontEngine.GetPairAdjustmentRecords(m_GlyphIndexListNewlyAdded);
		if (pairAdjustmentRecords != null)
		{
			AddPairAdjustmentRecords(pairAdjustmentRecords);
		}
	}

	private void AddPairAdjustmentRecords(GlyphPairAdjustmentRecord[] records)
	{
		float num = m_FaceInfo.pointSize / (float)m_FaceInfo.unitsPerEM;
		List<GlyphPairAdjustmentRecord> glyphPairAdjustmentRecords = m_FontFeatureTable.glyphPairAdjustmentRecords;
		Dictionary<uint, GlyphPairAdjustmentRecord> glyphPairAdjustmentRecordLookup = m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup;
		EnsureAdditionalCapacity(glyphPairAdjustmentRecordLookup, records.Length);
		EnsureAdditionalCapacity(glyphPairAdjustmentRecords, records.Length);
		for (int i = 0; i < records.Length; i++)
		{
			GlyphPairAdjustmentRecord glyphPairAdjustmentRecord = records[i];
			GlyphAdjustmentRecord firstAdjustmentRecord = glyphPairAdjustmentRecord.firstAdjustmentRecord;
			GlyphAdjustmentRecord secondAdjustmentRecord = glyphPairAdjustmentRecord.secondAdjustmentRecord;
			uint glyphIndex = firstAdjustmentRecord.glyphIndex;
			uint glyphIndex2 = secondAdjustmentRecord.glyphIndex;
			if (glyphIndex == 0 && glyphIndex2 == 0)
			{
				break;
			}
			uint key = (glyphIndex2 << 16) | glyphIndex;
			GlyphPairAdjustmentRecord glyphPairAdjustmentRecord2 = glyphPairAdjustmentRecord;
			GlyphValueRecord glyphValueRecord = firstAdjustmentRecord.glyphValueRecord;
			glyphValueRecord.xAdvance *= num;
			glyphPairAdjustmentRecord2.firstAdjustmentRecord = new GlyphAdjustmentRecord(glyphIndex, glyphValueRecord);
			if (glyphPairAdjustmentRecordLookup.TryAdd(key, glyphPairAdjustmentRecord2))
			{
				glyphPairAdjustmentRecords.Add(glyphPairAdjustmentRecord2);
			}
		}
	}

	internal void UpdateDiacriticalMarkAdjustmentRecords()
	{
		using (k_UpdateDiacriticalMarkAdjustmentRecordsMarker.Auto())
		{
			MarkToBaseAdjustmentRecord[] markToBaseAdjustmentRecords = FontEngine.GetMarkToBaseAdjustmentRecords(m_GlyphIndexListNewlyAdded);
			if (markToBaseAdjustmentRecords != null)
			{
				AddMarkToBaseAdjustmentRecords(markToBaseAdjustmentRecords);
			}
			MarkToMarkAdjustmentRecord[] markToMarkAdjustmentRecords = FontEngine.GetMarkToMarkAdjustmentRecords(m_GlyphIndexListNewlyAdded);
			if (markToMarkAdjustmentRecords != null)
			{
				AddMarkToMarkAdjustmentRecords(markToMarkAdjustmentRecords);
			}
		}
	}

	private void AddMarkToBaseAdjustmentRecords(MarkToBaseAdjustmentRecord[] records)
	{
		float num = m_FaceInfo.pointSize / (float)m_FaceInfo.unitsPerEM;
		for (int i = 0; i < records.Length; i++)
		{
			MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord = records[i];
			if (markToBaseAdjustmentRecord.baseGlyphID == 0 || markToBaseAdjustmentRecord.markGlyphID == 0)
			{
				break;
			}
			uint key = (markToBaseAdjustmentRecord.markGlyphID << 16) | markToBaseAdjustmentRecord.baseGlyphID;
			if (!m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.ContainsKey(key))
			{
				MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord2 = new MarkToBaseAdjustmentRecord
				{
					baseGlyphID = markToBaseAdjustmentRecord.baseGlyphID,
					baseGlyphAnchorPoint = new GlyphAnchorPoint
					{
						xCoordinate = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.xCoordinate * num,
						yCoordinate = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.yCoordinate * num
					},
					markGlyphID = markToBaseAdjustmentRecord.markGlyphID,
					markPositionAdjustment = new MarkPositionAdjustment
					{
						xPositionAdjustment = markToBaseAdjustmentRecord.markPositionAdjustment.xPositionAdjustment * num,
						yPositionAdjustment = markToBaseAdjustmentRecord.markPositionAdjustment.yPositionAdjustment * num
					}
				};
				m_FontFeatureTable.MarkToBaseAdjustmentRecords.Add(markToBaseAdjustmentRecord2);
				m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.Add(key, markToBaseAdjustmentRecord2);
			}
		}
	}

	private void AddMarkToMarkAdjustmentRecords(MarkToMarkAdjustmentRecord[] records)
	{
		float num = m_FaceInfo.pointSize / (float)m_FaceInfo.unitsPerEM;
		for (int i = 0; i < records.Length; i++)
		{
			MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord = records[i];
			if (records[i].baseMarkGlyphID == 0 || records[i].combiningMarkGlyphID == 0)
			{
				break;
			}
			uint key = (markToMarkAdjustmentRecord.combiningMarkGlyphID << 16) | markToMarkAdjustmentRecord.baseMarkGlyphID;
			if (!m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.ContainsKey(key))
			{
				MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord2 = new MarkToMarkAdjustmentRecord
				{
					baseMarkGlyphID = markToMarkAdjustmentRecord.baseMarkGlyphID,
					baseMarkGlyphAnchorPoint = new GlyphAnchorPoint
					{
						xCoordinate = markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.xCoordinate * num,
						yCoordinate = markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.yCoordinate * num
					},
					combiningMarkGlyphID = markToMarkAdjustmentRecord.combiningMarkGlyphID,
					combiningMarkPositionAdjustment = new MarkPositionAdjustment
					{
						xPositionAdjustment = markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.xPositionAdjustment * num,
						yPositionAdjustment = markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.yPositionAdjustment * num
					}
				};
				m_FontFeatureTable.MarkToMarkAdjustmentRecords.Add(markToMarkAdjustmentRecord2);
				m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.Add(key, markToMarkAdjustmentRecord2);
			}
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal void EnsureNativeFontAssetIsCreated()
	{
		if (!(m_NativeFontAsset != IntPtr.Zero) && !JobsUtility.IsExecutingJob)
		{
			if (atlasPopulationMode == AtlasPopulationMode.Static && characterTable.Count > 0)
			{
				Debug.LogWarning("Advanced text system cannot use static font asset " + base.name + ".");
				return;
			}
			if (atlasPopulationMode == AtlasPopulationMode.Dynamic && sourceFontFile == null)
			{
				Debug.LogWarning(base.name + " FontAsset is invalid. Please assign a Source Font File.");
				return;
			}
			IntPtr[] fallbacks = GetFallbacks();
			(IntPtr[], IntPtr[]) weightFallbacks = GetWeightFallbacks();
			Font sourceFont_EditorRef = null;
			m_NativeFontAsset = Create(faceInfo, sourceFontFile, sourceFont_EditorRef, m_SourceFontFilePath, base.instanceID, fallbacks, weightFallbacks.Item1, weightFallbacks.Item2, m_AtlasRenderMode, MarshalledUnityObject.MarshalNotNull(this));
		}
	}

	internal void UpdateFallbacks()
	{
		UpdateFallbacks(nativeFontAsset, GetFallbacks());
	}

	internal void UpdateWeightFallbacks()
	{
		(IntPtr[], IntPtr[]) weightFallbacks = GetWeightFallbacks();
		UpdateWeightFallbacks(nativeFontAsset, weightFallbacks.Item1, weightFallbacks.Item2);
	}

	internal void UpdateFaceInfo()
	{
		UpdateFaceInfo(nativeFontAsset, faceInfo);
	}

	internal void UpdateRenderMode()
	{
		UpdateRenderMode(nativeFontAsset, m_AtlasRenderMode);
	}

	internal IntPtr[] GetFallbacks()
	{
		List<IntPtr> list = new List<IntPtr>();
		if (fallbackFontAssetTable == null)
		{
			return list.ToArray();
		}
		foreach (FontAsset item in fallbackFontAssetTable)
		{
			if (!(item == null))
			{
				if (item.atlasPopulationMode == AtlasPopulationMode.Static && item.characterTable.Count > 0)
				{
					Debug.LogWarning("Advanced text system cannot use static font asset " + item.name + " as fallback.");
				}
				else if (!HasRecursion(item))
				{
					list.Add(item.nativeFontAsset);
				}
			}
		}
		return list.ToArray();
	}

	private bool HasRecursion(FontAsset fontAsset)
	{
		visitedFontAssets.Clear();
		return HasRecursionInternal(fontAsset);
	}

	private bool HasRecursionInternal(FontAsset fontAsset)
	{
		if (visitedFontAssets.Contains(fontAsset.instanceID))
		{
			return true;
		}
		visitedFontAssets.Add(fontAsset.instanceID);
		if (fontAsset.fallbackFontAssetTable != null)
		{
			foreach (FontAsset item in fontAsset.fallbackFontAssetTable)
			{
				if (HasRecursionInternal(item))
				{
					return true;
				}
			}
		}
		for (int i = 0; i < fontAsset.fontWeightTable.Length; i++)
		{
			FontWeightPair fontWeightPair = fontAsset.fontWeightTable[i];
			if (fontWeightPair.regularTypeface != null && HasRecursionInternal(fontWeightPair.regularTypeface))
			{
				return true;
			}
			if (fontWeightPair.italicTypeface != null && HasRecursionInternal(fontWeightPair.italicTypeface))
			{
				return true;
			}
		}
		visitedFontAssets.Remove(fontAsset.instanceID);
		return false;
	}

	private (IntPtr[], IntPtr[]) GetWeightFallbacks()
	{
		IntPtr[] array = new IntPtr[10];
		IntPtr[] array2 = new IntPtr[10];
		for (int i = 0; i < fontWeightTable.Length; i++)
		{
			FontWeightPair fontWeightPair = fontWeightTable[i];
			if (fontWeightPair.regularTypeface != null)
			{
				if (fontWeightPair.regularTypeface.atlasPopulationMode == AtlasPopulationMode.Static && fontWeightPair.regularTypeface.characterTable.Count > 0)
				{
					Debug.LogWarning("Advanced text system cannot use static font asset " + fontWeightPair.regularTypeface.name + " as fallback.");
					continue;
				}
				if (HasRecursion(fontWeightPair.regularTypeface))
				{
					Debug.LogWarning("Circular reference detected. Cannot add " + fontWeightPair.regularTypeface.name + " to the fallbacks.");
					continue;
				}
				array[i] = fontWeightPair.regularTypeface.nativeFontAsset;
			}
			if (fontWeightPair.italicTypeface != null)
			{
				if (fontWeightPair.italicTypeface.atlasPopulationMode == AtlasPopulationMode.Static && fontWeightPair.italicTypeface.characterTable.Count > 0)
				{
					Debug.LogWarning("Advanced text system cannot use static font asset " + fontWeightPair.italicTypeface.name + " as fallback.");
				}
				else if (HasRecursion(fontWeightPair.italicTypeface))
				{
					Debug.LogWarning("Circular reference detected. Cannot add " + fontWeightPair.italicTypeface.name + " to the fallbacks.");
				}
				else
				{
					array2[i] = fontWeightPair.italicTypeface.nativeFontAsset;
				}
			}
		}
		return (array, array2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static extern void CreateHbFaceIfNeeded();

	private unsafe static void UpdateFallbacks(IntPtr ptr, IntPtr[] fallbacks)
	{
		Span<IntPtr> span = new Span<IntPtr>(fallbacks);
		fixed (IntPtr* begin = span)
		{
			ManagedSpanWrapper fallbacks2 = new ManagedSpanWrapper(begin, span.Length);
			UpdateFallbacks_Injected(ptr, ref fallbacks2);
		}
	}

	private unsafe static void UpdateWeightFallbacks(IntPtr ptr, IntPtr[] regularFallbacks, IntPtr[] italicFallbacks)
	{
		Span<IntPtr> span = new Span<IntPtr>(regularFallbacks);
		fixed (IntPtr* begin = span)
		{
			ManagedSpanWrapper regularFallbacks2 = new ManagedSpanWrapper(begin, span.Length);
			Span<IntPtr> span2 = new Span<IntPtr>(italicFallbacks);
			fixed (IntPtr* begin2 = span2)
			{
				ManagedSpanWrapper italicFallbacks2 = new ManagedSpanWrapper(begin2, span2.Length);
				UpdateWeightFallbacks_Injected(ptr, ref regularFallbacks2, ref italicFallbacks2);
			}
		}
	}

	private unsafe static IntPtr Create(FaceInfo faceInfo, Font sourceFontFile, Font sourceFont_EditorRef, string sourceFontFilePath, EntityId fontEntityId, IntPtr[] fallbacks, IntPtr[] weightFallbacks, IntPtr[] italicFallbacks, GlyphRenderMode renderMode, IntPtr managedObject)
	{
		//The blocks IL_0037, IL_004c, IL_005a, IL_0072, IL_0080, IL_0098, IL_00a6 are reachable both inside and outside the pinned region starting at IL_0026. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.Marshal(sourceFontFile);
			IntPtr sourceFont_EditorRef2 = MarshalledUnityObject.Marshal(sourceFont_EditorRef);
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper sourceFontFilePath2;
			ref EntityId fontEntityId2;
			Span<IntPtr> span;
			ManagedSpanWrapper managedSpanWrapper2;
			ref ManagedSpanWrapper fallbacks2;
			Span<IntPtr> span2;
			ManagedSpanWrapper managedSpanWrapper3;
			ref ManagedSpanWrapper weightFallbacks2;
			Span<IntPtr> span3;
			ManagedSpanWrapper italicFallbacks2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(sourceFontFilePath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(sourceFontFilePath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					sourceFontFilePath2 = ref managedSpanWrapper;
					fontEntityId2 = ref fontEntityId;
					span = new Span<IntPtr>(fallbacks);
					fixed (IntPtr* begin2 = span)
					{
						managedSpanWrapper2 = new ManagedSpanWrapper(begin2, span.Length);
						fallbacks2 = ref managedSpanWrapper2;
						span2 = new Span<IntPtr>(weightFallbacks);
						fixed (IntPtr* begin3 = span2)
						{
							managedSpanWrapper3 = new ManagedSpanWrapper(begin3, span2.Length);
							weightFallbacks2 = ref managedSpanWrapper3;
							span3 = new Span<IntPtr>(italicFallbacks);
							fixed (IntPtr* begin4 = span3)
							{
								italicFallbacks2 = new ManagedSpanWrapper(begin4, span3.Length);
								return Create_Injected(ref faceInfo, intPtr, sourceFont_EditorRef2, ref sourceFontFilePath2, ref fontEntityId2, ref fallbacks2, ref weightFallbacks2, ref italicFallbacks2, renderMode, managedObject);
							}
						}
					}
				}
			}
			sourceFontFilePath2 = ref managedSpanWrapper;
			fontEntityId2 = ref fontEntityId;
			span = new Span<IntPtr>(fallbacks);
			fixed (IntPtr* begin2 = span)
			{
				managedSpanWrapper2 = new ManagedSpanWrapper(begin2, span.Length);
				fallbacks2 = ref managedSpanWrapper2;
				span2 = new Span<IntPtr>(weightFallbacks);
				fixed (IntPtr* begin3 = span2)
				{
					managedSpanWrapper3 = new ManagedSpanWrapper(begin3, span2.Length);
					weightFallbacks2 = ref managedSpanWrapper3;
					span3 = new Span<IntPtr>(italicFallbacks);
					fixed (IntPtr* begin4 = span3)
					{
						italicFallbacks2 = new ManagedSpanWrapper(begin4, span3.Length);
						return Create_Injected(ref faceInfo, intPtr, sourceFont_EditorRef2, ref sourceFontFilePath2, ref fontEntityId2, ref fallbacks2, ref weightFallbacks2, ref italicFallbacks2, renderMode, managedObject);
					}
				}
			}
		}
		finally
		{
		}
	}

	private static void UpdateFaceInfo(IntPtr ptr, FaceInfo faceInfo)
	{
		UpdateFaceInfo_Injected(ptr, ref faceInfo);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateRenderMode(IntPtr ptr, GlyphRenderMode renderMode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("FontAsset::Destroy")]
	private static extern void Destroy(IntPtr ptr, IntPtr managedObject);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateFallbacks_Injected(IntPtr ptr, ref ManagedSpanWrapper fallbacks);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateWeightFallbacks_Injected(IntPtr ptr, ref ManagedSpanWrapper regularFallbacks, ref ManagedSpanWrapper italicFallbacks);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create_Injected([In] ref FaceInfo faceInfo, IntPtr sourceFontFile, IntPtr sourceFont_EditorRef, ref ManagedSpanWrapper sourceFontFilePath, [In] ref EntityId fontEntityId, ref ManagedSpanWrapper fallbacks, ref ManagedSpanWrapper weightFallbacks, ref ManagedSpanWrapper italicFallbacks, GlyphRenderMode renderMode, IntPtr managedObject);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateFaceInfo_Injected(IntPtr ptr, [In] ref FaceInfo faceInfo);
}
