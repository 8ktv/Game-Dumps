using TMPro;
using UnityEngine;

public class DropdownSelectableCancel : MonoBehaviour
{
	public TMP_Dropdown dropdown;

	private void Awake()
	{
		GetComponent<MenuNavigation>().OnExitEvent += dropdown.Hide;
	}
}
