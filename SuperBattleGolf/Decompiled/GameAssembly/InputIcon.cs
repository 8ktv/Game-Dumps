using System;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;

[Serializable]
public class InputIcon
{
	[Serializable]
	public struct MergeData
	{
		[SerializeField]
		[InputControl]
		internal string mergePartner;

		[SerializeField]
		[InputControl]
		internal string result;
	}

	[SerializeField]
	[InputControl]
	internal string bindingPath;

	[SerializeField]
	internal string iconName;

	[SerializeField]
	internal Sprite icon;

	[SerializeField]
	[ElementName("Merge")]
	internal MergeData[] possibleMerges;

	public bool TryMergeWith(InputIcon other, out string resultBindingPath)
	{
		for (int i = 0; i < possibleMerges.Length; i++)
		{
			if (possibleMerges[i].mergePartner == other.bindingPath)
			{
				resultBindingPath = possibleMerges[i].result;
				return true;
			}
		}
		resultBindingPath = null;
		return false;
	}
}
