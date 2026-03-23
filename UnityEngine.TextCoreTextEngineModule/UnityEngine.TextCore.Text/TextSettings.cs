using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Serialization;

namespace UnityEngine.TextCore.Text;

[Serializable]
[ExcludeFromObjectFactory]
[ExcludeFromPreset]
[NativeHeader("Modules/TextCoreTextEngine/Native/TextSettings.h")]
public class TextSettings : ScriptableObject
{
	[Serializable]
	internal struct FontReferenceMap
	{
		public Font font;

		public FontAsset fontAsset;

		public FontReferenceMap(Font font, FontAsset fontAsset)
		{
			this.font = font;
			this.fontAsset = fontAsset;
		}
	}

	[SerializeField]
	protected string m_Version;

	[SerializeField]
	[FormerlySerializedAs("m_defaultFontAsset")]
	protected FontAsset m_DefaultFontAsset;

	[SerializeField]
	[FormerlySerializedAs("m_defaultFontAssetPath")]
	protected string m_DefaultFontAssetPath = "Fonts & Materials/";

	[SerializeField]
	[FormerlySerializedAs("m_fallbackFontAssets")]
	protected List<FontAsset> m_FallbackFontAssets;

	private static List<FontAsset> s_FallbackOSFontAssetInternal;

	[FormerlySerializedAs("m_matchMaterialPreset")]
	[SerializeField]
	protected bool m_MatchMaterialPreset;

	[SerializeField]
	[FormerlySerializedAs("m_missingGlyphCharacter")]
	protected int m_MissingCharacterUnicode;

	[SerializeField]
	protected bool m_ClearDynamicDataOnBuild = true;

	[SerializeField]
	private bool m_EnableEmojiSupport;

	[SerializeField]
	private List<TextAsset> m_EmojiFallbackTextAssets;

	[FormerlySerializedAs("m_defaultSpriteAsset")]
	[SerializeField]
	protected SpriteAsset m_DefaultSpriteAsset;

	[FormerlySerializedAs("m_defaultSpriteAssetPath")]
	[SerializeField]
	protected string m_DefaultSpriteAssetPath = "Sprite Assets/";

	[SerializeField]
	protected List<SpriteAsset> m_FallbackSpriteAssets;

	[SerializeField]
	protected uint m_MissingSpriteCharacterUnicode;

	[SerializeField]
	[FormerlySerializedAs("m_defaultStyleSheet")]
	protected TextStyleSheet m_DefaultStyleSheet;

	[SerializeField]
	protected string m_StyleSheetsResourcePath = "Text Style Sheets/";

	[FormerlySerializedAs("m_defaultColorGradientPresetsPath")]
	[SerializeField]
	protected string m_DefaultColorGradientPresetsPath = "Text Color Gradients/";

	[SerializeField]
	protected UnicodeLineBreakingRules m_UnicodeLineBreakingRules;

	[FormerlySerializedAs("m_warningsDisabled")]
	[SerializeField]
	protected bool m_DisplayWarnings = false;

	internal Dictionary<int, FontAsset> m_FontLookup;

	internal List<FontReferenceMap> m_FontReferences = new List<FontReferenceMap>();

	private IntPtr m_NativeTextSettings = IntPtr.Zero;

	private bool m_IsNativeTextSettingsDirty = true;

	public string version
	{
		get
		{
			return m_Version;
		}
		internal set
		{
			m_Version = value;
		}
	}

	public FontAsset defaultFontAsset
	{
		get
		{
			return m_DefaultFontAsset;
		}
		set
		{
			m_DefaultFontAsset = value;
		}
	}

	public string defaultFontAssetPath
	{
		get
		{
			return m_DefaultFontAssetPath;
		}
		set
		{
			m_DefaultFontAssetPath = value;
		}
	}

	public List<FontAsset> fallbackFontAssets
	{
		get
		{
			return m_FallbackFontAssets;
		}
		set
		{
			m_FallbackFontAssets = value;
			m_IsNativeTextSettingsDirty = true;
		}
	}

	internal List<FontAsset> fallbackOSFontAssets
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		get
		{
			if (GetStaticFallbackOSFontAsset() == null)
			{
				SetStaticFallbackOSFontAsset(GetOSFontAssetList());
			}
			return GetStaticFallbackOSFontAsset();
		}
	}

	public bool matchMaterialPreset
	{
		get
		{
			return m_MatchMaterialPreset;
		}
		set
		{
			m_MatchMaterialPreset = value;
		}
	}

	public int missingCharacterUnicode
	{
		get
		{
			return m_MissingCharacterUnicode;
		}
		set
		{
			m_MissingCharacterUnicode = value;
		}
	}

	public bool clearDynamicDataOnBuild
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

	public bool enableEmojiSupport
	{
		get
		{
			return m_EnableEmojiSupport;
		}
		set
		{
			m_EnableEmojiSupport = value;
		}
	}

	public List<TextAsset> emojiFallbackTextAssets
	{
		get
		{
			return m_EmojiFallbackTextAssets;
		}
		set
		{
			m_EmojiFallbackTextAssets = value;
			m_IsNativeTextSettingsDirty = true;
		}
	}

	public SpriteAsset defaultSpriteAsset
	{
		get
		{
			return m_DefaultSpriteAsset;
		}
		set
		{
			m_DefaultSpriteAsset = value;
		}
	}

	public string defaultSpriteAssetPath
	{
		get
		{
			return m_DefaultSpriteAssetPath;
		}
		set
		{
			m_DefaultSpriteAssetPath = value;
		}
	}

	[Obsolete("The Fallback Sprite Assets list is now obsolete. Use the emojiFallbackTextAssets instead.", true)]
	public List<SpriteAsset> fallbackSpriteAssets
	{
		get
		{
			return m_FallbackSpriteAssets;
		}
		set
		{
			m_FallbackSpriteAssets = value;
		}
	}

	internal static SpriteAsset s_GlobalSpriteAsset { get; private set; }

	public uint missingSpriteCharacterUnicode
	{
		get
		{
			return m_MissingSpriteCharacterUnicode;
		}
		set
		{
			m_MissingSpriteCharacterUnicode = value;
		}
	}

	public TextStyleSheet defaultStyleSheet
	{
		get
		{
			return m_DefaultStyleSheet;
		}
		set
		{
			m_DefaultStyleSheet = value;
		}
	}

	public string styleSheetsResourcePath
	{
		get
		{
			return m_StyleSheetsResourcePath;
		}
		set
		{
			m_StyleSheetsResourcePath = value;
		}
	}

	public string defaultColorGradientPresetsPath
	{
		get
		{
			return m_DefaultColorGradientPresetsPath;
		}
		set
		{
			m_DefaultColorGradientPresetsPath = value;
		}
	}

	public UnicodeLineBreakingRules lineBreakingRules
	{
		get
		{
			if (m_UnicodeLineBreakingRules == null)
			{
				m_UnicodeLineBreakingRules = new UnicodeLineBreakingRules();
				m_UnicodeLineBreakingRules.LoadLineBreakingRules();
			}
			return m_UnicodeLineBreakingRules;
		}
		set
		{
			m_UnicodeLineBreakingRules = value;
		}
	}

	public bool displayWarnings
	{
		get
		{
			return m_DisplayWarnings;
		}
		set
		{
			m_DisplayWarnings = value;
		}
	}

	internal IntPtr nativeTextSettings
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		get
		{
			UpdateNativeTextSettings();
			return m_NativeTextSettings;
		}
	}

	internal virtual List<FontAsset> GetStaticFallbackOSFontAsset()
	{
		return s_FallbackOSFontAssetInternal;
	}

	internal virtual void SetStaticFallbackOSFontAsset(List<FontAsset> fontAssets)
	{
		s_FallbackOSFontAssetInternal = fontAssets;
	}

	internal virtual List<FontAsset> GetFallbackFontAssets(bool isRaster, int textPixelSize = -1)
	{
		return fallbackFontAssets;
	}

	private void OnEnable()
	{
		lineBreakingRules.LoadLineBreakingRules();
		SetStaticFallbackOSFontAsset(null);
		if (s_GlobalSpriteAsset == null)
		{
			s_GlobalSpriteAsset = Resources.Load<SpriteAsset>("Sprite Assets/Default Sprite Asset");
		}
	}

	private void OnDestroy()
	{
		if (m_NativeTextSettings != IntPtr.Zero)
		{
			DestroyNativeObject(m_NativeTextSettings);
		}
	}

	protected void InitializeFontReferenceLookup()
	{
		if (m_FontReferences == null)
		{
			m_FontReferences = new List<FontReferenceMap>();
		}
		for (int i = 0; i < m_FontReferences.Count; i++)
		{
			FontReferenceMap fontReferenceMap = m_FontReferences[i];
			if (fontReferenceMap.font == null || fontReferenceMap.fontAsset == null)
			{
				Debug.LogWarning("Deleting invalid font reference.");
				m_FontReferences.RemoveAt(i);
				i--;
				continue;
			}
			int hashCode = fontReferenceMap.font.GetHashCode();
			if (!m_FontLookup.ContainsKey(hashCode))
			{
				m_FontLookup.Add(hashCode, fontReferenceMap.fontAsset);
			}
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal FontAsset GetCachedFontAsset(Font font)
	{
		if (font == null)
		{
			return null;
		}
		if (m_FontLookup == null)
		{
			m_FontLookup = new Dictionary<int, FontAsset>();
			InitializeFontReferenceLookup();
		}
		int hashCode = font.GetHashCode();
		if (m_FontLookup.ContainsKey(hashCode))
		{
			return m_FontLookup[hashCode];
		}
		if (TextGenerator.IsExecutingJob)
		{
			return null;
		}
		FontAsset fontAsset = FontAssetFactory.ConvertFontToFontAsset(font);
		if (fontAsset != null)
		{
			m_FontReferences.Add(new FontReferenceMap(font, fontAsset));
			m_FontLookup.Add(hashCode, fontAsset);
		}
		return fontAsset;
	}

	private List<FontAsset> GetOSFontAssetList()
	{
		string[] oSFallbacks = Font.GetOSFallbacks();
		return FontAsset.CreateFontAssetOSFallbackList(oSFallbacks);
	}

	[NativeMethod(Name = "TextSettings::Create")]
	private unsafe static IntPtr CreateNativeObject(IntPtr[] fallbacks)
	{
		Span<IntPtr> span = new Span<IntPtr>(fallbacks);
		IntPtr result;
		fixed (IntPtr* begin = span)
		{
			ManagedSpanWrapper fallbacks2 = new ManagedSpanWrapper(begin, span.Length);
			result = CreateNativeObject_Injected(ref fallbacks2);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSettings::Destroy")]
	private static extern void DestroyNativeObject(IntPtr m_NativeTextSettings);

	private unsafe static void UpdateFallbacks(IntPtr ptr, IntPtr[] fallbacks)
	{
		Span<IntPtr> span = new Span<IntPtr>(fallbacks);
		fixed (IntPtr* begin = span)
		{
			ManagedSpanWrapper fallbacks2 = new ManagedSpanWrapper(begin, span.Length);
			UpdateFallbacks_Injected(ptr, ref fallbacks2);
		}
	}

	private IntPtr[] GetGlobalFallbacks()
	{
		List<IntPtr> globalFontAssetFallbacks = new List<IntPtr>();
		fallbackFontAssets?.ForEach(delegate(FontAsset fallback)
		{
			if (!(fallback == null))
			{
				if (fallback.atlasPopulationMode == AtlasPopulationMode.Static && fallback.characterTable.Count > 0)
				{
					Debug.LogWarning("Advanced text system cannot use static font asset " + fallback.name + " as fallback.");
				}
				else
				{
					globalFontAssetFallbacks.Add(fallback.nativeFontAsset);
				}
			}
		});
		fallbackOSFontAssets?.ForEach(delegate(FontAsset fallback)
		{
			if (!(fallback == null))
			{
				if (fallback.atlasPopulationMode == AtlasPopulationMode.Static && fallback.characterTable.Count > 0)
				{
					Debug.LogWarning("Advanced text system cannot use static font asset " + fallback.name + " as fallback.");
				}
				else
				{
					globalFontAssetFallbacks.Add(fallback.nativeFontAsset);
				}
			}
		});
		emojiFallbackTextAssets?.ForEach(delegate(TextAsset fallback)
		{
			if (fallback is FontAsset fontAsset && !(fontAsset == null))
			{
				if (fontAsset.atlasPopulationMode == AtlasPopulationMode.Static && fontAsset.characterTable.Count > 0)
				{
					Debug.LogWarning("Advanced text system cannot use static font asset " + fallback.name + " as fallback.");
				}
				else
				{
					globalFontAssetFallbacks.Add(fontAsset.nativeFontAsset);
				}
			}
		});
		return globalFontAssetFallbacks.ToArray();
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal void UpdateNativeTextSettings()
	{
		if (m_NativeTextSettings == IntPtr.Zero)
		{
			m_NativeTextSettings = CreateNativeObject(GetGlobalFallbacks());
			m_IsNativeTextSettingsDirty = false;
		}
		else if (m_IsNativeTextSettingsDirty && m_NativeTextSettings != IntPtr.Zero)
		{
			UpdateFallbacks(m_NativeTextSettings, GetGlobalFallbacks());
			m_IsNativeTextSettingsDirty = false;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateNativeObject_Injected(ref ManagedSpanWrapper fallbacks);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateFallbacks_Injected(IntPtr ptr, ref ManagedSpanWrapper fallbacks);
}
