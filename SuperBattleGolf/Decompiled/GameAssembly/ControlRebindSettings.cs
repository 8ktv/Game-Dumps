using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Control rebind settings", menuName = "Settings/UI/Control rebinds")]
public class ControlRebindSettings : ScriptableObject
{
	[Serializable]
	private struct MapActionPair
	{
		public string map;

		public InputActionReference action;
	}

	[SerializeField]
	[ElementName("Map")]
	private string[] unrebindableMaps;

	[SerializeField]
	[ElementName("Action")]
	private InputActionReference[] unrebindableActions;

	[SerializeField]
	[DynamicElementName("map")]
	private MapActionPair[] mapAddedActions;

	public readonly HashSet<string> allUnrebindableMaps = new HashSet<string>();

	public readonly HashSet<Guid> allUnrebindableActions = new HashSet<Guid>();

	public readonly Dictionary<string, List<InputActionReference>> addedActionsPerMap = new Dictionary<string, List<InputActionReference>>();

	public readonly HashSet<Guid> actionsToIgnoreInOwnMapGuids = new HashSet<Guid>();

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
		allUnrebindableMaps.Clear();
		string[] array = unrebindableMaps;
		foreach (string item in array)
		{
			allUnrebindableMaps.Add(item);
		}
		allUnrebindableActions.Clear();
		InputActionReference[] array2 = unrebindableActions;
		foreach (InputActionReference inputActionReference in array2)
		{
			allUnrebindableActions.Add(inputActionReference.action.id);
		}
		addedActionsPerMap.Clear();
		actionsToIgnoreInOwnMapGuids.Clear();
		MapActionPair[] array3 = mapAddedActions;
		for (int i = 0; i < array3.Length; i++)
		{
			MapActionPair mapActionPair = array3[i];
			if (string.IsNullOrEmpty(mapActionPair.map))
			{
				Debug.LogError("Missing map name in action addition to map", this);
				continue;
			}
			if (mapActionPair.action == null)
			{
				Debug.LogError("Missing action reference in action addition to map", this);
				continue;
			}
			if (!addedActionsPerMap.TryGetValue(mapActionPair.map, out var value))
			{
				value = new List<InputActionReference>();
				addedActionsPerMap.Add(mapActionPair.map, value);
			}
			value.Add(mapActionPair.action);
			actionsToIgnoreInOwnMapGuids.Add(mapActionPair.action.action.id);
		}
	}
}
