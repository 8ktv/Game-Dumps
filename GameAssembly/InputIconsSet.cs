using System;
using TMPro;
using UnityEngine;

[Serializable]
public class InputIconsSet
{
	[SerializeField]
	internal InputIconCollection icons;

	[SerializeField]
	internal TMP_SpriteAsset spriteAsset;

	public Sprite GetIcon(string binding)
	{
		if (string.IsNullOrEmpty(binding))
		{
			return null;
		}
		return icons.GetIconSprite(binding);
	}

	public string GetIconRichTextTag(string binding)
	{
		if (string.IsNullOrEmpty(binding))
		{
			return null;
		}
		return "<sprite=\"" + spriteAsset.name + "\" name=\"" + icons.GetIconName(binding) + "\">";
	}

	public bool TryMerge(string bindingPathA, string bindingPathB, out InputIcon result)
	{
		return icons.TryMerge(bindingPathA, bindingPathB, out result);
	}

	public bool TryMerge(InputIcon a, InputIcon b, out InputIcon result)
	{
		return icons.TryMerge(a, b, out result);
	}
}
