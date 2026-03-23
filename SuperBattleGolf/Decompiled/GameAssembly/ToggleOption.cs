using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleOption : MonoBehaviour
{
	[SerializeField]
	private Toggle toggle;

	private Action onChanged;

	public bool isOn
	{
		get
		{
			return toggle.isOn;
		}
		set
		{
			SetIsOn(value);
		}
	}

	public void Initialize(Action onChanged, bool value)
	{
		this.onChanged = onChanged;
		SetIsOn(value);
	}

	public void SetIsOn(bool value)
	{
		toggle.isOn = value;
		onChanged?.Invoke();
	}

	public void OnValueChanged()
	{
		onChanged?.Invoke();
	}
}
