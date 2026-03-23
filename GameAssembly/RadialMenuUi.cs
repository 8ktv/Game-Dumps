using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class RadialMenuUi : MonoBehaviour
{
	private readonly List<RadialMenuOptionUi> options = new List<RadialMenuOptionUi>();

	private int highlightedIndex;

	public List<RadialMenuOptionUi> Options => options;

	private void Awake()
	{
		BNetworkManager.WillChangeScene += OnWillChangeScene;
	}

	private void OnDestroy()
	{
		BNetworkManager.WillChangeScene -= OnWillChangeScene;
	}

	private void OnWillChangeScene()
	{
		foreach (RadialMenuOptionUi option in options)
		{
			RadialMenu.ReturnOptionUi(option);
		}
	}

	public void ClearOptions()
	{
		foreach (RadialMenuOptionUi option in options)
		{
			RadialMenu.ReturnOptionUi(option);
		}
		options.Clear();
	}

	public void AddOption(Sprite icon, int selectionIndex = -1)
	{
		RadialMenuOptionUi unusedOptionUi = RadialMenu.GetUnusedOptionUi(base.transform);
		unusedOptionUi.Initialize(icon, selectionIndex);
		options.Add(unusedOptionUi);
	}

	public void DistributeOptions()
	{
		if (options != null && options.Count > 0)
		{
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSlice(i, options.Count);
			}
		}
	}

	public void SetHighlightedIndex(int index, bool forced)
	{
		if (forced || index != highlightedIndex)
		{
			highlightedIndex = index;
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetIsHighlighted(i == highlightedIndex, instant: false);
			}
			if (!forced)
			{
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.RadialMenuHover);
			}
		}
	}
}
