using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownOption : MonoBehaviour
{
	[SerializeField]
	private TMP_Dropdown dropdown;

	[SerializeField]
	private LocalizeDropdown localized;

	private Action onChanged;

	public TMP_Text captionText => dropdown.captionText;

	public LocalizeDropdown Localized => localized;

	public bool Interactable
	{
		get
		{
			return dropdown.interactable;
		}
		set
		{
			dropdown.interactable = value;
		}
	}

	public int OptionsCount => dropdown.options.Count;

	public Selectable Selectable => dropdown;

	public int value
	{
		get
		{
			return dropdown.value;
		}
		set
		{
			SetValue(value);
		}
	}

	public int valueWithoutNotify
	{
		set
		{
			dropdown.SetValueWithoutNotify(value);
		}
	}

	public void Initialize(Action onChanged, int startValue = -1)
	{
		this.onChanged = onChanged;
		if (startValue > -1)
		{
			SetValue(startValue);
		}
	}

	public void SetValue(int value)
	{
		localized?.RefreshOptions();
		dropdown.value = value;
		onChanged?.Invoke();
	}

	public void SetOptions(List<string> options)
	{
		dropdown.ClearOptions();
		dropdown.AddOptions(options);
	}

	public void OnValueChanged()
	{
		onChanged?.Invoke();
	}
}
