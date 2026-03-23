using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuTabs : MonoBehaviour
{
	public Button[] tabButtons;

	public GameObject[] tabs;

	public Color textSelectionColor;

	public int defaultSelection;

	public AnimationCurve selectAnimation;

	public float selectAnimationDuration = 0.5f;

	private Color[] textColors;

	private int selectedTab;

	private void Awake()
	{
		textColors = new Color[tabButtons.Length];
		for (int i = 0; i < tabButtons.Length; i++)
		{
			int buttonIndex = i;
			Button obj = tabButtons[i];
			obj.onClick.AddListener(delegate
			{
				SelectTab(buttonIndex);
			});
			TMP_Text component = obj.transform.GetChild(1).GetComponent<TMP_Text>();
			textColors[i] = component.color;
		}
		SelectTabInternal(defaultSelection, animate: false);
	}

	private void Update()
	{
		if (InputManager.CurrentGamepad != null && !InputManager.CurrentModeMask.HasMode(InputMode.ForceDisabled))
		{
			if (InputManager.CurrentGamepad.rightShoulder.wasPressedThisFrame)
			{
				CycleTabRight();
			}
			if (InputManager.CurrentGamepad.leftShoulder.wasPressedThisFrame)
			{
				CycleTabLeft();
			}
		}
	}

	private void CycleTabLeft()
	{
		SelectTabInternal(WrapIndex(selectedTab - 1), animate: true, forceSfx: true);
	}

	private void CycleTabRight()
	{
		SelectTabInternal(WrapIndex(selectedTab + 1), animate: true, forceSfx: true);
	}

	private int WrapIndex(int tab)
	{
		if (tab < 0)
		{
			tab = tabs.Length - 1;
		}
		else if (tab >= tabs.Length)
		{
			tab = 0;
		}
		return tab;
	}

	public void SelectTab(int tabIndex)
	{
		SelectTabInternal(tabIndex);
	}

	private void SelectTabInternal(int tabIndex, bool animate = true, bool forceSfx = false)
	{
		StopAllCoroutines();
		for (int i = 0; i < tabButtons.Length; i++)
		{
			bool flag = i == tabIndex;
			tabs[i].SetActive(flag);
			Button button = tabButtons[i];
			button.transform.GetChild(0).gameObject.SetActive(flag);
			button.transform.GetChild(1).GetComponent<TMP_Text>().color = (flag ? textSelectionColor : textColors[i]);
			Quaternion quaternion = (flag ? Quaternion.Euler(0f, 0f, 2f) : Quaternion.Euler(0f, 0f, -2f));
			if (animate)
			{
				StartCoroutine(AnimateButton(button, quaternion));
			}
			else
			{
				button.transform.localRotation = quaternion;
			}
		}
		selectedTab = tabIndex;
		if (tabs[selectedTab].TryGetComponent<MenuNavigation>(out var component))
		{
			component.Reselect();
		}
		if (forceSfx && tabButtons[selectedTab].TryGetComponent<UiSfx>(out var component2))
		{
			component2.PlaySelectSfx(InputManager.UsingGamepad);
		}
	}

	private IEnumerator AnimateButton(Button button, Quaternion targetRotation)
	{
		float time = 0f;
		Quaternion startRotation = button.transform.localRotation;
		while (time < selectAnimationDuration)
		{
			yield return null;
			time += Time.unscaledDeltaTime;
			button.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, selectAnimation.Evaluate(time / selectAnimationDuration));
		}
	}
}
