using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Info feed icon settings", menuName = "Settings/UI/Info feed icons")]
public class InfoFeedIconSettings : ScriptableObject
{
	[Serializable]
	private struct Icon
	{
		public InfoFeedIconType type;

		public Sprite icon;
	}

	[SerializeField]
	[DynamicElementName("type")]
	private Icon[] icons;

	private readonly Dictionary<InfoFeedIconType, Sprite> iconDictionary = new Dictionary<InfoFeedIconType, Sprite>();

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
		iconDictionary.Clear();
		Icon[] array = icons;
		for (int i = 0; i < array.Length; i++)
		{
			Icon icon = array[i];
			iconDictionary.Add(icon.type, icon.icon);
		}
	}

	public bool TryGetIcon(InfoFeedIconType iconType, out Sprite icon)
	{
		return iconDictionary.TryGetValue(iconType, out icon);
	}
}
