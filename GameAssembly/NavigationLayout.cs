using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NavigationLayout : MonoBehaviour
{
	public enum Direction
	{
		Horizontal,
		Vertical
	}

	public Direction direction;

	public bool ignoreOtherDirection;

	private Selectable[] selectables;

	private void Awake()
	{
		selectables = (from x in GetComponentsInChildren<Selectable>()
			where x.navigation.mode != Navigation.Mode.None
			select x).ToArray();
	}

	private void Update()
	{
		Selectable[] array = selectables.Where((Selectable x) => IsSelectableValid(x)).ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			Navigation navigation = array[num].navigation;
			int a = ((num == 0) ? (array.Length - 1) : (num - 1));
			int b = ((num != array.Length - 1) ? (num + 1) : 0);
			a = BMath.Min(a, array.Length - 1);
			b = BMath.Max(0, b);
			if (direction == Direction.Horizontal)
			{
				navigation.selectOnLeft = array[a];
				navigation.selectOnRight = array[b];
				if (!ignoreOtherDirection)
				{
					if (navigation.selectOnUp != null && !IsSelectableValid(navigation.selectOnUp))
					{
						navigation.selectOnUp = null;
					}
					if (navigation.selectOnDown != null && !IsSelectableValid(navigation.selectOnDown))
					{
						navigation.selectOnDown = null;
					}
				}
			}
			if (direction == Direction.Vertical)
			{
				navigation.selectOnUp = array[a];
				navigation.selectOnDown = array[b];
				if (!ignoreOtherDirection)
				{
					if (navigation.selectOnLeft != null && !IsSelectableValid(navigation.selectOnLeft))
					{
						navigation.selectOnLeft = null;
					}
					if (navigation.selectOnRight != null && !IsSelectableValid(navigation.selectOnRight))
					{
						navigation.selectOnRight = null;
					}
				}
			}
			navigation.mode = Navigation.Mode.Explicit;
			array[num].navigation = navigation;
		}
	}

	private bool IsSelectableValid(Selectable selectable)
	{
		if (selectable.enabled && selectable.interactable)
		{
			return selectable.gameObject.activeInHierarchy;
		}
		return false;
	}
}
