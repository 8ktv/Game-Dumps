using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Elimination settings", menuName = "Settings/Gameplay/Eliminations")]
public class EliminationSettings : ScriptableObject
{
	[SerializeField]
	[DynamicElementName("type")]
	private EliminationData[] eliminations;

	private readonly Dictionary<EliminationReason, EliminationData> eliminationDictionary = new Dictionary<EliminationReason, EliminationData>();

	[field: SerializeField]
	public float EliminationResponsibilityDuration { get; private set; }

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
		eliminationDictionary.Clear();
		EliminationData[] array = eliminations;
		for (int i = 0; i < array.Length; i++)
		{
			EliminationData value = array[i];
			eliminationDictionary.Add(value.type, value);
		}
	}

	public bool TryGetEliminationData(EliminationReason eliminationReason, out EliminationData data)
	{
		return eliminationDictionary.TryGetValue(eliminationReason, out data);
	}
}
