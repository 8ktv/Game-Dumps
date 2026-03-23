using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class SingleAxisNavigationGroup : MonoBehaviour
{
	public enum Axis
	{
		Vertical,
		Horizontal
	}

	public Axis axis;

	public bool wrapAround;

	private MenuNavigation parentMenuNavigation;

	private void Awake()
	{
		parentMenuNavigation = GetComponentInParent<MenuNavigation>(includeInactive: true);
	}

	private void OnEnable()
	{
		UpdateNavigation();
	}

	private void OnTransformChildrenChanged()
	{
		UpdateNavigation();
	}

	private void OnValidate()
	{
		UpdateNavigation();
	}

	public void UpdateNavigation()
	{
		List<Selectable> value;
		using (CollectionPool<List<Selectable>, Selectable>.Get(out value))
		{
			GetComponentsInChildren(includeInactive: false, value);
			if (parentMenuNavigation != null)
			{
				List<Selectable> value2;
				using (CollectionPool<List<Selectable>, Selectable>.Get(out value2))
				{
					foreach (Selectable item in value)
					{
						if (item.TryGetComponentInParent<MenuNavigation>(out var foundComponent, includeInactive: true) && foundComponent == parentMenuNavigation)
						{
							value2.Add(item);
						}
					}
					value = value2;
				}
			}
			for (int i = 0; i < value.Count; i++)
			{
				Selectable selectable = value[i];
				Navigation navigation = selectable.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				if (axis == Axis.Vertical)
				{
					object selectOnUp;
					if (i <= 0)
					{
						if (!wrapAround)
						{
							selectOnUp = null;
						}
						else
						{
							List<Selectable> list = value;
							selectOnUp = list[list.Count - 1];
						}
					}
					else
					{
						selectOnUp = value[i - 1];
					}
					navigation.selectOnUp = (Selectable)selectOnUp;
					navigation.selectOnDown = ((i < value.Count - 1) ? value[i + 1] : (wrapAround ? value[0] : null));
				}
				else
				{
					object selectOnLeft;
					if (i <= 0)
					{
						if (!wrapAround)
						{
							selectOnLeft = null;
						}
						else
						{
							List<Selectable> list2 = value;
							selectOnLeft = list2[list2.Count - 1];
						}
					}
					else
					{
						selectOnLeft = value[i - 1];
					}
					navigation.selectOnLeft = (Selectable)selectOnLeft;
					navigation.selectOnRight = ((i < value.Count - 1) ? value[i + 1] : (wrapAround ? value[0] : null));
				}
				selectable.navigation = navigation;
			}
		}
	}
}
