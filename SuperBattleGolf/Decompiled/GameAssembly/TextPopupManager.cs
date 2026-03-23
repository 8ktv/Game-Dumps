using System.Collections.Generic;
using UnityEngine;

public class TextPopupManager : SingletonBehaviour<TextPopupManager>
{
	[SerializeField]
	private TextPopupUi popupPrefab;

	[SerializeField]
	private int maxPoolSize;

	[SerializeField]
	private TextPopupUiSettings defaultPopupSettings;

	[SerializeField]
	private TextPopupUiSettings penaltyPopupSettings;

	private static Transform popupPoolParent;

	private static readonly Stack<TextPopupUi> popupPool = new Stack<TextPopupUi>();

	public static TextPopupUiSettings DefaultPopupSettings
	{
		get
		{
			if (!SingletonBehaviour<TextPopupManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<TextPopupManager>.Instance.defaultPopupSettings;
		}
	}

	public static TextPopupUiSettings PenaltyPopupSettings
	{
		get
		{
			if (!SingletonBehaviour<TextPopupManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<TextPopupManager>.Instance.penaltyPopupSettings;
		}
	}

	public static TextPopupUi GetUnusedPopup()
	{
		if (!SingletonBehaviour<TextPopupManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<TextPopupManager>.Instance.GetUnusedPopupInternal();
	}

	public static void ReturnPopup(TextPopupUi popup)
	{
		if (SingletonBehaviour<TextPopupManager>.HasInstance)
		{
			SingletonBehaviour<TextPopupManager>.Instance.ReturnPopupInternal(popup);
		}
	}

	private TextPopupUi GetUnusedPopupInternal()
	{
		EnsurePoolParentExists();
		TextPopupUi result = null;
		while (result == null)
		{
			if (!popupPool.TryPop(out result))
			{
				result = Object.Instantiate(popupPrefab);
			}
		}
		result.gameObject.SetActive(value: true);
		result.transform.SetParent(base.transform);
		result.transform.localScale = Vector3.one;
		return result;
	}

	private void ReturnPopupInternal(TextPopupUi popup)
	{
		if (popupPool.Count >= maxPoolSize)
		{
			Object.Destroy(popup.gameObject);
			return;
		}
		popup.gameObject.SetActive(value: false);
		popup.transform.SetParent(popupPoolParent);
		popupPool.Push(popup);
	}

	private void EnsurePoolParentExists()
	{
		if (!(popupPoolParent != null))
		{
			GameObject obj = new GameObject("Number popup pool");
			Object.DontDestroyOnLoad(obj);
			popupPoolParent = obj.transform;
		}
	}
}
