using System.Collections;
using TMPro;
using UnityEngine;

public class CurrencyDisplay : MonoBehaviour
{
	public TMP_Text currencyCounter;

	public TMP_Text currencyAdd;

	public UiVisibilityController uiVisibilityController;

	public Animator animator;

	public float timeout;

	public float fadeTime;

	public bool autoHide;

	private int addStack;

	private float lastUpdate;

	private Coroutine timeoutRoutine;

	private void OnEnable()
	{
		int credits = CosmeticsUnlocksManager.GetCredits();
		OnCurrencyUpdate(credits, credits);
		CosmeticsUnlocksManager.OnCreditsUpdate += OnCurrencyUpdate;
		currencyAdd.gameObject.SetActive(value: false);
		currencyCounter.text = CosmeticsUnlocksManager.GetCredits().ToString();
		addStack = 0;
		if (autoHide)
		{
			uiVisibilityController.SetDesiredAlpha(0f);
			animator.enabled = false;
		}
	}

	private void OnDisable()
	{
		CosmeticsUnlocksManager.OnCreditsUpdate -= OnCurrencyUpdate;
	}

	private void OnCurrencyUpdate(int prevCredits, int currCredits)
	{
		if (prevCredits == currCredits)
		{
			return;
		}
		if (autoHide)
		{
			animator.enabled = true;
			uiVisibilityController.AnimatedDesiredAlpha(1f, fadeTime, (float x) => x);
			if (timeoutRoutine != null)
			{
				StopCoroutine(timeoutRoutine);
			}
			timeoutRoutine = StartCoroutine(Timeout());
		}
		if (Time.time - lastUpdate > timeout)
		{
			currencyCounter.text = prevCredits.ToString();
			addStack = 0;
		}
		addStack += currCredits - prevCredits;
		currencyAdd.text = ((addStack < 0) ? addStack.ToString() : $"+{addStack}");
		animator.SetTrigger((prevCredits < currCredits) ? "Add" : "Remove");
		lastUpdate = Time.time;
	}

	private IEnumerator Timeout()
	{
		yield return new WaitForSeconds(timeout);
		uiVisibilityController.AnimatedDesiredAlpha(0f, fadeTime, (float x) => x);
		animator.enabled = false;
	}

	public void OnAnimationCreditsUpdate()
	{
		currencyCounter.text = CosmeticsUnlocksManager.GetCredits().ToString();
	}
}
