using UnityEngine.UIElements;

namespace UnityEngine.EventSystems;

public struct RaycastResult
{
	private GameObject m_GameObject;

	public BaseRaycaster module;

	public float distance;

	public float index;

	public int depth;

	public int sortingGroupID;

	public int sortingGroupOrder;

	public int sortingLayer;

	public int sortingOrder;

	public Vector3 origin;

	public Vector3 worldPosition;

	public Vector3 worldNormal;

	public Vector2 screenPosition;

	public int displayIndex;

	public UIDocument document;

	public VisualElement element;

	public GameObject gameObject
	{
		get
		{
			return m_GameObject;
		}
		set
		{
			m_GameObject = value;
		}
	}

	public bool isValid
	{
		get
		{
			if (module != null)
			{
				return gameObject != null;
			}
			return false;
		}
	}

	public void Clear()
	{
		gameObject = null;
		module = null;
		distance = 0f;
		index = 0f;
		depth = 0;
		sortingLayer = 0;
		sortingOrder = 0;
		origin = Vector3.zero;
		worldNormal = Vector3.up;
		worldPosition = Vector3.zero;
		screenPosition = Vector3.zero;
		displayIndex = 0;
		document = null;
		element = null;
	}

	public override string ToString()
	{
		if (!isValid)
		{
			return "";
		}
		return "Name: " + gameObject?.ToString() + "\nmodule: " + module?.ToString() + "\ndistance: " + distance + "\nindex: " + index + "\ndepth: " + depth + "\nworldNormal: " + worldNormal.ToString() + "\nworldPosition: " + worldPosition.ToString() + "\nscreenPosition: " + screenPosition.ToString() + "\nmodule.sortOrderPriority: " + module.sortOrderPriority + "\nmodule.renderOrderPriority: " + module.renderOrderPriority + "\nsortingLayer: " + sortingLayer + "\nsortingOrder: " + sortingOrder;
	}
}
