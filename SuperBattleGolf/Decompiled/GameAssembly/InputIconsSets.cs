using System.Collections.Generic;
using UnityEngine;

public class InputIconsSets : ScriptableObject
{
	[SerializeField]
	[DynamicElementName("device")]
	private InputIconsSet[] iconSets;

	private Dictionary<InputManager.DeviceType, int> setsIndex;

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	public InputIconsSet GetIconSet(InputManager.DeviceType deviceType)
	{
		if (setsIndex == null)
		{
			return null;
		}
		if (setsIndex.TryGetValue(deviceType, out var value))
		{
			return iconSets[value];
		}
		if (setsIndex.TryGetValue(InputManager.DeviceType.Xbox, out value))
		{
			return iconSets[value];
		}
		return null;
	}

	private void Initialize()
	{
		setsIndex = new Dictionary<InputManager.DeviceType, int>();
		for (int i = 0; i < iconSets.Length; i++)
		{
			setsIndex[iconSets[i].icons.Device] = i;
		}
	}
}
