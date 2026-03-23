using System.Collections.Generic;
using UnityEngine;

public class NameTagManager : SingletonBehaviour<NameTagManager>
{
	[SerializeField]
	private NameTagUi nameTagPrefab;

	[SerializeField]
	private int maxPoolSize;

	[SerializeField]
	private NameTagUiSettings playerNameTagSettings;

	[SerializeField]
	private NameTagUiSettings ballNameTagSettings;

	[SerializeField]
	private NameTagUiSettings spectatorNameTagSettings;

	private static Transform nameTagPoolParent;

	private static readonly Stack<NameTagUi> nameTagPool = new Stack<NameTagUi>();

	public static NameTagUiSettings PlayerNameTagSettings
	{
		get
		{
			if (!SingletonBehaviour<NameTagManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<NameTagManager>.Instance.playerNameTagSettings;
		}
	}

	public static NameTagUiSettings BallNameTagSettings
	{
		get
		{
			if (!SingletonBehaviour<NameTagManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<NameTagManager>.Instance.ballNameTagSettings;
		}
	}

	public static NameTagUiSettings SpectatorNameTagSettings
	{
		get
		{
			if (!SingletonBehaviour<NameTagManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<NameTagManager>.Instance.spectatorNameTagSettings;
		}
	}

	public static NameTagUi GetUnusedNameTag()
	{
		if (!SingletonBehaviour<NameTagManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<NameTagManager>.Instance.GetUnusedNameTagInternal();
	}

	public static void ReturnNameTag(NameTagUi nameTag)
	{
		if (SingletonBehaviour<NameTagManager>.HasInstance)
		{
			SingletonBehaviour<NameTagManager>.Instance.ReturnNameTagInternal(nameTag);
		}
	}

	private NameTagUi GetUnusedNameTagInternal()
	{
		EnsurePoolParentExists();
		NameTagUi result = null;
		while (result == null)
		{
			if (!nameTagPool.TryPop(out result))
			{
				result = Object.Instantiate(nameTagPrefab);
			}
		}
		result.gameObject.SetActive(value: true);
		result.transform.SetParent(base.transform);
		result.transform.localScale = Vector3.one;
		return result;
	}

	private void ReturnNameTagInternal(NameTagUi nameTag)
	{
		if (nameTagPool.Count >= maxPoolSize)
		{
			Object.Destroy(nameTag.gameObject);
			return;
		}
		nameTag.gameObject.SetActive(value: false);
		nameTag.transform.SetParent(nameTagPoolParent);
		nameTagPool.Push(nameTag);
	}

	private void EnsurePoolParentExists()
	{
		if (!(nameTagPoolParent != null))
		{
			GameObject obj = new GameObject("Name tag pool");
			Object.DontDestroyOnLoad(obj);
			nameTagPoolParent = obj.transform;
		}
	}
}
