using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizedFont : MonoBehaviour
{
	[Serializable]
	public class LocalizedFontAsset
	{
		[SerializeField]
		internal string localeCode;

		[SerializeField]
		internal TMP_FontAsset fontAsset;
	}

	[SerializeField]
	private LocalizedFontAsset[] localizedFonts;

	private TMP_FontAsset defaultFontAsset;

	private TMP_Text label;

	private void Awake()
	{
		label = GetComponent<TMP_Text>();
		defaultFontAsset = label.font;
		RefreshFont();
		LocalizationManager.LanguageChanged += RefreshFont;
	}

	private void OnDestroy()
	{
		LocalizationManager.LanguageChanged -= RefreshFont;
	}

	private void RefreshFont()
	{
		label.font = GetFont();
	}

	private TMP_FontAsset GetFont()
	{
		string code = LocalizationSettings.SelectedLocale.Identifier.Code;
		LocalizedFontAsset[] array = localizedFonts;
		foreach (LocalizedFontAsset localizedFontAsset in array)
		{
			if (localizedFontAsset.localeCode == code && localizedFontAsset.fontAsset != null)
			{
				return localizedFontAsset.fontAsset;
			}
		}
		return defaultFontAsset;
	}
}
