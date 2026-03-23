using System.Collections;
using UnityEngine;

public class TargetReticleUi : SingletonBehaviour<TargetReticleUi>
{
	[SerializeField]
	private UiVisibilityController reticleVisibilityController;

	private Vector3 lastInteractableReticleWorldPosition;

	private bool isVisible;

	private Coroutine visibilityCoroutine;

	private ButtonPrompt interactButtonPrompt;

	private IInteractable lastInteractable;

	protected override void Awake()
	{
		base.Awake();
		reticleVisibilityController.SetDesiredAlpha(0f);
	}

	private void LateUpdate()
	{
		bool flag = isVisible;
		isVisible = ShouldBeVisible();
		if (isVisible != flag)
		{
			if (visibilityCoroutine != null)
			{
				StopCoroutine(visibilityCoroutine);
			}
			visibilityCoroutine = StartCoroutine(AnimateVisibilityRoutine(isVisible ? 1f : 0f, 0.15f));
		}
		IInteractable interactable = (isVisible ? GameManager.LocalPlayerInteractableTargeter.FirstTargetInteracable : null);
		if (isVisible && interactable != null && lastInteractable != interactable)
		{
			if (interactButtonPrompt != null)
			{
				ButtonPromptManager.ReturnButtonPrompt(interactButtonPrompt);
			}
			interactButtonPrompt = ButtonPromptManager.GetButtonPrompt(PlayerInput.Controls.Gameplay.Interact, interactable.InteractString, ButtonPromptManager.Type.WorldSpace);
		}
		if (isVisible)
		{
			lastInteractableReticleWorldPosition = GameManager.LocalPlayerInteractableTargeter.CurrentTargetReticlePosition;
		}
		Vector3 position = CameraModuleController.WorldToScreenPoint(lastInteractableReticleWorldPosition);
		reticleVisibilityController.transform.position = position;
		if (interactButtonPrompt != null)
		{
			interactButtonPrompt.transform.position = position;
		}
		lastInteractable = interactable;
		static bool ShouldBeVisible()
		{
			if (GameManager.LocalPlayerInteractableTargeter == null)
			{
				return false;
			}
			if (!GameManager.LocalPlayerInteractableTargeter.HasTarget)
			{
				return false;
			}
			return true;
		}
	}

	private IEnumerator AnimateVisibilityRoutine(float targetAlpha, float duration)
	{
		float initialAlpha = reticleVisibilityController.DesiredAlpha;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float t = time / duration;
			float num = BMath.Lerp(initialAlpha, targetAlpha, BMath.EaseInOut(t));
			reticleVisibilityController.SetDesiredAlpha(num);
			if (interactButtonPrompt != null)
			{
				interactButtonPrompt.canvasGroup.alpha = num;
			}
			yield return null;
		}
		reticleVisibilityController.SetDesiredAlpha(targetAlpha);
		if (targetAlpha < float.Epsilon && interactButtonPrompt != null)
		{
			ButtonPromptManager.ReturnButtonPrompt(interactButtonPrompt);
			interactButtonPrompt = null;
		}
	}
}
