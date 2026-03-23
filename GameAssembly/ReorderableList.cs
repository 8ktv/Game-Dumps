using System;
using System.Collections;
using UnityEngine;

public class ReorderableList : MonoBehaviour
{
	public Transform contentRoot;

	public RectTransform dummy;

	private RectTransform rectTransform;

	private int targetIndex;

	private Coroutine animateRoutine;

	public static ReorderableList Current { get; private set; }

	public event Action<ReorderableListElement> OnElementMoved;

	private void Awake()
	{
		dummy.gameObject.SetActive(value: false);
		rectTransform = contentRoot.GetComponent<RectTransform>();
	}

	private void Update()
	{
		RefreshList();
	}

	public void RefreshList()
	{
		if (ReorderableListElement.Current == null)
		{
			Deactivate();
			return;
		}
		ReorderableListElement current = ReorderableListElement.Current;
		Vector3 position = current.transform.position;
		if (!RectTransformUtility.RectangleContainsScreenPoint(this.rectTransform, position))
		{
			Deactivate();
			return;
		}
		if (!dummy.gameObject.activeSelf)
		{
			ActivateDummy(current);
			Current = this;
			animateRoutine = null;
		}
		for (int i = 0; i < contentRoot.childCount; i++)
		{
			RectTransform rectTransform = contentRoot.GetChild(i) as RectTransform;
			if (!(rectTransform == null) && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, position))
			{
				targetIndex = i;
				dummy.SetSiblingIndex(i);
				break;
			}
		}
		void Deactivate()
		{
			if (dummy.gameObject.activeSelf && animateRoutine == null)
			{
				dummy.gameObject.SetActive(value: false);
				dummy.transform.SetAsLastSibling();
			}
			if (Current == this)
			{
				Current = null;
			}
		}
	}

	public void ResetDummy()
	{
		dummy.transform.SetAsLastSibling();
	}

	private void ActivateDummy(ReorderableListElement element)
	{
		dummy.sizeDelta = element.GetComponent<RectTransform>().sizeDelta;
		dummy.gameObject.SetActive(value: true);
	}

	public void AssignElement(ReorderableListElement element, int indexOverride = -1)
	{
		if (indexOverride >= 0)
		{
			targetIndex = indexOverride;
		}
		animateRoutine = StartCoroutine(AnimateRoutine(element));
	}

	private IEnumerator AnimateRoutine(ReorderableListElement element)
	{
		ActivateDummy(element);
		dummy.SetSiblingIndex(targetIndex);
		element.enabled = false;
		Vector3 velocity = Vector3.zero;
		for (float timer = 0.1f; timer > 0f; timer -= Time.deltaTime)
		{
			yield return null;
			element.transform.position = Vector3.SmoothDamp(element.transform.position, dummy.transform.position, ref velocity, 0.1f);
		}
		element.transform.SetParent(contentRoot);
		element.transform.SetSiblingIndex(targetIndex);
		element.enabled = true;
		dummy.gameObject.SetActive(value: false);
		dummy.SetAsLastSibling();
		yield return null;
		element.InformAssigned();
		this.OnElementMoved?.Invoke(element);
	}
}
