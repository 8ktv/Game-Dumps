using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpeedBoostWheelUi : MonoBehaviour, ILateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private Image background;

	[SerializeField]
	private Image staminaWheel;

	[SerializeField]
	private UiVisibilityController visibilityController;

	private float displayedNormalizedSpeedBoost;

	private bool isVisible;

	private Coroutine visibilityCoroutine;

	private void Awake()
	{
		BUpdate.RegisterCallback(this);
		visibilityController.SetDesiredAlpha(0f);
	}

	private void OnDestroy()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnLateBUpdate()
	{
		bool flag = isVisible;
		isVisible = ShouldBeVisible();
		if (isVisible)
		{
			UpdateSpeedBoost();
		}
		if (isVisible != flag)
		{
			if (visibilityCoroutine != null)
			{
				StopCoroutine(visibilityCoroutine);
			}
			visibilityCoroutine = StartCoroutine(AnimateVisibilityRoutine(isVisible ? 1f : 0f, 0.15f));
		}
		static bool ShouldBeVisible()
		{
			if (GameManager.LocalPlayerMovement == null)
			{
				return false;
			}
			if (!GameManager.LocalPlayerMovement.IsVisible)
			{
				return false;
			}
			if (GameManager.LocalPlayerAsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (GameManager.LocalPlayerAsGolfer.IsAimingSwing)
			{
				return false;
			}
			if (!GameManager.LocalPlayerMovement.StatusEffects.HasEffect(StatusEffect.SpeedBoost))
			{
				return false;
			}
			return true;
		}
		void UpdateSpeedBoost()
		{
			float num = displayedNormalizedSpeedBoost;
			displayedNormalizedSpeedBoost = BMath.Clamp01(GameManager.LocalPlayerMovement.SpeedBoostRemainingTime / GameManager.LocalPlayerMovement.Settings.MaxSpeedBoostDuration);
			if (displayedNormalizedSpeedBoost != num)
			{
				staminaWheel.fillAmount = displayedNormalizedSpeedBoost;
			}
		}
	}

	private IEnumerator AnimateVisibilityRoutine(float targetAlpha, float duration)
	{
		float initialAlpha = visibilityController.DesiredAlpha;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float t = time / duration;
			visibilityController.SetDesiredAlpha(BMath.Lerp(initialAlpha, targetAlpha, BMath.EaseInOut(t)));
			yield return null;
		}
		visibilityController.SetDesiredAlpha(targetAlpha);
	}
}
