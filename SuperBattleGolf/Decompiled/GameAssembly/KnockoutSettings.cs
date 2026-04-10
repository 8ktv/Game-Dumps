using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Knockout settings", menuName = "Settings/Gameplay/Knockouts")]
public class KnockoutSettings : ScriptableObject
{
	[SerializeField]
	[DynamicElementName("type")]
	private KnockoutData[] knockouts;

	private readonly Dictionary<KnockoutType, KnockoutData> knockoutDictionary = new Dictionary<KnockoutType, KnockoutData>();

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	private void Initialize()
	{
		knockoutDictionary.Clear();
		KnockoutData[] array = knockouts;
		for (int i = 0; i < array.Length; i++)
		{
			KnockoutData value = array[i];
			knockoutDictionary.Add(value.type, value);
		}
	}

	public bool TryGetKnockoutData(KnockoutType knockout, out KnockoutData data)
	{
		return knockoutDictionary.TryGetValue(knockout, out data);
	}
}
