using UnityEngine;

public class GameplayScreenspaceUiCanvas : SingletonBehaviour<GameplayScreenspaceUiCanvas>
{
	[SerializeField]
	private Canvas canvas;

	public static Canvas Canvas
	{
		get
		{
			if (!SingletonBehaviour<GameplayScreenspaceUiCanvas>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameplayScreenspaceUiCanvas>.Instance.canvas;
		}
	}
}
