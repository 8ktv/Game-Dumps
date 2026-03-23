using System.Collections.Generic;
using UnityEngine;

public class WorldspaceIconManager : SingletonBehaviour<WorldspaceIconManager>
{
	[SerializeField]
	private WorldspaceIconUi worldspaceIconPrefab;

	[SerializeField]
	private int maxPoolSize;

	[SerializeField]
	private Sprite objectiveIcon;

	[SerializeField]
	private Sprite ballDispenserIcon;

	[SerializeField]
	private Sprite ballIcon;

	[SerializeField]
	private Sprite holeIcon;

	[SerializeField]
	private Sprite homingWarningIcon;

	[SerializeField]
	private WorldspaceIconUiSettings objectiveIconSettings;

	[SerializeField]
	private WorldspaceIconUiSettings ballDispenserIconSettings;

	[SerializeField]
	private WorldspaceIconUiSettings ballIconSettings;

	[SerializeField]
	private WorldspaceIconUiSettings holeIconSettings;

	[SerializeField]
	private WorldspaceIconUiSettings homingWarningIconSettings;

	private static Transform iconPoolParent;

	private static readonly Stack<WorldspaceIconUi> iconPool = new Stack<WorldspaceIconUi>();

	public static Sprite ObjectiveIcon
	{
		get
		{
			if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<WorldspaceIconManager>.Instance.objectiveIcon;
		}
	}

	public static Sprite BallDispenserIcon
	{
		get
		{
			if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<WorldspaceIconManager>.Instance.ballDispenserIcon;
		}
	}

	public static Sprite BallIcon
	{
		get
		{
			if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<WorldspaceIconManager>.Instance.ballIcon;
		}
	}

	public static Sprite HoleIcon
	{
		get
		{
			if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<WorldspaceIconManager>.Instance.holeIcon;
		}
	}

	public static Sprite HomingWarningIcon
	{
		get
		{
			if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<WorldspaceIconManager>.Instance.homingWarningIcon;
		}
	}

	public static WorldspaceIconUiSettings ObjectiveIconSettings
	{
		get
		{
			if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<WorldspaceIconManager>.Instance.objectiveIconSettings;
		}
	}

	public static WorldspaceIconUiSettings BallDispenserIconSettings
	{
		get
		{
			if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<WorldspaceIconManager>.Instance.ballDispenserIconSettings;
		}
	}

	public static WorldspaceIconUiSettings BallIconSettings
	{
		get
		{
			if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<WorldspaceIconManager>.Instance.ballIconSettings;
		}
	}

	public static WorldspaceIconUiSettings HoleIconSettings
	{
		get
		{
			if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<WorldspaceIconManager>.Instance.holeIconSettings;
		}
	}

	public static WorldspaceIconUiSettings HomingWarningIconSettings
	{
		get
		{
			if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<WorldspaceIconManager>.Instance.homingWarningIconSettings;
		}
	}

	public static WorldspaceIconUi GetUnusedIcon()
	{
		if (!SingletonBehaviour<WorldspaceIconManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<WorldspaceIconManager>.Instance.GetUnusedIconInternal();
	}

	public static void ReturnIcon(WorldspaceIconUi icon)
	{
		if (SingletonBehaviour<WorldspaceIconManager>.HasInstance)
		{
			SingletonBehaviour<WorldspaceIconManager>.Instance.ReturnIconInternal(icon);
		}
	}

	private WorldspaceIconUi GetUnusedIconInternal()
	{
		EnsurePoolParentExists();
		WorldspaceIconUi result = null;
		while (result == null)
		{
			if (!iconPool.TryPop(out result))
			{
				result = Object.Instantiate(worldspaceIconPrefab);
			}
		}
		result.gameObject.SetActive(value: true);
		result.transform.SetParent(base.transform);
		result.transform.localScale = Vector3.one;
		return result;
	}

	private void ReturnIconInternal(WorldspaceIconUi icon)
	{
		if (iconPool.Count >= maxPoolSize)
		{
			Object.Destroy(icon.gameObject);
			return;
		}
		icon.gameObject.SetActive(value: false);
		icon.transform.SetParent(iconPoolParent);
		iconPool.Push(icon);
	}

	private void EnsurePoolParentExists()
	{
		if (!(iconPoolParent != null))
		{
			GameObject obj = new GameObject("Worldspace icon pool");
			Object.DontDestroyOnLoad(obj);
			iconPoolParent = obj.transform;
		}
	}
}
