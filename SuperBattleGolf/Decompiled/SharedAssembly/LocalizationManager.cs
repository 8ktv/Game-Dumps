using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public static class LocalizationManager
{
	private static bool isInitialized;

	public static string CurrentLanguage => LocalizationSettings.SelectedLocale.Identifier.Code;

	public static bool CurrentLanguageIsEnglish => CurrentLanguage == "en";

	public static event Action LanguageChanged;

	[CCommand("setLanguage", "", false, false, description = "Set game language (en, fr, it, de, es, ja, zh-CN, zh-TW)")]
	public static void SetLanguage(string languageCode)
	{
		foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
		{
			if (locale.Identifier.Code == languageCode)
			{
				LocalizationSettings.SelectedLocale = locale;
				break;
			}
		}
		foreach (Locale locale2 in LocalizationSettings.AvailableLocales.Locales)
		{
			if (languageCode.Contains(locale2.Identifier.Code))
			{
				LocalizationSettings.SelectedLocale = locale2;
				break;
			}
		}
	}

	public static void Initialize()
	{
		if (!isInitialized)
		{
			LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
			Application.quitting += OnApplicationQuit;
			isInitialized = true;
		}
	}

	private static void OnApplicationQuit()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
		Application.quitting -= OnApplicationQuit;
		isInitialized = false;
	}

	private static void OnLocaleChanged(Locale locale)
	{
		LocalizationManager.LanguageChanged?.Invoke();
	}

	public static string GetString(StringTable table, string id)
	{
		return LocalizationSettings.StringDatabase.GetLocalizedString(table.ToString(), id, null, FallbackBehavior.UseProjectSettings);
	}

	public static LocalizedString GetLocalizedString(StringTable table, string id)
	{
		return new LocalizedString
		{
			TableReference = table.ToString(),
			TableEntryReference = id
		};
	}

	public static bool StringExists(StringTable table, string id)
	{
		return StringExists(GetLocalizedString(table, id));
	}

	public static bool StringExists(LocalizedString localizedString)
	{
		UnityEngine.Localization.Tables.StringTable table = LocalizationSettings.StringDatabase.GetTable(localizedString.TableReference);
		if (table != null)
		{
			return table.GetEntryFromReference(localizedString.TableEntryReference) != null;
		}
		return false;
	}
}
