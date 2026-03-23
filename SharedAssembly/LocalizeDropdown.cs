using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[DisallowMultipleComponent]
public class LocalizeDropdown : MonoBehaviour
{
	[SerializeField]
	private List<LocalizedString> dropdownOptions;

	private TMP_Dropdown tmpDropdown;

	private void Awake()
	{
		tmpDropdown = GetComponent<TMP_Dropdown>();
		RefreshOptions();
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
	}

	private void OnDestroy()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}

	public void RefreshOptions()
	{
		if (tmpDropdown == null)
		{
			tmpDropdown = GetComponent<TMP_Dropdown>();
		}
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		foreach (LocalizedString dropdownOption in dropdownOptions)
		{
			list.Add(new TMP_Dropdown.OptionData(dropdownOption.GetLocalizedString()));
		}
		tmpDropdown.options = list;
	}

	private void OnLocaleChanged(Locale newLocale)
	{
		RefreshOptions();
	}
}
