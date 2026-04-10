using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScrollViewSnap : MonoBehaviour
{
	public float speed;

	public float damping;

	public Image[] pageIndicators;

	private ScrollRect scrollRect;

	private RectTransform rectTransform;

	private float velocity;

	private float moveTarget;

	private bool overrideTargetPosition;

	private float invStep;

	private int steps = 1;

	private void Awake()
	{
		scrollRect = GetComponent<ScrollRect>();
		rectTransform = base.transform as RectTransform;
		SetSteps(steps);
	}

	public void SetSteps(int steps)
	{
		this.steps = steps;
		invStep = 1f / (float)(steps - 1);
		overrideTargetPosition = false;
		velocity = 0f;
		scrollRect.horizontalNormalizedPosition = 0f;
	}

	private void Update()
	{
		if (Mouse.current.leftButton.isPressed && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Mouse.current.position.value))
		{
			overrideTargetPosition = false;
			return;
		}
		float num = CalculateTargetPosition();
		int num2 = BMath.FloorToInt(num * (float)(steps - 1));
		for (int i = 0; i < pageIndicators.Length; i++)
		{
			Color color = pageIndicators[i].color;
			color.a = ((num2 == i) ? 1f : 0.25f);
			pageIndicators[i].color = color;
			pageIndicators[i].gameObject.SetActive(i / 2 < steps - 1);
		}
		if (overrideTargetPosition && BMath.Abs(num - moveTarget) < 0.01f)
		{
			overrideTargetPosition = false;
		}
		float b = BMath.Clamp01(overrideTargetPosition ? moveTarget : num) - scrollRect.horizontalNormalizedPosition;
		velocity += BMath.Min(invStep, b) * Time.deltaTime;
		velocity *= 1f - damping * Time.deltaTime;
		scrollRect.horizontalNormalizedPosition += velocity;
		if ((!base.gameObject.TryGetComponentInParent<MenuNavigation>(out var foundComponent, includeInactive: false) || foundComponent.IsAtTopOfStack()) && Gamepad.current != null)
		{
			if (Gamepad.current.leftShoulder.wasPressedThisFrame)
			{
				MoveLeft();
			}
			if (Gamepad.current.rightShoulder.wasPressedThisFrame)
			{
				MoveRight();
			}
		}
	}

	[ContextMenu("Move Left")]
	public void MoveLeft()
	{
		moveTarget = BMath.Clamp01(CalculateTargetPosition() - invStep);
		velocity = 0f;
		overrideTargetPosition = true;
	}

	[ContextMenu("Move Right")]
	public void MoveRight()
	{
		moveTarget = BMath.Clamp01(CalculateTargetPosition() + invStep);
		velocity = 0f;
		overrideTargetPosition = true;
	}

	private float CalculateTargetPosition()
	{
		return BMath.Round(scrollRect.horizontalNormalizedPosition / invStep) * invStep;
	}
}
